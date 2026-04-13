# NumericUpDown 数字输入

## 概述

数字输入框（NumericUpDown）用于输入和调整数值，支持上下步进按钮、键盘方向键和鼠标滚轮进行数值调整。它是数据录入场景中最常用的数值类控件，既能限制输入范围，又能提供便捷的步进操作。

AtomUI 的 `NumericUpDown` 控件对应 [Ant Design 5.0 InputNumber](https://ant.design/components/input-number-cn) 组件，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的数字输入设计哲学

Ant Design 对数字输入框的定位是：**「通过鼠标或键盘，输入范围内的数值」**。它解决了普通文本输入框在数值场景下的几个痛点：

- **类型安全**：只允许输入有效数字，避免非法字符
- **范围约束**：通过 `Minimum` / `Maximum` 限制取值范围，步进按钮自动感知边界
- **精确步进**：通过 `Increment` 属性控制每次增减的步长，支持小数精度
- **高精度模式**：`StringMode` 支持超出 `decimal` 精度限制的高精度数值

**三种样式变体**（对齐 Ant Design 5.0 的 `variant` 属性）：

| 变体 | 设计意图 | 典型用途 |
|---|---|---|
| 🔲 **Outline（轮廓）** | 标准边框样式，视觉权重中等（默认） | 表单中的标准输入场景 |
| 🔳 **Filled（填充）** | 灰色填充背景，无明显边框 | 需要降低视觉权重的场景 |
| ➖ **Borderless（无边框）** | 完全无边框，仅保留文本和步进按钮 | 内嵌于表格或其他控件内的紧凑场景 |

**三种尺寸**（通过 `SizeType` 控制）：

| 尺寸 | 高度 | 说明 |
|---|---|---|
| `Large` | 40px | 大号，适合触摸设备或醒目场景 |
| `Middle` | 32px | 中号（默认），常规表单场景 |
| `Small` | 24px | 小号，紧凑布局 |

### Avalonia NumericUpDown 基础能力

AtomUI 的 `NumericUpDown` 继承自 Avalonia 框架的 `Avalonia.Controls.NumericUpDown`。其继承链为：

```
Control → TemplatedControl → ContentControl → Spinner → NumericUpDown
```

**Avalonia NumericUpDown 提供的核心能力：**

| 能力 | 说明 |
|---|---|
| `Value` | 当前数值（`decimal?`），支持 null 表示无值 |
| `Minimum` / `Maximum` | 取值范围约束 |
| `Increment` | 每次步进的增量 |
| `FormatString` | 显示格式化字符串 |
| `NumberFormat` | 自定义数字格式化对象 |
| `ParsingNumberStyle` | 解析样式（控制允许的数字格式） |
| `TextConverter` | 自定义值与文本之间的转换器 |
| `IsReadOnly` | 只读模式 |
| `Text` | 显示的文本内容 |
| `Watermark` | 水印提示文本（AtomUI 通过 `PlaceholderText` 转发） |
| `ValueChanged` 事件 | 值变更时触发 |
| `ValidSpinDirection` | 有效步进方向（自动感知边界） |

### AtomUI 的扩展设计

AtomUI `NumericUpDown` 在 Avalonia NumericUpDown 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种样式变体** | `InputControlStyleVariant` 枚举 + `StyleVariant` 属性 | 对齐 Ant Design 的 `variant` 属性 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **验证状态** | `IInputControlStatusAware` + `Status` 属性 | Error / Warning 视觉反馈 |
| **高精度字符串模式** | `StringMode` + `StringValue` 属性 | 支持超出 `decimal` 精度的数值操作 |
| **键盘控制开关** | `Keyboard` 属性 | 可禁止方向键调整数值 |
| **鼠标滚轮开关** | `MouseWheel` 属性 | 可禁止滚轮调整数值 |
| **前后置标签** | `LeftAddOn` / `RightAddOn` 属性 | 输入框外部的附加内容（如协议前缀） |
| **前后置内容** | `InnerLeftContent` / `InnerRightContent` 属性 | 输入框内部的图标或文本（如货币符号） |
| **清除按钮** | `IsAllowClear` + `ClearIcon` 属性 | 一键清空输入内容 |
| **占位文本** | `PlaceholderText` / `PlaceholderForeground` 属性 | 自定义占位提示文本和颜色 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整圆角和边框 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `NumericUpDownToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 高精度字符串模式（StringMode）

当处理超出 `decimal` 精度范围的数值时（如科学计算、财务高精度场景），可启用 `StringMode`：

- `StringMode = true` 时，控件通过 `StringValue` 属性（`string?` 类型）读写原始数值字符串
- 内部使用自定义 `NumericUpDownTextConverter` 绕过 `decimal` 精度限制
- `StringValue` 和 `Value` 会双向同步（在可解析范围内）
- 编辑时显示原始字符串，失焦时按格式化规则显示

### 键盘与鼠标控制

- **Keyboard 属性**（默认 `true`）：控制是否允许通过 Up/Down/PageUp/PageDown 键调整数值。设为 `false` 时，方向键事件被拦截且标记为 Handled
- **MouseWheel 属性**（默认 `true`）：控制是否允许通过鼠标滚轮调整数值。设为 `false` 时，滚轮事件被拦截

### 清除按钮

当 `IsAllowClear = true` 时：
- 输入框非空且非只读时，右侧显示清除按钮（默认图标 `CloseCircleFilled`）
- 点击清除按钮将 `Value` 设为 `null`
- 可通过 `ClearIcon` 属性自定义清除图标

### 前后置内容

NumericUpDown 支持两级内容附加：

| 类型 | 属性 | 位置 | 说明 |
|---|---|---|---|
| 外部标签 | `LeftAddOn` / `RightAddOn` | 输入框**外部**两侧 | 有独立背景的附加区域（如 `http://`、`.com`） |
| 内部内容 | `InnerLeftContent` / `InnerRightContent` | 输入框**内部**两侧 | 嵌入输入框内的图标或文本（如 `￥`、`RMB`） |

### 表单集成

通过 `IFormItemAware` 接口，NumericUpDown 可作为 `FormItem` 的子控件：
- `SetFormValue` / `GetFormValue` / `ClearFormValue` 操作 `Value` 属性
- `NotifyValidateStatus` 将表单验证状态映射到 `Status` 属性（Error → `InputControlStatus.Error`，Warning → `InputControlStatus.Warning`）

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 数值输入 | ✅ `value` / `onChange` | ✅ `Value` / `ValueChanged` | ✅ 完全对齐 |
| 最小/最大值 | ✅ `min` / `max` | ✅ `Minimum` / `Maximum` | ✅ 完全对齐 |
| 步长 | ✅ `step` | ✅ `Increment` | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 三种变体 `variant` | ✅ `outlined / filled / borderless` | ✅ `StyleVariant` 属性 | ✅ 完全对齐 |
| 验证状态 `status` | ✅ `error / warning` | ✅ `Status` 属性 | ✅ 完全对齐 |
| 高精度 `stringMode` | ✅ 布尔属性 | ✅ `StringMode` + `StringValue` | ✅ 完全对齐 |
| 键盘控制 `keyboard` | ✅ 布尔属性 | ✅ `Keyboard` 属性 | ✅ 完全对齐 |
| 鼠标滚轮 `changeOnWheel` | ✅ 5.14.0 新增 | ✅ `MouseWheel` 属性 | ✅ 完全对齐 |
| 前后置标签 `addonBefore/After` | ✅ ReactNode | ✅ `LeftAddOn` / `RightAddOn` | ✅ 完全对齐 |
| 前后置内容 `prefix/suffix` | ✅ ReactNode | ✅ `InnerLeftContent` / `InnerRightContent` | ✅ 完全对齐 |
| 清除 | ✅ `allowClear` 5.21 | ✅ `IsAllowClear` | ✅ 完全对齐 |
| 占位文本 `placeholder` | ✅ 字符串 | ✅ `PlaceholderText` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔属性 | ✅ `IsEnabled` 取反 | ✅ 完全对齐 |
| 只读 `readOnly` | ✅ 布尔属性 | ✅ `IsReadOnly` | ✅ 完全对齐 |
| 格式化 `formatter/parser` | ✅ 函数 | ⚠️ `FormatString` + `TextConverter` | ⚠️ 机制不同，语义对齐 |
| 控件宽度 `controls` | ✅ 控制步进按钮显隐 | ❌ 暂未暴露公共属性 | ⚠️ 通过 ButtonSpinner 可间接控制 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Primitives.Spinner
              └── Avalonia.Controls.NumericUpDown
                    └── AtomUI.Desktop.Controls.NumericUpDown
                          ├── implements ISizeTypeAware
                          ├── implements IMotionAwareControl
                          ├── implements ICompactSpaceAware
                          ├── implements IFormItemAware
                          ├── implements IInputControlStatusAware
                          └── implements IInputControlStyleVariantAware
```

`NumericUpDown` 通过 `using AvaloniaNumericUpDown = Avalonia.Controls.NumericUpDown;` 别名引用 Avalonia 原生控件，避免类名冲突。

> **注意**：与 Button/Tag 等控件不同，NumericUpDown 没有在 `AtomUI.Controls` 中定义 `Abstract*` 基类，而是直接继承 Avalonia 内置的 `NumericUpDown`。这是因为数字输入框的核心行为（数值解析、步进、范围校验）在 Avalonia 层已经完备，AtomUI 主要在其上叠加 Ant Design 样式体系。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Spinner` | 步进事件（`Spin`）、步进方向验证（`ValidSpinDirection`）基础设施 |
| `Avalonia.Controls.NumericUpDown` | `Value` / `Minimum` / `Maximum` / `Increment` 数值管理、文本解析与格式化、`ValueChanged` 事件 |
| `AtomUI.Desktop.Controls.NumericUpDown` | Ant Design 视觉体系（三种变体/三种尺寸/验证状态）、Design Token 集成、StringMode 高精度、键盘/滚轮控制开关、AddOn 附加内容、清除按钮、表单集成 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 Error / Warning 验证状态视觉反馈 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持 Outline / Filled / Borderless 样式变体 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/NumericUpDown/NumericUpDown.cs` | 桌面端 NumericUpDown 实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/NumericUpDown/NumericUpDownToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/NumberUpDownShowCase.axaml` | 使用范例 |
| Gallery ViewModel | `controlgallery/AtomUIGallery/ShowCases/ViewModels/DataEntry/NumberUpDownViewModel.cs` | 范例 ViewModel |

---

## 模板结构

NumericUpDown 的 ControlTemplate 委托给 `ButtonSpinner` 实现核心布局：

```
ButtonSpinner (PART_Spinner)                          ← 步进按钮容器（含 AddOn 装饰框）
  ├── [LeftAddOn]                                     ← 外部左侧标签（可选）
  ├── AddOnDecoratedBox (内部主体区域)
  │     ├── [InnerLeftContent]                        ← 内部左侧内容（前缀图标/文字）
  │     ├── TextBox (PART_TextBox)                    ← 核心文本输入框
  │     ├── StackPanel (InnerRightContent)            ← 内部右侧内容组合
  │     │     ├── InputClearIconButton (PART_ClearButton)  ← 清除按钮（条件显示）
  │     │     └── ContentPresenter                    ← 自定义内部右侧内容
  │     └── ButtonSpinnerHandle                       ← 上下步进按钮区域
  │           ├── IconButton (IncreaseButton ▲)       ← 增加按钮
  │           └── IconButton (DecreaseButton ▼)       ← 减少按钮
  └── [RightAddOn]                                    ← 外部右侧标签（可选）
```

**布局设计要点：**
- **委托模式**：NumericUpDown 不直接定义复杂模板，而是将布局委托给 `ButtonSpinner`，后者包含了完整的 AddOn 装饰框和步进按钮基础设施
- **悬浮步进按钮**：通过 `IsButtonSpinnerFloatable="True"`，步进按钮在鼠标悬停时浮动显示，不占用固定宽度
- **清除按钮与右侧内容并列**：通过 `StackPanel` 将清除按钮和自定义右侧内容水平排列
- **占位文本**：通过 `PlaceholderText` 转发到内部 `TextBox` 的同名属性

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `NumericUpDownThemeConstants.SpinnerPart` | `"PART_Spinner"` | ButtonSpinner 步进容器 |
| `NumericUpDownThemeConstants.ClearButtonPart` | `"PART_ClearButton"` | 清除按钮 |
