using System.Collections.Generic;
using System.Linq;
using DataStructures;
using UnityEditor;
using UnityEngine;
using Utility;

public class PolygonVertexInfoDebugVisualizer : MonoBehaviour
{
    public static PolygonVertexInfoDebugVisualizer Instance;
    private List<List<Vector3>> _finalPoly;

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
        if(_finalPoly == null || _finalPoly.Count == 0)
            return;
    
        foreach (var poly in _finalPoly)
        {
            var set = poly.Distinct().ToList();
            foreach (var v in set)
            {
                Handles.Label(v + Vector3.up * 0.07f, $"{set.IndexOf(v)}");
            }
            // for (int i = 0; i < poly.Count; i++)
            // {
            //     var vertPos = poly[i];
            //     Handles.Label(vertPos + Vector3.up * 0.07f, $"{i}");
            // }
        }
    }
}
