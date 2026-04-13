# SplitButton 组合按钮

## 概述

组合按钮（SplitButton）是将一个主操作按钮和一个下拉菜单按钮合并在一起的复合控件。左侧是主按钮（Primary Button），点击执行主操作；右侧是下拉触发按钮（Secondary Button），点击或悬浮弹出菜单供用户选择更多操作。

SplitButton 对应 Ant Design 的 `Dropdown.Button`（带 `type` 属性的变体）。AtomUI 的 `SplitButton` 完整复刻了 [Ant Design 5.0 Dropdown.Button](https://ant.design/components/dropdown-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 Dropdown.Button 设计哲学

Ant Design 的 `Dropdown.Button` 呈现为左右两部分：左侧主按钮可触发 `onClick`，右侧小按钮弹出下拉菜单。这种设计解决了一个常见的交互问题：**用户既需要一个快速执行主操作的入口，又需要一个展开更多可选操作的途径**。典型场景包括：

| 场景 | 示例 |
|---|---|
| 主操作 + 更多操作 | 「提交」+ 下拉「保存草稿 / 导出 PDF / 打印」 |
| 默认选项 + 替代选项 | 「下载」+ 下拉「下载为 PDF / Word / Excel」 |
| 快捷操作 + 完整菜单 | 「回复」+ 下拉「回复全部 / 转发 / 标记」 |

**两种按钮类型**（通过 `type` 属性控制）：

| 类型 | 设计意图 | 典型用途 |
|---|---|---|
| ⚪️ **Default** | 中等视觉权重，白色背景 + 边框，适合并列操作场景 | 「更多操作」「选项」 |
| 🔵 **Primary** | 视觉权重最高，实心填充主色调，强调主操作 | 「提交」+ 下拉更多 |

**状态修饰**（可与类型自由组合）：

| 状态 | 设计意图 |
|---|---|
| ⚠️ **危险（Danger）** | 红色系警示用户该操作具有破坏性 |
| 🚫 **禁用（Disabled）** | 降低不透明度 + 灰色调，操作不可用 |

### Avalonia ContentControl 基础能力

AtomUI 的 `SplitButton` 继承自 Avalonia 框架的 `Avalonia.Controls.ContentControl`。理解 ContentControl 的基础能力有助于理解 SplitButton 在其之上做了哪些扩展。

**Avalonia ContentControl 的核心职责：**

`ContentControl` 是 Avalonia 中可容纳任意内容的基础模板控件。它的继承链为：

```
Control → TemplatedControl → ContentControl
```

作为 `ContentControl`，SplitButton 能容纳任意内容（文本、图标、甚至复杂布局），并通过 `ContentPresenter` 在模板中呈现。

**ContentControl 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Content` | 控件内容，可以是文本字符串，也可以是任意控件 |
| `ContentTemplate` | 内容模板，用于自定义内容的呈现方式 |
| `HorizontalContentAlignment` | 内容水平对齐方式 |
| `VerticalContentAlignment` | 内容垂直对齐方式 |

**ContentControl 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:pointerover` | 鼠标悬浮在控件上 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 控件获得焦点 |
| `:focus-visible` | 通过键盘（Tab）获得焦点 |

### 独立的控件设计

与 `DropdownButton`（继承自 `Button`）不同，`SplitButton` 直接继承自 `ContentControl`。这是因为它需要管理两个独立的内部 Button（`PrimaryButton` 和 `SecondaryButton`），它们有各自的交互状态和样式。SplitButton 自身承担以下职责：

- **Command 绑定**：实现 `ICommandSource` 接口，将 `Command`/`CommandParameter` 绑定到主按钮的点击行为
- **尺寸协调**：统一管理两个内部 Button 的 `SizeType`
- **圆角分配**：在 `ArrangeOverride` 中精确计算和分配左右按钮的圆角
- **Flyout 管理**：通过 `FlyoutStateHelper` 管理弹出层，`AnchorTarget` 设置为 `SecondaryButton`
- **键盘交互**：支持 Space/Enter 触发主按钮、Alt+Down/F4 打开 Flyout、Escape 关闭 Flyout

### AtomUI 的扩展设计

AtomUI `SplitButton` 在 Avalonia ContentControl 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **两种按钮类型** | `IsPrimaryButtonType` 布尔属性 → `EffectiveButtonType` 内部属性驱动内部 Button 样式 | 对齐 Ant Design 的 `type` 属性，支持 Primary / Default |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **危险状态** | `IsDanger` 属性透传至内部 Button | 红色系警示样式 |
| **图标支持** | `Icon` 属性（主按钮图标）+ `OpenIndicator` 属性（下拉按钮图标） | 灵活的图标配置 |
| **Flyout 弹出** | `FlyoutStateHelper` + `Flyout` 属性 + `TriggerType` | 完整的下拉菜单管理 |
| **弹出位置** | `Placement` / `PlacementAnchor` / `PlacementGravity` | 灵活的弹出方向控制 |
| **触发方式** | `TriggerType`（Click / Hover） | 支持点击和悬浮两种触发模式 |
| **点击波纹** | `IWaveSpiritAwareControl` + 内部 Button 各自的 `WaveSpiritDecorator` | 复刻 Ant Design 的 Wave 点击涟漪效果 |
| **过渡动画** | `IsMotionEnabled` 透传至内部 Button 和 Flyout | 背景色、前景色、边框色平滑过渡 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整圆角和边框 |
| **Command 绑定** | `ICommandSource` 接口 | 完整的 MVVM Command 绑定支持 |
| **键盘快捷键** | `HotKey` 属性 | 键盘快捷键触发主操作 |
| **Design Token** | 复用 `ButtonToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 内部按钮结构

SplitButton 的模板包含两个 AtomUI `Button` 实例：
- **PrimaryButton**（`PART_PrimaryButton`）：左侧主按钮，接收 `Content`、`ContentTemplate`、`Icon`，触发 `Click` 事件和 `Command`
- **SecondaryButton**（`PART_SecondaryButton`）：右侧下拉按钮，显示 `OpenIndicator` 图标（默认 `EllipsisOutlined`），负责触发 Flyout

两个按钮共享以下属性（通过 `TemplateBinding` 同步）：
- `SizeType`：尺寸保持一致
- `IsDanger`：危险样式同步
- `ButtonType`：通过 `EffectiveButtonType` 统一驱动
- `IsMotionEnabled` / `IsWaveSpiritEnabled`：动画行为同步
- `IsEnabled`：启用/禁用状态同步

### 主按钮类型（IsPrimaryButtonType）

通过 `IsPrimaryButtonType` 控制是否使用 Primary 样式：
- `true`：左右两个按钮都使用 `ButtonType.Primary`（蓝色背景），中间绘制白色分隔线
- `false`（默认）：左右两个按钮都使用 `ButtonType.Default`（白色背景 + 边框）

当 `IsPrimaryButtonType=True` 时，SplitButton 在 `Render` 方法中使用 `SplitSeparatorBrush` 绘制两个按钮之间的分隔线背景，确保视觉上无缝衔接。

### 圆角分配

SplitButton 通过 `ConfigureButtonCornerRadius` 方法智能地将 `CornerRadius` 分配给两个内部按钮：
- **PrimaryButton**：仅保留左侧圆角 `(TopLeft, 0, 0, BottomLeft)`
- **SecondaryButton**：仅保留右侧圆角 `(0, TopRight, BottomRight, 0)`
- 这确保了组合后的外观是一个整体的圆角矩形

当控件处于 CompactSpace 中时，会先通过 `CompactSpace.CalculateEffectiveCornerRadius` 计算有效圆角，再进行左右分配。

### 布局精排（ArrangeOverride）

SplitButton 在 `ArrangeOverride` 中对 SecondaryButton 进行微调布局：
- **Default 样式**（`IsPrimaryButtonType=false`）：SecondaryButton 向左偏移一个边框宽度，使两个按钮的边框重叠，视觉上呈现为一条分隔线而非双边框
- **Primary 样式**（`IsPrimaryButtonType=true`）：SecondaryButton 向右收缩一个边框宽度，消除 Primary 模式下的边框冗余

### Flyout 管理

SplitButton 使用 `FlyoutStateHelper` 管理弹出层的生命周期：

- **AnchorTarget** 设置为 `SecondaryButton`，确保 Flyout 从右侧按钮弹出
- 支持 `MenuFlyout` 类型，内含 `MenuItem` 菜单项
- 属性透传：`Placement`、`PlacementAnchor`、`PlacementGravity`、`IsShowArrow`、`IsPointAtCenter`、`GutterToFlyout`、`IsMotionEnabled` 等属性通过 `BindUtils.RelayBind` 同步到 Flyout 实例
- 当 Flyout 为 `MenuFlyout` 时，禁用其内置的鼠标点击检测（`IsDetectMouseClickEnabled = false`），由 SplitButton 自身管理关闭逻辑

### ZIndex 管理

模板中的两个内部 Button 通过 ZIndex 控制叠放顺序：
- 正常状态：`NormalZIndex = 1000`
- 悬浮状态：`ActivatedZIndex = 2000`

这确保当鼠标悬浮在某个按钮上时，该按钮的边框和阴影不会被相邻按钮遮挡。

### 紧凑空间（Compact Space）

SplitButton 实现了 `ICompactSpaceAware` 接口。当放置在 `Space.Compact` 容器中时：
- 容器通知 SplitButton 其 `CompactSpaceItemPosition`（First / Middle / Last / OnlyOne）和 `CompactSpaceOrientation`（Horizontal / Vertical）
- SplitButton 根据位置先计算自身的有效圆角，再分配给左右内部 Button
- 边框厚度通过 `GetBorderThicknessForCompactSpace` 方法返回，供容器计算相邻控件的边距

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 左右分离式设计 | ✅ `Dropdown.Button` | ✅ `SplitButton` | ✅ 完全对齐 |
| 主按钮 Primary/Default | ✅ `type` prop | ✅ `IsPrimaryButtonType` | ✅ 完全对齐 |
| 危险样式 | ✅ `danger` | ✅ `IsDanger` | ✅ 完全对齐 |
| 尺寸 | ✅ `size` | ✅ `SizeType` | ✅ 完全对齐 |
| 触发方式 | ✅ `trigger` | ✅ `TriggerType` | ✅ 完全对齐 |
| 弹出位置 | ✅ `placement` | ✅ `Placement` | ✅ 完全对齐 |
| 主按钮点击事件 | ✅ `onClick` | ✅ `Click` 事件 | ✅ 完全对齐 |
| 主按钮图标 | ✅ `icon` | ✅ `Icon` | ✅ 完全对齐 |
| 自定义下拉图标 | ✅ `buttonsRender` | ✅ `OpenIndicator` | ✅ 对齐（实现方式不同，效果一致） |
| 禁用 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 完全对齐 |
| 下拉箭头 | ✅ `arrow` | ✅ `IsShowArrow` | ✅ 完全对齐 |
| 悬浮延迟 | ✅ `mouseEnterDelay` / `mouseLeaveDelay` | ✅ `MouseEnterDelay` / `MouseLeaveDelay` | ✅ 完全对齐 |
| 点击波纹 | ✅ Wave 组件 | ✅ 内部 Button 自带 `WaveSpiritDecorator` | ✅ 完全对齐 |
| Command 绑定 | — | ✅ `ICommandSource` | ➕ AtomUI 增强（桌面端特有） |
| 键盘快捷键 | — | ✅ `HotKey` + 键盘导航 | ➕ AtomUI 增强（桌面端特有） |
| `href` / `target` | ✅ 链接跳转 | ❌ 不适用 | — 桌面端无需 HTML 超链接语义 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Desktop.Controls.SplitButton
              ├── implements ICommandSource
              ├── implements ISizeTypeAware
              ├── implements IWaveSpiritAwareControl
              └── implements ICompactSpaceAware
```

`SplitButton` 不继承自 `Button`，而是直接继承自 `ContentControl`，通过在模板中组合两个独立的 `Button` 实例来实现左右分离的交互模式。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现 |
| `AtomUI.Desktop.Controls.SplitButton` | Ant Design 视觉体系（两种类型/三种尺寸）、Design Token 集成（复用 ButtonToken）、内部双 Button 布局协调、Flyout 弹出管理、Command 绑定、圆角分配、ZIndex 管理、紧凑空间适配 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ICommandSource` | `Avalonia.Input` | 支持 `Command` / `CommandParameter` MVVM 命令绑定 |
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` | 控制内部 Button 的点击涟漪（Wave）动画效果 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Buttons/SplitButton.cs` | 组合按钮实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Buttons/ButtonToken.cs` | 组件级 Design Token（复用 Button 的 Token） |
| 主题模板 | `src/AtomUI.Desktop.Controls/Buttons/Themes/SplitButtonTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/Buttons/Themes/SplitButtonThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml` | 使用范例 |

---

## 模板结构

SplitButton 的 ControlTemplate 采用 DockPanel 水平布局，包含两个独立的 AtomUI Button：

```
DockPanel#PART_MainLayout (水平布局容器)
  ├── Button#PART_PrimaryButton (主按钮 — 左侧)
  │     ├── Content = {TemplateBinding Content}
  │     ├── ContentTemplate = {TemplateBinding ContentTemplate}
  │     ├── Icon = {TemplateBinding Icon}
  │     ├── SizeType = {TemplateBinding SizeType}
  │     ├── IsDanger = {TemplateBinding IsDanger}
  │     ├── ButtonType = {TemplateBinding EffectiveButtonType}
  │     ├── IsMotionEnabled = {TemplateBinding IsMotionEnabled}
  │     └── IsWaveSpiritEnabled = {TemplateBinding IsWaveSpiritEnabled}
  └── Button#PART_SecondaryButton (下拉按钮 — 右侧)
        ├── Icon = {TemplateBinding OpenIndicator}  ← 默认 EllipsisOutlined
        ├── SizeType = {TemplateBinding SizeType}
        ├── IsDanger = {TemplateBinding IsDanger}
        ├── ButtonType = {TemplateBinding EffectiveButtonType}
        ├── IsMotionEnabled = {TemplateBinding IsMotionEnabled}
        └── IsWaveSpiritEnabled = {TemplateBinding IsWaveSpiritEnabled}
```

**设计理由：**
- **内部使用完整 Button**：两个内部 Button 是完整的 AtomUI `Button` 实例，自带波纹效果、过渡动画、加载图标等全部 Button 能力，无需在 SplitButton 层重复实现。
- **DockPanel 布局**：使用 DockPanel 实现水平排列，PrimaryButton 占据主要空间，SecondaryButton 停靠在右侧。
- **ArrangeOverride 微调**：通过 `ArrangeOverride` 对 SecondaryButton 进行亚像素级的位置调整，实现边框重叠（Default 模式）或边框消除（Primary 模式）。
- **ZIndex 动态管理**：悬浮状态的按钮 ZIndex 提升至 2000，确保边框和阴影正确叠放。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `SplitButtonThemeConstants.MainLayoutPart` | `"PART_MainLayout"` | 根布局面板（DockPanel） |
| `SplitButtonThemeConstants.PrimaryButtonPart` | `"PART_PrimaryButton"` | 左侧主按钮 |
| `SplitButtonThemeConstants.SecondaryButtonPart` | `"PART_SecondaryButton"` | 右侧下拉按钮 |
| `SplitButtonThemeConstants.NormalZIndex` | `1000` | 正常状态 ZIndex |
| `SplitButtonThemeConstants.ActivatedZIndex` | `2000` | 悬浮/激活状态 ZIndex |
