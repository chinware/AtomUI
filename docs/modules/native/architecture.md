# AtomUI.Native 项目架构文档

## 1. 项目概述

`AtomUI.Native` 是 AtomUI 组件库的 **原生平台交互层**，通过 P/Invoke 调用各操作系统的原生 API，为上层框架提供跨平台的窗口级底层操作能力。该项目是 `AtomUI.Core` 的直接依赖，为动效引擎（MotionScene）中的 `SceneLayer`（顶层动画渲染窗口）等场景提供关键的原生窗口控制能力。

### 核心能力

- **窗口鼠标事件穿透** — 设置/查询窗口是否忽略鼠标输入事件（click-through）
- **macOS 窗口按钮控制** — 调整标题栏红绿灯按钮位置、获取按钮尺寸、控制关闭按钮启用状态

### 依赖关系

```
AtomUI.Native
└── Avalonia    (UI 框架，用于 WindowBase 等类型)
```

### InternalsVisibleTo

以下项目可访问 `AtomUI.Native` 的 `internal` 成员：

- `AtomUI.Core`
- `AtomUI.Desktop.Controls`
- `AtomUI.Mobile.Controls`

### 项目配置

- **RootNamespace**: `AtomUI.Native`
- **AllowUnsafeBlocks**: `true`（用于非托管内存操作和 P/Invoke）

---

## 2. 目录结构

```
AtomUI.Native/
├── AtomUI.Native.csproj              # 项目文件
├── WindowExtensions.cs               # 跨平台窗口扩展方法入口（partial class）
│
├── Windows/                          # Windows 平台实现
│   ├── WindowUtils.Interop.cs        # Win32 API P/Invoke 声明
│   └── WindowUtils.Windows.cs        # Windows 平台业务逻辑
│
├── MacOS/                            # macOS 平台实现
│   ├── WindowUtils.Interop.cs        # Objective-C Runtime P/Invoke 声明
│   └── WindowUtils.MacOS.cs          # macOS 平台业务逻辑
│
└── Linux/                            # Linux 平台实现
    ├── WindowUtils.Interop.cs        # XCB/X11 P/Invoke 声明
    └── WindowUtils.Linux.cs          # Linux 平台业务逻辑
```

---

## 3. 架构设计

### 3.1 整体架构

项目采用 **partial class + 平台分离** 的架构模式，通过 C# 的 `partial class` 机制将跨平台的统一 API 与各平台的具体实现分离：

```
┌─────────────────────────────────────────────────────┐
│              上层调用方 (AtomUI.Core 等)              │
│                                                      │
│   window.SetWindowIgnoreMouseEvents(true)             │
│   window.SetMacOSOptionButtonsPosition(x, y)         │
└──────────────────────┬──────────────────────────────┘
                       │
          ┌────────────▼────────────┐
          │   WindowExtensions      │
          │   (partial class)       │
          │                         │
          │  ┌─ OS 检测路由 ──────┐ │
          │  │ IsWindows() ──────┼─┼──▶ Windows 实现
          │  │ IsMacOS()   ──────┼─┼──▶ macOS 实现
          │  │ IsLinux()   ──────┼─┼──▶ Linux 实现
          │  └───────────────────┘ │
          └────────────────────────┘
                       │
    ┌──────────────────┼──────────────────┐
    ▼                  ▼                  ▼
┌─────────┐    ┌──────────────┐    ┌──────────┐
│ Windows/ │    │    MacOS/    │    │  Linux/  │
│          │    │              │    │          │
│ Interop  │    │   Interop    │    │ Interop  │
│ (Win32)  │    │ (ObjC Runtime)│    │  (XCB)   │
│          │    │              │    │          │
│ Logic    │    │   Logic      │    │  Logic   │
└─────────┘    └──────────────┘    └──────────┘
```

### 3.2 代码组织模式

每个平台目录下统一包含两个文件：

| 文件 | 职责 |
|------|------|
| `WindowUtils.Interop.cs` | **P/Invoke 声明层** — 定义原生 API 的结构体、枚举、函数签名和辅助方法 |
| `WindowUtils.{Platform}.cs` | **业务逻辑层** — `WindowExtensions` 的 partial 实现，包含具体的平台调用逻辑 |

---

## 4. 平台实现详解

### 4.1 公共接口 (WindowExtensions.cs)

`WindowExtensions` 是一个 `internal static partial class`，作为所有原生窗口操作的统一入口。通过运行时 OS 检测路由到对应平台实现。

#### 跨平台 API

| 方法 | 签名 | 说明 |
|------|------|------|
| `SetWindowIgnoreMouseEvents` | `WindowBase.SetWindowIgnoreMouseEvents(bool flag)` | 设置窗口是否穿透鼠标事件 |
| `IsWindowIgnoreMouseEvents` | `WindowBase.IsWindowIgnoreMouseEvents() → bool` | 查询窗口是否正在穿透鼠标事件 |

**路由逻辑：**

```csharp
if (OperatingSystem.IsWindows())
    → SetWindowIgnoreMouseEventsWindows(handle, flag)
else if (OperatingSystem.IsMacOS())
    → SetWindowIgnoreMouseEventsMacOS(handle, flag)
else if (OperatingSystem.IsLinux())
    → SetWindowIgnoreMouseEventsLinux(handle, flag)
else
    → throw PlatformNotSupportedException
```

---

### 4.2 Windows 平台

#### 原生 API（Win32）

通过 `user32.dll` 的窗口扩展样式（Extended Window Styles）实现鼠标穿透。

| P/Invoke 函数 | 说明 |
|----------------|------|
| `GetWindowLongPtr` (`GetWindowLongW`) | 获取窗口扩展样式 |
| `SetWindowLongPtr` (`SetWindowLongW`) | 设置窗口扩展样式 |

| 常量 | 值 | 说明 |
|------|----|------|
| `GWL_EXSTYLE` | `-20` | 扩展窗口样式索引 |
| `WS_EX_TRANSPARENT` | `0x20L` | 鼠标事件穿透标志 |
| `WS_EX_LAYERED` | `0x80000L` | 分层窗口标志（穿透的前提条件） |

#### 实现原理

```
启用穿透: ExStyle |= (WS_EX_TRANSPARENT | WS_EX_LAYERED)
禁用穿透: ExStyle &= ~(WS_EX_TRANSPARENT | WS_EX_LAYERED)
查询穿透: (ExStyle & WS_EX_TRANSPARENT) != 0
```

---

### 4.3 macOS 平台

#### 原生 API（Objective-C Runtime）

通过 `libobjc.A.dylib` 直接调用 Objective-C 消息发送机制，操控 `NSWindow` 对象。

##### P/Invoke 核心函数

| 函数 | 说明 |
|------|------|
| `objc_getClass` | 获取 ObjC 类对象 |
| `sel_registerName` | 注册/获取 Selector |
| `objc_msgSend_*` | 消息发送（多种签名重载） |

##### 预注册 Selector 缓存

| Selector | 对应 ObjC 方法 |
|----------|----------------|
| `setIgnoresMouseEvents:` | `[NSWindow setIgnoresMouseEvents:]` |
| `ignoresMouseEvents` | `[NSWindow ignoresMouseEvents]` |
| `standardWindowButton:` | `[NSWindow standardWindowButton:]` |
| `frame` | `[NSView frame]` |
| `setFrame:` | `[NSView setFrame:]` |
| `superview` | `[NSView superview]` |
| `setNeedsDisplay:` | `[NSView setNeedsDisplay:]` |
| `styleMask` | `[NSWindow styleMask]` |
| `setStyleMask:` | `[NSWindow setStyleMask:]` |
| `setEnabled:` | `[NSControl setEnabled:]` |

##### 原生结构体与枚举

| 类型 | 说明 |
|------|------|
| `CGPoint` / `CGSize` | Core Graphics 几何基础类型 |
| `NSRect` / `NSRectFull` | AppKit 矩形结构体 |
| `NSPoint` / `NSSize` | AppKit 点和尺寸 |
| `NSWindowStyleMask` | 窗口样式掩码（Titled, Closable, Resizable 等） |
| `NSWindowButton` | 标准窗口按钮类型枚举 |
| `NSWindowCollectionBehavior` | 窗口集合行为（全屏、空间切换等） |
| `NSWindowLevel` | 窗口层级 |
| `NSBackingStoreType` | 后备存储类型 |
| `WindowButtonType` | 按钮类型（Close, Minimize, Zoom 等） |

#### macOS 专属 API

| 方法 | 说明 |
|------|------|
| `SetMacOSOptionButtonsPosition(x, y, spacing)` | 自定义红绿灯按钮位置（Close/Minimize/Zoom） |
| `GetMacOSOptionsSize(spacing)` | 获取红绿灯按钮组的总尺寸 |
| `SetMacOSWindowClosable(flag)` | 启用/禁用窗口关闭按钮 |

#### 红绿灯按钮位置调整原理

```
1. GetStandardWindowButton() 获取 Close/Minimize/Zoom 三个按钮的 NSView
2. GetFrame() 获取每个按钮当前 frame
3. 修改 frame.Origin (Cocoa 坐标系，Y轴向上)
4. SetFrame() 应用新位置
5. SetNeedsDisplay(superview) 强制重绘标题栏
```

---

### 4.4 Linux 平台

#### 原生 API（XCB / X11）

通过 `libxcb.so.1` 和 `libxcb-shape.so.0` 利用 **X11 SHAPE 扩展** 的输入形状（Input Shape）机制实现鼠标穿透。

##### P/Invoke 核心函数

**XCB 核心 (libxcb.so.1)：**

| 函数 | 说明 |
|------|------|
| `xcb_connect` | 连接 X 服务器 |
| `xcb_disconnect` | 断开连接 |
| `xcb_connection_has_error` | 检查连接错误 |
| `xcb_flush` | 刷新请求队列 |
| `xcb_request_check` | 检查请求是否出错 |
| `xcb_get_geometry` / `_reply` | 获取窗口几何信息 |

**XCB SHAPE 扩展 (libxcb-shape.so.0)：**

| 函数 | 说明 |
|------|------|
| `xcb_shape_query_version` / `_reply` | 查询 SHAPE 扩展版本 |
| `xcb_shape_rectangles_checked` | 设置窗口的形状区域 |
| `xcb_shape_get_rectangles` / `_reply` | 查询窗口的形状矩形 |
| `xcb_shape_get_rectangles_rectangles_length` | 获取形状矩形数量 |

##### 原生结构体与枚举

| 类型 | 说明 |
|------|------|
| `xcb_connection_t` | XCB 连接句柄 |
| `xcb_window_t` | X11 窗口 ID |
| `xcb_rectangle_t` | 矩形结构体 |
| `xcb_void_cookie_t` | 异步请求 cookie |
| `xcb_generic_error_t` | 通用错误结构体 |
| `xcb_get_geometry_reply_t` | 窗口几何信息 |
| `xcb_shape_op_t` | 形状操作类型（Set/Union/Subtract 等） |
| `xcb_shape_kind_t` | 形状类别（Bounding/Clip/**Input**） |
| `xcb_clip_ordering_t` | 裁剪排序方式 |

#### 实现原理

```
启用穿透:
  1. xcb_connect() 连接 X 服务器
  2. 检查 SHAPE 扩展可用性
  3. xcb_shape_rectangles_checked(
       op=SET, kind=INPUT, rects_len=0, rects=NULL)
     → 设置输入形状为空区域（所有鼠标事件穿透）
  4. xcb_flush() + xcb_disconnect()

禁用穿透:
  1. xcb_connect() 连接 X 服务器
  2. xcb_get_geometry() 获取窗口尺寸
  3. 创建覆盖整个窗口的 xcb_rectangle_t
  4. xcb_shape_rectangles_checked(
       op=SET, kind=INPUT, rects_len=1, rects=&full_rect)
     → 恢复输入形状为整个窗口
  5. xcb_flush() + xcb_disconnect()

查询穿透:
  1. xcb_shape_get_rectangles(kind=INPUT)
  2. 获取矩形数量 → 如果为 0 则表示穿透中
```

---

## 5. 类关系图

```
                    ┌─────────────────────────────────┐
                    │       WindowExtensions           │
                    │    (internal static partial)      │
                    │                                   │
                    │  ┌ 跨平台 API ────────────────┐  │
                    │  │ SetWindowIgnoreMouseEvents  │  │
                    │  │ IsWindowIgnoreMouseEvents   │  │
                    │  └────────────────────────────┘  │
                    │                                   │
                    │  ┌ macOS 专属 API ─────────────┐  │
                    │  │ SetMacOSOptionButtonsPosition│  │
                    │  │ GetMacOSOptionsSize          │  │
                    │  │ SetMacOSWindowClosable       │  │
                    │  └────────────────────────────┘  │
                    └──────┬──────────┬──────────┬─────┘
                           │          │          │
          ┌────────────────▼┐  ┌──────▼────────┐ │┌──────────────┐
          │  Windows/        │  │   MacOS/      │ ││   Linux/     │
          │  Interop         │  │   Interop     │ ││   Interop    │
          ├─────────────────┤  ├───────────────┤ │├──────────────┤
          │ user32.dll       │  │ libobjc.A.dylib│ ││ libxcb.so.1  │
          │                  │  │                │ ││ libxcb-shape │
          │ GetWindowLongPtr │  │ objc_msgSend_* │ ││ .so.0        │
          │ SetWindowLongPtr │  │ sel_registerName│ ││              │
          │                  │  │ objc_getClass  │ ││ xcb_connect  │
          │ WS_EX_TRANSPARENT│  │                │ ││ xcb_shape_*  │
          │ WS_EX_LAYERED   │  │ NSWindowButton │ ││ xcb_get_geom │
          └─────────────────┘  │ NSWindowStyle  │ │└──────────────┘
                                │ NSRect/CGPoint │ │
                                └───────────────┘ │
                                                   │
```

---

## 6. 使用场景

### 6.1 动画场景层 (SceneLayer)

`AtomUI.Core` 的 `MotionScene/SceneLayer` 是本项目的主要使用方。`SceneLayer` 是一个用于承载动画的顶层透明窗口，需要：

1. **鼠标事件穿透** — 动画窗口不应拦截用户与底层 UI 的交互
2. **透明背景** — 动画窗口需要完全透明

```csharp
// SceneLayer 构造函数中的使用
this.SetWindowIgnoreMouseEvents(true);
```

### 6.2 自定义标题栏 (macOS)

macOS 平台上定制窗口标题栏时，需要精确控制红绿灯按钮位置：

```csharp
if (OperatingSystem.IsMacOS())
{
    window.SetMacOSOptionButtonsPosition(x: 12, y: 8, spacing: 8);
    var buttonsSize = window.GetMacOSOptionsSize(spacing: 8);
    window.SetMacOSWindowClosable(true);
}
```

---

## 7. 设计亮点

1. **Partial Class 跨平台分离** — 统一的 API 表面，平台实现代码物理隔离，便于维护
2. **Interop/Logic 双层分离** — 每个平台将 P/Invoke 声明与业务逻辑分离，职责清晰
3. **Selector 预注册缓存** — macOS 平台将常用的 ObjC Selector 缓存为 `static readonly`，避免重复字符串查找
4. **SHAPE 扩展检测** — Linux 平台在操作前检测 X11 SHAPE 扩展可用性，提供优雅的错误处理
5. **`[SupportedOSPlatform]` 注解** — 所有平台特定代码都标注了平台兼容性特性，支持编译器静态分析
6. **非托管内存安全管理** — Linux 实现中通过 `try/finally` 确保 `Marshal.AllocHGlobal` 分配的内存正确释放
7. **最小化依赖** — 仅依赖 Avalonia 核心包，不引入任何额外的原生互操作库

