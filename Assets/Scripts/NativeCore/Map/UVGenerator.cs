using HexFlow.NativeCore.Map;
using HexFlow.NativeCore.Structures;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace HexFlow.NativeCore
{
    public class UVGenerator
    {
        public const string DllName = "Native_Main.dll";

        [DllImport(DllName, EntryPoint = "hex_mesh_gen_simple_uv_4t")]
        public static extern unsafe void GenSimpleUV_4T(Vector2* uv, Vector2Int chunkPos, IntPtr chunkContainer);

        [DllImport(DllName, EntryPoint = "hex_mesh_gen_simple_uv_6t")]
        public static extern unsafe void GenSimpleUV_6T(Vector2* uv, Vector2Int chunkPos, IntPtr chunkContainer);

        [DllImport(DllName, EntryPoint = "hex_mesh_gen_connective_uv_6t")]
        public static extern unsafe void GenConnectiveUV_6T(Vector2* uv, Vector2Int chunkPos, IntPtr chunkContainer);

        public static void GenerateUVForMap([NotNull] Mesh mesh, [NotNull] Chunked2DContainer<MapCellData> map, Vector2Int chunkPos, HexMeshType meshType, HexTextureType texType)
        {
            Profiler.BeginSample("GenerateUVForMap");
            // TODO: 尝试复用模型里的内存？
            NativeArray<Vector2> uvs = new NativeArray<Vector2>(mesh.vertexCount, Allocator.Temp);
            switch (meshType)
            {
                case HexMeshType.UniformTriangle:
                    // 支持所有 HexTextureType
                    switch (texType)
                    {
                        case HexTextureType.SimpleRepeat:
                            unsafe
                            {
                                GenSimpleUV_6T(uvs.Get(), chunkPos, map.NativePtr);
                            }
                            break;
                        case HexTextureType.Connective:
                            unsafe
                            {
                                GenConnectiveUV_6T(uvs.Get(), chunkPos, map.NativePtr);
                            }
                            break;
                        default:
                            unsafe
                            {
                                GenSimpleUV_6T(uvs.Get(), chunkPos, map.NativePtr);
                            }
                            break;
                    }
                    break;
                case HexMeshType.SymmetricalLeastTriangle:
                    // 3大1小的网格类型只能使用 SimpleRepeat 的纹理类型
                    // 如果有人填错配置却没有报错感觉会很难 debug, 不知道要在什么地方加点提示?
                    unsafe
                    {
                        GenSimpleUV_4T(uvs.Get(), chunkPos, map.NativePtr);
                    }
                    break;
            }
            mesh.SetUVs(0, uvs);
            mesh.UploadMeshData(false);
            Profiler.EndSample();
        }
    }
}
