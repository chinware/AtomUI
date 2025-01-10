// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/10.

#pragma once

#include "atomui/Config.h"

#ifdef __cplusplus
constexpr
#endif
inline void atomui_noop()
#ifdef __cplusplus
    noexcept
#endif
{}

#if defined(ATOMUI_LIBRARY)
#  define ATOMUI_EXPORT ATOMUI_DECL_EXPORT
#elif defined(ATOMUI_STATIC_LIB) // Abuse single files for manual tests
#  define ATOMUI_EXPORT
#else
#  define ATOMUI_EXPORT ATOMUI_DECL_IMPORT
#endif

namespace atomui {
ATOMUI_EXPORT std::string version_string();
}

typedef void * ATOMUI_HANDLE;

#if defined(ATOMUI_OS_WIN)
#ifndef STD_CALL_TYPE
#define STD_CALL_TYPE __stdcall
#endif
#else
#ifndef STD_CALL_TYPE
#define STD_CALL_TYPE
#endif
#endif
