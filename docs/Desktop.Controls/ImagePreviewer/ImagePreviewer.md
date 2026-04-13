# ImagePreviewer 图片预览

## 概述

图片预览（ImagePreviewer）用于预览和浏览图片。支持缩放、旋转、水平/垂直翻转、全屏查看、适应窗口/原始尺寸切换、鼠标拖拽移动、多图切换（键盘左右箭头及按钮）等功能。同时支持 SVG 和位图（PNG、JPEG、WebP 等）格式。

AtomUI 的 `ImagePreviewer` 对应 [Ant Design 5.0 Image](https://ant.design/components/image-cn) 组件，在 .NET / Avalonia 平台上提供封面预览 → 弹窗查看的完整图片浏览体验。

AtomUI 提供两个公共控件类：

| 控件 | 用途 |
|---|---|
| `ImagePreviewer` | **单图预览** — 显示一张封面图，点击后弹出预览对话框。支持多张图片轮播（从一张封面预览多张） |
| `ImageGroupPreviewer` | **多图组预览** — 以网格/列表形式显示多张封面缩略图，点击任意一张弹出预览对话框并定位到对应图片 |

---

## 设计原理

### Ant Design 的 Image 设计

Ant Design 的 `Image` 组件采用 **封面 → 遮罩 → 预览层** 的三层交互模型：

1. **封面层**：展示图片缩略图，鼠标悬浮时显示半透明遮罩和"预览"提示文字
2. **预览层**：点击后弹出全屏/弹窗式预览，提供缩放、旋转、翻转、多图切换等操作工具栏
3. **图片组**：`Image.PreviewGroup` 将多张图片组合，共享一个预览层

AtomUI 忠实复刻了这套交互模型，并将其适配为桌面窗口体验：预览层以独立 `Window` 形式呈现，拥有标题栏工具栏和浮动工具栏双操作区域。

### AtomUI 的架构设计

AtomUI 的 ImagePreviewer 采用 **控件 → 对话框 → 查看器** 的分层架构：

```
用户交互层（公共）
├── ImagePreviewer        → 单图封面控件（公共）
└── ImageGroupPreviewer   → 多图组封面控件（公共）
      ↓ 点击触发
预览窗口层（内部）
├── ImagePreviewerDialog  → 预览对话框窗口（继承 Window）
│   ├── ImagePreviewToolbar     → 标题栏工具栏（上/下一张、缩放、旋转、翻转）
│   └── ImageViewer             → 图片查看器
│       ├── ImagePreviewRenderer    → 图片渲染器（支持 SVG + Bitmap）
│       ├── ImagePreviewNavButton   → 左/右切换按钮
│       └── ImagePreviewFloatToolbar → 浮动工具栏（缩放、旋转、翻转、索引指示）
封面层（内部）
└── ImagePreviewerCover   → 封面展示 + 遮罩动画
```

### 扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **SVG + 位图双格式** | `PreviewImageSource` 自动检测 `.svg` 扩展名 | 桌面应用中 SVG 图标/插图与位图照片混用是常见场景 |
| **封面遮罩动画** | `ImagePreviewerCover` + `MaskOpacity` 过渡 | 复刻 Ant Design 的悬浮遮罩淡入效果 |
| **双工具栏** | 标题栏 `ImagePreviewToolbar` + 浮动 `ImagePreviewFloatToolbar` | 标题栏工具栏操作切换图片时**保留旋转/翻转状态**，浮动工具栏切换时**重置状态**（对齐 Ant Design 行为） |
| **窗口化预览** | `ImagePreviewerDialog` 继承 `Window` | 桌面端利用原生窗口能力（移动、缩放、最大化、置顶） |
| **对话框定位** | `DialogHorizontalStartupLocation` / `DialogVerticalStartupLocation` | 支持居中、左/右/上/下对齐，以及自定义偏移量 |
| **模态/非模态** | `IsDialogModal` 属性 | 非模态（默认）允许同时操作主窗口；模态阻塞主窗口交互 |
| **图片拖拽** | `IsImageMovable` + 鼠标事件处理 | 缩放后拖拽查看图片细节 |
| **缩放控制** | `ImageScaleStep` / `ImageMinScale` / `ImageMaxScale` | 精确控制缩放行为，支持鼠标滚轮缩放 |
| **键盘导航** | `Left` / `Right` 方向键切换图片 | 无需鼠标即可浏览 |
| **适应窗口** | `FitToWindow` / `OneToOne` 切换 | 在窗口适应和原始尺寸之间自由切换 |
| **Design Token** | `ImagePreviewerToken` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 封面交互

`ImagePreviewer` 展示一张封面图片。鼠标悬浮时，半透明遮罩（`MaskBgColor`）以动画淡入，显示"预览"提示（眼睛图标 + 本地化文字）。点击封面打开预览对话框。

- 可通过 `CoverImageSrc` 指定不同于预览图的封面图
- 可通过 `IsShowCoverMask` 控制是否显示遮罩
- 可通过 `CoverIndicatorContent` / `CoverIndicatorContentTemplate` 自定义遮罩内容

`ImageGroupPreviewer` 使用 `ItemsControl` 展示多张封面缩略图，点击任意一张直接定位到该图片的预览。

### 预览对话框

预览对话框（`ImagePreviewerDialog`）是一个独立的 `Window`，初始尺寸为屏幕工作区的 70%。提供：

- **标题栏工具栏**：上/下一张、缩小、放大、适应窗口/原始尺寸、水平翻转、垂直翻转、左旋转、右旋转
- **浮动工具栏**：底部居中，圆角胶囊形，含相同操作按钮 + 图片索引指示器（`1 / 3` 格式）
- **左/右导航按钮**：叠加在图片两侧，半透明圆形按钮

### 缩放与变换

- 缩放步进由 `ImageScaleStep` 控制（默认 0.5，即每次缩放 50%）
- 缩放范围由 `ImageMinScale`（默认 1.0）和 `ImageMaxScale`（默认 50.0）限制
- 鼠标滚轮支持平滑缩放（步进根据滚轮增量动态调整，范围 0.5% ~ 12%）
- 缩放后图片位置自动约束在可视区域内

### 图片切换策略

标题栏工具栏和浮动工具栏在切换图片时的变换保留策略不同：

| 工具栏位置 | 切换时旋转/翻转 | 动画 |
|---|---|---|
| 标题栏 | **保留** | 有动画 |
| 浮动栏 | **重置** | 无动画（即时切换） |

这一设计对齐 Ant Design 的行为：标题栏操作更"持久"，浮动栏操作更"轻量"。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 封面 + 遮罩 | ✅ 悬浮显示遮罩 | ✅ `MaskOpacity` 动画 | ✅ 完全对齐 |
| 预览弹窗 | ✅ 全屏遮罩层 | ✅ 独立 Window | ⚠️ 桌面窗口替代 Web 遮罩层 |
| 缩放 | ✅ 放大/缩小/滚轮 | ✅ `ScaleStep` + 滚轮 | ✅ 完全对齐 |
| 旋转 | ✅ 左旋/右旋 | ✅ 90° 步进 | ✅ 完全对齐 |
| 翻转 | ✅ 水平/垂直 | ✅ `ScaleX` / `ScaleY` 翻转 | ✅ 完全对齐 |
| 适应窗口/原始尺寸 | ✅ 切换 | ✅ `FitToWindow` / `OneToOne` | ✅ 完全对齐 |
| 多图组 | ✅ `Image.PreviewGroup` | ✅ `ImageGroupPreviewer` | ✅ 完全对齐 |
| 键盘导航 | ✅ 左/右方向键 | ✅ `Key.Left` / `Key.Right` | ✅ 完全对齐 |
| 拖拽移动 | ✅ 放大后可拖拽 | ✅ `IsImageMovable` | ✅ 完全对齐 |
| 自定义预览图 | ✅ `preview` 属性 | ✅ `CoverImageSrc` | ✅ 完全对齐 |
| 容错图片 | ✅ `fallback` | ✅ `FallbackImageSrc` | ✅ 完全对齐 |
| SVG 支持 | ❌ | ✅ 自动检测 | ✅ AtomUI 增强 |
| 模态/非模态 | ❌（Web 无此概念） | ✅ `IsDialogModal` | ✅ 桌面增强 |
| 窗口移动/缩放/最大化 | ❌（Web 无此概念） | ✅ 原生窗口 | ✅ 桌面增强 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.AbstractImagePreviewer (IMotionAwareControl)
        ├── AtomUI.Desktop.Controls.ImagePreviewer        ← 单图预览
        └── AtomUI.Desktop.Controls.ImageGroupPreviewer   ← 多图组预览
```

> **注意**：与大多数 AtomUI 控件不同，ImagePreviewer 的 `AbstractImagePreviewer` 基类位于 `AtomUI.Desktop.Controls` 而非 `AtomUI.Controls`，因为预览对话框（`Window`）是桌面特有概念。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `AbstractImagePreviewer` | 图片源管理、预览对话框生命周期（打开/关闭/事件）、对话框定位配置、缩放参数传递、SVG/Bitmap 加载 |
| `ImagePreviewer` | 封面图管理（`CoverImageSrc`）、封面遮罩和指示器内容（`CoverIndicatorContent`）、自动选取首张图作为封面 |
| `ImageGroupPreviewer` | 多图缩略图列表展示（`ItemsPanel`）、点击定位到对应图片索引 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 过渡动画开关 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Desktop.Controls/ImagePreviewer/AbstractImagePreviewer.cs` | 图片预览基类 |
| 单图控件 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewer.cs` | 单图预览控件 |
| 多图组控件 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImageGroupPreviewer.cs` | 多图组预览控件 |
| 事件参数 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImageFitToWindowEventArgs.cs` | 适应窗口事件参数 |
| Token 定义 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs` | 组件级 Design Token |
| 预览对话框 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerDialog.cs` | 预览窗口（internal） |
| 图片查看器 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImageViewer.cs` | 图片查看器（internal） |
| 封面组件 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerCover.cs` | 封面展示（internal） |
| 导航按钮 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewNavButton.cs` | 左/右导航按钮（internal） |
| 工具栏基类 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewBaseToolbar.cs` | 工具栏基类（internal） |
| 浮动工具栏 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewFloatToolbar.cs` | 底部浮动工具栏（internal） |
| 标题栏工具栏 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewToolbar.cs` | 标题栏工具栏（internal） |
| 图片渲染器 | `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewRenderer.cs` | SVG/Bitmap 渲染（internal） |
| 图片源 | `src/AtomUI.Desktop.Controls/ImagePreviewer/PreviewImageSource.cs` | 图片数据封装（internal） |
| 模板常量 | `src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerThemeConstants.cs` | 模板部件常量 |
| 本地化 en_US | `src/AtomUI.Desktop.Controls/ImagePreviewer/Localization/en_US.cs` | 英文语言资源 |
| 本地化 zh_CN | `src/AtomUI.Desktop.Controls/ImagePreviewer/Localization/zh_CN.cs` | 中文语言资源 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ImagePreviewerShowCase.axaml` | 使用范例 |

---

## 模板结构

### ImagePreviewer 模板

```
Border (背景、边框、圆角)
  └── ImagePreviewerCover (封面展示 + 遮罩)
        ├── ImagePreviewRenderer (图片渲染)
        └── Border#Mask (遮罩层)
              └── ContentPresenter (指示器内容：眼睛图标 + "预览"文字)
```

### ImageGroupPreviewer 模板

```
Border (背景、边框、圆角)
  └── ItemsControl#PART_CoverItemsControl (封面列表)
        └── [每项] ImagePreviewerCover (缩略图)
```

### ImagePreviewerDialog 模板

```
Panel
├── Border#WindowFrame (窗口主框架)
│   └── Panel
│       ├── ContentPresenter#WindowFrameLayer
│       └── Border (圆角裁剪)
│           └── DialogLayerManager
│               └── DockPanel#PART_ContentLayout
│                   ├── [DockPanel.Dock=Top] TitleBar (含 ImagePreviewToolbar)
│                   └── [Fill] ImageViewer#PART_ImageViewer
└── WindowResizer (窗口拖拽调整大小)
```

### ImageViewer 模板

```
Panel
├── Canvas#PART_ImageViewerScene
│   └── ImagePreviewRenderer#PART_ImageRenderer (图片渲染，支持变换)
├── ImagePreviewNavButton#PART_PreviousButton (左侧导航按钮)
├── ImagePreviewNavButton#PART_NextButton (右侧导航按钮)
├── ImagePreviewFloatToolbar (底部浮动工具栏)
└── WindowMediaQueryIndicator
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `CoverItemsControlPart` | `"PART_CoverItemsControl"` | 多图组封面列表 |
| `ImageViewerPart` | `"PART_ImageViewer"` | 图片查看器 |
| `ImageViewerScenePart` | `"PART_ImageViewerScene"` | 图片查看器画布 |
| `ImageRendererPart` | `"PART_ImageRenderer"` | 图片渲染器 |
| `PreviousButtonPart` | `"PART_PreviousButton"` | 上一张按钮 |
| `NextButtonPart` | `"PART_NextButton"` | 下一张按钮 |
| `ScaleDownButtonPart` | `"PART_ScaleDownButton"` | 缩小按钮 |
| `ScaleUpButtonPart` | `"PART_ScaleUpButton"` | 放大按钮 |
| `FitToWindowButtonPart` | `"PART_FitToWindowButton"` | 适应窗口/原始尺寸切换按钮 |
| `HorizontalFlipButtonPart` | `"PART_HorizontalFlipButton"` | 水平翻转按钮 |
| `VerticalFlipButtonPart` | `"PART_VerticalFlipButton"` | 垂直翻转按钮 |
| `RotateLeftButtonPart` | `"PART_RotateLeftButton"` | 左旋转按钮 |
| `RotateRightButtonPart` | `"PART_RotateRightButton"` | 右旋转按钮 |
