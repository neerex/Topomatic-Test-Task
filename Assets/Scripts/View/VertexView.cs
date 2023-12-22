using Controllers.Interfaces;
using UnityEngine;
using Utility.UnityUtility.ObjectUtility;

namespace View
{
    [RequireComponent(typeof(DragObject))]
    public class VertexView : MonoBehaviour
    {
        [SerializeField] private DragObject _dragObject;
        private IPolygonClippingController _polygonClippingController;

        public void Initialize(IPolygonClippingController polygonClippingController)
        {
            _polygonClippingController = polygonClippingController;
            
            _dragObject ??= GetComponent<DragObject>();
            _dragObject.OnPositionChanged += RecalculatePolygon;
            
            _polygonClippingController.CalculatePolygons();
        }
        
        private void OnDestroy()
        {
            _polygonClippingController.CalculatePolygons();
            _dragObject.OnPositionChanged -= RecalculatePolygon;
        }

        private void RecalculatePolygon(Vector3 v)
        {
            _polygonClippingController.CalculatePolygons();
        }
    }
}