using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Utility;

namespace GreinerHormannAlgorithm
{
    public class PolyClipper
    {
        public static List<Polygon2> PolygonClipper(Polygon2 poly, Polygon2 window, BooleanOperation operation)
        {
            //step 0:
            //polygons should be clockwise
            //lets check if they are clockwise, if they are not, then we form new polygons with clockwise input
            Polygon2 polyClockwise = GetClockwisePoly(poly);
            Polygon2 windowClockwise = GetClockwisePoly(window);

            // step 1:
            // form new polygons with intersection connections
            Polygon2 polyWithIntersectionVertices = FormPolyWithNewIntersectionVertices(poly, window);
            Polygon2 windowWithIntersectionVertices = FormPolyWithNewIntersectionVertices(window, poly);
            
            //step 2:
            // if no intersections, then check if one poly is inside or outside the other
            // if no intersections founded then if any point of poly is inside the other
            // then that poly is inside the other and union operation is simply the biggest poly of those 2
            if (polyClockwise.Count == polyWithIntersectionVertices.Count || windowClockwise.Count == windowWithIntersectionVertices.Count)
            {
                if(operation == BooleanOperation.Union)
                    return GetTheBiggestPolyIfOneInsideTheOtherOrEmpty(polyClockwise, windowClockwise);

                if (operation == BooleanOperation.Intersection)
                    return new List<Polygon2>();
            }

            //all tested till here...
            //fix intersection points on line, where is duplicates on edge to edge scenario
            
            //step 3:
            //get intersection polygons
            GetIntersectionPolygons(polyWithIntersectionVertices, windowWithIntersectionVertices);
            
            return new List<Polygon2>(){polyWithIntersectionVertices};
        }

        private static void GetIntersectionPolygons(Polygon2 poly, Polygon2 window)
        {
            //step 1:
            //get any point outside of window
            MyVector2 pointOutside = poly.Points.First(p => !window.Contains(p));
            bool isEntering = false;
            
            //step 2:
            //find intersecting point
            
        }

        private static List<Polygon2> GetTheBiggestPolyIfOneInsideTheOtherOrEmpty(Polygon2 poly, Polygon2 window)
        {
            if(poly.Points.Any(p => window.Contains(p)))
                return new List<Polygon2> {window};

            if(window.Points.Any(p => poly.Contains(p)))
                return new List<Polygon2> {poly};

            return new List<Polygon2>();
        }

        private static Polygon2 GetClockwisePoly(Polygon2 poly)
        {
            if (poly.IsClockwise)
                return poly;

            List<MyVector2> clockwisePolyPoints = poly.Points.ToList();
            clockwisePolyPoints.Reverse();
            return new Polygon2(clockwisePolyPoints);
        }

        public static Polygon2 FormPolyWithNewIntersectionVertices(Polygon2 poly, Polygon2 intersectionPoly)
        {
            var newPolyPoints = new List<MyVector2>();
            foreach (Edge2 edge1 in poly.Edges)
            {
                //add first point
                var edgeP1 = edge1.P1;
                newPolyPoints.Add(edgeP1);

                //find all intersections on that edge with other edges in intersectionPoly 
                var intersections = new List<MyVector2>();
                foreach (Edge2 edge2 in intersectionPoly.Edges)
                {
                    if (GeometryUtility.LineLine(edge1, edge2, true))
                    {
                        var intersectionPoint = GeometryUtility.GetLineLineIntersectionPoint(edge1, edge2);
                        intersectionPoint.IsIntersection = true;
                        intersections.Add(intersectionPoint);
                    }
                }
                
                //sort intersections and add to the newPolyList
                foreach (MyVector2 p in intersections.OrderBy(i => MyVector2.SqrMagnitude(edgeP1 - i)))
                {
                    if(newPolyPoints.Any(x => x.Equals(p)))
                        continue;
                    
                    newPolyPoints.Add(p);
                }
            }

            return new Polygon2(newPolyPoints);
        }
    }
}