using System.Collections.Generic;
using System.Linq;
using DataStructures;
using UnityEngine;
using Utility;
using GeometryUtility = Utility.GeometryUtility;

namespace GreinerHormannAlgorithm
{
    public class PolyClipper
    {
        public static (List<List<ClipVertex2>> polys, List<Polygon2> finalPoly) PolygonClipper(Polygon2 poly, Polygon2 window, BooleanOperation operation)
        {
            //check if polygon is self intersecting then return empty result
            if (poly.IsSelfIntersecting()) return default;
            if (window.IsSelfIntersecting()) return default;
            
            //simplify polygon, remove vertexes that are on the same line 1---2---3, so remove 2
            poly = SimplifyPolygon(poly);
            window = SimplifyPolygon(window);
            
            //step 0:
            //polygons should be clockwise
            //lets check if they are clockwise, if they are not, then we form new polygons with clockwise input
            Polygon2 polyClockwise = GetClockwisePoly(poly);
            Polygon2 windowClockwise = GetClockwisePoly(window);
            
            
            // step 1:
            // form new polygons with intersection connections and
            // init needed data structures
            List<ClipVertex2> polyWithIntersectionVertices = FormPolyWithNewIntersectionVertices(polyClockwise, windowClockwise);
            List<ClipVertex2> windowWithIntersectionVertices = FormPolyWithNewIntersectionVertices(windowClockwise, polyClockwise);

            //step2
            //connect each clipVertex to his neighbour vertex on the other polygon,
            //where neighbour vertex is vertex on other polygon with the same coords as initial vertex coord
            ConnectClipVertices(polyWithIntersectionVertices, windowWithIntersectionVertices);
            ConnectClipVertices(windowWithIntersectionVertices, polyWithIntersectionVertices);
            
            // check if any poly has same number of intersection as vertices in the original poly
            
            //step 3:
            // if no intersections, then check if one poly is inside or outside the other
            // if no intersections founded then if any point of poly is inside the other
            // then that poly is inside the other and union operation is simply the biggest poly of those 2
            
            if (polyClockwise.Count == polyWithIntersectionVertices.Count || windowClockwise.Count == windowWithIntersectionVertices.Count)
            {
                // if(operation == BooleanOperation.Union)
                //     return GetTheBiggestPolyIfOneInsideTheOtherOrEmpty(polyWithIntersectionVertices, windowWithIntersectionVertices);
            
                if (operation == BooleanOperation.Intersection)
                    return (new List<List<ClipVertex2>>(), new List<Polygon2>());
            }

            //step 4:
            //get intersection polygons
            List<Polygon2> finalPoly = GetIntersectionPolygons(polyWithIntersectionVertices, windowWithIntersectionVertices);

            return (new List<List<ClipVertex2>>
            {
                polyWithIntersectionVertices,
                windowWithIntersectionVertices
            }, finalPoly);
        }

        private static Polygon2 SimplifyPolygon(Polygon2 poly)
        {
            var simplifiedPolyVerts = new List<MyVector2>();
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

        private static void ConnectClipVertices(List<ClipVertex2> poly, IReadOnlyCollection<ClipVertex2> window)
        {
            foreach (ClipVertex2 cV in poly)
            {
                if(!cV.IsIntersection)
                    continue;
                
                ClipVertex2 neighbour = window.FirstOrDefault(v => cV.Coord.Equals(v.Coord));
                if (neighbour == null)
                {
                    UnityEngine.Debug.LogError($"No neighbour found. {cV.Coord}");
                    continue;
                }
                cV.Neighbour = neighbour;
            }
        }

        private static List<Polygon2> GetIntersectionPolygons(List<ClipVertex2> poly, List<ClipVertex2> window, bool shouldReverse = false)
        {
            List<Polygon2> result = new List<Polygon2>();
            
            //initialize needed parameters
            List<MyVector2> visitedIntersection = new List<MyVector2>();
            int totalEnteringIntersections = poly.Count(cV => cV.IsEntering);
            
            //Debug.Log($" Enterings = {totalEnteringIntersections}");
            
            while (visitedIntersection.Count <= totalEnteringIntersections)
            {
                //step 1:
                //get any entering intersection point
                ClipVertex2 entering = poly.FirstOrDefault(cV => cV.IsEntering && !visitedIntersection.Contains(cV.Coord));
                ClipVertex2 curr = entering;

                
                if(entering == null)
                    break;
                
                MyVector2 enteringCoord = entering.Coord;
                visitedIntersection.Add(entering.Coord);
                
                //step 2:
                //find union polygons
                var newPolyVertexList = new List<MyVector2>();
                do
                {
                    newPolyVertexList.Add(curr.Coord);
                    
                    //todo: switch to neighbour is reverse and its union operation then
                    
                    curr = curr.Next;
                    if (curr.IsIntersection)
                    {
                        if (curr.IsEntering) 
                            visitedIntersection.Add(curr.Coord);
                
                        curr = curr.Neighbour;
                    }
                
                } while (!curr.Coord.Equals(enteringCoord));

                result.Add(new Polygon2(newPolyVertexList));
            }
            return result;
        }

        // private static List<Polygon2> GetTheBiggestPolyIfOneInsideTheOtherOrEmpty(List<ClipVertex2> polyVertices, List<ClipVertex2> windowVertices)
        // {
        //     var polyPoints = polyVertices.Select(v => v.Coord).ToList();
        //     var windowPoint = windowVertices.Select(v => v.Coord).ToList();
        //
        //     var poly = new Polygon2(polyPoints);
        //     var window = new Polygon2(windowPoint);
        //
        //     if(poly.Points.Any(p => window.Contains(p)))
        //         return new List<Polygon2> {window};
        //
        //     if(window.Points.Any(p => poly.Contains(p)))
        //         return new List<Polygon2> {poly};
        //
        //     return new List<Polygon2>();
        // }

        private static Polygon2 GetClockwisePoly(Polygon2 poly)
        {
            if (poly.IsClockwise)
                return poly;

            List<MyVector2> clockwisePolyPoints = poly.Points.ToList();
            clockwisePolyPoints.Reverse();
            return new Polygon2(clockwisePolyPoints);
        }

        private static List<ClipVertex2> FormPolyWithNewIntersectionVertices(Polygon2 poly, Polygon2 intersectionPoly)
        {
            List<ClipVertex2> result = new List<ClipVertex2>();
            HashSet<MyVector2> visited = new HashSet<MyVector2>();
            //step1
            //loop through poly points and see if any is on the intersectionPoly edge. Mark them as IsOnOtherPolygonEdge = true
            //later we will decide if they are true intersection point or just form "tangent" with 2 edges and dont mark them as intersection point
            foreach (Edge2 edge1 in poly.Edges)
            {
                HashSet<MyVector2> intersections = new HashSet<MyVector2>();

                //check if point is on the otherPoly edge or not
                MyVector2 edgeP1 = edge1.P1;
                ClipVertex2 clipVert = new ClipVertex2(edgeP1);
                
                //check if point exactly on the end pont of the edge
                foreach (Edge2 edge2 in intersectionPoly.Edges)
                {
                    bool isExactlyOnTheEndpoint = edgeP1.Equals(edge2.P1) || edgeP1.Equals(edge2.P2);
                    if (isExactlyOnTheEndpoint)
                    {
                        clipVert.IsOnTheVertexOfOtherPolygon = true;
                        clipVert.IsIntersection = true;

                        if (visited.Add(clipVert.Coord)) 
                            intersections.Add(clipVert.Coord);
                        
                        break;
                    }

                    if (GeometryUtility.IsPointOnLine(edge2, edgeP1))
                    {
                        clipVert.IsOnOtherPolygonEdge = true;
                        clipVert.IsIntersection = true;

                        if (visited.Add(clipVert.Coord)) 
                            intersections.Add(clipVert.Coord);
                        
                        break;
                    }
                }

                if (!clipVert.IsIntersection)
                {
                    result.Add(clipVert);
                    visited.Add(clipVert.Coord);
                }
                
                //find all intersections on that edge with other edges in intersectionPoly 
                foreach (Edge2 edge2 in intersectionPoly.Edges)
                {
                    //check is both edges are parallel and intersecting
                    // so we have 2 situation where we have 1 or 2 intersections
                    if (GeometryUtility.LinesParallel(edge1, edge2))
                    {
                        if (GeometryUtility.IsEdgeContainsEdge(edge1, edge2))
                        {
                            if (visited.Add(edge2.P1))
                                intersections.Add(edge2.P1);

                            if (visited.Add(edge2.P2))
                                intersections.Add(edge2.P2);
                            
                            continue;
                        }
                    
                        if (GeometryUtility.IsEdgeTouchingEdge(edge1, edge2, out MyVector2 touchPoint))
                        {
                            if (visited.Add(touchPoint))
                                intersections.Add(touchPoint);
                            
                            continue;
                        }
                    }
                    
                    if (GeometryUtility.LineLine(edge1, edge2, false))
                    {
                        MyVector2 intersectionPoint = GeometryUtility.GetLineLineIntersectionPoint(edge1, edge2);
                    
                        if (visited.Add(intersectionPoint)) 
                            intersections.Add(intersectionPoint);
                    }

                    // if (GeometryUtility.LineSegmentsIntersect(edge1, edge2, out var intersectionPoint))
                    //     if (visited.Add(intersectionPoint))
                    //         intersections.Add(intersectionPoint);
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
                
                // var isContainingPrevMiddle = GeometryUtility.PointPolygon(intersectionPoly.Points, prevMid);
                // var isContainingNextMiddle = GeometryUtility.PointPolygon(intersectionPoly.Points, nextMid);
                
                var isContainingPrevMiddle = intersectionPoly.Contains(prevMid);
                var isContainingNextMiddle = intersectionPoly.Contains(nextMid);
                
                if (isContainingPrevMiddle && !isContainingNextMiddle)
                    cV.IsEntering = false;
                else if(!isContainingPrevMiddle && isContainingNextMiddle)
                    cV.IsEntering = true;
                
                //check edge cases where vertex is exactly on the other vertex
                if(isContainingPrevMiddle && isContainingNextMiddle || !isContainingPrevMiddle && !isContainingNextMiddle)
                    cV.IsIntersection = false;
            }

            return result.ToList();
        }
    }
}