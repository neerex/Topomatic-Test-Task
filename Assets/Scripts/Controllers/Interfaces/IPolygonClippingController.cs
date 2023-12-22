using DataStructures;

namespace Controllers.Interfaces
{
    public interface IPolygonClippingController
    {
        event PolygonRecalculationHandler OnPolygonsRecalculation;
        void CalculatePolygons();
        void SetOperation(BooleanOperation operationType);
    }
}