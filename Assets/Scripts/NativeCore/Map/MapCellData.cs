using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Collections;
using HexFlow.NativeCore.Structures;
using System;
using UnityEngine.Profiling;

namespace HexFlow.NativeCore.Map
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MapCellData
    {
        public const string DllName = "Native_Main.dll";

        public Color color;
        public bool enabled;

        public MapCellData(Color color, bool enabled)
        {
            this.color = color;
            this.enabled = enabled;
        }

        [DllImport(DllName, EntryPoint = "extract_render_data_to_mesh")]
        public static extern unsafe void ExtractRenderDataToMesh(MapCellData* data, int dataLengh, Color* vertColor, Vector2* uv, int cellVertNum);

        public static void ApplyDataToMesh(NativeArray<MapCellData> data, Mesh target, HexMeshType type)
        {
            Profiler.BeginSample("ApplyDataToMesh");
            int vertNum = type.GetVertNum();

            var colorBuffer = new NativeArray<Color>(target.colors, Allocator.Temp);
            var uvBuffer = new NativeArray<Vector2>(target.uv, Allocator.Temp);
            unsafe
            {
                ExtractRenderDataToMesh(data.Get(), data.Length, colorBuffer.Get(), uvBuffer.Get(), vertNum);
            }
            target.SetColors(colorBuffer);
            target.SetUVs(0, uvBuffer);
            target.UploadMeshData(false);
            Profiler.EndSample();
        }

        public static void ApplyDataToMesh(IntPtr data, int cellCount, Mesh target, HexMeshType type)
        {
            Profiler.BeginSample("ApplyDataToMesh");
            int vertNum = type.GetVertNum();
            int totalVertNum = vertNum * cellCount;

            NativeArray<Color> colorBuffer = new NativeArray<Color>(totalVertNum, Allocator.Persistent);
            NativeArray<Vector2> uvBuffer = UnsafeUtils.CreateArrayFromSourceOrNew(target.uv, totalVertNum);
            Profiler.BeginSample("ExtractMapDataToBuffer");
            unsafe
            {
                ExtractRenderDataToMesh((MapCellData*)data, cellCount, colorBuffer.Get(), uvBuffer.Get(), vertNum);
            }
            Profiler.EndSample();
            target.SetColors(colorBuffer);
            target.SetUVs(0, uvBuffer);
            colorBuffer.Dispose();
            uvBuffer.Dispose();
            target.UploadMeshData(false);
            Profiler.EndSample();
        }

        public static void ApplyDataToMesh(INative2DArray<MapCellData> rectArea, Mesh target, HexMeshType type)
        {
            ApplyDataToMesh(rectArea.RawPtr, rectArea.Size.x * rectArea.Size.y, target, type);
        }
    }
}
