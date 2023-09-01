using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HexFlow.NativeCore
{
    public static class UnsafeUtils
    {
        public static unsafe T* Get<T>(this NativeArray<T> array) where T : unmanaged => (T*)array.GetUnsafePtr();
    }
}
