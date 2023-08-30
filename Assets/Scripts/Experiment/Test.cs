using HexFlow.NativeCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Test : MonoBehaviour
{
    public Vector2Int offset;

    public static Vector2Int Offset2Axial(Vector2Int offset)
    {
        var q = offset.x - (offset.y - (offset.y & 1)) / 2;
        var r = offset.y;
        return new Vector2Int(q, r);
    }

    private void Update()
    {
        int repeatTime = 100;
        Profiler.BeginSample("Managed"); // ~ 0.002 ms
        for (int i = 0; i < repeatTime; i++)
        {
            Offset2Axial(offset);
        }
        Profiler.EndSample();

        Profiler.BeginSample("Native Interop"); // ~ 0.003 ms
        for (int i = 0; i < repeatTime; i++)
        {
            HexMath.Offset2Axial(offset);
        }
        Profiler.EndSample();
    }
}
