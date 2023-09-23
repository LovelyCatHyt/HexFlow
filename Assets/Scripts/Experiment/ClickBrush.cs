using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexFlow.ProceduralMesh;
using HexFlow.NativeCore;

[RequireComponent(typeof(HexChunkMesh))]
public class ClickBrush : MonoBehaviour
{
    MeshFilter _filter;
    HexChunkMesh _chunkMesh;

    private void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _chunkMesh = GetComponent<HexChunkMesh>();
        Debug.Log($"Mesh vertex count: {_chunkMesh.GeneratedMesh.vertexCount}", this);
    }

    private void Start()
    {
        
    }

    private void OnMouseUpAsButton()
    {
        // TODO: Click to color
    }
}
