using System;

namespace DataStructures
{
    
    public struct Triangle2
    {
        //Corners
        public MyVector2 P1;
        public MyVector2 P2;
        public MyVector2 P3;

        public Triangle2(MyVector2 p1, MyVector2 p2, MyVector2 p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }
        
        //Change orientation of triangle from cw -> ccw or ccw -> cw
        public void ChangeOrientation() => 
            (P1, P2) = (P2, P1);
        
        public float MinX() => 
            Math.Min(P1.X, Math.Min(P2.X, P3.X));

        public float MaxX() => 
            Math.Max(P1.X, Math.Max(P2.X, P3.X));

        public float MinY() => 
            Math.Min(P1.Y, Math.Min(P2.Y, P3.Y));

        public float MaxY() => 
            Math.Max(P1.Y, Math.Max(P2.Y, P3.Y));
        
        //Find the opposite edge to a vertex
        public Edge2 FindOppositeEdgeToVertex(MyVector2 p)
        {
            if (p.Equals(P1))
                return new Edge2(P2, P3);

            if (p.Equals(P2))
                return new Edge2(P3, P1);

            return new Edge2(P1, P2);
        }
        
        //Check if an edge is a part of this triangle
        public bool IsEdgePartOfTriangle(Edge2 e)
        {
            if ((e.P1.Equals(P1) && e.P2.Equals(P2)) || (e.P1.Equals(P2) && e.P2.Equals(P1)))
                return true;
            if ((e.P1.Equals(P2) && e.P2.Equals(P3)) || (e.P1.Equals(P3) && e.P2.Equals(P2)))
                return true;
            if ((e.P1.Equals(P3) && e.P2.Equals(P1)) || (e.P1.Equals(P1) && e.P2.Equals(P3)))
                return true;

            return false;
        }

        //Find the vertex which is not an edge
        public MyVector2 GetVertexWhichIsNotPartOfEdge(Edge2 e)
        {
            if (!P1.Equals(e.P1) && !P1.Equals(e.P2))
                return P1;
            if (!P2.Equals(e.P1) && !P2.Equals(e.P2))
                return P2;

            return P3;
        }
    }
}