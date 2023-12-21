using DataStructures;

namespace GreinerHormannAlgorithm
{
    public class ClipVertex
    {
        public readonly MyVector2 Coordinate;

        //Next and previous vertex in the chain that will form a polygon if we walk around it
        public ClipVertex Next;
        public ClipVertex Prev;

        //We may end up with more than one polygon, and this means we jump to that polygon from this polygon
        public ClipVertex NextPoly;

        //True if this is an intersection vertex
        public bool IsIntersection = false;

        //Is an intersect an entry to a neighbor polygon, otherwise its an exit from a polygon
        public bool IsEntry;

        //If this is an intersection vertex, then this is the same intersection vertex but on the other polygon
        public ClipVertex Neighbor;

        //If this is an intersection vertex, this is how far is is between two vertices that are not intersecting
        public float Alpha = 0f;

        //Is this vertex taken by the final polygon, which is more efficient than removing from a list
        //when we create the final polygon
        public bool IsTakenByFinalPolygon;

        public ClipVertex(MyVector2 coordinate)
        {
            Coordinate = coordinate;
        }
    }
}