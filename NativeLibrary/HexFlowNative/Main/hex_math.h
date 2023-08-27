#pragma once

#include "pch.h"
#include "math_type.h"

// Great thanks to this website: https://www.redblobgames.com/grids/hexagons

vector2i offset2axial(vector2i offset);

vector2i axial2offset(vector2i axial);

vector3i axial2cube(vector2i axial);

vector2i cube2axial(vector3i cube);

vector3f axial2position(vector2i axial);

vector2i position2axial(vector2f position, float cell_size);

int axial_distance(vector2i a, vector2i b);

vector3i cube_round(vector3f cube_frac);

