// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/10.

#pragma once
#include "atomui/Global.h"

#ifdef ATOMUI_OS_WINDOWS
using ATOMUI_WIN_HANDLE = HWND;
#else
using ATOMUI_WIN_HANDLE = void *;
#endif

namespace atomui
{
    class ATOMUI_EXPORT WindowUtils {
    public:
        WindowUtils() = delete;

        static void setIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle, bool flag);
        static bool ignoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle);
        static void lockWindowBuddyLayer(ATOMUI_WIN_HANDLE windowHandle,
                                         ATOMUI_WIN_HANDLE buddyHandle);
    };
}
