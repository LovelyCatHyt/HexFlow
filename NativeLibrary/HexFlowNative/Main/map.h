#pragma once

#include "pch.h"
#include "math_type.h"
#include "hex_math.h"

#pragma warning(push)
#pragma warning(disable: 4091)

#pragma pack(push)
#pragma pack(8)
typedef struct
{
    color color;
    bool enabled;
}map_cell_data;
extern "C" {

API_DEF void extract_render_data_to_mesh(map_cell_data* data, int data_len, color* color_buff, vector2f* uv_buff, int vert_num);

}
//
//// test calling function from cpp
//typedef void (*void_func_int)(int);
//typedef int (*int_func_vec2i)(vec2i_export);
//typedef void (*void_func_charStar)(const char*);
//
//void_func_int func_test_int = NULL;
//int_func_vec2i func_test_vec2i = NULL;
//void_func_charStar func_test_char_array = NULL;
//
//extern "C" {
//    void API_DEF set_void_func_int(void_func_int func){func_test_int = func;}
//    void API_DEF set_int_func_vec2i(int_func_vec2i func) {func_test_vec2i = func;}
//    void API_DEF set_void_func_charStar(void_func_charStar func){func_test_char_array = func;}
//    void API_DEF test_func_ptr(int test_count)
//    {
//        int num = 114514;
//        if(!func_test_vec2i) return;
//        for (size_t i = 0; i < test_count; i++)
//        {
//            num ^= func_test_vec2i({ 19, 37 });
//        }
//    }
//}
#pragma pack(pop)

#pragma warning(pop)
