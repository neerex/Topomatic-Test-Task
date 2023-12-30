using System.Collections.Generic;
using DataStructures;
using UnityEngine;

namespace Utility.UnityUtility
{
    public static class MeshUtility
    {
        public static Mesh Triangles2ToMesh(HashSet<Triangle2> triangles, bool useCompressedMesh, float meshHeight = 0f)
        {
            //2d to 3d
            HashSet<Triangle3> triangles_3d = new HashSet<Triangle3>();

            foreach (Triangle2 t in triangles)
            {
                triangles_3d.Add(new Triangle3(t.p1.ToMyVector3(meshHeight), 
                    t.p2.ToMyVector3(meshHeight), 
                    t.p3.ToMyVector3(meshHeight)));
            }

            //To mesh
            if (useCompressedMesh)
            {
                Mesh mesh = Triangle3ToCompressedMesh(triangles_3d);
                return mesh;
            }
            else
            {
                Mesh mesh = Triangle3ToMesh(triangles_3d);
                return mesh;
            }
        }

        private static Mesh Triangle3ToCompressedMesh(HashSet<Triangle3> triangles)
        {
            if (triangles == null)
            {
                return null;
            }
            
            //Step 2. Create the list with unique vertices
            //A hashset will make it fast to check if a vertex already exists in the collection
            HashSet<MyVector3> uniqueVertices = new HashSet<MyVector3>();

            foreach (Triangle3 t in triangles)
            {
                MyVector3 v1 = t.p1;
                MyVector3 v2 = t.p2;
                MyVector3 v3 = t.p3;

                if (!uniqueVertices.Contains(v1))
                {
                    uniqueVertices.Add(v1);
                }
                if (!uniqueVertices.Contains(v2))
                {
                    uniqueVertices.Add(v2);
                }
                if (!uniqueVertices.Contains(v3))
                {
                    uniqueVertices.Add(v3);
                }
            }

            //Create the list with all vertices
            List<MyVector3> meshVertices = new List<MyVector3>(uniqueVertices);


            //Step3. Create the list with all triangles by using the unique vertices
            List<int> meshTriangles = new List<int>();

            //Use a dictionay to quickly find which positon in the list a Vector3 has
            Dictionary<MyVector3, int> vector2Positons = new Dictionary<MyVector3, int>();

            for (int i = 0; i < meshVertices.Count; i++)
            {
                vector2Positons.Add(meshVertices[i], i);
            }

            foreach (Triangle3 t in triangles)
            {
                MyVector3 v1 = t.p1;
                MyVector3 v2 = t.p2;
                MyVector3 v3 = t.p3;

                meshTriangles.Add(vector2Positons[v1]);
                meshTriangles.Add(vector2Positons[v2]);
                meshTriangles.Add(vector2Positons[v3]);
            }
            
            //Step4. Create the final mesh
            Mesh mesh = new Mesh();

            //From MyVector3 to Vector3
            Vector3[] meshVerticesArray = new Vector3[meshVertices.Count];

            for (int i = 0; i < meshVerticesArray.Length; i++)
            {
                MyVector3 v = meshVertices[i];

                meshVerticesArray[i] = new Vector3(v.X, v.Y, v.Z);
            }

            mesh.vertices = meshVerticesArray;
            mesh.triangles = meshTriangles.ToArray();

            //Should maybe recalculate bounds and normals, maybe better to do that outside this method???
            //mesh.RecalculateBounds();
            //mesh.RecalculateNormals();

            return mesh;
        }

        private static Mesh Triangle3ToMesh(HashSet<Triangle3> triangles)
        {
            //Create the list with all vertices and triangles
            List<MyVector3> meshVertices = new List<MyVector3>();

            //Create the list with all triangles
            List<int> meshTriangles = new List<int>();

            int arrayPos = 0;

            foreach (Triangle3 t in triangles)
            {
                MyVector3 v1 = t.p1;
                MyVector3 v2 = t.p2;
                MyVector3 v3 = t.p3;

                meshVertices.Add(v1);
                meshVertices.Add(v2);
                meshVertices.Add(v3);

                meshTriangles.Add(arrayPos + 0);
                meshTriangles.Add(arrayPos + 1);
                meshTriangles.Add(arrayPos + 2);

                arrayPos += 3;
            }

            //Create the final mesh
            Mesh mesh = new Mesh();

            //From MyVector3 to Vector3
            Vector3[] meshVerticesArray = new Vector3[meshVertices.Count];

            for (int i = 0; i < meshVerticesArray.Length; i++)
            {
                MyVector3 v = meshVertices[i];

                meshVerticesArray[i] = new Vector3(v.X, v.Y, v.Z);
            }

            mesh.vertices = meshVerticesArray;
            mesh.triangles = meshTriangles.ToArray();

            return mesh;
        }
    }
}