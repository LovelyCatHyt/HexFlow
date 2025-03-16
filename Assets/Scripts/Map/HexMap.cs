using System;
using System.Collections.Generic;
using UnityEngine;
using HexFlow.NativeCore;
using HexFlow.NativeCore.Map;
using HexFlow.NativeCore.Structures;
using Unitilities.Serialization;
using System.IO;
using System.Linq;

namespace HexFlow.Map
{
    public class HexMap : MonoBehaviour
    {
        public const int ChunkSize = 32;

        [SerializeField] protected ChunkRenderer chunkRendererPrefab;

        public Chunked2DContainer<MapCellData> MapData { get; protected set; }
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

            MapData = new Chunked2DContainer<MapCellData>(default, (int)(Time.time * 1000), ChunkSize);
            MapData.onBeforeChunkRemoved += OnChunkRemoved;
            MapData.onChunkCreated += OnChunkCreated;
            MapData.onChunkRegen += OnChunkRegenerate;
            MapData.chunkGenerator = new CopyDefaultGenerator<MapCellData>(MapData.defaultValue);

            Renderers = new Dictionary<Vector2Int, ChunkRenderer>();
        }


        private void LateUpdate()
        {

            // 对每个待更新 chunk 再扫描一下八邻域的已创建 chunk，如果存在则再追加到待更新集合中
            var scanSourceList = pendingUpdateUVChunks.ToList();
            foreach (var src in scanSourceList)
            {
                foreach (var deltaChkPos in HexMath.Rect8Dirs)
                {
                    var nbrChunk = src + deltaChkPos;
                    if (MapData.ExistChunk(nbrChunk))
                    {
                        pendingUpdateUVChunks.Add(nbrChunk);
                    }
                }

            }

            // 用 Linq 复制一份坐标数组, 因为在生成过程可能会添加坐标(尽管就是参数里的同一个坐标)
            foreach (var chunkPos in pendingUpdateUVChunks.ToArray())
            {
                UpdateChunkUVImmiediate(chunkPos);
            }
            pendingUpdateUVChunks.Clear();
        }

        private void OnDestroy()
        {
            MapData.Dispose();
        }

        #endregion


        #region PublicMethod

        public void Generate(Vector2Int chunkPos)
        {
            MapData.Generate(chunkPos);
            // 创建时也要注意周围区块的更新
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx != 0 && dy != 0)
                    {
                        UpdateChunkUV(chunkPos + new Vector2Int(dx, dy));
                    }
                }
            }
        }

        public void Generate(Vector2Int startPos, Vector2Int endPos)
        {
            MapData.Generate(startPos, endPos);
        }

        public bool GenerateIfNotExist(Vector2Int chunkPos)
        {
            if (MapData.ExistChunk(chunkPos)) return false;

            Generate(chunkPos);
            return true;
        }
        public bool GenerateIfNotExist(Vector3 positionInWorld, out Vector2Int chunkPos)
        {
            chunkPos = GetChunkPos(positionInWorld);
            return GenerateIfNotExist(chunkPos);
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
            return MapData.ToChunkPos(GetCellPos(positionInWorld));
        }

        public Vector2Int GetChunkPos(Vector2Int cellPos)
        {
            return MapData.ToChunkPos(cellPos);
        }

        public void GetChunkAndCellPos(Vector3 positionInWorld, out Vector2Int chunkPos, out Vector2Int cellPos)
        {
            cellPos = GetCellPos(positionInWorld);
            chunkPos = MapData.ToChunkPos(cellPos);
        }

        public Vector3 GetChunkOrigin(Vector2Int chunkPos)
        {
            var axial = MapData.ToCellPos(chunkPos).ToAxial();
            var pos2D = HexMath.Axial2Position(axial) * Radius;
            return transform.TransformPoint(new Vector3(pos2D.x, 0, pos2D.y));
        }

        public MapCellData GetData(Vector2Int cellPos)
        {
            var chunkPos = MapData.ToChunkPos(cellPos);
            if (!MapData.ExistChunk(chunkPos)) return MapData.defaultValue;

            var chunkArray = MapData.GetChunkAsArray(chunkPos);
            cellPos = TransformMapToChunk(chunkPos, cellPos);
            return chunkArray[cellPos];
        }
        public MapCellData GetData(Vector3 positionInWorld)
        {
            GetChunkAndCellPos(positionInWorld, out var chunkPos, out var cellPos);
            return GetData(cellPos);
        }

        /// <summary>
        /// 向世界坐标下的格点设置数据, 若该坐标无区块则生成后再设置
        /// </summary>
        public void SetData(Vector2Int cellPos, MapCellData data)
        {
            var chunkPos = MapData.ToChunkPos(cellPos);
            GenerateIfNotExist(chunkPos);
            var chunkArr = MapData.GetChunkAsArray(chunkPos);
            var cellInChunk = TransformMapToChunk(chunkPos, cellPos);
            chunkArr[cellInChunk] = data;
            pendingUpdateUVChunks.Add(chunkPos);
        }
        /// <summary>
        /// 向世界坐标下的格点设置数据, 若该坐标无区块则生成后再设置
        /// </summary>
        public void SetData(Vector3 positionInWorld, MapCellData data)
        {
            var cellPos = GetCellPos(positionInWorld);
            SetData(cellPos, data);
        }

        public Vector2Int TransformMapToChunk(Vector2Int chunkPos, Vector2Int cellPos)
        {
            var origin = MapData.ChunkSize * chunkPos;
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

        #region Callback

        protected void OnChunkCreated(Vector2Int chunkPos, IntPtr chunkData)
        {
            CreateChunkGO(chunkPos, new ChunkDataProxy<MapCellData>(chunkData, MapData.ChunkSize));
        }

        protected void OnChunkRegenerate(Vector2Int chunkPos, IntPtr chunkData)
        {
            if (Renderers.TryGetValue(chunkPos, out var renderer))
            {
                renderer.Init(GetChunkOrigin(chunkPos), Radius, new ChunkDataProxy<MapCellData>(chunkData, MapData.ChunkSize));
                UpdateChunkUV(chunkPos);
            }
            else
            {
                CreateChunkGO(chunkPos, new ChunkDataProxy<MapCellData>(chunkData, MapData.ChunkSize));
            }
        }

        protected void OnChunkRemoved(Vector2Int chunkPos, IntPtr chunkData)
        {
            Destroy(Renderers[chunkPos].gameObject);
        }

        #endregion

        protected void CreateChunkGO(Vector2Int chunkPos, INative2DArray<MapCellData> chunkData)
        {
            Vector3 worldPos = GetChunkOrigin(chunkPos);

            // 实际情况应该不存在大量反复生成销毁的情形, 因此直接 Instantiate 和 Destroy 就完事了.
            var go = Instantiate(chunkRendererPrefab.gameObject, transform);
            go.name = $"chunk {chunkPos}";
            var r = go.GetComponent<ChunkRenderer>();
            Renderers[chunkPos] = r;
            r.Init(worldPos, Radius, chunkData);
            // 周围的区块的边界可能也会被影响, 因此都标记一下
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    UpdateChunkUV(chunkPos + new Vector2Int(dx, dy));
                }
            }

        }

        protected void RemoveChunkGO(Vector2Int chunkPos)
        {
            // 实际情况应该不存在大量反复生成销毁的情形, 因此直接 Instantiate 和 Destroy 就完事了.
            Destroy(Renderers[chunkPos].gameObject);
        }

        /// <summary>
        /// 更新指定区块 UV. 不会立即执行, 而是提交到等待队列, 在合适的时机执行.
        /// <para></para>
        /// </summary>
        protected void UpdateChunkUV(Vector2Int chunkPos, bool createIfNotExist = false)
        {
            if (createIfNotExist || MapData.ExistChunk(chunkPos))
            {
                pendingUpdateUVChunks.Add(chunkPos);
            }
        }

        /// <summary>
        /// 立即更新区块的 UV. 区块不存在则先生成再更新 UV. 尽量避免在除了 Update 批量处理等待列表之外的地方调用.
        /// </summary>
        /// <param name="chunkPos"></param>
        protected void UpdateChunkUVImmiediate(Vector2Int chunkPos)
        {
            if (!Renderers.TryGetValue(chunkPos, out var r))
            {
                // 不存在则先生成一下
                GenerateIfNotExist(chunkPos);
                r = Renderers[chunkPos];
            }

            UVGenerator.GenerateUVForMap(r.Generator.GeneratedMesh, MapData, chunkPos, r.meshType, r.textureType);
        }
    }
}
