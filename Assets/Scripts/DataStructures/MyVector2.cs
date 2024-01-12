﻿using System;
using Utility;

namespace DataStructures
{
    public class MyVector2 : IEquatable<MyVector2>
    {
        public readonly float X;
        public readonly float Y;

        public bool IsIntersection = false;

        public MyVector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public static float SqrMagnitude(MyVector2 a) => 
            a.X * a.X + a.Y * a.Y;

        public static MyVector2 operator +(MyVector2 a, MyVector2 b) => 
            new MyVector2(a.X + b.X, a.Y + b.Y);
        
        public static MyVector2 operator -(MyVector2 a, MyVector2 b) => 
            new MyVector2(a.X - b.X, a.Y - b.Y);
        
        public static MyVector2 operator *(float b, MyVector2 a) => 
            new MyVector2(a.X * b, a.Y * b);
        
        public static MyVector2 operator *(MyVector2 a, float b) => 
            new MyVector2(a.X * b, a.Y * b);
        
        public static float Magnitude(MyVector2 a) => 
            (float)Math.Sqrt(SqrMagnitude(a));

        public static float SqrDistance(MyVector2 a, MyVector2 b) => 
            SqrMagnitude(a - b);
        
        public static float Dot(MyVector2 a, MyVector2 b) => 
            a.X * b.X + a.Y * b.Y;

        public static MyVector2 Normalize(MyVector2 v)
        {
            float magnitude = Magnitude(v);
            MyVector2 normalized = new MyVector2(v.X / magnitude, v.Y / magnitude);
            return normalized;
        }
        
        public bool Equals(MyVector2 v)
        {
            float epsilon = MathUtility.Epsilon;
            return v != null && Math.Abs(v.X - X) <= epsilon && Math.Abs(v.Y - Y) <= epsilon;
        }

        public override string ToString() => 
            $"X:{X} Y:{Y}";
    }
}