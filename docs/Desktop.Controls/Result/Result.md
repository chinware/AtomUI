# Result 结果

## 概述

结果（Result）用于对用户的操作结果或异常状态进行反馈。它通常占据页面的主要区域，通过图标、标题、副标题和操作按钮等元素向用户传达明确的结果信息。适用于提交表单后的成功/失败展示、HTTP 错误码页面、任务完成状态等场景。

AtomUI 的 `Result` 控件复刻了 [Ant Design 5.0 Result](https://ant.design/components/result-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的结果页设计哲学

Ant Design 对 Result 的定位是：**「用于反馈一系列操作任务的处理结果」**。核心设计原则：

1. **明确反馈** — 通过大尺寸图标和醒目标题，让用户一眼了解操作结果
2. **语义化状态** — 四种基本状态（Info/Success/Warning/Error）+ 三种 HTTP 错误码（403/404/500），共七种状态
3. **可操作** — 结果下方提供操作按钮引导用户进行下一步（如"返回首页"、"再次购买"）
4. **层次清晰** — 图标 → 标题 → 副标题 → 操作按钮 → 详细内容，信息层次从上到下递进

### Avalonia 基础能力

AtomUI 的 `Result` 继承自 `ContentControl`（通过 `AbstractResult` 中间层），因此天然支持 `Content` 和 `ContentTemplate` 属性，可在结果区域下方放置任意详细内容。继承链为：

```
Control → TemplatedControl → ContentControl → AbstractResult → Result
```

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **七种状态** | `ResultStatus` 枚举 | 覆盖四种基本语义状态和三种 HTTP 错误码 |
| **状态图标自动匹配** | `Status` 属性驱动 | Info/Success/Warning/Error 自动展示对应 Ant Design 图标 |
| **HTTP 错误码插画** | 内嵌 SVG 资源（`ResultIndicator`） | 403/404/500 自动展示对应 SVG 插画 |
| **自定义图标** | `Icon` 属性覆盖默认图标 | 灵活定制结果展示，如将 Info 图标替换为笑脸 |
| **标题/副标题** | `Header` / `SubHeader` + Template | 支持文本字符串或自定义内容模板 |
| **额外操作区** | `Extra` / `ExtraTemplate` | 放置操作按钮或附加信息 |
| **子内容** | `Content` / `ContentTemplate`（继承自 ContentControl） | 在结果区域下方展示详细信息（带背景色区域） |
| **状态色** | 实例样式根据 `Status` 匹配 Token 颜色 | 不同状态图标自动着色（蓝/绿/黄/红） |
| **Design Token** | `ResultToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 派生 |

---

## 功能详解

### 七种状态类型

`ResultStatus` 枚举定义了七种状态：

| 状态 | 设计意图 | 默认图标 / 插画 | 图标颜色 |
|---|---|---|---|
| ℹ️ `Info` | 一般信息提示 | `ExclamationCircleFilled` | `ColorInfo`（蓝色） |
| ✅ `Success` | 操作成功 | `CheckCircleFilled` | `ColorSuccess`（绿色） |
| ⚠️ `Warning` | 需要注意/警告 | `WarningFilled` | `ColorWarning`（黄色） |
| ❌ `Error` | 操作失败/错误 | `CloseCircleFilled` | `ColorError`（红色） |
| 🔒 `ErrorCode403` | 无权限访问 | SVG 插画（锁头人物） | — |
| 🔍 `ErrorCode404` | 页面未找到 | SVG 插画（问号人物） | — |
| 💥 `ErrorCode500` | 服务器错误 | SVG 插画（感叹号服务器） | — |

### 状态图标 vs HTTP 错误码插画

- **Info / Success / Warning / Error**：显示 PathIcon 图标（`PART_StatusIconPresenter` 可见），图标大小由 `IconSize` Token 控制，颜色由状态匹配的 Token 颜色控制
- **ErrorCode403 / ErrorCode404 / ErrorCode500**：显示 SVG 矢量插画（`PART_ErrorCodeImage` 可见），插画尺寸固定为 250×295，通过内嵌 SVG 字符串渲染

两类指示器互斥显示——当 Status 为基本四种状态时，图标可见、SVG 隐藏；当 Status 为 HTTP 错误码时，SVG 可见、图标隐藏。

### 自定义图标

通过设置 `Icon` 属性可以覆盖默认的状态图标。`Icon` 仅对四种基本状态（Info/Success/Warning/Error）生效，对 HTTP 错误码状态无效。自定义图标的颜色仍由状态匹配的 Token 颜色控制。

### 子内容区域

Result 继承自 `ContentControl`，`Content` 属性用于在标题和额外操作区下方展示详细信息。子内容区域有自己的背景色（`ColorFillAlter`）和内边距（`ContentPadding`），适合展示错误详情、步骤指引等内容。

### 行高计算

标题和副标题的行高通过「相对行高 × 字体大小」动态计算：
- `HeaderLineHeight = RelativeLineHeightHeading3 × HeaderFontSize`
- `SubHeaderLineHeight = RelativeLineHeight × SubHeaderFontSize`

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 四种状态 | ✅ info/success/warning/error | ✅ `ResultStatus` 枚举 | ✅ 完全对齐 |
| HTTP 错误码 | ✅ 403/404/500 | ✅ `ErrorCode403`/`ErrorCode404`/`ErrorCode500` | ✅ 完全对齐 |
| 标题 `title` | ✅ ReactNode | ✅ `Header` + `HeaderTemplate` | ✅ 完全对齐 |
| 副标题 `subTitle` | ✅ ReactNode | ✅ `SubHeader` + `SubHeaderTemplate` | ✅ 完全对齐 |
| 额外内容 `extra` | ✅ ReactNode | ✅ `Extra` + `ExtraTemplate` | ✅ 完全对齐 |
| 自定义图标 `icon` | ✅ ReactNode | ✅ `Icon` (PathIcon) | ✅ 完全对齐 |
| 子内容 `children` | ✅ ReactNode | ✅ `Content` / `ContentTemplate` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Controls.Commons.AbstractResult   ← 设备无关基类
              └── AtomUI.Desktop.Controls.Result
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | `Content` / `ContentTemplate` 子内容展示 |
| `AbstractResult`（基类层） | 全部核心逻辑：`Status` 属性、`Header`/`SubHeader` 标题、`Extra` 额外内容、`Icon` 自定义图标、`HeaderFontSize`/`SubHeaderFontSize`、状态→图标自动匹配、SVG 插画加载、行高计算 |
| `Result`（桌面层） | Token 注册 (`ResultToken.ScopeProvider`)、实例样式配置（根据 Status 为图标设置不同颜色、尺寸） |

### 桌面端 Result 的实例样式

`Result.cs` 在构造函数中通过 C# 代码配置了基于 `Status` 属性的实例样式：

```
Status = Info     → 图标颜色 = ResultInfoIconColor (ColorInfo, 蓝)
Status = Success  → 图标颜色 = ResultSuccessIconColor (ColorSuccess, 绿)
Status = Warning  → 图标颜色 = ResultWarningIconColor (ColorWarning, 黄)
Status = Error    → 图标颜色 = ResultErrorIconColor (ColorError, 红)
```

图标的 `Width`/`Height` 统一设置为 `ResultToken.IconSize`。

---

## 内部控件架构

Result 控件使用了以下内部资源（均为 `internal`）：

| 内部类 | 作用 |
|---|---|
| `ResultIndicator` (partial class) | 静态类，提供三种 HTTP 错误码的 SVG 字符串资源 |
| `ResultIndicator.NotFoundImageSource()` | 404 SVG 插画 |
| `ResultIndicator.UnauthorizedImageSource()` | 403 SVG 插画 |
| `ResultIndicator.ServerErrorImageSource()` | 500 SVG 插画 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 枚举 | `src/AtomUI.Controls/Result/ResultStatus.cs` | `ResultStatus` 状态枚举 |
| 基类 | `src/AtomUI.Controls/Result/AbstractResult.cs` | 设备无关基类 |
| SVG 资源 | `src/AtomUI.Controls/Result/NoFoundImage.cs` | 404 SVG 插画 |
| SVG 资源 | `src/AtomUI.Controls/Result/UnauthorizedImage.cs` | 403 SVG 插画 |
| SVG 资源 | `src/AtomUI.Controls/Result/ServerErrorImage.cs` | 500 SVG 插画 |
| 控件类 | `src/AtomUI.Desktop.Controls/Result/Result.cs` | 桌面端 Result（含实例样式配置） |
| Token 定义 | `src/AtomUI.Desktop.Controls/Result/ResultToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Result/Themes/ResultTheme.axaml` | ControlTheme |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ResultShowCase.cs.axaml` | 使用范例 |

---

## 模板结构

```
Border#Frame (Padding=FramePadding)
└── StackPanel#RootLayout (HorizontalAlignment=Center)
    ├── ContentPresenter#PART_StatusIconPresenter     ← 状态图标（Info/Success/Warning/Error 时可见）
    │   └── PathIcon (Width/Height=IconSize, 颜色由 Status 对应的 Token 颜色控制)
    ├── Svg#PART_ErrorCodeImage                       ← HTTP 错误码 SVG 插画（403/404/500 时可见）
    │                                                    (Width=ImageWidth=250, Height=ImageHeight=295)
    ├── ContentPresenter#Header                       ← 标题（FontSize=HeaderFontSize, Color=ColorTextHeading）
    ├── ContentPresenter#SubHeader                    ← 副标题（FontSize=SubHeaderFontSize, Color=ColorTextDescription）
    ├── ContentPresenter#ExtraContent                 ← 额外操作区（Margin=ExtraMargin）
    └── ContentPresenter#Content                      ← 子内容（Background=ColorFillAlter, Padding=ContentPadding）
```

### 模板部件常量

| 部件名 | 控件类型 | 说明 |
|---|---|---|
| `Frame` | `Border` | 根框架，控制整体内边距 |
| `RootLayout` | `StackPanel` | 垂直布局容器 |
| `PART_StatusIconPresenter` | `ContentPresenter` | 状态图标容器，仅 Info/Success/Warning/Error 时可见 |
| `PART_ErrorCodeImage` | `Svg` | HTTP 错误码 SVG 插画，仅 403/404/500 时可见 |
| `Header` | `ContentPresenter` | 标题内容 |
| `SubHeader` | `ContentPresenter` | 副标题内容 |
| `ExtraContent` | `ContentPresenter` | 额外操作区（通常放置按钮） |
| `Content` | `ContentPresenter` | 子内容区域（带背景色） |

