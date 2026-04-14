# Tooltip 文字提示

## 概述

文字提示（ToolTip）是一种轻量级的浮层提示组件，在鼠标悬浮到目标元素上时显示提示信息。它是图形界面中最常见的辅助信息展示方式——无需用户主动操作，只要将鼠标移至目标区域即可获得上下文提示。常见用途包括：解释图标含义、补充说明按钮功能、展示被截断的完整文本等。

AtomUI 的 `ToolTip` 控件完整复刻了 [Ant Design 5.0 Tooltip](https://ant.design/components/tooltip-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

> **注意**：AtomUI 中该控件命名为 `ToolTip`，通过附加属性方式使用（`atom:ToolTip.Tip="xxx"`），可附加到任意控件上。

---

## 设计原理

### Ant Design 的 Tooltip 设计哲学

Ant Design 对 Tooltip 的定位是：**「简单的文字提示气泡框」**。它是最轻量的信息提示方式，相比 Popover（气泡卡片）和 Popconfirm（气泡确认框）更简洁、更专注。

Ant Design 的 Tooltip 设计原则：

| 原则 | 说明 |
|---|---|
| **轻量** | 仅承载简短文字，不建议放置复杂内容或交互控件 |
| **非干扰** | 悬浮触发、离开消失，不阻塞用户操作流程 |
| **辅助性** | 作为补充信息存在，不承载核心信息 |
| **上下文感知** | 通过箭头和定位明确关联目标元素 |

Ant Design Tooltip 的核心特性：
- **12 种弹出方向**：Top、Bottom、Left、Right 及各方向的对齐变体
- **箭头指示器**：明确指向目标元素，帮助用户建立视觉关联
- **预设颜色与自定义颜色**：支持使用调色板颜色或任意自定义颜色
- **延迟控制**：通过 `mouseEnterDelay` / `mouseLeaveDelay` 防抖，避免频繁弹出

### Avalonia ToolTip 基础能力

Avalonia 框架内置了基础的 ToolTip 支持（`Avalonia.Controls.ToolTip`），提供简单的文字提示功能。AtomUI 的 `ToolTip` 是完全独立的实现，**不继承** Avalonia 内置 ToolTip，而是继承自 `ContentControl` 并结合自定义的 `ToolTipService` 实现完整的提示逻辑。

**AtomUI ToolTip 与 Avalonia 内置 ToolTip 的对比：**

| 特性 | Avalonia 内置 ToolTip | AtomUI ToolTip |
|---|---|---|
| 箭头指示器 | ❌ 不支持 | ✅ 支持（`ArrowDecoratedBox`） |
| 弹出方向 | 基础方向 | 12 种精确方向 |
| 预设颜色 | ❌ 不支持 | ✅ Ant Design 14 色调色板 |
| 自定义颜色 | ❌ 不支持 | ✅ 任意 Color |
| 弹出/关闭动画 | ❌ 无 | ✅ Motion 动画系统 |
| Design Token | ❌ 无 | ✅ 完整 Token 集成 |
| 阴影效果 | ❌ 无 | ✅ Token 驱动的阴影 |
| 箭头居中指向 | ❌ 不支持 | ✅ `IsPointAtCenter` |

### AtomUI 的实现设计

AtomUI 的 ToolTip 通过**附加属性模式**工作——不需要将 ToolTip 作为控件放到可视树中，而是通过 `ToolTip.Tip` 附加属性将提示信息关联到任意目标控件上。当鼠标悬浮到目标控件时，`ToolTipService` 负责创建 ToolTip 实例并通过 `Popup` 弹出。

| 功能 | 实现方式 | 设计动机 |
|---|---|---|
| **附加属性模式** | `ToolTip.Tip` 附加属性 | 任何控件都可以添加 Tooltip，无需修改控件模板 |
| **12 种弹出方向** | `Placement` 附加属性 + `PopupUtils` | 全方位定位，自动计算锚点和重力方向 |
| **箭头指示器** | `ArrowDecoratedBox` 内部控件 | 通过几何绘制的三角箭头明确指向目标元素 |
| **箭头居中指向** | `IsPointAtCenter` + 偏移计算 | 箭头精确指向目标中心，而非气泡边缘 |
| **预设颜色** | `PresetColor` + `PresetPrimaryColor` 调色板 | 使用 Ant Design 调色板的 14 种标准颜色 |
| **自定义颜色** | `Color` 附加属性 | 任意背景色，满足品牌定制需求 |
| **延迟显示** | `ShowDelay` / `BetweenShowDelay` + `DispatcherTimer` | 防抖，避免鼠标快速滑过时频繁弹出 |
| **位置翻转** | `Popup.PositionFlipped` 事件处理 | 当空间不足时自动翻转方向，箭头随之调整 |
| **弹出/关闭动画** | `MotionAwareOpen` / `MotionAwareClose` | 平滑的淡入淡出动画，可通过 `IsMotionEnabled` 控制 |
| **Design Token** | `ToolTipToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 派生，支持主题切换 |

---

## 功能详解

### 附加属性使用模式

ToolTip 通过附加属性附加到任意控件上，核心附加属性包括：
- `atom:ToolTip.Tip`：设置提示内容（可以是字符串，也可以是任意控件）
- `atom:ToolTip.Placement`：设置弹出方向（默认 `Top`）
- `atom:ToolTip.IsShowArrow`：是否显示箭头（默认 `true`）

当 `Tip` 为字符串时，ToolTip 内部通过 `StringToTextBlockConverter` 自动将字符串转换为 `TextBlock`，并设置自动换行（`TextWrapping="Wrap"`）。

### 12 种弹出方向

通过 `Placement` 附加属性（`PlacementMode` 枚举）控制 ToolTip 的弹出位置：

```
          TL    Top    TR
           ┌──────────┐
      LT   │          │   RT
      Left │  Target  │   Right
      LB   │          │   RB
           └──────────┘
          BL   Bottom   BR
```

每种方向对应不同的锚点（`PlacementAnchor`）和重力（`PlacementGravity`）计算。当目标控件靠近屏幕边缘导致空间不足时，`Popup` 会自动翻转方向，ToolTip 通过 `PopupFlippedEventArgs` 事件同步调整箭头位置。

### 箭头控制

箭头由内部的 `ArrowDecoratedBox` 控件绘制，提供三种模式：

| 模式 | 属性设置 | 效果 |
|---|---|---|
| **显示箭头** | `IsShowArrow="True"`（默认） | 箭头指向目标控件方向的边缘 |
| **隐藏箭头** | `IsShowArrow="False"` | 不显示箭头，仅显示气泡框 |
| **箭头居中** | `IsShowArrow="True"` + `IsPointAtCenter="True"` | 箭头精确指向目标控件的中心点 |

> **注意**：在某些弹出方向组合下（如 `PlacementAnchor` 和 `PlacementGravity` 不兼容），箭头会被自动禁用。这通过 `PopupUtils.CanEnabledArrow()` 判断。

### 颜色系统

ToolTip 提供三层颜色控制，按优先级从高到低：

1. **预设颜色**（`PresetColor`）：使用 Ant Design 调色板的 14 种标准颜色（Blue、Red、Green、Orange、Gold、Yellow、Lime、Cyan、GeekBlue、Purple、Pink、Magenta、Volcano、Grey），颜色值由 `PresetPrimaryColor.GetColor()` 统一获取。
2. **自定义颜色**（`Color`）：使用任意 `Color` 值作为 ToolTip 背景色。
3. **默认颜色**：由 `ToolTipToken.ToolTipBackground` 控制（通常为深灰/黑色），文字为白色（`ToolTipToken.ToolTipColor`）。

设置 `PresetColor` 或 `Color` 后，背景色会覆盖默认 Token 值，文字色保持不变。

### ToolTipService（内部服务）

`ToolTipService` 是 ToolTip 系统的核心引擎，负责监听全局输入事件并管理 ToolTip 的显示/隐藏生命周期：

| 职责 | 实现方式 |
|---|---|
| 监听鼠标移动 | 订阅 `IInputManager.Process`，处理 `RawPointerEventArgs` |
| 命中测试 | 沿可视树向上遍历，找到设置了 `ToolTip.Tip` 的最近控件 |
| 延迟显示 | 根据 `ShowDelay` 启动 `DispatcherTimer` |
| 快速切换 | 根据 `BetweenShowDelay` 判断是否跳过延迟直接显示 |
| 自动关闭 | 鼠标离开目标控件/窗口时关闭，鼠标按下时关闭 |
| 自定义控制 | `IsCustomShowAndHide="True"` 时跳过自动关闭逻辑 |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 12 种方向 `placement` | ✅ | ✅ `PlacementMode` 枚举 | ✅ 完全对齐 |
| 箭头 `arrow` | ✅ `true / false / { pointAtCenter }` | ✅ `IsShowArrow` + `IsPointAtCenter` | ✅ 完全对齐 |
| 预设颜色 `color` | ✅ 预设名或自定义色 | ✅ `PresetColor` + `Color` | ✅ 完全对齐 |
| 延迟 `mouseEnterDelay` | ✅ 秒 | ✅ `ShowDelay` 毫秒 | ✅ 对齐（单位不同） |
| 连续延迟 `mouseLeaveDelay` | ✅ | ✅ `BetweenShowDelay` | ✅ 对齐 |
| 自定义内容 | ✅ ReactNode | ✅ `Tip` 接受任意 `object` | ✅ 对齐 |
| 受控模式 `open` | ✅ | ✅ `IsOpen` + `IsCustomShowAndHide` | ✅ 对齐 |
| 打开回调 `onOpenChange` | ✅ | ✅ `ToolTipOpening` / `ToolTipClosing` 事件 | ✅ 对齐 |
| `destroyTooltipOnHide` | ✅ | ❌ Popup 复用 | ⚠️ 无需，Avalonia 自动管理生命周期 |
| `getPopupContainer` | ✅ 指定挂载节点 | ❌ 不适用 | — 桌面端无 DOM 容器概念 |
| `overlayStyle` / `overlayClassName` | ✅ | ❌ 通过 Style 选择器实现 | ✅ Avalonia 方式等效 |
| `zIndex` | ✅ | ❌ 由 Popup 系统自动管理 | ✅ Avalonia 自动管理 |
| `fresh` | ✅ 5.10.0+ 强制重新渲染 | ❌ 暂未支持 | ⚠️ 待评估 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Desktop.Controls.ToolTip
              ├── implements IMotionAwareControl
              ├── implements IArrowAwareShadowMaskInfoProvider (internal)
              └── implements IPopupHostProvider
```

> **注意**：ToolTip 没有设备无关的基类（`AtomUI.Controls` 中无 `AbstractToolTip`），因为 ToolTip 的交互行为（鼠标悬浮触发）本身具有桌面端特性。未来移动端可能采用长按触发等不同交互方式，因此两端实现可能差异较大，不适合抽取共享基类。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现 |
| `AtomUI.Desktop.Controls.ToolTip` | Ant Design 视觉体系（箭头/方向/颜色）、附加属性接口、Design Token 集成、`ToolTipService` 交互服务、弹出动画、阴影效果 |

**实现的接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Core` | 支持 `IsMotionEnabled` 动画开关 |
| `IArrowAwareShadowMaskInfoProvider` | `AtomUI.Controls`（internal） | 为阴影渲染提供箭头感知的遮罩信息（圆角、边界、箭头位置） |
| `IPopupHostProvider` | `Avalonia.Controls` | 提供 Popup 宿主信息，用于弹出层管理 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Tooltip/Tooltip.cs` | ToolTip 控件及附加属性 |
| 服务类 | `src/AtomUI.Desktop.Controls/Tooltip/ToolTipService.cs` | ToolTip 交互服务（内部） |
| Token 定义 | `src/AtomUI.Desktop.Controls/Tooltip/ToolTipToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Tooltip/Themes/ToolTipTheme.axaml` | ControlTheme AXAML |
| 主题常量 | `src/AtomUI.Desktop.Controls/Tooltip/Themes/ToolTipThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml` | 使用范例 |

---

## 模板结构

ToolTip 的 ControlTemplate 结构简洁，以 `ArrowDecoratedBox` 为核心容器：

```
ArrowDecoratedBox (PART_ToolTipContainer)           ← 箭头装饰盒子（承载背景、圆角、箭头绘制、阴影）
└── ContentPresenter (PART_ToolTipContentPresenter)  ← 内容展示器（字符串自动转为 TextBlock）
```

**设计特点：**
- **ArrowDecoratedBox**：是 AtomUI 中专门用于箭头弹出组件的共享容器，在 ToolTip、Popover、Popconfirm 等组件中复用。它负责绘制三角箭头、管理内容区域圆角、计算箭头顶点位置。
- **StringToTextBlockConverter**：当 `Content` 为字符串时，自动转换为带自动换行的 `TextBlock`（`TextWrapping="Wrap"`），确保长文本正确显示。
- **背景色绑定**：`ArrowDecoratedBox.Background` 通过 `TemplateBinding` 绑定到 ToolTip 的 `Background` 属性，当设置 `PresetColor` 或 `Color` 时，ToolTip 会更新 `Background`，进而驱动 `ArrowDecoratedBox` 的背景和箭头颜色同步变化。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `ToolTipThemeConstants.ToolTipContainerPart` | `"PART_ToolTipContainer"` | 箭头装饰盒子容器 |
| `ToolTipThemeConstants.ToolTipContainerPresenterPart` | `"PART_ToolTipContentPresenter"` | 内容展示器 |
