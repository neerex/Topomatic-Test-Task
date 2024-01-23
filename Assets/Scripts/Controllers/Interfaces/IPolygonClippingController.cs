using System;
using System.Collections.Generic;
using DataStructures;
using GreinerHormannAlgorithm;

namespace Controllers.Interfaces
{
    public interface IPolygonClippingController
    {
        event PolygonRecalculationHandler OnPolygonsRecalculation;
        event Action<(List<List<ClipVertex2>> polys, List<Polygon2> finalPoly)> OnDraw;
        event Action<HashSet<MyVector2>> OnDraw2;
        void CalculatePolygons();
        void SetOperation(BooleanOperation operationType);
    }
}