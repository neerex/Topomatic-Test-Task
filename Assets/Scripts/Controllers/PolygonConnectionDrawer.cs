using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using UnityEngine;
using Utility;

namespace Controllers
{
    public class PolygonConnectionDrawer : MonoBehaviour
    {
        private IPolygonClippingController _polygonClippingController;
        
        [SerializeField] private LineRenderer _polyALineRenderer;
        [SerializeField] private LineRenderer _polyBLineRenderer;
        
        public void Initialize(IPolygonClippingController polygonClippingController)
        {
            _polygonClippingController = polygonClippingController;
            _polygonClippingController.OnPolygonsRecalculation += UpdateLineRenderers;
        }

        private void UpdateLineRenderers(List<MyVector2> polyA, List<MyVector2> polyB, List<List<MyVector2>> finalPoly)
        {
            var polyAV3 = polyA.ToListV3().ToArray();
            var polyBV3 = polyB.ToListV3().ToArray();
            
            _polyALineRenderer.positionCount = polyAV3.Length;
            _polyBLineRenderer.positionCount = polyBV3.Length;
            
            _polyALineRenderer.SetPositions(polyAV3);
            _polyBLineRenderer.SetPositions(polyBV3);
        }

        public void OnDestroy()
        {
            _polygonClippingController.OnPolygonsRecalculation -= UpdateLineRenderers;
        }
    }
}
