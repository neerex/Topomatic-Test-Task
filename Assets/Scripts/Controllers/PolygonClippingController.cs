using System;
using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using GreinerHormannAlgorithm;
using UnityEngine;

namespace Controllers
{
    public delegate void PolygonRecalculationHandler(List<MyVector2> polyA, List<MyVector2> polyB, List<List<MyVector2>> finalPoly);
    
    public class PolygonClippingController : IPolygonClippingController
    {
        private readonly IPolygonProvider _polygonProvider;
        
        private BooleanOperation _operation = BooleanOperation.Intersection;
        private List<MyVector2> _polyA = new();
        private List<MyVector2> _polyB = new();
        private List<List<MyVector2>> _finalPolygon = new();
        private (List<List<ClipVertex2>> polys, List<Polygon2> finalPoly) _drawPoly;
        public event PolygonRecalculationHandler OnPolygonsRecalculation;
        public event Action<(List<List<ClipVertex2>> polys, List<Polygon2> finalPoly)> OnDraw;
        public event Action<HashSet<MyVector2>> OnDraw2;

        public PolygonClippingController(IPolygonProvider polygonProvider)
        {
            _polygonProvider = polygonProvider;
        }

        public void CalculatePolygons()
        {
            _polyA.Clear();
            _polyB.Clear();
            _finalPolygon.Clear();
            
            _polyA = _polygonProvider.GetPolyVertices(PolygonType.A);
            _polyB = _polygonProvider.GetPolyVertices(PolygonType.B);

            Polygon2 polyA = new Polygon2(_polyA);
            Polygon2 polyB = new Polygon2(_polyB);

            HashSet<MyVector2> uniques = PolygonClippingAlgorithm.PolygonClipper(polyA, polyB, BooleanOperation.Intersection);
            OnDraw2?.Invoke(uniques);
            Debug.Log($"{uniques.Count}");
            
            //_drawPoly = PolyClipper.PolygonClipper(polyA, polyB, BooleanOperation.Intersection);
            //OnDraw?.Invoke(_drawPoly);
            //Debug.Log($"{_drawPoly?.First().Count}");
            
            //_finalPolygon = GreinerHormann.ClipPolygons(_polyA, _polyB, _operation);
            OnPolygonsRecalculation?.Invoke(_polyA, _polyB, _finalPolygon);
        }

        public void SetOperation(BooleanOperation operationType)
        {
            if(_operation == operationType)
                return;
            
            _operation = operationType;
            CalculatePolygons();
        }
    }
}