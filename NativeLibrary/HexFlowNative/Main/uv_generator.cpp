#include "pch.h"
#include "uv_generator.h"
#include "map.h"
#include "hex_map.h"
#include "mesh_generator.h"

// 可以根据 HexFlow\NativeLibrary\Readme.md 里的图和 
// HexFlow\NativeLibrary\HexFlowNative\Main\mesh_generator.cpp 里的函数确定不同网格类型里顶点的顺序

// 在正方形的内接正六边形的 uv 坐标数组, 其中六边形的尖顶分别与上下边界的中心相接
const vector2f uv_of_hex[] = 
{
    get_vec2_from_angle(30)  * 0.5f + vector2f(0.5f, 0.5f),
    get_vec2_from_angle(90)  * 0.5f + vector2f(0.5f, 0.5f),
    get_vec2_from_angle(150) * 0.5f + vector2f(0.5f, 0.5f),
    get_vec2_from_angle(210) * 0.5f + vector2f(0.5f, 0.5f),
    get_vec2_from_angle(270) * 0.5f + vector2f(0.5f, 0.5f),
    get_vec2_from_angle(330) * 0.5f + vector2f(0.5f, 0.5f)
};

// 构建滑动窗口完整数据
void make_window(map_cell_data neighbors[6], map_cell_data& center, const vector2i& cell_pos, /*NotNull*/ chunked_2d_container* container)
{
    center = get_cell_data_s<map_cell_data>(container, cell_pos);
    vector2i center_axial = offset2axial(cell_pos);
    for (int i = 0; i < 6; i++)
    {
        auto axial = center_axial + AxialDirs[i];
        neighbors[i] = get_cell_data_s<map_cell_data>(container, axial2offset(axial));
    }
}

// 滑动窗口, 向右移一格, 只需要读取3次数据
void move_window_add_col(map_cell_data neighbors[6], map_cell_data& center, const vector2i& next_cell_pos, /*NotNull*/ chunked_2d_container* container)
{
    neighbors[2]    = neighbors[1];
    neighbors[3]    = center;
    neighbors[4]    = neighbors[5];
    center          = neighbors[0];

    vector2i next_axial = offset2axial(next_cell_pos);
    vector2i target_axial;

    target_axial = next_axial + AxialDirs[0];
    neighbors[0]    = get_cell_data_s<map_cell_data>(container, axial2offset(target_axial));

    target_axial = next_axial + AxialDirs[1];
    neighbors[1] = get_cell_data_s<map_cell_data>(container, axial2offset(target_axial));

    target_axial = next_axial + AxialDirs[5];
    neighbors[5] = get_cell_data_s<map_cell_data>(container, axial2offset(target_axial));
}

void API_DEF hex_mesh_gen_simple_uv_4t(vector2f* uv_of_chunk, vector2i chunk_pos, chunked_2d_container* container)
{
    if(!uv_of_chunk) return;
    if(!is_valid_hex_map(container)) return;

    const int vert_count = 12;
    auto chunk_size = container->chunk_size;
    vector2i cell_start = container->chunk2cell(chunk_pos);
    for (int row = 0; row < chunk_size; row++)
    {
        for (int col = 0; col < chunk_size; col++)
        {
            auto data =  get_cell_data_s<map_cell_data>(container, vector2i(col, row) + cell_start);
            int start_id = (row * chunk_size + col) * vert_count;
            if (data.enabled)
            {
                // 中间大三角
                uv_of_chunk[start_id + 0] = uv_of_hex[3];
                uv_of_chunk[start_id + 1] = uv_of_hex[1];
                uv_of_chunk[start_id + 2] = uv_of_hex[5];
                start_id += 3;
                // 周边扁的三角形
                for (int i = 0; i < 3; i++)
                {
                    uv_of_chunk[start_id + i * 3 + 0] = uv_of_hex[(i * 2 + 5) % 6];
                    uv_of_chunk[start_id + i * 3 + 1] = uv_of_hex[(i * 2 + 1) % 6];
                    uv_of_chunk[start_id + i * 3 + 2] = uv_of_hex[(i * 2 + 0) % 6];
                }
            }
            else
            {
                // uv 全都是 0, 取到的纹理一定是透明的, 也就相当于透明格子
                for (int v = 0; v < vert_count; v++)
                {
                    uv_of_chunk[start_id + v] = vector2f(0, 0);
                }
            }
        }
    }
}

void API_DEF hex_mesh_gen_simple_uv_6t(vector2f* uv_of_chunk, vector2i chunk_pos, chunked_2d_container* container)
{
    if (!uv_of_chunk) return;
    if (!is_valid_hex_map(container)) return;
    
    const int vert_count = 18;
    auto chunk_size = container->chunk_size;
    vector2i cell_start = container->chunk2cell(chunk_pos);
    for (int row = 0; row < chunk_size; row++)
    {
        for (int col = 0; col < chunk_size; col++)
        {
            auto data = get_cell_data_s<map_cell_data>(container, vector2i(col, row) + cell_start);
            int start_id = (row * chunk_size + col) * vert_count;
            if (data.enabled)
            {
                for (int i = 0; i < 6; i++)
                {
                    uv_of_chunk[start_id + i * 3 + 0] = vector2f(.5f, .5f);
                    uv_of_chunk[start_id + i * 3 + 1] = uv_of_hex[i];
                    uv_of_chunk[start_id + i * 3 + 2] = uv_of_hex[(i + 5) % 6];
                }
            }
            else
            {
                for (int i = 0; i < vert_count; i++)
                {
                    uv_of_chunk[start_id + i] = vector2f(0, 0);
                }
            }
        }
    }
}

void API_DEF hex_mesh_gen_connective_uv_6t(vector2f* uv_of_chunk, vector2i chunk_pos, chunked_2d_container* container)
{
    if (!uv_of_chunk) return;
    if (!is_valid_hex_map(container)) return;

    const int vert_count = 18;
    auto chunk_size = container->chunk_size;
    vector2i cell_start = container->chunk2cell(chunk_pos);
    for (int row = 0; row < chunk_size; row++)
    {
        map_cell_data neighbors[6];
        map_cell_data center_data;
        make_window(neighbors, center_data, cell_start + vector2i(0, row), container);
        
        for (int col = 0; col < chunk_size; col++)
        {
            for (int i = 0; i < 6; i++)
            {
                int sub_area_id = 0;
                bool has_cell;

                // 分析四个位置是否存在 enabled 的格子
                has_cell = center_data.enabled;
                sub_area_id |= (int)has_cell;

                has_cell = neighbors[(i + 1) % 6].enabled;
                sub_area_id |= (int)has_cell << 1;

                has_cell = neighbors[(i + 0) % 6].enabled;
                sub_area_id |= (int)has_cell << 2;

                has_cell = neighbors[(i + 5) % 6].enabled;
                sub_area_id |= (int)has_cell << 3;


                int start_id = (row * chunk_size + col) * vert_count + i * 3;
                if(sub_area_id != 0)
                {
                    const vector2f uv0 = vector2f(1 - half_s3, 0.5f) * 0.25f;
                    const vector2f uv1 = vector2f(1, 1) * 0.25f;
                    const vector2f uv2 = vector2f(1, 0) * 0.25f;

                    float x = (sub_area_id % 4) * .25f;
                    float y = .75f - (sub_area_id / 4) * .25f;
                    vector2f uv_offset(x, y);

                    uv_of_chunk[start_id + 0] = uv0 + uv_offset;
                    uv_of_chunk[start_id + 1] = uv1 + uv_offset;
                    uv_of_chunk[start_id + 2] = uv2 + uv_offset;
                }
                else
                {
                    uv_of_chunk[start_id + 0] = vector2f(0, 0);
                    uv_of_chunk[start_id + 1] = vector2f(0, 0);
                    uv_of_chunk[start_id + 2] = vector2f(0, 0);
                }
            }

            move_window_add_col(neighbors, center_data, cell_start + vector2i(col + 1, row), container);
        }
    }
}
