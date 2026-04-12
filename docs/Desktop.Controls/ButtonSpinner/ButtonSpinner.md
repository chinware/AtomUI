# ButtonSpinner 按钮微调器

## 概述

按钮微调器（ButtonSpinner）是一个带有上下调整按钮的装饰容器，允许用户通过点击按钮、键盘方向键或鼠标滚轮来触发数值增减操作。它是一个通用的装饰容器——自身不管理数值，只负责将「增加/减少」意图通过 `Spin` 事件传递给外部逻辑。

**ButtonSpinner 是 AtomUI 的扩展控件，在 Ant Design 5.0 中没有直接对应的独立组件**。它的设计灵感来自 Ant Design 的 InputNumber（数字输入框）中的步进按钮区域，但被抽象为一个独立的、可复用的装饰容器。典型使用场景包括：

- 作为 `NumericUpDown` 的内部实现基础
- 为任意内容提供步进增减功能
- 需要上下调整按钮的自定义输入场景

ButtonSpinner 继承自 Avalonia 的 `Spinner` 基类，并融合了 AtomUI 的 Design Token 系统、尺寸系统、样式变体系统和输入控件状态系统，提供与 AtomUI 其他输入类控件一致的视觉体验。

---

## 设计原理

### Avalonia 基础能力

ButtonSpinner 继承自 `Avalonia.Controls.Primitives.Spinner`，该基类提供了以下核心能力：

| 能力 | 说明 |
|---|---|
| `Spin` 事件 | `EventHandler<SpinEventArgs>` — 当用户触发增减操作时引发 |
| `SpinDirection` | 枚举：`Increase` / `Decrease` — 指示增减方向 |
| `ValidSpinDirection` | 位标志枚举：`None` / `Increase` / `Decrease` — 控制哪些方向可用 |
| `Content` / `ContentTemplate` | 内容承载能力（继承自 `ContentControl`） |

### AtomUI 的扩展设计

ButtonSpinner 在 Avalonia 基础上增加了以下 AtomUI 特性：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种尺寸** | `ISizeTypeAware` — `SizeType` 属性（Large/Middle/Small） | 对齐 Ant Design 的尺寸系统 |
| **三种样式变体** | `IInputControlStyleVariantAware` — `StyleVariant` 属性（Outline/Filled/Borderless） | 对齐 Ant Design Input 的三种变体 |
| **输入状态** | `IInputControlStatusAware` — `Status` 属性（Error/Warning） | 对齐 Ant Design 的验证状态反馈 |
| **前后缀内容** | `InnerLeftContent` / `InnerRightContent` | 对齐 Ant Design Input 的 prefix/suffix |
| **前后附加组件** | `LeftAddOn` / `RightAddOn`（继承自 `AddOnDecoratedBox`） | 对齐 Ant Design Input 的 addonBefore/addonAfter |
| **操作按钮位置** | `ButtonSpinnerLocation` 枚举（Left/Right） | 灵活控制步进按钮的显示位置 |
| **浮动操作按钮** | `IsButtonSpinnerFloatable` 属性 | 操作按钮在鼠标悬浮时滑入，离开时滑出 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整布局 |
| **过渡动画** | `IMotionAwareControl` 接口 | 操作按钮的滑入/滑出动画 |
| **键盘/滚轮交互** | 方向键 ↑↓ / 鼠标滚轮 | 多种输入方式触发 Spin |

### 浮动操作按钮（Floatable Handle）

这是 ButtonSpinner 独有的特性：当 `IsButtonSpinnerFloatable="True"` 时，操作按钮平时隐藏，鼠标悬浮在控件上时通过滑动动画显示。该特性的设计目的是在不需要频繁调整数值的场景下减少视觉噪音。

**动画行为：**
- 鼠标进入 → 操作按钮从边缘滑入（带不透明度渐变），同时内容区域向反方向收缩让出空间
- 鼠标离开 → 操作按钮滑出，内容区域恢复原始位置
- 动画使用 `MotionDurationMid` Token 控制时长

---

## 继承关系

```
Avalonia.Controls.ContentControl
└── Avalonia.Controls.Primitives.Spinner (Spin 事件, ValidSpinDirection)
    └── AtomUI.Desktop.Controls.ButtonSpinner
        (ISizeTypeAware, IInputControlStyleVariantAware,
         IInputControlStatusAware, IMotionAwareControl, ICompactSpaceAware)
```

**内部组件关系：**

```
ButtonSpinner
├── ButtonSpinnerDecoratedBox (extends AddOnDecoratedBox)
│   ├── LeftAddOn / RightAddOn        ← 外部附加组件
│   ├── InnerLeftContent / InnerRightContent  ← 内部前后缀
│   ├── Content                        ← 用户内容
│   └── SpinnerContent
│       └── ButtonSpinnerHandle       ← 操作按钮容器
│           ├── IconButton (IncreaseButton, ↑)
│           └── IconButton (DecreaseButton, ↓)
└── ButtonSpinnerContentPanel (extends DockPanel) ← 内容布局面板
```

**实现的共享接口：**

| 接口 | 作用 |
|---|---|
| `ISizeTypeAware` | 支持 Large/Middle/Small 三种尺寸 |
| `IInputControlStyleVariantAware` | 支持 Outline/Filled/Borderless 三种样式变体 |
| `IInputControlStatusAware` | 支持 Error/Warning 验证状态 |
| `IMotionAwareControl` | 支持 `IsMotionEnabled` 动画控制 |
| `ICompactSpaceAware` | 支持紧凑空间自动布局调整 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinner.cs` | 主控件类 |
| 内部控件 | `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerDecoratedBox.cs` | 装饰容器（浮动动画逻辑） |
| 内部控件 | `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerHandle.cs` | 操作按钮容器（自定义渲染边框） |
| 内部控件 | `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerContentPanel.cs` | 内容布局面板 |
| 伪类 | `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerPseudoClass.cs` | 伪类常量 |
| Token 定义 | `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/ButtonSpinner/Themes/ButtonSpinnerTheme.axaml` | 控件主题模板 |
| 主题模板 | `src/AtomUI.Desktop.Controls/ButtonSpinner/Themes/ButtonSpinnerHandleTheme.axaml` | 操作按钮主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/ButtonSpinner/Themes/ButtonSpinnerDecoratedBoxTheme.axaml` | 装饰容器主题 |
| 主题常量 | `src/AtomUI.Desktop.Controls/ButtonSpinner/Themes/ButtonSpinnerThemeConstants.cs` | 模板部件名常量 |
| 主题注册 | `src/AtomUI.Desktop.Controls/ButtonSpinner/Themes/ButtonSpinnerThemes.axaml` | 主题资源注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ButtonSpinnerShowCase.axaml` | 使用范例 |

---

## 模板结构

```
ButtonSpinner
└── ButtonSpinnerDecoratedBox (PART_DecoratedBox)    ← 装饰容器
    ├── [LeftAddOn]                                   ← 左侧附加组件（可选）
    ├── InnerBox                                      ← 内部输入区域
    │   ├── [InnerLeftContent]                        ← 前缀（可选）
    │   ├── Content                                   ← 用户内容
    │   └── [InnerRightContent]                       ← 后缀（可选）
    ├── ButtonSpinnerHandle (SpinnerContent)           ← 操作按钮区域
    │   └── UniformGrid (1列 × 2行)
    │       ├── IconButton (PART_IncreaseButton, ↑)   ← 增加按钮
    │       └── IconButton (PART_DecreaseButton, ↓)   ← 减少按钮
    └── [RightAddOn]                                  ← 右侧附加组件（可选）
```

### 模板部件常量

| 常量 | 值 | 说明 |
|---|---|---|
| `ButtonSpinnerThemeConstants.DecoratedBoxPart` | `"PART_DecoratedBox"` | 装饰容器部件 |
| `ButtonSpinnerThemeConstants.IncreaseButtonPart` | `"PART_IncreaseButton"` | 增加按钮部件 |
| `ButtonSpinnerThemeConstants.DecreaseButtonPart` | `"PART_DecreaseButton"` | 减少按钮部件 |
| `ButtonSpinnerThemeConstants.SpinnerHandlePart` | `"PART_SpinnerHandle"` | 操作手柄部件 |

### 交互行为

| 触发方式 | 条件 | 行为 |
|---|---|---|
| 点击增加按钮 | `AllowSpin == true` | 触发 `Spin` 事件，方向 `Increase` |
| 点击减少按钮 | `AllowSpin == true` | 触发 `Spin` 事件，方向 `Decrease` |
| 键盘 ↑ 键 | `AllowSpin == true`，非 XY 导航模式 | 触发 `Spin` 事件，方向 `Increase` |
| 键盘 ↓ 键 | `AllowSpin == true`，非 XY 导航模式 | 触发 `Spin` 事件，方向 `Decrease` |
| 鼠标滚轮向上 | `AllowSpin == true`，控件获得键盘焦点 | 触发 `Spin` 事件，方向 `Increase` |
| 鼠标滚轮向下 | `AllowSpin == true`，控件获得键盘焦点 | 触发 `Spin` 事件，方向 `Decrease` |
