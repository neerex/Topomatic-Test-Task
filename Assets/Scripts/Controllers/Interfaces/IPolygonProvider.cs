using System.Collections.Generic;
using DataStructures;
using UnityEngine;

namespace Controllers.Interfaces
{
    public interface IPolygonProvider
    {
        List<MyVector2> GetPolyVertices(PolygonType polygonType);
        Transform GetPolygonContainerParent(PolygonType polygonType);
        void Initialize(IPolygonClippingController polygonClippingController);
    }
}