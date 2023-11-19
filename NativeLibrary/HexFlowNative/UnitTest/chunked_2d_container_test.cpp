#include "pch.h"
#include "CppUnitTest.h"
#include "chunked_2d_container_export.hpp"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace UnitTest
{
    TEST_CLASS(chunked_2d_container_export)
    {
    public:

        TEST_METHOD(TestCtorDtor)
        {
            auto ptr = chunked_2d_container_ctor_0(32, sizeof(int));
            chunked_2d_container_dtor(ptr);
        }
    };
}
