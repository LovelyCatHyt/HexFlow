#include "pch.h"
#include "CppUnitTest.h"
#include "hex_math.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

template<typename TData>
std::wstring ToString(const vector2_template<TData>& vec)
{
    return std::to_wstring(vec.x) + L"," + std::to_wstring(vec.y);
}

template<typename TData>
std::wstring ToString(const vector3_template<TData>& vec)
{
    return std::to_wstring(vec.x) + L"," + std::to_wstring(vec.y) + L"," + std::to_wstring(vec.z);
}

namespace UnitTest
{

    

    TEST_CLASS(hex_math)
    {
    public:

        TEST_METHOD(TestConvert)
        {
            for (int x = -10; x <= 10; x++)
            {
                for (int y = -0; y <= 10; y++)
                {
                    vector2i offset(x, y);
                    if (offset != (vector2i)axial2offset(offset2axial(offset))) Assert::Fail();
                }
            }

            for (int x = -10; x <= 10; x++)
            {
                for (int y = -0; y <= 10; y++)
                {
                    vector2i axial(x, y);
                    if (axial != (vector2i)offset2axial(axial2offset(axial))) Assert::Fail();
                }
            }

            for (int x = -10; x <= 10; x++)
            {
                for (int y = -0; y <= 10; y++)
                {
                    vector2i axial(x, y);
                    if (axial != (vector2i)cube2axial(axial2cube(axial))) Assert::Fail();
                }
            }

            for (int x = -10; x <= 10; x++)
            {
                for (int y = -0; y <= 10; y++)
                {
                    vector3i cube(x, y, -x - y);
                    if (cube != (vector3i)axial2cube(cube2axial(cube))) Assert::Fail();
                }
            }
        }

        // TODO: 与直角坐标系互转的函数貌似比较难做纯代码的单测, 大概只能实际运行看看

        TEST_METHOD(TestRotate)
        {

            for (int i = 0; i < 6; i++)
            {
                auto init = AxialDirs[i];
                auto msg_str = L"index = " + std::to_wstring(i);
                auto* msg = msg_str.c_str();
                Assert::AreEqual(AxialDirs[(i + 1) % 6], (vector2i)axial_rot_left_1(init), msg);
                Assert::AreEqual(AxialDirs[(i + 2) % 6], (vector2i)axial_rot_left_2(init), msg);
                Assert::AreEqual(AxialDirs[(i + 5) % 6], (vector2i)axial_rot_right_1(init), msg);
                Assert::AreEqual(AxialDirs[(i + 4) % 6], (vector2i)axial_rot_right_2(init), msg);
            }
        }
    };
}
