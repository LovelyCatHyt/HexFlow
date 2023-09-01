using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HexFlow.NativeCore
{
    public static class MeshGenerator
    {
        public const string DllName = "Native_Main.dll";

        [DllImport(DllName, EntryPoint = "calc_num_of_rect_layout")]
        public static extern int CalcNumOfRectLayout(int width, int height);

        [DllImport(DllName, EntryPoint = "gen_rect_layout")]
        public static extern unsafe void GenerateRectLayout(Vector3* vertices, Vector3* normals, Vector2* uvs, int* indices, int width, int height, Vector2Int origin, float cell_size);

        public static Mesh GenerateRectLayout(int width, int height, Vector2Int origin, float cellSize = 1f, Mesh mesh = null)
        {
            if (!mesh) mesh = new Mesh();

            int num = CalcNumOfRectLayout(width, height);
            NativeArray<Vector3> vertices = new NativeArray<Vector3>(num, Allocator.Temp);
            NativeArray<Vector3> normals = new NativeArray<Vector3>(num, Allocator.Temp);
            NativeArray<Vector2> uvs = new NativeArray<Vector2>(num, Allocator.Temp);
            NativeArray<int> indices = new NativeArray<int>(num, Allocator.Temp);
            unsafe
            {
                GenerateRectLayout(vertices.Get(), normals.Get(), uvs.Get(), indices.Get(), width, height, origin, cellSize);
            }
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
