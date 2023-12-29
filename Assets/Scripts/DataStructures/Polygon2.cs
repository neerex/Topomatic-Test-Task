using System.Collections.Generic;

namespace DataStructures
{
    public struct Polygon2
    {
        public List<MyVector2> vertices;
        
        public Polygon2(List<MyVector2> vertices) => 
            this.vertices = vertices;
    }
}