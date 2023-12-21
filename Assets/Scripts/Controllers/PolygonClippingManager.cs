using System.Collections.Generic;
using DataStructures;
using GreinerHormannAlgorithm;
using UnityEngine;
using Utility;

namespace Controllers
{
    public class PolygonClippingManager : MonoBehaviour
    {
        [SerializeField] private BooleanOperation _operation = BooleanOperation.Intersection;
        [SerializeField] private Transform _polyAParent;
        [SerializeField] private Transform _polyBParent;

        void OnDrawGizmos()
        {
            //Generate the polygons
            List<Vector3> polygonA = GetVerticesFromParent(_polyAParent);
            List<Vector3> polygonB = GetVerticesFromParent(_polyBParent);

            //3d to 2d
            List<MyVector2> polygonA_2D = new List<MyVector2>();
            List<MyVector2> polygonB_2D = new List<MyVector2>();

            foreach (Vector3 v in polygonA) 
                polygonA_2D.Add(v.ToMyVector2());

            foreach (Vector3 v in polygonB) 
                polygonB_2D.Add(v.ToMyVector2());

            //Display the original polygons
            DisplayPolygon(polygonA, Color.white);
            DisplayPolygon(polygonB, Color.blue);
            
            List<MyVector2> clipPoly = polygonA_2D;
            List<MyVector2> poly = polygonB_2D;
            
            TestGreinerHormann(poly, clipPoly);
        }
        
        private void TestGreinerHormann(List<MyVector2> poly, List<MyVector2> clipPoly)
        {
            List<List<MyVector2>> finalPolygon = GreinerHormann.ClipPolygons(poly, clipPoly, _operation);
            for (int i = 0; i < finalPolygon.Count; i++)
            {
                List<MyVector2> thisPolygon = finalPolygon[i];
                List<Vector3> polygonAfterClipping3D = new List<Vector3>();

                foreach (MyVector2 v in thisPolygon) 
                    polygonAfterClipping3D.Add(v.ToVector3());

                DisplayPolygon(polygonAfterClipping3D, Color.red);
            }
        }
        
        //Display one polygon's vertices and lines between the vertices
        private void DisplayPolygon(List<Vector3> vertices, Color color)
        {
            Gizmos.color = color;

            //Draw the polygons vertices
            float vertexSize = 0.05f;

            for (int i = 0; i < vertices.Count; i++) 
                Gizmos.DrawSphere(vertices[i], vertexSize);

            //Draw the polygons outlines
            for (int i = 0; i < vertices.Count; i++)
            {
                int iPlusOne = MathUtility.ClampListIndex(i + 1, vertices.Count);
                Gizmos.DrawLine(vertices[i], vertices[iPlusOne]);
            }
        }
        
        //Get child vertex positions from parent transform
        private List<Vector3> GetVerticesFromParent(Transform parent)
        {
            int childCount = parent.childCount;
            List<Vector3> children = new List<Vector3>();

            for (int i = 0; i < childCount; i++) 
                children.Add(parent.GetChild(i).position);

            return children;
        }
    }
}