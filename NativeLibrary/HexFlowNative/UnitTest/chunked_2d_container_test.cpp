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
            // 没异常就是通过
            auto ptr = chunked_2d_container_ctor_0(32, sizeof(int));
            chunked_2d_container_dtor(ptr);
        }

        TEST_METHOD(TestCreateAndQuery)
        {
            auto ptr = chunked_2d_container_ctor_0(32, sizeof(int));

            Assert::IsNotNull(chunked_2d_container_create_chunk(ptr, vector2i(0, 1)));

            Assert::IsNotNull(chunked_2d_container_get_chunk_data(ptr, vector2i(0, 1)));

            Assert::AreEqual(1, chunked_2d_container_chunk_count(ptr));

            chunked_2d_container_dtor(ptr);
        }

        TEST_METHOD(TestDataAccess)
        {
            auto ptr = chunked_2d_container_ctor_0(32, sizeof(int));

            auto chunk_ptr = (int*)chunked_2d_container_create_chunk(ptr, vector2i(0, 1));

            Assert::AreEqual((void*)chunk_ptr, chunked_2d_container_get_chunk_data(ptr, vector2i(0, 1)));

            auto data = std::rand();
            // 如果分配不成功 那这个九成会报错
            chunk_ptr[32 * 32 - 1] = data;

            chunked_2d_container_dtor(ptr);
        }
    };
}
