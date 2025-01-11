# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

if (WIN32)
    set(_ATOMUI_APP_PATH "bin")
    set(_ATOMUI_APP_TARGET "${ATOMUI_PACKAGE_NAME}")

    set(_ATOMUI_LIBRARY_BASE_PATH "lib")
    set(_ATOMUI_LIBRARY_PATH "${_ATOMUI_LIBRARY_BASE_PATH}")
    set(_ATOMUI_LIBEXEC_PATH "bin")
    set(_ATOMUI_DATA_PATH "share/${ATOMUI_PACKAGE_NAME}")
    set(_ATOMUI_BIN_PATH "bin")
    set(_ATOMUI_LIBRARY_ARCHIVE_PATH "${_ATOMUI_BIN_PATH}")
else ()
    include(GNUInstallDirs)
    set(_ATOMUI_APP_PATH "${CMAKE_INSTALL_BINDIR}")
    set(_ATOMUI_APP_TARGET "${ATOMUI_PACKAGE_NAME}")

    set(_ATOMUI_LIBRARY_BASE_PATH "${CMAKE_INSTALL_LIBDIR}")
    set(_ATOMUI_LIBRARY_PATH "${_ATOMUI_LIBRARY_BASE_PATH}")
    set(_ATOMUI_LIBEXEC_PATH "${CMAKE_INSTALL_LIBEXECDIR}")
    set(_ATOMUI_DATA_PATH "${CMAKE_INSTALL_DATAROOTDIR}/${ATOMUI_PACKAGE_NAME}")
    set(_ATOMUI_BIN_PATH "${CMAKE_INSTALL_BINDIR}")
    set(_ATOMUI_LIBRARY_ARCHIVE_PATH "${_ATOMUI_LIBRARY_PATH}")
endif ()

if (APPLE)
    set(_RPATH_BASE "@executable_path")
    set(_LIB_RPATH "@loader_path")
elseif (WIN32)
    set(_RPATH_BASE "")
    set(_LIB_RPATH "")
else ()
    set(_RPATH_BASE "\$ORIGIN")
    set(_LIB_RPATH "\$ORIGIN;")
endif ()

set(ATOMUI_APP_PATH "${_ATOMUI_APP_PATH}")                    # The target path of the ATOMUI application (relative to CMAKE_INSTALL_PREFIX).
set(ATOMUI_APP_TARGET "${_ATOMUI_APP_TARGET}")                # The ATOMUI application name.
set(ATOMUI_LIBRARY_BASE_PATH "${_ATOMUI_LIBRARY_BASE_PATH}")  # The ATOMUI library base path (relative to CMAKE_INSTALL_PREFIX).
set(ATOMUI_LIBRARY_PATH "${_ATOMUI_LIBRARY_PATH}")            # The ATOMUI library path (relative to CMAKE_INSTALL_PREFIX).
set(ATOMUI_LIBRARY_ARCHIVE_PATH "${_ATOMUI_LIBRARY_ARCHIVE_PATH}") # The ATOMUI library archive path (relative to CMAKE_INSTALL_PREFIX).
set(ATOMUI_LIBEXEC_PATH "${_ATOMUI_LIBEXEC_PATH}")            # The ATOMUI libexec path (relative to CMAKE_INSTALL_PREFIX).
set(ATOMUI_DATA_PATH "${_ATOMUI_DATA_PATH}")                  # The ATOMUI data path (relative to CMAKE_INSTALL_PREFIX).
set(ATOMUI_BIN_PATH "${_ATOMUI_BIN_PATH}")                    # The ATOMUI bin path (relative to CMAKE_INSTALL_PREFIX).

file(RELATIVE_PATH RELATIVE_LIBEXEC_PATH "/${ATOMUI_BIN_PATH}" "/${ATOMUI_LIBEXEC_PATH}")
file(RELATIVE_PATH RELATIVE_DATA_PATH "/${ATOMUI_BIN_PATH}" "/${ATOMUI_DATA_PATH}")
