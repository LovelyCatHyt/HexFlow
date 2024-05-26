using UnityEngine;
using HexFlow.Map;
using Unitilities.PropAttr;
using Unitilities.Serialization;
using Unitilities.DebugUtil;
using System.IO;
using HexFlow.NativeCore.Map;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{
    public HexMap map;
    public string fileNameNoExtend;
    
    [Space]
    public Vector2Int startChunk;
    public Vector2Int endChunk;

    [Space]
    [Min(0)]
    public float threshold = 1;
    public Vector2 noiseScale = Vector2.one;
    public Vector2 noiseOffset = Vector2.zero;
    public int waveNum = 4;

    private Camera _camera;
    private bool _shouldGenerate = false;
    // private Transform _mapTran;

    private void Awake()
    {
        if (!map) map = GetComponent<HexMap>();
        map.MapData.chunkGenerator = new PerlinNoiseTerrainGenerator(noiseScale, noiseOffset, waveNum, threshold);

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
        }else if (Input.GetMouseButton(2))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            map.RaycastToCell(ray, out var hitCell, out var _);
            map.GenerateIfNotExist(map.GetChunkPos(hitCell));
        }

        if(_shouldGenerate)
        {
            _shouldGenerate = false;
            map.Generate(startChunk, endChunk);
        }
    }

    private void OnValidate()
    {
        if(UnityEditor.EditorApplication.isPlaying && map && map.MapData != null)
        {
            map.MapData.chunkGenerator = new PerlinNoiseTerrainGenerator(noiseScale, noiseOffset, waveNum, threshold);
        }
    }

    [ButtonInvoke(nameof(GenerateArea))]
    public bool regenerate;

    public void GenerateArea()
    {
        _shouldGenerate = true;
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
        Stopwatch stopwatch = Stopwatch.StartNew();
        map.MapData.SaveTo(fileNameNoExtend, DataScope.Save);
        stopwatch.Stop();
        Debug.Log($"Saved: <color=#aaff55>{stopwatch.ElapsedMilliseconds}</color> ms");
    }

    [ButtonInvoke(nameof(LoadMap))]
    public bool loadMap;
    public void LoadMap()
    {
        try
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            map.MapData.LoadFrom(fileNameNoExtend, DataScope.Save);
            stopwatch.Stop();
            Debug.Log($"Loaded: <color=#aaff55>{stopwatch.ElapsedMilliseconds}</color> ms");
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("File not found!");
        }
    }
}
