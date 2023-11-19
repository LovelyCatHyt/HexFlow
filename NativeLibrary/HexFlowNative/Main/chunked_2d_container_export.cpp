#include "pch.h"

// [GeneratedCodeBegin]
// 请勿手动修改以下内容, 代码生成器会将其覆盖.
#include "chunked_2d_container_export.hpp"

void* chunked_2d_container_ctor_0(int chunk_size, int element_size)
{
	return (void*)(new chunked_2d_container(chunk_size, element_size));
}

void chunked_2d_container_dtor(void* this_raw)
{
	delete (chunked_2d_container*)this_raw;
}

int chunked_2d_container_chunk_count(void* this_raw)
{
	return ((chunked_2d_container*)this_raw)->chunk_count();
}

bool chunked_2d_container_exist_chunk(void* this_raw, vec2i_export chunk_pos)
{
	return ((chunked_2d_container*)this_raw)->exist_chunk(chunk_pos);
}

bool chunked_2d_container_exist_cell(void* this_raw, vec2i_export cell_pos)
{
	return ((chunked_2d_container*)this_raw)->exist_cell(cell_pos);
}

void* chunked_2d_container_get_chunk_data(void* this_raw, vec2i_export chunk_pos)
{
	return ((chunked_2d_container*)this_raw)->get_chunk_data(chunk_pos);
}

void* chunked_2d_container_create_chunk(void* this_raw, vec2i_export chunk_pos)
{
	return ((chunked_2d_container*)this_raw)->create_chunk(chunk_pos);
}

bool chunked_2d_container_remove_chunk(void* this_raw, vec2i_export chunk_pos)
{
	return ((chunked_2d_container*)this_raw)->remove_chunk(chunk_pos);
}

void* chunked_2d_container_get_cell_data(void* this_raw, vec2i_export cell_pos)
{
	return ((chunked_2d_container*)this_raw)->get_cell_data(cell_pos);
}

void chunked_2d_container_set_cell_data(void* this_raw, vec2i_export cell_pos, void* cell_data)
{
	((chunked_2d_container*)this_raw)->set_cell_data(cell_pos, cell_data);
}

void* chunked_2d_container_create_iter(void* this_raw)
{
	return ((chunked_2d_container*)this_raw)->create_iter();
}

bool chunked_2d_container_iter_valid(void* this_raw, void* iter_raw)
{
	return ((chunked_2d_container*)this_raw)->iter_valid(iter_raw);
}

void chunked_2d_container_iter_reset(void* this_raw, void* iter_raw)
{
	((chunked_2d_container*)this_raw)->iter_reset(iter_raw);
}

void chunked_2d_container_iter_delete(void* iter_raw)
{
	chunked_2d_container::iter_delete(iter_raw);
}

void chunked_2d_container_iter_advance(void* iter_raw)
{
	chunked_2d_container::iter_advance(iter_raw);
}

vec2i_export chunked_2d_container_iter_key(void* iter_raw)
{
	return chunked_2d_container::iter_key(iter_raw);
}

void* chunked_2d_container_iter_value(void* iter_raw)
{
	return chunked_2d_container::iter_value(iter_raw);
}


// 下一行注释之后可以任意添加用户内容, 代码生成器不会修改. 
// [GeneratedCodeEnd]
