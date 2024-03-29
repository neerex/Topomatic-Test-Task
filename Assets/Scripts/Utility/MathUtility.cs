﻿using System;
using DataStructures;
using UnityEngine;

namespace Utility
{
    public static class MathUtility
    {
        public const double Epsilon = 0.000000001d;
        
        public static int ClampListIndex(int index, int listSize) => 
            (index % listSize + listSize) % listSize;
        
        public static float Det2(float x1, float x2, float y1, float y2) => 
            x1 * y2 - y1 * x2;
        
        public static double Det2(double x1, double x2, double y1, double y2) => 
            x1 * y2 - y1 * x2;
        
        public static double Det2(MyVector2 a, MyVector2 b) => 
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

            float dot = (float)MyVector2.Dot(from, to);
            dot = Mathf.Clamp(dot, -1f, 1f);
            float angleRad = Mathf.Acos(dot);
            return angleRad;
        }
        
        //2D "cross product"
        public static double Determinant(MyVector2 a, MyVector2 b) => 
            a.X * b.Y - a.Y * b.X;
        
        public static float SignWithZero(float value, float zeroThreshold = 0.000001f) => 
            Math.Abs(value) < zeroThreshold ? 0 : Sign(value);
        
        public static double SignWithZero(double value, double zeroThreshold = 0.000001f) => 
            Math.Abs(value) < zeroThreshold ? 0 : Sign(value);
        
        public static float Sign(float value) => 
            value >= 0f ? 1 : -1;
        
        public static double Sign(double value) => 
            value >= 0f ? 1 : -1;

        public static bool IsZero(this float value) => 
            Math.Abs(value) <= Epsilon;
        
        public static bool IsZero(this double value) => 
            Math.Abs(value) <= Epsilon;

        public static bool EqualsWithEpsilon(this float self, float value)
        {
            float diff = self - value;
            return diff > -Epsilon && diff < Epsilon;
        }
        
        public static bool EqualsWithEpsilon(this double self, double value)
        {
            double diff = self - value;
            return diff > -Epsilon && diff < Epsilon;
        }

        public static int Mod(this int value, int length) => 
            value >= 0 ? value % length : (value % length + length) % length;
    }
}