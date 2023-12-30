using DataStructures;

namespace TriangulationAlgorithm.EarClipping
{
    public class LinkedVertex
    {
        public readonly MyVector2 Pos;

        public LinkedVertex PrevLinkedVertex;
        public LinkedVertex NextLinkedVertex;

        public LinkedVertex(MyVector2 pos)
        {
            Pos = pos;
        }
    }
}
