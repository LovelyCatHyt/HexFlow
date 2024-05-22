using HexFlow.NativeCore.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HexFlow.NativeCore.Map
{
    /// <summary>
    /// 基于分型柏林噪声的地形算法
    /// </summary>
    [Serializable]
    public class PerlinNoiseTerrainGenerator : IChunkGenerator<MapCellData>
    {
        public const string DllName = "Native_Main.dll";

        [DllImport(DllName, EntryPoint = "terr_gen_simple_perlin")]
        public static extern void GenrerateSimplePerlinTerrain(IntPtr dataPtr, int chunkSize, Vector2Int chunkPos, Vector2 noiseScale, Vector2 noiseOffset, int waveNum = 4, float enableThres = 0.25f);

        public Vector2 noiseScale;
        public Vector2 noiseOffset;
        public int waveNum;
        /// <summary>
        /// 将单元的 enabled 设置为 true 的阈值
        /// </summary>
        float enableThreshold;

        public PerlinNoiseTerrainGenerator()
        {
            noiseScale = Vector2.one;
            noiseOffset = Vector2.zero;
            waveNum = 4;
            enableThreshold = 1.0f;
        }

        public PerlinNoiseTerrainGenerator(Vector2 noiseScale, Vector2 noiseOffset, int waveNum = 4, float enableThreshold = 0.25f)
        {
            this.noiseScale = noiseScale;
            this.noiseOffset = noiseOffset;
            this.waveNum = waveNum;
            this.enableThreshold = enableThreshold;
        }

        public virtual void Generate(Vector2Int chunkPos, IntPtr dataPtr, int seed, int chunkSize)
        {
            GenrerateSimplePerlinTerrain(dataPtr, chunkSize, chunkPos, noiseScale, noiseOffset, waveNum, enableThreshold);
        }
    }

}
