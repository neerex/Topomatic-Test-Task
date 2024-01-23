using System.Collections.Generic;
using System.Linq;
using DataStructures;
using GeometryUtility = Utility.GeometryUtility;

namespace GreinerHormannAlgorithm
{
    public static class PolygonClippingAlgorithm
    {
        public static HashSet<MyVector2> PolygonClipper(Polygon2 poly, Polygon2 window, BooleanOperation operation)
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
            HashSet<MyVector2> verticesAndIntersectionPoints = new HashSet<MyVector2>();
            foreach (MyVector2 v in window.Points) verticesAndIntersectionPoints.Add(v);
            foreach (MyVector2 v in poly.Points)   verticesAndIntersectionPoints.Add(v);
            
            PopulateUniqueVerticesSetWithIntersections(poly, window, verticesAndIntersectionPoints);
            
            return verticesAndIntersectionPoints;
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
        
        private static void PopulateUniqueVerticesSetWithIntersections(Polygon2 poly, Polygon2 intersectionPoly, HashSet<MyVector2> uniques)
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
                        uniques.Add(intersectionPoint);
                }
            }
        }
    }
}