using System;
using Utility;

namespace DataStructures
{
    public readonly struct MyVector2 : IEquatable<MyVector2>
    {
        public readonly double X;
        public readonly double Y;

        public MyVector2(double x, double y)
        {
            X = x;
            Y = y;
        }
        
        public double Cross(MyVector2 v) => 
            X * v.Y - Y * v.X;
        
        public static double SqrMagnitude(MyVector2 a) => 
            a.X * a.X + a.Y * a.Y;

        public static double Magnitude(MyVector2 a) => 
            (double)Math.Sqrt(SqrMagnitude(a));

        public static double SqrDistance(MyVector2 a, MyVector2 b) => 
            SqrMagnitude(a - b);
        
        public static double Dot(MyVector2 a, MyVector2 b) => 
            a.X * b.X + a.Y * b.Y;

        public static MyVector2 Normalize(MyVector2 v)
        {
            double magnitude = Magnitude(v);
            MyVector2 normalized = new MyVector2(v.X / magnitude, v.Y / magnitude);
            return normalized;
        }
        
        public static MyVector2 operator +(MyVector2 a, MyVector2 b) => 
            new MyVector2(a.X + b.X, a.Y + b.Y);
        
        public static MyVector2 operator -(MyVector2 a, MyVector2 b) => 
            new MyVector2(a.X - b.X, a.Y - b.Y);
        
        public static MyVector2 operator *(double b, MyVector2 a) => 
            new MyVector2(a.X * b, a.Y * b);
        
        public static MyVector2 operator *(MyVector2 a, double b) => 
            new MyVector2(a.X * b, a.Y * b);
        
        public static MyVector2 operator /(MyVector2 a, double b) => 
            new MyVector2(a.X / b, a.Y / b);
        
        public static double operator *(MyVector2 v, MyVector2 w) => 
            v.X * w.X + v.Y * w.Y;
        
        public bool Equals(MyVector2 v)
        {
            double epsilon = MathUtility.Epsilon;
            return Math.Abs(v.X - X) <= epsilon && Math.Abs(v.Y - Y) <= epsilon;
        }
        
        public override int GetHashCode() => 
            HashCode.Combine(X, Y);

        public override string ToString() => 
            $"X:{X} Y:{Y}";
    }
}