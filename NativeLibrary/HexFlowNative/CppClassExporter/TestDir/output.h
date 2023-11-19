// [GeneratedCodeBegin]
// 请勿手动修改以下内容, 代码生成器会将其覆盖.
#include "test.hpp"

extern "C"{
	API_DEF void* chunked_2d_container_ctor_0(int chunk_size, int element_size);

	API_DEF void chunked_2d_container_dtor(void* this_raw);

	API_DEF int chunked_2d_container_chunk_count(void* this_raw);

	API_DEF bool chunked_2d_container_exist_chunk(void* this_raw, vec2i_export chunk_pos);

	API_DEF bool chunked_2d_container_exist_cell(void* this_raw, vec2i_export cell_pos);

	API_DEF void* chunked_2d_container_get_chunk_data(void* this_raw, vec2i_export chunk_pos);

	API_DEF void* chunked_2d_container_create_chunk(void* this_raw, vec2i_export chunk_pos);

	API_DEF void* chunked_2d_container_get_cell_data(void* this_raw, vec2i_export cell_pos);

	API_DEF void chunked_2d_container_set_cell_data(void* this_raw, vec2i_export cell_pos, void* cell_data);

}

// 下一行注释之后可以任意添加用户内容, 代码生成器不会修改. 
// [GeneratedCodeEnd]
