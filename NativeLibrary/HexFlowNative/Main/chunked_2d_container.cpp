#include "pch.h"

#include <stdlib.h>
#include "math.hpp"
#include "chunked_2d_container.h"

using iterator = std::unordered_map<vector2i, void*>::iterator;

vec2i_export chunked_2d_container::cell2chunk(vec2i_export cell_pos)
{
    vector2i _cell_pos = cell_pos;
    return vector2i(floor_div(_cell_pos.x, chunk_size), floor_div(_cell_pos.y, chunk_size));
}

vec2i_export chunked_2d_container::chunk2cell(vec2i_export chunk_pos)
{
    vector2i _chunk_pos = chunk_pos;
    return vector2i(_chunk_pos.x * chunk_size, _chunk_pos.y * chunk_size);
}

chunked_2d_container::~chunked_2d_container()
{
    for (auto& kv : _map)
    {    
        free(kv.second);
    }
}

bool chunked_2d_container::exist_chunk(vec2i_export chunk_pos) { return _map.contains(chunk_pos); }

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

bool chunked_2d_container::remove_chunk(vec2i_export chunk_pos)
{
    if (!_map.contains(chunk_pos)) return false;
    free(_map[chunk_pos]);
    _map.erase(chunk_pos);
    return true;
}

void* chunked_2d_container::get_cell_data(vec2i_export cell_pos)
{
    auto pos = vector2i(cell_pos);
    vector2i chunk_pos = cell2chunk(cell_pos);
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
    vector2i chunk_pos = cell2chunk(cell_pos);
    if (!exist_chunk(chunk_pos)) create_chunk(chunk_pos);
    auto chunk_head = _map[chunk_pos];
    // 当前区块的基坐标, 即最小坐标
    auto base_pos = vector2i(chunk_pos.x * chunk_size, chunk_pos.y * chunk_size);
    // 以元素为单位的偏移量
    auto element_offset = (pos.y - base_pos.y) * chunk_size + (pos.x - base_pos.x);

    auto cell_ptr = (char*)chunk_head + (element_size * element_offset);
    memcpy(cell_ptr, cell_data, element_size);
}

void* chunked_2d_container::create_iter()
{
    auto* it = new iterator();
    *it = _map.begin();
    return it;
}

void chunked_2d_container::iter_advance(void* iter_raw)
{
    auto it = (iterator*)iter_raw;
    ++*it;
}

bool chunked_2d_container::iter_valid(void* iter_raw)
{
    auto it = (iterator*)iter_raw;
    return *it != _map.end();
}

vec2i_export chunked_2d_container::iter_key(void* iter_raw)
{
    auto it = (iterator*)iter_raw;
    return (vector2i)(*it)->first;
}

void* chunked_2d_container::iter_value(void* iter_raw)
{
    auto it = (iterator*)iter_raw;
    return (*it)->second;
}

void chunked_2d_container::iter_reset(void* iter_raw)
{
    auto it = (iterator*)iter_raw;
    *it = _map.begin();
}

void chunked_2d_container::iter_delete(void* iter_raw)
{
    delete (iterator*)iter_raw;
}
