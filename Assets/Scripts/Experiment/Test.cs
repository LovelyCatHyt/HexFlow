using UnityEngine;
using HexFlow.Map;

public class Test : MonoBehaviour
{
    public HexMap map;

    private Camera _camera;
    // private Transform _mapTran;

    private void Awake()
    {
        if(!map) map = GetComponent<HexMap>();
        _camera = Camera.main;
    }

    private void Update()
    {
        Vector2Int pos;
        if (map.GenerateAt(_camera.transform.position, out pos))
        {
            Debug.Log($"Create chunk at: {pos}");
        }
    }
}
