using Controllers.Interfaces;
using UnityEngine;
using Utility.UnityUtility.CameraUtility;
using View;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class PolygonPointController : IPolygonPointController
    {
        private readonly Transform _polyAParent;
        private readonly Transform _polyBParent;

        private readonly VertexView _polyAVertexView;
        private readonly VertexView _polyBVertexView;

        private readonly IPolygonClippingController _polygonClippingController;
        private readonly Camera _camera;

        public PolygonPointController(IPolygonProvider polygonProvider, IPolygonClippingController polygonClippingController)
        {
            _polygonClippingController = polygonClippingController;
            _camera = Camera.main;

            _polyAVertexView = Resources.Load<VertexView>("Prefabs/VertexViewA");
            _polyBVertexView = Resources.Load<VertexView>("Prefabs/VertexViewB");

            _polyAParent = polygonProvider.GetPolygonContainerParent(PolygonType.A);
            _polyBParent = polygonProvider.GetPolygonContainerParent(PolygonType.B);
        }

        public void AddVertex(PolygonType polygonType)
        {
            VertexView view = polygonType switch
            {
                PolygonType.A => _polyAVertexView,
                PolygonType.B => _polyBVertexView
            };

            Transform parent = polygonType switch
            {
                PolygonType.A => _polyAParent,
                PolygonType.B => _polyBParent
            };

            Vector3 randomVector = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
            Vector3 randomPoint = parent.GetChild(0).position + randomVector;
            Vector3 clampedPoint = CameraUtility.ClampPointToViewportWithBorder(_camera, randomPoint);

            VertexView newVertex = Object.Instantiate(view, clampedPoint, Quaternion.identity, parent);
            newVertex.Initialize(_polygonClippingController);
            
            _polygonClippingController.CalculatePolygons();
        }

        public void RemoveVertex(PolygonType polygonType)
        {
            Transform parent = polygonType switch
            {
                PolygonType.A => _polyAParent,
                PolygonType.B => _polyBParent
            };
            RemoveLastChild(parent);
            _polygonClippingController.CalculatePolygons();
        }

        private void RemoveLastChild(Transform parent)
        {
            int childCount = parent.childCount;
            
            if(childCount == 3) 
                return;
            
            Transform lastChild = parent.GetChild(childCount - 1);
            Object.DestroyImmediate(lastChild.gameObject);
        }
    }

    public enum PolygonType
    {
        A = 0,
        B = 1
    }
}