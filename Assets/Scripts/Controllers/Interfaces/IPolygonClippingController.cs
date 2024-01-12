using System;
using System.Collections.Generic;
using DataStructures;
using Unity.VisualScripting;

namespace Controllers.Interfaces
{
    public interface IPolygonClippingController
    {
        event PolygonRecalculationHandler OnPolygonsRecalculation;
        event Action<List<Polygon2>> OnDraw;
        void CalculatePolygons();
        void SetOperation(BooleanOperation operationType);
    }
}