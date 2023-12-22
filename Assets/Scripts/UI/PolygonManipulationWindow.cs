using System;
using System.Collections.Generic;
using System.Globalization;
using Controllers;
using Controllers.Interfaces;
using DataStructures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GeometryUtility = Utility.GeometryUtility;

namespace UI
{
    public class PolygonManipulationWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _areaAText;
        [SerializeField] private TextMeshProUGUI _areaBText;

        [SerializeField] private Button _addVertexToAButton;
        [SerializeField] private Button _addVertexToBButton;
    
        [SerializeField] private Button _removeVertexFromAButton;
        [SerializeField] private Button _removeVertexFromBButton;

        [SerializeField] private Button _unionButton;
        [SerializeField] private Button _intersectButton;
        
        private IPolygonClippingController _polygonClippingController;
        private IPolygonPointController _polygonPointController;

        public void Initialize(IPolygonClippingController polygonClippingController, IPolygonPointController polygonPointController)
        {
            _polygonClippingController = polygonClippingController;
            _polygonPointController = polygonPointController;

            Subscribe();
        }

        private void Subscribe()
        {
            _addVertexToAButton.onClick.AddListener(() => AddVertex(PolygonType.A));
            _addVertexToBButton.onClick.AddListener(() => AddVertex(PolygonType.B));
        
            _removeVertexFromAButton.onClick.AddListener(() => RemoveVertex(PolygonType.A));
            _removeVertexFromBButton.onClick.AddListener(() => RemoveVertex(PolygonType.B));
        
            _unionButton.onClick.AddListener(() => SetNewPolygonCalculationOperation(BooleanOperation.Union));
            _intersectButton.onClick.AddListener(() => SetNewPolygonCalculationOperation(BooleanOperation.Intersection));

            _polygonClippingController.OnPolygonsRecalculation += ShowPolygonAreaCalculation;
        }
        
        private void ShowPolygonAreaCalculation(List<MyVector2> polyA, List<MyVector2> polyB, List<List<MyVector2>> finalPoly)
        {
            float areaA = (float)Math.Round(GeometryUtility.PolygonArea(polyA), 3);
            float areaB = (float)Math.Round(GeometryUtility.PolygonArea(polyB), 3);

            _areaAText.text = areaA.ToString(CultureInfo.InvariantCulture);
            _areaBText.text = areaB.ToString(CultureInfo.InvariantCulture);
        }

        private void AddVertex(PolygonType polygonType) => 
            _polygonPointController.AddVertex(polygonType);

        private void RemoveVertex(PolygonType polygonType) => 
            _polygonPointController.RemoveVertex(polygonType);

        private void SetNewPolygonCalculationOperation(BooleanOperation booleanOperation) => 
            _polygonClippingController.SetOperation(booleanOperation);
    }
}
