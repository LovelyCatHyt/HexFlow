using UnityEngine;
using Unity.Collections;
using HexFlow.NativeCore.Structures;
using System;

namespace HexFlow.NativeCore
{
    /// <summary>
    /// 支持多种下标索引的 2D 数组, 内部使用行主序的 <see cref="NativeArray{T}"/>
    /// <para>增加了两个特性: 越界读取返回默认值, 越界写入自动扩容并复制原数组</para>
    /// <para>支持负数索引</para>
    /// </summary>
    public class Array2DAuto<T> : Array2D<T> where T : unmanaged
    {
        public T defaultValue;

        /// <summary>
        /// 用于计算实际二维数组坐标偏移量. 虚拟坐标加上该偏移量得到的坐标即为实际数组坐标.
        /// </summary>
        public Vector2Int Offset { get; protected set; } = Vector2Int.zero;

        public Vector2Int Min => -Offset;
        public Vector2Int Max => -Offset + Size - Vector2Int.one;

        public Array2DAuto(Vector2Int size, Vector2Int offset, T defaultValue) : this(size, offset) 
        {
            this.defaultValue = defaultValue;
            for (int i = 0; i < Length; i++)
            {
                this[i] = defaultValue;
            }
        }

        public Array2DAuto(Vector2Int size, Vector2Int offset) : this(size.x, size.y, offset.x, offset.y) { }

        public Array2DAuto(int width, int height, int offsetX, int offsetY) : base(width, height)
        {
            Offset = new Vector2Int(offsetX, offsetY);
            defaultValue = new T();
        }

        /// <summary>
        /// 无坐标换算与自动扩容机制的直接访问, 因为外部传入的一维坐标对于该类无实际意义.
        /// <para>比基类多出越界读取返回默认值的功能</para>
        /// </summary>
        public override T this[int index]
        {
            get
            {
                if (index < 0 || index >= Length) return defaultValue;
                return base[index];
            }

            set => base[index] = value;
        }

        /// <summary>
        /// 可以任意访问的属性, 写入到原有区域外的坐标时会自动保留原数据并扩容.
        /// <para>暂不支持超前扩容, 每次扩容都是O(max(n1, n2))代价, n1 和 n2 分别为旧容量和新容量.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override T this[int x, int y]
        {
            get
            {
                var realX = x + Offset.x;
                var realY = y + Offset.y;
                if (realX >= 0 && realY >= 0 && realX < Width && realY < Height)
                {
                    return base[realX, realY];
                }
                else
                {
                    return defaultValue;
                }
            }

            set
            {
                var realX = x + Offset.x;
                var realY = y + Offset.y;
                if (realX >= 0 && realY >= 0 && realX < Width && realY < Height)
                {
                    base[realX, realY] = value;
                }
                else
                {
                    AddOutOfBoundPoint(x, y, value);
                }
            }
        }

        /// <summary>
        /// 重置已存储区域的范围, 并保留新旧范围重叠的数据.
        /// 两个端点可以是任一对角线, 任一顺序的两端点.
        /// </summary>
        public virtual void ResetRange(Vector2Int a, Vector2Int b)
        {
            // 算出实际的最小最大边界
            var realMin = Vector2Int.Min(a, b);
            var realMax = Vector2Int.Max(a, b);


            var newWidth = realMax.x - realMin.x + 1;
            var newHeight = realMax.y - realMin.y + 1;
            var newOffset = -realMin;

            if (newWidth == 0 || newHeight == 0)
            {
                throw new ArgumentException("Range cannot has zero width or zero height.");
            }

            var newData = new NativeArray<T>(newWidth * newHeight, Allocator.Persistent);

            for (int row = 0; row < newHeight; row++)
            {
                for (int col = 0; col < newWidth; col++)
                {
                    var dstIdx = row * newWidth + col;
                    // (col, row) 相当于实际坐标, 需要转换为虚拟坐标, 然后调用基于虚拟坐标的 getter 读取原 Data 中的数据
                    var virtualX = col - newOffset.x;
                    var virtualY = row - newOffset.y;
                    newData[dstIdx] = this[virtualX, virtualY];
                }
            }

            Data.Dispose();
            Data = newData;
            Offset = newOffset;
            Size = new Vector2Int(newWidth, newHeight);
        }

        /// <summary>
        /// 设置越界点的值
        /// </summary>
        /// <param name="x">已引入偏移量的越界点 x 坐标</param>
        /// <param name="y">已引入偏移量的越界点 y 坐标</param>
        /// <param name="value">欲设置的值</param>
        protected void AddOutOfBoundPoint(int x, int y, T value)
        {
            var point = new Vector2Int(x, y);
            ResetRange(Vector2Int.Min(point, Min), Vector2Int.Max(point, Max));
            this[x, y] = value;
        }
    }

}
