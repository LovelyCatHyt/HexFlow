#pragma once

#include "pch.h"
#include "math_type.h"
#include "chunked_2d_container.h"

/// <summary>
/// 直接提供 9 个 chunk 的缓存数据指针和尺寸数据, 即可提供对中心区块及其邻居范围内的快速访问.
/// <para> 和直接访问 chunked_2d_container 相比, 绕开了 unordered_map 的访问.
/// 注意: 在无效位置返回 nullptr
/// </summary>
struct neighbor_chunk_visitor
{
public:
    
    void* chunks[9];
    int32 chunk_size;
    int32 element_size;
    vector2i offset;
    
    neighbor_chunk_visitor(int32 _chunk_size, int32 _element_size, vector2i _offset)
    {
        for (size_t i = 0; i < 9; i++)
        {
            chunks[i] = nullptr;
        }

        chunk_size = _chunk_size;
        element_size = _element_size;
        offset = _offset;
    }

    neighbor_chunk_visitor(void* _chunks[9], int32 _chunk_size, int32 _element_size, vector2i _offset)
    {
        for (size_t i = 0; i < 9; i++)
        {
            chunks[i] = _chunks[i];
        }

        chunk_size = _chunk_size;
        element_size = _element_size;
        offset = _offset;
    }
    neighbor_chunk_visitor(void* chunk0, void* chunk1, void* chunk2,
        void* chunk3, void* chunk4, void* chunk5,
        void* chunk6, void* chunk7, void* chunk8,
        int32 _chunk_size, int32 _element_size, vector2i _offset)
    {
        chunks[0] = chunk0;
        chunks[1] = chunk1;
        chunks[2] = chunk2;
        chunks[3] = chunk3;
        chunks[4] = chunk4;
        chunks[5] = chunk5;
        chunks[6] = chunk6;
        chunks[7] = chunk7;
        chunks[8] = chunk8;

        chunk_size = _chunk_size;
        element_size = _element_size;
        offset = _offset;
    }


    void* get_cell_ptr(vector2i cell_pos) const;

    template<typename T>
    T get_cell_data(vector2i cell_pos) const
    {
        auto cell_ptr = get_cell_ptr(cell_pos);
        if(!cell_ptr) return T();
        return *(T*)cell_ptr;
    }

    static neighbor_chunk_visitor make_visitor_at_chunk(chunked_2d_container* container, vector2i chunk_pos);
};

