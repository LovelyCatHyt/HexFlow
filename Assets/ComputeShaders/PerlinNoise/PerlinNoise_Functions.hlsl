#ifndef PERLIN_NOISE_FUNCTIONS
#define PERLIN_NOISE_FUNCTIONS

float Fade(float t)
{
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float Grad(uint hash, float x, float y, float z)
{
    uint h = hash & 15;
    float u = h < 8 ? x : y;
    float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

float Remap_01(float x)
{
    return x * 0.5 + 0.5;
}

// see perlin_noise.hpp in this repo
float Noise3D(StructuredBuffer<int> Permutation, float x, float y, float z)
{
    float _x = floor(x);
    float _y = floor(y);
    float _z = floor(z);

    int ix = int(_x) & 255;
    int iy = int(_y) & 255;
    int iz = int(_z) & 255;

    float fx = (x - _x);
    float fy = (y - _y);
    float fz = (z - _z);

    float u = Fade(fx);
    float v = Fade(fy);
    float w = Fade(fz);

    uint A = (Permutation[ix & 255] + iy) & 255;
    uint B = (Permutation[(ix + 1) & 255] + iy) & 255;

    uint AA = (Permutation[A] + iz) & 255;
    uint AB = (Permutation[(A + 1) & 255] + iz) & 255;

    uint BA = (Permutation[B] + iz) & 255;
    uint BB = (Permutation[(B + 1) & 255] + iz) & 255;

    float p0 = Grad(Permutation[AA], fx, fy, fz);
    float p1 = Grad(Permutation[BA], fx - 1, fy, fz);
    float p2 = Grad(Permutation[AB], fx, fy - 1, fz);
    float p3 = Grad(Permutation[BB], fx - 1, fy - 1, fz);
    float p4 = Grad(Permutation[(AA + 1) & 255], fx, fy, fz - 1);
    float p5 = Grad(Permutation[(BA + 1) & 255], fx - 1, fy, fz - 1);
    float p6 = Grad(Permutation[(AB + 1) & 255], fx, fy - 1, fz - 1);
    float p7 = Grad(Permutation[(BB + 1) & 255], fx - 1, fy - 1, fz - 1);

    float q0 = lerp(p0, p1, u);
    float q1 = lerp(p2, p3, u);
    float q2 = lerp(p4, p5, u);
    float q3 = lerp(p6, p7, u);

    float r0 = lerp(q0, q1, v);
    float r1 = lerp(q2, q3, v);

    return lerp(r0, r1, w);
}

#endif
