# DropdownButton 下拉按钮

## 概述

下拉按钮（DropdownButton）是一个附带下拉菜单的按钮控件。点击或悬浮按钮后，会弹出一个菜单列表供用户选择操作。它将按钮的触发交互和菜单的选项展示结合在一起，是 Ant Design [Dropdown](https://ant.design/components/dropdown-cn) 组件的 AtomUI 实现。

DropdownButton 继承自 `Button`（AtomUI 的桌面端 Button 控件），因此完整具备 Button 的所有能力（按钮类型、尺寸、危险样式、图标、加载状态、波纹效果等），同时额外提供了弹出菜单的管理能力。

---

## 设计原理

### Ant Design 的下拉菜单设计哲学

Ant Design 中 `Dropdown` 是一个独立的行为组件，包裹在任意触发元素外层，为其附加下拉菜单弹出功能。`Dropdown.Button` 则是其带按钮外观的变体——一个将「按钮」与「下拉菜单」融合的复合控件，用户既能直接点击按钮执行主操作，又能通过下拉箭头访问更多操作。

核心设计思想：

| 设计维度 | 设计意图 |
|---|---|
| **操作聚合** | 将多个相关操作收纳到一个下拉菜单中，减少界面上的按钮数量 |
| **触发灵活** | 支持点击（Click）和悬浮（Hover）两种触发方式，适应不同场景 |
| **位置可控** | 弹出菜单支持多种方位（上下 × 左中右），自动适应可用空间 |
| **视觉一致** | 按钮部分与 Button 组件保持一致的视觉体系 |

### Avalonia Button 基础能力

DropdownButton 的继承链经过 AtomUI Button → Avalonia Button，因此天然具备 Avalonia Button 的全部基础能力：

- `Content` / `ContentTemplate` — 任意内容承载
- `Command` / `CommandParameter` — MVVM 命令绑定
- `HotKey` — 键盘快捷键
- `Click` 事件 — 按钮点击语义事件
- `IsEnabled` — 启用/禁用

同时也继承了 AtomUI Button 的全部扩展能力：五种按钮类型、三种形状、三种尺寸、危险/幽灵样式、加载状态、波纹效果、Design Token 集成等。

### AtomUI 的扩展设计

AtomUI 将 Ant Design 的 `Dropdown` 和 `Dropdown.Button` 统一为 `DropdownButton`：它本身就是一个 Button，同时内置了 Flyout 弹出管理机制。弹出的菜单通过 `DropdownFlyout` 属性（类型为 `MenuFlyout`）配置。

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **弹出菜单** | `DropdownFlyout` 属性（`MenuFlyout`） | 声明式配置下拉菜单内容 |
| **触发方式** | `TriggerType` 枚举（Click / Hover） | 对齐 Ant Design 的 `trigger` 属性 |
| **弹出位置** | `Placement` / `PlacementAnchor` / `PlacementGravity` | 精细控制弹出方位 |
| **箭头指示** | `IsShowArrow` / `IsPointAtCenter` | 弹出菜单上显示指向触发元素的箭头 |
| **延迟策略** | `MouseEnterDelay` / `MouseLeaveDelay` | 控制悬浮触发的进入/离开延迟 |
| **下拉指示器** | `OpenIndicator` / `IsShowOpenIndicator` | 按钮右侧的下拉箭头图标（默认 `DownOutlined`） |
| **菜单项事件** | `MenuItemClicked` 路由事件 | 统一监听弹出菜单中任意菜单项的点击 |

---

## 功能详解

### 弹出机制

DropdownButton 内部使用 `FlyoutStateHelper` 来管理弹出状态。`FlyoutStateHelper` 是 AtomUI 封装的 Flyout 生命周期管理器，负责：

1. **触发方式管理**：根据 `TriggerType` 决定是 Click 还是 Hover 触发弹出
2. **延迟控制**：`MouseEnterDelay` / `MouseLeaveDelay` 控制 Hover 模式下的弹出/关闭延迟
3. **生命周期**：在控件 Attach/Detach 到可视化树时自动管理弹出层的注册与销毁
4. **属性中继**：DropdownButton 上设置的 `Placement`、`IsShowArrow`、`IsPointAtCenter`、`MarginToAnchor`、`IsMotionEnabled` 等属性自动中继（Relay Bind）到 `MenuFlyout`

### 下拉指示器

DropdownButton 默认在按钮内容右侧显示一个下拉箭头图标（`DownOutlined`），初始化时自动设置。通过 `OpenIndicator` 属性可自定义图标，通过 `IsShowOpenIndicator` 可隐藏。

当按钮没有 Content、ContentTemplate、Icon 且不处于 Loading 状态时，内容区域自动隐藏，仅显示下拉指示器。这由内部的 `IsContentVisible` 属性智能控制。

### 菜单项点击事件

DropdownButton 提供了 `MenuItemClicked` 路由事件（`RoutingStrategies.Bubble`），当用户点击弹出菜单中的某个菜单项时触发。事件参数 `FlyoutMenuItemClickedEventArgs` 包含被点击的 `MenuItem` 引用，避免了手动为每个 `MenuItem` 单独注册 Click 事件。

### 弹出位置

支持 6 种标准弹出位置（对齐 Ant Design 的 `placement` 属性），默认为 `BottomEdgeAlignedLeft`：

| Placement 值 | 说明 | 对应 Ant Design |
|---|---|---|
| `BottomEdgeAlignedLeft` | 底部左对齐（默认） | `bottomLeft` |
| `Bottom` | 底部居中 | `bottom` |
| `BottomEdgeAlignedRight` | 底部右对齐 | `bottomRight` |
| `TopEdgeAlignedLeft` | 顶部左对齐 | `topLeft` |
| `Top` | 顶部居中 | `top` |
| `TopEdgeAlignedRight` | 顶部右对齐 | `topRight` |

### 点击隐藏策略

当 `TriggerType` 为 `Click` 时，点击弹出层外部即关闭菜单。当 `TriggerType` 为 `Hover` 时，仅当点击位置不在按钮区域内才关闭菜单。这确保了 Hover 模式下不会因误触按钮区域而关闭菜单。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 触发方式 `trigger` | ✅ `click` / `hover` / `contextMenu` | ✅ `TriggerType`（Click / Hover） | ⚠️ 不支持右键触发 |
| 菜单弹出位置 `placement` | ✅ 6 个方向 | ✅ `Placement`（6 个方向 + 更多） | ✅ 完全对齐 |
| 箭头指示 `arrow` | ✅ 布尔/对象 | ✅ `IsShowArrow` + `IsPointAtCenter` | ✅ 完全对齐 |
| 按钮类型 `type` | ✅ `type` prop | ✅ `ButtonType`（继承 Button） | ✅ 完全对齐 |
| 菜单项点击 `onMenuClick` | ✅ 回调函数 | ✅ `MenuItemClicked` 路由事件 | ✅ 完全对齐 |
| 悬浮延迟 | ✅ `mouseEnterDelay` / `mouseLeaveDelay` | ✅ `MouseEnterDelay` / `MouseLeaveDelay` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔 | ✅ `IsEnabled`（继承 Button） | ✅ 完全对齐 |
| 按钮尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType`（继承 Button） | ✅ 完全对齐 |
| 按钮危险样式 `danger` | ✅ 布尔 | ✅ `IsDanger`（继承 Button） | ✅ 完全对齐 |
| 加载状态 `loading` | ✅ 布尔 | ✅ `IsLoading`（继承 Button） | ✅ 完全对齐 |
| 受控弹出 `open` / `onOpenChange` | ✅ 受控模式 | ❌ 暂未提供 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button (ICommandSource, IClickableControl)
              └── AtomUI.Desktop.Controls.Button
                    ├── implements ISizeTypeAware
                    ├── implements IWaveSpiritAwareControl
                    ├── implements ICompactSpaceAware
                    └── implements IFormItemAware
                          └── AtomUI.Desktop.Controls.DropdownButton
                                └── + DropdownFlyout / TriggerType / Placement / 箭头 / 菜单项事件
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.Button` | 指针交互 → Click 事件、`ICommand` 绑定、`IsPressed` 状态、`HotKey` 快捷键、`Flyout` 弹出 |
| `AtomUI.Desktop.Controls.Button` | Ant Design 视觉体系（五种类型/三种形状/三种尺寸）、Design Token 集成、WaveSpirit 波纹、加载状态、危险/幽灵样式、紧凑空间适配、表单集成 |
| `AtomUI.Desktop.Controls.DropdownButton` | DropdownFlyout 管理、TriggerType、Placement、箭头指示、菜单项事件、下拉指示器 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Buttons/DropdownButton.cs` | 桌面端下拉按钮实现 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.cs` | 主题 code-behind |
| 模板常量 | `src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/DropdownButtonShowCase.axaml` | 使用范例 |

---

## 模板结构

DropdownButton 的模板继承自 Button 模板（通过 `ControlTheme.BasedOn`），为每种 `ButtonType`（Default / Primary / Link / Text）分别定义了独立的模板变体。核心差异在于：在 Button 模板的内容区域右侧增加了一个下拉指示器 `IconPresenter`，使用 `DockPanel` 布局使指示器固定在右侧。

```
Panel
├── WaveSpiritDecorator (PART_WaveSpirit)           ← 点击波纹效果层（仅 Default / Primary 类型）
├── Border#ShadowsFrame                              ← 阴影层（仅 Default 类型）
├── Border#Frame (主框架)                            ← 背景、边框、圆角
│   └── DockPanel (PART_RootLayout)                  ← 内容布局容器
│       ├── IconPresenter (PART_DropdownIndicator)   ← 下拉指示器（DockPanel.Dock="Right"）
│       └── StackPanel (PART_MainInfoLayout)         ← 内容区域（填充剩余空间）
│           ├── LoadingOutlined (PART_LoadingIcon)    ← 加载旋转图标
│           ├── IconPresenter (PART_ButtonIcon)       ← 按钮图标
│           └── ContentPresenter (PART_ContentPresenter) ← 文本/自定义内容
```

**模板变体差异：**

| ButtonType | 波纹效果 | 阴影层 | 说明 |
|---|---|---|---|
| `Default` | ✅ WaveSpiritDecorator | ✅ Border#ShadowsFrame | 白色背景 + 边框 + 阴影 |
| `Primary` | ✅ WaveSpiritDecorator | ❌ | 主色调填充 |
| `Link` | ❌ | ❌ | 无边框无背景 |
| `Text` | ❌ | ❌ | 无边框无背景 |

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `DropdownButtonThemeConstants.DropdownIndicatorPart` | `"PART_DropdownIndicator"` | 下拉指示器图标 |
| `ButtonThemeConstants.WaveSpiritPart` | `"PART_WaveSpirit"` | 波纹动画装饰器 |
| `ButtonThemeConstants.RootLayoutPart` | `"PART_RootLayout"` | 根布局面板 |
| `ButtonThemeConstants.MainInfoLayoutPart` | `"PART_MainInfoLayout"` | 主信息布局面板 |
| `ButtonThemeConstants.LoadingIconPart` | `"PART_LoadingIcon"` | 加载旋转图标 |
| `ButtonThemeConstants.ButtonIconPart` | `"PART_ButtonIcon"` | 按钮图标展示器 |
| `ButtonThemeConstants.ContentPresenterPart` | `"PART_ContentPresenter"` | 内容展示器 |

