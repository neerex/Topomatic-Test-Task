using System.Collections.Generic;
using DataStructures;
using GreinerHormannAlgorithm;
using UnityEngine;

namespace Controllers
{
    public delegate void PolygonRecalculationHandler(List<MyVector2> polyA, List<MyVector2> polyB, List<List<MyVector2>> finalPoly);
    
    public class PolygonClippingController : MonoBehaviour
    {
        private BooleanOperation _operation = BooleanOperation.Intersection;
        private List<MyVector2> _polyA;
        private List<MyVector2> _polyB;
        private List<List<MyVector2>> _finalPolygon;

        public static PolygonClippingController Instance { get; private set; }
        public event PolygonRecalculationHandler OnPolygonsRecalculation;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            CalculatePolygons();
        }
        
        public void CalculatePolygons()
        {
            //Generate the polygons
            _polyA = PolygonProvider.Instance.GetPolyVertices(PolygonType.A);
            _polyB = PolygonProvider.Instance.GetPolyVertices(PolygonType.B);
            
            _finalPolygon = GreinerHormann.ClipPolygons(_polyA, _polyB, _operation);
            PolygonMeshCreator.Instance.CreateMeshFromPolyVertexList(_finalPolygon);
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