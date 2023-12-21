namespace DataStructures
{
    public class MyVector2
    {
        public readonly float X;
        public readonly float Y;

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

    }
}