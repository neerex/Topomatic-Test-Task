using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DataStructures
{
    public class VerticalIntersectingLine
    {
        public readonly Edge2 Line;
        
        private readonly double _x;
        private readonly List<MyVector2> _uniqueIntersections = new();
        private List<MyVector2> _sortedUniqueIntersections = new();

        public IReadOnlyList<MyVector2> SortedUniqueIntersections => _sortedUniqueIntersections.AsReadOnly();
        
        public VerticalIntersectingLine(double minY, double maxY, double x)
        {
            //extend scanning line up and down a bit for future easier intersection calculation 
            _x = x;
            MyVector2 p1 = new MyVector2(x, minY - 1);
            MyVector2 p2 = new MyVector2(x, maxY + 1);
            Line = new Edge2(p1, p2);
        }

        public void PopulateIntersectionCollection(Polygon2 poly)
        {
            foreach (Edge2 edge in poly.Edges)
            {
                if(!CanIntersectWithEdge(edge))
                    continue;

                if (GeometryUtility.IsEdgeContainsEdge(Line, edge))
                {
                    _uniqueIntersections.Add(edge.P1);
                    _uniqueIntersections.Add(edge.P2);
                }
                else if (GeometryUtility.IsEdgeTouchingEdge(Line, edge, out MyVector2 v))
                {
                    if(!_uniqueIntersections.Any(p => p.Equals(v)))
                        _uniqueIntersections.Add(v);
                }
                else if (GeometryUtility.LineSegmentsIntersect(Line, edge, out MyVector2 intersection))
                {
                    if(!_uniqueIntersections.Any(p => p.Equals(intersection)))
                        _uniqueIntersections.Add(intersection);
                }
            }
        }

        //sort from yMax to yMin
        public void SortIntersectionFromYMaxToYMin() => 
            _sortedUniqueIntersections = _uniqueIntersections.OrderByDescending(v => v.Y).ToList();

        private bool CanIntersectWithEdge(Edge2 edge) => 
            edge.P1.X <= _x && edge.P2.X >= _x || edge.P1.X >= _x && edge.P2.X <= _x;
    }
}