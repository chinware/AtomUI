# TextBlock 文本块

## 概述

TextBlock 是 AtomUI 对 Avalonia 内置 `TextBlock` 的增强包装控件，用于在界面中显示只读文本内容。它在保留 Avalonia `TextBlock` 全部文本渲染能力的基础上，额外实现了 `IFormItemAware` 接口，使其能够参与 AtomUI Form 表单验证系统。

在 AtomUI 的组件体系中，TextBlock 被广泛用于其他控件的模板内部（如 Separator 的标题文本、Descriptions 的标签文本等），是最基础的文本呈现构件。

---

## 设计原理

### Ant Design 的文本设计

Ant Design 的 `Typography.Text` 组件提供了一套完整的文本排版方案，包括文本变体（secondary、success、warning、danger）、可复制、可编辑、省略号等能力。AtomUI 的 `TextBlock` 定位为基础文本呈现控件，侧重于文本渲染和 Form 集成能力。高级排版功能（如可编辑文本）在 AtomUI 中由 `Input` 等专门控件承担。

### Avalonia TextBlock 基础能力

AtomUI 的 `TextBlock` 直接继承自 Avalonia 的 `Avalonia.Controls.TextBlock`。理解 Avalonia TextBlock 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia TextBlock 的核心职责：**

Avalonia 的 `TextBlock` 是一个轻量级的文本渲染控件，不继承自 `TemplatedControl`（没有模板），而是直接在 `Render` 方法中通过 `FormattedText` 绘制文本。它的继承链为：

```
Control → TextBlock
```

作为非模板化控件，TextBlock 的渲染非常高效——没有视觉树子节点，没有模板应用开销，适合大量文本场景（如列表项、标签）。

**Avalonia TextBlock 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Text` | 要显示的文本字符串 |
| `TextWrapping` | 文本换行模式：`NoWrap`（默认）、`Wrap`、`WrapWithOverflow` |
| `TextTrimming` | 文本裁剪模式：`None`（默认）、`CharacterEllipsis`、`WordEllipsis` |
| `TextAlignment` | 文本对齐：`Left`、`Center`、`Right`、`Justify` |
| `TextDecorations` | 文本装饰（下划线、删除线等） |
| `FontSize` | 字体大小 |
| `FontWeight` | 字体粗细 |
| `FontStyle` | 字体样式（Normal、Italic、Oblique） |
| `FontFamily` | 字体族 |
| `Foreground` | 文本前景色 |
| `Background` | 控件背景色 |
| `LineHeight` | 行高 |
| `MaxLines` | 最大显示行数 |
| `Inlines` | 内联元素集合（Run、LineBreak、InlineUIContainer 等），支持富文本排版 |
| `Padding` | 内间距 |

**Avalonia TextBlock 的关键特性：**

| 特性 | 说明 |
|---|---|
| 富文本支持 | 通过 `Inlines` 属性支持混合格式文本（不同字号、颜色、粗细的文本片段） |
| 省略号裁剪 | `TextTrimming` 可在文本溢出时显示省略号（`…`） |
| 多行换行 | `TextWrapping="Wrap"` 实现自动换行 |
| 行数限制 | `MaxLines` 限制最大显示行数，超出部分裁剪 |
| 选择支持 | 配合 `SelectableTextBlock` 可实现文本选中复制 |

### AtomUI 的扩展设计

AtomUI `TextBlock` 在 Avalonia TextBlock 的基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **表单集成** | `IFormItemAware` 接口 | 使 TextBlock 可作为只读表单字段参与 Form 验证流程，支持 `SetFormValue` / `GetFormValue` / `ClearFormValue` |
| **默认不裁剪** | 构造函数中设置 `ClipToBounds = false` | 适配 AtomUI 控件模板内部使用场景，避免文本因精确布局偏差被意外裁剪 |
| **FontStyle 默认值** | 覆盖默认值为 `FontStyle.Normal` | 确保跨平台一致的字体样式默认行为 |
| **Text 变化通知** | 监听 `TextProperty` 变化并触发 `ValueChanged` | 与 Form 验证系统集成，Text 变化时自动通知表单 |

---

## 功能详解

### Form 表单集成

TextBlock 实现了 `IFormItemAware` 接口，支持以下表单操作：

| 方法 | 行为 |
|---|---|
| `SetFormValue(object?)` | 将传入值作为字符串设置到 `Text` 属性 |
| `GetFormValue()` | 返回当前 `Text` 属性值 |
| `ClearFormValue()` | 将 `Text` 设置为 `null` |
| `NotifyValidateStatus(status)` | 接收验证状态通知（当前为空实现，预留扩展） |
| `ValueChanged` 事件 | 当 `Text` 属性变化时自动触发，通知 Form 系统数据已更新 |

这使得 TextBlock 可以在 `FormItem` 中作为只读展示字段使用，参与表单的数据收集和验证流程。

### 默认不裁剪（ClipToBounds = false）

在 AtomUI 的控件模板中，TextBlock 经常被嵌入到精确布局的容器中（如 Separator 的标题、Tag 的文本等）。默认关闭 `ClipToBounds` 可以避免由于亚像素对齐或布局边界精度导致的文本意外裁剪现象，特别是在不同 DPI 缩放级别下。

---

## 与 Ant Design 对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 文本渲染 | ✅ `Typography.Text` | ✅ `TextBlock` | ✅ 基础能力对齐 |
| 文本换行 | ✅ — | ✅ `TextWrapping` | ✅ 完全对齐 |
| 省略号裁剪 | ✅ `ellipsis` | ✅ `TextTrimming` | ✅ 完全对齐 |
| 行数限制 | ✅ `ellipsis.rows` | ✅ `MaxLines` | ✅ 完全对齐 |
| 可复制 | ✅ `copyable` | ⚠️ 需使用 `SelectableTextBlock` | ⚠️ 需配合其他控件 |
| 可编辑 | ✅ `editable` | ❌ 不适用 | — 由 Input 控件承担 |
| 文本变体 | ✅ `type` (secondary/success/warning/danger) | ❌ 暂未支持 | ⚠️ 可通过 `Foreground` 手动实现 |
| 删除线/下划线 | ✅ `delete` / `underline` | ✅ `TextDecorations` | ✅ 完全对齐 |
| 表单集成 | — | ✅ `IFormItemAware` | ✅ AtomUI 扩展 |

---

## 继承关系

```
Avalonia.Controls.TextBlock
  └── AtomUI.Desktop.Controls.TextBlock (IFormItemAware)
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.TextBlock` | 文本渲染、字体属性、文本换行/裁剪/对齐、富文本 Inlines、行高/行数限制 |
| `AtomUI.Desktop.Controls.TextBlock` | Form 表单集成（`IFormItemAware`）、默认 ClipToBounds=false、FontStyle 默认值覆盖 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IFormItemAware` | `AtomUI.Controls` | 可作为 `FormItem` 的子控件参与表单数据收集和验证流程 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/TextBlock/TextBlock.cs` | AtomUI TextBlock 实现 |
| 反射扩展 | `src/AtomUI.Desktop.Controls/TextBlock/TextBlockReflectionExtensions.cs` | 内部辅助方法 |

> **注意**：TextBlock 不使用 ControlTheme，没有 Token 定义、没有主题文件。它的样式由 Avalonia 全局 TextBlock 样式 + 使用处的父控件主题控制。
