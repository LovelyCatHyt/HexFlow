#include "pch.h"
#include "CppUnitTest.h"
#include "math_type.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace UnitTest
{
    TEST_CLASS(math_type)
    {
    public:

        TEST_METHOD(TestStructSize)
        {
            vector2f v2;
            Assert::AreEqual(sizeof(float) * 2, sizeof(vector2f));
            Assert::AreEqual((void*)&v2, (void*)v2.data);

            vector3f v3;
            Assert::AreEqual(sizeof(float) * 3, sizeof(vector3f));
            Assert::AreEqual((void*)&v3, (void*)v3.data);
            Assert::AreEqual((void*)&v3, (void*)&v3.xy);

            // 相同模板的两个类型就不用管了
            /*vector2f i2;
            Assert::AreEqual(sizeof(int) * 2, sizeof(vector2f));
            Assert::AreEqual((void*)&i2, (void*)i2.data);

            vector3f i3;
            Assert::AreEqual(sizeof(float) * 3, sizeof(vector3f));
            Assert::AreEqual((void*)&i3, (void*)i3.data);*/
        }

        TEST_METHOD(TestStructConstruct)
        {
            vector2f v2(1, 2);
            vector3f v3(v2);
            Assert::AreEqual(v2.x, v3.x);
            Assert::AreEqual(v2.y, v3.y);
            Assert::AreEqual(0.0f, v3.z);
        }

        TEST_METHOD(TestAddAndSubstract)
        {
            vector2f v2 = vector2f(1, 2) + vector2f(3, 4);
            Assert::AreEqual(4.0f, v2.x);
            Assert::AreEqual(6.0f, v2.y);

            v2 = v2 - vector2f(5,6);
            Assert::AreEqual(-1.0f, v2.x);
            Assert::AreEqual(0.0f, v2.y);

            vector3f v3 = vector3f(1, 2, 3) + vector3f(4,5,6);
            Assert::AreEqual(5.0f, v3.x);
            Assert::AreEqual(7.0f, v3.y);
            Assert::AreEqual(9.0f, v3.z);

            v3 = v3 - vector3f(7,8,9);
            Assert::AreEqual(-2.0f, v3.x);
            Assert::AreEqual(-1.0f, v3.y);
            Assert::AreEqual(0.0f, v3.z);
        }
    };
}
