namespace DataStructures
{
    public class Plane2
    {
        public readonly MyVector2 Pos;
        public readonly MyVector2 Normal;
        
        public Plane2(MyVector2 pos, MyVector2 normal)
        {
            Pos = pos;
            Normal = normal;
        }
    }
}