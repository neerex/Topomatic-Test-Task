using System;
using System.Collections.Generic;
using System.Linq;
using DataComparers;
using DataStructures;
using GeometryUtility = Utility.GeometryUtility;

namespace PolygonClipper
{
    public static class PolygonClippingAlgorithm
    {
        public static List<Triangle2> PolygonClipper(Polygon2 poly, Polygon2 window, BooleanOperation operation)
        {
            List<Triangle2> result = new List<Triangle2>();

            // step 0
            // check if polygon is self intersecting, return empty result if they are
            if (poly.IsSelfIntersecting()) return result;
            if (window.IsSelfIntersecting()) return result;
            
            // check if one polygon is inside the other, so we have no need to calculate intersection
            if (operation == BooleanOperation.Intersection && (poly.ContainsPolygon(window) || window.ContainsPolygon(poly))) return result;
            
            // step 1
            // simplify polygon, remove vertexes that are on the same line for optimization purpose
            poly = SimplifyPolygon(poly);
            window = SimplifyPolygon(window);

            // step 2
            // add all vertices from all polygons and add all intersection vertices
            List<MyVector2> verticesAndIntersectionPoints = new List<MyVector2>();
            foreach (MyVector2 v in window.Points) verticesAndIntersectionPoints.Add(v);
            foreach (MyVector2 v in poly.Points)   verticesAndIntersectionPoints.Add(v);
            PopulateVerticesWithIntersectionPoints(poly, window, verticesAndIntersectionPoints);
            
            // step 3
            // Construct vertical lines that will slice our polygons on their vertex positions and intersection points
            // After slice we get vertical regions between every vertical line. Every region have edges between 2 lines.
            // We form edge-to-edge small polygons that are 4vertex or 3vertex size, and then triangulate them.
            // In the end we have triangle list which we can filter for every possible operation
            // not only union, intersection but also the other ones
            List<VerticalIntersectingLine> verticalIntersectionLines = InitializeVerticalIntersectionLines(verticesAndIntersectionPoints, poly, window);
            List<List<Edge2>> list = CalculateEdgesInSectors(verticalIntersectionLines, poly, window);
            List<Triangle2> triangles = TriangulateSectors(list);
            
            // step 4
            // apply filter for chosen boolean operation
            if (operation == BooleanOperation.Intersection)
            {
                result = FilterTriangles(IntersectionFilter(poly, window), triangles);
            }
            else if(operation == BooleanOperation.Union)
            {
                result = FilterTriangles(UnionFilter(poly, window), triangles);
            }
            
            return result;
        }

        private static Polygon2 SimplifyPolygon(Polygon2 poly)
        {
            List<MyVector2> simplifiedPolyVerts = new List<MyVector2>();
            for (int i = 0; i < poly.Count; i++)
            {
                MyVector2 p0 = poly[i];
                MyVector2 p1 = poly[i+1];
                MyVector2 p2 = poly[i+2];

                Edge2 edge = new Edge2(p0, p2);
                
                if (!GeometryUtility.IsPointOnLine(edge, p1)) 
                    simplifiedPolyVerts.Add(p1);
            }
            return new Polygon2(simplifiedPolyVerts);
        }

        private static void PopulateVerticesWithIntersectionPoints(Polygon2 poly, Polygon2 intersectionPoly, List<MyVector2> uniques)
        {
            foreach (Edge2 edge1 in poly.Edges)
            {
                foreach (Edge2 edge2 in intersectionPoly.Edges)
                {
                    if(edge1.NeverIntersectsWith(edge2))
                        continue;
                    
                    if(edge1.SameEdge(edge2))
                        continue;
                    
                    if(edge1.HasSameVertexWithOtherEdge(edge2))
                        continue;
                    
                    if(GeometryUtility.IsEdgeContainsEdge(edge1, edge2))
                        continue;
                    
                    if(GeometryUtility.IsEdgeTouchingEdge(edge1, edge2, out _))
                        continue;

                    if (GeometryUtility.LineSegmentsIntersect(edge1, edge2, out MyVector2 intersectionPoint))
                    {
                        uniques.Add(intersectionPoint);
                    }
                }
            }
        }

        private static List<VerticalIntersectingLine> InitializeVerticalIntersectionLines(List<MyVector2> verticesAndIntersectionPoints, 
            Polygon2 poly, Polygon2 window)
        {
            List<double> uniqueX = verticesAndIntersectionPoints.Select(v => v.X)
                .Distinct(new DoubleComparer())
                .OrderBy(x => x)
                .ToList();
            
            List<VerticalIntersectingLine> intersectingLines = new();
            double maxY = verticesAndIntersectionPoints.Max(v => v.Y);
            double minY = verticesAndIntersectionPoints.Min(v => v.Y);
            
            foreach (double x in uniqueX)
            {
                VerticalIntersectingLine line = new VerticalIntersectingLine(minY, maxY, x);
                intersectingLines.Add(line);
                line.PopulateIntersectionCollection(poly);
                line.PopulateIntersectionCollection(window);
                line.SortIntersectionFromYMaxToYMin();
            }

            return intersectingLines;
        }

        private static List<List<Edge2>> CalculateEdgesInSectors(List<VerticalIntersectingLine> verticalIntersectionLines, Polygon2 poly, Polygon2 window)
        {
            List<List<Edge2>> result = new List<List<Edge2>>();
            
            for (int i = 0; i < verticalIntersectionLines.Count - 1; i++)
            {
                List<Edge2> sectorEdges = new List<Edge2>();
                
                VerticalIntersectingLine curr = verticalIntersectionLines[i];
                VerticalIntersectingLine next = verticalIntersectionLines[i+1];
                
                IReadOnlyList<MyVector2> currPoints = curr.Intersections;
                IReadOnlyList<MyVector2> nextPoint = next.Intersections;
                
                int lastNextIntersection = 0;
                foreach (var v1 in currPoints)
                {
                    for (int k = lastNextIntersection; k < nextPoint.Count; k++)
                    {
                        MyVector2 v2 = nextPoint[k];
                        Edge2 edge = new Edge2(v1, v2);
                        if (IsEdgeLiesOnPolygonPerimeter(poly, edge) || IsEdgeLiesOnPolygonPerimeter(window, edge))
                        {
                            sectorEdges.Add(edge);
                            lastNextIntersection++;
                        }
                    }
                    lastNextIntersection = Math.Clamp(lastNextIntersection - 1, 0, nextPoint.Count - 1);
                }
                result.Add(sectorEdges);
            }

            return result;
        }

        private static List<Triangle2> TriangulateSectors(List<List<Edge2>> list)
        {
            List<Triangle2> triangles = new List<Triangle2>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count - 1; j++)
                {
                    Edge2 curr = list[i][j];
                    Edge2 next = list[i][j+1];
                    TriangulateEdge(curr, next, ref triangles);
                }
            }
            return triangles;
        }

        private static void TriangulateEdge(Edge2 top, Edge2 bottom, ref List<Triangle2> triangles)
        {
            if (top.P1.Equals(bottom.P1))
            {
                MyVector2 p1 = top.P1;
                MyVector2 p2 = top.P2;
                MyVector2 p3 = bottom.P2;
                Triangle2 triangle = new Triangle2(p1, p2, p3);
                triangles.Add(triangle);
            }
            else if (top.P2.Equals(bottom.P2))
            {
                MyVector2 p1 = top.P1;
                MyVector2 p2 = top.P2;
                MyVector2 p3 = bottom.P1;
                Triangle2 triangle = new Triangle2(p1, p2, p3);
                triangles.Add(triangle);
            }
            else
            {
                MyVector2 p1 = top.P1;
                MyVector2 p2 = top.P2;
                MyVector2 p3 = bottom.P1;
                MyVector2 p4 = bottom.P2;
                Triangle2 triangle1 = new Triangle2(p1, p2, p4);
                Triangle2 triangle2 = new Triangle2(p1, p4, p3);
                triangles.Add(triangle1);
                triangles.Add(triangle2);
            }
        }

        private static List<Triangle2> FilterTriangles(Func<Triangle2, bool> filter, List<Triangle2> allTriangles) => 
            allTriangles.Where(filter).ToList();

        private static Func<Triangle2, bool> IntersectionFilter(Polygon2 poly, Polygon2 window) => 
            t => poly.Contains(t.Centroid) && window.Contains(t.Centroid);

        private static Func<Triangle2, bool> UnionFilter(Polygon2 poly, Polygon2 window) => 
            t => poly.Contains(t.Centroid) || window.Contains(t.Centroid);

        private static bool IsEdgeLiesOnPolygonPerimeter(Polygon2 poly, Edge2 edge) => 
            poly.Edges.Any(e => GeometryUtility.IsEdgeContainsEdge(e, edge));

        private static Polygon2 GetClockwisePoly(Polygon2 poly)
        {
            if (poly.IsClockwise)
                return poly;

            List<MyVector2> clockwisePolyPoints = poly.Points.ToList();
            clockwisePolyPoints.Reverse();
            return new Polygon2(clockwisePolyPoints);
        }
    }
}