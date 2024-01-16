using System.Collections.Generic;
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

        public bool Contains(MyVector2 point) => WindingNumber(point) != 0;

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

        private List<Edge2> InitEdges()
        {
            var edges = new List<Edge2>();
            for (int i = 0; i < Count; i++)
            {
                int iNext = (i + 1) % Count;
                var edge = new Edge2(Points[i], Points[iNext]);
                edges.Add(edge);
            }
            return edges;
        }
    }
}
