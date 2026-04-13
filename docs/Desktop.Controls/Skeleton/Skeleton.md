# Skeleton 骨架屏

## 概述

骨架屏（Skeleton）在数据加载完成前展示页面的大致结构，给予用户「数据正在加载」的感知。它用灰色占位块模拟真实内容的布局，避免页面在加载过程中呈现空白，有效减少用户的等待焦虑。

AtomUI 的 Skeleton 控件完整实现了 [Ant Design 5.0 Skeleton](https://ant.design/components/skeleton-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和流光动画效果。

---

## 设计原理

### Ant Design 的骨架屏设计哲学

Ant Design 对骨架屏的定位是：**「在需要等待加载内容的位置提供一个占位图形组合」**。骨架屏的核心价值不是替代 Loading Spinner，而是提供内容结构的**预期暗示**——让用户在数据到达之前就能预判页面布局，降低认知负荷。

**两种使用模式：**

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| **组合骨架屏（Skeleton）** | 整体占位，模拟头像 + 标题 + 段落的典型内容结构。加载完成后切换为真实内容 | 列表项加载、卡片内容加载、文章详情初始化 |
| **独立元素骨架屏** | 单个 UI 元素占位，模拟按钮/输入框/头像/图片的外形。通常嵌入自定义布局 | 表单加载、操作栏加载、头像列表加载 |

**独立元素类型：**

| 元素 | 对应控件 | 模拟对象 |
|---|---|---|
| **按钮骨架** | `SkeletonButton` | 按钮占位（支持方形/圆形/胶囊形） |
| **头像骨架** | `SkeletonAvatar` | 头像占位（支持圆形/方形） |
| **输入框骨架** | `SkeletonInput` | 输入框占位 |
| **图片骨架** | `SkeletonImage` | 图片占位（内嵌图片图标） |
| **自定义骨架** | `SkeletonNode` | 自定义内容占位（可放入任意子元素） |

**流光动画：**

当 `IsActive=True` 时，骨架块呈现从左到右的渐变流光效果（shimmer），暗示数据正在加载中。动画通过三阶段渐变色（Start → Middle → End）的无限循环实现。

### Avalonia 基础

Skeleton 家族的所有控件均继承自 Avalonia 的 `TemplatedControl`，不依赖 Avalonia 内置的 Skeleton 控件（Avalonia 没有内置骨架屏组件）。所有骨架屏控件均为 AtomUI 从零构建。

### AtomUI 的架构设计

AtomUI Skeleton 采用**控件族**设计模式，通过抽象基类和继承体系实现代码复用：

| 设计决策 | 实现方式 | 设计动机 |
|---|---|---|
| **统一动画基础** | `AbstractSkeleton` 抽象基类 | 所有骨架控件共享流光动画逻辑，避免重复实现 |
| **动画同步** | `Follow/UnFollow` 机制 | 组合骨架屏中的子元素（头像、标题、段落）跟随父级动画节奏，确保视觉一致 |
| **组合模式** | `Skeleton` 内嵌 `SkeletonAvatar` + `SkeletonTitle` + `SkeletonParagraph` | 对齐 Ant Design 的组合骨架屏结构 |
| **独立元素基类** | `SkeletonElement` 抽象类 | `SkeletonButton` / `SkeletonInput` 共享尺寸和 Block 逻辑 |
| **内容切换** | `IsLoading` + `Content` 属性 | `Skeleton` 在加载态显示骨架，加载完成后显示真实内容 |
| **灵活尺寸** | `Dimension` 结构体（支持像素和百分比） | 标题宽度、段落行宽支持绝对值和相对值 |
| **Design Token** | `SkeletonToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 组合骨架屏（Skeleton）

`Skeleton` 控件是最常用的骨架屏形式，它内部组合了头像、标题和段落三个部分：

- **头像区域**（`SkeletonAvatar`）：通过 `IsShowAvatar` 控制是否显示，支持 `AvatarShape`（Circle/Square）和 `AvatarSizeType`（Large/Middle/Small/Custom）
- **标题区域**（`SkeletonTitle`）：通过 `IsShowTitle` 控制是否显示，宽度由 `TitleWidth` 控制（默认 50%）
- **段落区域**（`SkeletonParagraph`）：通过 `IsShowParagraph` 控制是否显示，行数由 `ParagraphRows` 控制，最后一行宽度由 `ParagraphLastLineWidth` 控制

主题会根据头像、标题、段落的显示组合自动调整默认值：
- 有头像 + 有段落：标题宽度默认 50%
- 无头像 + 有段落：标题宽度默认 38%
- 无头像 + 无标题：最后一行宽度默认 61%
- 无头像 + 有标题：段落行数默认 3

### 内容切换（IsLoading）

`Skeleton` 控件支持包裹真实内容。当 `IsLoading=True` 时显示骨架占位，`IsLoading=False` 时显示 `Content` 中的真实内容：

```xml
<atom:Skeleton IsLoading="{Binding IsDataLoading}">
    <TextBlock>真实内容</TextBlock>
</atom:Skeleton>
```

### 流光动画（IsActive）

所有骨架控件均支持 `IsActive` 属性。启用后，骨架块呈现从左到右的渐变流光效果：

- 动画使用三阶段 `LinearGradientBrush`（Start → Middle → End）
- 动画时长由 `MotionDuration` Token 控制（默认 1.4 秒）
- 使用 `CubicEaseOut` 缓动曲线
- 动画无限循环直到 `IsActive` 设为 `false` 或控件离开视觉树

### 动画同步（Follow 机制）

在组合骨架屏中，`Skeleton` 作为动画主控，子组件（`SkeletonAvatar`、`SkeletonTitle`、`SkeletonParagraph`）通过 `Follow(this)` 绑定到父级的 `AnimationLayerFill` 和 `IsActive` 属性，确保所有骨架块的流光动画保持同步。

### 独立元素骨架屏

独立元素骨架屏用于自定义布局中单独使用，每个元素各自管理动画：

| 控件 | 默认宽度 | 默认高度 | 形状支持 |
|---|---|---|---|
| `SkeletonButton` | `Height × 2` | `ControlHeight*` | Square / Round / Circle |
| `SkeletonInput` | `Height × 5` | `ControlHeight*` | — |
| `SkeletonAvatar` | `ControlHeight*` | `ControlHeight*` | Circle / Square |
| `SkeletonImage` | `ImageContainerSize` | `ImageContainerSize` | — |
| `SkeletonNode` | `ImageContainerSize` | `ImageContainerSize` | —（可放入自定义内容） |

> `*` 高度根据 `SizeType`（Large/Middle/Small）从 `ControlHeightLG`/`ControlHeight`/`ControlHeightSM` 获取。

### 圆角控制（IsRound）

`Skeleton` 和 `SkeletonParagraph` 的 `IsRound` 属性控制段落行的圆角样式：
- `false`（默认）：使用 `BlockRadius`（`BorderRadiusSM`）
- `true`：使用 `ParagraphLineRoundCornerRadius`（行高的一半，形成胶囊形）

### 尺寸系统（Dimension）

`TitleWidth`、`ParagraphLastLineWidth`、`ParagraphLineWidths` 等属性使用 `Dimension` 结构体，支持两种单位：
- **百分比**：`new Dimension(50, DimensionUnitType.Percentage)` 或 AXAML 中写 `"50%"`
- **像素**：`new Dimension(200, DimensionUnitType.Pixel)` 或 AXAML 中写 `"200"`

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本骨架屏 | ✅ `<Skeleton />` | ✅ `<Skeleton IsLoading="True" />` | ✅ 完全对齐 |
| 头像 + 标题 + 段落 | ✅ `avatar` / `title` / `paragraph` | ✅ `IsShowAvatar` / `IsShowTitle` / `IsShowParagraph` | ✅ 完全对齐 |
| 流光动画 | ✅ `active` | ✅ `IsActive` | ✅ 完全对齐 |
| 内容包裹 | ✅ `loading` + `children` | ✅ `IsLoading` + `Content` | ✅ 完全对齐 |
| 段落行数 | ✅ `paragraph.rows` | ✅ `ParagraphRows` | ✅ 完全对齐 |
| 段落行宽 | ✅ `paragraph.width` | ✅ `ParagraphLineWidths` / `ParagraphLastLineWidth` | ✅ 完全对齐 |
| 标题宽度 | ✅ `title.width` | ✅ `TitleWidth` | ✅ 完全对齐 |
| 头像形状/尺寸 | ✅ `avatar.shape` / `avatar.size` | ✅ `AvatarShape` / `AvatarSizeType` / `AvatarSize` | ✅ 完全对齐 |
| 圆角模式 | ✅ `round` | ✅ `IsRound` | ✅ 完全对齐 |
| 按钮骨架 | ✅ `Skeleton.Button` | ✅ `SkeletonButton` | ✅ 完全对齐 |
| 头像骨架 | ✅ `Skeleton.Avatar` | ✅ `SkeletonAvatar` | ✅ 完全对齐 |
| 输入框骨架 | ✅ `Skeleton.Input` | ✅ `SkeletonInput` | ✅ 完全对齐 |
| 图片骨架 | ✅ `Skeleton.Image` | ✅ `SkeletonImage` | ✅ 完全对齐 |
| 自定义节点 | ✅ `Skeleton.Node` | ✅ `SkeletonNode` | ✅ 完全对齐 |
| Block 模式 | ✅ `block` | ✅ `IsBlock` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AbstractSkeleton                              ← 流光动画基类（所有骨架控件共享）
        ├── Skeleton                                 ← 组合骨架屏（头像 + 标题 + 段落 + 内容切换）
        ├── SkeletonLine                             ← 单行骨架线条（段落的组成单元）
        │     └── SkeletonTitle                      ← 标题骨架（SkeletonLine 子类，默认宽度 50%）
        ├── SkeletonParagraph                        ← 段落骨架（动态生成多个 SkeletonLine）
        ├── SkeletonAvatar                           ← 头像骨架（支持圆形/方形 + 三种尺寸）
        ├── SkeletonElement                          ← 独立元素骨架基类（尺寸 + Block）
        │     ├── SkeletonButton                     ← 按钮骨架（方形/圆形/胶囊形）
        │     └── SkeletonInput                      ← 输入框骨架
        └── SkeletonImage                            ← 图片骨架（内嵌图片图标）
              └── SkeletonNode                       ← 自定义节点骨架（可放入任意内容）
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | Avalonia 模板化控件基础 |
| `AbstractSkeleton` | 流光动画逻辑（`IsActive`、`MotionDuration`、动画构建/启停）、Follow/UnFollow 同步机制、动画渐变色属性 |
| `Skeleton` | 组合骨架屏：内嵌 `SkeletonAvatar` + `SkeletonTitle` + `SkeletonParagraph`，`IsLoading` + `Content` 内容切换，Token 注册 |
| `SkeletonElement` | 独立元素基类：`SizeType`（可自定义尺寸）、`IsBlock`（撑满宽度） |
| `SkeletonButton` | 按钮骨架：`Shape`（Square/Round/Circle），自动计算宽度和圆角 |
| `SkeletonInput` | 输入框骨架：默认宽度 = 高度 × 5 |
| `SkeletonAvatar` | 头像骨架：`Shape`（Circle/Square）、`Size`（自定义尺寸）、`SizeType` |
| `SkeletonLine` | 单行骨架：`LineWidth`（Dimension）、`IsRound`，由 `SkeletonParagraph` 动态创建 |
| `SkeletonTitle` | 标题骨架：继承 `SkeletonLine`，默认 `LineWidth=50%` |
| `SkeletonParagraph` | 段落骨架：动态创建 `Rows` 个 `SkeletonLine`，支持 `LastLineWidth` 和 `LineWidths` |
| `SkeletonImage` | 图片骨架：固定尺寸 + 内嵌 `ImageFilled` 图标 |
| `SkeletonNode` | 自定义节点：继承 `SkeletonImage`，支持 `Content` / `ContentTemplate` |

**实现的共享接口：**

| 接口 | 实现控件 | 定义位置 | 作用 |
|---|---|---|---|
| `ICustomizableSizeTypeAware` | `SkeletonElement`、`SkeletonAvatar` | `AtomUI.Controls.Shared` | 支持 `CustomizableSizeType`（Large/Middle/Small/Custom）四种尺寸 |

> 注意：Skeleton 系列控件全部定义在 `AtomUI.Desktop.Controls` 中，不存在 `AtomUI.Controls` 基类层的设备无关抽象。这是因为骨架屏的渲染和动画行为与桌面平台紧密相关。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 动画基类 | `src/AtomUI.Desktop.Controls/Skeleton/AbstractSkeleton.cs` | 流光动画逻辑、Follow 机制 |
| 组合骨架 | `src/AtomUI.Desktop.Controls/Skeleton/Skeleton.cs` | 主骨架屏控件 |
| 元素基类 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonElement.cs` | 独立元素基类（SizeType + IsBlock） |
| 按钮骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonButton.cs` | 按钮占位 |
| 输入框骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonInput.cs` | 输入框占位 |
| 头像骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonAvatar.cs` | 头像占位 |
| 行骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonLine.cs` | 单行占位线条 |
| 标题骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonTitle.cs` | 标题占位 |
| 段落骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonParagraph.cs` | 段落占位（多行） |
| 图片骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonImage.cs` | 图片占位 |
| 节点骨架 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonNode.cs` | 自定义节点占位 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Skeleton/SkeletonToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonTheme.axaml` | Skeleton ControlTheme |
| 抽象主题 | `src/AtomUI.Desktop.Controls/Skeleton/Themes/AbstractSkeletonTheme.axaml` | 通用骨架主题基类 |
| 主题常量 | `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml` | 使用范例 |

---

## 模板结构

### Skeleton（组合骨架屏）

```
Panel
├── DockPanel (IsVisible = !IsContentVisible)          ← 骨架占位层
│   ├── SkeletonAvatar#PART_Avatar (DockPanel.Dock=Left) ← 头像区域
│   └── StackPanel#PART_Content (Orientation=Vertical)   ← 内容区域
│       ├── SkeletonTitle#PART_Title                     ← 标题行
│       └── SkeletonParagraph#PART_Paragraph             ← 段落区域
└── ContentPresenter (IsVisible = IsContentVisible)      ← 真实内容层
```

### AbstractSkeleton（通用骨架基础模板）

```
Panel#PART_RootLayout
├── Border#PART_ActiveAnimationLayer       ← 流光动画层（IsActive 时可见）
│   Background = AnimationLayerFill        ← 渐变画刷（动画驱动）
└── Border#PART_ContentLayer               ← 静态背景层（!IsActive 时可见）
    Background = GradientFromColor         ← 纯色填充
```

### 模板部件常量

| 常量 | 值 | 所属 | 说明 |
|---|---|---|---|
| `AbstractSkeletonThemeConstants.RootLayoutPart` | `"PART_RootLayout"` | 通用基础 | 根布局面板 |
| `AbstractSkeletonThemeConstants.ActiveAnimationLayerPart` | `"PART_ActiveAnimationLayer"` | 通用基础 | 流光动画层 |
| `AbstractSkeletonThemeConstants.ContentLayerPart` | `"PART_ContentLayer"` | 通用基础 | 静态背景层 |
| `SkeletonThemeConstants.AvatarPart` | `"PART_Avatar"` | Skeleton | 头像组件 |
| `SkeletonThemeConstants.TitlePart` | `"PART_Title"` | Skeleton | 标题组件 |
| `SkeletonThemeConstants.ContentPart` | `"PART_Content"` | Skeleton | 内容面板 |
| `SkeletonThemeConstants.ParagraphPart` | `"PART_Paragraph"` | Skeleton | 段落组件 |
| `SkeletonParagraphThemeConstants.LineLayoutPart` | `"PART_LineLayout"` | SkeletonParagraph | 行布局面板 |
