#ifndef HEX_MATH
#define HEX_MATH

static float s3 = 1.73205080756887729f;
static float half_s3 = 0.86602540378443864676f;
static float i_s3 = 0.577350269189625764509148f;

int2 Offset2Axial(int2 offset)
{
    int q = offset.x - (offset.y - (offset.y & 1)) / 2;
    int r = offset.y;
    return int2(q, r);
}

int2 Axial2Offset(int2 axial)
{
    int col = axial.x + (axial.y - (axial.y & 1)) / 2;
    int row = axial.y;
    return int2(col, row);
}

int3 Axial2Cube(int2 axial)
{
    return int3(axial.x, axial.y, -axial.x - axial.y);
}

int2 Cube2Axial(int3 cube)
{
    return cube.xy;
}

float2 Axial2Position(int2 axial)
{
    float x = s3 * axial.x + half_s3 * axial.y;
    float y = 1.5f * axial.y;
    return float2(x, y);
}

int3 CubeRound(float3 cube_frac)
{
    int q = (int) round(cube_frac.x);
    int r = (int) round(cube_frac.y);
    int s = (int) round(cube_frac.z);

    float q_diff = abs(q - cube_frac.x);
    float r_diff = abs(r - cube_frac.y);
    float s_diff = abs(s - cube_frac.z);

    if (q_diff > r_diff && q_diff > s_diff)
        q = -r-s;
    else if (r_diff > s_diff)
        r = -q-s;
    else
        s = -q-r;

    return int3(q, r, s);
}

int3 Position2Cube(float2 position, float cell_size)
{
    float q = i_s3 * position.x - 1.0f / 3 * position.y;
    q /= cell_size;
    float r = 2.0f / 3 * position.y;
    r /= cell_size;
    return CubeRound(float3(q, r, -q - r));
}

int2 Position2Axial(float2 position, float cell_size)
{
    return Cube2Axial(Position2Cube(position, cell_size));
}

int AxialDistance(int2 a, int2 b)
{
    int2 diff = a - b;
    return (abs(diff.x) + abs(diff.x + diff.y) + abs(diff.y)) / 2;
}

int2 AxialRotLeft_1(int2 axial)
{
    int s = -axial.x - axial.y;
    return int2(-axial.y, -s);
}

int2 AxialRotLeft_2(int2 axial)
{
    int s = -axial.x - axial.y;
    return int2(s, axial.x);
}

int2 AxialRotRight_1(int2 axial)
{
    int s = -axial.x - axial.y;
    return int2(-s, -axial.x);
}

int2 AxialRotRight_2(int2 axial)
{
    int s = -axial.x - axial.y;
    return int2(axial.y, s);
}

void HexBoard(in float2 uv, in float size, out float3 boardType)
{
    int3 cube = Position2Cube(uv, size);
    
    boardType.x = float(cube.y & cube.z & 1);
    boardType.y = float(cube.x & cube.z & 1);
    boardType.z = float(cube.x & cube.y & 1);
}

void NearstHexCenter(in float2 uv, in float size, out float2 center)
{
    float _q = (sqrt(3.) / 3. * uv.x - 1. / 3. * uv.y) / size;
    float _r = 2. / 3. * uv.y / size;
    float _s = -_q - _r;
    
    float q = round(_q);
    float r = round(_r);
    float s = round(_s);

    float qd = abs(_q - q);
    float rd = abs(_r - r);
    float sd = abs(_s - s);
       
    if (qd > rd && qd > sd)
    {
        q = -r - s;
    }
    else if (rd > sd)
    {
        r = -q - s;
    }
    else
    {
        s = -q - r;
    }

    //int iq = int(q);
    //int ir = int(r);
    //int is = int(s);
    
    center.x = size * (sqrt(3.) * q + sqrt(3.) / 2. * r);
    center.y = size * (1.5 * r);
}

void HexCenterDist(in float2 uv, in float size, out float value)
{
    float2 center;
    NearstHexCenter(uv, size, center);
    
    float2 dist = (uv - center) / size;
    
    float angle = degrees(atan2(dist.y, dist.x));
    
    float fan_angle = radians(floor((angle + 30.) / 60.) * 60.);
    
    float2 fan_vec = float2(cos(fan_angle), sin(fan_angle));
    
    value = dot(dist, fan_vec) * 2. / sqrt(3.);
}

// region ShaderGraph

void HexBoard_half(in half2 uv, in half size, out half3 boardType)
{
    HexBoard(uv, size, boardType);
}
void HexBoard_float(in float2 uv, in float size, out float3 boardType)
{
    HexBoard(uv, size, boardType);
}

void NearstHexCenter_half(in half2 uv, in half size, out half2 center)
{
    NearstHexCenter(uv, size, center);
}
void NearstHexCenter_float(in float2 uv, in float size, out float2 center)
{
    NearstHexCenter(uv, size, center);
}

void HexCenterDist_half(in half2 uv, in half size, out half value)
{
    HexCenterDist(uv, size, value);
}
void HexCenterDist_float(in float2 uv, in float size, out float value)
{
    HexCenterDist(uv, size, value);
}

// endregion ShaderGraph

#endif
