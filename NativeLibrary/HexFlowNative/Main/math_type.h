#pragma once
#include <type_traits>

// 用于输出dll的函数的返回值定义
#define DEF_EXPORT_VECTOR(NAME, T, LENGTH) typedef struct {T data[LENGTH];}NAME

DEF_EXPORT_VECTOR(vec2i_export, int, 2);
DEF_EXPORT_VECTOR(vec2f_export, float, 2);
DEF_EXPORT_VECTOR(vec3i_export, int, 3);
DEF_EXPORT_VECTOR(vec3f_export, float, 3);

//// 非泛型类可以写类型转换 但是下面的模板和上面的类型之间的转换似乎很难写成模板, 因此用宏是比较简便的选择
//
//// 对非指针类型的强制类型重解释, 但类型安全和数据长度问题自己负责
//#define BIT_CAST_UNCHECKED(T_TO, INPUT) *reinterpret_cast<T_TO*>(&(INPUT))

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

    vector2_template(const vec2i_export& v)
    {
        static_assert(std::is_same<T, int>::value, "only vector2_template<int> can use this convertion.");
        x = v.data[0];
        y = v.data[1];
    }

    vector2_template(const vec2f_export& v)
    {
        static_assert(std::is_same<T, float>::value, "only vector2_template<float> can use this convertion.");
        x = v.data[0];
        y = v.data[1];
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

    template<typename T>
    bool operator==(const vector2_template<T>& r) const noexcept
    {
        return x == r.x && y == r.y;
    }

    template<typename T>
    bool operator!=(const vector2_template<T>& r) const noexcept
    {
        return x != r.x || y != r.y;
    }

    operator vec2i_export()
    {
        static_assert(std::is_same<T, int>::value, "only vector2_template<int> can use this convertion.");
        return *reinterpret_cast<vec2i_export*>(this);
    }
    operator vec2f_export()
    {
        static_assert(std::is_same<T, float>::value, "only vector2_template<int> can use this convertion.");
        return *reinterpret_cast<vec2f_export*>(this);
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

    vector3_template(const vec3i_export& v)
    {
        static_assert(std::is_same<T, int>::value, "only vector3_template<int> can use this convertion.");
        x = v.data[0];
        y = v.data[1];
        z = v.data[2];
    }

    vector3_template(const vec3f_export& v)
    {
        static_assert(std::is_same<T, float>::value, "only vector3_template<float> can use this convertion.");
        x = v.data[0];
        y = v.data[1];
        z = v.data[2];
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

    template<typename T>
    bool operator==(const vector3_template<T>& r) const noexcept
    {
        return x == r.x && y == r.y && z == r.z;
    }

    template<typename T>
    bool operator!=(const vector3_template<T>& r) const noexcept
    {
        return x != r.x || y != r.y || z != r.z;
    }

    operator vec3i_export()
    {
        static_assert(std::is_same<T, int>::value, "only vector3_template<int> can use this convertion.");
        return *reinterpret_cast<vec3i_export*>(this);
    }
    operator vec3f_export()
    {
        static_assert(std::is_same<T, float>::value, "only vector3_template<float> can use this convertion.");
        return *reinterpret_cast<vec3f_export*>(this);
    }
};

typedef vector2_template<float> vector2f;
typedef vector2_template<int> vector2i;

typedef vector3_template<int> vector3i;
typedef vector3_template<float> vector3f;

template<typename T>
struct std::hash<vector2_template<T>>
{
    size_t operator()(const vector2_template<T> vec2) const
    {
        std::hash<T> h;
        return h(vec2.x) ^ 31 + h(vec2.y);
    }
};

template<typename T>
struct std::hash<vector3_template<T>>
{
    size_t operator()(const vector3_template<T> vec3) const
    {
        std::hash<T> h;
        return h(vec3.xy) ^ 31 + h(vec3.z);
    }
};

typedef struct
{
    unsigned char r;
    unsigned char g;
    unsigned char b;
    unsigned char a;
}color32;

typedef struct
{
    float r;
    float g;
    float b;
    float a;
}color;

// math utils -----------------------------------------------

/// <summary>
/// sqrt 3
/// </summary>
const float s3 = 1.73205080756887729f;
/// <summary>
/// (sqrt 3) / 2
/// </summary>
const float half_s3 = 0.86602540378443864676f;
/// <summary>
/// 1 / (sqrt 3)
/// </summary>
const float i_s3 = 0.577350269189625764509148f;
