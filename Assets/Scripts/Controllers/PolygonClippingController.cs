using System;
using System.Collections.Generic;
using Controllers.Interfaces;
using DataStructures;
using PolygonClipper;

namespace Controllers
{
    public delegate void PolygonRecalculationHandler(Polygon2 polyA, Polygon2 polyB, List<Triangle2> finalPoly);
    
    public class PolygonClippingController : IPolygonClippingController
    {
        private readonly IPolygonProvider _polygonProvider;
        
        private BooleanOperation _operation = BooleanOperation.Intersection;
        private List<MyVector2> _polyA = new();
        private List<MyVector2> _polyB = new();
        private List<Triangle2> _finalPolygon = new();
        public event PolygonRecalculationHandler OnPolygonsRecalculation;
        public event Action<PolygonType> OnSelfIntersection;

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

            if (polyA.IsSelfIntersecting()) OnSelfIntersection?.Invoke(PolygonType.A);
            if (polyB.IsSelfIntersecting()) OnSelfIntersection?.Invoke(PolygonType.B);

            _finalPolygon = PolygonClippingAlgorithm.PolygonClipper(polyA, polyB, _operation);
            
            OnPolygonsRecalculation?.Invoke(polyA, polyB, _finalPolygon);
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