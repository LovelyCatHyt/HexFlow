#pragma once

// _USRDLL 是 VS 创建 dll 项目时定义的宏, 其他环境不一定能用, 如果有更好的替代可以帮忙换一下
#ifdef _USRDLL
#   define API_DEF __declspec(dllexport)
#else
#   define API_DEF __declspec(dllimport)
#endif // COMPILER_EXPORTS

typedef unsigned char uint8;
typedef char int8;
typedef unsigned int uint32;
typedef int int32;
