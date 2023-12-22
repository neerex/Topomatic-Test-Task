using System.Collections.Generic;
using DataStructures;

namespace Controllers.Interfaces
{
    public interface IPolygonMeshCreator
    {
        void CreateMeshFromPolyVertexList(List<List<MyVector2>> finalPoly);
    }
}