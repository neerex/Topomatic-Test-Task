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
        private List<VerticalIntersectingLine> _uniques;

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
                foreach (VerticalIntersectingLine line in _uniques)
                {
                    Gizmos.DrawLine(line.Line.P1.ToVector3(), line.Line.P2.ToVector3());
                    Handles.Label(line.SortedUniqueIntersections[0].ToVector3() + Quaternion.AngleAxis(0, Vector3.forward) * Vector3.up * 0.5f, 
                        $"{_uniques.IndexOf(line)}");
                    foreach (MyVector2 p in line.SortedUniqueIntersections)
                    {
                        Gizmos.DrawSphere(p.ToVector3(), 0.1f);
                        // Handles.Label(p.ToVector3() + Quaternion.AngleAxis(0, Vector3.forward) * Vector3.up * 0.2f, 
                        //     $"{line.SortedUniqueIntersections}");
                    }
                    //Debug.Log($"line{_uniques.IndexOf(line)} | {string.Join(" ", line._sortedUniqueIntersections.Select(v => v.Y))}");
                }
            }
        }

        public Color GetRandomColor()
        {
            return new Color(Random.value, Random.value, Random.value);
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

        public void Draw2(List<VerticalIntersectingLine> set)
        {
            _uniques = set;
        }
    }
}
