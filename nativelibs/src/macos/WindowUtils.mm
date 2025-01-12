// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/10.

#include "atomui/WindowUtils.h"
#include <AppKit/NSWindow.h>
#include <iostream>

namespace atomui {

void WindowUtils::setIgnoresMouseEvents(ATOMUI_HANDLE windowHandle, bool flag) {
    NSWindow *window = reinterpret_cast<NSWindow *>(windowHandle);
    [window setIgnoresMouseEvents:flag];
}

bool WindowUtils::ignoresMouseEvents(ATOMUI_HANDLE windowHandle) {
    NSWindow *window = reinterpret_cast<NSWindow *>(windowHandle);
    return [window ignoresMouseEvents];
}

}