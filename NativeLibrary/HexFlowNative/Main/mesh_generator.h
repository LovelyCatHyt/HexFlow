#pragma once
#include "pch.h"
#include "hex_math.h"

//typedef struct
//{
//    vector3f* vertices;
//    vector3f* normals;
//    // vector3f* tangents;
//    vector2f* uvs;
//}mesh_buffer;

extern "C"
{
    /// <summary>
    /// 根据长宽尺寸计算矩形布局网格的顶点数
    /// </summary>
    /// <returns>总顶点数</returns>
    int API_DEF calc_num_of_rect_layout(int width, int height);

    /// <summary>
    /// 生成矩形布局的六边形网格, 存储到 buffer 结构体保存的指针
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="origin">原点位置(offset 坐标), 生成时从该坐标开始向两个轴的正方向扩展</param>
    /// <param name="cell_size">单元格大小, 即顶点到单元格中心的距离</param>
    void API_DEF gen_rect_layout(vector3f* vertices, vector3f* normals, vector2f* uvs, int* indices, int width, int height, vector2i origin, float cell_size);

}