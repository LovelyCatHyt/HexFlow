#include "pch.h"
#include "CppUnitTest.h"
#include "hex_math.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

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
    };
}
