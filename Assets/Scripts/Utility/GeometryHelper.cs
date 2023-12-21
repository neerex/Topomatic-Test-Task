using System.Collections.Generic;
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

        public static List<Vector3> ToListV3(this List<MyVector2> thisPolygon)
        {
            List<Vector3> polygonAfterClipping3D = new List<Vector3>();

            foreach (MyVector2 v in thisPolygon) 
                polygonAfterClipping3D.Add(v.ToVector3());

            return polygonAfterClipping3D;
        }
        
        public static List<MyVector2> ToListMyV2(this List<Vector3> thisPolygon)
        {
            List<MyVector2> polygonAfterClipping2D = new List<MyVector2>();

            foreach (Vector3 v in thisPolygon) 
                polygonAfterClipping2D.Add(v.ToMyVector2());

            return polygonAfterClipping2D;
        }
    }
}