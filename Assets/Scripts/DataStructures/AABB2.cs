using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public struct AABB2
    {
        public readonly MyVector2 min;
        public readonly MyVector2 max;
        
        //We know the min and max values
        public AABB2(float minX, float maxX, float minY, float maxY)
        {
            min = new MyVector2(minX, minY);
            max = new MyVector2(maxX, maxY);
        }
        
        //We have a list with points and want to find the min and max values
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
                {
                    minX = p.X;
                }
                else if (p.X > maxX)
                {
                    maxX = p.X;
                }

                if (p.Y < minY)
                {
                    minY = p.Y;
                }
                else if (p.Y > maxY)
                {
                    maxY = p.Y;
                }
            }

            this.min = new MyVector2(minX, minY);
            this.max = new MyVector2(maxX, maxY);
        }



        //Check if the rectangle is a rectangle and not flat in any dimension
        public bool IsRectangleARectangle()
        {
            float xWidth = Mathf.Abs(max.X - min.X);
            float yWidth = Mathf.Abs(max.Y - min.Y);

            float epsilon = float.Epsilon;

            if (xWidth < epsilon || yWidth < epsilon)
            {
                return false;
            }

            return true;
        }
    }
}