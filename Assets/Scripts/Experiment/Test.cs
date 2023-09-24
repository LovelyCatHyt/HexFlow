using HexFlow.NativeCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteAlways]
public class Test : MonoBehaviour
{
    public Vector3 Size = Vector3.one;

    private BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }
}
