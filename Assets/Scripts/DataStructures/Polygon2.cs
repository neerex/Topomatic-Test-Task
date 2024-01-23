using System.Collections.Generic;
using System.Linq;
using GreinerHormannAlgorithm;
using Utility;

namespace DataStructures
{
    public class Polygon2
    {
        public readonly IReadOnlyList<MyVector2> Points;
        public readonly IReadOnlyList<Edge2> Edges;

        public Polygon2(IReadOnlyList<MyVector2> points)
        {
            Points = points;
            Edges = InitEdges();
        }

        public MyVector2 this[int i] => Points[i.Mod(Count)];
        public Edge2 GetEdgeAt(int i) => Edges[i.Mod(Edges.Count)];
        
        public int Count => Points.Count;
        public bool IsClockwise => SignedArea > 0;

        public bool IsPointOnAnyEdge(MyVector2 point) => Edges.Any(e => GeometryUtility.IsPointOnLine(e, point));
        public bool Contains(MyVector2 point) => WindingNumber(point) != 0;

        public float MinY() => Points.Min(p => p.Y);
        public float MaxY() => Points.Max(p => p.Y);

        private float SignedArea 
        {
            get 
            {
                float sum = 0f;
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
            
            float IsLeft( MyVector2 a, MyVector2 b, MyVector2 p ) => 
                MathUtility.SignWithZero(MathUtility.Determinant(a.To(p), a.To(b)));
        }

        public bool IsSelfIntersecting()
        {
            for (int i = 0; i < Edges.Count - 1; i++)
            {
                for (int j = i + 1; j < Edges.Count; j++)
                {
                    var edge1 = Edges[i];
                    var edge2 = Edges[j];
                    if (GeometryUtility.LineLine(edge1, edge2, false))
                        return true;
                }
            }
            return false;
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
