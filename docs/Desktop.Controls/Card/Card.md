# Card 卡片

## 概述

卡片（Card）是通用容器组件，用于承载标题、内容、操作和额外信息。支持多种内容类型（默认、Meta、Grid、Tabs）、两种风格变体（Outline、Borderless）、三种尺寸、内嵌模式、悬浮阴影等。

AtomUI 的 `Card` 控件完整复刻了 [Ant Design 5.0 Card](https://ant.design/components/card-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的卡片设计哲学

Ant Design 对卡片的定位是：**「通用容器，用于承载与当前场景相关的信息和操作。每个卡片通常只关注一个主题，以内容为主」**。为了帮助用户在不同场景中灵活组织内容，Ant Design 建立了一套完整的卡片能力体系：

**四种内容排布方式**：

| 内容类型 | 设计意图 | 典型用途 |
|---|---|---|
| 📋 **默认内容** | 最基本的容器，自由放置任意内容 | 信息展示、表单区域、说明文本 |
| 👤 **Meta 描述** | 头像 + 标题 + 描述的结构化展示 | 用户卡片、产品信息卡片 |
| 🔲 **Grid 网格** | 等分网格布局，每个格子都可交互 | 功能入口、操作面板 |
| 📑 **Tabs 标签页** | 内嵌标签页，同一卡片内切换内容 | 多维度信息展示 |

**两种风格变体**：

| 风格 | 设计意图 |
|---|---|
| ✅ **有边框（Outline）** | 默认风格，适用于白色或浅色背景。通过边框划定卡片边界 |
| 🔳 **无边框（Borderless）** | 移除边框，使用投影代替。适用于灰色或有色背景，视觉更轻量 |

**其他能力**：

| 能力 | 设计意图 |
|---|---|
| 🔄 **加载状态** | 内容加载中时显示骨架屏占位，避免内容闪烁 |
| 📦 **内嵌模式** | 卡片嵌套卡片，内层卡片头部使用 `ColorFillAlter` 背景区分层级 |
| 🖱️ **悬浮阴影** | 鼠标悬浮时卡片「升起」，暗示卡片可点击或可交互 |

### Avalonia HeaderedContentControl 基础能力

AtomUI 的 `Card` 继承自 Avalonia 框架的 `Avalonia.Controls.Primitives.HeaderedContentControl`。理解其基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia HeaderedContentControl 的核心职责：**

`HeaderedContentControl` 是 Avalonia 提供的带头部区域的内容控件。其继承链为：

```
Control → TemplatedControl → ContentControl → HeaderedContentControl
```

作为 `ContentControl` 的扩展，它在主内容区域之外增加了一个 `Header` 区域，可通过 `Header` / `HeaderTemplate` 控制头部内容。

**Avalonia HeaderedContentControl 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Content` | 卡片主体内容，可以是文本、控件或任意对象 |
| `ContentTemplate` | 内容数据模板 |
| `Header` | 头部/标题内容 |
| `HeaderTemplate` | 头部数据模板 |
| `Background` | 背景色 |
| `BorderBrush` | 边框画刷 |
| `BorderThickness` | 边框粗细 |
| `CornerRadius` | 圆角半径 |
| `Padding` | 内间距 |

### AtomUI 的扩展设计

AtomUI `Card` 在 Avalonia `HeaderedContentControl` 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **额外区域（Extra）** | `Extra` + `ExtraTemplate` 属性 | 标题行右侧附加操作入口（如 "More" 链接） |
| **封面图片** | `Cover` + `CoverTemplate` 属性 | 卡片顶部全宽封面图片/媒体 |
| **操作区** | `Actions` 集合（`CardActionButton`） | 卡片底部等分操作按钮区 |
| **风格变体** | `CardStyleVariant` 枚举（Outline / Borderless） | 有边框/无边框两种视觉风格 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` | Large / Middle / Small 影响头部高度、字号、内间距 |
| **悬浮阴影** | `IsHoverable` + `BoxShadow` 过渡动画 | 鼠标悬浮时投影加深，卡片「升起」 |
| **内嵌模式** | `IsInnerMode` 属性 | 嵌套卡片场景，内层卡片头部背景变色 |
| **加载状态** | `IsLoading` + 内置 `Skeleton` 控件 | 骨架屏加载占位，数据加载中时自动展示 |
| **多种内容类型** | `CardMetaContent` / `CardTabsContent` / `CardGridContent` | 自动检测内容类型并调整布局（边框、圆角等） |
| **过渡动画** | `IsMotionEnabled` + `BoxShadowsTransition` | `BoxShadow` 悬浮阴影平滑过渡 |
| **Design Token** | `CardToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 风格变体（StyleVariant）

卡片风格通过 `StyleVariant` 属性设置，默认为 `Outline`。

| 风格 | 视觉表现 | 适用场景 |
|---|---|---|
| `Outline` | 有边框（`ColorBorderSecondary`），无阴影 | 白色/浅色背景 |
| `Borderless` | 无边框，使用三级阴影（`BoxShadowsTertiary`） | 灰色/有色背景 |

当 `StyleVariant = Borderless` 时，`EffectiveBorderThickness` 设为 0，阴影使用 `SharedToken.BoxShadowsTertiary`。

### 标题与额外区

卡片标题通过 `Header` 属性设置，额外区通过 `Extra` 属性设置。当 `Header`、`HeaderTemplate`、`Extra`、`ExtraTemplate` 均为 `null` 时，`:headerless` 伪类被激活，整个头部区域隐藏。

头部区域使用 `DockPanel` 布局：`Extra` 在右侧（`DockPanel.Dock="Right"`），`Header` 填充剩余空间。

### 封面（Cover）

通过 `Cover` 和 `CoverTemplate` 属性设置卡片封面。封面位于头部下方、内容上方。

- 无头部时（`:headerless`），封面顶部使用卡片的圆角（`CornerRadiusFilterConverter` 过滤 TopLeft / TopRight）
- 有头部时，封面圆角为 0

### 操作区（Actions）

通过 `Card.Actions` 集合添加操作按钮（推荐使用 `CardActionButton`）：

```xml
<atom:Card.Actions>
    <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}" />
    <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
</atom:Card.Actions>
```

操作区使用 `UniformGrid` 等分布局，操作按钮之间通过自定义绘制的分割线分隔。操作区顶部绘制分割线与内容区分离。

### 内容类型自动检测

Card 根据 `Content` 的类型自动设置 `ContentType`：

| Content 类型 | ContentType | 布局影响 |
|---|---|---|
| `CardMetaContent` | `Meta` | 默认内容内边距 |
| `CardTabsContent` | `Tabs` | 无内容内边距，Tab 控件自行管理 |
| `CardGridContent` | `Grid` | 无内容内边距，头部底部边框隐藏，圆角调整 |
| 其他 | `Default` | 默认内容内边距 |

### 尺寸（SizeType）

通过 `SizeType` 属性设置卡片尺寸，影响头部高度、头部字号和内容内边距：

| 尺寸 | 头部高度 | 头部字号 | 头部内边距 | 内容内边距 |
|---|---|---|---|---|
| `Large` | `HeaderHeightLG` | `HeaderFontSizeLG` | `HeaderPaddingLG` | `BodyPaddingLG` |
| `Middle` | `HeaderHeight` | `HeaderFontSize` | `HeaderPadding` | `BodyPadding` |
| `Small` | `HeaderHeightSM` | `HeaderFontSizeSM` | `HeaderPaddingSM` | `BodyPaddingSM` |

### 悬浮阴影（IsHoverable）

当 `IsHoverable = true` 时：
1. 鼠标光标变为手形（`Cursor = Hand`）
2. 鼠标悬浮时（`:pointerover`），`BoxShadow` 切换为 `CardShadows`（深阴影），边框隐藏
3. 配合 `BoxShadowsTransition` 实现平滑过渡

### 内嵌模式（IsInnerMode）

当 `IsInnerMode = true` 时：
- 头部背景变为 `SharedToken.ColorFillAlter`（区别于外层卡片的透明头部）
- 适合卡片嵌套场景，通过视觉层次区分内外卡片

### 加载状态（IsLoading）

当 `IsLoading = true` 时：
- 模板内的 `Skeleton` 控件接管内容区域，展示骨架屏
- 骨架屏默认 4 行段落、不显示标题
- 骨架屏动画处于活跃状态（`IsActive="True"`）

### 过渡动画（Transitions）

动画在 `OnLoaded` 时配置，`OnUnloaded` 时清除（避免不可见控件消耗资源）。

| 过渡属性 | 动画类型 | 用途 |
|---|---|---|
| `BoxShadow` | `BoxShadowsTransition` | 悬浮阴影渐变（`MotionDurationFast`） |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本卡片 | ✅ `<Card>` | ✅ `<atom:Card>` | ✅ 完全对齐 |
| 标题 `title` | ✅ ReactNode | ✅ `Header` 属性 | ✅ 完全对齐 |
| 额外区 `extra` | ✅ ReactNode | ✅ `Extra` 属性 | ✅ 完全对齐 |
| 封面 `cover` | ✅ ReactNode | ✅ `Cover` 属性 | ✅ 完全对齐 |
| 操作 `actions` | ✅ 数组 | ✅ `Actions` 集合 | ✅ 完全对齐 |
| 无边框 `bordered` | ✅ 布尔属性 | ✅ `StyleVariant="Borderless"` | ✅ 完全对齐（语义等价） |
| 尺寸 `size` | ✅ default / small | ✅ `SizeType`（Large / Middle / Small） | ✅ 扩展（增加 Large） |
| 悬浮阴影 `hoverable` | ✅ 布尔属性 | ✅ `IsHoverable` | ✅ 完全对齐 |
| 加载中 `loading` | ✅ 布尔属性 | ✅ `IsLoading` + 内置 Skeleton | ✅ 完全对齐 |
| Card.Meta | ✅ 组件 | ✅ `CardMetaContent` | ✅ 完全对齐 |
| Card.Grid | ✅ 组件 | ✅ `CardGridContent` + `CardGridItem` | ✅ 完全对齐 |
| 内嵌卡片 | ✅ `type="inner"` | ✅ `IsInnerMode` | ✅ 完全对齐 |
| 标签页 | ✅ `tabList` / `tabBarExtraContent` | ✅ `CardTabsContent` | ✅ 完全对齐 |
| `bodyStyle` | ✅ CSSProperties | ❌ 不适用 | — 通过 Style 覆盖实现 |
| `headStyle` | ✅ CSSProperties | ❌ 不适用 | — 通过 Style 覆盖实现 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Primitives.HeaderedContentControl
              └── AtomUI.Desktop.Controls.Card
                    ├── implements ISizeTypeAware
                    └── implements IMotionAwareControl
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现 |
| `HeaderedContentControl` | `Header` / `HeaderTemplate` 头部区域支持 |
| `AtomUI.Desktop.Controls.Card` | Ant Design 视觉体系（风格变体/三种尺寸/封面/操作区/内嵌模式）、Design Token 集成、悬浮阴影动画、骨架屏加载、多种内容类型自动检测 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持动画开关（`IsMotionEnabled`）控制过渡效果 |

### 相关类型

| 类型 | 继承自 | 说明 |
|---|---|---|
| `CardStyleVariant` | 枚举 | 风格变体：`Outline`、`Borderless` |
| `CardMetaContent` | `HeaderedContentControl` | 结构化 Meta 内容（Avatar + Header + Content） |
| `CardTabsContent` | `TemplatedControl` | 内嵌标签页内容 |
| `CardGridContent` | `ItemsControl` | 网格布局内容 |
| `CardGridItem` | `ContentControl` | 网格单元格 |
| `CardActionPanel` | `TemplatedControl`（内部类） | 操作区面板 |
| `CardActionButton` | `IconButton` | 操作区图标按钮 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Card/Card.cs` | Card 主控件 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Card/CardPseudoClass.cs` | 伪类定义 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Card/CardToken.cs` | 组件级 Design Token |
| Meta 内容 | `src/AtomUI.Desktop.Controls/Card/CardMetaContent.cs` | CardMetaContent 控件 |
| Tabs 内容 | `src/AtomUI.Desktop.Controls/Card/CardTabsContent.cs` | CardTabsContent 控件 |
| Grid 内容 | `src/AtomUI.Desktop.Controls/Card/CardGridContent.cs` | CardGridContent 控件 |
| Grid 单元格 | `src/AtomUI.Desktop.Controls/Card/CardGridItem.cs` | CardGridItem 控件 |
| 操作面板 | `src/AtomUI.Desktop.Controls/Card/CardActionPanel.cs` | 操作区面板（内部） |
| 操作按钮 | `src/AtomUI.Desktop.Controls/Card/CardActionButton.cs` | 操作区按钮 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Card/Themes/CardTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/Card/Themes/CardThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` | 使用范例 |

---

## 模板结构

Card 的 ControlTemplate 采用分层 Panel 布局：

```
Panel
├── Border#Frame                                    ← 主框架（背景、边框、圆角、阴影）
└── DockPanel (LastChildFill=True)                  ← 内容布局容器
    ├── Border#HeaderFrame (Dock=Top)               ← 头部区域（标题 + 额外区）
    │   └── DockPanel
    │       ├── ContentPresenter#HeaderExtra (Dock=Right) ← 额外区
    │       └── ContentPresenter#TitlePresenter     ← 标题
    ├── CardActionPanel#PART_ActionPanel (Dock=Bottom) ← 操作区面板
    ├── Border#CoverFrame (Dock=Top)                ← 封面区域（圆角裁剪）
    │   └── ContentPresenter#CoverContentPresenter  ← 封面内容
    └── Border#CardContent                          ← 内容区域
        └── Skeleton                                ← 骨架屏 / 实际内容
            └── ContentPresenter                    ← 用户内容
```

**分层设计理由：**
- **主框架独立**：`Border#Frame` 独立承载背景和阴影，与内容布局分离，使阴影不影响内容定位。
- **封面圆角裁剪**：`Border#CoverFrame` 使用 `ClipToBounds="True"` + 过滤后的圆角，使封面图片与卡片圆角贴合。
- **骨架屏集成**：内容区使用 `Skeleton` 控件包裹，通过 `IsLoading` 控制骨架屏/实际内容切换。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `CardThemeConstants.ActionPanelPart` | `"PART_ActionPanel"` | 操作区面板 |
| `CardActionPanelThemeConstants.GridPanelPart` | `"PART_GridPanel"` | 操作区内部均分网格 |
| `CardGridContentThemeConstants.ItemsPresenterPart` | `"PART_ItemsPresenter"` | Grid 内容的 ItemsPresenter |
| `CardTabsContentThemeConstants.TabControlPart` | `"PART_TabControl"` | Tabs 内容的 TabControl |
