using System.Collections.Generic;
using DataStructures;
using UnityEngine;

namespace Controllers.Interfaces
{
    public interface IPolygonMeshCreator
    {
        Mesh CreateMeshFromPolyVertexList(List<List<MyVector2>> finalPoly);
        Mesh CreateMeshFromTriangleList(List<Triangle2> finalPoly);
    }
}