using UnityEngine;
using Unitilities.Serialization;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{
    //public struct MyStruct
    //{
    //    public string Name;
    //    public string Value;
    //}

    //private void Awake()
    //{

    //}

    //public void IntFunc(int i) { Debug.Log($"Int function is called from {name}, got value {i}", this); }

    //public static int VectorFunc(Vector2Int vec) { return vec.x ^ vec.y; }

    //public void StringFunc(string str) { Debug.Log($"CharStar functio is called from {name}, got value {str}", this); }

    //public void ManagedCallManaged(Func<Vector2Int, int> func, int testCount)
    //{
    //    int num = 114514;
    //    for (int i = 0; i < testCount; i++)
    //    {
    //        num ^= func(new Vector2Int(19, 37));
    //    }
    //}

    //public void DirectCall(int testCount)
    //{
    //    int num = 114514;
    //    for (int i = 0; i < testCount; i++)
    //    {
    //        num ^= VectorFunc(new Vector2Int(19, 37));
    //    }
    //}

    [ContextMenu("Quick Test")]
    public void QuickTest()
    {
    }

    //[DllImport("Native_Main.dll", EntryPoint = "set_void_func_int")]
    //static extern void SetIntFunc(IntPtr funcPtr);

    //[DllImport("Native_Main.dll", EntryPoint = "set_int_func_vec2i")]
    //static extern void SetVecFunc(IntPtr funcPtr);

    //[DllImport("Native_Main.dll", EntryPoint = "set_void_func_charStar")]
    //static extern void SetCharArrayFunction(IntPtr funcPtr);

    //[DllImport("Native_Main.dll", EntryPoint = "test_func_ptr")]
    //static extern void TestFuncPtr(int testCount);
}
