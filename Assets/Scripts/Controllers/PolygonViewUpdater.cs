using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using UnityEngine;

namespace Controllers
{
    public class PolygonViewUpdater : MonoBehaviour
    {
        [SerializeField] private float _updateFrequency = 0.05f;
        private float _timeSinceLastUpdate;
        private bool _isUpdateRequested;
        
        private List<List<MyVector2>> _finalPoly;
        private List<MyVector2> _polyA;
        private List<MyVector2> _polyB;
        
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

        private void UpdateVisuals(List<MyVector2> polyA, List<MyVector2> polyB, List<List<MyVector2>> finalPoly)
        {
            _isUpdateRequested = true;
            _polyA = polyA;
            _polyB = polyB;
            _finalPoly = finalPoly;
        }

        private void UpdateVisuals()
        {
            if(_finalPoly == null || _polyA == null || _polyB == null)
                return;
            
            _polygonConnectionDrawer.UpdateLineRenderers(_polyA, _polyB, _finalPoly);
            _polygonMeshCreator.CreateMeshFromPolyVertexList(_finalPoly);
        }
                
    }
}