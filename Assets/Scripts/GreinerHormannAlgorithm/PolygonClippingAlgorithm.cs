using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Unity.VisualScripting;
using Utility;
using GeometryUtility = Utility.GeometryUtility;

namespace GreinerHormannAlgorithm
{
    public static class PolygonClippingAlgorithm
    {
        public static List<VerticalIntersectingLine> PolygonClipper(Polygon2 poly, Polygon2 window, BooleanOperation operation)
        {
            //check if polygon is self intersecting then return empty result
            if (poly.IsSelfIntersecting()) return default;
            if (window.IsSelfIntersecting()) return default;
            
            //simplify polygon, remove vertexes that are on the same line
            poly = SimplifyPolygon(poly);
            window = SimplifyPolygon(window);

            //step 0:
            //polygons should be clockwise
            //lets check if they are clockwise, if they are not, then we form new polygons with clockwise input
            //Polygon2 polyClockwise = GetClockwisePoly(poly);
            //Polygon2 windowClockwise = GetClockwisePoly(window);

            //add all vertices from all polygons
            List<MyVector2> verticesAndIntersectionPoints = new List<MyVector2>();
            foreach (MyVector2 v in window.Points) verticesAndIntersectionPoints.Add(v);
            foreach (MyVector2 v in poly.Points)   verticesAndIntersectionPoints.Add(v);
            
            PopulateUniqueVerticesSetWithIntersections(poly, window, verticesAndIntersectionPoints);

            List<VerticalIntersectingLine> verticalIntersectionLines = InitializeVerticalIntersectionLines(verticesAndIntersectionPoints, poly, window);
            CalculatePolygonEdges(verticalIntersectionLines, poly, window);
            return verticalIntersectionLines;
        }

        private static List<VerticalIntersectingLine> InitializeVerticalIntersectionLines(List<MyVector2> verticesAndIntersectionPoints, 
            Polygon2 poly, Polygon2 window)
        {
            List<float> uniqueX = verticesAndIntersectionPoints.Select(v => v.X)
                .Distinct(new FloatComparer())
                .OrderBy(x => x)
                .ToList();
            
            List<VerticalIntersectingLine> intersectingLines = new();
            float maxY = verticesAndIntersectionPoints.Max(v => v.Y);
            float minY = verticesAndIntersectionPoints.Min(v => v.Y);
            
            foreach (float x in uniqueX)
            {
                var line = new VerticalIntersectingLine(minY, maxY, x);
                intersectingLines.Add(line);
                line.PopulateIntersectionCollection(poly);
                line.PopulateIntersectionCollection(window);
                line.SortIntersectionFromYMaxToYMin();
            }

            return intersectingLines;
        }

        private static List<List<Edge2>> CalculatePolygonEdges(List<VerticalIntersectingLine> verticalIntersectionLines, Polygon2 poly, Polygon2 window)
        {
            List<List<Edge2>> result = new List<List<Edge2>>();
            
            for (int i = 0; i < verticalIntersectionLines.Count - 1; i++)
            {
                var sectorEdges = new List<Edge2>();
                
                var curr = verticalIntersectionLines[i];
                var next = verticalIntersectionLines[i+1];
                
                var currPoints = curr.SortedUniqueIntersections;
                var nextPoint = next.SortedUniqueIntersections;
                
                int currIndex = 0;
                int nextIndex = 0;
                
                while (currIndex < currPoints.Count - 1 && nextIndex < nextPoint.Count - 1)
                {
                    if (nextIndex == nextPoint.Count)
                    {
                        currIndex++;
                        nextIndex = Math.Clamp(nextIndex - 1, 0, nextPoint.Count - 1);
                    }
                        
                    var edge = new Edge2(currPoints[currIndex], nextPoint[nextIndex]);
                    if (IsEdgeLiesOnPolygonPerimeter(poly, edge) || IsEdgeLiesOnPolygonPerimeter(window, edge))
                    {
                        sectorEdges.Add(edge);
                        nextIndex++;
                    }
                    else
                    {
                        currIndex++;
                        nextIndex--;
                    }
                }
                result.Add(sectorEdges);
            }

            return result;
        }

        public static bool IsEdgeLiesOnPolygonPerimeter(Polygon2 poly, Edge2 edge)
        {
            return poly.Edges.Any(e => GeometryUtility.IsEdgeContainsEdge(e, edge));
        }

        private static Polygon2 SimplifyPolygon(Polygon2 poly)
        {
            List<MyVector2> simplifiedPolyVerts = new List<MyVector2>();
            for (int i = 0; i < poly.Count; i++)
            {
                var p0 = poly[i];
                var p1 = poly[i+1];
                var p2 = poly[i+2];

                var edge = new Edge2(p0, p2);
                
                if (!GeometryUtility.IsPointOnLine(edge, p1)) 
                    simplifiedPolyVerts.Add(p1);
            }
            return new Polygon2(simplifiedPolyVerts);
        }
        
        private static Polygon2 GetClockwisePoly(Polygon2 poly)
        {
            if (poly.IsClockwise)
                return poly;

            List<MyVector2> clockwisePolyPoints = poly.Points.ToList();
            clockwisePolyPoints.Reverse();
            return new Polygon2(clockwisePolyPoints);
        }
        
        private static void PopulateUniqueVerticesSetWithIntersections(Polygon2 poly, Polygon2 intersectionPoly, List<MyVector2> uniques)
        {
            foreach (Edge2 edge1 in poly.Edges)
            {
                foreach (Edge2 edge2 in intersectionPoly.Edges)
                {
                    if(edge1.NeverIntersectsWith(edge2))
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
    }
    
    public class FloatComparer : IEqualityComparer<float>
    {
        public bool Equals(float x, float y) => x.EqualsWithEpsilon(y);
        public int GetHashCode(float value) => 0;
    }
}