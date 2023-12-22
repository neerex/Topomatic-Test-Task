using System;
using System.Collections.Generic;
using DataStructures;
using UnityEngine;
using Utility;

namespace Controllers
{
    public class PolygonMeshCreator : MonoBehaviour
    {
        [SerializeField] PolygonCollider2D _collider;
        [SerializeField] MeshFilter _meshFilter;

        public static PolygonMeshCreator Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public void CreateMeshFromPolyVertexList(List<List<MyVector2>> finalPoly)
        {
            _collider.pathCount = finalPoly.Count;
            for (int i = 0; i < finalPoly.Count; i++)
            {
                var path = CreatePath(finalPoly[i]);
                _collider.SetPath(i, path);
            }
            Mesh mesh = _collider.CreateMesh(true, true);
            _meshFilter.mesh = mesh;
        }

        private Vector2[] CreatePath(List<MyVector2> poly)
        {
            var path = new Vector2[poly.Count];
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = poly[i].ToVector2();
            }
            return path;
        }
    }
}