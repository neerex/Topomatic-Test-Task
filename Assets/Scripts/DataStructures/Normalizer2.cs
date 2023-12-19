using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public class Normalizer2
    {
        private readonly float _dMax;
        private readonly AABB2 _boundingBox;
        
        public Normalizer2(List<MyVector2> points)
        {
            _boundingBox = new AABB2(points);
            _dMax = CalculateDMax(_boundingBox);
        }
        
        private float CalculateDMax(AABB2 boundingBox)
        {
            float dMax = Mathf.Max(boundingBox.Max.X - boundingBox.Min.X, boundingBox.Max.Y - boundingBox.Min.Y);
            return dMax;
        }
        
        public MyVector2 Normalize(MyVector2 point)
        {
            float x = (point.X - _boundingBox.Min.X) / _dMax;
            float y = (point.Y - _boundingBox.Min.Y) / _dMax;

            MyVector2 pNormalized = new MyVector2(x, y);

            return pNormalized;
        }
        
        public List<MyVector2> Normalize(List<MyVector2> points)
        {
            List<MyVector2> normalizedPoints = new List<MyVector2>();

            foreach (MyVector2 p in points) 
                normalizedPoints.Add(Normalize(p));

            return normalizedPoints;
        }
    }
}