using System;
using System.Collections.Generic;
using DataStructures;
using Utility;
using GeometryUtility = Utility.GeometryUtility;

namespace TriangulationAlgorithm.EarClipping
{
    //Triangulate a concave hull (with holes) by using an algorithm called Ear Clipping
    public static class EarClipping
    {
        //The points on the hull (vertices) should be ordered counter-clockwise (and no doubles)
        //Optimize triangles means that we will get a better-looking triangulation, which resembles a constrained Delaunay triangulation
        public static HashSet<Triangle2> Triangulate(List<MyVector2> vertices, bool optimizeTriangles = true)
        {
            //Validate the data
            if (vertices == null || vertices.Count <= 2)
                return null;
            
            vertices.Reverse();

            //Step 0. Create a linked list connecting all vertices with each other which will make the calculations easier and faster
            List<LinkedVertex> verticesLinked = new List<LinkedVertex>();

            for (int i = 0; i < vertices.Count; i++)
            {
                LinkedVertex v = new LinkedVertex(vertices[i]);
                verticesLinked.Add(v);
            }

            //Link them to each other
            for (int i = 0; i < verticesLinked.Count; i++)
            {
                LinkedVertex v = verticesLinked[i];
                v.PrevLinkedVertex = verticesLinked[MathUtility.ClampListIndex(i - 1, verticesLinked.Count)];
                v.NextLinkedVertex = verticesLinked[MathUtility.ClampListIndex(i + 1, verticesLinked.Count)];
            }

            //Step 1. Find:
            //- Convex vertices (interior angle smaller than 180 degrees)
            //- Reflect vertices (interior angle greater than 180 degrees) so should maybe be called concave vertices?
            //Interior angle is the angle between two vectors inside the polygon if we move around the polygon counter-clockwise
            //If they are neither we assume they are reflect (or we will end up with odd triangulations)
            HashSet<LinkedVertex> convexVerts = new HashSet<LinkedVertex>();
            HashSet<LinkedVertex> reflectVerts = new HashSet<LinkedVertex>();

            foreach (LinkedVertex v in verticesLinked)
            {            
                bool isConvex = IsVertexConvex(v);

                if (isConvex)
                    convexVerts.Add(v);
                else
                    reflectVerts.Add(v);
            }

            //Step 2. Find the initial ears
            HashSet<LinkedVertex> earVerts = new HashSet<LinkedVertex>();

            //An ear is always a convex vertex
            foreach (LinkedVertex v in convexVerts)
            {
                //And we only need to test the reflect vertices
                if (IsVertexEar(v, reflectVerts)) 
                    earVerts.Add(v);
            }

            //Step 3. Build the triangles
            HashSet<Triangle2> triangulation = new HashSet<Triangle2>();

            //We know how many triangles we will get (number of vertices - 2) which is true for all simple polygons
            //This can be used to stop the algorithm
            int maxTriangles = verticesLinked.Count - 2;

            //Because we use a while loop, having an extra safety is always good so we dont get stuck in infinite loop
            int safety = 0;

            while (true)
            {
                //Pick an ear vertex and form a triangle
                LinkedVertex ear = GetEarVertex(earVerts, optimizeTriangles);
                if (ear == null)
                    break;

                LinkedVertex v_prev = ear.PrevLinkedVertex;
                LinkedVertex v_next = ear.NextLinkedVertex;

                Triangle2 t = new Triangle2(ear.Pos, v_prev.Pos, v_next.Pos);

                //Try to flip this triangle according to Delaunay triangulation
                if (optimizeTriangles)
                    OptimizeTriangle(t, triangulation);
                else
                    triangulation.Add(t);

                //Check if we have found all triangles
                //This should also prevent us from getting stuck in an infinite loop
                if (triangulation.Count >= maxTriangles)
                    break;
                
                //If we havent found all triangles we have to reconfigure the data structure
                //Remove the ear we used to build a triangle
                convexVerts.Remove(ear);
                earVerts.Remove(ear);

                //Reconnect the vertices because one vertex has now been removed
                v_prev.NextLinkedVertex = v_next;
                v_next.PrevLinkedVertex = v_prev;

                //Reconfigure the adjacent vertices
                ReconfigureAdjacentVertex(v_prev, convexVerts, reflectVerts, earVerts);
                ReconfigureAdjacentVertex(v_next, convexVerts, reflectVerts, earVerts);
                
                safety++;

                if (safety > 5000)
                    break;
            }
            return triangulation;
        }
        
        //Optimize a new triangle according to Delaunay triangulation
        //TODO: This process would have been easier if we had used the HalfEdge data structure
        private static void OptimizeTriangle(Triangle2 t, HashSet<Triangle2> triangulation)
        {
            FindEdgeInTriangulation(t, triangulation, out var hasOppositeEdge, out var tOpposite, out var edgeToSwap);

            //If it has no opposite edge we just add triangle to the triangulation because it can't be improved
            if (!hasOppositeEdge)
            {
                triangulation.Add(t);
                return;
            }
            //Step 3. Check if we should swap this edge according to Delaunay triangulation rules
            //a, b, c belongs to the triangle and d is the point on the other triangle
            //a-c is the edge, which is important so we can flip it, by making the edge b-d
            MyVector2 a = edgeToSwap.P2;
            MyVector2 c = edgeToSwap.P1;
            MyVector2 b = t.GetVertexWhichIsNotPartOfEdge(edgeToSwap);
            MyVector2 d = tOpposite.GetVertexWhichIsNotPartOfEdge(edgeToSwap);

            bool shouldFlipEdge = GeometryUtility.ShouldFlipEdge(a, b, c, d);
            //bool shouldFlipEdge = DelaunayMethods.ShouldFlipEdgeStable(a, b, c, d);

            if (shouldFlipEdge)
            {
                //First remove the old triangle
                triangulation.Remove(tOpposite);

                //Build two new triangles
                Triangle2 t1 = new Triangle2(a, b, d);
                Triangle2 t2 = new Triangle2(b, c, d);

                triangulation.Add(t1);
                triangulation.Add(t2);
            }
            else
            {
                triangulation.Add(t);
            }
        }
        
        //Find an edge in a triangulation and return the triangle the edge is attached to
        private static void FindEdgeInTriangulation(Triangle2 tNew, HashSet<Triangle2> triangulation, out bool hasOppositeEdge, out Triangle2 tOpposite, out Edge2 edgeToSwap)
        {
            //Step 1. Find the triangle's biggest interior angle and its opposite edge
            float angleP1 = CalculateInteriorAngle(tNew.P3, tNew.P1, tNew.P2);
            float angleP2 = CalculateInteriorAngle(tNew.P1, tNew.P2, tNew.P3);
            float angleP3 = (float)Math.PI - (angleP1 + angleP2);

            MyVector2 vertexWithBiggestInteriorAngle = tNew.P1;

            if (angleP2 > angleP1)
            {
                vertexWithBiggestInteriorAngle = tNew.P2;

                if (angleP3 > angleP2) 
                    vertexWithBiggestInteriorAngle = tNew.P3;
            }
            else if (angleP3 > angleP1)
            {
                vertexWithBiggestInteriorAngle = tNew.P3;
            }
            edgeToSwap = tNew.FindOppositeEdgeToVertex(vertexWithBiggestInteriorAngle);
            
            //Step 2. Check if this edge exists among the already generated triangles, which means we have a neighbor
            hasOppositeEdge = false;
            tOpposite = new Triangle2();

            foreach (Triangle2 tTest in triangulation)
            {
                if (tTest.IsEdgePartOfTriangle(edgeToSwap))
                {
                    hasOppositeEdge = true;
                    tOpposite = tTest;
                    break;
                }
            }
        }
        
        //Reconfigure an adjacent vertex that was used to build a triangle
        private static void ReconfigureAdjacentVertex(LinkedVertex v, HashSet<LinkedVertex> convexVerts, HashSet<LinkedVertex> reflectVerts, HashSet<LinkedVertex> earVerts)
        {
            //If the adjacent vertex was reflect...
            if (reflectVerts.Contains(v))
            {
                //it may now be convex...
                if (IsVertexConvex(v))
                {
                    reflectVerts.Remove(v);
                    convexVerts.Add(v);

                    //and possible a new ear
                    if (IsVertexEar(v, reflectVerts)) 
                        earVerts.Add(v);
                }
            }
            //If an adjacent vertex was convex, it will always still be convex
            else
            {
                bool isEar = IsVertexEar(v, reflectVerts);

                //This vertex was an ear but is no longer an ear
                if (earVerts.Contains(v) && !isEar)
                    earVerts.Remove(v);
                else if (isEar) 
                    earVerts.Add(v);
            }
        }
        
        //Get the best ear vertex
        private static LinkedVertex GetEarVertex(HashSet<LinkedVertex> earVertices, bool optimizeTriangles)
        {
            LinkedVertex bestEarVertex = null;

            //To get better looking triangles we should always get the ear with the smallest interior angle
            if (optimizeTriangles)
            {
                float smallestInteriorAngle = float.MaxValue;

                foreach (LinkedVertex v in earVertices)
                {
                    float interiorAngle = CalculateInteriorAngle(v);

                    if (interiorAngle < smallestInteriorAngle)
                    {
                        bestEarVertex = v;
                        smallestInteriorAngle = interiorAngle;
                    }
                }
            }
            //Just get first best ear vertex
            else
            {
                foreach (LinkedVertex v in earVertices)
                {
                    bestEarVertex = v;
                    break;
                }
            }
            
            return bestEarVertex;
        }
        
        //Is a vertex an ear?
        private static bool IsVertexEar(LinkedVertex vertex, HashSet<LinkedVertex> reflectVertices)
        {
            //Consider the triangle
            MyVector2 p_prev = vertex.PrevLinkedVertex.Pos;
            MyVector2 p = vertex.Pos;
            MyVector2 p_next = vertex.NextLinkedVertex.Pos;

            Triangle2 t = new Triangle2(p_prev, p, p_next);

            //If any of the other vertices is within this triangle, then this vertex is not an ear
            //We only need to check the reflect vertices
            foreach (LinkedVertex otherVertex in reflectVertices)
            {
                MyVector2 test_p = otherVertex.Pos;

                //Dont compare with any of the vertices the triangle consist of
                if (test_p.Equals(p_prev) || test_p.Equals(p) || test_p.Equals(p_next))
                    continue;

                //If a relect vertex intersects with the triangle, then this vertex is not an ear
                if (GeometryUtility.PointTriangle(t, test_p, includeBorder: false))
                    return false;
            }
            //No vertex is intersecting with the triangle, so this vertex must be an ear
            return true;
        }
        
        //Is a vertex convex? (if not its concave or neither if its a straight line)
        private static bool IsVertexConvex(LinkedVertex v)
        {
            MyVector2 p_prev = v.PrevLinkedVertex.Pos;
            MyVector2 p = v.Pos;
            MyVector2 p_next = v.NextLinkedVertex.Pos;

            return IsVertexConvex(p_prev, p, p_next);
        }

        public static bool IsVertexConvex(MyVector2 p_prev, MyVector2 p, MyVector2 p_next)
        {
            float interiorAngle = CalculateInteriorAngle(p_prev, p, p_next);
            
            //Convex
            if (interiorAngle < Math.PI)
                return true;
            
            //Concave
            return false;
        }
        
        //Get interior angle (the angle within the polygon) of a vertex
        private static float CalculateInteriorAngle(LinkedVertex v)
        {
            MyVector2 p_prev = v.PrevLinkedVertex.Pos;
            MyVector2 p = v.Pos;
            MyVector2 p_next = v.NextLinkedVertex.Pos;

            return CalculateInteriorAngle(p_prev, p, p_next);
        }

        private static float CalculateInteriorAngle(MyVector2 p_prev, MyVector2 p, MyVector2 p_next)
        {
            //Two vectors going from the vertex
            //You (most likely) don't need to normalize these
            MyVector2 p_to_p_prev = p_prev - p;
            MyVector2 p_to_p_next = p_next - p;

            //The angle between the two vectors [rad]
            //This will calculate the outside angle
            float angle = MathUtility.AngleFromToCCW(p_to_p_prev, p_to_p_next);

            //The interior angle is the opposite of the outside angle
            float interiorAngle = (float)Math.PI * 2f - angle;

            return interiorAngle;
        }

        //Count vertices that are linked to each other in a looping way
        private static int CountLinkedVertices(LinkedVertex startVertex)
        {
            int counter = 1;

            LinkedVertex currentVertex = startVertex;

            while (true)
            {
                currentVertex = currentVertex.NextLinkedVertex;
            
                if (currentVertex == startVertex)
                    break;

                counter++;
            
                if (counter > 5000)
                    break;
            }

            return counter;
        }
    }
}
