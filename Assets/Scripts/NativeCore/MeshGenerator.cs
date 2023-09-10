using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HexFlow.NativeCore
{

    public enum HexMeshType
    {
        /// <summary>
        /// 均匀的三角形: 组成的网格整体上由全等的等边三角形均匀分布
        /// <para>每格18顶点</para>
        /// </summary>
        UniformTriangle,
        /// <summary>
        /// 对称的最少三角形布局: 每个六边形仅由必须的4个三角形组成, 使用对称的1大3小布局
        /// <para>每格12顶点</para>
        /// </summary>
        SymmetricalLeastTriangle
    }

    public static class MeshGenerator
    {
        public const string DllName = "Native_Main.dll";

        [DllImport(DllName, EntryPoint = "calc_num_of_rect_layout_a")]
        internal static extern int CalcNumOfRectLayoutA(int width, int height);

        [DllImport(DllName, EntryPoint = "gen_rect_layout_a")]
        internal static extern unsafe void GenerateRectLayoutA(Vector3* vertices, Vector3* normals, Vector2* uvs, int* indices, int width, int height, Vector2Int origin, float cell_size);

        [DllImport(DllName, EntryPoint = "calc_num_of_rect_layout_b")]
        internal static extern int CalcNumOfRectLayoutB(int width, int height);

        [DllImport(DllName, EntryPoint = "gen_rect_layout_b")]
        internal static extern unsafe void GenerateRectLayoutB(Vector3* vertices, Vector3* normals, Vector2* uvs, int* indices, int width, int height, Vector2Int origin, float cell_size);

        public static Mesh GenerateRectLayout(int width, int height, Vector2Int origin, float cellRadius = 1f, Mesh mesh = null, HexMeshType type = HexMeshType.UniformTriangle)
        {
            if (!mesh) mesh = new Mesh();

            int num;
            switch (type)
            {
                case HexMeshType.UniformTriangle:
                    num = CalcNumOfRectLayoutA(width, height);
                    break;
                case HexMeshType.SymmetricalLeastTriangle:
                    num = CalcNumOfRectLayoutB(width, height);
                    break;
                default:
                    num = CalcNumOfRectLayoutA(width, height);
                    break;
            }
            
            NativeArray<Vector3> vertices = new NativeArray<Vector3>(num, Allocator.Temp);
            NativeArray<Vector3> normals = new NativeArray<Vector3>(num, Allocator.Temp);
            NativeArray<Vector2> uvs = new NativeArray<Vector2>(num, Allocator.Temp);
            NativeArray<int> indices = new NativeArray<int>(num, Allocator.Temp);
            unsafe
            {

                switch (type)
                {
                    case HexMeshType.UniformTriangle:
                        GenerateRectLayoutA(vertices.Get(), normals.Get(), uvs.Get(), indices.Get(), width, height, origin, cellRadius);
                        break;
                    case HexMeshType.SymmetricalLeastTriangle:
                        GenerateRectLayoutB(vertices.Get(), normals.Get(), uvs.Get(), indices.Get(), width, height, origin, cellRadius);
                        break;
                    default:
                        GenerateRectLayoutA(vertices.Get(), normals.Get(), uvs.Get(), indices.Get(), width, height, origin, cellRadius);
                        break;
                }
                
            }
            if (mesh.vertexCount != num) mesh.Clear(true);
            
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            mesh.MarkModified();
            mesh.UploadMeshData(false);
            return mesh;
        }
    }
}
