#include "pch.h"
#include "neighbor_chunk_visitor.h"

void* neighbor_chunk_visitor::get_cell_ptr(vector2i cell_pos) const
{
    auto local_pos = cell_pos - offset;
    if (local_pos.x < 0 || local_pos.x >= chunk_size * 3) return nullptr;
    if (local_pos.y < 0 || local_pos.y >= chunk_size * 3) return nullptr;

    auto chunk_x = local_pos.x / chunk_size;
    auto chunk_y = local_pos.y / chunk_size;

    // 转成具体类型方便加减
    uint8* chunk = (uint8*)chunks[chunk_y * 3 + chunk_x];
    if(!chunk) return nullptr;

    local_pos.x -= chunk_x * chunk_size;
    local_pos.y -= chunk_y * chunk_size;

    return (void*) &chunk[(local_pos.y * chunk_size + local_pos.x) * element_size];
}

neighbor_chunk_visitor neighbor_chunk_visitor::make_visitor_at_chunk(chunked_2d_container* container, vector2i chunk_pos)
{
    auto ret = neighbor_chunk_visitor(container->chunk_size, container->element_size, container->chunk2cell(chunk_pos - vector2i(1, 1)));

    ret.chunks[0] = container->get_chunk_data(chunk_pos + vector2i(-1, -1));
    ret.chunks[1] = container->get_chunk_data(chunk_pos + vector2i(0, -1));
    ret.chunks[2] = container->get_chunk_data(chunk_pos + vector2i(1, -1));
    ret.chunks[3] = container->get_chunk_data(chunk_pos + vector2i(-1, 0));
    ret.chunks[4] = container->get_chunk_data(chunk_pos + vector2i(0, 0));
    ret.chunks[5] = container->get_chunk_data(chunk_pos + vector2i(1, 0));
    ret.chunks[6] = container->get_chunk_data(chunk_pos + vector2i(-1, 1));
    ret.chunks[7] = container->get_chunk_data(chunk_pos + vector2i(0, 1));
    ret.chunks[8] = container->get_chunk_data(chunk_pos + vector2i(1, 1));

    return ret;
}
