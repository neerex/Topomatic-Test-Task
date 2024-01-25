using System.Collections.Generic;
using System.Linq;
using DataStructures;
using TriangulationAlgorithm.EarClipping;
using UnityEngine;

namespace Utility.UnityUtility.DebugUtility
{
    public class PolygonVertexInfoDebugVisualizer : MonoBehaviour
    {
        public static PolygonVertexInfoDebugVisualizer Instance;
        private List<Triangle2> _finalPoly;
        private List<Mesh> _meshes = new();
        private List<VerticalIntersectingLine> _uniques;
        private List<List<Edge2>> _edges;
        private List<Triangle2> _triangles;

        private void Awake()
        {
            Instance = this;
        }
    
        public void Initialize(List<MyVector2> polya, List<MyVector2> polyb, List<Triangle2> finalpoly) => 
            Initialize(finalpoly);

        private void Initialize(List<Triangle2> finalPoly)
        {
            _finalPoly = finalPoly;
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

            // if (_uniques != null && _uniques.Count != 0)
            // {
            //     foreach (VerticalIntersectingLine line in _uniques)
            //     {
            //         Gizmos.DrawLine(line.Line.P1.ToVector3(), line.Line.P2.ToVector3());
            //         Handles.Label(line.Intersections[0].ToVector3() + Quaternion.AngleAxis(0, Vector3.forward) * Vector3.up * 0.5f, 
            //             $"{_uniques.IndexOf(line)}");
            //
            //         int i = 0;
            //         foreach (MyVector2 p in line.Intersections)
            //         {
            //             Gizmos.DrawSphere(p.ToVector3(), 0.1f);
            //             Handles.Label(p.ToVector3() + Quaternion.AngleAxis(0, Vector3.forward) * Vector3.up * 0.2f, 
            //                 $"{i}");
            //             i++;
            //         }
            //     }
            // }

            // if (_edges != null && _edges.Count != 0)
            // {
            //     var edges = _edges.SelectMany(e => e);
            //     foreach (Edge2 edge in edges)
            //     {
            //         Gizmos.DrawLine(edge.P1.ToVector3(), edge.P2.ToVector3());
            //         Gizmos.DrawSphere(edge.P1.ToVector3(), 0.1f);
            //         Gizmos.DrawSphere(edge.P2.ToVector3(), 0.1f);
            //     }
            // }

            // if (_triangles != null && _triangles.Count != 0)
            // {
            //     foreach (Triangle2 triangle in _triangles)
            //     {
            //         Gizmos.DrawLine(triangle.P1.ToVector3(), triangle.P2.ToVector3());
            //         Gizmos.DrawLine(triangle.P2.ToVector3(), triangle.P3.ToVector3());
            //         Gizmos.DrawLine(triangle.P3.ToVector3(), triangle.P1.ToVector3());
            //     }
            //     var mesh = CreateMeshFromPolyVertexList(_triangles);
            //     Gizmos.color = new Color(0,1,1,0.3f);
            //     Gizmos.DrawMesh(mesh, Vector3.zero);
            // }
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
        
        public Mesh CreateMeshFromPolyVertexList(List<Triangle2> finalPoly)
        {
            _meshes.Clear();
            HashSet<Triangle2> triangulation = finalPoly.ToHashSet();
            Mesh mesh = MeshUtility.Triangles2ToMesh(triangulation, false);
            _meshes.Add(mesh);
            
            CombineInstance[] combine = new CombineInstance[_meshes.Count];

            for (int i = 0; i < _meshes.Count; i++) 
                combine[i].mesh = _meshes[i];

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combine, true, false);
            finalMesh.RecalculateNormals();
            return finalMesh;
        }

        public void Draw2((List<List<Edge2>> list, List<VerticalIntersectingLine> verticalIntersectionLines, List<Triangle2> triangles) tuple)
        {
            _edges = tuple.list;
            _uniques = tuple.verticalIntersectionLines;
            _triangles = tuple.triangles;
        }
    }
}
