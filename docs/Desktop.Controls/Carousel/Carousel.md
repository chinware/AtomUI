# Carousel 走马灯

## 概述

走马灯（Carousel）是循环展示组件，用于在有限空间内循环播放一组内容（图片、卡片等）。支持自动播放、分页指示器、导航按钮、滑动手势、多种过渡效果和四个方向的分页位置。

AtomUI 的 `Carousel` 控件复刻了 [Ant Design 5.0 Carousel](https://ant.design/components/carousel-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的走马灯设计哲学

Ant Design 对走马灯的定位是：**「在有限空间内，循环播放同一类型的图片、文字等内容」**。典型使用场景包括：

| 场景 | 说明 |
|---|---|
| 🖼️ **首页横幅** | 多张 Banner 图片轮播展示 |
| 📦 **产品展示** | 产品多角度图片切换 |
| 📰 **新闻轮播** | 新闻标题自动滚动 |
| 🎯 **引导页** | 新用户引导步骤展示 |

**核心能力**：

| 能力 | 设计意图 |
|---|---|
| 🔄 **自动播放** | 内容定时自动切换，无需用户操作 |
| 📍 **分页指示器** | 显示当前位置和总页数，支持点击跳转 |
| ⬅️➡️ **导航箭头** | 前进/后退按钮，提供手动翻页 |
| 🔁 **无限循环** | 到达末尾后自动回到开头，形成循环 |
| 🎞️ **过渡效果** | 滑动（Scroll）和淡入淡出（Fade）两种切换方式 |
| ⏱️ **进度展示** | 指示器上展示自动播放的进度 |

### Avalonia SelectingItemsControl 基础能力

AtomUI 的 `Carousel` 继承自 Avalonia 框架的 `SelectingItemsControl`。其继承链为：

```
Control → TemplatedControl → ItemsControl → SelectingItemsControl
```

`SelectingItemsControl` 提供了选中项管理（`SelectedIndex` / `SelectedItem`）、选择模式（`SelectionMode`）等基础能力。Carousel 将其用于管理「当前页」的概念——每个页面就是一个选中项。

### AtomUI 的扩展设计

AtomUI `Carousel` 在 `SelectingItemsControl` 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **自动播放** | `IsAutoPlay` + `AutoPlaySpeed` + `DispatcherTimer` | 内容定时自动切换 |
| **分页指示器** | `CarouselPagination` + `CarouselPageIndicator` | 当前页位置可视化 + 点击跳转 |
| **导航箭头** | `CarouselNavButton` + `IsShowNavButtons` | 手动前进/后退翻页 |
| **无限循环** | `IsInfinite` | 到达末尾自动回到开头 |
| **过渡效果** | `CarouselTransitionEffect` (Scroll / Fade) | 滑动或淡入淡出 |
| **四方向分页** | `CarouselPaginationPosition` (Top / Bottom / Left / Right) | 指示器位置灵活配置 |
| **进度展示** | `IsShowTransitionProgress` + 进度动画 | 指示器上展示自动播放进度条 |
| **滑动手势** | `IsSwipeEnabled` + 指针手势检测 | 鼠标/触摸滑动切换页面 |
| **虚拟化面板** | `VirtualizingCarouselPanel` | 仅实例化当前页和过渡页，节省资源 |
| **Design Token** | `CarouselToken` + `RegisterTokenResourceScope` | 视觉值从 Token 派生 |

---

## 功能详解

### 自动播放（IsAutoPlay）

当 `IsAutoPlay = true` 时：
1. 创建 `DispatcherTimer`，间隔为 `AutoPlaySpeed`（默认 3 秒）
2. 每次触发自动调用 `Next()` 切换到下一页
3. 手动切换页面时（`SelectedIndex` 变化），定时器重新计时
4. 控件从可视树分离时自动停止，重新附加时自动恢复

### 分页指示器

分页指示器由 `CarouselPagination`（内部 `SelectingItemsControl`）和 `CarouselPageIndicator`（单个指示点）组成：

- 指示点默认为条形（宽 `IndicatorWidth`，高 `IndicatorHeight`）
- 选中的指示点自动变宽为 `IndicatorActiveWidth`
- 悬浮时不透明度从 0.2 变为 0.75
- 点击指示点可跳转到对应页面
- 通过 `IsShowPagination` 控制显示/隐藏

### 导航箭头（IsShowNavButtons）

当 `IsShowNavButtons = true` 时显示前进/后退按钮：
- 使用 `CarouselNavButton`（继承 `IconButton`）
- 图标为 `LeftOutlined` / `RightOutlined`
- 默认不透明度 0.2，悬浮时变为 1.0
- 非无限循环模式下，到达边界时自动隐藏对应按钮
- 横向模式时按钮位于左右两侧，纵向模式时按钮位于上下两侧

### 分页位置（PaginationPosition）

分页指示器和导航箭头的位置随 `PaginationPosition` 变化：

| 位置 | 指示器位置 | 导航按钮位置 | 滑动方向 | PageSlide 方向 |
|---|---|---|---|---|
| `Bottom`（默认） | 底部居中 | 左右两侧 | 水平 | Horizontal |
| `Top` | 顶部居中 | 左右两侧 | 水平 | Horizontal |
| `Left` | 左侧居中（旋转 90°） | 上下两侧 | 垂直 | Vertical |
| `Right` | 右侧居中（旋转 90°） | 上下两侧 | 垂直 | Vertical |

### 过渡效果（TransitionEffect）

| 效果 | 实现 | 说明 |
|---|---|---|
| `Scroll`（默认） | `PageSlide` | 页面滑动切换，方向跟随 `PaginationPosition` |
| `Fade` | `CrossFade` | 淡入淡出切换 |

过渡动画参数通过 `PageTransitionDuration`、`PageInEasing`、`PageOutEasing` 配置。

### 进度展示（IsShowTransitionProgress）

当 `IsAutoPlay = true` 且 `IsShowTransitionProgress = true` 时：
- 选中的指示点上显示进度条动画
- 进度条从左到右填充，周期与 `AutoPlaySpeed` 一致
- 悬浮指示点时进度条隐藏

### 滑动手势（IsSwipeEnabled）

当 `IsSwipeEnabled = true` 时：
- 鼠标/触摸按下并拖动超过 30px 阈值即触发页面切换
- 水平模式下左滑 = Next，右滑 = Previous
- 垂直模式下上滑 = Next，下滑 = Previous
- 导航按钮和分页指示器区域不触发滑动

### 虚拟化面板

`VirtualizingCarouselPanel` 实现 `ILogicalScrollable`，仅实例化当前页面和过渡中的页面，未显示的页面被回收到对象池，大幅减少内存占用。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本走马灯 | ✅ `<Carousel>` | ✅ `<atom:Carousel>` | ✅ 完全对齐 |
| 自动播放 `autoplay` | ✅ 布尔 | ✅ `IsAutoPlay` | ✅ 完全对齐 |
| 自动播放速度 `autoplaySpeed` | ✅ 毫秒数 | ✅ `AutoPlaySpeed` (TimeSpan) | ✅ 完全对齐 |
| 分页位置 `dotPosition` | ✅ top/bottom/left/right | ✅ `PaginationPosition` 枚举 | ✅ 完全对齐 |
| 淡入淡出 `effect="fade"` | ✅ | ✅ `TransitionEffect="Fade"` | ✅ 完全对齐 |
| 箭头 `arrows` | ✅ 5.17.0 新增 | ✅ `IsShowNavButtons` | ✅ 完全对齐 |
| 进度指示 `dotProgress` | ✅ (未正式发布) | ✅ `IsShowTransitionProgress` | ✅ 完全对齐 |
| 无限循环 `infinite` | ✅ | ✅ `IsInfinite` | ✅ 完全对齐 |
| 滑动手势 `swipe` | ✅ (触屏) | ✅ `IsSwipeEnabled` | ✅ 扩展（鼠标+触摸） |
| `beforeChange` / `afterChange` | ✅ 回调 | ⚠️ 通过 `SelectedIndex` 变化监听 | ⚠️ API 形式不同 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.Primitives.SelectingItemsControl
              └── AtomUI.Desktop.Controls.Carousel
                    └── implements IMotionAwareControl
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 子项管理、`Items` / `ItemsSource` 数据绑定、`ItemTemplate` 模板化 |
| `SelectingItemsControl` | 选中项管理（`SelectedIndex` / `SelectedItem`）、选择模式 |
| `AtomUI.Desktop.Controls.Carousel` | 自动播放、分页指示器、导航箭头、过渡效果、无限循环、滑动手势、虚拟化面板 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持动画开关（`IsMotionEnabled`） |

### 相关类型

| 类型 | 继承自 | 说明 |
|---|---|---|
| `CarouselPaginationPosition` | 枚举 | 分页位置：`Top`、`Bottom`、`Left`、`Right` |
| `CarouselTransitionEffect` | 枚举 | 过渡效果：`Scroll`、`Fade` |
| `CarouselPage` | `ContentControl` | 走马灯页面容器 |
| `CarouselNavButton` | `IconButton` | 导航箭头按钮 |
| `CarouselPageIndicator` | `ContentControl`（内部） | 单个分页指示点 |
| `CarouselPagination` | `SelectingItemsControl`（内部） | 分页指示器面板 |
| `VirtualizingCarouselPanel` | `VirtualizingPanel`（内部） | 虚拟化面板，仅实例化当前页 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Carousel/Carousel.cs` | Carousel 主控件 |
| 枚举 | `src/AtomUI.Desktop.Controls/Carousel/CarouselEnums.cs` | 枚举定义 |
| 导航按钮 | `src/AtomUI.Desktop.Controls/Carousel/CarouselNavButton.cs` | 导航箭头按钮 |
| 页面容器 | `src/AtomUI.Desktop.Controls/Carousel/CarouselPage.cs` | CarouselPage 控件 |
| 指示点 | `src/AtomUI.Desktop.Controls/Carousel/CarouselPageIndicator.cs` | 分页指示点（内部） |
| 分页面板 | `src/AtomUI.Desktop.Controls/Carousel/CarouselPagination.cs` | 分页指示器面板（内部） |
| 虚拟化面板 | `src/AtomUI.Desktop.Controls/Carousel/VirtualizingCarouselPanel.cs` | 虚拟化面板（内部） |
| Token 定义 | `src/AtomUI.Desktop.Controls/Carousel/CarouselToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Carousel/Themes/CarouselTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/Carousel/Themes/CarouselPageThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml` | 使用范例 |

---

## 模板结构

Carousel 的 ControlTemplate 采用分层 Panel 布局：

```
Panel
├── ScrollViewer#PART_ScrollViewer                       ← 滚动容器（隐藏滚动条）
│   └── ItemsPresenter#PART_ItemsPresenter               ← 页面展示（VirtualizingCarouselPanel）
├── CarouselNavButton#PART_PreviousButton                ← 前一页导航按钮（LeftOutlined）
├── CarouselNavButton#PART_NextButton                    ← 下一页导航按钮（RightOutlined）
└── LayoutTransformControl#PaginationLayoutTransform     ← 分页指示器布局（支持旋转）
    └── CarouselPagination#PART_Pagination               ← 分页指示器面板
        └── StackPanel (Horizontal, Spacing=IndicatorGap)
            └── CarouselPageIndicator × N                ← 每个页面一个指示点
```

**分层设计理由：**
- **滚动容器独立**：使用 `ScrollViewer` + `VirtualizingCarouselPanel` 实现虚拟化页面切换，隐藏原生滚动条。
- **导航按钮浮动**：按钮在 Panel 内浮动定位，根据 `PaginationPosition` 自动调整位置。
- **分页指示器旋转**：使用 `LayoutTransformControl` 包裹，左/右位置时旋转 90° 实现纵向指示器。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `CarouselThemeConstants.ScrollViewerPart` | `"PART_ScrollViewer"` | 滚动容器 |
| `CarouselThemeConstants.PaginationPart` | `"PART_Pagination"` | 分页指示器面板 |
| `CarouselThemeConstants.PreviousButtonPart` | `"PART_PreviousButton"` | 前一页按钮 |
| `CarouselThemeConstants.NextButtonPart` | `"PART_NextButton"` | 下一页按钮 |
| `CarouselPageIndicatorThemeConstants.FramePart` | `"PART_Frame"` | 指示点主框架 |
