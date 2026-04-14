# ToggleIconButton 切换图标按钮

## 概述

切换图标按钮（ToggleIconButton）是一个纯图标的切换按钮控件，在选中（Checked）和未选中（Unchecked）状态之间切换时可以显示不同的图标。它没有文本内容、无背景、无边框，仅通过图标和图标颜色来表达状态。

ToggleIconButton 常用于工具栏、面板头部的折叠/展开、收藏/取消收藏、排序方向切换等交互场景。在 Ant Design 中没有直接对应的组件，它是 AtomUI 基于 Avalonia `ToggleButton` 扩展的实用控件。

---

## 设计原理

### 设计定位

在 Ant Design 的组件体系中，切换类控件（Switch、Checkbox、Radio）都有较重的视觉表现和明确的表单语义。而在实际开发中，经常需要一种更轻量的切换方式——仅通过图标变化表达二元状态，不占据过多视觉空间。例如：
- **折叠/展开**：Collapse 面板的箭头图标在展开和收起时切换方向
- **排序切换**：表格列标题的排序图标在升序和降序之间切换
- **收藏/取消**：心形或星形图标在空心和实心之间切换

ToggleIconButton 就是为这类轻量切换场景设计的。

### Avalonia ToggleButton 基础能力

AtomUI 的 `ToggleIconButton` 通过 `AbstractToggleIconButton` 继承自 Avalonia 的 `Avalonia.Controls.Primitives.ToggleButton`。理解 Avalonia ToggleButton 的基础能力有助于理解 ToggleIconButton 的交互基础。

**Avalonia ToggleButton 的核心职责：**

Avalonia 的 `ToggleButton` 继承自 `Button`，在标准按钮的点击行为之上增加了 `IsChecked` 状态管理。每次点击时 `IsChecked` 在 `true` / `false`（以及可选的 `null`）之间切换。它的继承链为：

```
Control → TemplatedControl → ContentControl → Button → ToggleButton
```

**Avalonia ToggleButton 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `IsChecked` | 切换状态：`true`（选中）、`false`（未选中）、`null`（不确定，当 `IsThreeState=true` 时） |
| `IsThreeState` | 是否支持三态切换 |
| `Command` / `CommandParameter` | MVVM 命令绑定 |
| `ClickMode` | 点击触发时机 |
| `IsPressed` | 只读，是否处于按压状态 |

**Avalonia ToggleButton 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:checked` | `IsChecked == true` |
| `:unchecked` | `IsChecked == false` |
| `:indeterminate` | `IsChecked == null`（三态模式） |
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按下 |
| `:disabled` | 禁用 |

### AtomUI 的扩展设计

AtomUI `ToggleIconButton` 在 Avalonia ToggleButton 的基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **双图标切换** | `CheckedIcon` + `UnCheckedIcon` 属性 | 选中和未选中状态显示不同图标，通过模板中两个 `IconPresenter` 的可见性切换实现 |
| **精细颜色控制** | `NormalIconBrush` / `ActiveIconBrush` / `SelectedIconBrush` / `DisabledIconBrush` | 四种状态各有独立的颜色属性，主题根据伪类自动切换 |
| **当前生效颜色** | `IconBrush` 属性 | 由主题根据当前状态自动设置的生效颜色 |
| **图标尺寸** | `IconWidth` / `IconHeight` 属性 | 精确控制图标渲染尺寸 |
| **过渡动画** | `IMotionAwareControl` + `Background` / `RenderTransform` 过渡 | 平滑的背景色和变换过渡 |
| **Design Token** | `ButtonToken.ScopeProvider` | 复用 Button 的 Token 作用域 |

---

## 功能详解

### 双图标切换机制

ToggleIconButton 通过两个属性分别设置选中和未选中状态的图标：

| 属性 | 用途 | 可见性条件 |
|---|---|---|
| `CheckedIcon` | 选中状态（`IsChecked = true`）时显示的图标 | `:checked` 伪类激活时可见 |
| `UnCheckedIcon` | 未选中状态（`IsChecked = false`）时显示的图标 | `:unchecked` 伪类激活时可见 |

模板中包含两个 `IconPresenter`，通过 `IsChecked` 绑定控制它们的交替显示。

### 颜色状态系统

ToggleIconButton 提供了精细的颜色控制体系，支持四种状态各自独立的颜色定义：

| 属性 | 对应状态 | 说明 |
|---|---|---|
| `NormalIconBrush` | 未选中正常态 | 默认图标颜色 |
| `ActiveIconBrush` | 悬浮/按下态 | 鼠标悬浮或按下时的图标颜色 |
| `SelectedIconBrush` | 选中态 | `IsChecked = true` 时的图标颜色 |
| `DisabledIconBrush` | 禁用态 | `IsEnabled = false` 时的图标颜色 |
| `IconBrush` | 当前生效颜色 | 由主题根据当前状态自动赋值，实际渲染使用此属性 |

### 过渡动画

当 `IsMotionEnabled = true` 时，`OnLoaded` 阶段自动配置以下过渡：
- `Background`：`SolidColorBrushTransition`（背景色平滑过渡）
- `RenderTransform`：`TransformOperationsTransition`（缩放/旋转等变换平滑过渡）

`OnUnloaded` 时清除过渡，避免不可见控件消耗资源。

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button
              └── Avalonia.Controls.Primitives.ToggleButton
                    └── AtomUI.Controls.Commons.AbstractToggleIconButton (IMotionAwareControl)  ← 设备无关基类
                          └── AtomUI.Desktop.Controls.ToggleIconButton                          ← 桌面实现
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.Button` | 点击交互 → Click 事件、`ICommand` 绑定、`IsPressed` 状态 |
| `Avalonia.Controls.Primitives.ToggleButton` | `IsChecked` 状态管理、`:checked` / `:unchecked` 伪类、三态支持 |
| `AbstractToggleIconButton`（基类层） | `CheckedIcon` / `UnCheckedIcon` 双图标、四种颜色状态属性、`IconWidth` / `IconHeight`、过渡动画 |
| `ToggleIconButton`（桌面层） | 注册 `ButtonToken.ScopeProvider`，由主题控制视觉表现 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 过渡动画开关 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Buttons/AbstractToggleIconButton.cs` | 设备无关基类 |
| 控件类 | `src/AtomUI.Desktop.Controls/Buttons/ToggleIconButton.cs` | 桌面切换图标按钮 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Buttons/Themes/ToggleIconButtonTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Buttons/Themes/ToggleIconButtonTheme.cs` | 主题 Code-behind |
| 主题常量 | `src/AtomUI.Desktop.Controls/Buttons/Themes/ToggleIconButtonThemeConstants.cs` | 模板部件名常量 |

---

## 模板结构

```
Border#Frame (背景 + 边框 + 圆角)
  └── Panel#RootLayout (叠层容器)
        ├── IconPresenter#CheckedIconPresenter   (选中图标，IsChecked=True 时可见)
        └── IconPresenter#UnCheckedIconPresenter (未选中图标，IsChecked=False 时可见)
```

**模板设计理由：**
- **Panel 叠层容器**：两个 `IconPresenter` 叠加放置在同一位置，通过 `IsVisible` 绑定 `IsChecked` 实现交替显示。使用 `Panel` 而非 `Grid` 是因为只需简单叠层，无需行列定义。
- **两个独立 IconPresenter**：分开两个图标展示器而非运行时替换 `Icon` 属性，避免了图标切换时的测量/排列抖动。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `ToggleIconButtonThemeConstants.RootLayoutPart` | `"PART_RootLayout"` | 根布局面板 |
| `ToggleIconButtonThemeConstants.CheckedIconPresenterPart` | `"PART_CheckedIconPresenter"` | 选中态图标展示器 |
| `ToggleIconButtonThemeConstants.UnCheckedIconPresenterPart` | `"PART_UnCheckedIconPresenter"` | 未选中态图标展示器 |
