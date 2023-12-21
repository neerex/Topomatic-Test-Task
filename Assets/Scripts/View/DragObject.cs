using System;
using UnityEngine;
using Utility.UnityUtility;

namespace View
{
    public class DragObject : MonoBehaviour
    {
        private Vector3 _mOffset;
        private float _mZCoord;
        private Camera _camera;
        private Vector3 Pos => transform.position;

        public event Action<Vector3> OnPositionChanged;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnMouseDown()
        {
            _mZCoord = _camera.WorldToScreenPoint(Pos).z;
            _mOffset = Pos - GetMouseAsWorldPoint();
        }

        private void OnMouseDrag()
        {
            Vector3 newPoint = GetMouseAsWorldPoint() + _mOffset;
            newPoint = CameraUtility.ClampPointToViewportWithBorder(_camera, newPoint);
            transform.position = newPoint;
            OnPositionChanged?.Invoke(Pos);
        }

        private Vector3 GetMouseAsWorldPoint()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = _mZCoord;
            return _camera.ScreenToWorldPoint(mousePoint);
        }
    }
}
