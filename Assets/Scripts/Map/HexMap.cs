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
        #region UnityMessage
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

        protected float _radius = 1.0f;

        public void Awake()
        {
            if (!chunkRendererPrefab)
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

        #endregion

        public bool GenerateAt(Vector3 positionInWorld, out Vector2Int chunkPos)
        {
            chunkPos = GetChunkPos(positionInWorld);
            if (Map.ExistChunk(chunkPos)) return false;

            Map.Generate(chunkPos);
            return true;
        }

        public Vector2Int GetChunkPos(Vector3 positionInWorld)
        {
            var pos = transform.InverseTransformPoint(positionInWorld);
            var pos2D = new Vector2(pos.x, pos.z);
            var axial = HexMath.Position2Axial(pos2D, Radius);
            return Map.ToChunkPos(axial.ToOffset());
        }

        public Vector3 GetChunkOrigin(Vector2Int chunkPos)
        {
            var axial = Map.ToCellPos(chunkPos).ToAxial();
            var pos2D = HexMath.Axial2Position(axial) * Radius;
            return transform.TransformPoint(new Vector3(pos2D.x, 0, pos2D.y));
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
            Renderers[chunkPos] = r;
            r.Init(worldPos, Radius, chunkData);
        }

        protected void RemoveChunkGO(Vector2Int chunkPos)
        {
            // 实际情况应该不存在大量反复生成销毁的情形, 因此直接 Instantiate 和 Destroy 就完事了.
            Destroy(Renderers[chunkPos].gameObject);
        }
    }
}
