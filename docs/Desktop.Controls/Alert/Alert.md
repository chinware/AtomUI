# Alert 警告提示

## 概述

警告提示（Alert）用于页面中展示重要的提示信息。它是一种**非浮层的静态展现方式**，始终展现在页面内容区域，不会自动消失。适合展示需要用户长时间关注的信息，如操作结果反馈、注意事项、风险提示等。

Alert 区别于其他两种反馈机制：
- **Message（全局提示）**：轻量级浮层，自动消失，用于即时操作反馈
- **Notification（通知提醒框）**：右侧弹出浮层，带标题和内容，用于系统通知
- **Alert（警告提示）**：页面级静态区域，不自动消失，用于需要持续关注的信息

AtomUI 的 `Alert` 控件完整复刻了 [Ant Design 5.0 Alert](https://ant.design/components/alert-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验。

---

## 设计原理

### Ant Design 的警告提示设计哲学

Ant Design 对 Alert 的定位是：**「警告提示，展现需要关注的信息」**。Alert 是页面级的信息展示区域，用于引导用户注意与当前操作密切相关的重要信息。设计上遵循以下原则：

1. **语义化颜色** — 四种类型对应四种语义色系，用户一眼即可识别信息严重程度
2. **可组合** — 图标、描述、操作按钮可自由组合，适应不同信息密度
3. **非侵入** — 页面内嵌式展示，不阻断用户操作流

**四种类型**（按严重程度排列）：

| 类型 | 设计意图 | 色系 | 默认图标 | 典型用途 |
|---|---|---|---|---|
| ✅ **Success** | 操作成功 | 绿色系 | `CheckCircleFilled` | 表单提交成功、任务完成 |
| ℹ️ **Info** | 一般信息 | 蓝色系 | `InfoCircleFilled` | 使用说明、提示信息 |
| ⚠️ **Warning** | 需要注意 | 橙色系 | `ExclamationCircleFilled` | 重要提醒、注意事项 |
| ❌ **Error** | 错误或危险 | 红色系 | `CloseCircleFilled` | 错误提示、风险警告 |

### Avalonia TemplatedControl 基础能力

AtomUI 的 `Alert` 继承自 Avalonia 框架的 `Avalonia.Controls.Primitives.TemplatedControl`。理解这个基类的能力有助于理解 Alert 的实现方式。

**TemplatedControl 的核心职责：**

`TemplatedControl` 是 Avalonia 中模板化控件的基类，它提供了通过 `ControlTemplate` 定义 UI 结构、通过 `StyledProperty` 驱动数据绑定的基础架构。Alert 利用此能力将 UI 结构（模板）与行为逻辑（代码）分离。

**TemplatedControl 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Background` | 背景色（由主题根据 `Type` 设置不同颜色） |
| `BorderBrush` | 边框颜色（由主题根据 `Type` 设置） |
| `BorderThickness` | 边框粗细 |
| `CornerRadius` | 圆角半径 |
| `Padding` | 内间距 |
| `Foreground` | 前景色（文本颜色） |
| `FontSize` | 字体大小 |

### AtomUI 的扩展设计

AtomUI `Alert` 在 TemplatedControl 基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **四种类型** | `AlertType` 枚举 + 属性选择器驱动样式 | 通过语义化颜色引导用户注意力 |
| **自动类型图标** | `IsShowIcon` + 模板内四个预置图标 | 图标自动匹配类型，增强视觉辨识度 |
| **描述信息** | `Description` 属性 + `:has-description` 伪类 | 在主消息下方提供详细说明 |
| **可关闭** | `IsClosable` + `CloseIcon` + `CloseRequest` 事件 | 内置关闭按钮和事件机制 |
| **自定义关闭图标** | `CloseIcon` 属性（默认 `CloseOutlined`） | 允许替换默认关闭图标 |
| **额外操作** | `ExtraAction` 属性（`Control?` 类型） | 消息行右侧可放置操作按钮 |
| **跑马灯** | `IsMessageMarqueEnabled` + `MarqueeLabel` | AtomUI 扩展功能，长文本自动滚动 |
| **Design Token** | `AlertToken` + `RegisterTokenResourceScope` | 所有视觉值从全局 Token 派生 |

---

## 功能详解

### 四种类型（AlertType）

每种类型通过属性选择器 `[Type=Success]`、`[Type=Info]` 等应用不同的背景色、边框色和图标。类型之间的视觉差异完全由 Token 控制：

| 类型 | 背景色 Token | 边框色 Token | 图标色 Token | 图标 |
|---|---|---|---|---|
| `Success` | `ColorSuccessBg` | `ColorSuccessBorder` | `ColorSuccess` | `CheckCircleFilled` |
| `Info` | `ColorInfoBg` | `ColorInfoBorder` | `ColorPrimary` | `InfoCircleFilled` |
| `Warning` | `ColorWarningBg` | `ColorWarningBorder` | `ColorWarning` | `ExclamationCircleFilled` |
| `Error` | `ColorErrorBg` | `ColorErrorBorder` | `ColorError` | `CloseCircleFilled` |

### 带描述信息

设置 `Description` 后会触发一系列视觉变化：
1. `:has-description` 伪类激活
2. 内间距增大：`DefaultPadding` → `WithDescriptionPadding`
3. 图标变大：`IconSize` → `WithDescriptionIconSize`
4. 图标间距增大：`IconDefaultMargin` → `IconWithDescriptionMargin`
5. 消息文字使用更大字号（`FontSizeLG`），起到标题作用
6. 图标和关闭按钮垂直对齐方式从 `Center` → `Top`

### 可关闭

当 `IsClosable = true` 时：
- 右侧显示关闭按钮（默认图标为 `CloseOutlined`）
- 关闭按钮使用 `IconButton` 控件，自带悬浮/点击反馈
- 点击触发 `CloseRequest` 事件
- 可通过 `CloseIcon` 属性自定义关闭图标

> 注意：点击关闭按钮**不会自动移除** Alert，需要在 `CloseRequest` 事件处理中手动移除（这与 Ant Design 的行为一致）。

### 跑马灯效果

当 `IsMessageMarqueEnabled = true` 时：
- 普通 `Label#MessageLabel` 隐藏
- 替换为 `MarqueeLabel#MarqueeLabel`，长文本自动水平滚动
- 这是 AtomUI 的扩展功能，Ant Design React 版不具备

### 额外操作区

`ExtraAction` 属性接受任意 `Control`，放置在消息行右侧（通过 `DockPanel.Dock="Right"`）。典型用法是放置一个 Text 或 Link 类型的 Button。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 四种类型 `type` | ✅ success/info/warning/error | ✅ `AlertType` 枚举 | ✅ 完全对齐 |
| 图标 `showIcon` | ✅ 布尔属性 | ✅ `IsShowIcon` 属性 | ✅ 完全对齐 |
| 可关闭 `closable` | ✅ 布尔属性 | ✅ `IsClosable` 属性 | ✅ 完全对齐 |
| 关闭事件 `onClose` | ✅ 回调 | ✅ `CloseRequest` 事件 | ✅ 完全对齐 |
| 描述 `description` | ✅ ReactNode | ✅ `Description` 字符串 | ⚠️ 仅支持纯文本（React 版支持任意节点） |
| 关闭图标 `closeIcon` | ✅ ReactNode | ✅ `CloseIcon` (PathIcon) | ✅ 对齐（类型不同，语义一致） |
| 额外操作 `action` | ✅ ReactNode | ✅ `ExtraAction` (Control) | ✅ 完全对齐 |
| 消息内容 `message` | ✅ ReactNode | ✅ `Message` 字符串 | ⚠️ 仅支持纯文本 |
| 跑马灯 | ❌ 无 | ✅ `IsMessageMarqueEnabled` | 🆕 AtomUI 扩展 |
| Banner 模式 `banner` | ✅ 顶部通栏 | ❌ 暂未支持 | ⚠️ 待支持 |
| 关闭后动画 | ✅ 淡出动画 | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Alert
```

> Alert 未使用 Abstract 基类模式（没有 `AbstractAlert`），因为 Alert 的行为相对简单，且 Desktop 和 Mobile 平台上的交互差异较小，目前不需要跨平台抽象。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施、`Background`/`Foreground`/`BorderBrush` 等外观属性、`ControlTheme` 支持 |
| `Alert` | 四种类型 (`AlertType`)、图标展示 (`IsShowIcon`)、描述信息 (`Description`)、关闭功能 (`IsClosable` / `CloseIcon` / `CloseRequest`)、额外操作区 (`ExtraAction`)、跑马灯 (`IsMessageMarqueEnabled`)、Design Token 集成 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Alert/Alert.cs` | Alert 控件类（含枚举 `AlertType` 和伪类常量 `AlertPseudoClass`） |
| Token 定义 | `src/AtomUI.Desktop.Controls/Alert/AlertToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Alert/Themes/AlertTheme.axaml` | ControlTheme AXAML |
| 主题常量 | `src/AtomUI.Desktop.Controls/Alert/Themes/AlertThemeConstants.cs` | 模板部件名常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/AlertShowCase.axaml` | 使用范例 |

---

## 模板结构

Alert 的模板结构基于 `DockPanel` 布局，各区域通过 Dock 定位：

```
Border#Frame (Background + BorderBrush + CornerRadius + Padding)
└── DockPanel#RootLayout (LastChildFill=True)
    ├── Panel (Dock=Left, IsVisible=IsShowIcon)      ← 图标区域（四种类型图标互斥可见）
    │   ├── CheckCircleFilled#SuccessIcon             ← Success 类型图标
    │   ├── InfoCircleFilled#InfoIcon                 ← Info 类型图标
    │   ├── ExclamationCircleFilled#WarningIcon       ← Warning 类型图标
    │   └── CloseCircleFilled#ErrorIcon               ← Error 类型图标
    ├── IconButton#PART_CloseBtn (Dock=Right)         ← 关闭按钮（IsClosable=True 时可见）
    ├── ContentPresenter#ExtraActionPresenter (Dock=Right)  ← 额外操作区
    └── StackPanel (Orientation=Vertical, Fill)       ← 内容区域
        ├── Label#MessageLabel                        ← 消息文本（跑马灯禁用时可见）
        ├── MarqueeLabel#MarqueeLabel                 ← 消息滚动（跑马灯启用时可见）
        └── Label#DescriptionLabel                    ← 描述文本（有 Description 时可见）
```

**模板设计理由：**
- **四个图标预置**：模板中同时放置四个类型图标，通过属性选择器 `[Type=Success]` 控制可见性，避免运行时动态创建/销毁图标控件。
- **DockPanel 布局**：图标固定左侧、关闭按钮和额外操作固定右侧、内容区域填充中间，自然适应不同组合场景。
- **双消息展示器**：`Label` 和 `MarqueeLabel` 共存，通过 `IsMessageMarqueEnabled` 切换可见性，实现跑马灯和普通文本的无缝切换。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `AlertThemeConstants.CloseBtnPart` | `"PART_CloseBtn"` | 关闭按钮 |
