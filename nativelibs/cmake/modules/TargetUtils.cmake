# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

include_guard(GLOBAL)

# If we provide a list of plugins, executables, libraries, then the BUILD_<type>_BY_DEFAULT will be set to OFF
# and for every element we set BUILD_<type>_<element> to ON
# e.g. BUILD_PLUGINS=Core;TextEditor will result in BUILD_PLUGINS_BY_DEFAULT=OFF and BUILD_PLUGIN_CORE=ON and BUILD_PLUGIN_TEXTEDITOR ON

function(atomui_check_default_values_for_list list_type)
    set(PLUGINS_single plugin)
    set(EXECUTABLES_single executable)
    set(LIBRARIES_single library)
    set(TESTS_single test)

    if (NOT DEFINED ATOMUI_BUILD_${list_type})
        return()
    endif ()

    set(ATOMUI_BUILD_${list_type}_BY_DEFAULT OFF CACHE BOOL "" FORCE)

    foreach (element ${ATOMUI_BUILD_${list_type}})
        string(TOUPPER "${${list_type}_single}_${element}" upper_element)
        set(ATOMUI_BUILD_${upper_element} ON CACHE BOOL "Build ${${list_type}_single} ${element}.")
    endforeach ()
endfunction()

atomui_check_default_values_for_list(EXECUTABLES)
atomui_check_default_values_for_list(LIBRARIES)
atomui_check_default_values_for_list(TESTS)

function(atomui_library_enabled varName name)
    if (NOT (name IN_LIST __ATOMUI_LIBRARIES))
        message(FATAL_ERROR "atomui_library_enabled: Unknown library target \"${name}\"")
    endif ()
    if (TARGET ${name})
        set(${varName} ON PARENT_SCOPE)
    else ()
        set(${varName} OFF PARENT_SCOPE)
    endif ()
endfunction()

function(atomui_output_binary_dir varName)
    if (ATOMUI_MERGE_BINARY_DIR)
        set(${varName} ${ATOMUI_BINARY_DIR} PARENT_SCOPE)
    else ()
        set(${varName} ${PROJECT_BINARY_DIR} PARENT_SCOPE)
    endif ()
endfunction()

function(atomui_source_dir varName)
    if (ATOMUI_MERGE_BINARY_DIR)
        set(${varName} ${ATOMUI_SOURCE_DIR} PARENT_SCOPE)
    else ()
        set(${varName} ${PROJECT_SOURCE_DIR} PARENT_SCOPE)
    endif ()
endfunction()

function(atomui_add_library name)
    cmake_parse_arguments(_arg "STATIC;OBJECT;FEATURE_INFO"
        "DESTINATION;COMPONENT;SOURCES_PREFIX;BUILD_DEFAULT"
        "CONDITION;DEPENDS;PUBLIC_DEPENDS;DEFINES;PUBLIC_DEFINES;INCLUDES;PUBLIC_INCLUDES;SOURCES;PROPERTIES" ${ARGN}
    )

    set(default_defines_copy ${DEFAULT_DEFINES})

    if (${_arg_UNPARSED_ARGUMENTS})
        message(FATAL_ERROR "atomui_add_library had unparsed arguments")
    endif ()

    atomui_update_cached_list(__ATOMUI_LIBRARIES "${name}")

    atomui_condition_info(_extra_text _arg_CONDITION)
    if (NOT _arg_CONDITION)
        set(_arg_CONDITION ON)
    endif ()

    add_feature_info("Library ${name}" ON "${_extra_text}")

    set(library_type SHARED)
    if (_arg_STATIC)
        set(library_type STATIC)
    endif ()
    if (_arg_OBJECT)
        set(library_type OBJECT)
    endif ()

    add_library(${name} ${library_type})
    add_library(AtomUI::${name} ALIAS ${name})

    string(TOUPPER "ATOMUI_LIBRARY" EXPORT_SYMBOL)

    if (ATOMUI_WITH_TESTS)
        set(TEST_DEFINES WITH_TESTS SRCDIR="${CMAKE_CURRENT_SOURCE_DIR}")
    endif ()

    if (_arg_STATIC AND UNIX)
        # not added by Qt if reduce_relocations is turned off for it
        set_target_properties(${name} PROPERTIES POSITION_INDEPENDENT_CODE ON)
    endif ()

    atomui_extend_target(${name}
        SOURCES_PREFIX ${_arg_SOURCES_PREFIX}
        SOURCES ${_arg_SOURCES}
        INCLUDES ${_arg_INCLUDES}
        PUBLIC_INCLUDES ${_arg_PUBLIC_INCLUDES}
        DEFINES ${EXPORT_SYMBOL} ${default_defines_copy} ${_arg_DEFINES} ${TEST_DEFINES}
        PUBLIC_DEFINES ${_arg_PUBLIC_DEFINES}
        DEPENDS ${_arg_DEPENDS} ${IMPLICIT_DEPENDS}
        PUBLIC_DEPENDS ${_arg_PUBLIC_DEPENDS}
    )

    # everything is different with SOURCES_PREFIX
    if (NOT _arg_SOURCES_PREFIX)
        target_include_directories(${name}
            PRIVATE
            "$<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>"
            PUBLIC
            "$<BUILD_INTERFACE:${ATOMUI_BINARY_INCLUDE_DIR}>"
            "$<BUILD_INTERFACE:${ATOMUI_INCLUDE_DIR}>"
            "$<INSTALL_INTERFACE:${ATOMUI_HEADER_INSTALL_PATH}>"
        )
    endif ()

    set(_DESTINATION "${ATOMUI_BIN_PATH}")
    if (_arg_DESTINATION)
        set(_DESTINATION "${_arg_DESTINATION}")
    endif ()

    atomui_output_binary_dir(_output_binary_dir)
    string(REGEX MATCH "^[0-9]*" ATOMUI_VERSION_MAJOR ${ATOMUI_VERSION})

    set_target_properties(${name} PROPERTIES
        LINK_DEPENDS_NO_SHARED ON
        SOURCES_DIR "${CMAKE_CURRENT_SOURCE_DIR}"
        MACHO_CURRENT_VERSION ${ATOMUI_VERSION}
        MACHO_COMPATIBILITY_VERSION ${ATOMUI_VERSION_COMPAT}
        CXX_EXTENSIONS OFF
        CXX_VISIBILITY_PRESET hidden
        VISIBILITY_INLINES_HIDDEN ON
        BUILD_RPATH "${_LIB_RPATH};${CMAKE_BUILD_RPATH}"
        INSTALL_RPATH "${_LIB_RPATH};${CMAKE_INSTALL_RPATH}"
        RUNTIME_OUTPUT_DIRECTORY "${_output_binary_dir}/${_DESTINATION}"
        LIBRARY_OUTPUT_DIRECTORY "${_output_binary_dir}/${ATOMUI_LIBRARY_PATH}"
        ARCHIVE_OUTPUT_DIRECTORY "${_output_binary_dir}/${ATOMUI_LIBRARY_ARCHIVE_PATH}"
        ${_arg_PROPERTIES}
    )

    unset(NAMELINK_OPTION)
    if (library_type STREQUAL "SHARED")
        set(NAMELINK_OPTION NAMELINK_SKIP)
    endif ()

    unset(COMPONENT_OPTION)
    if (_arg_COMPONENT)
        set(COMPONENT_OPTION "COMPONENT" "${_arg_COMPONENT}")
    endif ()

    install(TARGETS ${name}
        EXPORT ATOMUI
        RUNTIME
        DESTINATION "${_DESTINATION}"
        ${COMPONENT_OPTION}
        OPTIONAL
        LIBRARY
        DESTINATION "${ATOMUI_LIBRARY_PATH}"
        ${NAMELINK_OPTION}
        ${COMPONENT_OPTION}
        OPTIONAL
        OBJECTS
        DESTINATION "${ATOMUI_LIBRARY_PATH}"
        COMPONENT Devel EXCLUDE_FROM_ALL
        ARCHIVE
        DESTINATION "${ATOMUI_LIBRARY_ARCHIVE_PATH}"
        COMPONENT Devel EXCLUDE_FROM_ALL
        OPTIONAL
    )

    if (ATOMUI_WITH_SANITIZE)
        atomui_enable_sanitize(${SANITIZE_FLAGS})
    endif ()

    if (NAMELINK_OPTION)
        install(TARGETS ${name}
            LIBRARY
            DESTINATION "${ATOMUI_LIBRARY_PATH}"
            NAMELINK_ONLY
            COMPONENT Devel EXCLUDE_FROM_ALL
            OPTIONAL
        )
    endif ()
endfunction()

function(atomui_extend_library target_name)
    atomui_library_enabled(_library_enabled ${target_name})
    if (NOT _library_enabled)
        return()
    endif ()
    atomui_extend_target(${target_name} ${ARGN})
endfunction()

function(atomui_extend_test target_name)
    if (NOT (target_name IN_LIST __ATOMUI_TESTS))
        message(FATAL_ERROR "atomui_extend_test: Unknown test target \"${target_name}\"")
    endif ()
    if (TARGET ${target_name})
        atomui_extend_target(${target_name} ${ARGN})
    endif ()
endfunction()

function(atomui_add_executable name)
    cmake_parse_arguments(_arg ""
        "DESTINATION;COMPONENT;BUILD_DEFAULT"
        "CONDITION;DEPENDS;DEFINES;INCLUDES;SOURCES;PROPERTIES" ${ARGN})

    if (${_arg_UNPARSED_ARGUMENTS})
        message(FATAL_ERROR "atomui_add_executable had unparsed arguments!")
    endif ()
    set(default_defines_copy ${DEFAULT_DEFINES})
    atomui_update_cached_list(__ATOMUI_EXECUTABLES "${name}")

    if (NOT _arg_CONDITION)
        set(_arg_CONDITION ON)
    endif ()

    string(TOUPPER "ATOMUI_BUILD_EXECUTABLE_${name}" _build_executable_var)
    if (DEFINED _arg_BUILD_DEFAULT)
        set(_build_executable_default ${_arg_BUILD_DEFAULT})
    else ()
        set(_build_executable_default ${ATOMUI_BUILD_EXECUTABLES_BY_DEFAULT})
    endif ()
    if (DEFINED ENV{${_build_executable_var}})
        set(_build_executable_default "$ENV{${_build_executable_var}}")
    endif ()
    set(${_build_executable_var} "${_build_executable_default}" CACHE BOOL "Build executable ${name}.")

    if ((${_arg_CONDITION}) AND ${_build_executable_var})
        set(_executable_enabled ON)
    else ()
        set(_executable_enabled OFF)
    endif ()
    if (NOT _executable_enabled)
        return()
    endif ()

    set(_DESTINATION "${ATOMUI_LIBEXEC_PATH}")
    if (_arg_DESTINATION)
        set(_DESTINATION "${_arg_DESTINATION}")
    endif ()

    set(_EXECUTABLE_PATH "${_DESTINATION}")
    if (APPLE)
        # path of executable might be inside app bundle instead of DESTINATION directly
        cmake_parse_arguments(_prop "" "MACOSX_BUNDLE;OUTPUT_NAME" "" "${_arg_PROPERTIES}")
        if (_prop_MACOSX_BUNDLE)
            set(_BUNDLE_NAME "${name}")
            if (_prop_OUTPUT_NAME)
                set(_BUNDLE_NAME "${_prop_OUTPUT_NAME}")
            endif ()
            set(_BUNDLE_CONTENTS_PATH "${_DESTINATION}/${_BUNDLE_NAME}.app/Contents")
            set(_EXECUTABLE_PATH "${_BUNDLE_CONTENTS_PATH}/MacOS")
            set(_EXECUTABLE_FILE_PATH "${_EXECUTABLE_PATH}/${_BUNDLE_NAME}")
            set(_BUNDLE_INFO_PLIST "${_BUNDLE_CONTENTS_PATH}/Info.plist")
        endif ()
    endif ()

    if (ATOMUI_WITH_TESTS)
        set(TEST_DEFINES WITH_TESTS SRCDIR="${CMAKE_CURRENT_SOURCE_DIR}")
    endif ()

    add_executable("${name}" ${_arg_SOURCES})

    atomui_extend_target("${name}"
        INCLUDES "${CMAKE_BINARY_DIR}/src" ${_arg_INCLUDES}
        DEFINES ${default_defines_copy} ${TEST_DEFINES} ${_arg_DEFINES}
        DEPENDS ${_arg_DEPENDS} ${IMPLICIT_DEPENDS}
    )

    file(RELATIVE_PATH relative_lib_path "/${_EXECUTABLE_PATH}" "/${ATOMUI_LIBRARY_PATH}")

    set(build_rpath "${_RPATH_BASE}/${relative_lib_path}")
    set(install_rpath "${_RPATH_BASE}/${relative_lib_path}")
    if (NOT WIN32 AND NOT APPLE)
        set(install_rpath "${install_rpath};")
    endif ()
    set(build_rpath "${build_rpath};${CMAKE_BUILD_RPATH}")
    set(install_rpath "${install_rpath};${CMAKE_INSTALL_RPATH}")

    atomui_output_binary_dir(_output_binary_dir)
    set_target_properties("${name}" PROPERTIES
        LINK_DEPENDS_NO_SHARED ON
        BUILD_RPATH "${build_rpath}"
        INSTALL_RPATH "${install_rpath}"
        RUNTIME_OUTPUT_DIRECTORY "${_output_binary_dir}/${_DESTINATION}"
        CXX_EXTENSIONS OFF
        CXX_VISIBILITY_PRESET hidden
        VISIBILITY_INLINES_HIDDEN ON
        ${_arg_PROPERTIES}
    )
endfunction()

function(atomui_extend_executable name)
    if (NOT (name IN_LIST __ATOMUI_EXECUTABLES))
        message(FATAL_ERROR "atomui_extend_executable: Unknown executable target \"${name}\"")
    endif ()
    if (TARGET ${name})
        atomui_extend_target(${name} ${ARGN})
    endif ()
endfunction()

function(atomui_copy_to_builddir custom_target_name)
    cmake_parse_arguments(_arg "CREATE_SUBDIRS" "DESTINATION" "FILES;DIRECTORIES" ${ARGN})
    set(timestampFiles)

    atomui_output_binary_dir(_output_binary_dir)
    set(allFiles ${_arg_FILES})

    foreach (srcFile ${_arg_FILES})
        string(MAKE_C_IDENTIFIER "${srcFile}" destinationTimestampFilePart)
        set(destinationTimestampFileName "${CMAKE_CURRENT_BINARY_DIR}/.${destinationTimestampFilePart}_timestamp")
        list(APPEND timestampFiles "${destinationTimestampFileName}")

        if (IS_ABSOLUTE "${srcFile}")
            set(srcPath "")
        else ()
            get_filename_component(srcPath "${srcFile}" DIRECTORY)
        endif ()

        add_custom_command(OUTPUT "${destinationTimestampFileName}"
            COMMAND "${CMAKE_COMMAND}" -E make_directory "${_output_binary_dir}/${_arg_DESTINATION}/${srcPath}"
            COMMAND "${CMAKE_COMMAND}" -E copy "${srcFile}" "${_output_binary_dir}/${_arg_DESTINATION}/${srcPath}"
            COMMAND "${CMAKE_COMMAND}" -E touch "${destinationTimestampFileName}"
            WORKING_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}"
            COMMENT "Copy ${srcFile} into build directory"
            DEPENDS "${srcFile}"
            VERBATIM
        )
    endforeach ()

    foreach (srcDirectory ${_arg_DIRECTORIES})
        string(MAKE_C_IDENTIFIER "${srcDirectory}" destinationTimestampFilePart)
        set(destinationTimestampFileName "${CMAKE_CURRENT_BINARY_DIR}/.${destinationTimestampFilePart}_timestamp")
        list(APPEND timestampFiles "${destinationTimestampFileName}")
        set(destinationDirectory "${_output_binary_dir}/${_arg_DESTINATION}")

        if (_arg_CREATE_SUBDIRS)
            set(destinationDirectory "${destinationDirectory}/${srcDirectory}")
        endif ()

        file(GLOB_RECURSE filesToCopy "${srcDirectory}/*")
        list(APPEND allFiles ${filesToCopy})
        add_custom_command(OUTPUT "${destinationTimestampFileName}"
            COMMAND "${CMAKE_COMMAND}" -E copy_directory "${srcDirectory}" "${destinationDirectory}"
            COMMAND "${CMAKE_COMMAND}" -E touch "${destinationTimestampFileName}"
            WORKING_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}"
            COMMENT "Copy ${srcDirectory}/ into build directory"
            DEPENDS ${filesToCopy}
            VERBATIM
        )
    endforeach ()

    add_custom_target("${custom_target_name}" ALL DEPENDS ${timestampFiles}
        SOURCES ${allFiles})
endfunction()

function(atomui_add_public_header header)
    if (NOT IS_ABSOLUTE ${header})
        set(header "${CMAKE_CURRENT_SOURCE_DIR}/${header}")
    endif ()

    atomui_source_dir(atomui_source_dir)
    get_filename_component(source_dir ${header} DIRECTORY)
    file(RELATIVE_PATH include_dir_relative_path "${atomui_source_dir}" "${source_dir}")

    install(
        FILES ${header}
        DESTINATION "${IDE_HEADER_INSTALL_PATH}/${include_dir_relative_path}"
        COMPONENT Devel EXCLUDE_FROM_ALL
    )
endfunction()