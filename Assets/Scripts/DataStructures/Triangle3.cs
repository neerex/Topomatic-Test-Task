namespace DataStructures
{
    public struct Triangle3
    {
        //Corners
        public MyVector3 p1;
        public MyVector3 p2;
        public MyVector3 p3;

        public Triangle3(MyVector3 p1, MyVector3 p2, MyVector3 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        //Change orientation of triangle from cw -> ccw or ccw -> cw
        public void ChangeOrientation()
        {
            //Swap two vertices
            (p1, p2) = (p2, p1);
        }
    }
}