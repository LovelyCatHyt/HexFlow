using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class TestComputeShader : MonoBehaviour
{
    public ComputeShader computeShader;

    public Vector2Int rtSize = new Vector2Int(1024, 1024);

    public bool initAsOrdered = false;
    public bool shuffle = false;
    public Vector2 scale;
    public Vector2 offset;
    [Range(1, 16)] public int fractalNum = 1;
    public float defaultZ;

    [SerializeField] private ComputeBuffer _permutation;
    [SerializeField] private RenderTexture _rt;
    private Material _material;
    private readonly int TilingID = Shader.PropertyToID("Tiling");
    private readonly int DefaultZID = Shader.PropertyToID("DefaultZ");
    private readonly int FractalNumID = Shader.PropertyToID("FractalNum");


    void Start()
    {
        _permutation = new ComputeBuffer(256, 4, ComputeBufferType.Default, ComputeBufferMode.Dynamic);

        var data = new int[256] { 151,160,137,91,90,15,
                131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
                190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
                88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
                77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
                102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
                135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
                5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
                223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
                129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
                251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
                49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
                138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180 };
        if(initAsOrdered) for (int i = 0; i < data.Length; i++) data[i] = i;
        if (shuffle)
        {
            System.Random random = new System.Random();
            for (int i = data.Length - 1; i >= 1; i--)
            {
                int targetId = random.Next(i);
                int t = data[targetId];
                data[targetId] = data[i];
                data[i] = t;
            } 
        }
        _permutation.SetData(data);

        _rt = new RenderTexture(rtSize.x, rtSize.y, 32);
        _rt.enableRandomWrite = true;
        _rt.Create();

        _material = GetComponent<Renderer>().material;
        computeShader.SetBuffer(1, "Permutation", _permutation);
        computeShader.SetTexture(1, "OutNoiseTex", _rt);
        _material.SetTexture("_MainTexture", _rt);
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetVector(TilingID, new Vector4(scale.x, scale.y, offset.x, offset.y));
        computeShader.SetFloat(DefaultZID, defaultZ);
        computeShader.SetInt(FractalNumID, fractalNum);
        computeShader.Dispatch(1, rtSize.x / 8, rtSize.y / 8, 1);
    }

    [ContextMenu("Shuffle")]
    public void ShufflePermutation()
    {
        var data = new int[256];
        for (int i = 0; i < data.Length; i++) data[i] = i;
        System.Random random = new System.Random();
        for (int i = data.Length - 1; i >= 1; i--)
        {
            int targetId = random.Next(i);
            int t = data[targetId];
            data[targetId] = data[i];
            data[i] = t;
        }

        _permutation.SetData(data);
        computeShader.SetBuffer(1, "Permutation", _permutation);
    }

    private void OnDestroy()
    {
        _rt.Release();
    }
    
    [ContextMenu("Save rt")]
    public void Save()
    {
        // _rt.GenerateMips();
    }
}
