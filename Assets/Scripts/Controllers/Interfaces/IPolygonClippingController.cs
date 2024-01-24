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
        event Action<(List<List<Edge2>> list, List<VerticalIntersectingLine> verticalIntersectionLines, List<Triangle2> triangles)> OnDraw2;
        void CalculatePolygons();
        void SetOperation(BooleanOperation operationType);
    }
}