#include "pch.h"
#include <cmath>
#include "mesh_generator.h"

float to_rad = 3.1415926536f / 180.0f;
float to_deg = 180.0f / 3.1415926536f;

vector3f get_vec3_from_angle(float angle, float len)
{
    return vector3f(cos(angle * to_rad) * len, sin(angle * to_rad) * len, 0);
}

int calc_num_of_rect_layout_a(int width, int height)
{
    // 由六个三角形组成的六边形
    return width * height * 18;
}

int calc_num_of_rect_layout_b(int width, int height)
{
    // 由四个三角形组成的六边形
    return width * height * 12;
}

void gen_rect_layout_a(vector3f* vertices, vector3f* normals, vector2f* uvs, int* indices, int width, int height, vector2i origin, float cell_size)
{

    // 总顶点数
    auto num_vert = calc_num_of_rect_layout_a(width, height);
    const int vert_per_cell = 18;
    const vector3f verts_in_cell[] =
    {
        get_vec3_from_angle(30, cell_size),
        get_vec3_from_angle(90, cell_size),
        get_vec3_from_angle(150, cell_size),
        get_vec3_from_angle(210, cell_size),
        get_vec3_from_angle(270, cell_size),
        get_vec3_from_angle(330, cell_size),
    };

    // 为了能尽可能访问连续内存地址, 三个数组分开赋值
    // 坐标
    for (int row = 0; row < height; row++)
    {
        for (int col = 0; col < width; col++)
        {
            auto axial = offset2axial(vector2i(col + origin.x, row + origin.y));
            auto center = (vector2f)axial2position(axial);
            center.x *= cell_size;
            center.y *= cell_size;
            int start_id = (row * width + col) * vert_per_cell;
            for (size_t i = 0; i < 6; i++)
            {
                // 顺时针布局时正面朝向自己
                vertices[start_id + i * 3 + 0] = (vector3f)center;
                vertices[start_id + i * 3 + 1] = (vector3f)(center) + verts_in_cell[i];
                vertices[start_id + i * 3 + 2] = (vector3f)(center) + verts_in_cell[(i + 5) % 6];
            }
        }
    }

    // 法线
    // 方向全都一样
    for (size_t i = 0; i < num_vert; i++)
    {
        normals[i] = vector3f(0, 0, 1);
    }

    // 纹理坐标
    // 重复相同单元格, 且每个格子由六个相同三角形构成的情况下, uv 与位置无关
    for (size_t i = 0; i < num_vert; i += 3)
    {
        uvs[i] = vector2f(0.5f, half_s3);
        uvs[i + 1] = vector2f(1, 0);
        uvs[i + 2] = vector2f(0, 0);
    }

    // 索引
    // 拓扑结构为三角形的情况下非常简单
    for (size_t i = 0; i < num_vert; i++)
    {
        indices[i] = (int)i;
    }
}

void gen_rect_layout_b(vector3f* vertices, vector3f* normals, vector2f* uvs, int* indices, int width, int height, vector2i origin, float cell_size)
{
    // 总顶点数
    auto num_vert = calc_num_of_rect_layout_b(width, height);
    const int num_per_cell = 12;
    const vector3f verts_in_cell[] =
    {
        get_vec3_from_angle(30, cell_size),
        get_vec3_from_angle(90, cell_size),
        get_vec3_from_angle(150, cell_size),
        get_vec3_from_angle(210, cell_size),
        get_vec3_from_angle(270, cell_size),
        get_vec3_from_angle(330, cell_size),
    };

    // 为了能尽可能访问连续内存地址, 三个数组分开赋值
    // 坐标
    for (int row = 0; row < height; row++)
    {
        for (int col = 0; col < width; col++)
        {
            auto axial = offset2axial(vector2i(col + origin.x, row + origin.y));
            auto center = (vector2f)axial2position(axial);
            center.x *= cell_size;
            center.y *= cell_size;
            int start_id = (row * width + col) * num_per_cell;
            // 中心大三角形
            vertices[start_id + 0] = (vector3f)center + verts_in_cell[3];   // 左下角
            vertices[start_id + 1] = (vector3f)center + verts_in_cell[1];   // 正上方
            vertices[start_id + 2] = (vector3f)center + verts_in_cell[5];   // 右下角
            start_id += 3;
            // 周边扁的三角形
            for (size_t i = 0; i < 3; i++)
            {
                vertices[start_id + i * 3 + 0] = (vector3f)center + verts_in_cell[(i * 2 + 5) % 6];
                vertices[start_id + i * 3 + 1] = (vector3f)center + verts_in_cell[(i * 2 + 1) % 6];
                vertices[start_id + i * 3 + 2] = (vector3f)center + verts_in_cell[(i * 2 + 0) % 6];
            }
        }
    }

    // 法线
    // 方向全都一样
    for (size_t i = 0; i < num_vert; i++) normals[i] = vector3f(0, 0, 1);

    // 纹理坐标
    // 六边形的b型划分有两种三角形, 需要两套uv
    for (size_t hex_id = 0; hex_id < num_vert; hex_id += num_per_cell)
    {
        // 大三角形
        uvs[hex_id + 0] = vector2f(0, 0);
        uvs[hex_id + 1] = vector2f(1, 0);
        uvs[hex_id + 2] = vector2f(0.5f, half_s3);
        // 扁三角形
        for (size_t i = 0; i < 3; i++)
        {
            uvs[hex_id + 3 + i * 3 + 0] = vector2f(1, 0);
            uvs[hex_id + 3 + i * 3 + 1] = vector2f(0.5f, half_s3);
            uvs[hex_id + 3 + i * 3 + 2] = vector2f(1, 0.5f);
        }
    }

    // 索引
    // 没有共享顶点的情况下非常简单
    for (size_t i = 0; i < num_vert; i++) indices[i] = (int)i;
}

template<typename T>
void repeat_copy(T* dst, T* src, int repeat, int length)
{
    for (size_t i = 0; i < length; i++)
    {
        auto dst_ptr = dst + repeat * i;
        for (size_t dst_i = 0; dst_i < repeat; dst_i++)
        {
            dst_ptr[dst_i] = src[i];
        }
    }
}

void set_vert_color32_gridmap(color32* colors, color32* gridColors, int vert_per_cell, int length)
{
    repeat_copy(colors, gridColors, vert_per_cell, length);
}

void set_vert_color_gridmap(color* colors, color* gridColors, int vert_per_cell, int length)
{
    repeat_copy(colors, gridColors, vert_per_cell, length);
}
