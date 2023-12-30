using System.Collections.Generic;
using DataStructures;
using UnityEngine;

namespace Utility
{
    public static class GeometryHelper
    {
        public static MyVector2 ToMyVector2(this Vector3 v) => 
            new MyVector2(v.x, v.y);

        public static Vector3 ToVector3(this MyVector2 v, float zPos = 0f) => 
            new Vector3(v.X, v.Y, zPos);

        public static Vector2 ToVector2(this MyVector2 v) => 
            new Vector2(v.X, v.Y);
        
        public static MyVector3 ToMyVector3(this MyVector2 v, float zPos = 0f) => 
            new MyVector3(v.X, v.Y, zPos);
        
        public static MyVector2 ToMyVector2(this MyVector3 v) => 
            new MyVector2(v.X, v.Y);

        public static Vector3 Edge2ToV3(this Edge2 edge, float zPos = 0)
        {
            MyVector2 p1 = edge.P1;
            MyVector2 p2 = edge.P2;
            MyVector2 v = p2 - p1;
            return new Vector3(v.X, v.Y, zPos);
        }
        
        public static MyVector3 Edge2ToMyV3(this Edge2 edge, float zPos = 0)
        {
            MyVector2 p1 = edge.P1;
            MyVector2 p2 = edge.P2;
            MyVector2 v = p2 - p1;
            return new MyVector3(v.X, v.Y, zPos);
        }

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