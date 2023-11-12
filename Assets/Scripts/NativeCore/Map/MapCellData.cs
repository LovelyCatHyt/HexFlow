using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Collections;
using HexFlow.NativeCore.Structures;
using System;

namespace HexFlow.NativeCore.Map
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MapCellData
    {
        public const string DllName = "Native_Main.dll";

        public Color color;
        public bool enabled;

        [DllImport(DllName, EntryPoint = "extract_render_data_to_mesh")]
        public static extern unsafe void ExtractRenderDataToMesh(MapCellData* data, int dataLengh, Color* vertColor, Vector2* uv, int cellVertNum);

        public static void ApplyDataToMesh(NativeArray<MapCellData> data, Mesh target, HexMeshType type)
        {
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
        }

        public static void ApplyDataToMesh(IntPtr data, int cellCount, Mesh target, HexMeshType type)
        {
            int vertNum = type.GetVertNum();

            var colorBuffer = new NativeArray<Color>(target.colors, Allocator.Temp);
            var uvBuffer = new NativeArray<Vector2>(target.uv, Allocator.Temp);
            unsafe
            {
                ExtractRenderDataToMesh((MapCellData*)data, cellCount, colorBuffer.Get(), uvBuffer.Get(), vertNum);
            }
            target.SetColors(colorBuffer);
            target.SetUVs(0, uvBuffer);
            target.UploadMeshData(false);
        }

        public static void ApplyDataToMesh(INative2DArray<MapCellData> rectArea, Mesh target, HexMeshType type)
        {
            ApplyDataToMesh(rectArea.RawPtr, rectArea.Size.x * rectArea.Size.y, target, type);
        }
    }
}
