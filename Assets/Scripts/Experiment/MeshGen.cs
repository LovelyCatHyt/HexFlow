using System.Collections;
using System.Collections.Generic;
using HexFlow.NativeCore;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGen : MonoBehaviour
{
    public float cellRadius;
    public Vector2Int origin;
    public Vector2Int gridSize;

    public int numOfVert;

    private MeshRenderer _renderer;
    private MeshFilter _filter;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();
        _filter.mesh = new Mesh()
        {
            hideFlags = HideFlags.DontSave,
            name = "GeneratedMesh"
        };
        _filter.mesh.MarkDynamic();

    }

    private void Update()
    {
        MeshGenerator.GenerateRectLayout(gridSize.x, gridSize.y, origin, cellRadius, _filter.mesh);
        numOfVert = _filter.mesh.vertexCount;
    }
}
