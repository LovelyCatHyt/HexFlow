using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

namespace HexFlow.NativeCore
{
    public static class MeshModifier
    {
        public const string DllName = "Native_Main.dll";

        [DllImport(DllName, EntryPoint = "set_vert_color_gridmap")]
        internal static extern unsafe void SetColorGridMap(Color* colors, Color* gridColors, int vertPerCell, int length);

        [DllImport(DllName, EntryPoint = "set_vert_color32_gridmap")]
        internal static extern unsafe void SetColor32GridMap(Color32* colors, Color32* gridColors, int vertPerCell, int length);

        /// <summary>
        /// 以单元格为最小单位设置网格颜色, 精度为<see cref="Color"/>, 即4个float
        /// </summary>
        public static void SetMeshColor(Mesh mesh, HexMeshType type, NativeArray<Color> array)
        {
            if(!mesh)
            {
                throw new MissingReferenceException("Mesh is null!");
            }

            NativeArray<Color> colors;

            unsafe
            {
                switch (type)
                {
                    case HexMeshType.UniformTriangle:
                        colors = new NativeArray<Color>(18 * array.Length, Allocator.Temp);
                        SetColorGridMap(colors.Get(), array.Get(), 18, array.Length);
                        break;
                    case HexMeshType.SymmetricalLeastTriangle:
                        colors = new NativeArray<Color>(12 * array.Length, Allocator.Temp);
                        SetColorGridMap(colors.Get(), array.Get(), 12, array.Length);
                        break;
                    default:
                        throw new ArgumentException($"Invalid HexMeshType: {type}");
                }
            }

            mesh.SetColors(colors);
            mesh.UploadMeshData(false);
        }

        /// <summary>
        /// 以单元格为最小单位设置网格颜色, 精度为<see cref="Color32"/>, 即4个uint8
        /// </summary>
        public static void SetMeshColor32(Mesh mesh, HexMeshType type, NativeArray<Color32> array)
        {
            if (!mesh)
            {
                throw new MissingReferenceException("Mesh is null!");
            }

            NativeArray<Color32> colors;

            unsafe
            {
                switch (type)
                {
                    case HexMeshType.UniformTriangle:
                        colors = new NativeArray<Color32>(18 * array.Length, Allocator.Temp);
                        SetColor32GridMap(colors.Get(), array.Get(), 18, array.Length);
                        break;
                    case HexMeshType.SymmetricalLeastTriangle:
                        colors = new NativeArray<Color32>(12 * array.Length, Allocator.Temp);
                        SetColor32GridMap(colors.Get(), array.Get(), 12, array.Length);
                        break;
                    default:
                        throw new ArgumentException($"Invalid HexMeshType: {type}");
                }
            }

            mesh.SetColors(colors);
            mesh.UploadMeshData(false);
        }
    }
}
