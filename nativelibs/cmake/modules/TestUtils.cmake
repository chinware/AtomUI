# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

# Generic support for adding a unittest.
function(atomui_add_unittest test_suite test_name)
    cmake_parse_arguments(_arg "GTEST;MANUALTEST" "TIMEOUT"
        "DEFINES;DEPENDS;INCLUDES;SOURCES;EXPLICIT_MOC;CONDITION;LINK_LIBS" ${ARGN})
    if ($_arg_UNPARSED_ARGUMENTS)
        message(FATAL_ERROR "atomui_add_unittest had unparsed arguments!")
    endif ()

    atomui_update_cached_list(__ATOMUI_TESTS "${test_name}")
    set(_link_libs ${ATOMUI_LINK_COMPONENTS})
    if (_arg_LINK_LIBS)
        atomui_merge_list(_link_libs _arg_LINK_LIBS)
    endif ()

    if (NOT _arg_CONDITION)
        set(_arg_CONDITION ON)
    endif ()

    if (NOT _arg_GTEST)
        set(_arg_GTEST ON)
    endif ()

    if (NOT ATOMUI_BUILD_TESTS)
        set(EXCLUDE_FROM_ALL ON)
    endif ()

    foreach (dependency ${_arg_DEPENDS})
        if (NOT TARGET ${dependency} AND NOT _arg_GTEST)
            if (ATOMUI_WITH_DEBUG_CMAKE)
                message(STATUS "'${dependency}' is not a target")
            endif ()
            return()
        endif ()
    endforeach ()

    if (SUPPORTS_VARIADIC_MACROS_FLAG)
        list(APPEND ATOMUI_COMPILE_FLAGS "-Wno-variadic-macros")
    endif ()

    # Some parts of gtest rely on this GNU extension, don't warn on it.
    if (SUPPORTS_GNU_ZERO_VARIADIC_MACRO_ARGUMENTS_FLAG)
        list(APPEND ATOMUI_COMPILE_FLAGS "-Wno-gnu-zero-variadic-macro-arguments")
    endif ()

    set(TEST_DEFINES SRCDIR="${CMAKE_CURRENT_SOURCE_DIR}")

    # relax cast requirements for tests
    set(default_defines_copy ${DEFAULT_DEFINES})
    file(RELATIVE_PATH _RPATH "/${ATOMUI_BIN_PATH}" "/${ATOMUI_LIBRARY_PATH}")

    atomui_add_executable(${test_name}
        DESTINATION ${ATOMUI_TEST_BIN_DIR}
        SKIP_INSTALL
        SOURCES ${_arg_SOURCES})

    # The runtime benefits of LTO don't outweight the compile time costs for tests.
    if (ATOMUI_ENABLE_LTO)
        if ((UNIX OR MINGW) AND LINKER_IS_LLD)
            set_property(TARGET ${test_name} APPEND_STRING PROPERTY
                LINK_FLAGS " -Wl,--lto-O0")
        elseif (LINKER_IS_LLD_LINK)
            set_property(TARGET ${test_name} APPEND_STRING PROPERTY
                LINK_FLAGS " /opt:lldlto=0")
        endif ()
    endif ()

    atomui_extend_target(${test_name}
        DEPENDS ${_arg_DEPENDS} ${IMPLICIT_DEPENDS}
        INCLUDES ${ATOMUI_BINARY_INCLUDE_DIR} ${ATOMUI_INCLUDE_DIR} ${_arg_INCLUDES}
        DEFINES ${_arg_DEFINES} ${TEST_DEFINES} ${default_defines_copy}
    )

    set_target_properties(${test_name} PROPERTIES
        LINK_DEPENDS_NO_SHARED ON
        CXX_VISIBILITY_PRESET hidden
        VISIBILITY_INLINES_HIDDEN ON
        BUILD_RPATH "${_RPATH_BASE}/${_RPATH};${CMAKE_BUILD_RPATH}"
        INSTALL_RPATH "${_RPATH_BASE}/${_RPATH};${CMAKE_INSTALL_RPATH}"
    )

    # libpthreads overrides some standard library symbols, so main
    # executable must be linked with it in order to provide consistent
    # API for all shared libaries loaded by this executable.
    if (_arg_GTEST)
        list(APPEND _link_libs
            GoogleTest
            #            GTest::gtest_main
            #            GTest::gtest
            #            GTest::gmock
            #            GTest::gmock_main
            ${ATOMUI_PTHREAD_LIB})
    endif ()

    target_link_libraries(${test_name} PRIVATE ${_link_libs})
    if (ATOMUI_COMPILE_FLAGS)
        target_compile_options(${test_name} PRIVATE ${ATOMUI_COMPILE_FLAGS})
    endif ()

    if (NOT TARGET ${test_suite})
        add_custom_target("${test_suite}")
        set_target_properties(${test_suite} PROPERTIES FOLDER "Tests")
    endif ()

    add_dependencies(${test_suite} ${test_name})
    # 现在直接通过源码编译的方式引入
    # add_dependencies(${test_name} thirdparty_gtest)
    get_target_property(test_suite_folder ${test_suite} FOLDER)
    if (test_suite_folder)
        set_property(TARGET ${test_name} PROPERTY FOLDER "${test_suite_folder}")
    endif ()

    if (NOT _arg_GTEST AND NOT _arg_MANUALTEST)
        add_test(NAME ${test_name} COMMAND ${test_name})
        if (DEFINED _arg_TIMEOUT)
            set(timeout_option TIMEOUT ${_arg_TIMEOUT})
        else ()
            set(timeout_option)
        endif ()
        atomui_finalize_test_setup(${test_name} ${timeout_option})
    endif ()
endfunction()