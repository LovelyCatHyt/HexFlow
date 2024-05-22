#include "pch.h"
#include "terrain_gen.h"
#include "hex_map.h"
#include "perlin_noise.hpp"

const siv::BasicPerlinNoise<float> STD_PERLIN_NOISE;

void terr_gen_simple_perlin(void* data_ptr, int32 chunk_size, vector2i chunk_pos, vector2f noise_scale, vector2f noise_offset, int wave_num, float enable_thres = 0.25f)
{
    if(!data_ptr || !chunk_size) return;

    vector2i base_cell = chunk_pos * chunk_size;
    map_cell_data* base_ptr = (map_cell_data*)data_ptr;

    // 算 wave_num 组柏林噪声的叠加, 每次频率翻倍, 幅度减半
    uint32 total_length = chunk_size * chunk_size;
    float* noise_cache = new float[total_length];
    // 不初始化就会烫烫烫了
    std::memset((void*)noise_cache, 0, total_length * sizeof(float));
    wave_num = std::max(wave_num, 1);
    float freq = 1;
    // 所有噪声波形的总幅值, 用于后面的归一化, 使其叠加的总和最大为 1
    float ampSum = 0;
    for (uint8 i = 0; i < wave_num; i++)
    {
        float amplitude = 1 / freq;
        ampSum += amplitude;
        for (int32 y = 0; y < chunk_size; y++)
        {
            for (int32 x = 0; x < chunk_size; x++)
            {
                float sample_x = (base_cell.x + x) * noise_scale.x + noise_offset.x;
                float sample_y = (base_cell.y + y) * noise_scale.y + noise_offset.y;
                noise_cache[y * chunk_size + x] += STD_PERLIN_NOISE.noise2D_01(sample_x * freq, sample_y * freq) * amplitude;
            }
        }
        freq *= 2;
    }

    // 基于上面算好的噪声数据
    for (int32 y = 0; y < chunk_size; y++)
    {
        for (int32 x = 0; x < chunk_size; x++)
        {
#pragma warning(push)
#pragma warning(disable: 6385) // C6385: 正在从 noise_cache 读取无效数据
            // 缓存的数组尺寸可以和上面的构造对齐, 因此忽略 C6385
            float value = noise_cache[y * chunk_size + x] / ampSum;
#pragma warning(pop)

            auto& target_cell = base_ptr[y * chunk_size + x];
            target_cell.enabled = value > enable_thres;
            value = std::min(value, 1.f);
            target_cell.color = color(value, value, value, value);
        }
    }

    delete[] noise_cache;
}

