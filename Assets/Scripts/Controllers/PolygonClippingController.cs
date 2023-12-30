using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Interfaces;
using DataStructures;
using GreinerHormannAlgorithm;
using UnityEngine;
using Utility;
using GeometryUtility = Utility.GeometryUtility;
using Random = UnityEngine.Random;

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
        public event PolygonRecalculationHandler OnPolygonsRecalculation;

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

            _finalPolygon = GreinerHormann.ClipPolygons(_polyA, _polyB, _operation);
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