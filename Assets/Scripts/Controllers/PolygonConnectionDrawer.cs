using System.Collections.Generic;
using DataStructures;
using UnityEngine;
using Utility;

namespace Controllers
{
    public class PolygonConnectionDrawer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _polyALineRenderer;
        [SerializeField] private LineRenderer _polyBLineRenderer;
        
        public void UpdateLineRenderers(List<MyVector2> polyA, List<MyVector2> polyB, List<Triangle2> finalPoly)
        {
            Vector3[] polyAV3 = polyA.ToListV3().ToArray();
            Vector3[] polyBV3 = polyB.ToListV3().ToArray();
            
            _polyALineRenderer.positionCount = polyAV3.Length;
            _polyBLineRenderer.positionCount = polyBV3.Length;
            
            _polyALineRenderer.SetPositions(polyAV3);
            _polyBLineRenderer.SetPositions(polyBV3);
        }
    }
}
