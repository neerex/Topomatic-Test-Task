using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;

namespace Utility
{
    public static class GeometryUtility
    {
        public static bool LineLine(Edge2 a, Edge2 b, bool includeEndPoints)
        {
            float epsilon = float.Epsilon;
            bool isIntersecting = false;

            float denominator = (b.P2.Y - b.P1.Y) * (a.P2.X - a.P1.X) - (b.P2.X - b.P1.X) * (a.P2.Y - a.P1.Y);

            //if denominator is 0 the lines are parallel
            if (denominator > epsilon || denominator < -epsilon)
            {
                float u_a = ((b.P2.X - b.P1.X) * (a.P1.Y - b.P1.Y) - (b.P2.Y - b.P1.Y) * (a.P1.X - b.P1.X)) / denominator;
                float u_b = ((a.P2.X - a.P1.X) * (a.P1.Y - b.P1.Y) - (a.P2.Y - a.P1.Y) * (a.P1.X - b.P1.X)) / denominator;
                
                float zero = -epsilon;
                float one = 1f + epsilon;
                
                if (includeEndPoints)
                {
                    if (u_a >= zero && u_a <= one && u_b >= zero && u_b <= one) 
                        isIntersecting = true;
                }
                else
                {
                    if (u_a > zero && u_a < one && u_b > zero && u_b < one) 
                        isIntersecting = true;
                }
            }

            return isIntersecting;
        }
        
        public static MyVector2 GetLineLineIntersectionPoint(Edge2 a, Edge2 b)
        {
            float denominator = (b.P2.Y - b.P1.Y) * (a.P2.X - a.P1.X) - (b.P2.X - b.P1.X) * (a.P2.Y - a.P1.Y);
            float u_a = ((b.P2.X - b.P1.X) * (a.P1.Y - b.P1.Y) - (b.P2.Y - b.P1.Y) * (a.P1.X - b.P1.X)) / denominator;
            MyVector2 intersectionPoint = a.P1 + u_a * (a.P2 - a.P1);
            return intersectionPoint;
        }
        
        //Is a point intersecting with a polygon?
        //The list describing the polygon has to be sorted either clockwise or counter-clockwise
        public static bool PointPolygon(List<MyVector2> polygonPoints, MyVector2 point)
        {
            //Step 1. Find a point outside of the polygon
            //Pick a point with a x position larger than the polygons max x position, which is always outside
            MyVector2 maxXPosVertex = polygonPoints[0];

            for (int i = 1; i < polygonPoints.Count; i++)
                if (polygonPoints[i].X > maxXPosVertex.X)
                    maxXPosVertex = polygonPoints[i];

            //The point should be outside so just pick a number to move it outside
            //Should also move it up a little to minimize floating point precision issues
            //This is where it fails if this line is exactly on a vertex
            MyVector2 pointOutside = maxXPosVertex + new MyVector2(1f, 0.01f);
            
            //Step 2. Create an edge between the point we want to test with the point thats outside
            MyVector2 l1_p1 = point;
            MyVector2 l1_p2 = pointOutside;
            
            //Step 3. Find out how many edges of the polygon this edge is intersecting with
            int numberOfIntersections = 0;

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                //Line 2
                int iPlusOne = MathUtility.ClampListIndex(i + 1, polygonPoints.Count);
                MyVector2 l2_p1 = polygonPoints[i];
                MyVector2 l2_p2 = polygonPoints[iPlusOne];

                //Are the lines intersecting?
                if (LineLine(new Edge2(l1_p1, l1_p2), new Edge2(l2_p1, l2_p2), includeEndPoints: true))
                    numberOfIntersections++;
            }

            //Step 4. Is the point inside or outside?
            //The point is outside the polygon if number of intersections is even or 0
            bool isInside = !(numberOfIntersections == 0 || numberOfIntersections % 2 == 0);
            
            return isInside;
        }

        //Gauss area formula
        public static float PolygonArea(List<MyVector2> polygonPoints)
        {
            //formula working correctly if the initial poly is in +X +Y quadrant
            //so we move every point on dV, where dV is Vec2(minX, minY)
            MyVector2 dV = GetVectorForMovingPolyIntoPlusXPlusYArea(polygonPoints);
            MyVector2 firstP = polygonPoints[0] + dV;
            MyVector2 lastP = polygonPoints[^1] + dV;
            
            float area = lastP.X * firstP.Y - firstP.X * lastP.Y;
            int len = polygonPoints.Count;

            for (int i = 0; i < len - 2; i++)
            {
                MyVector2 p1 = polygonPoints[i] + dV;
                MyVector2 p2 = polygonPoints[i+1] + dV;
                area += p1.X * p2.Y;
            }
            
            for (int i = 1; i < len - 1; i++)
            {
                MyVector2 p1 = polygonPoints[i] + dV;
                MyVector2 p2 = polygonPoints[i-1] + dV;
                area -= p1.X * p2.Y;
            }

            float totalArea = 0.5f * Math.Abs(area);
            return totalArea;
        }

        //formula for getting dV for moving poly into +X +Y area
        public static MyVector2 GetVectorForMovingPolyIntoPlusXPlusYArea(List<MyVector2> polygonPoints)
        {
            float minX = polygonPoints.Min(p => p.X);
            float minY = polygonPoints.Min(p => p.Y);

            return new MyVector2(Math.Abs(minX), Math.Abs(minY));
        }
    }
}