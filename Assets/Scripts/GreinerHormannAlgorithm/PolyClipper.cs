using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Utility;

namespace GreinerHormannAlgorithm
{
    public class PolyClipper
    {
        public static List<List<ClipVertex2>> PolygonClipper(Polygon2 poly, Polygon2 window, BooleanOperation operation)
        {
            //step 0:
            //polygons should be clockwise
            //lets check if they are clockwise, if they are not, then we form new polygons with clockwise input
            Polygon2 polyClockwise = GetClockwisePoly(poly);
            Polygon2 windowClockwise = GetClockwisePoly(window);
            
            
           
            // step 1-1:
            // form new polygons with intersection connections
            List<ClipVertex2> polyWithIntersectionVertices = FormPolyWithNewIntersectionVertices(poly, window);
            List<ClipVertex2> windowWithIntersectionVertices = FormPolyWithNewIntersectionVertices(window, poly);
            
            //step 1-2
            // init needed data structures

            
            //check if any poly has only has same number of intersection as vertices in the original poly
            
            //step 2:
            // if no intersections, then check if one poly is inside or outside the other
            // if no intersections founded then if any point of poly is inside the other
            // then that poly is inside the other and union operation is simply the biggest poly of those 2
            
            // if (polyClockwise.Count == polyWithIntersectionVertices.Count || windowClockwise.Count == windowWithIntersectionVertices.Count)
            // {
            //     if(operation == BooleanOperation.Union)
            //         return GetTheBiggestPolyIfOneInsideTheOtherOrEmpty(polyClockwise, windowClockwise);
            //
            //     if (operation == BooleanOperation.Intersection)
            //         return new List<Polygon2>();
            // }

            //all tested till here...
            //fix intersection points on line, where is duplicates on edge to edge scenario
            
            //step 3:
            //get intersection polygons
            //List<Polygon2> result = new List<Polygon2>();// GetIntersectionPolygons(polyWithIntersectionVertices, windowWithIntersectionVertices);

            return new List<List<ClipVertex2>> {polyWithIntersectionVertices, /*windowWithIntersectionVertices*/};
        }

        // private static List<Polygon2> GetIntersectionPolygons(Polygon2 poly, Polygon2 window)
        // {
        //     List<Polygon2> result = new List<Polygon2>();
        //     
        //     //step 1:
        //     //get any point outside of window
        //     MyVector2 point = poly.Points.First(p => !window.Contains(p));
        //     bool isEntering = false;
        //     
        //     //step 2:
        //     //find union polygons
        //     
        //     //might be bad because need to fix intersection points on line, where is duplicates on edge to edge scenario
        //     Polygon2 currentPoly = poly;
        //     HashSet<MyVector2> visitedIntersectionPoints = new HashSet<MyVector2>();
        //     int totalIntersectionVertices = poly.Points.Count(p => p.IsIntersection);
        //     poly.Current = poly.LinkedPoints.Find(point);
        //     while (true)
        //     {
        //         //if we visited all intersection points then we break
        //         if(totalIntersectionVertices == visitedIntersectionPoints.Count)
        //             break;
        //         
        //         //find polygon
        //         //find non-visited intersection point
        //         //p.s. might need i counter for one cycle and not be afraid of infinite loop
        //         while (!poly.Current.Value.IsIntersection && !visitedIntersectionPoints.Contains(poly.Current.Value)) 
        //             poly.Current = poly.Current.Next;
        //
        //         isEntering = !isEntering;
        //         
        //         //add founded intersection point to the list of visited and start traverse new polygon
        //         MyVector2 startingIntersectionPoint = poly.Current.Value;
        //         List<MyVector2> newPolygonVertexList = new List<MyVector2> {startingIntersectionPoint};
        //         visitedIntersectionPoints.Add(startingIntersectionPoint);
        //
        //         LinkedListNode<MyVector2> curr = poly.Current.Next ?? poly.LinkedPoints.First;
        //         while (!curr.Value.Equals(startingIntersectionPoint))
        //         {
        //             //add new vertex
        //             curr = curr.Next;
        //             newPolygonVertexList.Add(curr.Value);
        //             
        //             //if we found intersection then change polygon linked list
        //             if (curr.Value.IsIntersection)
        //             {
        //                 visitedIntersectionPoints.Add(curr.Value);
        //                 
        //                 //if we found intersection then switch to other poly and continue to traverse
        //                 currentPoly = currentPoly == poly ? window : poly;
        //                 curr = currentPoly.LinkedPoints.Find(curr.Value);
        //             }
        //         }
        //         result.Add( new Polygon2(newPolygonVertexList));
        //     }
        //     
        //     return result;
        // }

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

        public static List<ClipVertex2> FormPolyWithNewIntersectionVertices(Polygon2 poly, Polygon2 intersectionPoly)
        {
            List<ClipVertex2> result = new List<ClipVertex2>();
            
            //step1
            //loop through poly points and see if any is on the intersectionPoly edge. Mark them as IsOnOtherPolygonEdge = true
            //later we will decide if they are true intersection point or just form "tangent" with 2 edges and dont mark them as intersection point

            foreach (Edge2 edge1 in poly.Edges)
            {
                //check if point is on the otherPoly edge or not
                MyVector2 edgeP1 = edge1.P1;
                ClipVertex2 clipVert = new ClipVertex2(edgeP1);
                foreach (Edge2 edge2 in intersectionPoly.Edges)
                {
                    if (GeometryUtility.IsPointOnLine(edge2, edgeP1))
                    {
                        clipVert.IsOnOtherPolygonEdge = true;
                        break;
                    }
                }
                result.Add(clipVert);
                
                //find all intersections on that edge with other edges in intersectionPoly 
                List<MyVector2> intersections = new List<MyVector2>();
                foreach (Edge2 edge2 in intersectionPoly.Edges)
                {
                    if (GeometryUtility.LineLine(edge1, edge2, false))
                    {
                        MyVector2 intersectionPoint = GeometryUtility.GetLineLineIntersectionPoint(edge1, edge2);
                        intersections.Add(intersectionPoint);
                    }
                }
                
                //sort intersections and add new clipVertex to the result
                foreach (MyVector2 p in intersections.OrderBy(i => MyVector2.SqrMagnitude(edgeP1 - i)))
                {
                    var clipIntersectionVert = new ClipVertex2(p)
                    {
                        IsIntersection = true
                    };
                    result.Add(clipIntersectionVert);
                }
            }
            
            //step2
            //connect ClipVertices
            for (int i = 0; i < result.Count; i++)
            {
                int iNext = MathUtility.ClampListIndex(i + 1, result.Count);
                int iPrev = MathUtility.ClampListIndex(i - 1, result.Count);

                result[i].Next = result[iNext];
                result[i].Prev = result[iPrev];
            }
            
            //step3
            //resolve clipVertices IsEntering parameter
            //loop through new edges,
            //if middle of the prev edge is inside other poly and middle of the next edge outside other poly then it's ENTER. other way around is EXITING

            foreach (ClipVertex2 cV in result)
            {
                if(!cV.IsIntersection)
                    continue;

                MyVector2 prevMid = new Edge2(cV.Prev.Coord, cV.Coord).Middle();
                MyVector2 nextMid = new Edge2(cV.Coord, cV.Next.Coord).Middle();
                
                var isContainingPrevMiddle = intersectionPoly.Contains(prevMid);
                var isContainingNextMiddle = intersectionPoly.Contains(nextMid);
                
                if (isContainingPrevMiddle && !isContainingNextMiddle)
                    cV.IsEntering = false;
                else if(!isContainingPrevMiddle && isContainingNextMiddle)
                    cV.IsEntering = true;
            }
            
            return result;
        }
        
        /// <summary>
        /// returns true if out point vector is inside the otherPoly
        /// </summary>
        private static bool GetRandomPolyPointInsideOrOutsideOtherPoly(Polygon2 poly, Polygon2 otherPoly, out MyVector2 vec)
        {
            foreach (MyVector2 v in poly.Points)
            {
                foreach (Edge2 edge in otherPoly.Edges)
                {
                    if(GeometryUtility.IsPointOnLine(edge, v))
                        break;
                }
                
                if (otherPoly.Contains(v))
                {
                    vec = v;
                    return true;
                }
            }

            vec = new MyVector2();
            return false;
        }
    }
}