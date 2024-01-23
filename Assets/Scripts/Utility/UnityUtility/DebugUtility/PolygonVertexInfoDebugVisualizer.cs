using System.Collections.Generic;
using System.Linq;
using DataStructures;
using GreinerHormannAlgorithm;
using TriangulationAlgorithm.EarClipping;
using UnityEditor;
using UnityEngine;

namespace Utility.UnityUtility.DebugUtility
{
    public class PolygonVertexInfoDebugVisualizer : MonoBehaviour
    {
        public static PolygonVertexInfoDebugVisualizer Instance;
        private List<List<Vector3>> _finalPoly;
        private (List<List<ClipVertex2>> polys, List<Polygon2> finalPoly) _polyToDraw;
        private List<Mesh> _meshes = new();
        private HashSet<MyVector2> _uniques;

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

            // if (_polyToDraw.polys != null)
            // {
            //     var angle = 45;
            //     foreach (List<ClipVertex2> poly in _polyToDraw.polys)
            //     {
            //         GUIStyle style = new GUIStyle();
            //         style.normal.textColor = _polyToDraw.polys.IndexOf(poly) % 2 == 0 ? Color.blue : Color.green;
            //         
            //         for (int i = 0; i < poly.Count; i++)
            //         {
            //             MyVector2 p = poly[i].Coord;
            //             
            //             Gizmos.color = Color.black;
            //
            //             if (poly[i].IsIntersection && poly[i].IsEntering)
            //                 Gizmos.color = Color.yellow;
            //             
            //             if (poly[i].IsIntersection && !poly[i].IsEntering)
            //                 Gizmos.color = Color.magenta;
            //
            //             if (poly[i].IsOnOtherPolygonEdge)
            //                 Gizmos.color = Color.cyan;
            //             
            //             if (poly[i].IsOnTheVertexOfOtherPolygon)
            //                 Gizmos.color = Color.red;
            //             
            //             var radius = Gizmos.color == Color.black ? 0.1f : 0.05f;
            //             Gizmos.DrawSphere(p.ToVector3() + Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * 0.1f, radius);
            //             Handles.Label(p.ToVector3() + Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * 0.3f, $"{i}", style);
            //         }
            //         angle += 45;
            //     }
            // }
            //
            // if (_polyToDraw.finalPoly != null && _polyToDraw.finalPoly.Count != 0)
            // {
            //     //Debug.Log($"Total final polys {_polyToDraw.finalPoly.Count}");
            //     var polys = new List<List<MyVector2>>();
            //     foreach (Polygon2 poly in _polyToDraw.finalPoly) 
            //         polys.Add(poly.Points.ToList());
            //
            //     var mesh = CreateMeshFromPolyVertexList(polys);
            //     Gizmos.color = new Color(0,1,1,0.5f);
            //     Gizmos.DrawMesh(mesh, Vector3.zero);
            // }

            if (_uniques != null && _uniques.Count != 0)
            {
                foreach (MyVector2 v in _uniques)
                {
                    Gizmos.DrawSphere(v.ToVector3(), 0.2f);
                }
            }
        }

        public void Draw((List<List<ClipVertex2>> polys, List<Polygon2> finalPoly) polyToDraw)
        {
            _polyToDraw = polyToDraw;
        }
        
        public Mesh CreateMeshFromPolyVertexList(List<List<MyVector2>> finalPoly)
        {
            _meshes.Clear();
            foreach (var poly in finalPoly)
            {
                HashSet<Triangle2> triangulation = EarClipping.Triangulate(poly.Distinct().ToList(), false);
                
                if (triangulation == null)
                    continue;

                Mesh mesh = MeshUtility.Triangles2ToMesh(triangulation, false);
                _meshes.Add(mesh);
            }
            
            CombineInstance[] combine = new CombineInstance[_meshes.Count];

            for (int i = 0; i < _meshes.Count; i++) 
                combine[i].mesh = _meshes[i];

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combine, true, false);
            finalMesh.RecalculateNormals();
            return finalMesh;
        }

        public void Draw2(HashSet<MyVector2> set)
        {
            _uniques = set;
        }
    }
}
