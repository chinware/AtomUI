# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

cmake_minimum_required(VERSION 3.30)
set(CMAKE_DISABLE_IN_SOURCE_BUILD YES)

project(nativelibs)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_CXX_VISIBILITY_PRESET hidden)
set(CMAKE_C_STANDARD 11)
set(CMAKE_OSX_DEPLOYMENT_TARGET 11) # 目前先支持到 11
set_property(GLOBAL PROPERTY JOB_POOLS "${CMAKE_JOB_POOLS}")

# setup some important path
set(ATOMUI_SOURCE_DIR "${CMAKE_CURRENT_SOURCE_DIR}") # --src-root
set(ATOMUI_BINARY_DIR "${CMAKE_CURRENT_BINARY_DIR}") # --prefix
set(ATOMUI_CMAKE_MODULES_DIR "${ATOMUI_SOURCE_DIR}/cmake/modules")
set(ATOMUI_INCLUDE_DIR "${ATOMUI_SOURCE_DIR}/include")
set(ATOMUI_BINARY_INCLUDE_DIR "${CMAKE_CURRENT_BINARY_DIR}/include") # --includedir
set(ATOMUI_TEST_DIR "${ATOMUI_SOURCE_DIR}/tests")
set(ATOMUI_TEST_BIN_DIR "tests/bin")
set(ATOMUI_SYSTEM_NAME ${CMAKE_HOST_SYSTEM_NAME})
string(TOLOWER ${ATOMUI_SYSTEM_NAME} ATOMUI_SYSTEM_NORMAL_NAME)

# Add path for custom CMake modules.
list(APPEND CMAKE_MODULE_PATH
        "${CMAKE_CURRENT_SOURCE_DIR}/cmake"
        "${ATOMUI_CMAKE_MODULES_DIR}"
)

include(MetaInfo)
include(FeatureSummary)

project(${ATOMUI_PACKAGE_NAME} LANGUAGES C CXX
        VERSION ${ATOMUI_VERSION_MAJOR}.${ATOMUI_VERSION_MINOR}.${ATOMUI_VERSION_PATCH})

if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Debug CACHE STRING "Build type" FORCE)
endif()

if (CMAKE_BUILD_TYPE STREQUAL "Debug")
    set(ATOMUI_DEBUG_BUILD ON)
endif()

if (${CMAKE_HOST_SYSTEM_NAME} STREQUAL Linux)
    set(LINUX ON)
endif()

string(TOLOWER ${CMAKE_BUILD_TYPE} ATOMUI_BUILD_TYPE)
if (CMAKE_BUILD_TYPE AND
        NOT ATOMUI_BUILD_TYPE MATCHES "^(debug|release|relwithdebinfo|minsizerel)$")
    message(FATAL_ERROR "Invalid value for CMAKE_BUILD_TYPE: ${CMAKE_BUILD_TYPE}")
endif()

option(ATOMUI_WITH_DEBUG_CMAKE "Enabled CMake project debugging functionality (e.g. source file disk checking)" OFF)
option(ATOMUI_BUILD_UNITTESTS "Generate build targets for the atomui library unittests." ON)

set(CMAKE_INCLUDE_CURRENT_DIR ON)

include(Utils)
include(HandleHostEnv)
include(HandleRequirePackages)
include(HandleConfig)
include(HandleAppPath)
include(TargetUtils)
include(TestUtils)

include_directories(${ATOMUI_SOURCE_DIR}
        ${ATOMUI_INCLUDE_DIR}
        ${ATOMUI_INCLUDE_DIR}/atomui
        ${ATOMUI_BINARY_INCLUDE_DIR}
        ${ATOMUI_BINARY_INCLUDE_DIR}/atomui
        ${CMAKE_CURRENT_BINARY_DIR})

if (ATOMUI_BUILD_UNITTESTS)
    enable_testing()
    add_subdirectory(tests/unittests)
endif()

add_subdirectory(include/atomui)
add_subdirectory(src)

include(SummaryOutput)
