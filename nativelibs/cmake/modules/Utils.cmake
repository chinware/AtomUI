# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

include_guard(GLOBAL)
include(CheckCSourceCompiles)
include(CheckSymbolExists)

# handle SCCACHE hack
# SCCACHE does not work with the /Zi option, which makes each compilation write debug info
# into the same .pdb file - even with /FS, which usually makes this work in the first place.
# Replace /Zi with /Z7, which leaves the debug info in the object files until link time.
# This increases memory usage, disk space usage and linking time, so should only be
# enabled if necessary.
# Must be called after project(...).
function(atomui_handle_sccache_support)
    if (MSVC AND WITH_SCCACHE_SUPPORT)
        foreach (config DEBUG RELWITHDEBINFO)
            foreach (lang C CXX)
                set(flags_var "CMAKE_${lang}_FLAGS_${config}")
                string(REPLACE "/Zi" "/Z7" ${flags_var} "${${flags_var}}")
                set(${flags_var} "${${flags_var}}" PARENT_SCOPE)
            endforeach ()
        endforeach ()
    endif ()
endfunction()

function(atomui_enable_release_for_debug_configuration)
    if (MSVC)
        string(REPLACE "/Od" "/O2" CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}")
        string(REPLACE "/Ob0" "/Ob1" CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}")
        string(REPLACE "/RTC1" "" CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}")
    else ()
        set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -O2")
    endif ()
    set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}" PARENT_SCOPE)
endfunction()

function(atomui_enable_sanitize _sanitize_flags)
    if (CMAKE_CXX_COMPILER_ID MATCHES "Clang" OR CMAKE_CXX_COMPILER_ID STREQUAL "GNU")
        set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -fsanitize=${_sanitize_flags}")
    endif ()
    set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}" PARENT_SCOPE)
endfunction()

function(atomui_update_cached_list name value)
    set(_tmp_list "${${name}}")
    list(APPEND _tmp_list "${value}")
    set("${name}" "${_tmp_list}" CACHE INTERNAL "*** Internal ***")
endfunction()

function(atomui_set_public_headers target sources)
    foreach (source IN LISTS sources)
        if (source MATCHES "\.h$|\.hpp$")
            atomui_add_public_header(${source})
        endif ()
    endforeach ()
endfunction()

function(atomui_set_public_includes target includes)
    foreach (inc_dir IN LISTS includes)
        if (NOT IS_ABSOLUTE ${inc_dir})
            set(inc_dir "${CMAKE_CURRENT_SOURCE_DIR}/${inc_dir}")
        endif ()
        file(RELATIVE_PATH include_dir_relative_path ${PROJECT_SOURCE_DIR} ${inc_dir})
        target_include_directories(${target} PUBLIC
                $<BUILD_INTERFACE:${inc_dir}>
                $<INSTALL_INTERFACE:${_ATOMUI_HEADER_INSTALL_PATH}/${include_dir_relative_path}>
        )
    endforeach ()
endfunction()

function(atomui_finalize_test_setup test_name)
    cmake_parse_arguments(_arg "" "TIMEOUT" "" ${ARGN})
    if (DEFINED _arg_TIMEOUT)
        set(timeout ${_arg_TIMEOUT})
    else ()
        set(timeout 5)
    endif ()
    # Never translate tests:
    set_tests_properties(${name}
            PROPERTIES
            TIMEOUT ${timeout}
    )

    if (WIN32)
        list(APPEND env_path $ENV{PATH})
        list(APPEND env_path ${CMAKE_BINARY_DIR}/${_ATOMUI_PLUGIN_PATH})
        list(APPEND env_path ${CMAKE_BINARY_DIR}/${_ATOMUI_BIN_PATH})

        string(REPLACE "/" "\\" env_path "${env_path}")
        string(REPLACE ";" "\\;" env_path "${env_path}")

        set_tests_properties(${test_name} PROPERTIES ENVIRONMENT "PATH=${env_path}")
    endif ()
endfunction()

function(atomui_check_disabled_targets target_name dependent_targets)
    foreach (dependency IN LISTS ${dependent_targets})
        foreach (type PLUGIN LIBRARY)
            string(TOUPPER "BUILD_${type}_${dependency}" build_target)
            if (DEFINED ${build_target} AND NOT ${build_target})
                message(SEND_ERROR "Target ${name} depends on ${dependency} which was disabled via ${build_target} set to ${${build_target}}")
            endif ()
        endforeach ()
    endforeach ()
endfunction()

function(atomui_add_depends target_name)
    cmake_parse_arguments(_arg "" "" "PRIVATE;PUBLIC" ${ARGN})
    if (${_arg_UNPARSED_ARGUMENTS})
        message(FATAL_ERROR "atomui_add_depends had unparsed arguments")
    endif ()

    atomui_check_disabled_targets(${target_name} _arg_PRIVATE)
    atomui_check_disabled_targets(${target_name} _arg_PUBLIC)

    set(depends "${_arg_PRIVATE}")
    set(public_depends "${_arg_PUBLIC}")

    get_target_property(target_type ${target_name} TYPE)
    if (NOT target_type STREQUAL "OBJECT_LIBRARY")
        target_link_libraries(${target_name} PRIVATE ${depends} PUBLIC ${public_depends})
    else ()
        list(APPEND object_lib_depends ${depends})
        list(APPEND object_public_depends ${public_depends})
    endif ()

    foreach (obj_lib IN LISTS object_lib_depends)
        target_compile_options(${target_name} PRIVATE $<TARGET_PROPERTY:${obj_lib},INTERFACE_COMPILE_OPTIONS>)
        target_compile_definitions(${target_name} PRIVATE $<TARGET_PROPERTY:${obj_lib},INTERFACE_COMPILE_DEFINITIONS>)
        target_include_directories(${target_name} PRIVATE $<TARGET_PROPERTY:${obj_lib},INTERFACE_INCLUDE_DIRECTORIES>)
    endforeach ()
    foreach (obj_lib IN LISTS object_public_depends)
        target_compile_options(${target_name} PUBLIC $<TARGET_PROPERTY:${obj_lib},INTERFACE_COMPILE_OPTIONS>)
        target_compile_definitions(${target_name} PUBLIC $<TARGET_PROPERTY:${obj_lib},INTERFACE_COMPILE_DEFINITIONS>)
        target_include_directories(${target_name} PUBLIC $<TARGET_PROPERTY:${obj_lib},INTERFACE_INCLUDE_DIRECTORIES>)
    endforeach ()
endfunction()

function(atomui_find_dependent_plugins varName)
    set(_RESULT ${ARGN})

    foreach (i ${ARGN})
        get_property(_dep TARGET "${i}Plugin" PROPERTY _arg_DEPENDS)
        if (_dep)
            atomui_find_dependent_plugins(_REC ${_dep})
            list(APPEND _RESULT ${_REC})
        endif ()
    endforeach ()

    if (_RESULT)
        list(REMOVE_DUPLICATES _RESULT)
        list(SORT _RESULT)
    endif ()

    set("${varName}" ${_RESULT} PARENT_SCOPE)
endfunction()

function(atomui_condition_info varName condition)
    if (NOT ${condition})
        set(${varName} "" PARENT_SCOPE)
    else ()
        string(REPLACE ";" " " _contents "${${condition}}")
        set(${varName} "with CONDITION ${_contents}" PARENT_SCOPE)
    endif ()
endfunction()

function(atomui_extend_target target_name)
    cmake_parse_arguments(_arg
            ""
            "SOURCES_PREFIX;SOURCES_PREFIX_FROM_TARGET;FEATURE_INFO"
            "CONDITION;DEPENDS;PUBLIC_DEPENDS;DEFINES;PUBLIC_DEFINES;INCLUDES;PUBLIC_INCLUDES;SOURCES;PROPERTIES"
            ${ARGN}
    )

    if (${_arg_UNPARSED_ARGUMENTS})
        message(FATAL_ERROR "atomui_extend_target had unparsed arguments")
    endif ()

    atomui_condition_info(_extra_text _arg_CONDITION)
    if (NOT _arg_CONDITION)
        set(_arg_CONDITION ON)
    endif ()
    if (${_arg_CONDITION})
        set(_feature_enabled ON)
    else ()
        set(_feature_enabled OFF)
    endif ()
    if (_arg_FEATURE_INFO)
        add_feature_info(${_arg_FEATURE_INFO} _feature_enabled "${_extra_text}")
    endif ()
    if (NOT _feature_enabled)
        return()
    endif ()

    if (_arg_SOURCES_PREFIX_FROM_TARGET)
        if (NOT TARGET ${_arg_SOURCES_PREFIX_FROM_TARGET})
            return()
        else ()
            get_target_property(_arg_SOURCES_PREFIX ${_arg_SOURCES_PREFIX_FROM_TARGET} SOURCES_DIR)
        endif ()
    endif ()

    atomui_add_depends(${target_name}
            PRIVATE ${_arg_DEPENDS}
            PUBLIC ${_arg_PUBLIC_DEPENDS}
    )
    target_compile_definitions(${target_name}
            PRIVATE ${_arg_DEFINES}
            PUBLIC ${_arg_PUBLIC_DEFINES}
    )
    target_include_directories(${target_name} PRIVATE ${_arg_INCLUDES})

    atomui_set_public_includes(${target_name} "${_arg_PUBLIC_INCLUDES}")

    if (_arg_SOURCES_PREFIX)
        foreach (source IN LISTS _arg_SOURCES)
            list(APPEND prefixed_sources "${_arg_SOURCES_PREFIX}/${source}")
        endforeach ()

        if (NOT IS_ABSOLUTE ${_arg_SOURCES_PREFIX})
            set(_arg_SOURCES_PREFIX "${CMAKE_CURRENT_SOURCE_DIR}/${_arg_SOURCES_PREFIX}")
        endif ()
        target_include_directories(${target_name} PRIVATE $<BUILD_INTERFACE:${_arg_SOURCES_PREFIX}>)

        set(_arg_SOURCES ${prefixed_sources})
    endif ()
    target_sources(${target_name} PRIVATE ${_arg_SOURCES})

    atomui_set_public_headers(${target_name} "${_arg_SOURCES}")

    if (_arg_PROPERTIES)
        set_target_properties(${target_name} PROPERTIES ${_arg_PROPERTIES})
    endif ()
endfunction()

# list functions
function(atomui_list_subtract lhs rhs result_var_name)
    set(result)
    foreach (item IN LISTS lhs)
        if (NOT "${item}" IN_LIST rhs)
            list(APPEND result "${item}")
        endif ()
    endforeach ()
    set("${result_var_name}" "${result}" PARENT_SCOPE)
endfunction()

function(atomui_list_intersect lhs rhs result_var_name)
    set(result)
    foreach (item IN LISTS lhs)
        if ("${item}" IN_LIST rhs)
            list(APPEND result "${item}")
        endif ()
    endforeach ()
    set("${result_var_name}" "${result}" PARENT_SCOPE)
endfunction()

function(atomui_list_union lhs rhs result_var_name)
    set(result)
    foreach (item IN LISTS lhs rhs)
        if (NOT "${item}" IN_LIST result)
            list(APPEND result "${item}")
        endif ()
    endforeach ()
    set("${result_var_name}" "${result}" PARENT_SCOPE)
endfunction()

function(_list_add_string_suffix input_list suffix result_var_name)
    set(result)
    foreach (element ${input_list})
        list(APPEND result "${element}${suffix}")
    endforeach ()
    set("${result_var_name}" "${result}" PARENT_SCOPE)
endfunction()

function(_list_escape_for_shell input_list result_var_name)
    set(result "")
    foreach (element ${input_list})
        string(REPLACE " " "\\ " element "${element}")
        set(result "${result}${element} ")
    endforeach ()
    set("${result_var_name}" "${result}" PARENT_SCOPE)
endfunction()

function(atomui_list_replace input_list old new)
    set(replaced_list)
    foreach (item ${${input_list}})
        if (${item} STREQUAL ${old})
            list(APPEND replaced_list ${new})
        else ()
            list(APPEND replaced_list ${item})
        endif ()
    endforeach ()
    set("${input_list}" "${replaced_list}" PARENT_SCOPE)
endfunction()

macro(atomui_merge_list target list)
    foreach (_listItem ${${list}})
        list(APPEND ${target} ${_listItem})
    endforeach ()
    if (target)
        list(REMOVE_DUPLICATES ${target})
    endif ()
endmacro()

function(atomui_precondition var)
    cmake_parse_arguments(
            PRECONDITION # prefix
            "NEGATE" # options
            "MESSAGE" # single-value args
            "" # multi-value args
            ${ARGN})

    if (PRECONDITION_NEGATE)
        if (${var})
            if (PRECONDITION_MESSAGE)
                message(FATAL_ERROR "Error! ${PRECONDITION_MESSAGE}")
            else ()
                message(FATAL_ERROR "Error! Variable ${var} is true or not empty. The value of ${var} is ${${var}}.")
            endif ()
        endif ()
    else ()
        if (NOT ${var})
            if (PRECONDITION_MESSAGE)
                message(FATAL_ERROR "Error! ${PRECONDITION_MESSAGE}")
            else ()
                message(FATAL_ERROR "Error! Variable ${var} is false, empty or not set.")
            endif ()
        endif ()
    endif ()
endfunction()

# Assert is 'NOT ${LHS} ${OP} ${RHS}' is true.
function(atomui_precondition_binary_op OP LHS RHS)
    cmake_parse_arguments(
            PRECONDITIONBINOP # prefix
            "NEGATE" # options
            "MESSAGE" # single-value args
            "" # multi-value args
            ${ARGN})

    if (PRECONDITIONBINOP_NEGATE)
        if (${LHS} ${OP} ${RHS})
            if (PRECONDITIONBINOP_MESSAGE)
                message(FATAL_ERROR "Error! ${PRECONDITIONBINOP_MESSAGE}")
            else ()
                message(FATAL_ERROR "Error! ${LHS} ${OP} ${RHS} is true!")
            endif ()
        endif ()
    else ()
        if (NOT ${LHS} ${OP} ${RHS})
            if (PRECONDITIONBINOP_MESSAGE)
                message(FATAL_ERROR "Error! ${PRECONDITIONBINOP_MESSAGE}")
            else ()
                message(FATAL_ERROR "Error! ${LHS} ${OP} ${RHS} is false!")
            endif ()
        endif ()
    endif ()
endfunction()

# Translate a yes/no variable to the presence of a given string in a
# variable.
#
# Usage:
#   atomui_translate_flag(is_set flag_name var_name)
#
# If is_set is true, sets ${var_name} to ${flag_name}. Otherwise,
# unsets ${var_name}.
function(atomui_translate_flag is_set flag_name var_name)
    if (${is_set})
        set("${var_name}" "${flag_name}" PARENT_SCOPE)
    else ()
        set("${var_name}" "" PARENT_SCOPE)
    endif ()
endfunction()

macro(atomui_translate_flags prefix options)
    foreach (var ${options})
        atomui_translate_flag("${${prefix}_${var}}" "${var}" "${prefix}_${var}_keyword")
    endforeach ()
endmacro()

# Set ${outvar} to ${${invar}}, asserting if ${invar} is not set.
function(atomui_precondition_translate_flag invar outvar)
    atomui_precondition(${invar})
    set(${outvar} "${${invar}}" PARENT_SCOPE)
endfunction()

# Set variable to value if value is not null or false. Otherwise set variable to
# default_value.
function(atomui_set_with_default variable value)
    cmake_parse_argument(
            SWD
            ""
            "DEFAULT"
            "" ${ARGN})
    atomui_precondition(SWD_DEFAULT
            MESSAGE "Must specify a default argument")
    if (value)
        set(${variable} ${value} PARENT_SCOPE)
    else ()
        set(${variable} ${SWD_DEFAULT} PARENT_SCOPE)
    endif ()
endfunction()

function(atomui_dump_vars)
    set(ATOMUI_STDLIB_GLOBAL_CMAKE_CACHE)
    get_cmake_property(variableNames VARIABLES)
    foreach (variableName ${variableNames})
        if (variableName MATCHES "^ATOMUI")
            set(ATOMUI_STDLIB_GLOBAL_CMAKE_CACHE "${ATOMUI_STDLIB_GLOBAL_CMAKE_CACHE}set(${variableName} \"${${variableName}}\")\n")
            message("set(${variableName} \"${${variableName}}\")")
        endif ()
    endforeach ()
endfunction()

#
# Look for either a program in execute_process()'s path or for a hardcoded path.
# Find a program's version and set it in the parent scope.
# Replace newlines with spaces so it prints on one line.
#
function(atomui_find_version cmd flag find_in_path)
    if (find_in_path)
        message(STATUS "Finding installed version for: ${cmd}")
    else ()
        message(STATUS "Finding version for: ${cmd}")
    endif ()
    execute_process(
            COMMAND ${cmd} ${flag}
            OUTPUT_VARIABLE out
            OUTPUT_STRIP_TRAILING_WHITESPACE)
    if (NOT out)
        if (find_in_path)
            message(STATUS "tried to find version for ${cmd}, but ${cmd} not found in path, continuing")
        else ()
            message(FATAL_ERROR "tried to find version for ${cmd}, but ${cmd} not found")
        endif ()
    else ()
        string(REPLACE "\n" " " out2 ${out})
        message(STATUS "Found version: ${out2}")
    endif ()
    message(STATUS "")
endfunction()

function(atomui_append_flag value)
    foreach (variable ${ARGN})
        set(${variable} "${${variable}} ${value}" PARENT_SCOPE)
    endforeach (variable)
endfunction()

function(atomui_append_flag_if condition value)
    if (${condition})
        foreach (variable ${ARGN})
            set(${variable} "${${variable}} ${value}" PARENT_SCOPE)
        endforeach (variable)
    endif ()
endfunction()

macro(atomui_add_flag_if_supported flag name)
    check_c_compiler_flag("-Werror ${flag}" "C_SUPPORTS_${name}")
    atomui_append_flag_if("C_SUPPORTS_${name}" "${flag}" CMAKE_C_FLAGS)
    check_cxx_compiler_flag("-Werror ${flag}" "CXX_SUPPORTS_${name}")
    atomui_append_flag_if("CXX_SUPPORTS_${name}" "${flag}" CMAKE_CXX_FLAGS)
endmacro()

function(atomui_add_flag_or_print_warning flag name)
    check_c_compiler_flag("-Werror ${flag}" "C_SUPPORTS_${name}")
    check_cxx_compiler_flag("-Werror ${flag}" "CXX_SUPPORTS_${name}")
    if (C_SUPPORTS_${name} AND CXX_SUPPORTS_${name})
        message(STATUS "Building with ${flag}")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} ${flag}" PARENT_SCOPE)
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} ${flag}" PARENT_SCOPE)
        set(CMAKE_ASM_FLAGS "${CMAKE_ASM_FLAGS} ${flag}" PARENT_SCOPE)
    else ()
        message(WARNING "${flag} is not supported.")
    endif ()
endfunction()

function(atomui_add_files target)
    list(REMOVE_AT ARGV 0)
    foreach (file ${ARGV})
        list(APPEND ${target} ${CMAKE_CURRENT_SOURCE_DIR}/${file})
    endforeach ()
    set(${target} ${${target}} PARENT_SCOPE)
endfunction()

function(atomui_generate_header_guard filename output)
    string(TOUPPER ${filename} filename)
    string(REPLACE "." "_" filename ${filename})
    string(REPLACE "/" "_" filename ${filename})
    set(${output} HAVE_${filename} PARENT_SCOPE)
endfunction()

function(atomui_define_have name)
    atomui_generate_header_guard(${name} haveName)
    set(${haveName} ON PARENT_SCOPE)
endfunction()

macro(atomui_find_parent_dir path output)
    get_filename_component(${output} ${path} ABSOLUTE)
    get_filename_component(${output} ${${output}} DIRECTORY)
endmacro()

macro(atomui_add_rt_require_lib name)
    list(APPEND ATOMUI_RT_REQUIRE_LIBS ${name})
    if (NOT ${CMAKE_CURRENT_SOURCE_DIR} STREQUAL ${CMAKE_SOURCE_DIR})
        set(ATOMUI_RT_REQUIRE_LIBS ${ATOMUI_RT_REQUIRE_LIBS} PARENT_SCOPE)
    endif ()
endmacro()

macro(atomui_check_library_exists library function location variable)
    check_library_exists("${library}" "${function}" "${location}" "${variable}")
    if (${${variable}})
        set(ATOMUI_${variable} ON)
    endif ()
endmacro()

macro(atomui_check_symbol_exists symbol files variable)
    check_symbol_exists("${symbol}" "${files}" "${variable}")
    if (${${variable}})
        set(ATOMUI_${variable} ON)
    endif ()
endmacro()

# Beware that there is no implementation of remove_llvm_definitions.

macro(atomui_add_definitions)
    # We don't want no semicolons on ATOMUI_DEFINITIONS:
    foreach (arg ${ARGN})
        if (DEFINED ATOMUI_DEFINITIONS)
            set(ATOMUI_DEFINITIONS "${ATOMUI_DEFINITIONS} ${arg}")
        else ()
            set(ATOMUI_DEFINITIONS ${arg})
        endif ()
    endforeach (arg)
    add_definitions(${ARGN})
endmacro()

macro(atomui_check_headers)
    foreach (_filename ${ARGV})
        atomui_generate_header_guard(${_filename} _guardName)
        check_include_file(${_filename} ${_guardName})
        if (${${_guardName}})
            set(ATOMUI_${_guardName} ON)
        endif ()
    endforeach ()
endmacro()

macro(atomui_check_funcs)
    foreach (_func ${ARGV})
        string(TOUPPER ${_func} upcase)
        check_function_exists(${_func} HAVE_${upcase})
        if (${HAVE_${upcase}})
            set(ATOMUI_HAVE_${upcase} ON)
        endif ()
    endforeach ()
endmacro()

# Set each output directory according to ${CMAKE_CONFIGURATION_TYPES}.
# Note: Don't set variables CMAKE_*_OUTPUT_DIRECTORY any more,
# or a certain builder, for eaxample, msbuild.exe, would be confused.
function(atomui_set_output_directory target)
    cmake_parse_arguments(ARG "" "BINARY_DIR;LIBRARY_DIR" "" ${ARGN})

    # module_dir -- corresponding to LIBRARY_OUTPUT_DIRECTORY.
    # It affects output of add_library(MODULE).
    if (WIN32 OR CYGWIN)
        # DLL platform
        set(module_dir ${ARG_BINARY_DIR})
    else ()
        set(module_dir ${ARG_LIBRARY_DIR})
    endif ()
    if (NOT "${CMAKE_CFG_INTDIR}" STREQUAL ".")
        foreach (build_mode ${CMAKE_CONFIGURATION_TYPES})
            string(TOUPPER "${build_mode}" CONFIG_SUFFIX)
            if (ARG_BINARY_DIR)
                string(REPLACE ${CMAKE_CFG_INTDIR} ${build_mode} bi ${ARG_BINARY_DIR})
                set_target_properties(${target} PROPERTIES "RUNTIME_OUTPUT_DIRECTORY_${CONFIG_SUFFIX}" ${bi})
            endif ()
            if (ARG_LIBRARY_DIR)
                string(REPLACE ${CMAKE_CFG_INTDIR} ${build_mode} li ${ARG_LIBRARY_DIR})
                set_target_properties(${target} PROPERTIES "ARCHIVE_OUTPUT_DIRECTORY_${CONFIG_SUFFIX}" ${li})
            endif ()
            if (module_dir)
                string(REPLACE ${CMAKE_CFG_INTDIR} ${build_mode} mi ${module_dir})
                set_target_properties(${target} PROPERTIES "LIBRARY_OUTPUT_DIRECTORY_${CONFIG_SUFFIX}" ${mi})
            endif ()
        endforeach ()
    else ()
        if (ARG_BINARY_DIR)
            set_target_properties(${target} PROPERTIES RUNTIME_OUTPUT_DIRECTORY ${ARG_BINARY_DIR})
        endif ()
        if (ARG_LIBRARY_DIR)
            set_target_properties(${target} PROPERTIES ARCHIVE_OUTPUT_DIRECTORY ${ARG_LIBRARY_DIR})
        endif ()
        if (module_dir)
            set_target_properties(${target} PROPERTIES LIBRARY_OUTPUT_DIRECTORY ${module_dir})
        endif ()
    endif ()
endfunction()

macro(atomui_check_prog_awk)
    find_program(ATOMUI_PROGRAM_AWK awk NAMES gawk nawk mawk
            PATHS /usr/xpg4/bin/)
    if (NOT ATOMUI_PROGRAM_AWK)
        message(FATAL_ERROR "Could not find awk; Install GNU awk")
    else ()
        if (ATOMUI_PROGRAM_AWK MATCHES ".*mawk")
            message(WARNING "mawk is known to have problems on some systems. You should install GNU awk")
        else ()
            message(STATUS "checking wether ${ATOMUI_PROGRAM_AWK} is broken")
            execute_process(COMMAND ${ATOMUI_PROGRAM_AWK} "function foo() {}" ">/dev/null 2>&1"
                    RESULT_VARIABLE _awkExecRet)
            if (_awkExecRet)
                message(FATAL_ERROR "You should install GNU awk")
                unset(ATOMUI_PROGRAM_AWK)
            else ()
                message(STATUS "${ATOMUI_PROGRAM_AWK} is works")
            endif ()
        endif ()
    endif ()
endmacro()

function(atomui_set_if_target var target)
    if (TARGET "${target}")
        set(_result ON)
    else ()
        set(_result OFF)
    endif ()
    set(${var} "${_result}" PARENT_SCOPE)
endfunction()
