using System;
using UnityEngine;

namespace Utility.UnityUtility.ObjectUtility
{
    public class DragObject : MonoBehaviour
    {
        private Vector3 _offset;
        private float _zCoord;
        private Camera _camera;
        private Vector3 Pos => transform.position;

        public event Action<Vector3> OnPositionChanged;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnMouseDown()
        {
            _zCoord = _camera.WorldToScreenPoint(Pos).z;
            _offset = Pos - GetMouseAsWorldPoint();
        }

        private void OnMouseDrag()
        {
            Vector3 newPoint = GetMouseAsWorldPoint() + _offset;
            newPoint = CameraUtility.CameraUtility.ClampPointToViewportWithBorder(_camera, newPoint);
            transform.position = newPoint;
            OnPositionChanged?.Invoke(Pos);
        }

        private Vector3 GetMouseAsWorldPoint()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = _zCoord;
            return _camera.ScreenToWorldPoint(mousePoint);
        }
    }
}
