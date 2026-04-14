# ColorPicker 颜色选择器

## 概述

颜色选择器（ColorPicker）是一种用于选取颜色的交互控件，用户可以通过色谱面板、滑块、输入框等方式精确选择颜色值。AtomUI 提供两种颜色选择器变体：

- **ColorPicker**：纯色选择器，用于选取单一颜色值（`Color`），支持 HEX / HSVA / RGBA 格式。
- **GradientColorPicker**：渐变色选择器，用于选取线性渐变色（`LinearGradientBrush`），支持多色标（GradientStop）编辑。

AtomUI 的 ColorPicker 控件完整复刻了 [Ant Design 5.0 ColorPicker](https://ant.design/components/color-picker-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的颜色选择器设计哲学

Ant Design 5.0 引入 ColorPicker 组件，其设计定位是：**「提供一种直观、高效的颜色选取方式，同时保持与设计系统的一致性」**。核心设计理念包括：

| 设计原则 | 说明 |
|---|---|
| **所见即所得** | 触发器（Trigger）实时展示当前选中颜色，用户无需打开面板即可预览 |
| **渐进式交互** | 默认仅显示触发器，点击/悬浮展开完整面板，减少界面占用 |
| **多格式支持** | 支持 HEX / HSVA / RGBA 三种颜色格式，满足不同场景需求 |
| **透明度控制** | 可选启用 Alpha 通道，支持半透明色选取 |
| **预设色板** | 内置 Ant Design 标准色板，也支持自定义预设色 |
| **渐变色支持** | GradientColorPicker 支持线性渐变，可编辑多个色标 |

### Avalonia 基础能力

AtomUI 的 ColorPicker 继承自 Avalonia 框架的 `Avalonia.Controls.Button`。选择 Avalonia Button 作为基类的原因是：ColorPicker 的触发器本质上是一个可点击的按钮，点击后弹出 Flyout 面板展示颜色选择界面。

**Avalonia Button 提供的基础能力：**

- 指针交互 → Click 事件
- `ICommand` 命令绑定
- `IsPressed` / `IsEnabled` 状态管理
- `Flyout` 弹出机制
- 键盘焦点和无障碍支持

**AtomUI 在此基础上扩展了：**

- Flyout 内嵌颜色选择面板（ColorPickerView / GradientColorPickerView）
- 触发器色块 + 文本显示
- 多种触发方式（Click / Hover）
- 颜色格式切换
- Alpha 通道控制
- 预设色板
- 尺寸系统（Small / Middle / Large）
- 输入控件样式变体（Outlined / Filled / Borderless）
- 输入控件状态（Default / Warning / Error）
- 紧凑空间适配
- 表单集成

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **Flyout 颜色面板** | `AbstractColorPickerFlyout` + `ColorPickerView` / `GradientColorPickerView` | 点击/悬浮触发器时弹出完整颜色选择面板 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **颜色格式** | `ColorFormat` 枚举（Hex / Hsva / Rgba） | 对齐 Ant Design 的 `format` 属性 |
| **Alpha 通道** | `IsAlphaEnabled` 属性 | 控制是否允许选取半透明色 |
| **格式切换** | `IsFormatEnabled` 属性 | 控制面板中是否显示格式切换按钮 |
| **清除功能** | `IsClearEnabled` 属性 | 允许用户清除已选颜色 |
| **文本显示** | `IsShowText` 属性 | 在触发器上显示颜色文本值 |
| **预设色板** | `IsPaletteGroupEnabled` + `PaletteGroup` 属性 | 提供快速选色能力 |
| **触发方式** | `TriggerType` 属性（Click / Hover） | 对齐 Ant Design 的触发方式配置 |
| **值同步模式** | `ValueSyncStrategy` 属性（Immediate / OnCompleted） | 控制颜色值是实时同步还是关闭面板后同步 |
| **自定义文本** | `ColorTextFormatter` 附加属性 | 允许自定义触发器文本渲染 |
| **样式变体** | `StyleVariant` 属性（Outlined / Filled / Borderless） | 对齐 Input 控件的样式变体系统 |
| **输入状态** | `Status` 属性（Default / Warning / Error） | 支持表单验证状态显示 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整圆角和边框 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **过渡动画** | `IsMotionEnabled` + `Transitions` 动态配置 | 边框色平滑过渡 |
| **Design Token** | `ColorPickerToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 纯色选择器（ColorPicker）

`ColorPicker` 用于选取单一颜色值，核心属性为 `Value`（`Color?` 类型）和 `DefaultValue`。

**值同步模式：**

| 模式 | 说明 |
|---|---|
| `Immediate` | 拖动滑块时实时更新 `Value`，触发 `ValueChanged` 事件 |
| `OnCompleted` | 仅在关闭 Flyout 面板时更新 `Value`，触发 `ValueSelected` 事件 |

**颜色格式：**

| 格式 | 示例 | 说明 |
|---|---|---|
| `Hex` | `#1677ff` | 十六进制格式（默认） |
| `Hsva` | `hsva(217, 100%, 100%, 1.00)` | 色相-饱和度-明度-透明度 |
| `Rgba` | `rgba(22, 119, 255, 1.00)` | 红-绿-蓝-透明度 |

### 渐变色选择器（GradientColorPicker）

`GradientColorPicker` 用于选取线性渐变色，核心属性为 `Value`（`LinearGradientBrush?` 类型）和 `DefaultValue`。

渐变色选择器在面板中支持：
- 添加/删除色标（GradientStop）
- 拖动色标改变位置
- 选中色标后编辑其颜色
- 色标索引高亮显示

### 触发器（Trigger）

触发器是 ColorPicker 在未展开状态下的视觉表现，由色块（ColorBlock）和可选文本组成：

- **色块**：显示当前选中颜色，圆角矩形，带内阴影
- **文本**：当 `IsShowText=True` 时，在色块右侧显示颜色文本值
- **空状态**：当颜色被清除时，色块显示透明棋盘格背景

### 颜色面板（ColorPickerView）

颜色面板是 Flyout 中的核心内容，包含以下区域：

1. **色谱区域（ColorSpectrum）**：二维色谱，X 轴为饱和度，Y 轴为明度
2. **色相滑块（HueSlider）**：0°-360° 色相选择
3. **透明度滑块（AlphaSlider）**：当 `IsAlphaEnabled=True` 时显示
4. **输入区域（ColorPickerInput）**：颜色值输入框，支持格式切换
5. **预设色板（PaletteGroup）**：当 `IsPaletteGroupEnabled=True` 时显示

### 预设色板

AtomUI 内置了 Ant Design 标准色板，包含以下预设色组：

- 推荐色（Recommended）
- 最近使用色（Recently Used）

用户也可通过 `PaletteGroup` 属性自定义预设色组。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 纯色选择 | ✅ | ✅ `ColorPicker` | ✅ 完全对齐 |
| 渐变色选择 | ✅ `mode="gradient"` | ✅ `GradientColorPicker` | ✅ 完全对齐（独立控件而非 mode 切换） |
| 颜色格式 HEX/HSVA/RGB | ✅ `format` | ✅ `Format` 属性 | ✅ 完全对齐 |
| Alpha 通道 | ✅ `allowAlpha` | ✅ `IsAlphaEnabled` | ✅ 完全对齐 |
| 格式切换 | ✅ `allowFormat` | ✅ `IsFormatEnabled` | ✅ 完全对齐 |
| 清除颜色 | ✅ `allowClear` | ✅ `IsClearEnabled` | ✅ 完全对齐 |
| 预设色板 | ✅ `presets` | ✅ `IsPaletteGroupEnabled` + `PaletteGroup` | ✅ 完全对齐 |
| 触发器文本 | ✅ `showText` | ✅ `IsShowText` | ✅ 完全对齐 |
| 自定义文本渲染 | ✅ `showText` 函数 | ✅ `ColorTextFormatter` | ✅ 对齐（API 形式不同，语义一致） |
| 三种尺寸 | ✅ `size` | ✅ `SizeType` | ✅ 完全对齐 |
| 禁用状态 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 完全对齐 |
| 受控/非受控 | ✅ `value` / `defaultValue` | ✅ `Value` / `DefaultValue` | ✅ 完全对齐 |
| 值同步模式 | ✅ `changeOnClose` | ✅ `ValueSyncStrategy` | ✅ 完全对齐 |
| 触发方式 | ✅ `trigger` | ✅ `TriggerType` | ✅ 完全对齐 |
| 弹出位置 | ✅ `placement` | ✅ `Placement` | ✅ 完全对齐 |
| 禁用色相滑块 | ✅ `disableHue` | ❌ 暂未支持 | ⚠️ 待支持 |
| 禁用透明度滑块 | ✅ `disableAlpha` | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

### ColorPicker 继承链

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button (ICommandSource, IClickableControl)
              └── AbstractColorPicker (抽象基类)
                    ├── implements ISizeTypeAware
                    ├── implements IMotionAwareControl
                    ├── implements ICompactSpaceAware
                    ├── implements IFormItemAware
                    ├── implements IInputControlStatusAware
                    ├── implements IInputControlStyleVariantAware
                    ├── ColorPicker (纯色选择器)
                    └── GradientColorPicker (渐变色选择器)
```

### ColorPickerView 继承链

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AbstractColorPickerView (抽象基类)
        ├── implements IMotionAwareControl
        ├── ColorPickerView (纯色面板)
        └── GradientColorPickerView (渐变色面板)
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.Button` | 指针交互 → Click 事件、`ICommand` 绑定、`IsPressed` 状态、Flyout 弹出、无障碍支持 |
| `AbstractColorPicker` | Flyout 管理、触发器色块/文本渲染、颜色格式化、尺寸/样式变体/状态/紧凑空间/表单集成 |
| `ColorPicker` | 纯色值管理（`Value`/`DefaultValue`）、值同步模式、`ValueChanged`/`ValueSelected` 事件、`ColorTextFormatter` |
| `GradientColorPicker` | 渐变色值管理（`Value`/`DefaultValue` 为 `LinearGradientBrush`）、色标索引管理、`GradientValueChanged`/`ValueSelected` 事件 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持过渡动画开关 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持输入控件状态（Default / Warning / Error） |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持输入控件样式变体（Outlined / Filled / Borderless） |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 抽象基类 | `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs` | ColorPicker 抽象基类 |
| 纯色选择器 | `src/AtomUI.Desktop.Controls.ColorPicker/ColorPicker.cs` | ColorPicker 具体实现 |
| 渐变色选择器 | `src/AtomUI.Desktop.Controls.ColorPicker/GradientColorPicker.cs` | GradientColorPicker 具体实现 |
| 面板抽象基类 | `src/AtomUI.Desktop.Controls.ColorPicker/ColorView/AbstractColorPickerView.cs` | 颜色面板抽象基类 |
| 纯色面板 | `src/AtomUI.Desktop.Controls.ColorPicker/ColorView/ColorPickerView.cs` | ColorPickerView 实现 |
| 渐变色面板 | `src/AtomUI.Desktop.Controls.ColorPicker/ColorView/GradientColorPickerView.cs` | GradientColorPickerView 实现 |
| Flyout 基类 | `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPickerFlyout.cs` | Flyout 抽象基类 |
| 纯色 Flyout | `src/AtomUI.Desktop.Controls.ColorPicker/ColorPickerFlyout.cs` | ColorPickerFlyout |
| 渐变色 Flyout | `src/AtomUI.Desktop.Controls.ColorPicker/GradientColorPickerFlyout.cs` | GradientColorPickerFlyout |
| Token 定义 | `src/AtomUI.Desktop.Controls.ColorPicker/ColorPickerToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls.ColorPicker/Themes/AbstractColorPickerTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls.ColorPicker/Themes/AbstractColorPickerTheme.cs` | 主题 code-behind |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/ColorPickerShowCase.axaml` | 使用范例 |

---

## 模板结构

### ColorPicker 触发器模板

```
Border#Frame (主框架)
└── DockPanel (PART_RootLayout)
    ├── ColorBlock (PART_ColorIndicator)     ← 色块指示器（DockPanel.Dock="Left"）
    └── WrapPanel (PART_ColorTextPanel)      ← 颜色文本面板（填充剩余空间）
        └── TextBlock                        ← 颜色文本值
```

### ColorPickerView 面板模板

```
Border#Frame (主框架)
└── StackPanel (垂直布局)
    ├── Border#SpectrumBorder                ← 色谱区域边框
    │   └── ColorSpectrum                    ← 二维色谱控件
    ├── StackPanel#SliderContainer           ← 滑块容器
    │   ├── ColorPickerSlider                ← 色相滑块
    │   └── ColorPickerSlider                ← 透明度滑块（条件显示）
    ├── ColorPickerInput                     ← 颜色值输入区域
    │   ├── InputControl (Hex)               ← Hex 输入框
    │   ├── InputControl (H)                 ← 色相输入框
    │   ├── InputControl (S)                 ← 饱和度输入框
    │   ├── InputControl (V)                 ← 明度输入框
    │   └── InputControl (A)                 ← 透明度输入框（条件显示）
    └── ColorPickerPaletteGroup              ← 预设色板（条件显示）