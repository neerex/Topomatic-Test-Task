﻿using System;
using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using UnityEngine;
using Utility;

namespace Controllers
{
    public class PolygonMeshCreator : IPolygonMeshCreator
    {
        private readonly IPolygonClippingController _polygonClippingController;
        private PolygonCollider2D _collider;
        private MeshFilter _meshFilter;

        public PolygonMeshCreator(IPolygonClippingController polygonClippingController)
        {
            _polygonClippingController = polygonClippingController;
            SetupMeshCreator();
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
            _meshFilter.sharedMesh = mesh;
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