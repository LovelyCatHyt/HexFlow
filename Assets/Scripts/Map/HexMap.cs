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

        protected HashSet<Vector2Int> pendingUpdateUVChunks = new HashSet<Vector2Int>();

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
            Map.chunkGenerator = new CopyDefaultGenerator<MapCellData>(Map.defaultValue);

            Renderers = new Dictionary<Vector2Int, ChunkRenderer>();
        }

        private void LateUpdate()
        {
            foreach(var chunkPos in pendingUpdateUVChunks)
            {
                UpdateChunkUV(chunkPos);
            }
            pendingUpdateUVChunks.Clear();
        }

        private void OnDestroy()
        {
            Map.Dispose();
        }

        #endregion


        #region PublicMethod
        public bool GenearteIfNotExist(Vector3 positionInWorld, out Vector2Int chunkPos)
        {
            chunkPos = GetChunkPos(positionInWorld);
            if (Map.ExistChunk(chunkPos)) return false;

            Map.Generate(chunkPos);
            return true;
        }
        public bool GenerateIfNotExist(Vector2Int chunkPos)
        {
            if (Map.ExistChunk(chunkPos)) return false;

            Map.Generate(chunkPos);
            return true;
        }

        public Vector2Int GetCellPos(Vector3 positionInWorld)
        {
            var pos = transform.InverseTransformPoint(positionInWorld);
            var pos2D = new Vector2(pos.x, pos.z);
            var axial = HexMath.Position2Axial(pos2D, Radius);
            return axial.ToOffset();
        }

        public Vector2Int GetChunkPos(Vector3 positionInWorld)
        {
            return Map.ToChunkPos(GetCellPos(positionInWorld));
        }

        public void GetChunkAndCellPos(Vector3 positionInWorld, out Vector2Int chunkPos, out Vector2Int cellPos)
        {
            cellPos = GetCellPos(positionInWorld);
            chunkPos = Map.ToChunkPos(cellPos);
        }

        public Vector3 GetChunkOrigin(Vector2Int chunkPos)
        {
            var axial = Map.ToCellPos(chunkPos).ToAxial();
            var pos2D = HexMath.Axial2Position(axial) * Radius;
            return transform.TransformPoint(new Vector3(pos2D.x, 0, pos2D.y));
        }

        public MapCellData GetData(Vector2Int cellPos)
        {
            var chunkPos = Map.ToChunkPos(cellPos);
            if (!Map.ExistChunk(chunkPos)) return Map.defaultValue;

            var chunkArray = Map.GetChunkAsArray(chunkPos);
            cellPos = TransformMapToChunk(chunkPos, cellPos);
            return chunkArray[cellPos];
        }
        public MapCellData GetData(Vector3 positionInWorld) 
        {
            GetChunkAndCellPos(positionInWorld, out var chunkPos, out var cellPos);
            if (!Map.ExistChunk(chunkPos)) return Map.defaultValue;

            var chunkArray = Map.GetChunkAsArray(chunkPos);
            cellPos = TransformMapToChunk(chunkPos, cellPos);
            return chunkArray[cellPos];
        }

        /// <summary>
        /// 向世界坐标下的格点设置数据, 若该坐标无区块则生成后再设置
        /// </summary>
        public void SetData(Vector3 positionInWorld, MapCellData data, bool updateNeighborChunk = true)
        {
            GenearteIfNotExist(positionInWorld, out var chunkPos);
            var cellPos = GetCellPos(positionInWorld);
            var chunkArr = Map.GetChunkAsArray(chunkPos);
            cellPos = TransformMapToChunk(chunkPos, cellPos);
            chunkArr[cellPos] = data;
            pendingUpdateUVChunks.Add(chunkPos);
            if(updateNeighborChunk)
            {
                if (cellPos.x == 0) pendingUpdateUVChunks.Add(chunkPos - new Vector2Int(1, 0));
                if (cellPos.y == 0) pendingUpdateUVChunks.Add(chunkPos - new Vector2Int(0, 1));
                if (cellPos.x == Map.ChunkSize - 1) pendingUpdateUVChunks.Add(chunkPos + new Vector2Int(1, 0));
                if (cellPos.y == Map.ChunkSize - 1) pendingUpdateUVChunks.Add(chunkPos + new Vector2Int(0, 1));
            }
        }
        public void SetData(Vector2Int cellPos, MapCellData data, bool updateNeighborChunk = true)
        {
            var chunkPos = Map.ToChunkPos(cellPos);
            var chunkArr = Map.GetChunkAsArray(chunkPos);
            cellPos = TransformMapToChunk(cellPos, cellPos);
            chunkArr[cellPos] = data;
            pendingUpdateUVChunks.Add(chunkPos);
            if (updateNeighborChunk)
            {
                if (cellPos.x == 0) pendingUpdateUVChunks.Add(chunkPos - new Vector2Int(1, 0));
                if (cellPos.y == 0) pendingUpdateUVChunks.Add(chunkPos - new Vector2Int(0, 1));
                if (cellPos.x == Map.ChunkSize - 1) pendingUpdateUVChunks.Add(chunkPos + new Vector2Int(1, 0));
                if (cellPos.y == Map.ChunkSize - 1) pendingUpdateUVChunks.Add(chunkPos + new Vector2Int(0, 1));
            }
        }

        public Vector2Int TransformMapToChunk(Vector2Int chunkPos, Vector2Int cellPos)
        {
            var origin = Map.ChunkSize * chunkPos;
            return cellPos - origin;
        }

        public bool RaycastToCell(Ray ray, out Vector2Int cellPos, out Vector3 hitPos)
        {
            var virtualPlane = new Plane(transform.up, transform.position.magnitude);
            if (virtualPlane.Raycast(ray, out var enter))
            {
                hitPos = ray.GetPoint(enter);
                cellPos = GetCellPos(hitPos);
                return true;
            }
            cellPos = new Vector2Int();
            hitPos = ray.GetPoint(0);
            return false;
        }

        #endregion


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
            UpdateChunkUV(chunkPos);
        }

        protected void RemoveChunkGO(Vector2Int chunkPos)
        {
            // 实际情况应该不存在大量反复生成销毁的情形, 因此直接 Instantiate 和 Destroy 就完事了.
            Destroy(Renderers[chunkPos].gameObject);
        }

        protected void UpdateChunkUV(Vector2Int chunkPos)
        {
            if (Renderers.TryGetValue(chunkPos, out var r))
            {
                UVGenerator.GenerateUVForMap(r.Generator.GeneratedMesh, Map, chunkPos, r.meshType, r.textureType);
            }
        }
    }
}
