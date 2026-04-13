# MarqueeLabel 跑马灯

## 概述

跑马灯（MarqueeLabel）用于在有限空间内水平滚动展示文本内容。当文本宽度超出容器可用宽度时，控件自动启动从右向左的无缝循环滚动动画；当鼠标悬浮时动画暂停，移出后从暂停位置继续滚动。适用于公告栏、通知条、循环播报等需要在固定区域展示长文本的场景。

这是 AtomUI 的扩展控件，在 Ant Design 中没有直接对应组件。目前 AtomUI 内部的 `Alert` 组件使用 MarqueeLabel 实现消息循环滚动功能。

---

## 设计原理

### 跑马灯的设计目标

跑马灯解决的核心问题是：**在有限的水平空间内展示超出容器宽度的文本内容**。相比于截断（Trimming）或换行（Wrapping），跑马灯通过连续滚动让用户无需交互即可看到完整文本，同时保持单行显示不打破布局。

**关键交互设计：**

| 场景 | 行为 |
|---|---|
| 文本宽度 ≤ 容器宽度 | 静态显示，不启动动画 |
| 文本宽度 > 容器宽度 | 自动启动向左无缝循环滚动 |
| 鼠标悬浮（`:pointerover`） | 暂停动画，保持当前位置 |
| 鼠标移出 | 从暂停位置恢复滚动 |
| 容器/文本尺寸变化 | 自动重新计算动画参数 |

### Avalonia TextBlock 基础能力

AtomUI 的 `AbstractMarqueeLabel` 继承自 Avalonia 框架的 `Avalonia.Controls.TextBlock`。`TextBlock` 是 Avalonia 中最基础的文本渲染控件，它直接利用 `TextLayout` 引擎进行文字排版和绘制。

**TextBlock 的核心能力：**

| 能力 | 说明 |
|---|---|
| `Text` | 显示的文本内容 |
| `FontSize` / `FontFamily` / `FontWeight` / `FontStyle` | 字体控制 |
| `Foreground` | 文本前景色 |
| `TextWrapping` | 文本换行模式 |
| `TextTrimming` | 文本截断模式 |
| `TextAlignment` | 文本对齐方式 |
| `LineHeight` | 行高 |
| `Padding` | 内间距 |
| `TextLayout` | 底层文本排版引擎，提供文字宽度测量等能力 |

**TextBlock 支持的标准伪类：**

| 伪类 | 说明 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

MarqueeLabel 正是利用 `TextBlock` 的 `TextLayout` 能力来测量文本实际宽度，并通过重写 `RenderTextLayout` 方法实现双份文本的偏移绘制，从而达到无缝循环滚动的视觉效果。

### AtomUI 的扩展设计

AtomUI `MarqueeLabel` 在 TextBlock 的基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **自动滚动判断** | `MeasureOverride` 对比文本宽度与容器宽度 | 文本未超出时不做无意义滚动 |
| **无缝循环滚动** | `RenderTextLayout` 双份绘制 + `PivotOffset` 驱动 | 前一份文本滚出视口时，后一份无缝接续 |
| **可配置周期间隔** | `CycleSpace` 属性 | 控制两份文本之间的视觉间距 |
| **可配置滚动速度** | `MoveSpeed` 属性 | 控制滚动速度（像素/秒） |
| **悬浮暂停/恢复** | `IsPointerOver` 属性变化监听 | 鼠标悬浮时暂停，移出后从暂停处恢复 |
| **Design Token** | `MarqueeLabelToken` + `RegisterTokenResourceScope` | 默认速度和间隔从 Token 系统派生 |

---

## 功能详解

### 自动滚动判断

MarqueeLabel 在每次布局测量（`MeasureOverride`）时进行智能判断：

- **文本宽度 ≤ 可用宽度**：静态显示，清除动画，重置偏移为 0
- **文本宽度 > 可用宽度**：自动配置并启动滚动动画

当容器尺寸或文本内容发生变化导致溢出关系反转时，控件会自动切换状态。

### 无缝循环滚动

滚动效果的核心实现在 `RenderTextLayout` 方法中：

1. 通过 `PivotOffset`（内部动画属性）控制文本绘制的水平偏移量
2. 当 `PivotOffset < 0` 时（即文本开始左移），在第一份文本右侧间隔 `CycleSpace` 距离处绘制第二份相同文本
3. 动画将 `PivotOffset` 从当前位置匀速推进到 `-(textWidth + CycleSpace)`，然后无缝循环

动画使用 Avalonia 的 `Animation` API，设置 `IterationCount = long.MaxValue` 实现无限循环。动画时长根据文本宽度和 `MoveSpeed` 动态计算，确保不同长度的文本滚动体验一致。

### 悬浮暂停与恢复

当 `IsPointerOver` 变为 `true` 时：
1. 记录当前 `PivotOffset` 值作为恢复起点（`_pivotOffsetStartValue`）
2. 取消当前动画

当 `IsPointerOver` 恢复为 `false` 时：
1. 从记录的起点重新配置动画
2. 重新启动滚动

这种设计确保鼠标悬浮时用户可以仔细阅读当前可见内容，且恢复时不会有突然跳变。

### 动态参数调整

当 `CycleSpace` 或 `MoveSpeed` 在运行时发生变化时，控件会：
1. 停止当前动画
2. 根据新参数重新配置动画
3. 如果变化前动画正在运行，则自动重新启动

---

## 继承关系

```
Avalonia.Controls.Control
  └── Avalonia.Controls.TextBlock
        └── AtomUI.Controls.Commons.AbstractMarqueeLabel (行为、属性、动画逻辑)
              └── AtomUI.Desktop.Controls.MarqueeLabel (+ 桌面端 Token 注册)
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TextBlock` | 文本渲染、字体控制、`TextLayout` 排版引擎、`Text` 属性、`Foreground`、`Padding` 等基础文本展示能力 |
| `AbstractMarqueeLabel`（基类，设备无关） | 自动滚动判断、无缝循环滚动动画、悬浮暂停/恢复、`CycleSpace` / `MoveSpeed` 属性、`MeasureOverride` 布局逻辑、`RenderTextLayout` 双份绘制 |
| `MarqueeLabel`（桌面端具体实现） | 注册 `MarqueeLabelToken` Token 资源作用域、从 Token 系统获取 `CycleSpace` 和 `DefaultSpeed` 的默认值 |

### 跨平台分层说明

MarqueeLabel 采用 AtomUI 标准的两层继承模型：

| 层 | 类 | 项目 | 职责 |
|---|---|---|---|
| 基控件层 | `AbstractMarqueeLabel` | `AtomUI.Controls` | 设备无关的核心行为：滚动动画逻辑、布局测量、绘制、暂停/恢复 |
| 平台控件层 | `MarqueeLabel` | `AtomUI.Desktop.Controls` | 桌面端特化：Token 资源注册、从 Token 获取默认参数 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/MarqueeLabel/AbstractMarqueeLabel.cs` | 设备无关的抽象基类，包含核心滚动逻辑 |
| 控件类 | `src/AtomUI.Desktop.Controls/MarqueeLabel/MarqueeLabel.cs` | 桌面端具体实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/MarqueeLabel/MarqueeLabelToken.cs` | 组件级 Design Token |
| 内部使用示例 | `src/AtomUI.Desktop.Controls/Alert/Themes/AlertTheme.axaml` | Alert 组件中使用 MarqueeLabel 实现消息滚动 |
| Gallery 间接示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/AlertShowCase.axaml` | Alert 的 Loop Banner 示例中间接展示了 MarqueeLabel 效果 |

---

## 渲染原理

MarqueeLabel 的渲染采用「双份文本偏移绘制」策略：

```
容器可视区域 [====================]

初始状态（PivotOffset = 0）：
[Hello World This is ...]

滚动中（PivotOffset < 0）：
... is a long text ][  CycleSpace  ][ Hello World This ...
      第一份文本         间隔             第二份文本

循环点（PivotOffset = -(textWidth + CycleSpace)）：
等价于 PivotOffset = 0，动画无缝循环
```

**绘制关键代码逻辑：**

1. 第一份文本始终绘制在 `origin + PivotOffset` 位置
2. 当 `PivotOffset < 0` 时，第二份文本绘制在 `origin + PivotOffset + textWidth + CycleSpace` 位置
3. 两份文本的间距恰好等于 `CycleSpace`，形成无缝循环
