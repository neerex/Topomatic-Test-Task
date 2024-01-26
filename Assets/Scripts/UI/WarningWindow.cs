using Controllers.Interfaces;
using DataStructures;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WarningWindow : MonoBehaviour
    {
        [SerializeField] private float _warningMessageLifeTime = 1f;
        [SerializeField] private TextMeshProUGUI _tmp;
        
        private IPolygonClippingController _polygonClippingController;
        private float _warningTimeLeft = 1f;

        public void Initialize(IPolygonClippingController polygonClippingController)
        {
            _polygonClippingController = polygonClippingController;
            _tmp.gameObject.SetActive(false);
            Subscribe();
        }

        private void Update()
        {
            if (_warningTimeLeft >= 0)
            {
                _warningTimeLeft -= Time.deltaTime;
                return;
            }
            _tmp.gameObject.SetActive(false);
        }

        private void Subscribe()
        {
            _polygonClippingController.OnSelfIntersection += ShowWarning;
        }

        private void ShowWarning(PolygonType polyType)
        {
            _tmp.gameObject.SetActive(true);
            _warningTimeLeft = _warningMessageLifeTime;
            _tmp.text = $"Polygon{polyType} is self intersecting";
        }
    }
}
