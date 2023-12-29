using DataStructures;
using UnityEngine;

namespace Utility
{
    public static class MathUtility
    {
        public static int ClampListIndex(int index, int listSize) => 
            (index % listSize + listSize) % listSize;
        
        public static float Det2(float x1, float x2, float y1, float y2) => 
            x1 * y2 - y1 * x2;
        
        public static float Det2(MyVector2 a, MyVector2 b) => 
            a.X * b.Y - a.Y * b.X;

        public static float AngleFromToCCW(MyVector2 from, MyVector2 to, bool shouldNormalize = false)
        {
            from = MyVector2.Normalize(from);
            to = MyVector2.Normalize(to);

            float angleRad = AngleBetween(from, to, shouldNormalize = false);

            //The determinant is similar to the dot product
            //The dot product is always 0 no matter in which direction the perpendicular vector is pointing
            //But the determinant is -1 or 1 depending on which way the perpendicular vector is pointing (up or down)
            //AngleBetween goes from 0 to 180 so we can now determine if we need to compensate to get 360 degrees
            if (MathUtility.Det2(from, to) > 0f)
            {
                return angleRad;
            }
            else
            {
                return (Mathf.PI * 2f) - angleRad;
            }
        }
        
        //The angle between two vectors 0 <= angle <= 180
        //Same as Vector2.Angle() but we are using MyVector2
        public static float AngleBetween(MyVector2 from, MyVector2 to, bool shouldNormalize = true)
        {
            //from and to should be normalized
            //But sometimes they are already normalized and then we dont need to do it again
            if (shouldNormalize)
            {
                from = MyVector2.Normalize(from);
                to = MyVector2.Normalize(to);
            }

            //dot(a_normalized, b_normalized) = cos(alpha) -> acos(dot(a_normalized, b_normalized)) = alpha
            float dot = MyVector2.Dot(from, to);

            //This shouldn't happen but may happen because of floating point precision issues
            dot = Mathf.Clamp(dot, -1f, 1f);

            float angleRad = Mathf.Acos(dot);

            return angleRad;
        }
    }
}