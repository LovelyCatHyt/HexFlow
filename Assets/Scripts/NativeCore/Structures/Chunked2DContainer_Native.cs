using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HexFlow.NativeCore.Structures
{
    public class Chunked2DContainer_Native
    {
        public const string DllName = "Native_Main.dll";

        [DllImport(DllName, EntryPoint = "chunked_2d_container_ctor_0")]
        public static extern IntPtr Ctor_0(int chunkSize, int elementSize);
        
        [DllImport(DllName, EntryPoint = "chunked_2d_container_dtor")]
        public static extern void Dtor(IntPtr ptr);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_chunk_count")]
        public static extern int ChunkCount(IntPtr ptr);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_exist_chunk")]
        public static extern bool ExistChunk(IntPtr ptr, Vector2Int chunkPos);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_exist_cell")]
        public static extern bool ExistCell(IntPtr ptr, Vector2Int cellPos);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_get_chunk_data")]
        public static extern IntPtr GetChunkData(IntPtr ptr, Vector2Int chunkPos);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_create_chunk")]
        public static extern IntPtr CreateChunk(IntPtr ptr, Vector2Int chunkPos);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_remove_chunk")]
        public static extern bool RemoveChunk(IntPtr ptr, Vector2Int chunkPos);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_get_cell_data")]
        public static extern IntPtr GetCellData(IntPtr ptr, Vector2Int cellPos);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_set_cell_data")]
        public static extern void SetCellData(IntPtr ptr, Vector2Int cellPos, IntPtr dataPtr);
        [DllImport(DllName, EntryPoint = "chunked_2d_container_create_iter")]
        public static extern IntPtr CreateIter(IntPtr ptr);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_iter_valid")]
        public static extern bool IterValid(IntPtr ptr, IntPtr iterPtr);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_iter_advance")]
        public static extern void IterAdvance(IntPtr iterPtr);
        
        [DllImport(DllName, EntryPoint = "chunked_2d_container_iter_key")]
        public static extern Vector2Int IterKey(IntPtr iterPtr);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_iter_value")]
        public static extern IntPtr IterValue(IntPtr iterPtr);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_iter_delete")]
        public static extern void IterDelete(IntPtr iterPtr);

        [DllImport(DllName, EntryPoint = "chunked_2d_container_iter_reset")]
        public static extern void IterReset(IntPtr ptr, IntPtr iterPtr);

    }
}

