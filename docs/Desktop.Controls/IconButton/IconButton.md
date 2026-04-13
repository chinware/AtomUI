# IconButton 图标按钮

## 概述

图标按钮（IconButton）是一种极简的按钮控件，它仅展示一个图标，不包含文本内容。IconButton 常用于工具栏、操作栏、表格行内操作等需要节省空间的场景，用户通过图标的含义即可理解按钮的功能。

AtomUI 的 `IconButton` 是 AtomUI 控件体系中的基础构件，广泛用于其他复合控件的内部（如 Input 的清除按钮、Tag 的关闭按钮、DatePicker 的日历图标按钮、Collapse 的展开/收起按钮等），是按钮家族中最轻量的成员。

---

## 设计原理

### Ant Design 的图标按钮设计

Ant Design 中没有独立的 "IconButton" 组件。图标按钮通常通过两种方式实现：一是 `Button` 组件设置 `shape="circle"` + `icon` 属性；二是组件内部直接渲染可点击的图标区域（如 Input 的后缀清除图标、Modal 的关闭按钮、Tabs 的关闭图标等）。

这种隐式模式导致了两个问题：每个需要图标按钮的组件都要重复实现相同的交互逻辑（悬浮变色、按下反馈、禁用态），且行为不一致。AtomUI 将这一模式抽象为独立的 `IconButton` 控件，使其可以在各种场景中复用，同时保持一致的交互行为。

### Avalonia Button 基础能力

AtomUI 的 `IconButton` 通过 `AbstractIconButton` 继承自 Avalonia 的 `Avalonia.Controls.Button`。理解 Avalonia Button 的基础能力有助于理解 IconButton 的交互基础。

**Avalonia Button 的核心职责：**

Avalonia 的 `Button` 是一个标准的 `ContentControl`，实现了 `ICommandSource` 和 `IClickableControl` 接口。其核心行为是：**响应指针操作，在指针按下时提供视觉按压反馈（`:pressed` 伪类），在指针释放时触发 `Click` 事件**。它的继承链为：

```
Control → TemplatedControl → ContentControl → Button
```

**Avalonia Button 提供的基础属性（IconButton 复用）：**

| 属性 | 说明 |
|---|---|
| `Command` / `CommandParameter` | MVVM 命令绑定，点击时调用 `ICommand.Execute`；`CanExecute` 返回 `false` 时自动禁用 |
| `ClickMode` | 点击触发时机：`Release`（默认）或 `Press` |
| `HotKey` | 键盘快捷键 |
| `IsPressed` | 只读，指示按钮当前是否处于按压状态 |
| `Flyout` | 附加弹出层，点击时自动展开 |
| `IsEnabled` | 是否启用 |

**Avalonia Button 提供的基础伪类（IconButton 继承）：**

| 伪类 | 说明 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按钮被按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |
| `:flyout-open` | 附加的 Flyout 处于打开状态 |

> **注意**：IconButton 并不使用 `Content` / `ContentPresenter`，而是通过专门的 `IconPresenter` 展示图标。`Content` 属性虽然存在（继承自 `ContentControl`），但在 IconButton 的模板中不被使用。

### AtomUI 的扩展设计

AtomUI `IconButton` 在 Avalonia Button 的基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **图标属性** | `Icon` 属性（`PathIcon?` 类型） | 专门的图标栏位，使用 Ant Design 图标集 |
| **图标颜色** | `IconBrush` 属性 + 主题伪类驱动 | 不同交互状态（正常/悬浮/按下/禁用）自动切换图标颜色 |
| **图标尺寸** | `IconWidth` / `IconHeight` 属性 | 精确控制图标渲染尺寸 |
| **加载动画** | `LoadingAnimation` + `LoadingAnimationDuration` | 支持图标旋转等加载动画效果 |
| **鼠标事件穿透** | `IsPassthroughMouseEvent` 属性 | 嵌入其他控件时不阻断事件冒泡 |
| **过渡动画** | `IMotionAwareControl` + `Background` / `RenderTransform` 过渡 | 平滑的背景色和变换过渡 |
| **Design Token** | `ButtonToken.ScopeProvider` | 复用 Button 的 Token 作用域 |

---

## 功能详解

### 极简模板

IconButton 的模板非常简洁：一个 `Border`（承载背景、边框、圆角）包裹一个 `IconPresenter`。没有 `ContentPresenter`、没有波纹层、没有阴影层——这是有意为之的轻量设计，因为 IconButton 的使用场景通常是在其他控件内部或工具栏中，过重的视觉效果会造成干扰。

### 交互状态颜色系统

IconButton 通过 `IconBrush` 属性在不同状态下切换图标颜色，主题通过伪类选择器自动驱动颜色变化：

| 状态 | IconBrush 来源 | Background |
|---|---|---|
| 正常 | `SharedToken.ColorIcon` | 透明 |
| 悬浮（`:pointerover`） | `SharedToken.ColorIconHover` | 透明 |
| 按下（`:pressed`） | `SharedToken.ColorText` | 透明 |
| 禁用（`:disabled`） | `SharedToken.ColorTextDisabled` | 透明 |

### 鼠标事件穿透

`IsPassthroughMouseEvent` 是 IconButton 特有且至关重要的设计。当 IconButton 嵌入其他控件内部时（如 Input 的清除按钮、Tag 的关闭按钮），需要解决一个常见问题：**嵌套的 Button 会拦截鼠标事件，导致父控件无法正常响应**。

设置 `IsPassthroughMouseEvent = true` 后：
- `OnPointerPressed` / `OnPointerReleased` / `OnPointerMoved` 均将 `e.Handled` 设为 `false`
- 鼠标事件继续冒泡到父控件
- IconButton 自身的 Click 事件仍然正常触发

### 过渡动画

当 `IsMotionEnabled = true` 时，`OnLoaded` 阶段自动配置以下过渡：
- `Background`：`SolidColorBrushTransition`（背景色平滑过渡）
- `RenderTransform`：`TransformOperationsTransition`（缩放/旋转等变换平滑过渡）

`OnUnloaded` 时清除过渡，避免不可见控件消耗资源。

---

## 与 Ant Design 对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 纯图标可点击区域 | 各组件内部隐式实现 | ✅ `IconButton` 独立控件 | ✅ 行为对齐，AtomUI 提供了更好的可复用抽象 |
| 悬浮/按下颜色变化 | ✅ | ✅ Token 驱动 | ✅ 完全对齐 |
| 禁用态 | ✅ 灰色调 | ✅ `ColorTextDisabled` | ✅ 完全对齐 |
| 加载动画 | ✅ 旋转 | ✅ `LoadingAnimation` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button (ICommandSource, IClickableControl)
              └── AtomUI.Controls.Commons.AbstractIconButton (IMotionAwareControl)  ← 设备无关基类
                    └── AtomUI.Desktop.Controls.IconButton                          ← 桌面实现
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.Button` | 点击交互 → Click 事件、`ICommand` 绑定、`IsPressed` 状态、`HotKey` 快捷键、`Flyout` 弹出 |
| `AbstractIconButton`（基类层） | `Icon` 属性、`IconBrush` 颜色、`IconWidth` / `IconHeight` 尺寸、`LoadingAnimation` 加载动画、`IsPassthroughMouseEvent` 事件穿透、`Background` + `RenderTransform` 过渡动画 |
| `IconButton`（桌面层） | 注册 `ButtonToken.ScopeProvider`，由主题控制视觉表现 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 过渡动画开关 |

---

## 作为内部构件的广泛使用

IconButton 在 AtomUI 控件体系中被大量复合控件用作内部构件。以下列出所有使用 IconButton 的控件及其具体场景：

| 宿主控件 | 使用场景 | 关键属性 | 模板文件 |
|---|---|---|---|
| **Tag** | 关闭按钮 | `IsPassthroughMouseEvent="True"` | `src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.axaml` |
| **Alert** | 关闭按钮 | — | `src/AtomUI.Desktop.Controls/Alert/Themes/AlertTheme.axaml` |
| **NotificationCard** | 关闭按钮 | — | `src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationCardTheme.axaml` |
| **TabItem** | 标签关闭按钮 | `IsPassthroughMouseEvent` | `src/AtomUI.Desktop.Controls/TabControl/Themes/BaseTabItemTheme.axaml` |
| **CardTabItem** | 卡片标签关闭按钮 | — | `src/AtomUI.Desktop.Controls/TabControl/Themes/CardTabItemTheme.axaml` |
| **TabControl** | 添加标签按钮 | — | `src/AtomUI.Desktop.Controls/TabControl/Themes/CardTabControlTheme.axaml` |
| **TabScrollViewer** | 滚动指示按钮 | — | `src/AtomUI.Desktop.Controls/TabControl/Themes/BaseTabScrollViewerTheme.axaml` |
| **OverflowMenuItem** | 溢出菜单关闭按钮 | — | `src/AtomUI.Desktop.Controls/TabControl/Themes/BaseOverflowMenuItemTheme.axaml` |
| **CalendarItem** | 前/后月、年导航按钮 | — | `src/AtomUI.Desktop.Controls/DatePicker/CalendarView/` |
| **Drawer** | 关闭按钮 | — | `src/AtomUI.Desktop.Controls/Drawer/Themes/` |
| **OverlayDialog** | 关闭/最大化按钮 | — | `src/AtomUI.Desktop.Controls/Dialog/Themes/` |
| **NumericUpDown** | 增/减步进按钮 | `IsPassthroughMouseEvent="False"` | `src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownTheme.axaml` |
| **Upload** | 删除/预览操作按钮 | — | `src/AtomUI.Desktop.Controls/Upload/Themes/` |
| **TextBox** | 清除按钮 | — | `src/AtomUI.Desktop.Controls/Input/Themes/` |

> 💡 **模式总结**：当 IconButton 嵌入父控件内部且需要父控件正常响应鼠标事件时，设置 `IsPassthroughMouseEvent="True"`；当 IconButton 需要独占点击（如 NumericUpDown 的步进按钮），保持默认 `false`。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Buttons/AbstractIconButton.cs` | 设备无关基类（Icon、颜色、尺寸、事件穿透、动画） |
| 控件类 | `src/AtomUI.Desktop.Controls/Buttons/IconButton.cs` | 桌面端实现（仅注册 Token 作用域） |
| Token 定义 | `src/AtomUI.Desktop.Controls/Buttons/ButtonToken.cs` | 复用 Button 的组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Buttons/Themes/IconButtonTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Buttons/Themes/IconButtonTheme.cs` | 主题 Code-behind |

---

## 模板结构

IconButton 的 ControlTemplate 采用极简两层结构：

```
Border#Frame (背景、边框、圆角、内间距)
  └── IconPresenter#IconPresenter (图标展示器)
        └── PathIcon (实际图标渲染)
```

**极简设计理由：**
- **无波纹层**：IconButton 主要作为内部构件使用，波纹效果由外层控件处理（如外层 Button 的 `WaveSpiritDecorator`）
- **无阴影层**：轻量级控件无需阴影增强立体感
- **无 ContentPresenter**：IconButton 仅展示图标，不需要通用内容容器

### 模板部件

| 部件名 | 控件类型 | 说明 |
|---|---|---|
| `Frame` | `Border` | 主框架，承载背景色（默认透明）、边框和圆角 |
| `IconPresenter` | `IconPresenter` | 图标展示器，通过 `IconBrush` 控制颜色，支持 `LoadingAnimation` |
