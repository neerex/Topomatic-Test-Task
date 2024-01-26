using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;

namespace Utility
{
    public static class GeometryUtility
    {
        public static bool LineSegmentsIntersect(Edge2 edge1, Edge2 edge2, out MyVector2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = new MyVector2();

            var p = edge1.P1;
            var p2 = edge1.P2;
            var q = edge2.P1;
            var q2 = edge2.P2;
            
            MyVector2 r = p2 - p;
            MyVector2 s = q2 - q;
            double rxs = r.Cross(s);
            double qpxr = (q - p).Cross(r);

            if (rxs.IsZero() && qpxr.IsZero())
            {
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
                        return true;
                
                return false;
            }

            if (rxs.IsZero() && !qpxr.IsZero())
                return false;
            
            var t = (q - p).Cross(s) / rxs;
            var u = (q - p).Cross(r) / rxs;

            if (!rxs.IsZero() && t is >= 0 and <= 1 && u is >= 0 and <= 1)
            {
                intersection = p + t * r;
                return true;
            }

            return false;
        }
        
        public static bool LineLine(Edge2 a, Edge2 b, bool includeEndPoints)
        {
            double epsilon = MathUtility.Epsilon;
            bool isIntersecting = false;

            double denominator = (b.P2.Y - b.P1.Y) * (a.P2.X - a.P1.X) - (b.P2.X - b.P1.X) * (a.P2.Y - a.P1.Y);

            //if denominator is 0 the lines are parallel
            if (denominator > epsilon || denominator < -epsilon)
            {
                double u_a = ((b.P2.X - b.P1.X) * (a.P1.Y - b.P1.Y) - (b.P2.Y - b.P1.Y) * (a.P1.X - b.P1.X)) / denominator;
                double u_b = ((a.P2.X - a.P1.X) * (a.P1.Y - b.P1.Y) - (a.P2.Y - a.P1.Y) * (a.P1.X - b.P1.X)) / denominator;
                
                if (includeEndPoints)
                {
                    double zero = -epsilon;
                    double one = 1f + epsilon;
                    
                    if (u_a >= zero && u_a <= one && u_b >= zero && u_b <= one) 
                        isIntersecting = true;
                }
                else
                {
                    double zero = epsilon;
                    double one = 1f - epsilon;
                    
                    if (u_a > zero && u_a < one && u_b > zero && u_b < one) 
                        isIntersecting = true;
                }
            }

            return isIntersecting;
        }

        public static bool LinesParallel(Edge2 a, Edge2 b)
        {
            double epsilon = MathUtility.Epsilon;
            double denominator = (b.P2.Y - b.P1.Y) * (a.P2.X - a.P1.X) - (b.P2.X - b.P1.X) * (a.P2.Y - a.P1.Y);
            return Math.Abs(denominator) <= epsilon;
        }

        public static bool IsEdgeTouchingEdge(Edge2 a, Edge2 touchingEdge, out MyVector2 touchPoint)
        {
            touchPoint = default;
            bool isPoint1 = IsPointOnLine(a, touchingEdge.P1);
            bool isPoint2 = IsPointOnLine(a, touchingEdge.P2);

            if (isPoint1)
                touchPoint = touchingEdge.P1;
            
            if (isPoint2)
                touchPoint = touchingEdge.P2;
            
            return isPoint1 || isPoint2;
        }

        public static bool IsEdgeContainsEdge(Edge2 a, Edge2 containingEdge) => 
            IsPointOnLine(a, containingEdge.P1) && IsPointOnLine(a, containingEdge.P2);

        public static bool IsPointOnLine(Edge2 edge, MyVector2 p)
        {
            if (edge.StartsOrEndsWith(p)) return true;
            
            double epsilon = MathUtility.Epsilon;
            var a_b = edge.P1 - edge.P2;
            var a_p = edge.P1 - p;
            var b_p = edge.P2 - p;
            
            var lenTotal = MyVector2.Magnitude(a_b);
            var lenSum = MyVector2.Magnitude(a_p) + MyVector2.Magnitude(b_p);
            
            return Math.Abs(lenTotal - lenSum) <= epsilon;
        }
        
        public static MyVector2 GetLineLineIntersectionPoint(Edge2 a, Edge2 b)
        {
            double denominator = (b.P2.Y - b.P1.Y) * (a.P2.X - a.P1.X) - (b.P2.X - b.P1.X) * (a.P2.Y - a.P1.Y);
            double u_a = ((b.P2.X - b.P1.X) * (a.P1.Y - b.P1.Y) - (b.P2.Y - b.P1.Y) * (a.P1.X - b.P1.X)) / denominator;
            MyVector2 intersectionPoint = a.P1 + u_a * (a.P2 - a.P1);
            return intersectionPoint;
        }

        //Gauss area formula
        public static double PolygonArea(IReadOnlyList<MyVector2> polygonPoints)
        {
            //formula working correctly if the initial poly is in +X +Y quadrant
            //so we move every point on dV, where dV is opposite of Vec2(minX, minY)
            MyVector2 dV = GetVectorForMovingPolyIntoPlusXPlusYArea(polygonPoints);
            MyVector2 firstP = polygonPoints[0] + dV;
            MyVector2 lastP = polygonPoints[^1] + dV;
            
            double area = lastP.X * firstP.Y - firstP.X * lastP.Y;
            int len = polygonPoints.Count;

            for (int i = 0; i < len - 1; i++)
            {
                MyVector2 p1 = polygonPoints[i] + dV;
                MyVector2 p2 = polygonPoints[i+1] + dV;
                area += p1.X * p2.Y;
            }
            
            for (int i = 1; i < len; i++)
            {
                MyVector2 p1 = polygonPoints[i] + dV;
                MyVector2 p2 = polygonPoints[i-1] + dV;
                area -= p1.X * p2.Y;
            }

            double totalArea = 0.5f * Math.Abs(area);
            return totalArea;
            
            //formula for getting dV for moving poly into +X +Y area
            MyVector2 GetVectorForMovingPolyIntoPlusXPlusYArea(IReadOnlyList<MyVector2> polygonPoints)
            {
                double minX = polygonPoints.Min(p => p.X);
                double minY = polygonPoints.Min(p => p.Y);

                return new MyVector2(Math.Abs(minX), Math.Abs(minY));
            }
        }

        public static bool IsTriangleOrientedClockwise(MyVector2 p1, MyVector2 p2, MyVector2 p3)
        {
            bool isClockWise = true;
            double determinant = p1.X * p2.Y + p3.X * p1.Y + p2.X * p3.Y - p1.X * p3.Y - p3.X * p2.Y - p2.X * p1.Y;
            
            if (determinant > 0f) 
                isClockWise = false;
            
            return isClockWise;
        }

        public static bool PointTriangle(Triangle2 t, MyVector2 p, bool includeBorder)
        {
            //To avoid floating point precision issues we can add a small value
            float epsilon = float.Epsilon;

            //Based on Barycentric coordinates
            double denominator = (t.P2.Y - t.P3.Y) * (t.P1.X - t.P3.X) + (t.P3.X - t.P2.X) * (t.P1.Y - t.P3.Y);

            double a = ((t.P2.Y - t.P3.Y) * (p.X - t.P3.X) + (t.P3.X - t.P2.X) * (p.Y - t.P3.Y)) / denominator;
            double b = ((t.P3.Y - t.P1.Y) * (p.X - t.P3.X) + (t.P1.X - t.P3.X) * (p.Y - t.P3.Y)) / denominator;
            double c = 1 - a - b;

            bool isWithinTriangle = false;

            if (includeBorder)
            {
                float zero = 0f - epsilon;
                float one = 1f + epsilon;

                //The point is within the triangle or on the border
                if (a >= zero && a <= one && b >= zero && b <= one && c >= zero && c <= one) 
                    isWithinTriangle = true;
            }
            else
            {
                float zero = 0f + epsilon;
                float one = 1f - epsilon;

                //The point is within the triangle
                if (a > zero && a < one && b > zero && b < one && c > zero && c < one) 
                    isWithinTriangle = true;
            }

            return isWithinTriangle;
        }
        
        public static bool ShouldFlipEdge(MyVector2 a, MyVector2 b, MyVector2 c, MyVector2 d)
        {
            bool shouldFlipEdge = false;

            //Use the circle test to test if we need to flip this edge
            //We should flip if d is inside a circle formed by a, touchingEdge, c
            IntersectionCases intersectionCases = PointCircle(a, b, c, d);

            if (intersectionCases == IntersectionCases.IsInside)
            {
                //Are these the two triangles forming a convex quadrilateral? Otherwise the edge cant be flipped
                if (IsQuadrilateralConvex(a, b, c, d))
                {
                    //If the new triangle after a flip is not better, then dont flip
                    //This will also stop the algorithm from ending up in an endless loop
                    IntersectionCases intersectionCases2 = PointCircle(b, c, d, a);

                    if (intersectionCases2 == IntersectionCases.IsOnEdge || intersectionCases2 == IntersectionCases.IsInside)
                    {
                        shouldFlipEdge = false;
                    }
                    else
                    {
                        shouldFlipEdge = true;
                    }
                }
            }
            return shouldFlipEdge;
        }
        
        public static IntersectionCases PointCircle(MyVector2 a, MyVector2 b, MyVector2 c, MyVector2 testPoint)
        {
            //Center of circle
            MyVector2 circleCenter = CalculateCircleCenter(a, b, c);

            //The radius sqr of the circle
            double radiusSqr = MyVector2.SqrDistance(a, circleCenter);

            //The distance sqr from the point to the circle center
            double distPointCenterSqr = MyVector2.SqrDistance(testPoint, circleCenter);
            
            //Add/remove a small value becuse we will never be exactly on the edge because of floating point precision issues
            //Mutiply epsilon by two because we are using sqr root???
            if (distPointCenterSqr < radiusSqr - float.Epsilon * 2f)
                return IntersectionCases.IsInside;

            if (distPointCenterSqr > radiusSqr + float.Epsilon * 2f)
                return IntersectionCases.NoIntersection;

            return IntersectionCases.IsOnEdge;
        }
        
        public static bool IsQuadrilateralConvex(MyVector2 a, MyVector2 b, MyVector2 c, MyVector2 d)
        {
            bool isConvex = false;

            bool abc = IsTriangleOrientedClockwise(a, b, c);
            bool abd = IsTriangleOrientedClockwise(a, b, d);
            bool bcd = IsTriangleOrientedClockwise(b, c, d);
            bool cad = IsTriangleOrientedClockwise(c, a, d);

            if (abc && abd && bcd & !cad)
                isConvex = true;
            else if (abc && abd && !bcd & cad)
                isConvex = true;
            else if (abc && !abd && bcd & cad)
                isConvex = true;
            //The opposite sign, which makes everything inverted
            else if (!abc && !abd && !bcd & cad)
                isConvex = true;
            else if (!abc && !abd && bcd & !cad)
                isConvex = true;
            else if (!abc && abd && !bcd & !cad) 
                isConvex = true;

            return isConvex;
        }
        
        public static MyVector2 CalculateCircleCenter(MyVector2 a, MyVector2 b, MyVector2 c)
        {
            //Make sure the triangle a-touchingEdge-c is counterclockwise
            if (!IsTriangleOrientedClockwise(a, b, c)) 
                (a, b) = (b, a);

            //The area of the triangle
            double X_1 = b.X - a.X;
            double X_2 = c.X - a.X;
            double Y_1 = b.Y - a.Y;
            double Y_2 = c.Y - a.Y;

            double A = 0.5f * MathUtility.Det2(X_1, Y_1, X_2, Y_2);

            double L_10_square = MyVector2.SqrMagnitude(b - a);
            double L_20_square = MyVector2.SqrMagnitude(c - a);

            double one_divided_by_4A = 1f / (4f * A);

            double x = a.X + one_divided_by_4A * ((Y_2 * L_10_square) - (Y_1 * L_20_square));
            double y = a.Y + one_divided_by_4A * ((X_1 * L_20_square) - (X_2 * L_10_square));

            MyVector2 center = new MyVector2(x, y);

            return center;
        }
    }
}