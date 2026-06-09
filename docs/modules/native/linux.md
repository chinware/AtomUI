# AtomUI.Native — Linux 平台 API 文档

## 概述

Linux 平台通过 XCB（X C Binding）库调用 X11 协议的 **SHAPE 扩展**，利用输入形状（Input Shape）机制实现窗口级鼠标事件穿透。所有 API 均标注 `[SupportedOSPlatform("linux")]`。

**依赖的原生库：**

| 库 | 说明 |
|----|------|
| `libxcb.so.1` | XCB 核心库（X11 连接、窗口几何查询等） |
| `libxcb-shape.so.0` | XCB SHAPE 扩展库（窗口形状操作） |

---

## 1. 公共扩展方法

### 1.1 SetWindowIgnoreMouseEvents

设置窗口是否忽略（穿透）鼠标输入事件。

```csharp
internal static void SetWindowIgnoreMouseEvents(this WindowBase window, bool flag)
```

**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| `window` | `WindowBase` | Avalonia 窗口实例 |
| `flag` | `bool` | `true` 启用穿透，`false` 禁用穿透 |

**异常：**

| 异常类型 | 条件 |
|----------|------|
| `ArgumentException` | 窗口句柄无效（`IntPtr.Zero`） |
| `InvalidOperationException` | XCB 连接失败 |
| `InvalidOperationException` | SHAPE 扩展不可用 |
| `InvalidOperationException` | 禁用穿透时获取窗口几何信息失败 |

**Linux 实现原理：**

利用 X11 SHAPE 扩展的 `Input Shape`（输入形状）概念：

- **启用穿透**：将窗口的输入形状设置为 **空区域**（`rects_len=0, rects=NULL`），所有鼠标事件将穿过该窗口
- **禁用穿透**：将窗口的输入形状恢复为 **整个窗口区域**（一个覆盖全窗口的矩形）

**启用穿透流程：**

```
1. xcb_connect()                     → 连接 X 服务器
2. IsShapeExtensionAvailable()       → 检查 SHAPE 扩展
3. xcb_shape_rectangles_checked(
     op=SET, kind=INPUT,
     rects_len=0, rects=NULL)         → 设置输入形状为空
4. xcb_request_check()               → 检查错误
5. xcb_flush()                        → 刷新请求
6. xcb_disconnect()                   → 断开连接
```

**禁用穿透流程：**

```
1. xcb_connect()                     → 连接 X 服务器
2. IsShapeExtensionAvailable()       → 检查 SHAPE 扩展
3. xcb_get_geometry()                → 获取窗口尺寸
4. 创建 xcb_rectangle_t { 0, 0, width, height }
5. Marshal.AllocHGlobal()            → 分配非托管内存
6. xcb_shape_rectangles_checked(
     op=SET, kind=INPUT,
     rects_len=1, rects=&rect)        → 恢复输入形状为全窗口
7. Marshal.FreeHGlobal()             → 释放非托管内存
8. xcb_request_check()               → 检查错误
9. xcb_flush()                        → 刷新请求
10. xcb_disconnect()                  → 断开连接
```

---

### 1.2 IsWindowIgnoreMouseEvents

查询窗口当前是否处于鼠标事件穿透状态。

```csharp
internal static bool IsWindowIgnoreMouseEvents(this WindowBase window)
```

**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| `window` | `WindowBase` | Avalonia 窗口实例 |

**返回值：**

| 类型 | 说明 |
|------|------|
| `bool` | `true` 表示窗口正在穿透鼠标事件（输入形状矩形数量为 0） |

**Linux 实现原理：**

查询窗口 Input Shape 中的矩形数量：

```
1. xcb_connect()                          → 连接 X 服务器
2. IsShapeExtensionAvailable()            → 检查 SHAPE 扩展
3. xcb_shape_get_rectangles(kind=INPUT)   → 查询输入形状矩形
4. xcb_shape_get_rectangles_rectangles_length() → 获取矩形数量
5. numRects == 0 → true (穿透中)
6. xcb_disconnect()                       → 断开连接
```

---

## 2. 内部辅助方法

### 2.1 IsShapeExtensionAvailable

检查 X 服务器是否支持 SHAPE 扩展。

```csharp
[SupportedOSPlatform("linux")]
private static bool IsShapeExtensionAvailable(IntPtr connection)
```

| 参数 | 类型 | 说明 |
|------|------|------|
| `connection` | `IntPtr` | XCB 连接指针 |

**返回值：** `true` 表示 SHAPE 扩展可用。

**实现：** 调用 `xcb_shape_query_version` 并检查是否返回有效的版本信息。

---

### 2.2 GetWindowGeometry

获取窗口的几何信息（位置和尺寸）。

```csharp
[SupportedOSPlatform("linux")]
private static IntPtr GetWindowGeometry(IntPtr connection, uint windowId)
```

| 参数 | 类型 | 说明 |
|------|------|------|
| `connection` | `IntPtr` | XCB 连接指针 |
| `windowId` | `uint` | X11 窗口 ID |

**返回值：** 指向 `xcb_get_geometry_reply_t` 的非托管内存指针。调用方负责使用 `Marshal.FreeHGlobal` 释放。

---

## 3. P/Invoke 声明

> 命名空间：`AtomUI.Native.Linux`  
> 类：`internal static class WindowUtilsInterop`  
> 平台：`[SupportedOSPlatform("linux")]`

### 3.1 XCB 核心函数 (libxcb.so.1)

#### xcb_connect

连接到 X 服务器。

```csharp
[DllImport("libxcb.so.1")]
internal static extern IntPtr xcb_connect(string display, IntPtr screen);
```

| 参数 | 类型 | 说明 |
|------|------|------|
| `display` | `string` | X 显示标识（空字符串使用默认 `$DISPLAY`） |
| `screen` | `IntPtr` | 屏幕号输出指针（可传 `IntPtr.Zero`） |

**返回值：** XCB 连接指针。

---

#### xcb_disconnect

断开 X 服务器连接。

```csharp
[DllImport("libxcb.so.1")]
internal static extern void xcb_disconnect(IntPtr connection);
```

---

#### xcb_connection_has_error

检查连接是否有错误。

```csharp
[DllImport("libxcb.so.1")]
internal static extern int xcb_connection_has_error(IntPtr connection);
```

**返回值：** `0` 表示无错误，非零值表示有错误。

---

#### xcb_flush

刷新待发送的请求到 X 服务器。

```csharp
[DllImport("libxcb.so.1")]
internal static extern int xcb_flush(IntPtr connection);
```

---

#### xcb_request_check

检查已发送请求的错误状态。

```csharp
[DllImport("libxcb.so.1")]
internal static extern IntPtr xcb_request_check(IntPtr connection, xcb_void_cookie_t cookie);
```

**返回值：** 错误指针，`IntPtr.Zero` 表示无错误。

---

#### xcb_get_geometry / xcb_get_geometry_reply

获取窗口几何信息（位置、尺寸、深度等）。

```csharp
[DllImport("libxcb.so.1")]
internal static extern uint xcb_get_geometry(IntPtr connection, uint window);

[DllImport("libxcb.so.1")]
internal static extern IntPtr xcb_get_geometry_reply(IntPtr connection, uint cookie, IntPtr error);
```

---

### 3.2 XCB SHAPE 扩展函数 (libxcb-shape.so.0)

#### xcb_shape_query_version / _reply

查询 SHAPE 扩展版本。

```csharp
[DllImport("libxcb-shape.so.0")]
internal static extern uint xcb_shape_query_version(IntPtr connection);

[DllImport("libxcb-shape.so.0")]
internal static extern IntPtr xcb_shape_query_version_reply(
    IntPtr connection, uint cookie, IntPtr error);
```

---

#### xcb_shape_rectangles_checked

设置窗口的形状区域（核心操作）。

```csharp
[DllImport("libxcb-shape.so.0")]
internal static extern xcb_void_cookie_t xcb_shape_rectangles_checked(
    IntPtr c,
    xcb_shape_op_t operation,
    xcb_shape_kind_t destination_kind,
    byte ordering,
    uint destination_window,
    short x_offset,
    short y_offset,
    uint rectangles_len,
    IntPtr rectangles);
```

| 参数 | 类型 | 说明 |
|------|------|------|
| `c` | `IntPtr` | XCB 连接 |
| `operation` | `xcb_shape_op_t` | 形状操作类型（通常使用 `SET`） |
| `destination_kind` | `xcb_shape_kind_t` | 形状类别（`INPUT` 用于鼠标穿透） |
| `ordering` | `byte` | 矩形排序方式 |
| `destination_window` | `uint` | 目标窗口 ID |
| `x_offset` | `short` | X 偏移 |
| `y_offset` | `short` | Y 偏移 |
| `rectangles_len` | `uint` | 矩形数量（`0` = 空区域 = 穿透） |
| `rectangles` | `IntPtr` | 矩形数组指针（`IntPtr.Zero` = 空） |

---

#### xcb_shape_get_rectangles / _reply / _rectangles_length

查询窗口的形状矩形。

```csharp
[DllImport("libxcb-shape.so.0")]
internal static extern uint xcb_shape_get_rectangles(
    IntPtr connection, uint window, xcb_shape_kind_t shapeKind);

[DllImport("libxcb-shape.so.0")]
internal static extern IntPtr xcb_shape_get_rectangles_reply(
    IntPtr connection, uint cookie, IntPtr error);

[DllImport("libxcb-shape.so.0")]
internal static extern int xcb_shape_get_rectangles_rectangles_length(IntPtr reply);
```

---

### 3.3 结构体定义

#### xcb_connection_t

XCB 连接句柄（不透明类型）。

```csharp
internal struct xcb_connection_t { }
```

#### xcb_window_t

X11 窗口标识符。

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct xcb_window_t
{
    public uint Id;
}
```

#### xcb_void_cookie_t

异步请求的 Cookie，用于后续错误检查。

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct xcb_void_cookie_t
{
    public uint Sequence;
}
```

#### xcb_rectangle_t

矩形区域，用于定义窗口形状。

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct xcb_rectangle_t
{
    public short X;       // 左上角 X
    public short Y;       // 左上角 Y
    public ushort Width;  // 宽度
    public ushort Height; // 高度
}
```

#### xcb_generic_error_t

通用错误信息结构体。

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct xcb_generic_error_t
{
    public byte ResponseType;
    public byte ErrorCode;       // 错误代码
    public ushort Sequence;
    public uint ResourceId;
    public ushort MinorCode;
    public byte MajorCode;
    public byte Pad0;
    public uint FullSequence;
}
```

#### xcb_get_geometry_reply_t

窗口几何信息。

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct xcb_get_geometry_reply_t
{
    public byte ResponseType;
    public byte Depth;           // 颜色深度
    public ushort Sequence;
    public uint Length;
    public short X;              // 窗口 X 坐标
    public short Y;              // 窗口 Y 坐标
    public ushort Width;         // 窗口宽度
    public ushort Height;        // 窗口高度
    public ushort BorderWidth;   // 边框宽度
    public byte Pad0;
    public byte Pad1;
}
```

#### xcb_shape_query_version_reply_t

SHAPE 扩展版本信息。

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct xcb_shape_query_version_reply_t
{
    public byte ResponseType;
    public byte Pad0;
    public ushort Sequence;
    public uint Length;
    public ushort MajorVersion;  // 主版本号
    public ushort MinorVersion;  // 次版本号
    public uint Pad1;
}
```

#### xcb_shape_get_rectangles_reply_t

SHAPE 矩形查询结果。

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct xcb_shape_get_rectangles_reply_t
{
    public byte ResponseType;
    public byte Ordering;
    public ushort Sequence;
    public uint Length;
    public uint RectanglesLen;   // 矩形数量
    public uint Pad0;
    public uint Pad1;
    public uint Pad2;
}
```

---

### 3.4 枚举定义

#### xcb_shape_op_t

形状操作类型。

```csharp
internal enum xcb_shape_op_t : byte
{
    XCB_SHAPE_SO_SET       = 0,  // 替换（覆盖现有形状）
    XCB_SHAPE_SO_UNION     = 1,  // 合并
    XCB_SHAPE_SO_INTERSECT = 2,  // 交集
    XCB_SHAPE_SO_SUBTRACT  = 3,  // 差集
    XCB_SHAPE_SO_INVERT    = 4   // 反转
}
```

#### xcb_shape_kind_t

形状类别。

```csharp
internal enum xcb_shape_kind_t : byte
{
    XCB_SHAPE_SK_BOUNDING = 0,  // 边界形状（影响窗口可见区域）
    XCB_SHAPE_SK_CLIP     = 1,  // 裁剪形状（影响子窗口裁剪）
    XCB_SHAPE_SK_INPUT    = 2   // 输入形状（影响鼠标事件接收区域）⭐
}
```

> ⭐ `XCB_SHAPE_SK_INPUT` 是实现鼠标穿透的关键类别。

#### xcb_clip_ordering_t

裁剪矩形排序方式。

```csharp
internal enum xcb_clip_ordering_t : byte
{
    XCB_CLIP_ORDERING_UNSORTED  = 0,  // 未排序
    XCB_CLIP_ORDERING_Y_SORTED  = 1,  // 按 Y 排序
    XCB_CLIP_ORDERING_YX_SORTED = 2,  // 按 Y-X 排序
    XCB_CLIP_ORDERING_YX_BANDED = 3   // 按 Y-X 带状排序
}
```

---

## 4. 使用示例

```csharp
// 启用窗口鼠标事件穿透
window.SetWindowIgnoreMouseEvents(true);

// 查询是否处于穿透状态
bool isPassThrough = window.IsWindowIgnoreMouseEvents();

// 禁用穿透，恢复正常鼠标交互
window.SetWindowIgnoreMouseEvents(false);
```

---

## 5. 注意事项

1. **SHAPE 扩展依赖** — 实现依赖 `libxcb-shape.so.0`，如果目标系统缺少该库会抛出 `DllNotFoundException`
2. **连接管理** — 每次操作都会创建新的 XCB 连接并在完成后断开（`try/finally` 保证释放），不复用连接
3. **非托管内存安全** — 禁用穿透时通过 `Marshal.AllocHGlobal` 分配的矩形内存在 `try/finally` 中确保释放
4. **Wayland 兼容性** — 当前实现基于 X11/XCB 协议，在纯 Wayland 环境下不可用。Wayland 需要通过 compositor 特定协议实现类似功能
5. **句柄转换** — Avalonia 的窗口句柄（`IntPtr`）通过 `(uint)handle.ToInt64()` 转换为 X11 窗口 ID
6. **错误日志** — SHAPE 操作错误通过 `Console.Error.WriteLine` 输出到标准错误流，不会抛出异常中断执行
7. **SHAPE 扩展版本检查** — 每次操作前通过 `xcb_shape_query_version` 验证扩展可用性，提供优雅的错误报告

