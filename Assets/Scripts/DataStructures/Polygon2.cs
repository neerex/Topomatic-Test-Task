using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DataStructures
{
    public class Polygon2
    {
        public readonly IReadOnlyList<MyVector2> Points;
        public readonly IReadOnlyList<Edge2> Edges;
        public readonly IReadOnlyList<Triangle2> Triangles;

        public Polygon2(IReadOnlyList<MyVector2> points)
        {
            Points = points;
            Edges = InitEdges();
        }
        
        public Polygon2(IReadOnlyList<Triangle2> triangles)
        {
            Triangles = triangles;
        }

        private double SignedArea 
        {
            get 
            {
                double sum = 0f;
                for( int i = 0; i < Count; i++ )
                {
                    int iNext = (i + 1) % Count;
                    MyVector2 a = Points[i];
                    MyVector2 b = Points[iNext];
                    sum += (b.X - a.X) * (b.Y + a.Y);
                }
                return sum * 0.5f;
            }
        }

        public MyVector2 this[int i] => Points[i.Mod(Count)];

        public int Count => Points.Count;

        public bool IsClockwise => SignedArea > 0;

        public bool Contains(MyVector2 point) => WindingNumber(point) != 0;

        public double MinY() => Points.Min(p => p.Y);

        public double MaxY() => Points.Max(p => p.Y);

        private int WindingNumber(MyVector2 point) 
        {
            int winding = 0;
            for(int i = 0; i < Count; i++) 
            {
                int iNext = (i + 1) % Count;
                if(Points[i].Y <= point.Y) 
                {
                    if(Points[iNext].Y > point.Y && IsLeft(Points[i], Points[iNext], point) > 0)
                        winding--;
                } 
                else 
                {
                    if(Points[iNext].Y <= point.Y && IsLeft(Points[i], Points[iNext], point ) < 0)
                        winding++;
                }
            }
            
            return winding;
            
            double IsLeft(MyVector2 a, MyVector2 b, MyVector2 p) => 
                MathUtility.SignWithZero(MathUtility.Determinant(a.To(p), a.To(b)));
        }

        public bool IsSelfIntersecting()
        {
            for (int i = 0; i < Edges.Count - 1; i++)
            {
                Edge2 edge1 = Edges[i];
                for (int j = i + 1; j < Edges.Count; j++)
                {
                    Edge2 edge2 = Edges[j];
                    
                    if(edge1.NeverIntersectsWith(edge2))
                        continue;
                    
                    if(edge1.SameEdge(edge2))
                        return true;
                    
                    if(GeometryUtility.IsEdgeTouchingEdge(edge1, edge2, out _))
                        continue;

                    if (GeometryUtility.LineLine(edge1, edge2, false))
                        return true;
                }
            }
            return false;
        }

        public bool ContainsPolygon(Polygon2 poly)
        {
            foreach (Edge2 edge in poly.Edges)
                if (Edges.Any(e => GeometryUtility.LineLine(e, edge, false)))
                    return false;

            return true;
        }

        private List<Edge2> InitEdges()
        {
            List<Edge2> edges = new List<Edge2>();
            for (int i = 0; i < Count; i++)
            {
                int iNext = (i + 1) % Count;
                Edge2 edge = new Edge2(Points[i], Points[iNext]);
                edges.Add(edge);
            }
            return edges;
        }
    }
}
