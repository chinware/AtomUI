// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/12.

#include "atomui/WindowUtils.h"
#include <windows.h>

namespace atomui
{
long window_style(ATOMUI_WIN_HANDLE windowHandle)
{
    return GetWindowLong(windowHandle, GWL_STYLE);
}

void set_window_style(ATOMUI_WIN_HANDLE windowHandle, long style)
{
    SetWindowLong(windowHandle, GWL_STYLE, style);
}

long window_extended_style(ATOMUI_WIN_HANDLE windowHandle)
{
    return GetWindowLong(windowHandle, GWL_EXSTYLE);
}

void set_window_extended_style(ATOMUI_WIN_HANDLE windowHandle, long style)
{
    SetWindowLong(windowHandle, GWL_EXSTYLE, style);
}

void WindowUtils::setIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle, bool flag)
{
    auto styles = window_extended_style(windowHandle);
    if (flag)
    {
        styles |= (WS_EX_TRANSPARENT | WS_EX_LAYERED);
    }
    else
    {
        styles &= ~(WS_EX_TRANSPARENT | WS_EX_LAYERED);
    }
    set_window_extended_style(windowHandle, styles);
}

bool WindowUtils::ignoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle)
{
    const auto styles = window_extended_style(windowHandle);
    return styles & WS_EX_TRANSPARENT;
}

}
