using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using UnityEngine;

namespace Controllers
{
    public class PolygonViewUpdater : MonoBehaviour
    {
        private readonly float _updateFrequency = 0.02f;
        private float _timeSinceLastUpdate;
        private bool _isUpdateRequested;
        
        private List<Triangle2> _finalPoly;
        private Polygon2 _polyA;
        private Polygon2 _polyB;
        
        private IPolygonClippingController _polygonClippingController;
        private IPolygonMeshCreator _polygonMeshCreator;
        private PolygonConnectionDrawer _polygonConnectionDrawer;


        public void Initialize(IPolygonClippingController polygonClippingController, 
            IPolygonMeshCreator polygonMeshCreator, 
            PolygonConnectionDrawer polygonConnectionDrawer)
        {
            _polygonClippingController = polygonClippingController;
            _polygonMeshCreator = polygonMeshCreator;
            _polygonConnectionDrawer = polygonConnectionDrawer;
            
            _polygonClippingController.OnPolygonsRecalculation += UpdateVisuals;
        }

        private void Update()
        {
            _timeSinceLastUpdate += Time.deltaTime;
            if (_timeSinceLastUpdate > _updateFrequency)
            {
                _timeSinceLastUpdate -= _updateFrequency;

                if (_isUpdateRequested)
                {
                    _isUpdateRequested = false;
                    UpdateVisuals();
                }
            }
        }

        private void UpdateVisuals(Polygon2 polyA, Polygon2 polyB, List<Triangle2> finalPoly)
        {
            _isUpdateRequested = true;
            _polyA = polyA;
            _polyB = polyB;
            _finalPoly = finalPoly;
        }

        private void UpdateVisuals()
        {
            if(_finalPoly == null || _polyA.Points == null || _polyB.Points == null)
                return;
            
            _polygonConnectionDrawer.UpdateLineRenderers(_polyA, _polyB, _finalPoly);
            _polygonMeshCreator.CreateMeshFromTriangleList(_finalPoly);
        }
                
    }
}