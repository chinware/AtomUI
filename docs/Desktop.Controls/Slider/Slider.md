# Slider 滑动条

## 概述

滑动条（Slider）用于在给定的数值范围内选择一个值或一段范围。支持水平/垂直方向、刻度标记（Marks）、刻度吸附、范围选择（双滑块）、自定义值格式化、值包含模式等。

AtomUI 的 `Slider` 控件完整实现了 [Ant Design 5.0 Slider](https://ant.design/components/slider-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的滑动条设计哲学

Ant Design 对滑动条的定位是：**「当用户需要在数值区间/自定义区间内进行选择时，可为连续或离散值」**。滑动条相比纯数字输入框更加直观，用户可以通过拖拽快速感知值的相对位置。

**核心设计维度：**

| 维度 | 设计意图 | 典型用途 |
|---|---|---|
| **单值模式** | 在连续数值范围内选择一个值 | 音量控制、亮度调节、透明度设置 |
| **范围模式（Range）** | 通过双滑块选择一段范围 | 价格区间、时间段筛选 |
| **刻度标记（Marks）** | 在轨道上标注关键刻度点 | 温度选择、评分等级 |
| **刻度吸附（Step）** | 滑块自动吸附到最近刻度 | 离散值选择 |
| **值包含（Included）** | 控制轨道覆盖部分是否高亮 | 强调/弱化选中范围 |
| **方向** | 水平/垂直两个方向 | 适配不同布局场景 |
| **Tooltip** | 拖拽时显示当前值 | 精确反馈 |

### Avalonia RangeBase 基础能力

AtomUI 的 `Slider` 继承自 Avalonia 的 `RangeBase`（而非 Avalonia 内置的 `Slider`），直接基于 `RangeBase` 从头构建。`RangeBase` 提供：

| 属性 | 说明 |
|---|---|
| `Minimum` | 最小值 |
| `Maximum` | 最大值 |
| `Value` | 当前值 |
| `SmallChange` | 小步进值 |
| `LargeChange` | 大步进值 |

AtomUI 的 `Slider` 不继承 Avalonia 内置 `Slider`，避免了其有限的功能限制（如不支持范围模式、刻度标记等）。

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **范围模式** | `IsRangeMode` + `RangeValue` | 双滑块范围选择，对齐 Ant Design `range` |
| **方向** | `Orientation` | 水平/垂直两个方向 |
| **刻度标记** | `Marks` 属性（`List<SliderMark>`） | 自定义标签、样式的刻度标记 |
| **刻度吸附** | `IsSnapToTickEnabled` + `TickFrequency` | 滑块自动吸附到最近刻度 |
| **Tooltip 格式化** | `ValueFormatTemplate` | 自定义 Tooltip 显示格式 |
| **值包含** | `Included` | 控制轨道覆盖部分是否高亮 |
| **方向反转** | `IsDirectionReversed` | 反转滑动方向 |
| **波纹动画** | `IsWaveAnimationEnabled` | 滑块聚焦时的波纹效果 |
| **表单集成** | `IFormItemAware` | 可参与 FormItem 验证流程 |
| **无障碍** | `SliderAutomationPeer` | 符合 Avalonia 自动化标准 |
| **自绘滑块** | `SliderThumb.Render()` | 精确控制圆形滑块和 outline 环的绘制 |
| **Design Token** | `SliderToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 单值模式

默认模式。通过 `Value` 属性获取/设置当前值，轨道高亮从 `Minimum` 到 `Value` 的部分：

```xml
<atom:Slider Minimum="0" Maximum="100" Value="50" />
```

### 范围模式（IsRangeMode）

启用 `IsRangeMode` 后，出现双滑块，通过 `RangeValue` 控制范围。`RangeValue` 是一个 `SliderRangeValue` 记录结构体，包含 `StartValue` 和 `EndValue`：

```xml
<atom:Slider IsRangeMode="True" RangeValue="20, 80" Maximum="100" />
```

轨道高亮 `StartValue` 到 `EndValue` 之间的部分。拖拽时自动判断离哪个滑块更近。

### 刻度标记（Marks）

通过 `Marks` 属性定义关键刻度点。每个 `SliderMark` 包含标签文字和对应值，还可以自定义颜色、字体样式和字重：

```csharp
slider.Marks = new List<SliderMark>
{
    new("0°C", 0),
    new("26°C", 26),
    new("37°C", 37),
    new("100°C", 100) { LabelBrush = Brushes.Red, LabelFontWeight = FontWeight.Bold }
};
```

刻度标记在轨道上显示为小圆点，标签文字渲染在刻度下方（水平）或右侧（垂直）。点击标记可直接跳转到对应值。

### 刻度吸附（Snap to Tick）

设置 `TickFrequency` 定义刻度间隔，配合 `IsSnapToTickEnabled=True` 可让滑块只停留在刻度位置：

```xml
<atom:Slider TickFrequency="10" IsSnapToTickEnabled="True" Maximum="100" />
```

### 值包含（Included）

`Included` 属性控制轨道覆盖部分是否高亮：
- `true`（默认）：高亮 `Minimum` 到 `Value`（或 `StartValue` 到 `EndValue`）之间的轨道
- `false`：轨道不高亮，只显示刻度标记和滑块位置

### Tooltip 格式化

通过 `ValueFormatTemplate` 自定义拖拽时 Tooltip 的显示格式，使用 `string.Format` 语法：

```xml
<atom:Slider ValueFormatTemplate="\{0\}%" Value="50" Maximum="100" />
<!-- Tooltip 显示 "50%" -->
```

### 键盘导航

Slider 支持完整的键盘操作：`←`/`↓` 减少、`→`/`↑` 增加、`PageUp`/`PageDown` 大步进、`Home`/`End` 跳转极值。

### 自绘滑块（SliderThumb）

`SliderThumb` 通过重写 `Render` 方法直接绘制圆形滑块和 outline 环，而非使用模板内控件。这样可以精确控制：
- 滑块圆形大小（正常态 / 悬浮态）
- 边框粗细（正常态 / 悬浮态）
- Outline 环的颜色和厚度
- 平滑的过渡动画

### 轨道自绘（SliderTrack）

`SliderTrack` 负责整个轨道区域的渲染和布局，包括：
- 轨道底层（Rail）：灰色背景条
- 已覆盖部分（Track Bar）：主色高亮条
- 刻度标记点和标签文字
- 滑块位置计算和布局

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本滑动条 | ✅ `<Slider />` | ✅ `<atom:Slider />` | ✅ 完全对齐 |
| 范围模式 `range` | ✅ 布尔 | ✅ `IsRangeMode` + `RangeValue` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔 | ✅ `IsEnabled` | ✅ 完全对齐 |
| 垂直 `vertical` | ✅ 布尔 | ✅ `Orientation="Vertical"` | ✅ 完全对齐 |
| 刻度标记 `marks` | ✅ 对象 | ✅ `Marks` 属性 | ✅ 完全对齐 |
| 刻度吸附 `step` | ✅ 数字/null | ✅ `IsSnapToTickEnabled` + `TickFrequency` | ✅ 完全对齐 |
| 包含 `included` | ✅ 布尔 | ✅ `Included` | ✅ 完全对齐 |
| Tooltip `tooltip` | ✅ formatter | ✅ `ValueFormatTemplate` | ✅ 完全对齐 |
| `min/max` | ✅ 数字 | ✅ `Minimum/Maximum` | ✅ 完全对齐 |
| 反转方向 `reverse` | ✅ 布尔 | ✅ `IsDirectionReversed` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.RangeBase
  └── AtomUI.Desktop.Controls.Slider (IMotionAwareControl, IFormItemAware)
```

### 辅助类型

```
Avalonia.Controls.Primitives.TemplatedControl
  ├── AtomUI.Desktop.Controls.SliderTrack    ← 轨道控件（自绘轨道、刻度标记、滑块布局）
  └── AtomUI.Desktop.Controls.SliderThumb    ← 滑块手柄（自绘圆形 + outline 环，可拖拽）
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `RangeBase`（Avalonia） | `Minimum`/`Maximum`/`Value`/`SmallChange`/`LargeChange` 基础范围属性 |
| `Slider` | 范围模式、刻度吸附、Marks、Tooltip 格式化、Included、方向、键盘导航、表单集成、Token 注册 |
| `SliderTrack` | 轨道自绘渲染（Rail + Track Bar + Mark 点/标签）、滑块位置计算和布局、指针交互 |
| `SliderThumb` | 滑块手柄自绘（圆形 + 边框 + outline 环）、拖拽事件（DragStarted/DragDelta/DragCompleted）、过渡动画 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 控制过渡动画开关（`IsMotionEnabled`） |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 参与表单验证流程；单值模式表单值为 `double`，范围模式为 `SliderRangeValue` |

> 注意：Slider 全部定义在 `AtomUI.Desktop.Controls` 中，不存在 `AtomUI.Controls` 基类层的设备无关抽象。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Slider/Slider.cs` | Slider 主控件（759 行） |
| 轨道 | `src/AtomUI.Desktop.Controls/Slider/SliderTrack.cs` | 内部轨道控件（1092 行） |
| 滑块 | `src/AtomUI.Desktop.Controls/Slider/SliderThumb.cs` | 可拖拽手柄（255 行） |
| 无障碍 | `src/AtomUI.Desktop.Controls/Slider/SliderAutomationPeer.cs` | Slider Automation Peer |
| Token | `src/AtomUI.Desktop.Controls/Slider/SliderToken.cs` | 组件级 Design Token |
| 主题 | `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTheme.axaml` | Slider ControlTheme |
| 主题 | `src/AtomUI.Desktop.Controls/Slider/Themes/SliderThumbTheme.axaml` | SliderThumb ControlTheme |
| 主题 | `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTrackTheme.axaml` | SliderTrack ControlTheme |
| 主题常量 | `src/AtomUI.Desktop.Controls/Slider/Themes/SliderThemeConstants.cs` | 模板部件名称常量 |
| Gallery | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SliderShowCase.axaml` | 使用范例 |

---

## 模板结构

### Slider

Slider 的模板非常简洁，核心是一个 `SliderTrack`：

```
SliderTrack#PART_Track
├── Rail（轨道底层，灰色背景）          ← SliderTrack 自绘
├── Track Bar（已覆盖部分，主色高亮）    ← SliderTrack 自绘
├── Mark Dots（刻度标记圆点）           ← SliderTrack 自绘
├── Mark Labels（刻度标注文字）          ← SliderTrack 自绘
├── SliderThumb#PART_StartThumb          ← 起始滑块（单值模式 / 范围起点）
└── SliderThumb#PART_EndThumb            ← 结束滑块（仅范围模式可见）
```

### SliderThumb

SliderThumb 不使用模板，而是通过 `Render()` 直接绘制：
- 白色底圆 + 主色边框 = 滑块主体
- 主色半透明 outline 环 = 聚焦/悬浮效果

### 模板部件常量

| 常量 | 值 | 说明 |
|---|---|---|
| `SliderThemeConstants.TrackPart` | `"PART_Track"` | 轨道控件 |
| `SliderThemeConstants.StartThumbPart` | `"PART_StartThumb"` |  起始滑块 |
| `SliderThemeConstants.EndThumbPart` | `"PART_EndThumb"` | 结束滑块（范围模式） |

