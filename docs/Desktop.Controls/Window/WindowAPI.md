# Window API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### OsType（来自 `AtomUI`）

操作系统类型枚举。

| 值 | 说明 |
|---|---|
| `Windows` | Windows 平台 |
| `macOS` | macOS 平台 |
| `Linux` | Linux 平台 |
| `Android` | Android 平台 |
| `iOS` | iOS 平台 |
| `FreeBSD` | FreeBSD 平台 |
| `Unknown` | 未知平台 |

### MediaBreakPoint（来自 `AtomUI.Controls`）

媒体断点枚举，定义响应式断点阈值。

| 值 | 数值 | 对应宽度 |
|---|---|---|
| `ExtraSmall` | `0` | ≤ 575px |
| `Small` | `576` | ≥ 576px |
| `Medium` | `768` | ≥ 768px |
| `Large` | `992` | ≥ 992px（默认值） |
| `ExtraLarge` | `1200` | ≥ 1200px |
| `ExtraExtraLarge` | `1600` | ≥ 1600px |

### WindowState（来自 `Avalonia.Controls`）

窗口状态枚举。

| 值 | 说明 |
|---|---|
| `Normal` | 正常状态 |
| `Minimized` | 最小化 |
| `Maximized` | 最大化 |
| `FullScreen` | 全屏 |

---

## Window 公共属性（StyledProperty）

### 标题栏相关

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsTitleBarVisible` | `bool` | `true` | 是否显示自定义标题栏 |
| `TitleFontSize` | `double` | `13` | 标题文字字号 |
| `TitleFontWeight` | `FontWeight` | `FontWeight.Bold` | 标题文字字重 |
| `Logo` | `Control?` | `null` | 标题栏 Logo 控件（通常放置应用图标） |
| `TitleBarContextMenu` | `ContextMenu?` | `null` | 标题栏右键菜单 |

### 标题按钮配置

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsFullScreenCaptionButtonEnabled` | `bool` | `false` | 是否显示全屏按钮 |
| `IsPinCaptionButtonEnabled` | `bool` | `false` | 是否显示置顶（Pin）按钮 |
| `IsCloseCaptionButtonEnabled` | `bool` | `true` | 是否启用关闭按钮（macOS 下通过原生 API 控制） |

### 窗口框架层（Window Frame Layer）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `WindowFrameLayer` | `object?` | `null` | 窗口级框架背景内容（覆盖整个窗口区域） |
| `WindowFrameLayerTemplate` | `IDataTemplate?` | `null` | 窗口框架背景内容模板 |
| `WindowFrameLayerOpacity` | `double` | `1.0` | 窗口框架背景层不透明度 |

### 内容框架层（Content Frame Layer）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ContentFrameLayer` | `object?` | `null` | 内容区域框架背景内容 |
| `ContentFrameLayerTemplate` | `IDataTemplate?` | `null` | 内容框架背景内容模板 |
| `ContentFrameLayerOpacity` | `double` | `1.0` | 内容框架背景层不透明度 |
| `ContentFrameBackground` | `IBrush?` | `null` | 内容框架背景画刷 |

### 标题栏框架层（TitleBar Frame Layer）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TitleBarFrameLayer` | `object?` | `null` | 标题栏框架背景内容 |
| `TitleBarFrameLayerTemplate` | `IDataTemplate?` | `null` | 标题栏框架背景内容模板 |
| `TitleBarFrameLayerOpacity` | `double` | `1.0` | 标题栏框架背景层不透明度 |
| `TitleBarFrameBackground` | `IBrush?` | `null` | 标题栏框架背景画刷 |

### 窗口行为

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `MaxWidthScreenRatio` | `double` | `double.NaN` | 窗口最大宽度占屏幕工作区的比例（0~1），`NaN` 表示不限制 |
| `MaxHeightScreenRatio` | `double` | `double.NaN` | 窗口最大高度占屏幕工作区的比例（0~1），`NaN` 表示不限制 |
| `IsMoveEnabled` | `bool` | `true` | 是否允许通过标题栏拖拽移动窗口 |

### 操作系统感知（来自 `IOperationSystemAware`）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `OsType` | `OsType` | `OsType.Unknown` | 当前操作系统类型（构造时自动检测，只读） |
| `OsVersion` | `Version` | - | 当前操作系统版本（构造时自动检测，只读） |

### 响应式布局（来自 `IMediaBreakAwareControl`）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `MediaBreakPoint` | `MediaBreakPoint` | `MediaBreakPoint.Large` | 当前媒体断点（基于内容区宽度自动计算） |

### macOS 特有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `MacOSCaptionGroupOffset` | `Point` | `(10, 0)` | macOS 红绿灯按钮组的偏移量 |
| `MacOSCaptionGroupSpacing` | `double` | `10.0` | macOS 红绿灯按钮之间的间距 |

> 注意：`MacOSCaptionGroupOffset` 和 `MacOSCaptionGroupSpacing` 标记了 `[SupportedOSPlatform("macos")]`，仅在 macOS 平台有效。

### 继承自 Avalonia.Controls.Window 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Title` | `string` | `""` | 窗口标题文本 |
| `Icon` | `WindowIcon?` | `null` | 窗口图标（任务栏/标题栏使用） |
| `Content` | `object?` | `null` | 窗口内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `WindowState` | `WindowState` | `Normal` | 窗口状态 |
| `WindowStartupLocation` | `WindowStartupLocation` | `Manual` | 启动位置 |
| `CanResize` | `bool` | `true` | 是否允许调整大小 |
| `CanMinimize` | `bool` | `true` | 是否允许最小化 |
| `CanMaximize` | `bool` | `true` | 是否允许最大化 |
| `Topmost` | `bool` | `false` | 是否置顶 |
| `ShowInTaskbar` | `bool` | `true` | 是否在任务栏显示 |
| `SystemDecorations` | `SystemDecorations` | 由主题按平台设置 | 系统装饰风格 |
| `ExtendClientAreaToDecorationsHint` | `bool` | `true`（主题设置） | 客户区扩展到装饰区域 |
| `TransparencyLevelHint` | `WindowTransparencyLevel[]` | `[Transparent]`（主题设置） | 透明度级别提示 |
| `Background` | `IBrush?` | 由 Token 控制 | 背景色 |
| `Foreground` | `IBrush?` | 由 Token 控制 | 前景色 |
| `FontSize` | `double` | 由 Token 控制 | 字体大小 |
| `FontFamily` | `FontFamily` | 由 Token 控制 | 字体族 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |
| `Padding` | `Thickness` | `0` | 内容区内间距 |
| `Width` / `Height` | `double` | `NaN` | 窗口宽度/高度 |
| `MinWidth` / `MinHeight` | `double` | `0` | 最小宽度/高度 |
| `MaxWidth` / `MaxHeight` | `double` | `+∞` | 最大宽度/高度（可由 `MaxWidthScreenRatio` 自动约束） |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `MediaBreakPointChanged` | `EventHandler<MediaBreakPointChangedEventArgs>` | 媒体断点变化时触发 |

### 继承自 Avalonia.Controls.Window 的常用事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Opened` | `EventHandler` | 窗口打开时触发 |
| `Closing` | `EventHandler<WindowClosingEventArgs>` | 窗口即将关闭时触发（可取消） |
| `Closed` | `EventHandler` | 窗口关闭后触发 |
| `ScalingChanged` | `EventHandler` | DPI 缩放比变化时触发 |

---

## 静态方法

| 方法签名 | 返回类型 | 说明 |
|---|---|---|
| `Window.GetMainWindow()` | `Window?` | 获取应用程序主窗口（从 `IClassicDesktopStyleApplicationLifetime` 获取） |

---

## 扩展方法（WindowExtensions）

`WindowExtensions` 提供以下公共扩展方法：

| 方法签名 | 返回类型 | 说明 |
|---|---|---|
| `window.GetHostScreen()` | `Screen?` | 获取窗口所在的屏幕对象 |
| `window.CenterOnScreen()` | `void` | 将窗口居中到当前所在屏幕 |
| `window.CenterOnScreen(Screen?)` | `void` | 将窗口居中到指定屏幕 |
| `window.ConstrainMaxSizeToScreenRatio(double, double)` | `void` | 按屏幕工作区比例约束窗口的 `MaxWidth` / `MaxHeight` |

---

## ReactiveWindow\<TViewModel\> 专属属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ViewModel` | `TViewModel?` | `null` | ViewModel 实例，与 `DataContext` 自动双向同步 |

`ReactiveWindow<TViewModel>` 实现 `IViewFor<TViewModel>` 接口。当 `DataContext` 变化时自动更新 `ViewModel`，反之亦然。如果 `TViewModel` 实现了 `IActivatableViewModel`，窗口显示时自动激活 ViewModel 的 `WhenActivated` 生命周期。

---

## 实现的接口

### IOperationSystemAware

```csharp
public OsType OsType { get; }       // 当前操作系统类型
public Version OsVersion { get; }   // 当前操作系统版本
```

Window 构造时自动调用 `this.ConfigureOsType()` 检测平台，后续可在样式选择器中通过 `[OsType=Windows]`、`[OsType=Linux]`、`[OsType=macOS]` 进行平台判断。

### IMediaBreakAwareControl

```csharp
public MediaBreakPoint MediaBreakPoint { get; }
public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;
```

提供内容区域宽度的响应式断点检测。`MediaBreakPoint` 由模板内的 `WindowMediaQueryIndicator` 通过 Container Query 自动更新。

### IDisposable

```csharp
public void Dispose();
```

窗口关闭时（`OnClosed`）自动调用，清理 `ScalingChanged` 等事件订阅，避免内存泄漏。
