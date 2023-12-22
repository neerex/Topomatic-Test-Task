using Controllers;
using Controllers.Interfaces;
using UI;
using UnityEngine;

namespace Bootstrap
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private PolygonDataProvider _polygonDataProvider;
        [SerializeField] private PolygonManipulationWindow _polygonManipulationWindow;
        [SerializeField] private PolygonConnectionDrawer _polygonConnectionDrawer;

        private void Start()
        {
            IPolygonClippingController polygonClippingController = new PolygonClippingController(_polygonDataProvider);
            IPolygonMeshCreator polygonMeshCreator = new PolygonMeshCreator(polygonClippingController);
            _polygonDataProvider.Initialize(polygonClippingController);

            _polygonConnectionDrawer.Initialize(polygonClippingController);
            PolygonPointController polygonPointController = new PolygonPointController(_polygonDataProvider, polygonClippingController);
            _polygonManipulationWindow.Initialize(polygonClippingController, polygonPointController);
        
            polygonClippingController.CalculatePolygons();
        }
    }
}
