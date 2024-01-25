using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Unity.VisualScripting;
using UnityEngine;
using Utility;
using GeometryUtility = Utility.GeometryUtility;

namespace GreinerHormannAlgorithm
{
    public static class PolygonClippingAlgorithm
    {
        public static (List<List<Edge2>> list, List<VerticalIntersectingLine> verticalIntersectionLines, List<Triangle2> triangles) 
            PolygonClipper(Polygon2 poly, Polygon2 window, BooleanOperation operation)
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
            var list = CalculatePolygonEdges(verticalIntersectionLines, poly, window);
            var triangles = TriangulateSectors(list);

            var trianglesResultOperation = new List<Triangle2>();
            if (operation == BooleanOperation.Intersection)
            {
                trianglesResultOperation = FilterTriangles(IntersectionFilter(poly, window), triangles);
            }
            else if(operation == BooleanOperation.Union)
            {
                trianglesResultOperation = FilterTriangles(UnionFilter(poly, window), triangles);
            }
            
            return (list, verticalIntersectionLines, trianglesResultOperation);
        }

        private static List<Triangle2> FilterTriangles(Func<Triangle2, bool> filter, List<Triangle2> allTriangles) => 
            allTriangles.Where(filter).ToList();

        private static Func<Triangle2, bool> IntersectionFilter(Polygon2 poly, Polygon2 window) => 
            t => poly.Contains(t.Centroid) && window.Contains(t.Centroid);
        
        private static Func<Triangle2, bool> UnionFilter(Polygon2 poly, Polygon2 window) => 
            t => poly.Contains(t.Centroid) || window.Contains(t.Centroid);

        private static List<Triangle2> TriangulateSectors(List<List<Edge2>> list)
        {
            var triangles = new List<Triangle2>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count - 1; j++)
                {
                    var curr = list[i][j];
                    var next = list[i][j+1];
                    TriangulateEdge(curr, next, ref triangles);
                }
            }
            return triangles;
        }

        private static void TriangulateEdge(Edge2 top, Edge2 bottom, ref List<Triangle2> triangles)
        {
            if (top.P1.Equals(bottom.P1))
            {
                var p1 = top.P1;
                var p2 = top.P2;
                var p3 = bottom.P2;
                var triangle = new Triangle2(p1, p2, p3);
                triangles.Add(triangle);
            }
            else if (top.P2.Equals(bottom.P2))
            {
                var p1 = top.P1;
                var p2 = top.P2;
                var p3 = bottom.P1;
                var triangle = new Triangle2(p1, p2, p3);
                triangles.Add(triangle);
            }
            else
            {
                var p1 = top.P1;
                var p2 = top.P2;
                var p3 = bottom.P1;
                var p4 = bottom.P2;
                var triangle1 = new Triangle2(p1, p2, p4);
                var triangle2 = new Triangle2(p1, p4, p3);
                triangles.Add(triangle1);
                triangles.Add(triangle2);
            }
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

                //Debug.Log($"Line{i}Count: {currPoints.Count} Line{i+1}Count: {nextPoint.Count}");
                int lastNextIntersection = 0;
                for (var j = 0; j < currPoints.Count; j++)
                {
                    var v1 = currPoints[j];
                    for (var k = lastNextIntersection; k < nextPoint.Count; k++)
                    {
                        var v2 = nextPoint[k];
                        var edge = new Edge2(v1, v2);
                        if (IsEdgeLiesOnPolygonPerimeter(poly, edge) || IsEdgeLiesOnPolygonPerimeter(window, edge))
                        {
                            //Debug.Log($"Found laying edge. Lines: {i}-{i + 1}. Points: {j} and {k}");
                            sectorEdges.Add(edge);
                            lastNextIntersection++;
                        }
                    }
                    lastNextIntersection = Math.Clamp(lastNextIntersection - 1, 0, nextPoint.Count - 1);
                }

                // while (currIndex + nextIndex < currPoints.Count + nextPoint.Count - 2)
                // {
                //     var edge = new Edge2(currPoints[currIndex], nextPoint[nextIndex]);
                //     if (IsEdgeLiesOnPolygonPerimeter(poly, edge) || IsEdgeLiesOnPolygonPerimeter(window, edge))
                //     {
                //         Debug.Log($"Found laying edge. Lines: {i}-{i+1}. Points: {currIndex} and {nextIndex}");
                //         sectorEdges.Add(edge);
                //         if(currIndex + nextIndex == currPoints.Count + nextPoint.Count - 2)
                //             break;
                //         
                //         nextIndex = Math.Clamp(nextIndex + 1, 0, nextPoint.Count - 1);
                //     }
                //     else
                //     {
                //         Debug.Log($"Lines: {i}-{i+1}. Points: {currIndex} and {nextIndex} not laying on any edge");
                //         currIndex = Math.Clamp(currIndex + 1, 0, currPoints.Count - 1);
                //         nextIndex = Math.Clamp(nextIndex - 1, 0, nextPoint.Count - 1);
                //     }
                // }
                
                result.Add(new List<Edge2>(sectorEdges));
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