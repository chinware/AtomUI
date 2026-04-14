# Tour 漫游式引导

## 概述

漫游式引导（Tour）用于分步引导用户了解产品功能。它以气泡卡片的形式，依次聚焦到页面上的各个功能点，配合标题、描述、封面图等内容进行逐步讲解。当用户完成所有步骤或主动关闭时，引导消失。

AtomUI 的 `Tour` 控件复刻了 [Ant Design 5.0 Tour](https://ant.design/components/tour-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 Tour 设计哲学

Ant Design 对 Tour 的定位是：**「用于分步引导用户了解产品功能的组件，通过气泡卡片聚焦页面功能点，进行逐步讲解」**。Tour 的核心价值在于降低新用户的认知负荷——通过将复杂界面拆解为有序的引导步骤，帮助用户快速理解核心功能。

**两种视觉风格**：

| 风格 | 设计意图 | 典型用途 |
|---|---|---|
| ⚪️ **Default** | 白色背景卡片，低视觉侵入性，适合常规场景 | 产品功能介绍、新手引导 |
| 🔵 **Primary** | 主色调背景卡片，高视觉权重，适合需要强调的场景 | 重要功能高亮、非模态引导（无遮罩时更醒目） |

**关键设计决策**：

| 特性 | 设计意图 |
|---|---|
| **遮罩层（Mask）** | 模态引导时使用半透明遮罩覆盖全屏，聚焦用户注意力到目标控件上，目标区域镂空显示 |
| **箭头指向** | 气泡卡片带指向箭头，明确引导内容与目标控件的关联关系 |
| **步骤指示器** | 底部圆点/文字指示器展示当前进度，帮助用户了解引导流程的整体长度和当前位置 |
| **上一步/下一步导航** | 标准的分步导航按钮，用户可自由前进/后退 |
| **封面图支持** | 每个步骤可附带封面图，增强引导的视觉表现力 |
| **12种弹出位置** | 支持 Top/Bottom/Left/Right 及对齐变体，自动适配页面布局 |

### AtomUI 的实现架构

AtomUI `Tour` 在 Avalonia 的 `TemplatedControl` 基础上，利用 `Popup` 弹出层机制和自定义 `TourLayer` 遮罩层实现完整的漫游引导功能。

| 组成部分 | 说明 |
|---|---|
| **Tour** | 主控件，管理步骤集合、弹出/关闭逻辑、遮罩层和 Popup 定位 |
| **TourStep** | 单个引导步骤控件，承载标题、描述、封面、关闭按钮等内容 |
| **TourStepsView** | 内部选择控件，管理步骤的切换、导航按钮、指示器展示 |
| **TourLayer** | 全屏遮罩层，使用 `CombinedGeometry` 排除法实现目标区域镂空 |
| **TourIndicator** | 步骤指示器抽象基类 |
| **DefaultTourIndicator** | 默认圆点指示器，通过自定义绘制展示进度 |
| **TextTourIndicator** | 文字指示器（"1 / 3" 格式） |
| **ITourAction** | 自定义操作按钮接口，允许扩展导航区域 |

**核心机制**：
- Tour 通过 `Popup` 控件将引导卡片弹出到目标控件附近
- 通过 `TourLayer` 在 `VisualLayerManager` 上叠加半透明遮罩，并使用 `CombinedGeometry.Exclude` 镂空目标区域
- 当无 `Target` 时，气泡卡片居中显示在屏幕中央
- 步骤切换时动态更新 `Popup.PlacementTarget`、箭头方向和遮罩镂空区域

---

## 功能详解

### 步骤定义

每个引导步骤通过 `TourStep` 定义，支持以下内容：
- **Title**：步骤标题
- **Description**：步骤描述文字
- **Cover**：封面图内容（可以是 Image 或任意控件）
- **Target**：目标控件引用（通过 `ElementName` 绑定）

步骤可以直接在 `Tour.Steps`（Content 属性）中声明，也可以通过 `StepsSource` 属性绑定数据源（配合 `ITourStepOption` 接口和 `ItemTemplate`）。

### 风格类型（StyleType）

Tour 支持两种视觉风格，通过 `StyleType` 属性全局设置，也可在每个 `TourStep` 上单独覆盖：

| 风格 | 背景色 | 文字色 | 导航按钮样式 |
|---|---|---|---|
| `Default` | 白色（`ColorBgElevated`） | 默认文字色 | 标准 Button |
| `Primary` | 主色调（`ColorPrimary`） | 白色（`ColorTextLightSolid`） | 反色按钮 |

### 弹出位置（Placement）

支持 13 种弹出位置，通过 `TourPlacementMode` 枚举控制：

| 值 | 说明 |
|---|---|
| `Center` | 屏幕居中（无 Target 时自动使用） |
| `Top` / `TopLeft` / `TopRight` | 目标上方 |
| `Bottom` / `BottomLeft` / `BottomRight` | 目标下方（默认） |
| `Left` / `LeftTop` / `LeftBottom` | 目标左侧 |
| `Right` / `RightTop` / `RightBottom` | 目标右侧 |

当步骤未指定 `Target` 时，无论 `Placement` 设置如何，气泡卡片都会居中显示，且箭头自动隐藏。

### 遮罩层（Mask）

- `IsShowMask=True`（默认）：显示半透明遮罩，目标区域镂空
- `IsShowMask=False`：无遮罩，适合非模态引导（建议配合 `StyleType=Primary` 使用以增强醒目度）
- `MaskColor`：自定义遮罩颜色（默认使用 `SharedToken.ColorBgMask`）
- 每个 `TourStep` 可单独覆盖 `IsShowMask` 和 `MaskColor`

### 高亮区域（Gap）

遮罩层镂空的目标区域可以通过以下属性微调：
- `GapOffsetX` / `GapOffsetY`：镂空区域相对目标控件的水平/垂直外扩偏移（默认 6px）
- `GapRadius`：镂空区域的圆角半径（默认 2px）

### 箭头

- `IsShowArrow=True`（默认）：显示指向目标的箭头
- `IsPointAtCenter`：箭头是否指向目标中心（需配合 `IsShowArrow`）
- 每个 `TourStep` 可单独覆盖箭头设置

### 步骤指示器（Indicator）

Tour 支持两种内置指示器，也支持自定义：

| 指示器 | 说明 |
|---|---|
| `DefaultTourIndicator` | 圆点指示器（默认），当前步骤高亮 |
| `TextTourIndicator` | 文字指示器，显示 "当前/总数" 格式 |

通过 `Tour.Indicator` 属性设置：

```xml
<atom:Tour>
    <atom:Tour.Indicator>
        <atom:TextTourIndicator />
    </atom:Tour.Indicator>
    ...
</atom:Tour>
```

### 自定义操作按钮（CustomActions）

通过 `Tour.CustomActions` 集合可以在导航区域添加自定义按钮（如"跳过"按钮）。自定义按钮需实现 `ITourAction` 接口以接收步骤信息同步：

```csharp
public interface ITourAction
{
    int StepCount { get; set; }
    int ActiveIndex { get; set; }
    TourStyleType StyleType { get; set; }
    void NotifyAttached(Tour tour) {}
}
```

### 本地化

Tour 的导航按钮文本通过本地化系统管理：

| 资源键 | 中文 | 英文 |
|---|---|---|
| `Previous` | 上一步 | Previous |
| `Next` | 下一步 | Next |
| `Finish` | 结束导览 | Finish |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本引导 | ✅ 步骤分步引导 | ✅ `TourStep` 集合 | ✅ 完全对齐 |
| 两种风格 `type` | ✅ `default / primary` | ✅ `TourStyleType` 枚举 | ✅ 完全对齐 |
| 遮罩 `mask` | ✅ 布尔/配置对象 | ✅ `IsShowMask` + `MaskColor` | ✅ 完全对齐 |
| 12种弹出位置 `placement` | ✅ 12 个方向 + center | ✅ `TourPlacementMode` 枚举（13个值） | ✅ 完全对齐 |
| 自定义指示器 `indicatorsRender` | ✅ 自定义渲染 | ✅ `TourIndicator` 抽象基类 | ✅ 完全对齐 |
| 箭头 `arrow` | ✅ 布尔/`{ pointAtCenter }` | ✅ `IsShowArrow` + `IsPointAtCenter` | ✅ 完全对齐 |
| 封面图 `cover` | ✅ ReactNode | ✅ `Cover` + `CoverTemplate` | ✅ 完全对齐 |
| 标题/描述 | ✅ `title` / `description` | ✅ `Title` + `TitleTemplate` / `Description` + `DescriptionTemplate` | ✅ 完全对齐 |
| 关闭按钮 `closeIcon` | ✅ 自定义图标 | ✅ `CloseIcon` 属性 | ✅ 完全对齐 |
| 高亮区域 `gap` | ✅ `{ offset, radius }` | ✅ `GapOffsetX` / `GapOffsetY` / `GapRadius` | ✅ 完全对齐 |
| 滚动到视图 `scrollIntoViewOptions` | ✅ 自动滚动 | ✅ `IsScrollIntoView` 布尔 | ⚠️ 仅支持开关，不支持滚动选项 |
| 禁用交互 `disabledInteraction` | ✅ 布尔属性 | ✅ `IsDisabledInteraction` | ✅ 完全对齐 |
| `onChange` / `onClose` | ✅ 回调函数 | ✅ 通过 `CurrentIndex` / `IsOpen` 绑定 | ✅ 对齐（MVVM 模式） |
| `current` 受控当前步骤 | ✅ 受控属性 | ✅ `CurrentIndex` 双向绑定 | ✅ 完全对齐 |
| `open` 受控显隐 | ✅ 受控属性 | ✅ `IsOpen` 双向绑定 | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Tour
        └── implements IMotionAwareControl
```

### 相关辅助类继承关系

```
Avalonia.Controls.ContentControl
  └── AtomUI.Desktop.Controls.TourStep
        └── implements ITourStepOption

Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.TourIndicator (abstract)
        ├── implements IMotionAwareControl
        ├── AtomUI.Desktop.Controls.DefaultTourIndicator
        └── AtomUI.Desktop.Controls.TextTourIndicator

Avalonia.Controls.Primitives.SelectingItemsControl
  └── AtomUI.Desktop.Controls.TourStepsView (internal)

Avalonia.Controls.Control
  └── AtomUI.Desktop.Controls.TourLayer (internal)
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化呈现、主题/样式支持 |
| `Tour` | 步骤集合管理、Popup 弹出定位、遮罩层控制、箭头方向计算、步骤切换逻辑、指示器绑定、自定义操作按钮 |
| `TourStep` | 单步骤内容呈现（标题、描述、封面、关闭按钮）、per-step 属性覆盖 |
| `TourStepsView` | 步骤选择/切换、导航按钮（上一步/下一步/完成）、指示器展示、自定义操作集成 |
| `TourLayer` | 全屏遮罩绘制、目标区域镂空（CombinedGeometry.Exclude） |
| `TourIndicator` | 步骤进度指示（圆点/文字） |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 动画开关 |
| `ITourStepOption` | `AtomUI.Desktop.Controls` | 步骤选项数据接口，用于数据绑定场景 |
| `ITourAction` | `AtomUI.Desktop.Controls` | 自定义操作按钮接口，用于扩展导航区域 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Tour/Tour.cs` | Tour 主控件 |
| 控件类 | `src/AtomUI.Desktop.Controls/Tour/TourStep.cs` | 单步骤控件 |
| 控件类 | `src/AtomUI.Desktop.Controls/Tour/TourStepsView.cs` | 步骤视图（内部） |
| 控件类 | `src/AtomUI.Desktop.Controls/Tour/TourIndicator.cs` | 指示器抽象基类 |
| 控件类 | `src/AtomUI.Desktop.Controls/Tour/DefaultTourIndicator.cs` | 默认圆点指示器 |
| 控件类 | `src/AtomUI.Desktop.Controls/Tour/TextTourIndicator.cs` | 文字指示器 |
| 控件类 | `src/AtomUI.Desktop.Controls/Tour/TourLayer.cs` | 遮罩层（内部） |
| 接口 | `src/AtomUI.Desktop.Controls/Tour/ITourAction.cs` | 自定义操作按钮接口 |
| 接口 | `src/AtomUI.Desktop.Controls/Tour/TourStepOption.cs` | 步骤选项接口与 POCO 实现 |
| 枚举 | `src/AtomUI.Desktop.Controls/Tour/TourPlacementMode.cs` | 弹出位置枚举 |
| 事件参数 | `src/AtomUI.Desktop.Controls/Tour/TourStepNavRequestEventArgs.cs` | 导航请求事件参数 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Tour/TourToken.cs` | 组件级 Design Token |
| 本地化 | `src/AtomUI.Desktop.Controls/Tour/Localization/zh_CN.cs` | 中文语言资源 |
| 本地化 | `src/AtomUI.Desktop.Controls/Tour/Localization/en_US.cs` | 英文语言资源 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Tour/Themes/TourTheme.axaml` | Tour 主控件 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Tour/Themes/TourStepTheme.axaml` | TourStep ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Tour/Themes/TourStepsViewTheme.axaml` | TourStepsView ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Tour/Themes/DefaultTourIndicatorTheme.axaml` | 默认指示器 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Tour/Themes/TextTourIndicatorTheme.axaml` | 文字指示器 ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Tour/Themes/TourThemes.axaml` | 主题资源合并 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TourShowCase.axaml` | 使用范例 |

---

## 模板结构

### Tour 模板

Tour 本身的模板非常简洁，核心是一个 Popup + ArrowDecoratedBox + TourStepsView 的组合：

```
Popup (PART_Popup)                          ← 弹出层，定位到目标控件附近
└── ArrowDecoratedBox (PART_ArrowDecorator)  ← 带箭头的装饰容器
    └── TourStepsView (StepsView)            ← 步骤内容视图
```

同时，Tour 还在 `VisualLayerManager` 上注册了一个 `TourLayer`，用于渲染全屏遮罩。

### TourStep 模板

```
Border#Frame                              ← 主框架
└── DockPanel#RootLayout                  ← 根布局
    ├── DockPanel (DockPanel.Dock=Top)    ← 标题行
    │   ├── DialogCaptionButton#CloseButton (Dock=Right)  ← 关闭按钮
    │   └── ContentPresenter#Title        ← 标题内容
    └── StackPanel#ContentLayout          ← 内容区域
        ├── ContentPresenter#CoverPresenter    ← 封面图
        └── ContentPresenter#DescriptionPresenter  ← 描述文字
```

### TourStepsView 模板

```
Border#Frame                              ← 主框架
└── DockPanel#RootLayout                  ← 根布局
    ├── Border#FooterFrame (Dock=Bottom)  ← 底部导航栏
    │   └── DockPanel                     ← 导航布局
    │       ├── ContentPresenter#IndicatorPresenter (Dock=Left)  ← 步骤指示器
    │       └── Panel                     ← 操作按钮区域
    │           └── StackPanel#ActionsLayout (HAlign=Right)
    │               ├── [CustomActions...]  ← 自定义操作按钮（插入到前面）
    │               ├── Button#PreviousButton  ← 上一步
    │               ├── Button#NextButton      ← 下一步
    │               └── Button#FinishButton    ← 完成
    └── ItemsPresenter                    ← 步骤内容（只显示选中项）
```

**导航按钮可见性逻辑**（通过 `IndexPosition` 内部属性驱动）：

| 步骤位置 | PreviousButton | NextButton | FinishButton | IndicatorPresenter |
|---|---|---|---|---|
| `OnePage`（仅一步） | 隐藏 | 隐藏 | 显示 | 隐藏 |
| `First`（第一步） | 隐藏 | 显示 | 隐藏 | 显示 |
| `Middle`（中间步骤） | 显示 | 显示 | 隐藏 | 显示 |
| `Last`（最后一步） | 显示 | 隐藏 | 显示 | 显示 |
