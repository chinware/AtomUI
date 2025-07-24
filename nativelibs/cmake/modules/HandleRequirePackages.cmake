# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

if (APPLE)
    find_library(FWCoreFoundation CoreFoundation)
    find_library(FWCoreServices CoreServices)
    find_library(FWFoundation Foundation)
    find_library(FWAppKit AppKit)
    find_library(FWIOKit IOKit)
    find_library(FWSecurity Security)
    find_library(FWSystemConfiguration SystemConfiguration)
    find_library(FWWebKit WebKit)
endif()

if(LINUX)
    # 查找必需的 XCB 组件
    find_package(PkgConfig REQUIRED)
    pkg_check_modules(XCB REQUIRED IMPORTED_TARGET
        xcb
        xcb-shape
    )
    find_package(X11 REQUIRED)
endif()

if(WIN32)
    # 设置共享库不添加前缀
    set(CMAKE_SHARED_LIBRARY_PREFIX "")
    set(CMAKE_SHARED_LIBRARY_SUFFIX ".dll")

    # 设置静态库不添加前缀
    set(CMAKE_STATIC_LIBRARY_PREFIX "")
    set(CMAKE_STATIC_LIBRARY_SUFFIX ".lib")

    # 可选：确保导入库也保持一致命名
    set(CMAKE_IMPORT_LIBRARY_PREFIX "")
    set(CMAKE_IMPORT_LIBRARY_SUFFIX ".lib")
endif()