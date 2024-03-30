#pragma once
#include "pch.h"

#include "math_type.h"
#include <unordered_map>

class chunked_2d_container
{

private:

    std::unordered_map<vector2i, void*> _map;

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

    ~chunked_2d_container();

    int chunk_count() { return (int)_map.size(); }

    bool exist_chunk(vec2i_export chunk_pos);

    bool exist_cell(vec2i_export cell_pos);

    vec2i_export cell2chunk(vec2i_export cell_pos);

    vec2i_export chunk2cell(vec2i_export chunk_pos);

    void* get_chunk_data(vec2i_export chunk_pos);

    void* create_chunk(vec2i_export chunk_pos);

    bool remove_chunk(vec2i_export chunk_pos);

    void* get_cell_data(vec2i_export cell_pos);

    void set_cell_data(vec2i_export cell_pos, void* cell_data);

#pragma region Iterator
    void* create_iter();

    static void iter_delete(void* iter_raw);

    static void iter_advance(void* iter_raw);

    bool iter_valid(void* iter_raw);

    static vec2i_export iter_key(void* iter_raw);

    static void* iter_value(void* iter_raw);

    void iter_reset(void* iter_raw);
#pragma endregion
};

template<typename T>
T get_cell_data_s(chunked_2d_container* container, vector2i cell_pos)
{
    T* ptr = static_cast<T*>(container->get_cell_data(cell_pos));
    if(ptr) return *ptr;
    return T();
}
