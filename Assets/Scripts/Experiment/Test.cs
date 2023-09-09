using HexFlow.NativeCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteAlways]
public class Test : MonoBehaviour
{
    public bool flag;

    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void Start()
    {
        Debug.Log("Start");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    private void OnValidate()
    {   
        /*
        Debug.Log($"OnValidate, " +
        $"GetPrefabAssetType: {UnityEditor.PrefabUtility.GetPrefabAssetType(this)}, " +
        $"GetPrefabInstanceStatus: {UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject)}");
        */
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }

}
