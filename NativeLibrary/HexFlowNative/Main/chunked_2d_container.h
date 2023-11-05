#pragma once
#include "pch.h"

#include "math_type.h"
#include <unordered_map>

class chunked_2d_container
{

private:

    std::unordered_map<vector2i, void*> _map;

    vector2i cell2chunk(vector2i chunk_pos);

public:

    int chunk_size;
    int element_size;
    int chunk_data_size;

    chunked_2d_container(int chunk_size = 32, int element_size = sizeof(int))
    {
        this->chunk_size = chunk_size;
        this->element_size = element_size;
        chunk_data_size = chunk_size * chunk_size * element_size;
    }

    int chunk_count() { return (int)_map.size(); }

    bool exist_chunk(vec2i_export chunk_pos) { return _map.contains(chunk_pos); }

    bool exist_cell(vec2i_export cell_pos);

    void* get_chunk_data(vec2i_export chunk_pos);

    void* create_chunk(vec2i_export chunk_pos);

    void* get_cell_data(vec2i_export cell_pos);

    void set_cell_data(vec2i_export cell_pos, void* cell_data);
};
