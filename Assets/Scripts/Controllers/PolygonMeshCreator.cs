using System;
using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using Habrador_Computational_Geometry;
using UnityEngine;
using Utility;
using Utility.UnityUtility;

namespace Controllers
{
    public class PolygonMeshCreator : IPolygonMeshCreator
    {
        private readonly IPolygonClippingController _polygonClippingController;
        private PolygonCollider2D _collider;
        private MeshFilter _meshFilter;
        private List<Mesh> _meshes = new();

        public PolygonMeshCreator(IPolygonClippingController polygonClippingController)
        {
            _polygonClippingController = polygonClippingController;
            SetupMeshCreator();
        }

        public void CreateMeshFromPolyVertexList(List<List<MyVector2>> finalPoly)
        {
            foreach (var poly in finalPoly)
            {
                HashSet<Triangle2> triangulation = EarClipping.Triangulate(poly, null, false);
                if (triangulation == null)
                    continue;

                //Convert from triangle to mesh
                Mesh mesh = MeshUtility.Triangles2ToMesh(triangulation, false);
                _meshes.Add(mesh);
            }
            
            CombineInstance[] combine = new CombineInstance[_meshes.Count];

            for (int i = 0; i < _meshes.Count; i++) 
                combine[i].mesh = _meshes[i];

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combine);
            _meshFilter.sharedMesh = finalMesh;
            
            // _collider.pathCount = finalPoly.Count;
            // for (int i = 0; i < finalPoly.Count; i++)
            // {
            //     var path = CreatePath(finalPoly[i]);
            //     _collider.SetPath(i, path);
            // }
            // Mesh mesh = _collider.CreateMesh(true, true);
            // _meshFilter.sharedMesh = mesh;
        }

        private Vector2[] CreatePath(List<MyVector2> poly)
        {
            Vector2[] path = new Vector2[poly.Count];
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = poly[i].ToVector2();
            }
            return path;
        }

        private void SetupMeshCreator()
        {
            GameObject meshCreator = new GameObject("PolygonMeshCreator");
            _collider = meshCreator.AddComponent<PolygonCollider2D>();
            _meshFilter = meshCreator.AddComponent<MeshFilter>();
            MeshRenderer renderer = meshCreator.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Resources.Load<Material>("Materials/Poly");
        }
    }
}