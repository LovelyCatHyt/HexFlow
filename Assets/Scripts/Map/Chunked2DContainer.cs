using HexFlow.NativeCore;
using HexFlow.NativeCore.Structures;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unitilities;
using Unitilities.Serialization;
using UnityEngine;

namespace HexFlow.Map
{
    public interface IChunkGenerator<T> where T : unmanaged
    {
        /// <summary>
        /// 生成一个区块
        /// </summary>
        void Generate(Vector2Int chunkPos, Array2D<T> array, int seed);
    }

    public interface IChunkBatchGenerator<T> where T : unmanaged
    {
        /// <summary>
        /// 生成一个区域的区块, 由调用者保证 start 的各分量 <= end
        /// </summary>
        void Generate(Vector2Int start, Vector2Int end, int seed);
    }

    /// <summary>
    /// 按区块存储的, 以2D坐标索引的容器
    /// </summary>
    public class Chunked2DContainer<T> : IBinarySerializable where T : unmanaged
    {
        /// <summary>
        /// 区块事件
        /// </summary>
        /// <param name="chunkPos">区块坐标</param>
        /// <param name="chunkData">区块数据</param>
        public delegate void ChunkEvent(Vector2Int chunkPos, Array2D<T> chunkData);

        public readonly int ChunkSize = 32;

        public int ChunkCount => _map.Count;

        /// <summary>
        /// 每个区块序列化时占用的实际数据长度
        /// </summary>
        public int ChunkDataByteLength =>
            UnsafeUtils.SizeOf<Vector2Int>() +                  /*区块坐标*/
            UnsafeUtils.SizeOf<T>() * ChunkSize * ChunkSize;    /*单个区块数据*/

        public int seed;

        public T defaultValue = new();

        public IChunkGenerator<T> chunkGenerator;

        public IChunkBatchGenerator<T> chunkBatchGenerator;

        public event ChunkEvent onChunkCreated;

        // 不太确定在容器内跟踪区块数据更新的必要性, 先不做
        // public event ChunkEvent onChunkUpdate;

        /// <summary>
        /// 内部数据结构
        /// </summary>
        protected Dictionary<Vector2Int, Array2D<T>> _map;

        public Chunked2DContainer(T defaultValue, IChunkGenerator<T> chunkGenerator = null, IChunkBatchGenerator<T> chunkBatchGenerator = null, int seed = 0)
        {
            this.defaultValue = defaultValue;
            this.chunkGenerator = chunkGenerator;
            this.chunkBatchGenerator = chunkBatchGenerator;
            this.seed = seed;
            Generate(Vector2Int.zero);
        }

        #region IBinarySerializable
        public long SerializeByteLength =>
            sizeof(long) +                      /*表示区块数据总长度的长整数*/
            ChunkCount * ChunkDataByteLength;   /*所有区块实际数据的长度*/

        public string FileExtension => $"{nameof(T)}.chk"; // 保留类型名, 方便未来做特殊处理或 Debug. "chk" 表示 chunk.

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(SerializeByteLength);
            var chunkPosArray = _map.Keys.ToArray();
            BinarySerializeUtils.Serialize(chunkPosArray, writer, false);
            foreach (var pos in chunkPosArray)
            {
                var data = _map[pos];
                UnsafeUtils.SaveToBinaryWriter(data.Data, writer);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            var byteLength = reader.ReadInt32();
            var calculatedCount = byteLength / ChunkDataByteLength;
            if (byteLength % calculatedCount != 0)
            {
                // 不能整除, 说明肯定有哪里不对劲
                // 由于二进制序列化的额外信息极其有限, 无法确定下一个有效数据是什么,
                // 假设所有数据块都在开头标记字节长度, 则跳过这个长度是相对安全的
                reader.BaseStream.Seek(byteLength, SeekOrigin.Current);
                throw new SizeOfByteNotMatchException(byteLength, calculatedCount * ChunkDataByteLength);
            }
            // 读取坐标数组
            var posArray = BinarySerializeUtils.Deserialize<Vector2Int>(reader, calculatedCount);
            // 读取具体数据
            foreach (var pos in posArray)
            {
                Array2D<T> newData = new Array2D<T>(ChunkSize, ChunkSize);
                _map[pos] = newData;
                UnsafeUtils.LoadFromBinaryReader(newData.Data, reader);
            }
            // 调用事件
            if (onChunkCreated != null)
            {
                foreach (var pos in posArray)
                {
                    onChunkCreated.Invoke(pos, _map[pos]);
                }
            }
        }
        #endregion

        /// <summary>
        /// 在指定区块坐标上生成区块
        /// <para>如果该区块已创建, 会覆盖已有的数据</para>
        /// </summary>
        public void Generate(Vector2Int chunkPos)
        {
            if (!_map.TryGetValue(chunkPos, out Array2D<T> array))
            {
                array = _map[chunkPos] = new Array2D<T>(ChunkSize, ChunkSize);
            }

            if (chunkGenerator == null)
            {
                array.Fill(defaultValue);
            }
            else
            {
                chunkGenerator.Generate(chunkPos, array, seed);
            }

            onChunkCreated?.Invoke(chunkPos, array);
        }

        /// <summary>
        /// 在指定区块区域上生成区块, 区域包含两个端点
        /// <para>如果该区块已创建, 会覆盖已有的数据</para>
        /// <para>考虑并行和数据连续性, 直接生成一个区域通常比逐块生成要快</para>
        /// </summary>
        public void Generate(Vector2Int start, Vector2Int end)
        {
            MathTool.CorrectMinMax(start, end, out start, out end);
            if (chunkBatchGenerator == null)
            {
                // 没有区域生成器则手动遍历每个区块
                for (int row = start.y; row <= end.y; row++)
                {
                    for (int col = start.x; col < end.x; col++)
                    {
                        Generate(new Vector2Int(col, row));
                    }
                }
            }
            else
            {
                for (int row = start.y; row <= end.y; row++)
                {
                    for (int col = start.x; col < end.x; col++)
                    {
                        var chunkPos = new Vector2Int(col, row);
                        if (!_map.ContainsKey(chunkPos))
                        {
                            _map[chunkPos] = new Array2D<T>(ChunkSize, ChunkSize);
                        }
                    }
                }
                chunkBatchGenerator.Generate(start, end, seed);
                // 提前判断, 避免循环内无意义的重复判断
                if (onChunkCreated != null)
                {
                    for (int row = start.y; row <= end.y; row++)
                    {
                        for (int col = start.x; col < end.x; col++)
                        {
                            Vector2Int chunkPos = new Vector2Int(col, row);
                            onChunkCreated.Invoke(chunkPos, _map[chunkPos]);
                        }
                    }
                }
            }
        }

    }

}

