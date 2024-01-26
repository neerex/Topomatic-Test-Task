using System;
using DataStructures;

namespace Controllers.Interfaces
{
    public interface IPolygonClippingController
    {
        void CalculatePolygons();
        void SetOperation(BooleanOperation operationType);
        event PolygonRecalculationHandler OnPolygonsRecalculation;
        event Action<PolygonType> OnSelfIntersection;
    }
}