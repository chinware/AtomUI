# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.
#
# FindGoogleTest
# -----------------
#
# Try to locate the GoogleTest source files, and then build them as a
# static library.
#
# The ``GOOGLETEST_DIR`` (CMake or Environment) variable should be used
# to pinpoint the GoogleTest source files.
#
# If found, this will define the following variables:
#
# ``GoogleTest_FOUND``
#     True if the GoogleTest source package has been found.
#
# ``GoogleTest``
#     Target compiled as static library.
#

find_path(GOOGLE_TEST_INCLUDE_DIR
   NAMES gtest/gtest.h
   PATH_SUFFIXES googletest/include
   HINTS
   "${GOOGLETEST_DIR}" ENV GOOGLETEST_DIR
   "${PROJECT_SOURCE_DIR}/googletest"
   "${PROJECT_SOURCE_DIR}/../googletest"
   "${PROJECT_SOURCE_DIR}/../../googletest"
   "${PROJECT_SOURCE_DIR}/thirdparty/googletest"
   )

find_path(GOOGLE_TEST_SRC_ALL
   NAMES gtest-all.cc
   PATH_SUFFIXES googletest/src
   HINTS
   "${GOOGLETEST_DIR}" ENV GOOGLETEST_DIR
   "${PROJECT_SOURCE_DIR}/googletest"
   "${PROJECT_SOURCE_DIR}/../googletest"
   "${PROJECT_SOURCE_DIR}/../../googletest"
   "${PROJECT_SOURCE_DIR}/thirdparty/googletest"
   )

find_path(GOOGLE_MOCK_INCLUDE_DIR
   NAMES gmock/gmock.h
   PATH_SUFFIXES googlemock/include
   HINTS
   "${GOOGLETEST_DIR}" ENV GOOGLETEST_DIR
   "${PROJECT_SOURCE_DIR}/googletest"
   "${PROJECT_SOURCE_DIR}/../googletest"
   "${PROJECT_SOURCE_DIR}/../../googletest"
   "${PROJECT_SOURCE_DIR}/thirdparty/googletest"
   )

find_path(GOOGLE_MOCK_SRC_ALL
   NAMES gmock-all.cc
   PATH_SUFFIXES googlemock/src
   HINTS
   "${GOOGLETEST_DIR}" ENV GOOGLETEST_DIR
   "${PROJECT_SOURCE_DIR}/googletest"
   "${PROJECT_SOURCE_DIR}/../googletest"
   "${PROJECT_SOURCE_DIR}/../../googletest"
   "${PROJECT_SOURCE_DIR}/thirdparty/googletest"
   )

include(FindPackageHandleStandardArgs)
find_package_handle_standard_args(GoogleTest
   DEFAULT_MSG
   GOOGLE_TEST_INCLUDE_DIR GOOGLE_MOCK_INCLUDE_DIR
   GOOGLE_TEST_SRC_ALL GOOGLE_MOCK_SRC_ALL
   )
find_package(Threads REQUIRED)

if(GoogleTest_FOUND AND NOT TARGET GoogleTest)
   add_library(GoogleTest STATIC
      "${GOOGLE_TEST_SRC_ALL}/gtest-all.cc"
      "${GOOGLE_MOCK_SRC_ALL}/gmock-all.cc"
      )
   target_include_directories(GoogleTest
      PUBLIC
      "${GOOGLE_TEST_INCLUDE_DIR}"
      "${GOOGLE_MOCK_INCLUDE_DIR}"
      PRIVATE
      "${GOOGLE_TEST_SRC_ALL}/.."
      "${GOOGLE_MOCK_SRC_ALL}/.."
      )
   target_compile_definitions(GoogleTest
      PRIVATE
      GTEST_HAS_STD_INITIALIZER_LIST_
      GTEST_LANG_CXX11
      GTEST_HAS_STD_TUPLE_
      GTEST_HAS_STD_TYPE_TRAITS_
      GTEST_HAS_STD_FUNCTION_
      GTEST_HAS_RTTI
      GTEST_HAS_STD_BEGIN_AND_END_
      GTEST_HAS_STD_UNIQUE_PTR_
      GTEST_HAS_EXCEPTIONS
      GTEST_HAS_STREAM_REDIRECTION
      GTEST_HAS_TYPED_TEST
      GTEST_HAS_TYPED_TEST_P
      GTEST_HAS_PARAM_TEST
      GTEST_HAS_DEATH_TEST
      )
   set_target_properties(GoogleTest PROPERTIES AUTOMOC OFF AUTOUIC OFF)
   set_property(TARGET GoogleTest PROPERTY POSITION_INDEPENDENT_CODE ON)
   target_compile_definitions(GoogleTest PUBLIC GOOGLE_TEST_IS_FOUND)
   target_link_libraries(GoogleTest Threads::Threads)
endif()

mark_as_advanced(
   GOOGLE_TEST_INCLUDE_DIR
   GOOGLE_MOCK_INCLUDE_DIR
   GOOGLE_TEST_SRC_ALL
   GOOGLE_MOCK_SRC_ALL)

include(FeatureSummary)
set_package_properties(GoogleTest PROPERTIES
   URL "https://github.com/google/googletest"
   DESCRIPTION "Google Testing and Mocking Framework")
