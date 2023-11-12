using System;
using UnityEngine;

namespace HexFlow.NativeCore.Structures
{
    public interface INative2DArray<T> where T : unmanaged
    {
        public IntPtr RawPtr { get; }

        public Vector2Int Size { get; }

        public T this[Vector2Int pos] { get; set; }

        public T this[int x, int y] { get; set; }
    }
}
