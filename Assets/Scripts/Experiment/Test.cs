using HexFlow.NativeCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Test : MonoBehaviour
{
    private void Update()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (!meshCollider.sharedMesh) meshCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonUp(0))
        {
            
            if (meshCollider.Raycast(ray, out var info, 114514f))
            {
                Debug.Log($"mouse hit at {info.point}");
            }
        }
    }
}
