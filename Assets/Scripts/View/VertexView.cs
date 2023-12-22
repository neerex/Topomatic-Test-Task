using Controllers;
using UnityEngine;

namespace View
{
    [RequireComponent(typeof(DragObject))]
    public class VertexView : MonoBehaviour
    {
        private DragObject _dragObject;

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
            PolygonClippingController.Instance.CalculatePolygons();
        }
    }
}