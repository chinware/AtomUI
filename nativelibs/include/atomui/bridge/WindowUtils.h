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

#if defined(__APPLE__) && defined(__MACH__)
#include <CoreGraphics/CGGeometry.h>
#endif

struct CGSize;

extern "C" ATOMUI_EXPORT void AtomUISetWindowIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle, bool flag);
extern "C" ATOMUI_EXPORT bool AtomUIGetWindowIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle);
extern "C" ATOMUI_EXPORT void AtomUISetMacOSCaptionButtonsPosition(ATOMUI_WIN_HANDLE windowHandle, double x, double y, double spacing);

#if defined(__APPLE__) && defined(__MACH__)
extern "C" ATOMUI_EXPORT CGSize AtomUIMacOSCaptionsSize(ATOMUI_WIN_HANDLE windowHandle, double spacing);
#endif
