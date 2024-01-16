using System;
using DataStructures;

namespace GreinerHormannAlgorithm
{
    public class ClipVertex2 : IEquatable<ClipVertex2>
    {
        public readonly MyVector2 Coord;
        
        public bool IsIntersection;
        public bool IsEntering;
        public ClipVertex2 Next;
        public ClipVertex2 Prev;
        public ClipVertex2 SameVertexInOtherPoly;
        public bool IsOnOtherPolygonEdge;

        public ClipVertex2(MyVector2 coord)
        {
            Coord = coord;
        }

        public bool Equals(ClipVertex2 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Coord, other.Coord);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Coord);
        }
    }
}