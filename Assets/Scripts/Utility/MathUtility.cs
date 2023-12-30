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

            float angleRad = AngleBetween(from, to, false);

            if (Det2(from, to) > 0f)
                return angleRad;

            return Mathf.PI * 2f - angleRad;
        }
        
        //The angle between two vectors 0 <= angle <= 180
        //Same as Vector2.Angle() but we are using MyVector2
        public static float AngleBetween(MyVector2 from, MyVector2 to, bool shouldNormalize = true)
        {
            if (shouldNormalize)
            {
                from = MyVector2.Normalize(from);
                to = MyVector2.Normalize(to);
            }

            float dot = MyVector2.Dot(from, to);
            dot = Mathf.Clamp(dot, -1f, 1f);
            float angleRad = Mathf.Acos(dot);
            return angleRad;
        }
    }
}