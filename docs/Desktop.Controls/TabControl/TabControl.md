# TabControl 标签页

## 概述

标签页（Tabs）用于在同一区域内切换显示不同的内容面板，是组织和导航大量关联内容的核心交互模式。用户通过点击标签页标题在不同面板之间切换，同一时刻仅展示一个面板的内容。

AtomUI 的标签页控件完整复刻了 [Ant Design 5.0 Tabs](https://ant.design/components/tabs-cn) 的设计规范，提供**线条式**（TabControl / TabStrip）和**卡片式**（CardTabControl / CardTabStrip）两种风格，并支持四方向布局、标签溢出滚动、可关闭标签、添加标签按钮等丰富功能。

> **控件家族说明**：AtomUI 的标签页体系包含两组并行的控件：
> - **TabControl** 系列 — 带内容面板，选中标签时自动切换显示对应内容（等同于 Ant Design 的 `Tabs`）。
> - **TabStrip** 系列 — 仅标签栏，不包含内容区域，适合自行管理内容展示的场景。

---

## 设计原理

### Ant Design 的标签页设计哲学

Ant Design 对标签页的定位是：**「提供平级的区域将大块内容进行收纳和展现，保持界面整洁」**。适用于：
- **卡片式标签页**：与卡片容器结合，视觉上强调容器边界
- **线条式标签页**：轻量风格，适合嵌入式导航
- **可编辑标签页**：支持新增/关闭标签页，适合多文档管理

### Avalonia 基础能力

AtomUI 的 `BaseTabControl` 继承自 Avalonia 的 `SelectingItemsControl`，获得项目容器化、选中项管理、键盘导航等基础设施。`BaseTabStrip` 继承自 Avalonia 的 `TabStrip`（即 `SelectingItemsControl`）。

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **两种风格** | `TabControl` / `CardTabControl` | 线条式与卡片式，对齐 Ant Design `type="line"` / `type="card"` |
| **四方向布局** | `TabStripPlacement` (Top/Right/Bottom/Left) | 对齐 Ant Design `tabPosition` |
| **三种尺寸** | `SizeType` (Large/Middle/Small) | 对齐 Ant Design `size` |
| **选中指示器动画** | 线条式下方/侧方滑块 + Transform 动画 | 复刻 Ant Design 的指示器滑动效果 |
| **标签溢出滚动** | 自定义 `TabControlScrollViewer` + 溢出菜单 | 标签过多时自动出现滚动按钮和更多菜单 |
| **可关闭标签** | `IsTabClosable` / `IsClosable` + `Closing` / `Closed` 事件 | 对齐 Ant Design 可编辑标签页 |
| **添加标签** | `IsShowAddTabButton` + `AddTabRequest` 事件（仅 Card 类型） | 对齐 Ant Design 可编辑标签页 |
| **头部额外内容** | `HeaderStartExtraContent` / `HeaderEndExtraContent` | 对齐 Ant Design `tabBarExtraContent` |
| **标签居中** | `TabAlignmentCenter` | 对齐 Ant Design `centered` |
| **图标支持** | `TabItem.Icon` 属性 | 对齐 Ant Design `tab.icon` |
| **Design Token** | `TabControlToken` | 所有视觉值从 Token 派生 |

---

## 控件家族

### TabControl 系列（带内容面板）

| 控件 | 风格 | 说明 |
|---|---|---|
| `TabControl` | 线条式 | 标签下方有选中指示器，最常用的标签页形式 |
| `CardTabControl` | 卡片式 | 标签呈卡片样式，可显示添加按钮 |
| `TabItem` | — | 标签项容器，包含标题、图标、关闭按钮、内容面板 |

### TabStrip 系列（仅标签栏）

| 控件 | 风格 | 说明 |
|---|---|---|
| `TabStrip` | 线条式 | 仅标签栏，无内容区域 |
| `CardTabStrip` | 卡片式 | 卡片式标签栏，可显示添加按钮 |
| `TabStripItem` | — | 标签栏项容器，包含文本、图标、关闭按钮 |

### 辅助类型

| 类型 | 说明 |
|---|---|
| `ITabItemData` | 数据绑定接口，包含 `Header`、`Icon`、`CloseIcon`、`IsEnabled`、`IsClosable`、`IsAutoHideCloseButton` |
| `TabItemData` | `ITabItemData` 的默认实现类，可直接用于 `ItemsSource` 绑定 |
| `TabSharp` | 枚举：`Line`（线条式）、`Card`（卡片式），内部使用 |
| `TabClosingEventArgs` | 标签关闭前事件参数，可设置 `Cancel = true` 阻止关闭 |
| `TabClosedEventArgs` | 标签关闭后事件参数 |

---

## 功能详解

### 线条式 vs 卡片式

- **线条式**（`TabControl` / `TabStrip`）：标签下方/侧方有主色调选中指示器（InkBar），指示器在标签切换时有滑动动画。
- **卡片式**（`CardTabControl` / `CardTabStrip`）：标签呈卡片形状，有背景色和边框，选中标签的边框与内容区域边框融合。支持添加按钮。

### 四方向布局

通过 `TabStripPlacement` 属性设置标签栏位置：
- `Top`（默认）— 标签在上方，内容在下方
- `Bottom` — 标签在下方，内容在上方
- `Left` — 标签在左侧，内容在右侧
- `Right` — 标签在右侧，内容在左侧

方向变化时，卡片式标签的圆角会自动调整（如 Top 时仅上方有圆角，Left 时仅左侧有圆角）。

### 标签溢出滚动

当标签数量超过可显示区域时：
- 自动出现左/右（或上/下）滚动指示器
- 提供溢出菜单（下拉按钮），可快速跳转到不可见的标签

### 可关闭标签

- 在 `BaseTabControl` / `BaseTabStrip` 上设置 `IsTabClosable="True"` 开启全局关闭按钮
- 在单个 `TabItem` / `TabStripItem` 上设置 `IsClosable="False"` 禁止关闭特定标签
- `IsTabAutoHideCloseButton="True"` 使关闭按钮仅在鼠标悬浮时显示
- 关闭时触发 `Closing` 事件（可取消）→ 实际关闭后触发 `Closed` 事件

### 头部额外内容

通过 `HeaderStartExtraContent` / `HeaderEndExtraContent` 在标签栏头尾添加自定义内容（如操作按钮）。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 线条式标签 `type="line"` | ✅ | ✅ `TabControl` / `TabStrip` | ✅ 完全对齐 |
| 卡片式标签 `type="card"` | ✅ | ✅ `CardTabControl` / `CardTabStrip` | ✅ 完全对齐 |
| 可编辑卡片 `type="editable-card"` | ✅ | ✅ `IsShowAddTabButton` + `IsTabClosable` | ✅ 完全对齐 |
| 四方向 `tabPosition` | ✅ | ✅ `TabStripPlacement` | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ | ✅ `SizeType` | ✅ 完全对齐 |
| 标签居中 `centered` | ✅ | ✅ `TabAlignmentCenter` | ✅ 完全对齐 |
| 标签图标 `tab.icon` | ✅ | ✅ `TabItem.Icon` | ✅ 完全对齐 |
| 额外内容 `tabBarExtraContent` | ✅ | ✅ `HeaderStartExtraContent` / `HeaderEndExtraContent` | ✅ 完全对齐 |
| 禁用标签 `disabled` | ✅ | ✅ `TabItem.IsEnabled` | ✅ 完全对齐 |
| 指示器动画 | ✅ | ✅ Transform + Transition | ✅ 完全对齐 |
| 标签溢出滚动 | ✅ | ✅ 自定义 ScrollViewer + 溢出菜单 | ✅ 完全对齐 |

---

## 继承关系

### TabControl 系列

```
Avalonia.Controls.SelectingItemsControl
  └── AtomUI.Desktop.Controls.BaseTabControl (IMotionAwareControl)     ← 共享基类
        ├── AtomUI.Desktop.Controls.TabControl                          ← 线条式（带指示器动画）
        └── AtomUI.Desktop.Controls.CardTabControl                     ← 卡片式（带添加按钮）
```

### TabStrip 系列

```
Avalonia.Controls.Primitives.TabStrip (SelectingItemsControl)
  └── AtomUI.Desktop.Controls.BaseTabStrip (ISizeTypeAware, IMotionAwareControl)  ← 共享基类
        ├── AtomUI.Desktop.Controls.TabStrip                                        ← 线条式
        └── AtomUI.Desktop.Controls.CardTabStrip                                   ← 卡片式
```

### TabItem / TabStripItem

```
Avalonia.Controls.HeaderedContentControl
  └── AtomUI.Desktop.Controls.TabItem (ISelectable)     ← TabControl 用标签项

Avalonia.Controls.Primitives.TabStripItem (ContentControl)
  └── AtomUI.Desktop.Controls.TabStripItem              ← TabStrip 用标签项
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `SelectingItemsControl` | 选中项管理、容器化、键盘导航、`SelectedIndex` / `SelectedItem` |
| `BaseTabControl` | 标签位置、尺寸、头部额外内容、可关闭标签、内容面板管理、边框分割线渲染 |
| `TabControl` | 线条式选中指示器 + 滑动动画 |
| `CardTabControl` | 卡片样式圆角管理 + 添加按钮 |
| `BaseTabStrip` | 同 BaseTabControl，但无内容面板 |
| `TabStrip` / `CardTabStrip` | 同 TabControl / CardTabControl，但无内容面板 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 共享基类 | `src/AtomUI.Desktop.Controls/TabControl/BaseTabControl.cs` | TabControl 系列共享基类 |
| 线条式 | `src/AtomUI.Desktop.Controls/TabControl/TabControl.cs` | 线条式 TabControl |
| 卡片式 | `src/AtomUI.Desktop.Controls/TabControl/CardTabControl.cs` | 卡片式 CardTabControl |
| 标签项 | `src/AtomUI.Desktop.Controls/TabControl/TabItem.cs` | TabItem 标签项 |
| 数据接口 | `src/AtomUI.Desktop.Controls/TabControl/TabItemData.cs` | ITabItemData / TabItemData |
| 伪类常量 | `src/AtomUI.Desktop.Controls/TabControl/TabPseudoClass.cs` | 位置伪类定义 |
| Token 定义 | `src/AtomUI.Desktop.Controls/TabControl/TabControlToken.cs` | 组件级 Design Token |
| TabStrip 基类 | `src/AtomUI.Desktop.Controls/TabControl/TabStrip/BaseTabStrip.cs` | TabStrip 系列共享基类 |
| 线条式 Strip | `src/AtomUI.Desktop.Controls/TabControl/TabStrip/TabStrip.cs` | 线条式 TabStrip |
| 卡片式 Strip | `src/AtomUI.Desktop.Controls/TabControl/TabStrip/CardTabStrip.cs` | 卡片式 CardTabStrip |
| Strip 标签项 | `src/AtomUI.Desktop.Controls/TabControl/TabStrip/TabStripItem.cs` | TabStripItem |
| 主题模板 | `src/AtomUI.Desktop.Controls/TabControl/Themes/` | ControlTheme 目录 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/TabControlShowCase.axaml` | 使用范例 |

---

## 模板结构

### TabControl / CardTabControl 模板（简化）

```
DockPanel
├── Panel#PART_AlignWrapper (DockPanel.Dock=根据 TabStripPlacement)
│   ├── ContentPresenter#HeaderStartExtraContent
│   ├── TabControlScrollViewer#PART_TabsContainer
│   │   └── ItemsPresenter#PART_ItemsPresenter
│   │       └── StackPanel (ItemsPanel)
│   │           └── TabItem... (自动生成)
│   ├── ContentPresenter#HeaderEndExtraContent
│   ├── Border#PART_SelectedItemIndicator (仅 TabControl)
│   └── IconButton#PART_AddTabButton (仅 CardTabControl)
└── ContentPresenter#PART_SelectedContentHost
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `TabControlThemeConstants.ItemsPresenterPart` | `"PART_ItemsPresenter"` | 标签项容器 |
| `TabControlThemeConstants.TabsContainerPart` | `"PART_TabsContainer"` | 滚动容器 |
| `TabControlThemeConstants.AlignWrapperPart` | `"PART_AlignWrapper"` | 标签栏对齐包装器 |
| `TabControlThemeConstants.SelectedItemIndicatorPart` | `"PART_SelectedItemIndicator"` | 选中指示器（线条式） |
| `TabControlThemeConstants.AddTabButtonPart` | `"PART_AddTabButton"` | 添加按钮（卡片式） |
| `TabItemThemeConstants.ItemCloseButtonPart` | `"PART_ItemCloseButton"` | 标签关闭按钮 |
