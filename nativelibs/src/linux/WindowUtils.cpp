// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/07/05.

#include "atomui/WindowUtils.h"
#include <iostream>
#include <cstring>
#include <xcb/xcb.h>
#include <xcb/shape.h>
#include <X11/Xlib.h>

namespace atomui
{
void WindowUtils::setIgnoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle, bool flag)
{
    xcb_connection_t* connection = xcb_connect(nullptr, nullptr);
    if (int err = xcb_connection_has_error(connection))
    {
        std::cerr << "XCB connection error: " << err << std::endl;
        return;
    }

    // 检查形状扩展
    xcb_shape_query_version_cookie_t shape_cookie = xcb_shape_query_version(connection);
    xcb_generic_error_t* error = nullptr;
    xcb_shape_query_version_reply_t* shape_reply =
        xcb_shape_query_version_reply(connection, shape_cookie, &error);

    if (error)
    {
        std::cerr << "SHAPE extension error: " << error->error_code << std::endl;
        free(error);
        xcb_disconnect(connection);
        return;
    }

    if (!shape_reply)
    {
        std::cerr << "SHAPE extension not available" << std::endl;
        xcb_disconnect(connection);
        return;
    }

    free(shape_reply);

    auto window = static_cast<xcb_window_t>(reinterpret_cast<uintptr_t>(windowHandle));

    xcb_void_cookie_t cookie;

    if (flag)
    {
        // 启用鼠标事件穿透：设置输入形状为空
        cookie = xcb_shape_rectangles_checked(
            connection,
            XCB_SHAPE_SO_SET,
            XCB_SHAPE_SK_INPUT,
            XCB_CLIP_ORDERING_UNSORTED,
            window,
            0,
            0,
            0, // num_rects = 0 表示空区域
            nullptr // rects = nullptr
        );
    }
    else
    {
        // 禁用鼠标事件穿透：恢复默认输入形状（整个窗口）
        // 获取窗口几何信息
        xcb_get_geometry_cookie_t geom_cookie = xcb_get_geometry(connection, window);
        xcb_get_geometry_reply_t* geom_reply = xcb_get_geometry_reply(connection, geom_cookie, nullptr);

        if (!geom_reply)
        {
            std::cerr << "Failed to get window geometry" << std::endl;
            xcb_disconnect(connection);
            return;
        }

        // 创建一个覆盖整个窗口的矩形
        xcb_rectangle_t rect = {
            0, // x
            0, // y
            geom_reply->width,
            geom_reply->height
        };

        free(geom_reply);

        // 设置输入形状为整个窗口
        cookie = xcb_shape_rectangles_checked(
            connection,
            XCB_SHAPE_SO_SET,
            XCB_SHAPE_SK_INPUT,
            XCB_CLIP_ORDERING_UNSORTED,
            window,
            0,
            0,
            1,
            &rect
        );
    }

    error = xcb_request_check(connection, cookie);
    if (error)
    {
        std::cerr << "Shape operation error: " << error->error_code << std::endl;
        free(error);
    }

    xcb_flush(connection);
    xcb_disconnect(connection);
}

bool WindowUtils::ignoresMouseEvents(ATOMUI_WIN_HANDLE windowHandle)
{
    xcb_connection_t* connection = xcb_connect(nullptr, nullptr);
    if (int err = xcb_connection_has_error(connection))
    {
        std::cerr << "XCB connection error: " << err << std::endl;
        return false; // 默认返回 false
    }

    // 检查形状扩展
    xcb_shape_query_version_cookie_t shape_cookie = xcb_shape_query_version(connection);
    xcb_generic_error_t* error = nullptr;
    xcb_shape_query_version_reply_t* shape_reply =
        xcb_shape_query_version_reply(connection, shape_cookie, &error);

    if (error)
    {
        std::cerr << "SHAPE extension error: " << error->error_code << std::endl;
        free(error);
        xcb_disconnect(connection);
        return false;
    }

    if (!shape_reply)
    {
        std::cerr << "SHAPE extension not available" << std::endl;
        xcb_disconnect(connection);
        return false;
    }
    free(shape_reply);

    auto window = static_cast<xcb_window_t>(reinterpret_cast<uintptr_t>(windowHandle));

    // 查询窗口的输入形状
    xcb_shape_get_rectangles_cookie_t rects_cookie =
        xcb_shape_get_rectangles(connection, window, XCB_SHAPE_SK_INPUT);

    xcb_shape_get_rectangles_reply_t* rects_reply =
        xcb_shape_get_rectangles_reply(connection, rects_cookie, &error);

    if (error)
    {
        std::cerr << "Shape query error: " << error->error_code << std::endl;
        free(error);
        xcb_disconnect(connection);
        return false;
    }

    bool ignores = false;

    if (rects_reply)
    {
        int num_rects = xcb_shape_get_rectangles_rectangles_length(rects_reply);

        // 如果矩形数量为0，表示输入区域为空（鼠标穿透）
        ignores = (num_rects == 0);

        free(rects_reply);
    }
    else
    {
        ignores = false;
    }

    xcb_disconnect(connection);
    return ignores;
}

void WindowUtils::moveWindow(ATOMUI_WIN_HANDLE windowHandle, int x, int y)
{
    Display* display = XOpenDisplay(nullptr);
    if (!display) {
        std::cerr << "无法打开 X11 显示连接" << std::endl;
        return;
    }
    // 获取原子属性
    Atom net_move_resize = XInternAtom(display, "_NET_WM_MOVERESIZE", False);
    if (net_move_resize == None) {
        std::cerr << "_NET_WM_MOVERESIZE 协议不支持" << std::endl;
        return;
    }
    auto window = reinterpret_cast<Window>(windowHandle);
    // 创建客户端消息事件
    XEvent ev = {0};
    ev.xclient.type = ClientMessage;
    ev.xclient.window = window;
    ev.xclient.message_type = net_move_resize;
    ev.xclient.format = 32;

    // 设置事件数据 (移动操作)
    // 参数格式: [x_root, y_root, direction, button, source]
    // 8 = 移动操作 (NET_WM_MOVERESIZE_MOVE)
    // 1 = 忽略边界 (NET_WM_MOVERESIZE_FORCE)
    ev.xclient.data.l[0] = x;      // 目标X位置
    ev.xclient.data.l[1] = y;      // 目标Y位置
    ev.xclient.data.l[2] = 8;      // 移动操作
    ev.xclient.data.l[3] = 0;      // 不使用特定鼠标按钮
    ev.xclient.data.l[4] = 1;      // 忽略边界标志

    // 发送事件到根窗口
    XSendEvent(display, DefaultRootWindow(display),
               False, SubstructureRedirectMask | SubstructureNotifyMask,
               &ev);
    std::cout << x << ", " << y << std::endl;

    // 确保事件被处理
    XFlush(display);
    XCloseDisplay(display);
}

}
