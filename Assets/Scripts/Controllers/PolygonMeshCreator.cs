using System.Collections.Generic;
using System.Linq;
using Controllers.Interfaces;
using DataStructures;
using TriangulationAlgorithm.EarClipping;
using UnityEngine;
using Utility.UnityUtility;

namespace Controllers
{
    public class PolygonMeshCreator : IPolygonMeshCreator
    {
        private MeshFilter _meshFilter;
        private readonly List<Mesh> _meshes = new();

        public PolygonMeshCreator()
        {
            SetupMeshCreator();
        }

        public Mesh CreateMeshFromPolyVertexList(List<List<MyVector2>> finalPoly)
        {
            _meshes.Clear();
            foreach (var poly in finalPoly)
            {
                HashSet<Triangle2> triangulation = EarClipping.Triangulate(poly, false);
                
                if (triangulation == null)
                    continue;

                Mesh mesh = MeshUtility.Triangles2ToMesh(triangulation, false);
                _meshes.Add(mesh);
            }
            
            CombineInstance[] combine = new CombineInstance[_meshes.Count];

            for (int i = 0; i < _meshes.Count; i++) 
                combine[i].mesh = _meshes[i];

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combine, true, false);
            _meshFilter.sharedMesh = finalMesh;
            return finalMesh;
        }

        private void SetupMeshCreator()
        {
            GameObject meshCreator = new GameObject("PolygonMeshCreator");
            _meshFilter = meshCreator.AddComponent<MeshFilter>();
            MeshRenderer renderer = meshCreator.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Resources.Load<Material>("Materials/Poly");
        }
    }
}