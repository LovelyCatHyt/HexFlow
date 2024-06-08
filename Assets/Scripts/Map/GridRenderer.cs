using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HexFlow.Map
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField] private HexMap _map;
        private Material _material;
        private Renderer _renderer;

        private readonly int OriginPropID = Shader.PropertyToID("_Origin");
        private readonly int MousePropID = Shader.PropertyToID("_Mouse");
        private readonly int SizePropID = Shader.PropertyToID("_Size");

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
        }

        private void Start()
        {
            Vector3 mapOrigin = _map.GetChunkOrigin(Vector2Int.zero);
            _material.SetVector(OriginPropID, mapOrigin);
            _material.SetFloat(SizePropID, _map.Radius);
            var keyword = _material.shader.keywordSpace.FindKeyword("");
            _material.SetKeyword(in keyword, true);
        }

        private void Update()
        {
            // 定位的基本假设: 材质基于世界坐标系计算, 地图所在平面平行于 XoZ

            var oldPos = transform.position;
            var mapPlane = new Plane(Vector3.up, oldPos.y);
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float hit;
            mapPlane.Raycast(mouseRay, out hit);
            var hitPos = mouseRay.GetPoint(hit);

            transform.position = new Vector3(hitPos.x, oldPos.y, hitPos.z);
            _material.SetVector(MousePropID, new Vector4(hitPos.x, hitPos.y, hitPos.z));
        }
    }

}
