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
#include "atomui/WindowUtils.h"

extern "C" ATOMUI_EXPORT void AtomUISetWindowIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle, bool flag);
extern "C" ATOMUI_EXPORT bool AtomUIGetWindowIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle);
