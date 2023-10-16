#include "pch.h"
#include "map.h"

void extract_render_data_to_mesh(map_cell_data* data, int data_len, color* color_buff, vector2f* uv_buff, int vert_num)
{
    for (size_t i = 0; i < data_len; i++)
    {
        for (int vert = 0; vert < vert_num; vert++)
        {
            color_buff[i*vert_num + vert] = data[i].color;
            // 暂时没 uv 数据, 下次一定
            // uv_buff[i*vert_num + vert] = data[i].
        }
    }
}
