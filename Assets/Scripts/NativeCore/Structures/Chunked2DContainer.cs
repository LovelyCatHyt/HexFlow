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

        public void ThrowIfInValidIndex(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Size.x || y >= Size.y)
            {
                throw new IndexOutOfRangeException($"Try to access ChunkDataProxy with x: {x}, y: {y}, but array size is {Size}");
            }
        }

        public T this[Vector2Int pos] { get => this[pos.x, pos.y]; set => this[pos.x, pos.y] = value; }
        public unsafe T this[int x, int y]
        {
            get
            {
                ThrowIfInValidIndex(x, y);
                var typedPtr = (T*)_data;
                return typedPtr[y * Size.x + x];
            }
            set
            {
                ThrowIfInValidIndex(x, y);
                var typedPtr = (T*)_data;
                typedPtr[y * Size.x + x] = value;
            }
        }
    }

    public class ChunkNotFoundException : Exception
    {
        public ChunkNotFoundException(Vector2Int chunkIndex) 
        : base($"Chunk {chunkIndex} is not found in container.")
        {
        }
    }

    /// <summary>
    /// 按区块存储的, 以2D坐标索引的容器
    /// </summary>
    public class Chunked2DContainer<T> : IDisposable, IEnumerable<KeyValuePair<Vector2Int, IntPtr>>, IBinarySerializable
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
                _cachedValid = false;
            }

            public Vector2Int CurrentPos => Ntv.IterKey(_iter);
            public IntPtr CurrentData => Ntv.IterValue(_iter);

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

            public bool _cachedValid = false;

            // 由于 cpp 层的迭代器比 c# 的迭代器提前一步, 因此要先存一对数据
            public KeyValuePair<Vector2Int, IntPtr> _cachedPair;

            // 当前迭代元素的 key 和 value
            public KeyValuePair<Vector2Int, IntPtr> Current => _cachedValid ? _cachedPair : throw new IndexOutOfRangeException();

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                // C# 迭代器中, MoveNext 的结果对应的是 cpp 迭代前的结果
                _cachedValid = Ntv.IterValid(_ptr, _iter);
                if(_cachedValid)
                {
                    _cachedPair = new KeyValuePair<Vector2Int, IntPtr>(CurrentPos, CurrentData);
                    Ntv.IterAdvance(_iter);
                }
                return _cachedValid;
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
            // Generate(Vector2Int.zero);
        }

        #region IBinarySerializable

        /// <summary>
        /// 每个区块核心数据的长度
        /// </summary>
        public int ChunkDataLengthNoHead => UnsafeUtils.SizeOf<T>() * ChunkSize * ChunkSize;

        /// <summary>
        /// 每个区块序列化时占用的实际数据长度
        /// </summary>
        public int ChunkDataLengthWithHead => UnsafeUtils.SizeOf<Vector2Int>() + ChunkDataLengthNoHead;
        public long SerializeByteLength =>
            sizeof(int) +                           /*表示区块数目的整数*/
            ChunkCount * ChunkDataLengthWithHead;   /*所有区块实际数据的长度*/

        public string FileExtension => $"{typeof(T).Name}.chk"; // 保留类型名, 方便未来做特殊处理或 Debug. "chk" 表示 chunk.

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChunkCount);
            foreach (var kv in this)
            {
                writer.Write(kv.Key.x);
                writer.Write(kv.Key.y);
                unsafe
                {
                    var span = new ReadOnlySpan<byte>(kv.Value.ToPointer(), ChunkDataLengthNoHead);
                    writer.Write(span);
                }
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            // 先清除已有的所有区块, 比如初始化大概率已经生成的
            var keys = new List<Vector2Int>();
            foreach (var kv in this)
            {
                keys.Add(kv.Key);
            }
            foreach(var key in keys)
            {
                Remove(key);
            }

            int chunkCount = reader.ReadInt32();
            for (int i = 0; i < chunkCount; i++)
            {
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var chunkPos = new Vector2Int(x, y);
                
                var chunkPtr = CreateRawChunk(chunkPos);
                unsafe
                {
                    reader.BaseStream.Read(new Span<byte>(chunkPtr.ToPointer(), ChunkDataLengthNoHead));
                }
                onChunkCreated?.Invoke(chunkPos, chunkPtr);
            }
        }
        #endregion

        public Vector2Int ToCellPos(Vector2Int chunkPos) => chunkPos * ChunkSize;

        public Vector2Int ToChunkPos(Vector2Int cellPos)
        {
            return new Vector2Int(MathTool.FloorDiv(cellPos.x, ChunkSize), MathTool.FloorDiv(cellPos.y, ChunkSize));
        }

        public IntPtr CreateRawChunk(Vector2Int chunkPos)
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
            return targetChunkData;
        }

        /// <summary>
        /// 在指定区块坐标上生成区块
        /// <para>如果该区块已创建, 会覆盖已有的数据</para>
        /// </summary>
        public void Generate(Vector2Int chunkPos, bool runGenerator = true)
        {
            IntPtr targetChunkData = CreateRawChunk(chunkPos);
            
            if (chunkGenerator != null && runGenerator) chunkGenerator.Generate(chunkPos, targetChunkData, seed, ChunkSize);

            onChunkCreated?.Invoke(chunkPos, targetChunkData);
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
            if(!ExistChunk(chunkPos))
            {
                throw new ChunkNotFoundException(chunkPos);
            }
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
                    onBeforeChunkRemoved?.Invoke(kv.Key, kv.Value);
                }

                Ntv.Dtor(_ptr);
            }
            _ptr = IntPtr.Zero;
        }

        public IEnumerator<KeyValuePair<Vector2Int, IntPtr>> GetEnumerator() => new Iterator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Iterator(this);
    }
}

