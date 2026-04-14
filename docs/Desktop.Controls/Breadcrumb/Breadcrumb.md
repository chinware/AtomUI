# Breadcrumb 面包屑

## 概述

面包屑（Breadcrumb）是一种辅助导航控件，用于显示当前页面在网站层级结构中的位置，并提供返回上级页面的导航链接。它以水平链式排列的方式呈现页面路径，帮助用户在多层级信息架构中快速定位自身位置、理解页面之间的关系，并便捷地返回任意上级页面。

AtomUI 的 `Breadcrumb` 控件复刻了 [Ant Design 5.0 Breadcrumb](https://ant.design/components/breadcrumb-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的面包屑设计哲学

Ant Design 对面包屑的定位是：**「显示当前页面在系统层级结构中的位置，并能向上返回」**。面包屑导航的核心价值在于：

1. **位置感知** — 让用户清楚知道自己在整个应用信息架构中的位置
2. **快速回溯** — 提供一键返回任意上级页面的能力，避免频繁使用浏览器后退
3. **减少认知负荷** — 以简洁的文字链展示路径层级，不干扰主内容区

**面包屑的典型使用场景：**

| 场景 | 说明 |
|---|---|
| 🏠 **多级页面导航** | 当网站/应用层级 ≥ 2 层时，在页面顶部展示导航路径（如：首页 / 用户管理 / 用户列表） |
| 📂 **文件浏览器** | 展示当前目录的完整路径，支持点击任意层级快速跳转 |
| 🛒 **电商分类** | 展示商品所属的分类路径（如：电子产品 / 手机 / 智能手机） |

**设计建议：**
- 面包屑通常放在页面内容区顶部、页面标题之上
- 最后一项（当前页面）不需要可点击，显示为普通文本
- 分隔符默认使用 `/`，可根据设计风格自定义

### Avalonia ItemsControl 基础能力

AtomUI 的 `Breadcrumb` 继承自 Avalonia 框架的 `ItemsControl`。理解 `ItemsControl` 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia ItemsControl 的核心职责：**

`ItemsControl` 是 Avalonia 中所有集合类控件的基类，它管理一组子项（Items），并通过 `ItemsPanel` 控制子项的布局方式、通过 `ItemTemplate` 控制子项的呈现方式。其继承链为：

```
Control → TemplatedControl → ItemsControl
```

作为 `ItemsControl`，Breadcrumb 能够：
- 通过 `Items` 直接添加 `BreadcrumbItem` 子项（声明式用法）
- 通过 `ItemsSource` 绑定数据集合（数据驱动用法），配合 `ItemTemplate` 自定义呈现
- 自动管理子项容器的创建和回收（`CreateContainerForItemOverride` / `PrepareContainerForItemOverride`）

**Avalonia ItemsControl 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Items` | 子项集合，可直接在 AXAML 中添加 `BreadcrumbItem` 子控件 |
| `ItemsSource` | 数据源绑定，用于 MVVM 数据驱动模式 |
| `ItemTemplate` | 子项数据模板，控制数据如何呈现为 UI |
| `ItemCount` | 只读属性，返回当前子项数量 |

### AtomUI 的扩展设计

AtomUI `Breadcrumb` 在 Avalonia `ItemsControl` 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **自定义分隔符** | `Separator` / `SeparatorTemplate` 属性 | 对齐 Ant Design 的 `separator` 属性，支持文本或自定义控件作为分隔符 |
| **面包屑项独立分隔符** | `BreadcrumbItem.Separator` / `BreadcrumbItem.SeparatorTemplate` | 每个面包屑项可独立设置分隔符，提供更灵活的定制能力 |
| **最后一项自动识别** | `IsLast` 内部属性 + `:is-last` 伪类 | 自动识别最后一项并应用不同样式（不可点击、不同文字颜色） |
| **导航支持** | `NavigateUri` / `NavigateContext` 属性 | 面包屑项可携带导航信息，点击时自动触发导航 |
| **URI 导航** | `NavigateUri` + `TopLevel.Launcher` | 点击时自动通过系统启动器打开 URI（支持外部链接） |
| **导航事件** | `NavigateRequest` 事件 | 应用层可监听导航请求，实现自定义路由跳转逻辑 |
| **图标支持** | `BreadcrumbItem.Icon` 属性 | 每个面包屑项可配置前置图标，增强视觉辨识度 |
| **过渡动画** | `IsMotionEnabled` + `Transitions` | 悬浮时前景色、背景色平滑过渡 |
| **数据模型** | `IBreadcrumbItemData` / `BreadcrumbItemData` | 提供标准数据模型接口，支持 MVVM 数据驱动 |
| **Design Token** | `BreadcrumbToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 面包屑与面包屑项

Breadcrumb 控件由两个核心组件构成：

- **`Breadcrumb`** — 容器控件，继承自 `ItemsControl`，管理子项集合、全局分隔符配置和导航事件
- **`BreadcrumbItem`** — 子项控件，继承自 `Avalonia.Controls.Button`，代表路径中的每一层级

`BreadcrumbItem` 继承自 Button 意味着它天然支持点击交互、`:pointerover` / `:pressed` 等标准伪类以及可访问性支持。但并非所有面包屑项都需要可点击——当面包屑项未设置 `NavigateUri` 或 `NavigateContext` 时，它仅作为静态文本展示。

### 分隔符（Separator）

分隔符是面包屑项之间的视觉分割标记，默认为 `/`。

**全局分隔符**：通过 `Breadcrumb.Separator` 属性设置，所有面包屑项共享同一分隔符。

**独立分隔符**：通过 `BreadcrumbItem.Separator` 属性为单个面包屑项设置独立分隔符，优先级高于全局设置。

**自定义分隔符模板**：通过 `SeparatorTemplate` 属性使用 `DataTemplate` 实现更复杂的分隔符样式（如图标分隔符）。

分隔符的可见性规则：最后一个面包屑项的分隔符自动隐藏（通过 `IsLast` 属性结合 `BoolConverters.Not` 转换器控制）。

### 导航机制

BreadcrumbItem 提供两种导航方式：

1. **URI 导航**（`NavigateUri`）— 设置后点击面包屑项会通过 `TopLevel.Launcher.LaunchUriAsync` 打开指定 URI，适合外部链接跳转
2. **上下文导航**（`NavigateContext`）— 设置后点击面包屑项会触发父级 `Breadcrumb` 的 `NavigateRequest` 事件，将 `BreadcrumbItem` 实例传递给事件处理器，适合应用内路由跳转

当 `NavigateUri` 或 `NavigateContext` 任一不为 null 时，面包屑项进入「可导航响应」状态（`IsNavigateResponsive = true`），鼠标光标变为手形，文字显示链接色，悬浮时出现背景色变化。

### 最后一项自动标记

`Breadcrumb` 在子项集合变化时（通过 `LogicalChildren.CollectionChanged`），自动遍历所有子项并标记最后一项的 `IsLast = true`。最后一项的视觉表现：
- 使用 `LastItemColor`（更深的文字颜色），而非普通项的 `ItemColor`
- 如果最后一项同时未设置导航（`IsNavigateResponsive = false`），则显示为不可点击的静态文本

### 数据驱动模式

除了直接在 AXAML 中声明 `BreadcrumbItem`，Breadcrumb 还支持通过 `ItemsSource` 绑定数据集合。数据对象实现 `IBreadcrumbItemData` 接口或使用内置的 `BreadcrumbItemData` 类，Breadcrumb 在 `PrepareContainerForItemOverride` 中自动将数据映射到 `BreadcrumbItem` 的对应属性（Icon、Content、Separator、NavigateUri、NavigateContext 等）。

### 过渡动画（Transitions）

BreadcrumbItem 在 `OnLoaded` 时配置过渡动画，`OnUnloaded` 时清除。当 `IsMotionEnabled` 为 `true` 时，以下属性变化会有平滑过渡效果：
- `Foreground` — 文字颜色过渡
- `Background` — 背景色过渡

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本面包屑 | ✅ `<Breadcrumb items={...} />` | ✅ `<atom:Breadcrumb>` + `BreadcrumbItem` | ✅ 完全对齐 |
| 自定义分隔符 `separator` | ✅ 全局分隔符属性 | ✅ `Separator` / `SeparatorTemplate` | ✅ 完全对齐 |
| 面包屑项独立分隔符 | ❌ 不支持 | ✅ `BreadcrumbItem.Separator` | ✅ 超越 Ant Design |
| 图标 | ✅ `items[].icon` | ✅ `BreadcrumbItem.Icon` (PathIcon) | ✅ 完全对齐 |
| 导航 `href` | ✅ `items[].href` | ✅ `NavigateUri` (Uri) | ✅ 完全对齐 |
| 点击事件 | ✅ `items[].onClick` | ✅ `NavigateContext` + `NavigateRequest` 事件 | ✅ 完全对齐 |
| 数据驱动 `items` | ✅ 数组数据 | ✅ `ItemsSource` + `IBreadcrumbItemData` | ✅ 完全对齐 |
| 下拉菜单 `items[].menu` | ✅ 面包屑项下拉选择 | ❌ 暂未支持 | ⚠️ 待支持 |
| `itemRender` | ✅ 自定义渲染函数 | ✅ `ItemTemplate` | ✅ 完全对齐 |

---

## 继承关系

### Breadcrumb

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── AtomUI.Desktop.Controls.Breadcrumb
              └── implements IMotionAwareControl
```

### BreadcrumbItem

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button (ICommandSource, IClickableControl)
              └── AtomUI.Desktop.Controls.BreadcrumbItem
```

`BreadcrumbItem` 通过 `using AvaloniaButton = Avalonia.Controls.Button;` 别名引用 Avalonia 原生 Button，避免类名冲突。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 子项集合管理、`Items` / `ItemsSource` 数据绑定、`ItemTemplate` 模板化、容器创建/回收 |
| `Breadcrumb` | Ant Design 面包屑语义（全局分隔符、导航事件、自动标记最后一项）、Design Token 集成、过渡动画控制 |
| `Avalonia.Controls.Button` | 指针交互 → Click 事件、`:pointerover` / `:pressed` 伪类、`Content` / `ContentTemplate`、无障碍支持 |
| `BreadcrumbItem` | 面包屑项语义（图标、分隔符、导航 URI/上下文、最后一项标记）、悬浮/导航态样式 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Core` | 支持 `IsMotionEnabled` 过渡动画开关 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Breadcrumb/Breadcrumb.cs` | Breadcrumb 容器控件 |
| 子项控件 | `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbItem.cs` | BreadcrumbItem 面包屑项控件 |
| 数据接口 | `src/AtomUI.Desktop.Controls/Breadcrumb/IBreadcrumbItemData.cs` | 面包屑项数据接口 |
| 数据模型 | `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbItemData.cs` | 面包屑项数据默认实现 |
| 导航事件 | `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbNavigateEventArgs.cs` | 导航请求事件参数 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbPseudoClass.cs` | 伪类定义 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Breadcrumb/Themes/BreadcrumbTheme.axaml` | Breadcrumb ControlTheme |
| 子项主题 | `src/AtomUI.Desktop.Controls/Breadcrumb/Themes/BreadcrumbItemTheme.axaml` | BreadcrumbItem ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Breadcrumb/Themes/BreadcrumbThemes.axaml` | ResourceDictionary 注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/BreadcrumbShowCase.axaml` | 使用范例 |

---

## 模板结构

### Breadcrumb 模板

Breadcrumb 的 ControlTemplate 非常简洁，仅包含一个水平排列的 `ItemsPresenter`：

```
ItemsPresenter
└── StackPanel (Orientation=Horizontal)  ← ItemsPanel，水平排列所有 BreadcrumbItem
    ├── BreadcrumbItem[0]
    ├── BreadcrumbItem[1]
    ├── ...
    └── BreadcrumbItem[n]
```

### BreadcrumbItem 模板

BreadcrumbItem 的 ControlTemplate 由两部分组成：

```
StackPanel#RootLayout (Orientation=Horizontal)
├── Border#ContentInfoFrame                        ← 内容框架（承载背景、内间距和圆角）
│   └── StackPanel (Orientation=Horizontal)        ← 内容布局
│       ├── IconPresenter#IconPresenter            ← 图标展示器（仅 Icon 不为 null 时可见）
│       └── ContentPresenter#Content               ← 文本/自定义内容
└── ContentPresenter#Separator                     ← 分隔符展示器（最后一项不可见）
```

**分层设计理由：**
- **内容框架独立**：`Border#ContentInfoFrame` 承载背景色和圆角，悬浮态背景变化仅作用于内容区域，不影响分隔符
- **分隔符独立**：分隔符位于内容框架之外，有独立的外间距（`SeparatorMargin`）和颜色（`SeparatorColor`），最后一项的分隔符通过 `IsLast` 绑定 `BoolConverters.Not` 自动隐藏
