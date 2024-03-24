#pragma once

#include "pch.h"

// [GeneratedCodeBegin]
// 请勿手动修改以下内容, 代码生成器会将其覆盖.
#include "chunked_2d_container.h"

extern "C"{
	API_DEF void* chunked_2d_container_ctor_0(int chunk_size, int element_size);

	API_DEF void chunked_2d_container_dtor(void* this_raw);

	API_DEF int chunked_2d_container_chunk_count(void* this_raw);

	API_DEF bool chunked_2d_container_exist_chunk(void* this_raw, vec2i_export chunk_pos);

	API_DEF bool chunked_2d_container_exist_cell(void* this_raw, vec2i_export cell_pos);

	API_DEF vec2i_export chunked_2d_container_cell2chunk(void* this_raw, vec2i_export cell_pos);

	API_DEF vec2i_export chunked_2d_container_chunk2cell(void* this_raw, vec2i_export chunk_pos);

	API_DEF void* chunked_2d_container_get_chunk_data(void* this_raw, vec2i_export chunk_pos);

	API_DEF void* chunked_2d_container_create_chunk(void* this_raw, vec2i_export chunk_pos);

	API_DEF bool chunked_2d_container_remove_chunk(void* this_raw, vec2i_export chunk_pos);

	API_DEF void* chunked_2d_container_get_cell_data(void* this_raw, vec2i_export cell_pos);

	API_DEF void chunked_2d_container_set_cell_data(void* this_raw, vec2i_export cell_pos, void* cell_data);

	API_DEF void* chunked_2d_container_create_iter(void* this_raw);

	API_DEF bool chunked_2d_container_iter_valid(void* this_raw, void* iter_raw);

	API_DEF void chunked_2d_container_iter_reset(void* this_raw, void* iter_raw);

	API_DEF void chunked_2d_container_iter_delete(void* iter_raw);

	API_DEF void chunked_2d_container_iter_advance(void* iter_raw);

	API_DEF vec2i_export chunked_2d_container_iter_key(void* iter_raw);

	API_DEF void* chunked_2d_container_iter_value(void* iter_raw);

}

// 下一行注释之后可以任意添加用户内容, 代码生成器不会修改. 
// [GeneratedCodeEnd]
