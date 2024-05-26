using HexFlow.NativeCore.Structures;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace HexFlow.NativeCore.Map
{
    
    /// <summary>
    /// 基于分型柏林噪声的地形算法
    /// </summary>
    [Serializable]
    public class PerlinNoiseTerrainGenerator : IBatchChunkGenerator<MapCellData>
    {
        public const string DllName = "Native_Main.dll";

        [DllImport(DllName, EntryPoint = "terr_gen_simple_perlin")]
        public static extern void GenrerateSimplePerlinTerrain(IntPtr dataPtr, int chunkSize, Vector2Int chunkPos, Vector2 noiseScale, Vector2 noiseOffset, int waveNum = 4, float enableThres = 0.25f);

        public struct GenerateTerrain : IJobParallelFor
        {
            NativeArray<IntPtr> ptrArray;
            int chunkSize;
            Vector2Int startPos;
            int areaChunkWidth;
            Vector2 noiseScale;
            Vector2 noiseOffset;
            int waveNum;
            float enableThres;

            public GenerateTerrain(NativeArray<IntPtr> ptrArray, int chunkSize, Vector2Int startPos, Vector2Int endPos, Vector2 noiseScale, Vector2 noiseOffset, int waveNum, float enableThres)
            {
                this.ptrArray = ptrArray;
                this.chunkSize = chunkSize;
                this.startPos = startPos;
                areaChunkWidth = endPos.x - startPos.x + 1;
                this.noiseScale = noiseScale;
                this.noiseOffset = noiseOffset;
                this.waveNum = waveNum;
                this.enableThres = enableThres;
            }

            public void Execute(int index)
            {
                var chunkPos = startPos + new Vector2Int(index % areaChunkWidth, index / areaChunkWidth);
                GenrerateSimplePerlinTerrain(ptrArray[index], chunkSize, chunkPos, noiseScale, noiseOffset, waveNum, enableThres);
            }
        }

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
   
        public void GenerateArea(Vector2Int startPos, Vector2Int endPos, IntPtr[] dataPtr, int seed, int chunkSize)
        {
            var tempArr = new  NativeArray<IntPtr>(dataPtr, Allocator.Temp);
            ScheduleJob(startPos, endPos, tempArr, seed, chunkSize).Complete();
        }

        public JobHandle ScheduleJob(Vector2Int startPos, Vector2Int endPos, NativeArray<IntPtr> dataPtr, int seed, int chunkSize)
        {
            var job = new GenerateTerrain(dataPtr, chunkSize, startPos, endPos, noiseScale, noiseOffset, waveNum, enableThreshold);
            return job.Schedule(dataPtr.Length, Mathf.Max(1, JobsUtility.JobWorkerMaximumCount / 2));
        }
    }

}
