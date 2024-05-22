#pragma once

#include "pch.h"
#include "hex_math.h"
#include "chunked_2d_container.h"

// 地形生成相关的函数

extern "C" 
{
    void API_DEF terr_gen_simple_perlin(void* data_ptr, int32 chunk_size, vector2i chunk_pos, vector2f noise_scale, vector2f noise_offset, int wave_num, float enable_thres);
}
