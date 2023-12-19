using System.Collections.Generic;

namespace DataStructures
{
    //Axis Aligned Bounding Box 2D
    public struct AABB2
    {
        public readonly MyVector2 Min;
        public readonly MyVector2 Max;
        
        public AABB2(List<MyVector2> points)
        {
            MyVector2 p1 = points[0];

            float minX = p1.X;
            float maxX = p1.X;
            float minY = p1.Y;
            float maxY = p1.Y;

            for (int i = 1; i < points.Count; i++)
            {
                MyVector2 p = points[i];

                if (p.X < minX)
                    minX = p.X;
                else if (p.X > maxX) 
                    maxX = p.X;

                if (p.Y < minY)
                    minY = p.Y;
                else if (p.Y > maxY) 
                    maxY = p.Y;
            }

            Min = new MyVector2(minX, minY);
            Max = new MyVector2(maxX, maxY);
        }
    }
}