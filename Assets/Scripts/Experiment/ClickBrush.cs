using System.Collections.Generic;
using UnityEngine;
using HexFlow.ProceduralMesh;
using HexFlow.NativeCore;

[RequireComponent(typeof(HexChunkMesh))]
public class ClickBrush : MonoBehaviour
{
    [Tooltip("使用盒碰撞体, 否则使用网格碰撞体")]
    public bool useBoxCollider;

    public Color colorLeft = Color.white;
    public Color colorRight = Color.black;

    private HexChunkMesh _chunkMesh;
    private Collider _collider;

    private Array2DAuto<Color> _colorMap;
    // private Vector2Int _lastOrigin = Vector2Int.zero;

    private void Awake()
    {
        _chunkMesh = GetComponent<HexChunkMesh>();
        if (useBoxCollider) _collider = gameObject.AddComponent<BoxCollider>();
        else _collider = gameObject.AddComponent<MeshCollider>();

    }

    private void Start()
    {
        OnMeshUpdated();
        _chunkMesh.OnMeshUpdated.AddListener(OnMeshUpdated);
    }

    private void OnDestroy()
    {
        if (_colorMap != null) _colorMap.Dispose();
    }

    private void OnMeshUpdated()
    {
        RefreshCollider();
        RefreshColorMap();
        ApplyColor();
    }

    private void RefreshColorMap()
    {
        if (_colorMap == null)
        {
            _colorMap = new Array2DAuto<Color>(_chunkMesh.ChunkSize, -_chunkMesh.Origin, colorRight);
        }
        else
        {
            _colorMap.ResetRange(_chunkMesh.Origin, _chunkMesh.Origin + _chunkMesh.ChunkSize - Vector2Int.one);
        }

    }

    private void RefreshCollider()
    {
        if (useBoxCollider && _collider is BoxCollider boxCollider)
        {
            var mesh = _chunkMesh.GeneratedMesh;
            List<Vector3> vertices = new List<Vector3>();
            mesh.GetVertices(vertices);
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            // 直接循环取 min max, 但是理论上可以在生成时就算出来
            foreach (var v in vertices)
            {
                min = Vector3.Min(v, min);
                max = Vector3.Max(v, max);
            }
            boxCollider.center = 0.5f * (min + max);
            Vector3 size2D = max - min;
            boxCollider.size = new Vector3(size2D.x, size2D.y, 0.1f);
        }
        else if (!useBoxCollider && _collider is MeshCollider meshCollider)
        {
            meshCollider.sharedMesh = _chunkMesh.GeneratedMesh;
        }
        else
        {
            Debug.LogError($"Collider intance type conflict with \"useBoxCollider\" settings!");
        }
    }

    private void ApplyColor()
    {
        var mesh = _chunkMesh.GeneratedMesh;
        MeshModifier.SetMeshColor(mesh, _chunkMesh.Type, _colorMap.Data);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var Hit))
            {
                var localPos = transform.InverseTransformPoint(Hit.point);
                var axial = HexMath.Position2Axial(localPos, _chunkMesh.Radiuos);
                var offset = HexMath.Axial2Offset(axial);
                Debug.Log($"Offset position = {offset}");
                _colorMap[offset] = Input.GetMouseButtonDown(0) ? colorLeft : colorRight;
                ApplyColor();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _chunkMesh.Origin += new Vector2Int(-1, 0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _chunkMesh.Origin += new Vector2Int(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _chunkMesh.Origin += new Vector2Int(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _chunkMesh.Origin += new Vector2Int(0, -1);
        }
    }
}
