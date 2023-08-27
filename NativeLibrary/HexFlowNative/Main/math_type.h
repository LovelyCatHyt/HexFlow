#pragma once

template<typename T>
class vector2_template
{
public:
    union
    {
        struct
        {
            T x;
            T y;
        };
        T data[2];
    };

    /// <summary>
    /// 全零默认构造函数
    /// </summary>
    vector2_template() { x = 0; y = 0; }


    /// <summary>
    /// 构造函数, 避免括号构造表达式产生意外的行为
    /// </summary>
    vector2_template(T x, T y) noexcept
    {
        this->x = x;
        this->y = y;
    }

    template<typename T>
    vector2_template<T> operator+(const vector2_template<T>& r)
    {
        return vector2_template<T>(x + r.x, y + r.y);
    }

    template<typename T>
    vector2_template<T> operator-(const vector2_template<T>& r)
    {
        return vector2_template<T>(x - r.x, y - r.y);
    }
};

template<typename T>
class vector3_template
{
public:

    union
    {
        struct
        {
            T x;
            T y;
            T z;
        };
        vector2_template<T> xy;
        T data[3];
    };

    // 从二维构造三维的快捷方式
    vector3_template(const vector2_template<T>& v) noexcept
    {
        xy = v;
        z = 0;
    }

    /// <summary>
    /// 全零默认构造函数
    /// </summary>
    vector3_template() { x = 0; y = 0; z = 0; }

    /// <summary>
    /// 构造函数, 避免括号构造表达式产生意外的行为
    /// </summary>
    vector3_template(T x, T y, T z) noexcept
    {
        this->x = x;
        this->y = y;
        this->z = z;
    }

    template<typename T>
    vector3_template<T> operator+(const vector3_template<T>& r)
    {
        return vector3_template<T>(x + r.x, y + r.y, z + r.z);
    }

    template<typename T>
    vector3_template<T> operator-(const vector3_template<T>& r)
    {
        return vector3_template<T>(x - r.x, y - r.y, z - r.z);
    }
};

typedef vector2_template<float> vector2f;
typedef vector3_template<float> vector3f;

typedef vector2_template<int> vector2i;
typedef vector3_template<int> vector3i;
