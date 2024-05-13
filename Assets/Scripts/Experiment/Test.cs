using UnityEngine;
using HexFlow.Map;
using Unitilities.PropAttr;
using Unitilities.Serialization;

public class Test : MonoBehaviour
{
    public HexMap map;
    public string fileNameNoExtend;
    private Camera _camera;
    // private Transform _mapTran;

    private void Awake()
    {
        if (!map) map = GetComponent<HexMap>();
        _camera = Camera.main;
    }

    private void Update()
    {
        Vector2Int pos;
        if (map.GenerateIfNotExist(_camera.transform.position, out pos))
        {
            Debug.Log($"Create chunk at: {pos}");
        }

        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            map.RaycastToCell(ray, out _, out var hitPos);
            var data = map.GetData(hitPos);
            data.enabled = true;
            map.SetData(hitPos, data);

        }
        else if (Input.GetMouseButton(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            map.RaycastToCell(ray, out _, out var hitPos);
            var data = map.GetData(hitPos);
            data.enabled = false;
            map.SetData(hitPos, data);
        }

        if (Input.GetMouseButtonUp(2))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            map.RaycastToCell(ray, out var cellPos, out var hitPos);
            Debug.Log($"mouseup at cellPos: {cellPos}");
        }
    }

    [ButtonInvoke(nameof(IterateMap))]
    public bool iterateMap;

    public void IterateMap()
    {
        var count = 0;
        foreach (var keyValue in map.Map)
        {
            Debug.Log($"position:{keyValue.Key}");
            count++;
        }
        Debug.Log($"Iterate count({count}) == Map.ChunkCount({map.Map.ChunkCount}): {count == map.Map.ChunkCount}");
    }

    [ButtonInvoke(nameof(SaveMap))]
    public bool saveMap;
    public void SaveMap()
    {
        map.Map.SaveTo(fileNameNoExtend, DataScope.Save);
        Debug.Log("Saved");
    }

    [ButtonInvoke(nameof(LoadMap))]
    public bool loadMap;
    public void LoadMap()
    {
        map.Map.LoadFrom(fileNameNoExtend, DataScope.Save);
        Debug.Log("Loaded");
    }
}
