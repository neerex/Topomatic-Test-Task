using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Utility;
using GeometryUtility = Utility.GeometryUtility;

namespace GreinerHormannAlgorithm
{
    public static class GreinerHormann
    {
        public static List<List<MyVector2>> ClipPolygons(List<MyVector2> polyVector2, List<MyVector2> clipPolyVector2, BooleanOperation booleanOperation)
        {
            List<List<MyVector2>> finalPoly = new List<List<MyVector2>>();

            //Step 0. Check if any verts from polyVector2 is intersecting with clipPoly edges
            //if they are then align them correctly by adjusting verts on some epsilon value
            //and do a reverse check also
            CorrectPolyAlignments(ref polyVector2, clipPolyVector2);
            CorrectPolyAlignments(ref clipPolyVector2, polyVector2);
            
            //Step 1. Initializing needed data structure
            List<ClipVertex> poly = InitDataStructure(polyVector2);
            List<ClipVertex> clipPoly = InitDataStructure(clipPolyVector2);
            
            //Step 2. Find intersection points
            //Need to test if we have found an intersection point 
            //If none is found, the polygons dont intersect, or one polygon is inside the other
            bool hasFoundIntersection = false;

            for (int i = 0; i < poly.Count; i++)
            {
                ClipVertex currentVertex = poly[i];
                int iPlusOne = MathUtility.ClampListIndex(i + 1, poly.Count);

                MyVector2 a = poly[i].Coordinate;
                MyVector2 b = poly[iPlusOne].Coordinate;

                Edge2 a_b = new Edge2(a, b);

                for (int j = 0; j < clipPoly.Count; j++)
                {
                    int jPlusOne = MathUtility.ClampListIndex(j + 1, clipPoly.Count);
                    
                    MyVector2 c = clipPoly[j].Coordinate;
                    MyVector2 d = clipPoly[jPlusOne].Coordinate;
                    
                    Edge2 c_d = new Edge2(c, d);

                    //Are these lines intersecting?
                    if (GeometryUtility.LineLine(a_b, c_d, true))
                    {
                        hasFoundIntersection = true;
                        MyVector2 intersectionPoint2D = GeometryUtility.GetLineLineIntersectionPoint(a_b, c_d);

                        //We need to insert this intersection vertex into both polygons
                        //Insert into the polygon
                        ClipVertex vertexOnPolygon = InsertIntersectionVertex(a, b, intersectionPoint2D, currentVertex);

                        //Insert into the clip polygon
                        ClipVertex vertexOnClipPolygon = InsertIntersectionVertex(c, d, intersectionPoint2D, clipPoly[j]);

                        //Also connect the intersection vertices with each other
                        vertexOnPolygon.Neighbor = vertexOnClipPolygon;
                        vertexOnClipPolygon.Neighbor = vertexOnPolygon;
                    }
                }
            }
            
            //If the polygons are intersecting
            if (hasFoundIntersection)
            {
                //Step 3. Trace each polygon and mark entry and exit points to the other polygon's interior
                MarkEntryExit(poly, clipPolyVector2);
                MarkEntryExit(clipPoly, polyVector2);

                //Step 4. Create the desired clipped polygon
                if (booleanOperation == BooleanOperation.Intersection)
                {
                    //Where the two polygons intersect
                    List<ClipVertex> intersectionVertices = GetClippedPolygon(poly, true);
                    AddPolygonToList(intersectionVertices, finalPoly, false);
                }
                else if (booleanOperation == BooleanOperation.Union)
                {
                    //Where the two polygons intersect
                    List<ClipVertex> intersectionVertices = GetClippedPolygon(poly, true);
                    AddPolygonToList(intersectionVertices, finalPoly, false);

                    //Whats outside of the polygon that doesnt intersect
                    List<ClipVertex> outsidePolyVertices = GetClippedPolygon(poly, false);
                    AddPolygonToList(outsidePolyVertices, finalPoly, true);

                    List<ClipVertex> outsideClipPolyVertices = GetClippedPolygon(clipPoly, false);
                    AddPolygonToList(outsideClipPolyVertices, finalPoly, true);
                }
            }

            return finalPoly;
        }
        
        private static void CorrectPolyAlignments(ref List<MyVector2> polyA, List<MyVector2> polyB)
        {
            float epsilon = 0.001f;
            List<MyVector2> alignedPolyA = new List<MyVector2>();
            
            //form edges for cross check
            var polyBEdges = new List<Edge2> {new Edge2(polyB[^1], polyB[0])};
            for (int i = 1; i < polyB.Count; i++)
            {
                var prev = polyB[i - 1];
                var curr = polyB[i];
                polyBEdges.Add(new Edge2(prev, curr));
            }

            //check if edge is crossing point then adjust that point perpendicular to that line on small epsilon value
            foreach (var p in polyA)
            {
                MyVector2 pWithAlignment = p;
                foreach (var edge in polyBEdges)
                {
                    if (GeometryUtility.IsPointOnLine(edge, p))
                    {
                        MyVector3 edgeV3 = edge.Edge2ToMyV3();
                        MyVector2 cross = MyVector3.Cross(edgeV3, new MyVector3(0,0,1)).ToMyVector2();
                        cross = MyVector2.Normalize(cross);
                        MyVector2 alignmentV = epsilon * cross;
                        pWithAlignment += alignmentV;
                    }
                }
                alignedPolyA.Add(pWithAlignment);
            }

            polyA = alignedPolyA;
        }
        
        //We may end up with several polygons, so this will split the connected list into one list per polygon
        private static void AddPolygonToList(List<ClipVertex> verticesToAdd, List<List<MyVector2>> finalPoly, bool shouldReverse)
        {
            List<MyVector2> thisPolyList = new List<MyVector2>();
            finalPoly.Add(thisPolyList);

            for (int i = 0; i < verticesToAdd.Count; i++)
            {
                ClipVertex v = verticesToAdd[i];
                thisPolyList.Add(v.Coordinate);

                //Have we found a new polygon?
                if (v.NextPoly == null) 
                    continue;
                
                //If we are finding the !intersection, the vertices will be clockwise
                //so we should reverse the list to make it easier to triangulate
                if (shouldReverse) 
                    thisPolyList.Reverse();

                thisPolyList = new List<MyVector2>();
                finalPoly.Add(thisPolyList);
            }

            if (shouldReverse) 
                finalPoly[^1].Reverse();
        }

        //Get the clipped polygons: either the intersection or the !intersection
        //We might end up with more than one polygon and they are connected via ClipVertex NextPoly
        //To get the intersection, we should
        //Traverse the polygon until an intersection is encountered. Add this intersection to the clipped polygon. 
        //Traversal direction is determined by the entry/exit bool. 
        // - If the intersection is an entry, move forward along the current polygon 
        // - If the intersection is an exit, then change polygon, proceed in the backward direction of the other polygon 
        // - Change polygon if a new intersection is found
        //If we want to the !intersection, we just travel in the reverse direction from the first intersection vertex
        private static List<ClipVertex> GetClippedPolygon(List<ClipVertex> poly, bool getIntersectionPolygon)
        {
            ResetVertices(poly);
            
            List<ClipVertex> finalPolygon = new List<ClipVertex>();
            ClipVertex thisVertex = FindFirstEntryVertex(poly);
            ClipVertex firstVertex = thisVertex;

            finalPolygon.Add(thisVertex);

            thisVertex.IsTakenByFinalPolygon = true;
            thisVertex.Neighbor.IsTakenByFinalPolygon = true;

            //These rows is the only part that's different if we want to get the intersection or the !intersection
            //Are needed once again if there are more than one polygon
            bool isMovingForward = getIntersectionPolygon;
            thisVertex = getIntersectionPolygon ? thisVertex.Next : thisVertex.Prev;

            int safety = 0;

            while (true)
            {
                if (thisVertex.Equals(firstVertex) || (thisVertex.Neighbor != null && thisVertex.Neighbor.Equals(firstVertex)))
                {
                    //Try to find the next intersection point in case we end up with more than one polygon 
                    ClipVertex nextVertex = FindFirstEntryVertex(poly);

                    //Stop if we are out of intersection vertices
                    if (nextVertex == null)
                        break;
                    
                    //Connect the polygons
                    finalPolygon[^1].NextPoly = nextVertex;
                    
                    //Change to a new polygon
                    thisVertex = nextVertex;
                    firstVertex = nextVertex;
                    finalPolygon.Add(thisVertex);

                    thisVertex.IsTakenByFinalPolygon = true;
                    thisVertex.Neighbor.IsTakenByFinalPolygon = true;

                    isMovingForward = getIntersectionPolygon;
                    thisVertex = getIntersectionPolygon ? thisVertex.Next : thisVertex.Prev;
                }
                
                //If this is not an intersection, then just add it
                if (!thisVertex.IsIntersection)
                {
                    finalPolygon.Add(thisVertex);
                    thisVertex = isMovingForward ? thisVertex.Next : thisVertex.Prev;
                }
                else
                {
                    thisVertex.IsTakenByFinalPolygon = true;
                    thisVertex.Neighbor.IsTakenByFinalPolygon = true;

                    //Jump to the other polygon
                    thisVertex = thisVertex.Neighbor;
                    finalPolygon.Add(thisVertex);

                    //Move forward/back depending on if this is an entry/exit vertex and if we want to find the intersection or not
                    if (getIntersectionPolygon)
                    {
                        isMovingForward = thisVertex.IsEntry;
                        thisVertex = thisVertex.IsEntry ? thisVertex.Next : thisVertex.Prev;
                    }
                    else
                    {
                        isMovingForward = !isMovingForward;
                        thisVertex = isMovingForward ? thisVertex.Next : thisVertex.Prev;
                    }
                }

                safety++;
                if (safety > 1000)
                    break;
            }

            return finalPolygon;
        }
        
        //Reset vertices before we find the final polygon(s)
        private static void ResetVertices(List<ClipVertex> poly)
        {
            ClipVertex resetVertex = poly[0];
            int safety = 0;

            while (true)
            {
                resetVertex.IsTakenByFinalPolygon = false;
                resetVertex.NextPoly = null;

                //Reset the neighbor
                if (resetVertex.IsIntersection) 
                    resetVertex.Neighbor.IsTakenByFinalPolygon = false;

                resetVertex = resetVertex.Next;

                //All vertices are reset
                if (resetVertex.Equals(poly[0]))
                    break;

                safety++;
                if (safety > 1000)
                    break;
            }
        }

        //Find the the first entry vertex in a polygon
        private static ClipVertex FindFirstEntryVertex(List<ClipVertex> poly)
        {
            ClipVertex thisVertex = poly[0];
            ClipVertex firstVertex = thisVertex;

            int safety = 0;

            while (true)
            {
                //Is this an available entry vertex?
                if (thisVertex.IsIntersection && thisVertex.IsEntry && !thisVertex.IsTakenByFinalPolygon)
                    break;

                thisVertex = thisVertex.Next;

                //We have travelled the entire polygon without finding an available entry vertex
                if (thisVertex.Equals(firstVertex))
                {
                    thisVertex = null;
                    break;
                }

                safety++;
                if (safety > 1000)
                    break;
            }

            return thisVertex;
        }

        private static List<ClipVertex> InitDataStructure(List<MyVector2> polyVector)
        {
            List<ClipVertex> poly = polyVector.Select(coord => new ClipVertex(coord)).ToList();

            //Connect the vertices
            for (int i = 0; i < poly.Count; i++)
            {
                int iPlusOne = MathUtility.ClampListIndex(i + 1, poly.Count);
                int iMinusOne = MathUtility.ClampListIndex(i - 1, poly.Count);

                poly[i].Next = poly[iPlusOne];
                poly[i].Prev = poly[iMinusOne];
            }

            return poly;
        }
        
        //Insert intersection vertex
        private static ClipVertex InsertIntersectionVertex(MyVector2 a, MyVector2 b, MyVector2 intersectionPoint, ClipVertex currentVertex)
        {
            //Calculate alpha which is how far the intersection Coordinate is between a and b
            //so we can insert this vertex at the correct position
            //pos = start + dir * alpha
            float alpha = MyVector2.SqrMagnitude(a - intersectionPoint) / MyVector2.SqrMagnitude(a - b);

            ClipVertex intersectionVertex = new ClipVertex(intersectionPoint)
            {
                IsIntersection = true,
                Alpha = alpha
            };

            //Now we need to insert this intersection point somewhere after currentVertex
            ClipVertex insertAfterThisVertex = currentVertex;
            int safety = 0;

            while (true)
            {
                //If the next vertex is an intersection vertex with a higher alpha 
                //or if the next vertex is not an intersection vertex, we cant improve, so we break
                if (insertAfterThisVertex.Next.Alpha > alpha || !insertAfterThisVertex.Next.IsIntersection)
                     break;

                insertAfterThisVertex = insertAfterThisVertex.Next;

                safety++;
                if (safety > 1000)
                    break;
            }

            //Connect the vertex to the surrounding vertices
            intersectionVertex.Next = insertAfterThisVertex.Next;
            intersectionVertex.Prev = insertAfterThisVertex;
            insertAfterThisVertex.Next.Prev = intersectionVertex;
            insertAfterThisVertex.Next = intersectionVertex;

            return intersectionVertex;
        }
        
        //Mark entry exit points
        private static void MarkEntryExit(List<ClipVertex> poly, List<MyVector2> clipPolyVector)
        {
            //First see if the first vertex starts inside or outside
            bool isInside = GeometryUtility.PointPolygon(clipPolyVector, poly[0].Coordinate);

            ClipVertex currentVertex = poly[0];
            ClipVertex firstVertex = currentVertex;

            int safety = 0;

            while (true)
            {
                if (currentVertex.IsIntersection)
                {
                    currentVertex.IsEntry = !isInside;
                    isInside = !isInside;
                }

                currentVertex = currentVertex.Next;

                //We have travelled around the entire polygon
                if (currentVertex.Equals(firstVertex))
                    break;

                safety++;
                if (safety > 1000)
                    break;
            }
        }
    }
}