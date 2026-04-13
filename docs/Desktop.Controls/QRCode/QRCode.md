# QRCode 二维码

## 概述

二维码（QRCode）是一种将信息编码为二维矩阵图形的控件，广泛用于移动端扫码登录、支付、信息分享等场景。用户通过手机或其他扫码设备扫描二维码即可获取其中编码的文本或 URL 信息。

AtomUI 的 `QRCode` 控件对齐了 [Ant Design 5.0 QRCode](https://ant.design/components/qr-code-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。底层使用 SkiaSharp 生成二维码图像，支持自定义大小、颜色、纠错等级、中心图标以及多种状态展示。

---

## 设计原理

### Ant Design 的二维码设计哲学

Ant Design 对二维码的定位是：**「将文本或链接转化为二维码图形，用于移动端扫码交互」**。为了满足不同场景需求，Ant Design 为 QRCode 提供了以下核心能力：

| 能力 | 设计意图 |
|---|---|
| 📏 **自定义尺寸** | 通过 `size` 属性控制二维码的像素大小，适配不同布局场景 |
| 🎨 **自定义颜色** | 通过 `color` / `bgColor` 属性控制二维码前景色和背景色，适配品牌色或深色模式 |
| 🖼 **中心图标** | 通过 `icon` / `iconSize` 属性在二维码中央叠加 Logo 图标，增强品牌识别 |
| 🔧 **纠错等级** | 通过 `errorLevel` 属性控制二维码的容错能力（L/M/Q/H），容错越高可覆盖越多区域（如放置图标）|
| 📊 **多种状态** | 支持 `active`（正常）、`loading`（加载中）、`expired`（已过期）、`scanned`（已扫描）四种状态 |
| 🔄 **刷新机制** | 过期状态下提供刷新按钮，允许用户重新生成二维码 |
| 🔲 **边框控制** | 通过 `bordered` 属性控制是否显示外框，适配嵌入式使用场景 |

### Avalonia 基础能力

AtomUI 的 `QRCode` 继承自 Avalonia 框架的 `Avalonia.Controls.Primitives.TemplatedControl`。`TemplatedControl` 是 Avalonia 的模板化控件基类，它提供了通过 `ControlTemplate` 定义视觉结构的能力，并通过 `OnApplyTemplate` 生命周期方法访问模板内部的命名部件。

**继承链：**

```
Control → TemplatedControl → AbstractQRCode → QRCode
```

`TemplatedControl` 提供的核心基础设施：

| 能力 | 说明 |
|---|---|
| `Template` | 定义控件的视觉模板（`ControlTemplate`） |
| `OnApplyTemplate` | 模板应用后的回调，可获取模板内命名部件 |
| `Background` / `Foreground` | 基础画刷属性 |
| `BorderBrush` / `BorderThickness` / `CornerRadius` | 边框控制属性 |
| `FontSize` / `FontFamily` | 字体属性 |
| `Padding` | 内间距属性 |

### AtomUI 的扩展设计

AtomUI `QRCode` 在 Avalonia TemplatedControl 基础上，遵循 Ant Design 5.0 规范实现了以下能力：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **二维码生成** | SkiaSharp + SkiaSharp.QrCode 库 | 使用 Skia 渲染引擎在内存中生成高质量二维码位图 |
| **自定义尺寸** | `Size` 属性控制像素大小 | 对齐 Ant Design 的 `size` 属性 |
| **自定义颜色** | `Color` 属性控制前景色，`Background` 控制背景色 | 对齐 Ant Design 的 `color` / `bgColor` 属性 |
| **中心图标** | `Icon` / `IconSize` / `IconBgColor` 属性 | 在二维码中央叠加 Logo 并提供背景色设置 |
| **纠错等级** | `EccLevel` 枚举属性 | 对齐 Ant Design 的 `errorLevel`，支持 L/M/Q/H 四级 |
| **四种状态** | `Status` 枚举属性 + 模板内状态面板 | 对齐 Ant Design 的 `status`，通过样式切换显示不同状态 UI |
| **自定义状态内容** | `LoadingContent` / `ExpiredContent` / `ScannedContent` + 对应 DataTemplate | 允许开发者完全自定义各状态下的渲染内容 |
| **刷新事件** | `RefreshRequested` 事件 + 模板内刷新按钮 | 过期状态下提供刷新能力 |
| **边框控制** | `IsBordered` 属性 | 对齐 Ant Design 的 `bordered` 属性 |
| **Design Token** | `QRCodeToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |
| **国际化** | `LanguageProvider` 本地化系统 | 状态文案（"已过期"、"已扫描"、"刷新"）支持中英文 |

---

## 功能详解

### 二维码生成机制

QRCode 控件使用 `SkiaSharp.QrCode` 库生成二维码。当 `Value`、`Color`、`Background`、`EccLevel` 或 `Size` 属性发生变化时，控件会自动重新生成二维码位图：

1. 调用 `QRCodeGenerator.CreateQrCode()` 生成 QR 矩阵数据
2. 创建 `SKSurface` 画布，按 `Size` 像素尺寸渲染
3. 使用 `Color` 属性指定的颜色绘制二维码像素，`Background` 指定背景色
4. 将渲染结果编码为 PNG 格式的 `Bitmap`
5. 通过内部 `BitmapProperty` 更新模板中的 `Image` 控件

### 状态管理（QRCodeStatus）

二维码支持四种状态，通过 `Status` 属性控制：

| 状态 | 视觉表现 | 典型场景 |
|---|---|---|
| `Active` | 正常显示二维码，无遮罩 | 默认状态，等待用户扫码 |
| `Loading` | 二维码上方覆盖半透明遮罩 + 旋转加载指示器 | 二维码正在生成或数据正在加载 |
| `Expired` | 二维码上方覆盖半透明遮罩 + "已过期"文案 + 刷新按钮 | 二维码超时失效，提示用户刷新 |
| `Scanned` | 二维码上方覆盖半透明遮罩 + "已扫描"文案 | 用户已成功扫码，等待后续操作 |

当状态不为 `Active` 时，模板中的 `Border#Mask` 半透明遮罩层变为可见，遮盖原始二维码图像。

### 自定义状态内容

每种非 Active 状态均支持自定义渲染内容：

- **Loading**：通过 `LoadingContent` / `LoadingContentTemplate` 自定义。默认显示 `Spin` 旋转加载器。
- **Expired**：通过 `ExpiredContent` / `ExpiredContentTemplate` 自定义。默认显示"二维码过期"文案 + 刷新链接按钮。
- **Scanned**：通过 `ScannedContent` / `ScannedContentTemplate` 自定义。默认显示"已扫描"文案。

当对应的 `Content` 属性为 `null` 时使用默认 UI；设置后则完全替换为自定义内容。

### 中心图标

通过 `Icon` 属性可在二维码中央放置 Logo 或品牌图标：

- `Icon`：图标图像源（`IImage` 类型，可使用 `avares://` 协议引用嵌入资源）
- `IconSize`：图标区域大小（默认 40px），图标会被居中显示在此区域内
- `IconBgColor`：图标背景色，用于提高图标与二维码之间的对比度

> **注意**：使用中心图标时建议将 `EccLevel` 设为 `Q` 或 `H`，以确保被图标遮挡区域的信息仍能被正确识别。

### 纠错等级（EccLevel）

纠错等级决定了二维码被部分遮挡或损坏后仍能被正确识别的能力：

| 等级 | 容错率 | 说明 |
|---|---|---|
| `L` | ~7% | 最低容错，二维码最紧凑 |
| `M` | ~15% | 默认等级，平衡容错与紧凑度 |
| `Q` | ~25% | 较高容错，适合放置小图标 |
| `H` | ~30% | 最高容错，适合放置较大图标或在复杂背景上使用 |

### 边框控制（IsBordered）

通过 `IsBordered` 属性控制是否显示外边框：

- `true`（默认）：显示边框，应用 `ColorSplit` 边框色和 `PaddingSM` 内间距，适合独立展示
- `false`：无边框无内间距，适合嵌入 Flyout、Popover 等弹出容器

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 文本值 `value` | ✅ 字符串 | ✅ `Value` 属性 | ✅ 完全对齐 |
| 尺寸 `size` | ✅ 数值 | ✅ `Size` 属性（int） | ✅ 完全对齐 |
| 前景色 `color` | ✅ CSS 颜色 | ✅ `Color` 属性（`IBrush`） | ✅ 完全对齐（类型不同，语义一致） |
| 背景色 `bgColor` | ✅ CSS 颜色 | ✅ `Background` 属性（继承自 `TemplatedControl`） | ✅ 对齐（使用框架原生属性） |
| 纠错等级 `errorLevel` | ✅ `L/M/Q/H` | ✅ `EccLevel` 枚举 | ✅ 完全对齐 |
| 图标 `icon` | ✅ 字符串 URL | ✅ `Icon` 属性（`IImage`） | ✅ 对齐（类型不同，语义一致） |
| 图标大小 `iconSize` | ✅ 数值 | ✅ `IconSize` 属性（int） | ✅ 完全对齐 |
| 边框 `bordered` | ✅ 布尔 | ✅ `IsBordered` 属性 | ✅ 完全对齐 |
| 状态 `status` | ✅ `active/expired/loading/scanned` | ✅ `QRCodeStatus` 枚举 | ✅ 完全对齐 |
| 刷新回调 `onRefresh` | ✅ 回调函数 | ✅ `RefreshRequested` 事件 | ✅ 对齐（事件 vs 回调，语义一致） |
| 自定义状态 UI `statusRender` | ✅ render 函数 | ✅ `LoadingContent` / `ExpiredContent` / `ScannedContent` + DataTemplate | ✅ 对齐（Avalonia 模板 vs React render） |
| 类型 `type` | ✅ `canvas/svg` | ❌ 仅位图渲染 | ⚠️ 仅支持位图方式，无 SVG 输出 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.Commons.AbstractQRCode (设备无关基类)
        └── AtomUI.Desktop.Controls.QRCode (桌面端具体实现)
```

### 跨平台分层设计

QRCode 控件遵循 AtomUI 的两层继承模型：

| 层级 | 类 | 项目 | 职责 |
|---|---|---|---|
| **基类（设备无关）** | `AbstractQRCode` | `AtomUI.Controls` | 定义所有公共属性、事件、二维码生成逻辑、模板部件交互 |
| **具体实现（桌面端）** | `QRCode` | `AtomUI.Desktop.Controls` | 注册桌面端 Token 作用域，关联桌面端主题和视觉样式 |

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施（`ControlTemplate`、`OnApplyTemplate`）、基础视觉属性（`Background`、`BorderBrush`、`CornerRadius` 等） |
| `AbstractQRCode`（`AtomUI.Controls`） | 二维码生成逻辑（SkiaSharp）、所有公共属性定义（`Value`、`Size`、`Color`、`EccLevel`、`Icon`、`Status` 等）、状态内容属性（`LoadingContent`、`ExpiredContent`、`ScannedContent`）、`RefreshRequested` 事件、模板部件交互（刷新按钮） |
| `QRCode`（`AtomUI.Desktop.Controls`） | 注册 `QRCodeToken.ScopeProvider` Token 作用域、关联桌面端 ControlTheme 和视觉样式 |

### AbstractQRCode 基类详情

`AbstractQRCode` 位于 `AtomUI.Controls.Commons` 命名空间，是 QRCode 的设备无关基类。它包含了 QRCode 的全部业务逻辑和属性定义：

**属性定义**（均为 `StyledProperty`）：

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `string` | `""` | 二维码编码的文本内容 |
| `IsBordered` | `bool` | `true` | 是否显示边框 |
| `Color` | `IBrush?` | `null`（由主题控制） | 二维码前景色 |
| `EccLevel` | `QRCodeEccLevel` | `M` | 纠错等级 |
| `Size` | `int` | `160` | 二维码像素大小 |
| `IconSize` | `int` | `40` | 中心图标区域大小 |
| `Icon` | `IImage?` | `null` | 中心图标图像 |
| `IconBgColor` | `IBrush?` | `null`（由主题控制） | 中心图标背景色 |
| `Status` | `QRCodeStatus` | `Active` | 二维码状态 |
| `LoadingContent` | `object?` | `null` | 加载状态自定义内容 |
| `LoadingContentTemplate` | `IDataTemplate?` | `null` | 加载状态内容模板 |
| `ExpiredContent` | `object?` | `null` | 过期状态自定义内容 |
| `ExpiredContentTemplate` | `IDataTemplate?` | `null` | 过期状态内容模板 |
| `ScannedContent` | `object?` | `null` | 已扫描状态自定义内容 |
| `ScannedContentTemplate` | `IDataTemplate?` | `null` | 已扫描状态内容模板 |

**事件定义**：

| 事件 | 类型 | 说明 |
|---|---|---|
| `RefreshRequested` | `EventHandler?` | 用户点击刷新按钮时触发 |

**内部逻辑**：

- `SetupQRCode()`：核心二维码生成方法，使用 SkiaSharp 渲染位图
- `OnPropertyChanged()`：监听属性变化，自动重新生成二维码
- `OnApplyTemplate()`：绑定刷新按钮点击事件

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/QRCode/AbstractQRCode.cs` | 设备无关基类，包含全部属性和生成逻辑 |
| 枚举定义 | `src/AtomUI.Controls/QRCode/QRCodeEnums.cs` | `QRCodeEccLevel`、`QRCodeStatus` 枚举 |
| 控件类 | `src/AtomUI.Desktop.Controls/QRCode/QRCode.cs` | 桌面端 QRCode 具体实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/QRCode/QRCodeToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/QRCode/Themes/QRCodeTheme.axaml` | ControlTheme AXAML |
| 本地化(en) | `src/AtomUI.Desktop.Controls/QRCode/Localization/en_US.cs` | 英文语言资源 |
| 本地化(zh) | `src/AtomUI.Desktop.Controls/QRCode/Localization/zh_CN.cs` | 中文语言资源 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/QRCodeShowCase.axaml` | 使用范例 |

---

## 模板结构

QRCode 的 ControlTemplate 采用分层 Panel 布局：

```
Border#Frame (主框架：承载背景、边框、圆角)
└── Panel
    ├── Panel (二维码内容层)
    │   ├── Image (二维码位图)
    │   └── Border#ImageFrame (中心图标框)
    │       └── Image (中心图标)
    ├── Border#Mask (半透明遮罩层，非 Active 状态时可见)
    ├── Panel#LoadingLayout (加载状态 UI)
    │   ├── Spin (默认旋转加载器)
    │   └── ContentPresenter (自定义加载内容)
    ├── Panel#ExpiredLayout (过期状态 UI)
    │   ├── StackPanel (默认过期 UI)
    │   │   ├── TextBlock ("二维码过期"文案)
    │   │   └── Button#PART_RefreshButton (刷新链接按钮)
    │   └── ContentPresenter (自定义过期内容)
    └── Panel#ScannedLayout (已扫描状态 UI)
        ├── TextBlock (默认"已扫描"文案)
        └── ContentPresenter (自定义已扫描内容)
```

**分层设计理由：**
- **遮罩层独立**：`Border#Mask` 覆盖在二维码图像上方，通过 `QRCodeMaskBackgroundColor` Token 控制半透明背景色，在非 Active 状态时遮盖原始二维码。
- **状态面板独立**：Loading / Expired / Scanned 各有独立的 Panel，通过 `Status` 属性选择器控制可见性，互不干扰。
- **默认/自定义内容共存**：每个状态面板同时包含默认 UI 和 `ContentPresenter`，通过 `Content` 属性是否为 `null` 来切换显示。

### 模板部件

| 名称 | 类型 | 说明 |
|---|---|---|
| `PART_RefreshButton` | `Button` | 过期状态下的刷新按钮，点击触发 `RefreshRequested` 事件 |

---

## 本地化支持

QRCode 控件通过 `LanguageProvider` 系统支持国际化，当前提供以下语言资源：

| 资源键 | 英文 (en_US) | 中文 (zh_CN) | 使用位置 |
|---|---|---|---|
| `Refresh` | "Refresh" | "点击刷新" | 过期状态下的刷新按钮文案 |
| `Expired` | "QR code expired" | "二维码过期" | 过期状态下的提示文案 |
| `Scanned` | "Scanned" | "已扫描" | 已扫描状态下的提示文案 |

在 AXAML 中通过 `{atom:QRCodeLangResource Expired}` 标记扩展访问本地化资源。
