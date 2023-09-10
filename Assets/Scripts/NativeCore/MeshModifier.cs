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

        public static void SetMeshColor(Mesh mesh, HexMeshType type, NativeArray<Color32> array)
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
