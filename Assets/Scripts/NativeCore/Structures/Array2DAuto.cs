using HexFlow.NativeCore.Structures;
using UnityEngine;

namespace HexFlow.NativeCore
{
    /// <summary>
    /// 支持多种下标索引的 2D 数组, 内部使用行主序的 <see cref="NativeArray{T}"/>
    /// <para>与基类 <see cref="Array2D{T}"/> 相比, 增加了两个特性: 越界读取返回默认值, 越界写入自动扩容并复制原数组</para>
    /// </summary>
    public class Array2DAuto<T> : Array2D<T> where T : unmanaged
    {
        public T defaultValue;

        public Array2DAuto(Vector2Int size) : this(size.x, size.y) { }

        public Array2DAuto(int width, int height) : base(width, height) { defaultValue = new T(); }
    }

}
