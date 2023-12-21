using Controllers;
using UnityEngine;

namespace View
{
    [RequireComponent(typeof(DragObject))]
    public class VertexView : MonoBehaviour
    {
        [SerializeField] private PolygonClippingController _polygonClippingController;
        private DragObject _dragObject;
        //private PolygonClippingController _polygonClippingController;

        public void Initialize(PolygonClippingController polygonClippingController)
        {
            _polygonClippingController = polygonClippingController;
        }
        
        private void Awake()
        {
            _dragObject = GetComponent<DragObject>();
        }

        private void OnEnable()
        {
            _dragObject.OnPositionChanged += RecalculatePolygon;
        }

        private void OnDisable()
        {
            _dragObject.OnPositionChanged -= RecalculatePolygon;
        }

        private void RecalculatePolygon(Vector3 v)
        {
            _polygonClippingController.CalculatePolygons();
        }
    }
}