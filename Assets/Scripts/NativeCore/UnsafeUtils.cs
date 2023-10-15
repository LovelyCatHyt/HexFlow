using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unitilities.Serialization;

namespace HexFlow.NativeCore
{
    public static class UnsafeUtils
    {
        /// <summary>
        /// 从 <see cref="NativeArray{T}"/> 获取 <see cref="{T}"/>* 类型的指针
        /// </summary>
        public static unsafe T* Get<T>(this NativeArray<T> array) where T : unmanaged => (T*)array.GetUnsafePtr();

        /// <summary>
        /// 类似于 C++ 中的 sizeof, 但其实只是 <see cref="Marshal.SizeOf{T}"/> 的封装
        /// </summary>
        public static int SizeOf<T>() => Marshal.SizeOf<T>();

        /// <summary>
        /// 按字节计算 <see cref="NativeArray{T}"/> 的数据长度
        /// <para>假设长度可以用 int 表示, 超出范围会抛出 <see cref="System.NotImplementedException"/></para>
        /// </summary>
        public static int SizeOfByte<T>(this NativeArray<T> array) where T : unmanaged
        {
            long temp = (long)array.Length * SizeOf<T>();
            if(temp>int.MaxValue)
            {
                // 这可能吗? 或许在创建 NativeArray 的时候已经报错了吧
                throw new System.NotImplementedException($"Does not support NativeArray that holds more than int.MaxValue of bytes, but get {temp}.");
            }
            return (int)temp;
        }

        /// <summary>
        /// 保存 <see cref="NativeArray{T}"/> 的数据到 <see cref="BinaryWriter"/> 中
        /// <para>直接保存连续的数据, 不包含长度提示</para>
        /// </summary>
        public static unsafe void SaveToBinaryWriter<T>(NativeArray<T> array, BinaryWriter writer) where T : unmanaged
        {
            int sizeInByte = array.SizeOfByte();
            // writer.Write(sizeInByte);
            System.Span<byte> buffer = new System.Span<byte>((byte*)array.GetUnsafePtr(), sizeInByte);
            writer.Write(buffer);
        }

        /// <summary>
        /// 根据 <see cref="NativeArray{T}"/> 的数据长度, 从 <see cref="BinaryReader"/> 中直接读取连续的字节
        /// <para>直接读取连续的数据, 假设数据的长度与 <paramref name="array"/> 的数据长度相同</para>
        /// </summary>
        public static unsafe void LoadFromBinaryReader<T>(NativeArray<T> array, BinaryReader reader) where T : unmanaged
        {
            int sizeOfByte = array.SizeOfByte();
            var data = new System.Span<byte>(reader.ReadBytes(sizeOfByte));
            System.Span<byte> buffer = new System.Span<byte>((byte*)array.GetUnsafePtr(), sizeOfByte);
            data.CopyTo(buffer);
        }
    }
}
