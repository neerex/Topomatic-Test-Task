namespace Controllers.Interfaces
{
    public interface IPolygonPointController
    {
        void AddVertex(PolygonType polygonType);
        void RemoveVertex(PolygonType polygonType);
    }
}