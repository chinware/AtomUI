# AtomUI.Native — Windows 平台 API 文档

## 概述

Windows 平台通过 `user32.dll` 的窗口扩展样式（Extended Window Styles）机制实现窗口级鼠标事件穿透。所有 API 均标注 `[SupportedOSPlatform("windows")]`。

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
| `PlatformNotSupportedException` | 当前平台不受支持 |

**Windows 实现原理：**

通过修改窗口扩展样式（`GWL_EXSTYLE`）中的 `WS_EX_TRANSPARENT` 和 `WS_EX_LAYERED` 位标志：

- **启用穿透**：`ExStyle |= (WS_EX_TRANSPARENT | WS_EX_LAYERED)`
- **禁用穿透**：`ExStyle &= ~(WS_EX_TRANSPARENT | WS_EX_LAYERED)`

> ⚠️ `WS_EX_LAYERED` 是 `WS_EX_TRANSPARENT` 生效的前提条件，两者必须同时设置。

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
| `bool` | `true` 表示窗口正在穿透鼠标事件 |

**Windows 实现原理：**

读取窗口扩展样式并检查 `WS_EX_TRANSPARENT` 位：

```csharp
(GetWindowLongPtr(handle, GWL_EXSTYLE) & WS_EX_TRANSPARENT) != 0
```

---

## 2. P/Invoke 声明

> 命名空间：`AtomUI.Native.Windows`  
> 类：`internal static class WindowUtilsInterop`  
> 平台：`[SupportedOSPlatform("windows")]`

### 2.1 Win32 函数

#### GetWindowLongPtr

获取窗口的扩展样式或其他窗口属性。

```csharp
[DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
public static extern long GetWindowLongPtr(IntPtr hWnd, int nIndex);
```

| 参数 | 类型 | 说明 |
|------|------|------|
| `hWnd` | `IntPtr` | 窗口句柄 |
| `nIndex` | `int` | 属性索引（如 `GWL_EXSTYLE = -20`） |

**返回值：** 请求的窗口属性值（`long`）。

---

#### SetWindowLongPtr

设置窗口的扩展样式或其他窗口属性。

```csharp
[DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
public static extern long SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong);
```

| 参数 | 类型 | 说明 |
|------|------|------|
| `hWnd` | `IntPtr` | 窗口句柄 |
| `nIndex` | `int` | 属性索引 |
| `dwNewLong` | `long` | 新的属性值 |

**返回值：** 设置前的旧属性值（`long`）。

---

### 2.2 常量定义

| 常量 | 值 | 类型 | 说明 |
|------|----|------|------|
| `GWL_STYLE` | `-16` | `int` | 窗口样式索引 |
| `GWL_EXSTYLE` | `-20` | `int` | 扩展窗口样式索引 |
| `WS_EX_TRANSPARENT` | `0x20L` | `long` | 鼠标事件穿透标志位 |
| `WS_EX_LAYERED` | `0x80000L` | `long` | 分层窗口标志位（穿透的前提条件） |

---

## 3. 使用示例

```csharp
// 启用窗口鼠标事件穿透
window.SetWindowIgnoreMouseEvents(true);

// 查询是否处于穿透状态
bool isPassThrough = window.IsWindowIgnoreMouseEvents();

// 禁用穿透，恢复正常鼠标交互
window.SetWindowIgnoreMouseEvents(false);
```

---

## 4. 注意事项

1. **句柄有效性** — 调用前确保 `window.PlatformImpl?.Handle?.Handle` 不为 `null` 或 `IntPtr.Zero`
2. **样式组合** — `WS_EX_TRANSPARENT` 必须与 `WS_EX_LAYERED` 同时使用才能实现完整的鼠标穿透
3. **线程安全** — Win32 窗口操作应在 UI 线程执行
4. **副作用** — 设置 `WS_EX_LAYERED` 可能影响窗口的渲染行为，对于已经是分层窗口的场景无额外影响

