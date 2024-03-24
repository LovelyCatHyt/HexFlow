using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexFlow.NativeCore.Structures;
using HexFlow.ProceduralMesh;

using MapCellData = HexFlow.NativeCore.Map.MapCellData;
using HexFlow.NativeCore;

namespace HexFlow.Map
{
    [RequireComponent(typeof(HexChunkMesh))]
    public class ChunkRenderer : MonoBehaviour
    {
        public HexMeshType meshType;
        public HexTextureType textureType;

        public HexChunkMesh Generator => _generator;
        private HexChunkMesh _generator;
        private INative2DArray<MapCellData> _data;

        private void Awake()
        {
            _generator = GetComponent<HexChunkMesh>();
        }

        public void Init(Vector3 positionInWorld, float radius, INative2DArray<MapCellData> chunkData)
        {
            _generator.ChunkSize = chunkData.Size;
            _generator.Type = meshType;
            _generator.Radius = radius;
            transform.position = positionInWorld;
            _data = chunkData;
            UpdateMesh();
        }

        /// <summary>
        /// 重置半径
        /// <para>由于区块单元半径会影响区块位置, 因此还需要给出坐标</para>
        /// </summary>
        public void ResetRadius(Vector3 positionInWorld, float radius)
        {
            _generator.Radius = radius;
            transform.position = positionInWorld;
            UpdateMesh();
        }

        /// <summary>
        /// 刷新网格
        /// </summary>
        protected void UpdateMesh()
        {
            MapCellData.ApplyDataToMesh(_data, _generator.GeneratedMesh, _generator.Type);
        }
    }
}
