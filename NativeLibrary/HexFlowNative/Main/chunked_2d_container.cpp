#include "pch.h"

#include <stdlib.h>
#include "math.hpp"
#include "chunked_2d_container.h"

vector2i chunked_2d_container::cell2chunk(vector2i chunk_pos)
{
    return vector2i(floor_div(chunk_pos.x, chunk_size), floor_div(chunk_pos.y, chunk_size));
}

bool chunked_2d_container::exist_cell(vec2i_export cell_pos)
{
    return exist_chunk(cell2chunk(cell_pos));
}

void* chunked_2d_container::get_chunk_data(vec2i_export chunk_pos)
{
    if (_map.contains(chunk_pos))
    {
        return _map[chunk_pos];
    }
    else
    {
        return nullptr;
    }
}

void* chunked_2d_container::create_chunk(vec2i_export chunk_pos)
{
    if (!_map.contains(chunk_pos))
    {
        _map.emplace(chunk_pos, malloc(chunk_data_size));
    }
    return _map[chunk_pos];
}

void* chunked_2d_container::get_cell_data(vec2i_export cell_pos)
{
    auto pos = vector2i(cell_pos);
    auto chunk_pos = cell2chunk(cell_pos);
    if (!exist_chunk(chunk_pos)) return nullptr;
    auto chunk_head = _map[chunk_pos];
    // 当前区块的基坐标, 即最小坐标
    auto base_pos = vector2i(chunk_pos.x * chunk_size, chunk_pos.y * chunk_size);
    // 以元素为单位的偏移量
    auto element_offset = (pos.y - base_pos.y) * chunk_size + (pos.x - base_pos.x);

    return (char*)chunk_head + (element_size * element_offset);
}

void chunked_2d_container::set_cell_data(vec2i_export cell_pos, void* cell_data)
{
    auto pos = vector2i(cell_pos);
    auto chunk_pos = cell2chunk(cell_pos);
    if (!exist_chunk(chunk_pos)) create_chunk(chunk_pos);
    auto chunk_head = _map[chunk_pos];
    // 当前区块的基坐标, 即最小坐标
    auto base_pos = vector2i(chunk_pos.x * chunk_size, chunk_pos.y * chunk_size);
    // 以元素为单位的偏移量
    auto element_offset = (pos.y - base_pos.y) * chunk_size + (pos.x - base_pos.x);

    auto cell_ptr = (char*)chunk_head + (element_size * element_offset);
    memcpy(cell_ptr, cell_data, element_size);
}
