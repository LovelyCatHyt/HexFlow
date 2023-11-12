using System;
using System.Collections.Generic;
using UnityEngine;
using HexFlow.NativeCore;
using HexFlow.NativeCore.Map;
using HexFlow.NativeCore.Structures;

namespace HexFlow.Map
{
    public class HexMap : MonoBehaviour
    {
        public const int ChunkSize = 32;

        [SerializeField] protected ChunkRenderer chunkRendererPrefab;

        public Chunked2DContainer<MapCellData> Map { get; protected set; }
        public Dictionary<Vector2Int, ChunkRenderer> Renderers { get; protected set; }

        public float Radius
        {
            get => _radius; 
            set
            {
                _radius = Mathf.Max(0, value);
                foreach (var r in Renderers)
                {
                    r.Value.ResetRadius(GetChunkOrigin(r.Key), _radius);
                }
            }
        }

        protected float _radius;

        public void Awake()
        {
            if(!chunkRendererPrefab)    
            {
                Debug.LogError("Chunk renderer prefab missing! Will throw NRE when creating chunks.");
            }

            Map = new Chunked2DContainer<MapCellData>(default, (int)(Time.time * 1000), ChunkSize);
            Map.onBeforeChunkRemoved += OnChunkRemoved;
            Map.onChunkCreated += OnChunkCreated;

            Renderers = new Dictionary<Vector2Int, ChunkRenderer>();
        }

        private void OnDestroy()
        {
            Map.Dispose();
        }

        protected void OnChunkCreated(Vector2Int chunkPos, IntPtr chunkData)
        {
            CreateChunkGO(chunkPos, new ChunkDataProxy<MapCellData>(chunkData, Map.ChunkSize));
        }

        protected void OnChunkRemoved(Vector2Int chunkPos, IntPtr chunkData)
        {
            Destroy(Renderers[chunkPos].gameObject);
        }

        protected void CreateChunkGO(Vector2Int chunkPos, INative2DArray<MapCellData> chunkData)
        {
            Vector3 worldPos = GetChunkOrigin(chunkPos);

            // 实际情况应该不存在大量反复生成销毁的情形, 因此直接 Instantiate 和 Destroy 就完事了.
            var go = Instantiate(chunkRendererPrefab.gameObject, transform);
            go.name = $"chunk {chunkPos}";
            var r = go.GetComponent<ChunkRenderer>();
            r.Init(worldPos, Radius, chunkData);
        }

        public Vector3 GetChunkOrigin(Vector2Int chunkPos)
        {
            var axial = Map.ToCellPos(chunkPos).ToAxial();
            return transform.TransformPoint(HexMath.Axial2Position(axial) * Radius); 
        }

        protected void RemoveChunkGO(Vector2Int chunkPos)
        {
            // 实际情况应该不存在大量反复生成销毁的情形, 因此直接 Instantiate 和 Destroy 就完事了.
            Destroy(Renderers[chunkPos].gameObject);
        }
    }
}
