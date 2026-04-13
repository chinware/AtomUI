# Mentions 提及

## 概述

提及（Mentions）用于在输入框中通过特定前缀字符（默认 `@`）触发候选列表，从中选择人员、标签或其他条目并插入到文本中。适用于评论、聊天、协作编辑等社交和协同场景。

AtomUI 的 `Mentions` 控件对应 [Ant Design 5.0 Mentions](https://ant.design/components/mentions-cn) 组件，在 .NET / Avalonia 平台上提供一致的交互体验。

---

## 设计原理

### Ant Design 的 Mentions 设计哲学

Ant Design 的 Mentions 定位为：**在文本输入过程中，通过触发字符（如 `@`）弹出候选列表，实现结构化引用的输入能力**。核心设计目标包括：

- **触发式交互**：用户输入指定前缀字符后自动弹出候选列表，无需额外操作
- **实时过滤**：随着用户继续输入，候选列表根据已输入内容动态过滤
- **无缝插入**：选中候选项后，完整引用文本自动插入到光标位置
- **多触发前缀**：支持配置多个触发字符（如 `@` 提及人员、`#` 提及标签）

### Avalonia 基础能力

AtomUI 的 `Mentions` 是一个 `TemplatedControl`，内部组合了自定义的 `MentionTextArea`（继承自 `TextArea`）作为文本输入区域，并使用 `Popup` + `CandidateList` 展示候选项。`Mentions` 不直接继承自 `TextArea` 或 `TextBox`，而是通过组合模式将它们整合。

**Avalonia TemplatedControl 的核心能力：**

| 能力 | 说明 |
|---|---|
| `Template` | 通过 `ControlTheme` 定义可视化结构 |
| `OnApplyTemplate` | 模板应用后获取模板内部件引用 |
| `PseudoClasses` | 通过伪类驱动主题样式切换 |
| `StyledProperty` | 声明式属性系统，支持数据绑定 |

### AtomUI 的扩展设计

AtomUI `Mentions` 在 Avalonia 基础控件之上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **触发式候选弹窗** | `MentionTextArea` 检测触发字符 + `Popup` + `CandidateList` | 核心交互：输入触发字符后弹出候选列表 |
| **多触发前缀** | `TriggerPrefix` 属性（`IList<string>`） | 支持 `@`、`#` 等多种触发字符 |
| **候选过滤** | `IValueFilter` / `IMentionOptionFilter` / `FilterValueSelector` | 支持同步和自定义过滤逻辑 |
| **异步加载** | `IMentionOptionsAsyncLoader` + `IsLoading` 状态 | 支持从远程服务器异步加载候选项 |
| **四种样式变体** | `IInputControlStyleVariantAware`（Outline/Filled/Borderless/Underlined） | 对齐 Ant Design 的 `variant` 属性 |
| **三种尺寸** | `ISizeTypeAware`（Large/Middle/Small） | 全局统一尺寸系统 |
| **验证状态** | `IInputControlStatusAware`（Error/Warning/Default） | 表单验证视觉反馈 |
| **允许清除** | `IsAllowClear` + `ClearIcon` | 一键清空输入内容 |
| **自动高度** | `IsAutoSize` + `Lines` / `MinLines` / `MaxLines` | 输入区域高度自适应内容 |
| **候选项弹窗位置** | `Placement`（Top/Bottom） | 控制候选列表弹出方向 |
| **表单集成** | `IFormItemAware` + `IFormItemFeedbackAware` | 可作为 FormItem 子控件参与验证 |
| **动画支持** | `IMotionAwareControl` | 支持弹窗动画开关 |
| **Design Token** | `MentionsToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 系统派生 |

---

## 功能详解

### 触发机制

当用户在文本输入区域中键入触发前缀字符（默认 `@`）时，`MentionTextArea` 内部会：

1. 从当前光标位置向前扫描，查找是否存在触发字符
2. 如果找到，记录触发字符位置和已输入的过滤文本
3. 触发 `CandidateOpenRequest` 事件，通知 `Mentions` 打开候选弹窗
4. 弹窗位置基于触发字符在文本中的渲染位置（通过 `TextLayout.HitTestTextPosition` 精确定位）

当用户继续输入时，过滤值实时更新，候选列表自动刷新。当光标移出触发范围或用户按 Escape 时，候选弹窗关闭。

### 候选过滤

过滤支持三种模式：

| 模式 | 实现 | 说明 |
|---|---|---|
| 默认过滤 | 内置 `Contains` 模式 | 候选项 Header/Value/Key 包含输入文本即匹配 |
| 自定义过滤器 | `Filter` 属性（`IValueFilter`） | 完全自定义过滤逻辑 |
| 自定义取值 | `FilterValueSelector` 委托 | 自定义从 `IMentionOption` 中提取用于过滤比较的值 |

### 异步加载

通过 `OptionsAsyncLoader` 属性设置异步加载器（`IMentionOptionsAsyncLoader`），当触发弹窗时：

1. `IsLoading` 变为 `true`，弹窗内显示加载指示器（`Spin`）
2. 调用 `LoadAsync` 获取远程数据
3. 加载完成后更新 `OptionsSource` 并刷新候选列表
4. 支持取消操作（使用 `CancellationToken`）

### 候选项选择与插入

用户可通过以下方式选择候选项：

- **鼠标点击**：直接点击候选列表中的项目
- **键盘导航**：↑/↓ 键移动选中项，Enter 确认
- **Escape**：取消选择，关闭候选弹窗

选中后，候选项的 `Value`（或 `Header`）将插入到文本中，触发字符保留。如果设置了 `Split` 属性，会在插入内容前后添加分隔符。

### 弹窗位置

通过 `Placement` 属性控制候选弹窗弹出方向：

| 值 | 效果 |
|---|---|
| `Bottom`（默认） | 候选列表在触发字符下方弹出 |
| `Top` | 候选列表在触发字符上方弹出 |

弹窗水平位置始终与触发字符对齐。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本提及 | ✅ `@` 触发 | ✅ `TriggerPrefix` 属性 | ✅ 完全对齐 |
| 多触发前缀 `prefix` | ✅ 字符串数组 | ✅ `IList<string>` | ✅ 完全对齐 |
| 弹窗位置 `placement` | ✅ `top` / `bottom` | ✅ `MentionsPlacementMode` | ✅ 完全对齐 |
| 样式变体 `variant` | ✅ `outlined` / `filled` / `borderless` | ✅ `StyleVariant`（含 `Underlined`） | ✅ 超集 |
| 自动高度 `autoSize` | ✅ 布尔/对象 | ✅ `IsAutoSize` + `MinLines` / `MaxLines` | ✅ 对齐 |
| 允许清除 `allowClear` | ✅ 布尔 | ✅ `IsAllowClear` | ✅ 完全对齐 |
| 验证状态 `status` | ✅ `error` / `warning` | ✅ `InputControlStatus` | ✅ 完全对齐 |
| 过滤选项 `filterOption` | ✅ 函数 | ✅ `Filter` / `OptionFilter` | ✅ 对齐 |
| 异步加载 `loading` | ✅ 布尔 | ✅ `OptionsAsyncLoader` + `IsLoading` | ✅ 扩展（内置异步基础设施） |
| 只读 `readOnly` | ✅ 布尔 | ✅ `IsReadOnly` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔 | ✅ `IsEnabled`（继承） | ✅ 完全对齐 |
| 默认值 `defaultValue` | ✅ 字符串 | ✅ `DefaultValue` | ✅ 完全对齐 |
| 分隔符 `split` | ✅ 字符串 | ✅ `Split` | ✅ 完全对齐 |
| 选项模板 | ✅ React 子组件 | ✅ `OptionTemplate` | ✅ 对齐（Avalonia DataTemplate） |
| `notFoundContent` | ✅ React 节点 | ✅ `EmptyIndicator` + `IsShowEmptyIndicator` | ✅ 对齐 |
| `getPopupContainer` | ✅ 函数 | ❌ 不适用 | — 桌面端使用 Popup |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Mentions
        ├── implements IMotionAwareControl
        ├── implements IFormItemAware
        ├── implements IFormItemFeedbackAware
        ├── implements IInputControlStatusAware
        ├── implements IInputControlStyleVariantAware
        └── implements ISizeTypeAware
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础、`ControlTheme`、`OnApplyTemplate`、`PseudoClasses` |
| `Mentions` | 触发式候选弹窗、候选过滤与选择、异步加载、表单集成、Design Token 集成、四种样式变体、三种尺寸、验证状态 |

**内部组合的关键控件：**

| 控件 | 类型 | 说明 |
|---|---|---|
| `MentionTextArea` | `internal`，继承自 `TextArea` | 文本输入区域，负责检测触发字符和管理光标 |
| `Popup` | AtomUI `Popup` | 候选列表弹窗容器 |
| `CandidateList` | AtomUI `CandidateList` | 候选项列表组件 |
| `Spin` | AtomUI `Spin` | 异步加载指示器 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持弹窗动画开关 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 Error / Warning / Default 验证状态 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持 Outline / Filled / Borderless / Underlined 样式变体 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可参与 Form 验证流程 |
| `IFormItemFeedbackAware` | `AtomUI.Controls.Shared` | 支持表单验证反馈控件 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Mentions/Mentions.cs` | 主控件实现 |
| 输入区域 | `src/AtomUI.Desktop.Controls/Mentions/MentionTextArea.cs` | 内部文本输入区域 |
| 候选项接口 | `src/AtomUI.Desktop.Controls/Mentions/MentionOption.cs` | `IMentionOption` 接口和 `MentionOption` 记录 |
| 过滤器接口 | `src/AtomUI.Desktop.Controls/Mentions/IMentionOptionFilter.cs` | 选项过滤器接口 |
| 异步加载 | `src/AtomUI.Desktop.Controls/Mentions/DataLoad/` | 异步加载器接口和结果类型 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Mentions/MentionPseudoClass.cs` | 伪类定义 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Mentions/MentionsToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Mentions/Themes/MentionsTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/Mentions/Themes/MentionsThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/MentionsShowCase.axaml` | 使用范例 |

---

## 模板结构

Mentions 的 ControlTemplate 采用 Panel 布局，包含文本输入区域和候选弹窗两大部分：

```
Panel
├── MentionTextArea (PART_TextArea)           ← 文本输入区域（继承自 TextArea）
│   ├── 支持 StyleVariant / Status / SizeType
│   ├── 支持 IsAllowClear / ClearIcon
│   ├── 支持 ContentLeftAddOn / ContentRightAddOn
│   └── 检测触发字符并通知 Mentions
└── Popup (PART_Popup)                        ← 候选列表弹窗
    └── Border#PopupFrame                     ← 弹窗框架（背景、圆角、阴影）
        └── Panel
            ├── Spin#LoadingIndicator         ← 异步加载指示器（IsLoading 时可见）
            └── CandidateList (PART_CandidateList) ← 候选项列表（非加载时可见）
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `MentionsThemeConstants.TextAreaPart` | `"PART_TextArea"` | 文本输入区域 |
| `MentionsThemeConstants.PopupPart` | `"PART_Popup"` | 候选列表弹窗 |
| `MentionsThemeConstants.CandidateListPart` | `"PART_CandidateList"` | 候选项列表 |
