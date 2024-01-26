using System;

namespace DataStructures
{
    public readonly struct Edge2
    {
        public readonly MyVector2 P1;
        public readonly MyVector2 P2;
        
        private readonly double _minX;
        private readonly double _minY;
        private readonly double _maxX;
        private readonly double _maxY;

        public Edge2(MyVector2 p1, MyVector2 p2)
        {
            P1 = p1;
            P2 = p2;
            
            _minX = Math.Min(P1.X, P2.X);
            _maxX = Math.Max(P1.X, P2.X);
            _minY = Math.Min(P1.Y, P2.Y);
            _maxY = Math.Max(P1.Y, P2.Y);
        }

        public bool IsSame(Edge2 edge) => 
            edge.P1.Equals(P1) && edge.P2.Equals(P2) || edge.P1.Equals(P2) && edge.P2.Equals(P1);

        public bool StartsOrEndsWith(MyVector2 p) => 
            P1.Equals(p) || P2.Equals(p);

        public bool HasSameVertexWith(Edge2 edge) => 
            P1.Equals(edge.P1) || P1.Equals(edge.P2) || P2.Equals(edge.P1) || P2.Equals(edge.P2);

        public bool HasSameVertices() => 
            P1.Equals(P2);

        public bool IsPointInEdgeBox(MyVector2 p)
        {
            bool xBound = p.X >= _minX && p.X <= _maxX;
            bool yBound = p.Y >= _minY && p.Y <= _maxY;
            return xBound && yBound;
        }

        public bool NeverIntersectsWith(Edge2 edge)
        {
            MyVector2 p1 = edge.P1;
            MyVector2 p2 = edge.P2;
            
            bool isLeft  = p1.X < _minX && p2.X < _minX;
            bool isRight = p1.X > _maxX && p2.X > _maxX;
            bool isDown  = p1.Y < _minY && p2.Y < _minY;
            bool isUp    = p1.Y > _maxY && p2.Y > _maxY;
            
            return isLeft || isRight || isDown || isUp;
        }
    }
}