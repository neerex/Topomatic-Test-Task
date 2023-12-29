using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using UnityEngine;
using Utility;
using View;

namespace Controllers
{
    public class PolygonDataProvider : MonoBehaviour, IPolygonProvider
    {
        [SerializeField] private Transform _polyAParent;
        [SerializeField] private Transform _polyBParent;

        public void Initialize(IPolygonClippingController polygonClippingController)
        {
            VertexView[] initialVertexesA = _polyAParent.GetComponentsInChildren<VertexView>();
            VertexView[] initialVertexesB = _polyBParent.GetComponentsInChildren<VertexView>();

            foreach (VertexView vertexView in initialVertexesA) 
                vertexView.Initialize(polygonClippingController);
            
            foreach (VertexView vertexView in initialVertexesB) 
                vertexView.Initialize(polygonClippingController);
        }

        public Transform GetPolygonContainerParent(PolygonType polygonType)
        {
            return polygonType switch
            {
                PolygonType.A => _polyAParent,
                PolygonType.B => _polyBParent
            };
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