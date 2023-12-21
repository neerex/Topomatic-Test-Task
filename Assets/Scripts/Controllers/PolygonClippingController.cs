using System.Collections.Generic;
using DataStructures;
using GreinerHormannAlgorithm;
using UnityEngine;
using Utility;
using GeometryUtility = Utility.GeometryUtility;

namespace Controllers
{
    public class PolygonClippingController : MonoBehaviour
    {
        [SerializeField] private BooleanOperation _operation = BooleanOperation.Intersection;
        [SerializeField] private Transform _polyAParent;
        [SerializeField] private Transform _polyBParent;

        private List<MyVector2> _polyA;
        private List<MyVector2> _polyB;
        private List<List<MyVector2>> _finalPolygon;

        public void OnDrawGizmos()
        {
            if(_polyA != null) DrawPolygon(_polyA.ToListV3(), Color.white);
            if(_polyB != null) DrawPolygon(_polyB.ToListV3(), Color.blue);
            if(_finalPolygon != null) DrawFinalPolygon(_finalPolygon, Color.red);
        }

        public void CalculatePolygons()
        {
            //Generate the polygons
            _polyA = GetVerticesFromParent(_polyAParent).ToListMyV2();
            _polyB = GetVerticesFromParent(_polyBParent).ToListMyV2();
            
            _finalPolygon = GreinerHormann.ClipPolygons(_polyA, _polyB, _operation);

            Debug.Log(GeometryUtility.PolygonArea(_polyA));
        }

        private List<Vector3> GetVerticesFromParent(Transform parent)
        {
            int childCount = parent.childCount;
            List<Vector3> children = new List<Vector3>();

            for (int i = 0; i < childCount; i++) 
                children.Add(parent.GetChild(i).position);

            return children;
        }

        private void DrawFinalPolygon(List<List<MyVector2>> finalPolygon, Color color)
        {
            for (int i = 0; i < finalPolygon.Count; i++)
            {
                List<MyVector2> thisPolygon = _finalPolygon[i];
                List<Vector3> polygonAfterClipping3D = thisPolygon.ToListV3();

                DrawPolygon(polygonAfterClipping3D, color);
            }
        }

        private void DrawPolygon(List<Vector3> vertices, Color color)
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
    }
}