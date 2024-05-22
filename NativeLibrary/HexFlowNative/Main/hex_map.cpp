#include "pch.h"
#include "hex_map.h"

bool is_valid_hex_map(chunked_2d_container* container)
{
    return container && container->element_size == sizeof(map_cell_data);
}
