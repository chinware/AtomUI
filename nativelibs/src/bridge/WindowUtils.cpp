// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/10.

#include "atomui/bridge/WindowUtils.h"
#include "atomui/WindowUtils.h"

using namespace atomui;

void AtomUISetWindowIgnoresMouseEvents(ATOMUI_HANDLE windowHandle, bool flag)
{
    WindowUtils::setIgnoresMouseEvents(windowHandle, flag);
}
