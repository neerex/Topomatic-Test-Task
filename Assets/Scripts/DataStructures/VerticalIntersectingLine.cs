using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DataStructures
{
    public class VerticalIntersectingLine
    {
        public readonly Edge2 Line;
        public readonly float X;
        
        private List<MyVector2> _sortedUniqueIntersections = new();
        private readonly HashSet<MyVector2> _uniqueIntersections = new();

        public IReadOnlyList<MyVector2> SortedUniqueIntersections => _sortedUniqueIntersections.AsReadOnly();
        
        public VerticalIntersectingLine(float minY, float maxY, float x)
        {
            //extend scanning line up and down a bit for future easier intersection calculation 
            X = x;
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
            edge.P1.X <= X && edge.P2.X >= X || edge.P1.X >= X && edge.P2.X <= X;
    }
}