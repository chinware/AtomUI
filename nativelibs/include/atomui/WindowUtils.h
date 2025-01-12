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

namespace atomui
{
    class WindowUtils
    {
    public:
        WindowUtils() = delete;
        static void setIgnoresMouseEvents(ATOMUI_HANDLE windowHandle, bool flag);
        static bool ignoresMouseEvents(ATOMUI_HANDLE windowHandle);
    };
}
