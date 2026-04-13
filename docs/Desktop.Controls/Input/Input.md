# Input 输入框控件族

## 概述

输入框是图形界面中最基础的表单域控件，用于通过键盘输入内容。AtomUI 的 Input 控件族完整复刻了 [Ant Design 5.0 Input](https://ant.design/components/input-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

AtomUI 的 Input 控件族包含以下控件：

| 控件 | 对应 Ant Design | 说明 |
|---|---|---|
| `TextBox` | — | 底层基础文本输入控件（内部基类） |
| `LineEdit` | `Input` | 单行输入框，支持前缀/后缀装饰、左右附加组件（AddOn） |
| `SearchEdit` | `Input.Search` | 搜索输入框，带有搜索按钮 |
| `TextArea` | `Input.TextArea` | 多行文本域，支持自适应高度、字符计数、可拖拽调整大小 |

> 注意：Ant Design 的 `Input.Password` 在 AtomUI 中并非独立控件，而是通过 `LineEdit` 的 `PasswordChar` + `IsEnableRevealButton` 属性组合实现。

---

## 设计原理

### Ant Design 的输入框设计哲学

Ant Design 对输入框的定位是：**「通过鼠标或键盘输入内容，是最基础的表单域的包装」**。为了适配不同的使用场景，Ant Design 建立了一套完整的输入框变体体系：

**四种输入框变体**：

| 变体 | 设计意图 | 典型用途 |
|---|---|---|
| 📝 **基础输入框（Input）** | 最常用的单行文本输入 | 用户名、邮箱、地址等 |
| 🔍 **搜索框（Input.Search）** | 带搜索操作的输入框，右侧附带搜索按钮或图标 | 全局搜索、列表过滤 |
| 🔒 **密码框（Input.Password）** | 遮蔽输入内容，带可选的明文切换按钮 | 登录密码、安全验证 |
| 📄 **文本域（Input.TextArea）** | 多行文本输入，可自适应高度 | 评论、备注、描述 |

**四种样式变体**（所有输入框类型均支持）：

| 变体 | 设计意图 |
|---|---|
| 📦 **Outline（默认）** | 标准边框样式，最通用的输入框外观 |
| 🎨 **Filled** | 填充背景样式，在复杂表单中降低视觉噪音 |
| ➖ **Borderless** | 无边框样式，用于特定场景下的极简风格 |
| 📏 **Underlined** | 下划线样式，Material Design 风格 |

**两种验证状态**：

| 状态 | 设计意图 |
|---|---|
| ⚠️ **Warning** | 橙色边框，提示用户输入可能存在问题 |
| ❌ **Error** | 红色边框，明确告知用户输入不合法 |

### Avalonia TextBox 基础能力

AtomUI 的输入框控件族构建在 Avalonia 框架的 `Avalonia.Controls.TextBox` 之上。Avalonia TextBox 的继承链为：

```
Control → TemplatedControl → InputElement → TextBox
```

**Avalonia TextBox 提供的核心能力：**

| 能力 | 说明 |
|---|---|
| 文本编辑 | `Text` 双向绑定、光标管理（`CaretIndex`）、选区（`SelectionStart` / `SelectionEnd`） |
| 密码模式 | `PasswordChar` 掩码字符、`RevealPassword` 明文切换 |
| 多行支持 | `AcceptsReturn`、`AcceptsTab`、`MaxLines` / `MinLines` |
| 输入限制 | `MaxLength`、`IsReadOnly` |
| 占位文本 | `Watermark`（AtomUI 使用 `PlaceholderText` 替代） |
| 滚动支持 | 内置 `ScrollViewer` 支持文本超出可视区域时滚动 |
| 键盘导航 | 支持 Tab 导航、焦点管理 |

**Avalonia TextBox 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |
| `:disabled` | `IsEnabled == false` |
| `:empty` | 文本为空 |

### AtomUI 的扩展设计

AtomUI 在 Avalonia TextBox 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **四种样式变体** | `StyleVariant` 枚举 + 伪类驱动样式 | 对齐 Ant Design 的 `variant` 属性 |
| **验证状态反馈** | `Status` 枚举 + `IInputControlStatusAware` 接口 | 对齐 Ant Design 的 `status`，与 Form 表单验证无缝集成 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **AddOn 装饰机制** | `AddOnDecoratedBox` 容器 | 支持左右附加组件（LeftAddOn / RightAddOn）和内部前后缀装饰 |
| **清除按钮** | `IsAllowClear` 属性 + `InputClearIconButton` | 内置清除图标，点击清空输入内容 |
| **密码显示切换** | `IsEnableRevealButton` + `RevealButton` | 复刻 Ant Design Input.Password 的密码可见性切换 |
| **字符计数** | `IsShowCount` + `MaxLength` | 实时显示已输入字符数 / 最大字符数 |
| **搜索按钮** | `SearchEdit` + `SearchButton` | 复刻 Ant Design Input.Search 的搜索按钮交互 |
| **自适应高度** | `TextArea.IsAutoSize` + `MinLines` / `MaxLines` | 文本域根据内容自动调整高度 |
| **可拖拽调整** | `TextArea.IsResizable` + `ResizeHandle` | 文本域支持拖拽调整大小 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 中自动调整圆角和边框 |
| **表单集成** | `IFormItemAware` + `IFormItemFeedbackAware` | 可参与 FormItem 验证流程 |
| **Design Token** | `LineEditToken` / `TextAreaToken` | 所有视觉值从全局 Token 派生，支持主题切换 |

---

## 控件层级关系

Input 控件族的所有控件均定义在桌面端控件层（`AtomUI.Desktop.Controls`），没有对应的设备无关基类（`AtomUI.Controls` 层），因为这些控件直接继承自 Avalonia 的 `TextBox`。

```
Avalonia.Controls.TextBox
  └── AtomUI.Desktop.Controls.TextBox (ISizeTypeAware, IMotionAwareControl, ICompactSpaceAware, IFormItemAware)
        ├── AtomUI.Desktop.Controls.LineEdit (+ IInputControlStatusAware, IInputControlStyleVariantAware)
        │     └── AtomUI.Desktop.Controls.SearchEdit (+ SearchButtonClick 事件)
        └── [独立继承]
             AtomUI.Desktop.Controls.TextArea (ISizeTypeAware, IMotionAwareControl, IInputControlStatusAware, IInputControlStyleVariantAware, IFormItemAware)
```

### 各层级职责划分

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.TextBox` | 文本编辑（输入/删除/选区/光标）、密码模式、多行支持、输入限制、滚动、键盘导航 |
| `AtomUI.Desktop.Controls.TextBox` | 尺寸系统（`SizeType`）、清除按钮（`IsAllowClear`）、密码可见性切换（`IsEnableRevealButton`）、字符计数（`IsShowCount`）、占位文本（`PlaceholderText`）、紧凑空间适配、表单集成、过渡动画 |
| `AtomUI.Desktop.Controls.LineEdit` | 样式变体（`StyleVariant`）、验证状态（`Status`）、AddOn 装饰（LeftAddOn / RightAddOn + InnerLeftContentTemplate / InnerRightContentTemplate）、与 `AddOnDecoratedBox` 集成 |
| `AtomUI.Desktop.Controls.SearchEdit` | 搜索按钮样式（`SearchButtonStyle`）、搜索按钮文本（`SearchButtonText`）、加载状态（`IsOperating`）、搜索按钮点击事件（`SearchButtonClick`） |
| `AtomUI.Desktop.Controls.TextArea` | 多行输入（`Lines`）、自适应高度（`IsAutoSize`）、可拖拽调整大小（`IsResizable`）、样式变体、验证状态、内部前后缀装饰 |

### 实现的共享接口

| 接口 | 定义位置 | 控件 | 作用 |
|---|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | TextBox, TextArea | 支持 `SizeType`（Small / Middle / Large）尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | TextBox, TextArea | 支持动画开关 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | TextBox | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | TextBox, TextArea | 参与 FormItem 表单验证 |
| `IFormItemFeedbackAware` | `AtomUI.Controls.Shared` | TextBox, TextArea | 接收表单验证反馈控件 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | LineEdit, TextArea | 支持验证状态反馈（Error / Warning） |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | LineEdit, TextArea | 支持样式变体切换（Outline / Filled / Borderless） |

---

## 功能详解

### 装饰机制（AddOnDecoratedBox）

LineEdit 使用 `AddOnDecoratedBox` 作为装饰容器，提供了灵活的布局扩展能力：

```
┌──────────────────────────────────────────────────────────────────────┐
│ LeftAddOn │ InnerLeft │ [Text Input] │ InnerRight │ RightAddOn │
│  (http://)│  (🔍 icon)│              │ (clear/eye)│  (.com)    │
└──────────────────────────────────────────────────────────────────────┘
```

- **LeftAddOn / RightAddOn**：输入框外侧的附加组件，通过 `AddOnDecoratedBox` 的外部附加区域实现，常用于固定的前缀/后缀标签（如 `http://`、`.com`）。
- **InnerLeftContentTemplate / InnerRightContentTemplate**：输入框内侧的装饰元素，位于 `AddOnDecoratedBox` 的内容区域内，常用于图标（搜索图标、用户图标）。
- **ContentRightAddOn**（模板内部）：清除按钮、密码切换按钮、表单反馈图标等功能性元素。

### 搜索框特性（SearchEdit）

SearchEdit 继承自 LineEdit，额外提供：

- **SearchButtonStyle**：搜索按钮样式，支持 `Default`（仅图标）和 `Primary`（主色调按钮）两种。
- **SearchButtonText**：搜索按钮文字，设置后按钮显示文字而非图标。
- **IsOperating**：加载状态，设为 `true` 时搜索按钮显示加载动画。
- **SearchButtonClick**：搜索按钮点击事件，路由事件（Bubble 策略）。

搜索框内部使用 `SearchEditDecoratedBox`（继承自 `AddOnDecoratedBox`）和 `SearchEditPanel`（自定义 Panel）实现独特的搜索按钮布局。

### 多行文本域特性（TextArea）

TextArea 独立继承自 Avalonia TextBox（不通过 AtomUI TextBox），提供：

- **Lines**：指定显示行数（默认 2），自动计算高度。
- **IsAutoSize**：开启后根据输入内容自动调整高度。
- **MinLines / MaxLines**：配合 `IsAutoSize` 限制自适应高度范围。
- **IsResizable**：开启后右下角显示拖拽手柄，可手动调整高度。
- **字符计数**：通过 `IsShowCount` + `MaxLength` 显示 `已输入 / 最大` 格式的计数。

TextArea 内部使用 `TextAreaDecoratedBox`（继承自 `AddOnDecoratedBox`）和 `ResizeHandle`（拖拽手柄控件）。

### 清除与密码切换

- **清除按钮**：由 `InputClearIconButton`（继承自 `IconButton`）实现，默认使用 `CloseCircleFilled` 图标。仅当 `IsAllowClear == true` 且文本不为空且非只读时显示。
- **密码切换按钮**：由 `RevealButton`（继承自 `ToggleIconButton`）实现，默认使用 `EyeOutlined`（已选中/可见）和 `EyeInvisibleOutlined`（未选中/隐藏）图标，通过 `IsEnableRevealButton` 启用。

### 表单集成

TextBox 和 TextArea 均实现了 `IFormItemAware` 和 `IFormItemFeedbackAware` 接口：

- **SetFormValue / GetFormValue / ClearFormValue**：表单系统可通过统一接口读写输入值。
- **NotifyValidateStatus**：表单验证失败时自动设置 `Status`（仅 LineEdit 和 TextArea 支持，因为它们实现了 `IInputControlStatusAware`）。
- **SetFeedbackControl**：接收并显示表单验证反馈控件（如错误/成功图标）。

### CompactSpace 支持

TextBox 实现了 `ICompactSpaceAware` 接口，在 `Space.Compact` 容器中：
- 根据位置（First / Middle / Last / OnlyOne）自动裁剪圆角。
- LineEdit 会根据内部 `AddOnDecoratedBox` 的边框厚度提供紧凑边框值。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基础输入 `Input` | ✅ | ✅ `LineEdit` | ✅ 完全对齐 |
| 搜索框 `Input.Search` | ✅ | ✅ `SearchEdit` | ✅ 完全对齐 |
| 密码框 `Input.Password` | ✅ 独立组件 | ✅ `LineEdit` + `PasswordChar` + `IsEnableRevealButton` | ✅ 功能对齐（非独立组件） |
| 文本域 `Input.TextArea` | ✅ | ✅ `TextArea` | ✅ 完全对齐 |
| 前缀/后缀 `prefix`/`suffix` | ✅ | ✅ `InnerLeftContentTemplate` / `InnerRightContentTemplate` | ✅ 完全对齐 |
| 前置/后置标签 `addonBefore`/`addonAfter` | ✅ | ✅ `LeftAddOn` / `RightAddOn` | ✅ 完全对齐 |
| 样式变体 `variant` | ✅ `outlined/filled/borderless` | ✅ `StyleVariant` + `Underlined` | ✅ 完全对齐（多一个 Underlined） |
| 验证状态 `status` | ✅ `error/warning` | ✅ `Status` | ✅ 完全对齐 |
| 允许清除 `allowClear` | ✅ | ✅ `IsAllowClear` | ✅ 完全对齐 |
| 字符计数 `showCount` | ✅ | ✅ `IsShowCount` | ✅ 完全对齐 |
| 自适应高度 `autoSize` | ✅ | ✅ `IsAutoSize` + `MinLines` / `MaxLines` | ✅ 完全对齐 |
| 搜索按钮 `enterButton` | ✅ | ✅ `SearchButtonStyle` + `SearchButtonText` | ✅ 完全对齐 |
| 搜索加载 `loading` | ✅ | ✅ `IsOperating` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ | ✅ `IsEnabled` | ✅ 完全对齐 |
| 可拖拽调整大小 | ✅ CSS resize | ✅ `IsResizable` + `ResizeHandle` | ✅ 完全对齐 |
| `onChange` 回调 | ✅ | ✅ `TextChangedEvent` + `IFormItemAware.ValueChanged` | ✅ 完全对齐 |
| `onSearch` 回调 | ✅ | ✅ `SearchButtonClick` 事件 | ✅ 完全对齐 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| TextBox 基类 | `src/AtomUI.Desktop.Controls/Input/TextBox.cs` | 底层文本输入控件（内部基类） |
| LineEdit 控件 | `src/AtomUI.Desktop.Controls/Input/LineEdit.cs` | 单行输入框实现 |
| SearchEdit 控件 | `src/AtomUI.Desktop.Controls/Input/SearchEdit.cs` | 搜索输入框实现 |
| TextArea 控件 | `src/AtomUI.Desktop.Controls/Input/TextArea.cs` | 多行文本域实现 |
| LineEdit Token | `src/AtomUI.Desktop.Controls/Input/LineEditToken.cs` | LineEdit / TextBox 组件 Token |
| TextArea Token | `src/AtomUI.Desktop.Controls/Input/TextAreaToken.cs` | TextArea 组件 Token |
| TextBox 主题 | `src/AtomUI.Desktop.Controls/Input/Themes/TextBoxTheme.axaml` | TextBox ControlTheme |
| LineEdit 主题 | `src/AtomUI.Desktop.Controls/Input/Themes/LineEditTheme.axaml` | LineEdit ControlTheme |
| SearchEdit 主题 | `src/AtomUI.Desktop.Controls/Input/Themes/SearchEditTheme.axaml` | SearchEdit ControlTheme |
| TextArea 主题 | `src/AtomUI.Desktop.Controls/Input/Themes/TextAreaTheme.axaml` | TextArea ControlTheme |
| 主题常量 | `src/AtomUI.Desktop.Controls/Input/Themes/InputThemeConstants.cs` | 模板部件名称常量 |
| 搜索按钮 | `src/AtomUI.Desktop.Controls/Input/SearchButton.cs` | 搜索按钮内部控件 |
| 搜索装饰盒 | `src/AtomUI.Desktop.Controls/Input/SearchEditDecoratedBox.cs` | 搜索编辑框装饰容器 |
| 搜索布局面板 | `src/AtomUI.Desktop.Controls/Input/SearchEditPanel.cs` | 搜索编辑框布局面板 |
| 清除按钮 | `src/AtomUI.Desktop.Controls/Input/InputClearIconButton.cs` | 清除图标按钮 |
| 密码切换按钮 | `src/AtomUI.Desktop.Controls/Input/RevealButton.cs` | 密码显示/隐藏切换按钮 |
| 文本域装饰盒 | `src/AtomUI.Desktop.Controls/Input/TextAreaDecoratedBox.cs` | 文本域装饰容器 |
| 拖拽手柄 | `src/AtomUI.Desktop.Controls/Input/ResizeHandle.cs` | 文本域拖拽调整大小手柄 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/LineEditShowCase.axaml` | 使用范例 |

---

## 模板结构

### LineEdit 模板结构

LineEdit 使用 `AddOnDecoratedBox` 作为根容器，提供完整的装饰能力：

```
AddOnDecoratedBox (PART_AddOnDecoratedBox)
├── [LeftAddOn 区域]                    ← 左侧附加组件（如 "http://"）
├── [ContentLeftAddOn 区域]             ← 内侧左装饰（如用户图标）
├── [主内容区域]
│   └── ScrollViewer (PART_ScrollViewer)
│       └── Panel
│           ├── TextBlock#Placeholder   ← 占位文本（Text 为空时显示）
│           └── TextPresenter           ← 文本展示器（PART_TextPresenter）
├── [ContentRightAddOn 区域]            ← 内侧右装饰
│   └── StackPanel (Horizontal)
│       ├── InputClearIconButton        ← 清除按钮（PART_ClearButton）
│       ├── RevealButton                ← 密码显示切换（PART_RevealButton）
│       ├── ContentPresenter#FormFeedBack ← 表单验证反馈图标
│       ├── ContentPresenter            ← InnerRightContent 展示
│       └── TextBlock#TextCountIndicator ← 字符计数显示
└── [RightAddOn 区域]                   ← 右侧附加组件（如 ".com"）
```

### SearchEdit 模板结构

SearchEdit 的模板基于 LineEdit，但使用 `SearchEditDecoratedBox` 替代标准 `AddOnDecoratedBox`，搜索按钮通过 `SearchEditPanel` 进行特殊布局。

### TextArea 模板结构

```
DockPanel
├── TextBlock#TextCountIndicator (Dock=Bottom) ← 字符计数（底部对齐）
└── Panel
    ├── TextAreaDecoratedBox (PART_AddOnDecoratedBox)
    │   ├── [ContentRightAddOn 区域]
    │   │   └── StackPanel (Vertical)
    │   │       ├── InputClearIconButton  ← 清除按钮（PART_ClearButton）
    │   │       ├── ContentPresenter#FormFeedBack ← 表单反馈
    │   │       └── ContentPresenter      ← InnerRightContent
    │   └── Panel#ContentLayout
    │       ├── TextBlock#Placeholder     ← 占位文本
    │       └── TextPresenter             ← 文本展示器（PART_TextPresenter）
    └── ResizeHandle (PART_ResizeHandle)  ← 拖拽调整大小手柄（右下角）
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `TextBoxThemeConstants.ClearButtonPart` | `"PART_ClearButton"` | 清除按钮 |
| `TextBoxThemeConstants.RevealButtonPart` | `"PART_RevealButton"` | 密码显示切换按钮 |
| `TextAreaThemeConstants.ResizeHandle` | `"PART_ResizeHandle"` | 拖拽调整大小手柄 |
| `TextAreaThemeConstants.ScrollViewerPart` | `"PART_ScrollViewer"` | 文本域滚动容器 |
| `TextAreaThemeConstants.ClearButtonPart` | `"PART_ClearButton"` | 文本域清除按钮 |
| `TextAreaThemeConstants.TextPresenterPart` | `"PART_TextPresenter"` | 文本展示器 |
