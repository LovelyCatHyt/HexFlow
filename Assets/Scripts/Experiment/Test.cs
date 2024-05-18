using UnityEngine;
using HexFlow.Map;
using Unitilities.PropAttr;
using Unitilities.Serialization;
using Unitilities.DebugUtil;
using System.IO;

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
    }

    [ButtonInvoke(nameof(IterateMap))]
    public bool iterateMap;

    public void IterateMap()
    {
        Vector2Int[] chkPosArr = map.MapData.GetKeys(true);
        Debug.Log($"Iterate count({chkPosArr.Length}) == Map.ChunkCount({map.MapData.ChunkCount}): {chkPosArr.Length == map.MapData.ChunkCount}");
        Debug.Log(ListPrinter.PrintLines(chkPosArr));
    }

    [ButtonInvoke(nameof(SaveMap))]
    public bool saveMap;
    public void SaveMap()
    {
        map.MapData.SaveTo(fileNameNoExtend, DataScope.Save);
        Debug.Log("Saved");
    }

    [ButtonInvoke(nameof(LoadMap))]
    public bool loadMap;
    public void LoadMap()
    {
        try
        {
            map.MapData.LoadFrom(fileNameNoExtend, DataScope.Save);
            Debug.Log("Loaded");
        }
        catch (FileNotFoundException)
        {
            Debug.Log("File not found!");
        }
    }
}
