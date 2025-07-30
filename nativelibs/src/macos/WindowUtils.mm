// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/10.

#include "atomui/WindowUtils.h"
#include "atomui/bridge/WindowUtils.h"
#include <AppKit/NSWindow.h>
#include <iostream>
#import <Cocoa/Cocoa.h>
#import <objc/runtime.h>

namespace atomui {

void WindowUtils::setIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle, bool flag) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    [window setIgnoresMouseEvents:flag];
}

bool WindowUtils::ignoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    return [window ignoresMouseEvents];
}

// 定义按钮类型
enum WindowButtonType {
    CloseButton = 0,
    MinimizeButton,
    ZoomButton
};

// 调整按钮位置的方法
void adjust_window_captions_position(NSWindow* window, CGFloat x, CGFloat y, CGFloat spacing) {
    if (!window) {
        return;
    }

    // 获取三个标准按钮
    NSButton* closeButton = [window standardWindowButton:NSWindowCloseButton];
    NSButton* minimizeButton = [window standardWindowButton:NSWindowMiniaturizeButton];
    NSButton* zoomButton = [window standardWindowButton:NSWindowZoomButton];

    CGFloat offset = x;
    // 仅调整垂直位置（下移）
    if (closeButton) {
        NSRect frame = closeButton.frame;
        frame.origin.x = offset;
        frame.origin.y = -y;
        [closeButton setFrame:frame];
        offset += spacing + frame.size.width;
    }

    if (minimizeButton) {
        NSRect frame = minimizeButton.frame;
        frame.origin.x = offset;
        frame.origin.y = -y;
        [minimizeButton setFrame:frame];
        offset += spacing + frame.size.width;
    }

    if (zoomButton) {
        NSRect frame = zoomButton.frame;
        frame.origin.x = offset;
        frame.origin.y = -y;
        [zoomButton setFrame:frame];
    }

    // 强制重绘标题栏
    [[closeButton superview] setNeedsDisplay:YES];
}

CGSize window_captions_size(NSWindow* window, CGFloat spacing)
{
    if (!window) {
        return CGSize{0, 0};
    }

    // 获取三个标准按钮
    NSButton* closeButton = [window standardWindowButton:NSWindowCloseButton];
    NSButton* minimizeButton = [window standardWindowButton:NSWindowMiniaturizeButton];
    NSButton* zoomButton = [window standardWindowButton:NSWindowZoomButton];

    // 计算总宽度和最大高度
    CGFloat totalWidth = 0;
    CGFloat maxHeight = 0;

    if (closeButton) {
        NSRect frame = closeButton.frame;
        totalWidth += spacing + frame.size.width;
        maxHeight = MAX(maxHeight, frame.size.height);
    }

    if (minimizeButton) {
        NSRect frame = minimizeButton.frame;
        totalWidth += spacing + frame.size.width;
        maxHeight = MAX(maxHeight, frame.size.height);
    }

    if (zoomButton) {
        NSRect frame = zoomButton.frame;
        totalWidth += frame.size.width;
        maxHeight = MAX(maxHeight, frame.size.height);
    }

    return CGSize{totalWidth, maxHeight};
}

}

extern "C" {
void AtomUISetMacOSCaptionButtonsPosition(ATOMUI_WIN_HANDLE windowHandle, double x, double y, double spacing)
{
    auto window = (__bridge NSWindow *)windowHandle;
    atomui::adjust_window_captions_position(window, x, y, spacing);
}

CGSize AtomUIMacOSCaptionsSize(ATOMUI_WIN_HANDLE windowHandle, double spacing)
{
    auto window = (__bridge NSWindow *)windowHandle;
    return atomui::window_captions_size(window, spacing);
}
}