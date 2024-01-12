using System.Collections.Generic;
using System.Linq;
using DataStructures;
using UnityEditor;
using UnityEngine;

namespace Utility.UnityUtility.DebugUtility
{
    public class PolygonVertexInfoDebugVisualizer : MonoBehaviour
    {
        public static PolygonVertexInfoDebugVisualizer Instance;
        private List<List<Vector3>> _finalPoly;
        private List<Polygon2> _polyToDraw;

        private void Awake()
        {
            Instance = this;
        }
    
        public void Initialize(List<MyVector2> polya, List<MyVector2> polyb, List<List<MyVector2>> finalpoly) => 
            Initialize(finalpoly);

        private void Initialize(List<List<MyVector2>> finalPoly)
        {
            _finalPoly = new List<List<Vector3>>();
            foreach (var poly in finalPoly) 
                _finalPoly.Add(poly.ToListV3());
        }
    
        public void OnDrawGizmos()
        {
            // if (_finalPoly != null && _finalPoly.Count != 0)
            // {
            //     var angle = 0f;
            //     foreach (var poly in _finalPoly)
            //     {
            //         var set = poly.Distinct().ToList();
            //         foreach (var v in set)
            //         {
            //             Handles.Label(v + Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * 0.07f, $"{set.IndexOf(v)}");
            //             angle += 10f;
            //         }
            //     }
            // }

            if (_polyToDraw != null)
            {
                var angle = 45;
                foreach (var polygon2 in _polyToDraw)
                {
                    for (int i = 0; i < polygon2.Count; i++)
                    {
                        MyVector2 p = polygon2[i];
                        Gizmos.color = p.IsIntersection ? Color.red : Color.black;
                        var radius = Gizmos.color == Color.black ? 0.3f : 0.2f;
                        Gizmos.DrawSphere(p.ToVector3() + Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * 0.1f, radius);
                        Handles.Label(p.ToVector3() + Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * 0.3f, $"{i}");
                    }
                    angle += 45;
                }
            }
        }

        public void Draw(List<Polygon2> polyToDraw)
        {
            _polyToDraw = polyToDraw;
        }
    }
}
