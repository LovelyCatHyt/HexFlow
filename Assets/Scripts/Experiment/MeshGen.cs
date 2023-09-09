using System.Collections;
using System.Collections.Generic;
using HexFlow.NativeCore;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGen : MonoBehaviour
{

    public HexMeshType gridType;
    [Min(0.001f)]
    public float cellRadius = 1;
    public Vector2Int origin;
    public Vector2Int gridSize;

    public int numOfVert;

    // private MeshRenderer _renderer;
    private MeshFilter _filter;

    private bool IsInEditor => !Application.isPlaying;

    private void Awake()
    {
#if UNITY_EDITOR
        if (_filter) _filter.sharedMesh = null;
#endif
        Initialize();
    }

    private bool Initialize()
    {
        if (!_filter) _filter = GetComponent<MeshFilter>();
        if (!_filter) return false;
        if (!_filter.sharedMesh) _filter.sharedMesh = new Mesh()
        {
            hideFlags = HideFlags.DontSave,
            name = "GeneratedMesh"
        };
        return true;
    }


    private void OnValidate()
    {
        Initialize();

        gridSize = Vector2Int.Max(gridSize, new Vector2Int(1, 1));

        MeshGenerator.GenerateRectLayout(gridSize.x, gridSize.y, origin, cellRadius, _filter.sharedMesh, gridType);
    }


    private void Reset()
    {
        if(_filter) _filter.sharedMesh = null;
        OnValidate();
        Debug.Log("Reset.");
    }
}
