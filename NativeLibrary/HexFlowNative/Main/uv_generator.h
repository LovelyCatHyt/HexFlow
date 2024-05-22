#pragma once

#include "pch.h"
#include "hex_math.h"
#include "chunked_2d_container.h"

extern "C"
{
    // 基于 4 三角形, 其中 1 大 3 小的, 作为单个单元的网格类型生成简单 uv
    void API_DEF hex_mesh_gen_simple_uv_4t(vector2f * uv_of_chunk, vector2i chunk_pos, chunked_2d_container* container);
    // 基于 6 全等三角形为单个单元的网格类型生成简单 uv
    void API_DEF hex_mesh_gen_simple_uv_6t(vector2f * uv_of_chunk, vector2i chunk_pos, chunked_2d_container* container);

    // 基于 6 全等三角形为单个单元的网格类型生成连接型 uv, 4t 网格类型不支持连接型 uv
    void API_DEF hex_mesh_gen_connective_uv_6t(vector2f * uv_of_chunk, vector2i chunk_pos, chunked_2d_container* container);
}
