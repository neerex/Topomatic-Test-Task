using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DataStructures
{
    public class VerticalIntersectingLine
    {
        private readonly Edge2 _line;
        private readonly double _x;
        
        private readonly List<MyVector2> _intersections = new();
        public IReadOnlyList<MyVector2> Intersections => _intersections.AsReadOnly();
        
        public VerticalIntersectingLine(double minY, double maxY, double x)
        {
            _x = x;
            
            //extend scanning line up and down a bit for future easier intersection calculation 
            MyVector2 p1 = new MyVector2(x, minY - 1);
            MyVector2 p2 = new MyVector2(x, maxY + 1);
            
            _line = new Edge2(p1, p2);
        }

        public void PopulateIntersectionCollection(Polygon2 poly)
        {
            foreach (Edge2 edge in poly.Edges)
            {
                if(!CanIntersectWithEdge(edge))
                    continue;

                if (GeometryUtility.IsEdgeContainsEdge(_line, edge))
                {
                    _intersections.Add(edge.P1);
                    _intersections.Add(edge.P2);
                }
                else if (GeometryUtility.IsEdgeTouchingEdge(_line, edge, out MyVector2 v))
                {
                    if(!_intersections.Any(p => p.Equals(v)))
                        _intersections.Add(v);
                }
                else if (GeometryUtility.LineSegmentsIntersect(_line, edge, out MyVector2 intersection))
                {
                    if(!_intersections.Any(p => p.Equals(intersection)))
                        _intersections.Add(intersection);
                }
            }
        }

        //sort from yMax to yMin
        public void SortIntersectionFromYMaxToYMin() => 
            _intersections.Sort((v1,v2) => v2.Y.CompareTo(v1.Y));

        private bool CanIntersectWithEdge(Edge2 edge) => 
            edge.P1.X <= _x && edge.P2.X >= _x || edge.P1.X >= _x && edge.P2.X <= _x;
    }
}