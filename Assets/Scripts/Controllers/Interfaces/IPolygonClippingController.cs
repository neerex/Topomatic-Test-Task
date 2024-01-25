using System;
using System.Collections.Generic;
using DataStructures;

namespace Controllers.Interfaces
{
    public interface IPolygonClippingController
    {
        event PolygonRecalculationHandler OnPolygonsRecalculation;
        event Action<List<Triangle2>> OnDraw;
        void CalculatePolygons();
        void SetOperation(BooleanOperation operationType);
    }
}