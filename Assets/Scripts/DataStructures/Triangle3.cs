namespace DataStructures
{
    public struct Triangle3
    {
        public MyVector3 P1;
        public MyVector3 P2;
        public MyVector3 P3;

        public Triangle3(MyVector3 p1, MyVector3 p2, MyVector3 p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public void ChangeOrientation() =>
            (P1, P2) = (P2, P1);
    }
}