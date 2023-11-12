using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unitilities;
using Unitilities.Serialization;
using UnityEngine;

using Ntv = HexFlow.NativeCore.Structures.Chunked2DContainer_Native;

namespace HexFlow.NativeCore.Structures
{
    public interface IChunkGenerator<T> where T : unmanaged
    {
        /// <summary>
        /// 生成一个区块
        /// </summary>
        void Generate(Vector2Int chunkPos, IntPtr dataPtr, int seed, int chunkSize);
    }

    //public interface IChunkBatchGenerator<T> where T : unmanaged
    //{
    //    /// <summary>
    //    /// 生成一个区域的区块, 由调用者保证 start 的各分量 <= end
    //    /// </summary>
    //    void Generate(Vector2Int start, Vector2Int end, int seed);
    //}

    /// <summary>
    /// 复制默认值的区块生成器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CopyDefaultGenerator<T> : IChunkGenerator<T> where T : unmanaged
    {
        T @default = default(T);

        public CopyDefaultGenerator(T @default = default)
        {
            this.@default = @default;
        }

        public void Generate(Vector2Int chunkPos, IntPtr dataPtr, int seed, int chunkSize)
        {
            unsafe
            {
                T* typedPtr = (T*)dataPtr;
                int elementCount = chunkSize * chunkSize;
                for (int i = 0; i < elementCount; ++i)
                {
                    typedPtr[i] = @default;
                }
            }
        }
    }

    /// <summary>
    /// 区块数据代理, 提供一个数组形式的访问器用于简单的读写操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChunkDataProxy<T> : INative2DArray<T>
        where T : unmanaged
    {
        private IntPtr _data;

        public Vector2Int Size { get; set; }

        public ChunkDataProxy(IntPtr data, Vector2Int size)
        {
            _data = data;
            Size = size;
        }

        public ChunkDataProxy(IntPtr data,  int size)
        {
            _data = data;
            Size = new Vector2Int(size, size);
        }

        public IntPtr RawPtr => _data;

        public T this[Vector2Int pos] { get => this[pos.x, pos.y]; set => this[pos.x, pos.y] = value; }
        public unsafe T this[int x, int y]
        {
            get
            {
                var typedPtr = (T*)_data;
                return typedPtr[y * Size.x + x];
            }
            set
            {
                var typedPtr = (T*)_data;
                typedPtr[y * Size.x + x] = value;
            }
        }
    }

    /// <summary>
    /// 按区块存储的, 以2D坐标索引的容器
    /// </summary>
    public class Chunked2DContainer<T> : IDisposable, IEnumerable<KeyValuePair<Vector2Int, IntPtr>>
        where T : unmanaged
    {
        public class Iterator : IEnumerator<KeyValuePair<Vector2Int, IntPtr>>
        {
            private IntPtr _ptr = IntPtr.Zero;
            private IntPtr _iter;

            public Iterator(Chunked2DContainer<T> container)
            {
                _ptr = container._ptr;
                _iter = Ntv.CreateIter(_ptr);
            }

            public Vector2Int CurrentPos => Ntv.IterKey(_iter);
            public IntPtr CurrentData => Ntv.IterValue(_iter);

            public KeyValuePair<Vector2Int, IntPtr> Current => new KeyValuePair<Vector2Int, IntPtr>(CurrentPos, CurrentData);

            object IEnumerator.Current => Current;

            // 析构函数也删除一下以防坏事发生
            ~Iterator()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (_iter != IntPtr.Zero) Ntv.IterDelete(_iter);
                _iter = IntPtr.Zero;
            }

            public bool MoveNext()
            {
                Ntv.IterAdvance(_iter);
                return Ntv.IterValid(_ptr, _iter);
            }

            public void Reset()
            {
                Ntv.IterReset(_ptr, _iter);
            }
        }
               

        /// <summary>
        /// 区块操作
        /// </summary>
        /// <param name="chunkPos">区块坐标</param>
        /// <param name="chunkData">区块数据</param>
        public delegate void ChunAction(Vector2Int chunkPos, IntPtr chunkData);

        public readonly int ChunkSize = 32;

        public readonly int ElementCountPerChunk;

        public int ChunkCount => Ntv.ChunkCount(_ptr);

        /// <summary>
        /// 每个区块序列化时占用的实际数据长度
        /// </summary>
        public int ChunkDataByteLength =>
            UnsafeUtils.SizeOf<Vector2Int>() +                  /*区块坐标*/
            UnsafeUtils.SizeOf<T>() * ChunkSize * ChunkSize;    /*单个区块数据*/

        public int seed;

        public T defaultValue = new();

        public IChunkGenerator<T> chunkGenerator;

        //public IChunkBatchGenerator<T> chunkBatchGenerator;

        public event ChunAction onChunkCreated;

        public event ChunAction onBeforeChunkRemoved;

        /// <summary>
        /// Native 实现的对象指针
        /// </summary>
        public IntPtr NativePtr => _ptr;

        /// <summary>
        /// Native 实现的对象指针
        /// </summary>
        private IntPtr _ptr;

        // 不太确定在容器内跟踪区块数据更新的必要性, 先不做
        // public event ChunkEvent onChunkUpdate;


        public Chunked2DContainer(T defaultValue, int seed = 0, int chunkSize = 32)
        {
            this.defaultValue = defaultValue;
            this.seed = seed;
            ChunkSize = chunkSize;
            ElementCountPerChunk = ChunkSize * ChunkSize;
            _ptr = Ntv.Ctor_0(ChunkSize, UnsafeUtils.SizeOf<T>());
            Generate(Vector2Int.zero);
        }

        #region IBinarySerializable
        public long SerializeByteLength =>
            sizeof(long) +                      /*表示区块数据总长度的长整数*/
            ChunkCount * ChunkDataByteLength;   /*所有区块实际数据的长度*/

        public string FileExtension => $"{nameof(T)}.chk"; // 保留类型名, 方便未来做特殊处理或 Debug. "chk" 表示 chunk.

        //public void Serialize(BinaryWriter writer)
        //{
        //    writer.Write(SerializeByteLength);
        //    var chunkPosArray = _map.Keys.ToArray();
        //    BinarySerializeUtils.Serialize(chunkPosArray, writer, false);
        //    foreach (var pos in chunkPosArray)
        //    {
        //        var data = _map[pos];
        //        UnsafeUtils.SaveToBinaryWriter(data.Data, writer);
        //    }
        //}

        //public void Deserialize(BinaryReader reader)
        //{
        //    var byteLength = reader.ReadInt32();
        //    var calculatedCount = byteLength / ChunkDataByteLength;
        //    if (byteLength % calculatedCount != 0)
        //    {
        //        // 不能整除, 说明肯定有哪里不对劲
        //        // 由于二进制序列化的额外信息极其有限, 无法确定下一个有效数据是什么,
        //        // 假设所有数据块都在开头标记字节长度, 则跳过这个长度是相对安全的
        //        reader.BaseStream.Seek(byteLength, SeekOrigin.Current);
        //        throw new SizeOfByteNotMatchException(byteLength, calculatedCount * ChunkDataByteLength);
        //    }
        //    // 读取坐标数组
        //    var posArray = BinarySerializeUtils.Deserialize<Vector2Int>(reader, calculatedCount);
        //    // 读取具体数据
        //    foreach (var pos in posArray)
        //    {
        //        Array2D<T> newData = new Array2D<T>(ChunkSize, ChunkSize);
        //        _map[pos] = newData;
        //        UnsafeUtils.LoadFromBinaryReader(newData.Data, reader);
        //    }
        //    // 调用事件
        //    if (onChunkCreated != null)
        //    {
        //        foreach (var pos in posArray)
        //        {
        //            onChunkCreated.Invoke(pos, _map[pos]);
        //        }
        //    }
        //}
        #endregion

        public Vector2Int ToCellPos(Vector2Int chunkPos) => chunkPos * ChunkSize;

        public Vector2Int ToChunkPos(Vector2Int cellPos)
        {
            return new Vector2Int(MathTool.FloorDiv(cellPos.x, ChunkSize), MathTool.FloorDiv(cellPos.y, ChunkSize));
        }

        /// <summary>
        /// 在指定区块坐标上生成区块
        /// <para>如果该区块已创建, 会覆盖已有的数据</para>
        /// </summary>
        public void Generate(Vector2Int chunkPos)
        {
            IntPtr targetChunkData;
            if (!Ntv.ExistChunk(_ptr, chunkPos))
            {
                targetChunkData = Ntv.CreateChunk(_ptr, chunkPos);
            }
            else
            {
                targetChunkData = Ntv.GetChunkData(_ptr, chunkPos);
            }

            if (chunkGenerator != null) chunkGenerator.Generate(chunkPos, targetChunkData, seed, ChunkSize);

            onChunkCreated?.Invoke(chunkPos, Ntv.GetChunkData(_ptr, chunkPos));
        }
        /// <summary>
        /// 在指定区块区域上生成区块, 区域包含两个端点
        /// <para>如果该区块已创建, 会覆盖已有的数据</para>
        /// <para>考虑并行和数据连续性, 直接生成一个区域通常比逐块生成要快</para>
        /// </summary>
        public void Generate(Vector2Int start, Vector2Int end)
        {
            MathTool.CorrectMinMax(start, end, out start, out end);
            // 没有区域生成器则手动遍历每个区块
            for (int row = start.y; row <= end.y; row++)
            {
                for (int col = start.x; col < end.x; col++)
                {
                    Generate(new Vector2Int(col, row));
                }
            }
        }

        /// <summary>
        /// 指定坐标的区块是否存在
        /// </summary>
        /// <param name="chunkPos"></param>
        /// <returns></returns>
        public bool ExistChunk(Vector2Int chunkPos)
        {
            return Ntv.ExistChunk(_ptr, chunkPos);
        }

        /// <summary>
        /// 移除一个区块
        /// </summary>
        public bool Remove(Vector2Int pos)
        {
            if (ExistChunk(pos))
            {
                onBeforeChunkRemoved?.Invoke(pos, Ntv.GetChunkData(_ptr, pos));
                return Ntv.RemoveChunk(_ptr, pos);
            }
            return false;
        }

        public IntPtr GetChunk(Vector2Int chunkPos)
        {
            return Ntv.GetChunkData(_ptr, chunkPos);
        }

        public INative2DArray<T> GetChunkAsArray(Vector2Int chunkPos)
        {
            return new ChunkDataProxy<T>(GetChunk(chunkPos), ChunkSize);
        }

        ~Chunked2DContainer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_ptr != IntPtr.Zero)
            {
                foreach(var kv in this)
                {
                    Remove(kv.Key);
                }

                Ntv.Dtor(_ptr);
            }
            _ptr = IntPtr.Zero;
        }

        public IEnumerator<KeyValuePair<Vector2Int, IntPtr>> GetEnumerator() => new Iterator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Iterator(this);
    }
}

