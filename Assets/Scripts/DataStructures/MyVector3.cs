using System;

namespace DataStructures
{
    public struct MyVector3
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public MyVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public static MyVector3 Cross(MyVector3 a, MyVector3 b)
        {
            float x = a.Y * b.Z - a.Z * b.Y;
            float y = a.Z * b.X - a.X * b.Z;
            float z = a.X * b.Y - a.Y * b.X;

            MyVector3 crossProduct = new MyVector3(x, y, z);

            return crossProduct;
        }
        
        public static float Magnitude(MyVector3 a) => 
            (float)Math.Sqrt(SqrMagnitude(a));

        public static float SqrMagnitude(MyVector3 a) => 
            a.X * a.X + a.Y * a.Y + a.Z * a.Z;
        
        public override string ToString()
        {
            return $"X:{X} Y:{Y} Z:{Z}";
        }
    }
}