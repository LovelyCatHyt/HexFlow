#pragma once

#include "pch.h"
#include "math_type.h"

// Great thanks to this website: https://www.redblobgames.com/grids/hexagons
extern "C" {

    vec2i_export API_DEF offset2axial(vector2i offset);

    vec2i_export API_DEF axial2offset(vector2i axial);

    vec3i_export API_DEF axial2cube(vector2i axial);

    vec2i_export API_DEF cube2axial(vector3i cube);

    vec2f_export API_DEF axial2position(vector2i axial);

    vec2i_export API_DEF position2axial(vector2f position, float cell_size);

    int API_DEF axial_distance(vector2i a, vector2i b);

    vec3i_export API_DEF cube_round(vector3f cube_frac);

    vec2i_export API_DEF axial_rot_left_1(vector2i axial);
    vec2i_export API_DEF axial_rot_left_2(vector2i axial);
    vec2i_export API_DEF axial_rot_right_1(vector2i axial);
    vec2i_export API_DEF axial_rot_right_2(vector2i axial);

}

// 从右边开始, 逆时针方向的六个 Axial 向量
const vector2i AxialDirs[] = 
{
    vector2i(1, 0),
    vector2i(0, 1),
    vector2i(-1, 1),
    vector2i(-1, 0),
    vector2i(0, -1),
    vector2i(1, -1),
};
