# GroupBox 分组框

## 概述

分组框（GroupBox）用于将一组相关的控件组织在一起，并通过标题为其提供视觉上的分组标识。它在概念上类似于 HTML 的 `<fieldset>` 元素——一个带标题的围栏，将逻辑上相关的表单字段或内容归拢到一起。

这是 **AtomUI 的扩展控件**，在 Ant Design 5.0 中没有直接对应的组件。在设计语言上，它最接近 Ant Design 的 `Card` 组件（带标题的内容容器），但 GroupBox 采用了经典的「边框从标题中间穿过、标题区域遮挡边框」的 fieldset 式视觉效果，更适合表单分组和配置面板等场景。

---

## 设计原理

### 传统 GroupBox 的设计哲学

GroupBox 是桌面 GUI 的经典控件（WinForms、WPF、Qt 等均有内置），其核心设计意图是：

- **视觉分组**：通过边框围栏将相关控件聚合在一起，帮助用户快速识别功能区域
- **标题标注**：在边框顶部放置标题文本，为分组提供语义说明
- **内容容纳**：作为容器承载任意子控件

### Avalonia 基础能力

Avalonia 框架本身没有内置 GroupBox 控件。AtomUI 的 `GroupBox` 直接继承自 Avalonia 的 `ContentControl`，利用其内容容纳和模板化能力，在此基础上实现了 fieldset 式的分组视觉效果。

**Avalonia ContentControl 提供的基础能力：**

| 属性 | 说明 |
|---|---|
| `Content` | 容器内容，可以是任意控件或布局面板 |
| `ContentTemplate` | 内容数据模板 |
| `Background` | 背景画刷 |
| `BorderBrush` | 边框画刷 |
| `BorderThickness` | 边框厚度 |
| `CornerRadius` | 圆角半径 |
| `Padding` | 内容区域内间距 |
| `IsEnabled` | 是否启用 |

### AtomUI GroupBox 的扩展设计

AtomUI `GroupBox` 在 `ContentControl` 的基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **标题文本** | `HeaderTitle` 属性 | 为分组提供文字标注 |
| **标题位置** | `HeaderTitlePosition` 枚举（Left / Center / Right） | 支持三种标题对齐方式，适应不同布局风格 |
| **标题图标** | `HeaderIcon` 属性（`PathIcon` 类型） | 在标题文本前显示图标，增强语义辨识 |
| **标题字体定制** | `HeaderFontSize` / `HeaderFontStyle` / `HeaderFontWeight` | 精细控制标题的字体外观 |
| **标题颜色** | `HeaderTitleColor` 属性 | 自定义标题文本颜色 |
| **Fieldset 式边框** | 自定义 `Render` 方法 + `BorderRenderHelper` | 复刻经典 fieldset 效果：边框从标题垂直中心开始绘制，标题区域用背景色遮挡边框 |
| **Design Token** | `GroupBoxToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 标题位置（HeaderTitlePosition）

标题位置通过 `HeaderTitlePosition` 属性设置，支持三种对齐方式：

| 位置 | 视觉效果 | 典型用途 |
|---|---|---|
| `Left`（默认） | 标题左对齐 | 最常见的分组标题位置 |
| `Center` | 标题居中 | 对称布局、居中型面板 |
| `Right` | 标题右对齐 | 特殊布局需求 |

### 标题图标（HeaderIcon）

通过 `HeaderIcon` 属性在标题文本左侧添加图标。图标使用 Ant Design 图标集提供，显示在标题文本之前，图标与文本之间通过 `HeaderIconMargin` Token 控制间距。

### 标题字体定制

GroupBox 提供三个属性精细控制标题的字体外观：

- `HeaderFontSize`：标题字号（默认由 `SharedToken.FontSize` 控制）
- `HeaderFontStyle`：字体风格（Normal / Italic / Oblique）
- `HeaderFontWeight`：字重（Normal / Bold / Medium 等）
- `HeaderTitleColor`：标题颜色（默认由 `SharedToken.ColorText` 控制）

### Fieldset 式边框渲染

GroupBox 最核心的视觉特征是「边框从标题中间穿过」的效果。实现原理如下：

1. **布局阶段**（`ArrangeOverride`）：计算标题容器的垂直中心位置，以此作为边框的起始 Y 坐标
2. **渲染阶段**（`Render`）：
   - 首先通过 `BorderRenderHelper` 在偏移后的位置绘制边框和背景（边框区域从标题垂直中心开始到控件底部）
   - 然后在标题容器的精确位置绘制一个矩形遮挡区域，用背景色填充以「擦除」标题下方的边框线段

这种两阶段渲染创造了标题「嵌入」边框的经典 fieldset 视觉效果。

```
    ┌── Title Info ──────────────────────┐
    │                                    │  ← 边框从标题垂直中心开始
    │        Content of group box        │
    │                                    │
    └────────────────────────────────────┘
         ↑ 标题区域的边框被背景色遮挡
```

---

## 与 Ant Design 规范的对齐程度

GroupBox 是 AtomUI 的**扩展控件**，在 Ant Design 5.0 中没有直接对应的组件。

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 说明 |
|---|---|---|---|
| 分组容器 | `Card` 组件（最接近） | `GroupBox` | 设计理念类似，但 GroupBox 采用 fieldset 式边框 |
| Divider（带标题分割线） | `Divider` 组件 | — | GroupBox 的标题位置效果与 Divider 有些类似 |
| 标题位置 | `Card` 不支持标题对齐 | ✅ Left / Center / Right | AtomUI 扩展功能 |
| 标题图标 | `Card` 不内置图标栏位 | ✅ `HeaderIcon` 属性 | AtomUI 扩展功能 |
| 标题字体定制 | 通过 CSS 自定义 | ✅ 内置属性 | AtomUI 提供开箱即用的字体控制 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Desktop.Controls.GroupBox
```

GroupBox 没有在 `AtomUI.Controls` 中定义抽象基类，因为它是桌面端特有的扩展控件。如果未来需要移动端对应实现，可以抽取公共逻辑到 `AtomUI.Controls` 层。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现、`Background` / `BorderBrush` / `BorderThickness` / `CornerRadius` / `Padding` 等视觉属性 |
| `AtomUI.Desktop.Controls.GroupBox` | 标题文本/位置/图标/字体定制、Fieldset 式边框渲染（自定义 Render）、Design Token 集成 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/GroupBox/GroupBox.cs` | GroupBox 桌面端实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/GroupBox/GroupBoxToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/GroupBoxShowCase.axaml` | 使用范例 |

---

## 模板结构

GroupBox 的 ControlTemplate 结构如下：

```
Border#PART_Frame                           ← 主框架（承载布局，但实际边框由 Render 绘制）
└── DockPanel (LastChildFill=True)          ← 内容布局容器
    ├── Panel#PART_HeaderContainer           ← 标题区域容器（DockPanel.Dock="Top"）
    │   └── Decorator#PART_HeaderContent     ← 标题内容定位器（控制 HorizontalAlignment）
    │       └── StackPanel (Horizontal)      ← 图标 + 文本水平排列
    │           ├── IconPresenter#PART_HeaderIconPresenter  ← 标题图标
    │           └── TextBlock#PART_HeaderPresenter          ← 标题文本
    └── ContentPresenter#PART_ContentPresenter  ← 用户内容区域（填充剩余空间）
```

**设计要点：**
- 边框和背景不通过 `Border#PART_Frame` 的属性绘制，而是通过 `GroupBox.Render()` 自定义绘制，以实现标题遮挡边框的 fieldset 效果
- 标题位置通过 `Decorator#PART_HeaderContent` 的 `HorizontalAlignment` 控制（Left / Center / Right）
- 标题图标通过 `IsVisible` 绑定 `HeaderIcon` 属性是否为 null 来控制显隐

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `GroupBoxThemeConstants.FramePart` | `"PART_Frame"` | 主框架 Border |
| `GroupBoxThemeConstants.HeaderContainerPart` | `"PART_HeaderContainer"` | 标题区域容器 Panel |
| `GroupBoxThemeConstants.HeaderContentPart` | `"PART_HeaderContent"` | 标题内容定位器 Decorator |
| `GroupBoxThemeConstants.HeaderIconPresenterPart` | `"PART_HeaderIconPresenter"` | 标题图标展示器 |
| `GroupBoxThemeConstants.HeaderPresenterPart` | `"PART_HeaderPresenter"` | 标题文本 TextBlock |
| `GroupBoxThemeConstants.ContentPresenterPart` | `"PART_ContentPresenter"` | 内容展示器 |
