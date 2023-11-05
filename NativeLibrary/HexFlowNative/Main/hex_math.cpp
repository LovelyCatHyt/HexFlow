#include "pch.h"
#include <cmath>
#include "hex_math.h"

vec2i_export API_DEF offset2axial(vector2i offset)
{
    auto q = offset.x - (offset.y - (offset.y & 1)) / 2;
    auto r = offset.y;
    return vector2i(q, r);
}

vec2i_export API_DEF axial2offset(vector2i axial)
{
    auto col = axial.x + (axial.y - (axial.y & 1)) / 2;
    auto row = axial.y;
    return vector2i(col, row);
}

vec3i_export API_DEF axial2cube(vector2i axial)
{
    return vector3i(axial.x, axial.y, -axial.x - axial.y);
}

vec2i_export API_DEF cube2axial(vector3i cube)
{
    return cube.xy;
}

vec2f_export API_DEF axial2position(vector2i axial)
{
    auto x = s3 * axial.x + half_s3 * axial.y;
    auto y = 1.5f * axial.y;
    return vector2f(x, y);
}

vec2i_export API_DEF position2axial(vector2f position, float cell_size)
{
    auto q = i_s3 * position.x - 1.0f / 3 * position.y;
    q /= cell_size;
    auto r = 2.0f / 3 * position.y;
    r /= cell_size;
    return cube2axial(cube_round(vector3f(q, r, -q - r)));
}

int API_DEF axial_distance(vector2i a, vector2i b)
{
    auto diff = a - b;
    return (abs(diff.x) + abs(diff.x + diff.y) + abs(diff.y)) / 2;
}

vec3i_export API_DEF cube_round(vector3f cube_frac)
{
    auto q = (int)round(cube_frac.x);
    auto r = (int)round(cube_frac.y);
    auto s = (int)round(cube_frac.z);

    auto q_diff = abs(q - cube_frac.x);
    auto r_diff = abs(r - cube_frac.y);
    auto s_diff = abs(s - cube_frac.z);

    if (q_diff > r_diff && q_diff > s_diff)
        q = -r - s;
    else if (r_diff > s_diff)
        r = -q - s;
    else
        s = -q - r;

    return vector3i(q, r, s);
}
