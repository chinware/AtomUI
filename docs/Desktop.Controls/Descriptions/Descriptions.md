# Descriptions 描述列表

## 概述

描述列表（Descriptions）用于成组展示多个只读字段，常见于详情页面的信息展示。支持水平和垂直两种布局方向、有边框和无边框两种样式、三种尺寸，以及响应式列数配置。

AtomUI 的 `Descriptions` 控件完整复刻了 [Ant Design 5.0 Descriptions](https://ant.design/components/descriptions-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验。

---

## 设计原理

### Ant Design 的描述列表设计哲学

Ant Design 对描述列表的定位是：**「成组展示多个只读字段，常见于详情页的头部信息展示」**。它不是一个用于编辑的表单控件，而是纯粹的信息展示组件，核心目标是以结构化、可扫描的方式呈现键值对数据。

**核心设计特性**：

| 特性 | 设计意图 |
|---|---|
| 📋 **键值对展示** | 每个子项由「标签」+「内容」组成，标签描述字段含义，内容展示具体值 |
| 📐 **水平/垂直布局** | 水平布局标签和内容在同一行；垂直布局标签在上、内容在下 |
| 🔲 **有边框/无边框** | 有边框模式使用表格风格展示，无边框模式更紧凑 |
| 📏 **三种尺寸** | Large / Middle / Small 控制子项内间距 |
| 📱 **响应式列数** | 根据窗口宽度断点自动调整列数 |
| 🔢 **跨列支持** | 单个子项可横跨多列，适配不同宽度的内容 |
| 📝 **标题与额外操作** | 支持 Header 标题和 Extra 额外操作区域 |

### Avalonia 基础能力

AtomUI 的 `Descriptions` 继承自 Avalonia 的 `TemplatedControl`，是一个完全自定义的模板化控件。它并不基于 Avalonia 内置的任何列表或表格控件，而是通过内部 `Grid` 布局实现多列排列。

**TemplatedControl 提供的核心能力：**

| 能力 | 说明 |
|---|---|
| 模板化 | 通过 `ControlTemplate` 定义视觉结构 |
| 样式系统 | 支持 `Style` 和 `ControlTheme` 覆盖 |
| 属性系统 | `StyledProperty` + `DirectProperty` 双属性系统 |

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **键值对子项** | `DescriptionItem` record 类 + `DescriptionItems` 集合 | 声明式描述标签/内容/跨列信息 |
| **两种布局** | `Layout` 属性（Horizontal / Vertical） | 对齐 Ant Design 的 `layout` prop |
| **边框模式** | `IsBordered` 属性 + 两套内部控件 | 有边框使用 `DescriptionBorderedCell`，无边框使用 `DescriptionDefaultItem` |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 属性 | 控制子项内间距大小 |
| **响应式列数** | `ColumnInfo` 属性 + `DescriptionsMediaBreakInfo` | 按窗口断点自动调整列数 |
| **跨列** | `DescriptionItem.Span` 属性 | 支持每个断点独立配置跨列数 |
| **占满行** | `DescriptionItem.IsFilled` 属性 | 强制子项占满当前行剩余列 |
| **标题/额外操作** | `Header` + `Extra` 属性 | 支持自定义标题和右侧操作区域 |
| **冒号控制** | `IsShowColon` 属性 | 控制标签后是否显示冒号 |
| **数据绑定** | `ItemsSource` 属性 | 支持从数据源动态生成子项 |
| **Design Token** | `DescriptionsToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 水平布局（默认）

默认布局方向为水平（`Orientation.Horizontal`），标签和内容在同一行并排展示。多个子项按列数自动换行排列。

### 垂直布局

当 `Layout="Vertical"` 时，标签在上方、内容在下方，每个子项占据一个垂直区域。适合标签或内容较长的场景。

### 边框模式

当 `IsBordered="True"` 时：
- **水平布局**：使用表格风格，标签列有背景色，内容列无背景，列之间有边框分隔
- **垂直布局**：每个子项用边框围绕，标签区域有背景色，内容区域白底

### 响应式列数

`ColumnInfo` 属性接受 `DescriptionsMediaBreakInfo` 类型，支持两种配置方式：

1. **固定列数**：`ColumnInfo="3"` — 所有断点都是 3 列
2. **按断点配置**：`ColumnInfo="xs: 1, sm: 2, md: 3, lg: 3, xl: 4, xxl: 4"` — 不同窗口宽度使用不同列数

支持的断点：`xs`（ExtraSmall）、`sm`（Small）、`md`（Medium）、`lg`（Large）、`xl`（ExtraLarge）、`xxl`（ExtraExtraLarge）。

### 跨列

每个 `DescriptionItem` 的 `Span` 属性同样接受 `DescriptionsMediaBreakInfo`，支持：
- 固定跨列：`Span="2"` — 所有断点跨 2 列
- 按断点跨列：`Span="xl: 2, xxl: 2"` — 仅在 xl 和 xxl 断点跨 2 列

### 占满行

当 `DescriptionItem.IsFilled="True"` 时，该子项强制占满当前行剩余的所有列。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基础描述列表 | ✅ | ✅ `Descriptions` 控件 | ✅ 完全对齐 |
| 水平/垂直布局 `layout` | ✅ | ✅ `Layout` 属性 | ✅ 完全对齐 |
| 边框模式 `bordered` | ✅ | ✅ `IsBordered` 属性 | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ | ✅ `SizeType` 属性（默认 Large） | ✅ 完全对齐 |
| 列数 `column` | ✅ 数字/响应式对象 | ✅ `ColumnInfo` 属性 | ✅ 完全对齐 |
| 跨列 `span` | ✅ | ✅ `DescriptionItem.Span` | ✅ 完全对齐 |
| 标题 `title` | ✅ | ✅ `Header` 属性 | ✅ 完全对齐 |
| 额外操作 `extra` | ✅ | ✅ `Extra` 属性 | ✅ 完全对齐 |
| 冒号 `colon` | ✅ | ✅ `IsShowColon` 属性 | ✅ 完全对齐 |
| 子项标签 `label` | ✅ | ✅ `DescriptionItem.Label` | ✅ 完全对齐 |
| 子项内容 `children` | ✅ | ✅ `DescriptionItem.Content` | ✅ 完全对齐 |
| 响应式断点 | ✅ `xs/sm/md/lg/xl/xxl` | ✅ `DescriptionsMediaBreakInfo` | ✅ 完全对齐 |
| 占满行 | ❌ | ✅ `DescriptionItem.IsFilled` | 🆕 AtomUI 扩展 |
| `contentStyle` / `labelStyle` | ✅ 内联样式 | ❌ 暂不支持 | ⚠️ 待支持 |
| `items` 数组配置 | ✅ | ✅ `ItemsSource` 属性 | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Descriptions
        └── implements ISizeTypeAware
```

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸 |

---

## 内部组件体系

Descriptions 的实现涉及多个内部组件协同工作：

| 组件 | 可见性 | 职责 |
|---|---|---|
| `Descriptions` | `public` | 主控件，管理子项集合、响应式列数、Grid 布局 |
| `DescriptionItem` | `public` | 数据记录类（record），描述标签/内容/跨列/占满行 |
| `DescriptionItems` | `public` | `AvaloniaList<DescriptionItem>` 的子项集合 |
| `DescriptionsMediaBreakInfo` | `public` | 响应式断点信息，支持字符串解析 |
| `DescriptionDefaultItem` | `internal` | 无边框模式的子项控件（水平+垂直布局） |
| `DescriptionBorderedCell` | `internal` | 有边框模式的基类单元格控件 |
| `DescriptionBorderedItemLabel` | `internal` | 有边框模式的标签单元格 |
| `DescriptionBorderedItemContent` | `internal` | 有边框模式的内容单元格 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Descriptions/Descriptions.cs` | 主控件 |
| 数据模型 | `src/AtomUI.Desktop.Controls/Descriptions/DescriptionItem.cs` | 子项 record 类 + 集合类 |
| 数据模型 | `src/AtomUI.Desktop.Controls/Descriptions/DescriptionsMediaBreakInfo.cs` | 响应式断点信息 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Descriptions/DescriptionDefaultItem.cs` | 无边框模式子项 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedCell.cs` | 有边框单元格基类 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemLabel.cs` | 有边框标签单元格 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemContent.cs` | 有边框内容单元格 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Descriptions/DescriptionsToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionsTheme.axaml` | 主控件主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionDefaultItemTheme.axaml` | 无边框子项主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionBorderedItemLabelTheme.axaml` | 有边框标签主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionBorderedItemContentTheme.axaml` | 有边框内容主题 |
| 主题常量 | `src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionsThemeConstants.cs` | 模板部件名称常量 |
| 主题注册 | `src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionsThemes.axaml` | 主题资源注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DescriptionsShowCase.axaml` | 使用范例 |

---

## 模板结构

### Descriptions 主控件模板

```
StackPanel (Vertical)
├── DockPanel#HeaderLayout (标题栏，Header 或 Extra 存在时可见)
│   ├── ContentPresenter#ExtraPresenter (右侧额外操作区域，DockPanel.Dock=Right)
│   └── ContentPresenter#HeaderPresenter (标题文本)
└── Border#ContentFrame (内容区域框架，IsBordered=True 时有边框和圆角)
    └── Grid#PART_GridLayout (子项网格布局)
        ├── DescriptionDefaultItem / DescriptionBorderedItemLabel + DescriptionBorderedItemContent
        └── ... (根据 Items 动态生成)
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `DescriptionsThemeConstants.GridLayoutPart` | `"PART_GridLayout"` | 子项网格布局容器 |
