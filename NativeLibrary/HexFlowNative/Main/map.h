#pragma once

#include "pch.h"
#include "math_type.h"
#include "hex_math.h"

#pragma warning(push)
#pragma warning(disable: 4091)

#pragma pack(push)
#pragma pack(8)
typedef struct 
{
    color color;
    bool enabled;
}map_cell_data;

void extract_render_data_to_mesh(map_cell_data* data, int data_len, color* color_buff, vector2f* uv_buff, int vert_num);

#pragma pack(pop)

#pragma warning(pop)
