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
        if (map.GenearteIfNotExist(_camera.transform.position, out pos))
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
            
        }else if(Input.GetMouseButton(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            map.RaycastToCell(ray, out _, out var hitPos);
            var data = map.GetData(hitPos);
            data.enabled = false;
            map.SetData(hitPos, data);
        }
    }
}
