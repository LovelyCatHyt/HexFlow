using System.Collections;
using System.Collections.Generic;
using HexFlow.NativeCore;
using UnityEngine;

namespace HexFlow.ProceduralMesh
{
    /// <summary>
    /// 六边形区块网格
    /// </summary>
    [ExecuteAlways] // 仅基类标注这个属性是无效的. 以防万一, 下面两个也加上
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HexChunkMesh : ProceduralMeshBase
    {
        /// <summary>
        /// 六边形网格类型
        /// </summary>
        public HexMeshType Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
                    OnValidate();
                }
            }
        }
        [Tooltip("六边形网格类型")]
        [SerializeField]
        protected HexMeshType _type;

        /// <summary>
        /// 半径: 顶点到中心的距离
        /// </summary>
        public float Radiuos
        {
            get => _radiuos;
            set
            {
                value = Mathf.Max(0.001f, value);
                if (value != _radiuos)
                {
                    _radiuos = value;
                    OnValidate();
                }
            }
        }
        [Tooltip("半径: 顶点到中心的距离")]
        [Min(0.001f)]
        [SerializeField]
        protected float _radiuos = 1;

        /// <summary>
        /// 块的尺寸
        /// </summary>
        public Vector2Int ChunkSize
        {
            get => _chunkSize;
            set
            {
                value = Vector2Int.Max(Vector2Int.one, value);
                if (value != _chunkSize)
                {
                    _chunkSize = value;
                    OnValidate();
                }
            }
        }
        [Tooltip("块的尺寸")]
        [SerializeField]
        protected Vector2Int _chunkSize = Vector2Int.one;

        /// <summary>
        /// 起点坐标
        /// </summary>
        public Vector2Int Origin
        {
            get => _origin;
            set
            {
                if (value != _origin)
                {
                    _origin = value;
                    OnValidate();
                }
            }
        }
        [Tooltip("起点坐标")]
        [SerializeField]
        protected Vector2Int _origin = Vector2Int.zero;

        protected override void OnParamCheck()
        {
            _chunkSize = Vector2Int.Max(Vector2Int.one, _chunkSize);
        }

        protected override void UpdateMeshNocheck()
        {
            MeshGenerator.GenerateRectLayout(_chunkSize.x, _chunkSize.y, _origin, _radiuos, _filter.sharedMesh, _type);
            MeshAvailable = true;
        }
    }
}
