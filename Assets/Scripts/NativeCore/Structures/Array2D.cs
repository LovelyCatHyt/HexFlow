using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace HexFlow.NativeCore.Structures
{
    /// <summary>
    /// 支持多种下标索引的 2D 数组, 内部使用行主序的 <see cref="NativeArray{T}"/>
    /// </summary>
    public class Array2D<T> : IDisposable where T : unmanaged
    {
        public NativeArray<T> Data { get; private set; }

        public readonly Vector2Int Size;

        public Array2D(int width, int height)
        {
            if (width < 1 || height < 1)
            {
                throw new ArgumentException($"Both width and height should no less than 1, but get width={width}, height={height}");
            }

            Size.x = width; Size.y = height;
            Data = new NativeArray<T>(width * height, Allocator.Persistent);
        }

        public void Dispose()
        {
            Data.Dispose();
        }

        public T this[int index]
        {
            get => Data[index];
            set
            {
                if (index >= Data.Length)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                unsafe
                {
                    Data.Get()[index] = value;
                }
            }
        }

        public T this[int x, int y]
        {
            get => this[y * Size.x + x];
            set => this[y * Size.x + x] = value;
        }

        public T this[Vector2Int pos]
        {
            get => this[pos.x, pos.y];
            set => this[pos.x, pos.y] = value;
        }

        public T[] ToArray()
        {
            return Data.ToArray();
        }
    }
}