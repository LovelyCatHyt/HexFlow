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

}
