# Spin 加载中

## 概述

加载中（Spin）用于页面和区域的加载状态展示。当页面局部处于等待异步数据或操作结果时，合适的加载动效能有效缓解用户的焦虑感，同时暗示用户「系统正在工作」。

Spin 由两个控件协作完成：
- **`Spin`**：容器控件，包裹内容区域，管理加载状态切换、遮罩效果、提示文字
- **`SpinIndicator`**：纯指示器控件，负责旋转动画的渲染（四点旋转或自定义指示器）

两者均可独立使用：`SpinIndicator` 适合只需展示旋转指示器的场景；`Spin` 适合需要遮罩内容区域的容器场景。

AtomUI 的 `Spin` 控件复刻了 [Ant Design 5.0 Spin](https://ant.design/components/spin-cn) 的设计规范。

---

## 设计原理

### Ant Design 的加载设计哲学

Ant Design 对 Spin 的定位是：**「用于页面和区块的加载中状态」**。核心设计原则：

1. **即时反馈** — 操作发起后立即展示加载状态，让用户确认系统已响应。相比空白等待，旋转动画传达了「正在处理」的明确信号。
2. **非阻塞** — 加载时内容区域仍可见（半透明遮罩），让用户了解即将加载的内容，降低焦虑感。
3. **可定制** — 支持自定义指示器、提示文字、三种预设尺寸，适应不同的页面区域和语义场景。

**Ant Design 指示器视觉规范：**

Ant Design 的 Spin 默认指示器是一个旋转的四圆点图案——四个圆点呈十字形分布，在旋转过程中通过正弦波函数控制每个圆点的透明度，形成「追光」效果。这种设计比简单的旋转圆圈更具节奏感，视觉上更加柔和。

### Avalonia ContentControl 基础能力

`AbstractSpin` 继承自 Avalonia 的 `ContentControl`，这使得 Spin 天然具备容器能力：它可以通过 `Content` 属性包裹任意子控件。当 `IsSpinning = true` 时，子控件上方叠加加载遮罩和旋转指示器。

**ContentControl 提供的基础设施：**

| 能力 | 说明 |
|---|---|
| `Content` / `ContentTemplate` | 容纳任意内容，模板化呈现 |
| `ContentPresenter` | 在 ControlTemplate 中呈现 Content |
| 逻辑树管理 | Content 自动加入逻辑树 |

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种尺寸** | `SizeType`（Small / Middle / Large） | 适应不同区域大小：文本加载用 Small，卡片用 Middle，页面用 Large |
| **容器遮罩** | `Content` + `MaskOpacity` + `IsMaskBlurEnabled` | 加载时半透明或模糊遮罩内容 |
| **提示文字** | `Tip` + `IsShowTip` | 在指示器下方展示加载提示，增强信息反馈 |
| **自定义指示器** | `CustomIndicator` / `CustomIndicatorTemplate` | 替换默认旋转点为自定义动画（如 LoadingOutlined 图标） |
| **遮罩背景** | `IsMaskBackgroundEnabled` | 加载时可选显示半透明背景遮罩（`ColorBgMask`） |
| **模糊效果** | `IsMaskBlurEnabled` | 加载时内容区域启用高斯模糊（Radius=5），替代降低透明度 |
| **动画控制** | `IsMotionEnabled` / `MotionDuration` / `MotionEasingCurve` | 控制旋转动画和过渡动画行为 |
| **Design Token** | `SpinToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 派生，支持主题切换 |

---

## 功能详解

### 独立模式 vs 容器模式

**独立模式**（`SpinIndicator`）：不设置 `Content`，单独展示旋转指示器。适用于在布局中嵌入小型加载指示。

```xml
<!-- 独立模式 -->
<atom:SpinIndicator SizeType="Middle" />
```

**容器模式**（`Spin`）：设置 `Content` 后，Spin 包裹内容区域：
- `IsSpinning = false`：正常显示内容，`MaskOpacity = 1.0`，遮罩层隐藏
- `IsSpinning = true`：遮罩层可见，内容区域变半透明（`MaskOpacity` 降至 `0.5`），上方叠加指示器 + 可选提示文字

```xml
<!-- 容器模式 -->
<atom:Spin IsSpinning="True" Tip="Loading..." IsShowTip="True">
    <atom:Alert Message="Alert message title" Type="Info" />
</atom:Spin>
```

### SpinIndicator 渲染算法

`SpinIndicator`（通过 `AbstractSpinIndicator`）使用自定义渲染实现四圆点旋转动画。其核心算法如下：

1. **四点布局**：四个圆点分别位于指示器区域的上、右、下、左四个方位，以中心点为参考对称分布。
2. **旋转变换**：通过 `IndicatorAngle` 属性（0°→360° 无限循环）控制整个绘制区域的旋转角度。旋转使用 `Matrix.CreateRotation` 以指示器中心为轴心。
3. **正弦波透明度**：每个圆点的透明度由正弦波函数计算，公式为：
   ```
   opacity = DOT_START_OPACITY + (1 - DOT_START_OPACITY) × (sin(θ + offset) + 1) / 2
   ```
   其中 `DOT_START_OPACITY = 0.3`，四个圆点的 `offset` 分别为 0°、90°、180°、270°，产生依次明灭的「追光」效果。
4. **动画驱动**：使用 Avalonia `Animation` API 驱动 `IndicatorAngle` 从 0° 到 360° 的无限循环，缓动曲线默认为 `LinearEasing`（匀速旋转）。

**自定义指示器**：当设置 `CustomIndicator` 或 `CustomIndicatorTemplate` 时，内置的四圆点渲染被跳过，改为展示自定义内容。自定义指示器同样应用旋转变换（通过 `RenderTransform` 设置 `RotateTransform`），保持旋转行为一致。

### 尺寸（SizeType）

Spin 指示器的尺寸由 `SizeType` 控制，同时影响圆点大小和指示器整体大小：

| 尺寸 | 指示器大小 Token | 圆点大小 Token | 适用场景 |
|---|---|---|---|
| `Small` | `IndicatorSizeSM` | `DotSizeSM` | 文本级加载、行内指示 |
| `Middle`（默认） | `IndicatorSize` | `DotSize` | 卡片级加载、区块指示 |
| `Large` | `IndicatorSizeLG` | `DotSizeLG` | 页面级加载、全屏指示 |

当使用自定义指示器（如 `PathIcon` 或 `Icon`）时，`SpinIndicator` 通过实例级样式根据 `SizeType` 自动调整图标尺寸（`Width` / `Height`）为对应的 `IndicatorSize` Token 值。

### 提示文字

设置 `Tip` 和 `IsShowTip = true` 后，旋转指示器下方显示提示文字（颜色为 `ColorPrimary`），两者之间的间距为 `SpacingXXS`。提示文字使用 `atom:TextBlock` 渲染，自动跟随主题字体。

### 遮罩效果

容器模式下的遮罩行为由三个属性协同控制：

| 属性 | 效果 | 优先级 |
|---|---|---|
| `IsMaskBlurEnabled = true` | 内容区域启用高斯模糊（`BlurEffect Radius=5`），不降低透明度 | 与透明度互斥 |
| `IsMaskBlurEnabled = false`（默认） | 内容区域透明度降至 `0.5` | 默认行为 |
| `IsMaskBackgroundEnabled = true` | 遮罩层显示半透明背景（`ColorBgMask`） | 可与上述两种叠加 |

当 `IsMotionEnabled = true` 时，`MaskOpacity` 的变化使用 `DoubleTransition` 平滑过渡，避免突兀的透明度跳变。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 三种尺寸 `size` | ✅ `small` / `default` / `large` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 加载状态 `spinning` | ✅ 布尔属性 | ✅ `IsSpinning` 属性 | ✅ 完全对齐 |
| 提示文字 `tip` | ✅ `ReactNode` | ✅ `Tip` 字符串 + `IsShowTip` | ✅ 完全对齐 |
| 容器模式 `children` | ✅ 包裹子元素 | ✅ `Content` 属性（继承自 ContentControl） | ✅ 完全对齐 |
| 自定义指示器 `indicator` | ✅ `ReactNode` | ✅ `CustomIndicator` / `CustomIndicatorTemplate` | ✅ 完全对齐 |
| 全屏加载 `fullscreen` | ✅ 布尔属性 | ❌ 暂未支持 | ⚠️ 可通过布局实现 |
| 延迟显示 `delay` | ✅ 毫秒数 | ❌ 暂未支持 | ⚠️ 待支持 |
| 模糊遮罩 | ❌ 无 | ✅ `IsMaskBlurEnabled` | 🆕 AtomUI 扩展 |
| 遮罩背景 | ❌ 无 | ✅ `IsMaskBackgroundEnabled` | 🆕 AtomUI 扩展 |
| 独立指示器组件 | ❌ 内嵌 | ✅ `SpinIndicator` 可独立使用 | 🆕 AtomUI 扩展 |

---

## 继承关系

### Spin（容器控件）

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Controls.Commons.AbstractSpin      ← 设备无关基类（AtomUI.Controls）
              ├── implements IMotionAwareControl
              └── AtomUI.Desktop.Controls.Spin         ← 桌面端具体实现
```

### SpinIndicator（指示器控件）

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.Commons.AbstractSpinIndicator   ← 设备无关基类（AtomUI.Controls）
        ├── implements ISizeTypeAware
        └── AtomUI.Desktop.Controls.SpinIndicator      ← 桌面端具体实现
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现 |
| `AbstractSpin`（基类层） | `SizeType` 尺寸、`IsSpinning` 加载控制、`Tip` / `IsShowTip` 提示文字、`CustomIndicator` / `CustomIndicatorTemplate` 自定义指示器、`IsMaskBlurEnabled` / `IsMaskBackgroundEnabled` 遮罩控制、`IsMotionEnabled` / `MotionDuration` / `MotionEasingCurve` 动画控制、`MaskOpacity` 过渡管理 |
| `Spin`（桌面层） | 注册 `SpinToken.ScopeProvider`，由桌面主题控制视觉表现 |
| `AbstractSpinIndicator`（基类层） | `SizeType` 尺寸、`CustomIndicator` / `CustomIndicatorTemplate` 自定义指示器、`MotionDuration` / `MotionEasingCurve` 动画参数、四圆点渲染算法（`Render` 方法）、旋转动画驱动（`Animation` + `IndicatorAngle`）、`DotSize` / `DotBgBrush` 渲染属性 |
| `SpinIndicator`（桌面层） | 注册 `SpinToken.ScopeProvider`、配置实例级样式（自定义指示器图标尺寸随 `SizeType` 变化） |

**实现的共享接口：**

| 接口 | 定义位置 | 控件 | 作用 |
|---|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | `AbstractSpin` | 支持 `IsMotionEnabled` 动画开关 |
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | `AbstractSpinIndicator` | 支持 `SizeType`（Small / Middle / Large）三种尺寸 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| Spin 基类 | `src/AtomUI.Controls/Spin/AbstractSpin.cs` | 设备无关的容器基类 |
| SpinIndicator 基类 | `src/AtomUI.Controls/Spin/AbstractSpinIndicator.cs` | 设备无关的指示器基类 |
| Spin 控件类 | `src/AtomUI.Desktop.Controls/Spin/Spin.cs` | 桌面端 Spin 具体实现 |
| SpinIndicator 控件类 | `src/AtomUI.Desktop.Controls/Spin/SpinIndicator.cs` | 桌面端 SpinIndicator 具体实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Spin/SpinToken.cs` | 组件级 Design Token |
| Spin 主题 | `src/AtomUI.Desktop.Controls/Spin/Themes/SpinTheme.axaml` | Spin ControlTheme |
| SpinIndicator 主题 | `src/AtomUI.Desktop.Controls/Spin/Themes/SpinIndicatorTheme.axaml` | SpinIndicator ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Spin/Themes/SpinThemes.axaml` | 合并资源字典 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SpinShowCase.axaml` | 使用范例 |

---

## 模板结构

### Spin 模板

```
Panel#RootLayout
├── ContentPresenter#ContentPresenter              ← 内容区域（Opacity 由 MaskOpacity 控制）
└── Panel#MaskLayout (IsVisible=IsSpinning)        ← 遮罩层（仅加载时可见）
    ├── Border#Mask (Background=Transparent)       ← 遮罩背景（IsMaskBackgroundEnabled 时变为 ColorBgMask）
    └── StackPanel#IndicatorLayout (Center)        ← 指示器 + 提示文字（Spacing=SpacingXXS）
        ├── Panel
        │   ├── ContentPresenter#CustomIndicator   ← 自定义指示器（IsCustomIndicator 时可见）
        │   └── SpinIndicator#Indicator            ← 默认旋转圆点（非自定义时可见）
        └── TextBlock#Tip                          ← 提示文字（IsShowTip 时可见，颜色=ColorPrimary）
```

### SpinIndicator 模板

```
ContentPresenter#PART_CustomIndicatorPresenter     ← 自定义指示器展示器
                                                      （无自定义时由 Render 方法绘制四圆点）
```

**模板设计理由：**
- **分层叠加**：`ContentPresenter` 和 `MaskLayout` 叠放在 `Panel` 中，加载时遮罩层自然覆盖内容。
- **双指示器互斥**：默认 `SpinIndicator` 和自定义 `ContentPresenter` 互斥可见，通过 `IsCustomIndicator` 切换。自定义指示器也会应用旋转动画（通过 `RenderTransform`）。
- **过渡动画**：`MaskOpacity` 属性配合 `DoubleTransition` 实现内容透明度的平滑过渡。
- **模糊与透明度互斥**：`IsMaskBlurEnabled = true` 时使用 `BlurEffect`，`false` 时降低 `MaskOpacity`。两种遮罩方式不叠加。
