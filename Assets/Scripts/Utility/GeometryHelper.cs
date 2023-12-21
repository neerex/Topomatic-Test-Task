using DataStructures;
using UnityEngine;

namespace Utility
{
    public static class GeometryHelper
    {
        public static MyVector2 ToMyVector2(this Vector3 v) => 
            new MyVector2(v.x, v.z);

        public static Vector3 ToVector3(this MyVector2 v, float yPos = 0f) => 
            new Vector3(v.X, yPos, v.Y);
    }
}