using UnityEngine;
using Utility.UnityUtility;
using View;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class PolygonPointController : MonoBehaviour
    {
        [SerializeField] private Transform _polyAParent;
        [SerializeField] private Transform _polyBParent;

        [SerializeField] private VertexView _polyAVertexView;
        [SerializeField] private VertexView _polyBVertexView;

        private Camera _camera;
        public static PolygonPointController Instance { get; private set; }
        
        public void Awake()
        {
            Instance = this;
            _camera = Camera.main;
        }

        public void AddVertex(PolygonType polygonType)
        {
            VertexView view = polygonType switch
            {
                PolygonType.A => _polyAVertexView,
                PolygonType.B => _polyBVertexView
            };

            Transform parent = polygonType switch
            {
                PolygonType.A => _polyAParent,
                PolygonType.B => _polyBParent
            };

            Vector3 randomVector = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
            Vector3 randomPoint = parent.GetChild(0).position + randomVector;
            Vector3 clampedPoint = CameraUtility.ClampPointToViewportWithBorder(_camera, randomPoint);

            Instantiate(view, clampedPoint, Quaternion.identity, parent);
            PolygonClippingController.Instance.CalculatePolygons();
        }

        public void RemoveVertex(PolygonType polygonType)
        {
            Transform parent = polygonType switch
            {
                PolygonType.A => _polyAParent,
                PolygonType.B => _polyBParent
            };
            RemoveLastChild(parent);
            PolygonClippingController.Instance.CalculatePolygons();
        }

        private void RemoveLastChild(Transform parent)
        {
            int childCount = parent.childCount;
            
            if(childCount == 3) 
                return;
            
            Transform lastChild = parent.GetChild(childCount - 1);
            Destroy(lastChild.gameObject);
        }
    }

    public enum PolygonType
    {
        A = 0,
        B = 1
    }
}