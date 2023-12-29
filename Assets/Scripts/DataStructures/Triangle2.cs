using System;

namespace DataStructures
{
    
    public struct Triangle2
    {
        //Corners
        public MyVector2 p1;
        public MyVector2 p2;
        public MyVector2 p3;

        public Triangle2(MyVector2 p1, MyVector2 p2, MyVector2 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
        
        //Change orientation of triangle from cw -> ccw or ccw -> cw
        public void ChangeOrientation() => 
            (p1, p2) = (p2, p1);
        
        public float MinX() => 
            Math.Min(p1.X, Math.Min(p2.X, p3.X));

        public float MaxX() => 
            Math.Max(p1.X, Math.Max(p2.X, p3.X));

        public float MinY() => 
            Math.Min(p1.Y, Math.Min(p2.Y, p3.Y));

        public float MaxY() => 
            Math.Max(p1.Y, Math.Max(p2.Y, p3.Y));
        
        //Find the opposite edge to a vertex
        public Edge2 FindOppositeEdgeToVertex(MyVector2 p)
        {
            if (p.Equals(p1))
                return new Edge2(p2, p3);

            if (p.Equals(p2))
                return new Edge2(p3, p1);

            return new Edge2(p1, p2);
        }
        
        //Check if an edge is a part of this triangle
        public bool IsEdgePartOfTriangle(Edge2 e)
        {
            if ((e.P1.Equals(p1) && e.P2.Equals(p2)) || (e.P1.Equals(p2) && e.P2.Equals(p1)))
                return true;
            if ((e.P1.Equals(p2) && e.P2.Equals(p3)) || (e.P1.Equals(p3) && e.P2.Equals(p2)))
                return true;
            if ((e.P1.Equals(p3) && e.P2.Equals(p1)) || (e.P1.Equals(p1) && e.P2.Equals(p3)))
                return true;

            return false;
        }

        //Find the vertex which is not an edge
        public MyVector2 GetVertexWhichIsNotPartOfEdge(Edge2 e)
        {
            if (!p1.Equals(e.P1) && !p1.Equals(e.P2))
                return p1;
            if (!p2.Equals(e.P1) && !p2.Equals(e.P2))
                return p2;

            return p3;
        }
    }
}