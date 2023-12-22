using System;
using System.Collections.Generic;
using DataStructures;
using UnityEngine;
using Utility;

namespace Controllers
{
    public class PolygonProvider : MonoBehaviour
    {
        [SerializeField] private Transform _polyAParent;
        [SerializeField] private Transform _polyBParent;

        public static PolygonProvider Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public List<MyVector2> GetPolyVertices(PolygonType polygonType)
        {
            List<MyVector2> poly = polygonType switch
            {
                PolygonType.A => GetVerticesFromParent(_polyAParent).ToListMyV2(),
                PolygonType.B => GetVerticesFromParent(_polyBParent).ToListMyV2()
            };

            return poly;
        }

        private List<Vector3> GetVerticesFromParent(Transform parent)
        {
            int childCount = parent.childCount;
            List<Vector3> children = new List<Vector3>();

            for (int i = 0; i < childCount; i++) 
                children.Add(parent.GetChild(i).position);

            return children;
        }
    }
}