#pragma once

#include "pch.h"

// [GeneratedCodeBegin]
// 请勿手动修改以下内容, 代码生成器会将其覆盖.
#include "chunked_2d_container.h"

extern "C"{
	void* API_DEF chunked_2d_container_ctor_0(int chunk_size, int element_size)
	{
		return (void*)(new chunked_2d_container(chunk_size, element_size));
	}
	void API_DEF chunked_2d_container_dtor(void* this_raw)
	{
		delete (chunked_2d_container*)this_raw;
	}
	int API_DEF chunked_2d_container_chunk_count(void* this_raw)
	{
		return ((chunked_2d_container*)this_raw)->chunk_count();
	}
	bool API_DEF chunked_2d_container_exist_chunk(void* this_raw, vec2i_export chunk_pos)
	{
		return ((chunked_2d_container*)this_raw)->exist_chunk(chunk_pos);
	}
	bool API_DEF chunked_2d_container_exist_cell(void* this_raw, vec2i_export cell_pos)
	{
		return ((chunked_2d_container*)this_raw)->exist_cell(cell_pos);
	}
	void* API_DEF chunked_2d_container_get_chunk_data(void* this_raw, vec2i_export chunk_pos)
	{
		((chunked_2d_container*)this_raw)->get_chunk_data(chunk_pos);
	}
	void* API_DEF chunked_2d_container_create_chunk(void* this_raw, vec2i_export chunk_pos)
	{
		((chunked_2d_container*)this_raw)->create_chunk(chunk_pos);
	}
	void* API_DEF chunked_2d_container_get_cell_data(void* this_raw, vec2i_export cell_pos)
	{
		((chunked_2d_container*)this_raw)->get_cell_data(cell_pos);
	}
	void API_DEF chunked_2d_container_set_cell_data(void* this_raw, vec2i_export cell_pos, void* cell_data)
	{
		((chunked_2d_container*)this_raw)->set_cell_data(cell_pos, cell_data);
	}
}

// 下一行注释之后可以任意添加用户内容, 代码生成器不会修改. 
// [GeneratedCodeEnd]
