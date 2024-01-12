using Controllers;
using Controllers.Interfaces;
using UI;
using UnityEngine;
using Utility.UnityUtility.DebugUtility;

namespace Bootstrap
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private PolygonDataProvider _polygonDataProvider;
        [SerializeField] private PolygonManipulationWindow _polygonManipulationWindow;
        [SerializeField] private PolygonConnectionDrawer _polygonConnectionDrawer;
        [SerializeField] private PolygonViewUpdater _polygonViewUpdater;
        
        private void Start()
        {
            IPolygonClippingController polygonClippingController = new PolygonClippingController(_polygonDataProvider);
            IPolygonMeshCreator polygonMeshCreator = new PolygonMeshCreator();
            
            _polygonViewUpdater.Initialize(polygonClippingController, polygonMeshCreator, _polygonConnectionDrawer);
            _polygonDataProvider.Initialize(polygonClippingController);
            
            PolygonPointController polygonPointController = new PolygonPointController(_polygonDataProvider, polygonClippingController);
            _polygonManipulationWindow.Initialize(polygonClippingController, polygonPointController);

            polygonClippingController.OnPolygonsRecalculation += PolygonVertexInfoDebugVisualizer.Instance.Initialize;
            polygonClippingController.OnDraw += PolygonVertexInfoDebugVisualizer.Instance.Draw;
            
            polygonClippingController.CalculatePolygons();
        }
    }
}
