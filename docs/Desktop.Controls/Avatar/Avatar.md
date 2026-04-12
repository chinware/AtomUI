# Avatar 头像

## 概述

头像（Avatar）用于代表用户或事物，支持图标、图片（位图/SVG）、文字三种展现形式。当文字内容超出容器宽度时，控件会根据 `Gap` 属性自动缩放文字以适应容器。可单独使用，也可通过 `AvatarGroup` 组合展示多个头像，并支持折叠超出数量的头像。

AtomUI 的 `Avatar` 控件完整复刻了 [Ant Design 5.0 Avatar](https://ant.design/components/avatar-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的头像设计哲学

Ant Design 对头像的定位是：**「用来代表用户或事物，支持图片、图标或字符展示」**。为了在不同场景下灵活展示用户或实体信息，Ant Design 建立了以下设计原则：

**多种内容形式**（按优先级排列）：

| 类型 | 设计意图 | 典型用途 |
|---|---|---|
| 🖼️ **图片头像** | 展示用户真实照片或实体图片，最直观的身份标识 | 用户头像、产品图片 |
| 🎨 **图标头像** | 当无用户照片时使用默认图标占位，传达"此处是人/事物"的语义 | 默认头像、匿名用户 |
| 🔤 **文字头像** | 使用用户姓名首字母或缩写，在无照片时提供个性化标识 | 用户名首字母、团队缩写 |

**两种形状**：

| 形状 | 设计意图 |
|---|---|
| ⭕ **圆形（Circle）** | 默认形状，柔和的圆形边缘传达"人"的语义，最常用于用户头像 |
| 🟦 **方形（Square）** | 带圆角的方形，适合代表应用、组织、产品等"非人"实体 |

**统一尺寸系统**：

| 尺寸 | 设计意图 |
|---|---|
| 🔸 **大（Large）** | 突出显示，用于个人资料页、卡片头部 |
| 🔹 **中（Middle）** | 默认尺寸，适用于列表、评论区 |
| 🔻 **小（Small）** | 紧凑场景，如表格行内、标签旁 |
| 📐 **自定义** | 数字像素值，满足特殊布局需求 |

**分组展示**：头像组（Avatar.Group）支持将多个头像重叠排列，超出最大显示数的头像折叠为 `+N` 计数头像，悬浮或点击可展开查看。

### Avalonia TemplatedControl 基础能力

AtomUI 的 `Avatar` 基类 `AbstractAvatar` 继承自 Avalonia 的 `TemplatedControl`。与 Button 继承自 `ContentControl` 不同，Avatar 是一个轻量级的模板化控件，不使用通用的 `ContentPresenter` 机制，而是在模板中预置了四种内容展示器（图标、位图、SVG、文字），通过内部状态 `ContentType` 切换可见性。

**TemplatedControl 提供的基础能力：**

| 能力 | 说明 |
|---|---|
| `Template` | 支持通过 `ControlTheme` 定义视觉模板 |
| `Background` / `Foreground` | 背景色和前景色 |
| `BorderBrush` / `BorderThickness` | 边框颜色和厚度 |
| `CornerRadius` | 圆角半径 |
| `FontSize` / `FontFamily` | 字体设置（影响文字头像） |
| `Width` / `Height` | 控件尺寸 |
| `Padding` | 内间距 |

### AtomUI 的扩展设计

AtomUI `Avatar` 在 TemplatedControl 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **四种内容类型** | `Icon` / `BitmapSrc` / `Src` / `Text` 四个属性 + `ContentType` 状态机 | 支持 PathIcon、Bitmap 图片、SVG 图片、文字四种内容，内部自动检测优先级 |
| **两种形状** | `AvatarShape` 枚举 + 代码动态计算 `CornerRadius` | 对齐 Ant Design 的 `shape`，圆形通过 `Width/2` 实现完全圆角 |
| **三种预设 + 自定义尺寸** | `CustomizableSizeType` 枚举 + `Size` 属性 | 比标准 `SizeType` 多一个 `Custom` 值，设置 `Size` 后自动切换 |
| **文字自适应缩放** | `TextRenderTransform` + `Gap` 属性 | 当文字宽度超出容器（减去 Gap 间距）时，自动缩小文字保证完整显示 |
| **头像组** | `AvatarGroup` 容器 + 自定义 Measure/Arrange | 支持重叠排列、最大显示数、折叠弹出（Hover/Click 触发） |
| **属性级联** | AvatarGroup → 子 Avatar 属性绑定 | 组统一设置 Shape/Size/SizeType，自动传递给所有子头像 |
| **Design Token** | `AvatarToken` + `RegisterTokenResourceScope` | 所有视觉值（尺寸、颜色、间距）从全局 Token 派生，支持主题切换 |
| **过渡动画** | `IMotionAwareControl` 接口 | 支持全局动画开关 |

---

## 功能详解

### 内容类型检测（ContentType）

Avatar 支持四种内容类型，通过内部状态 `ContentType` 驱动模板中各展示器的可见性。检测按以下优先级自动进行：

```
Src (不为 null) → SvgImage
  ↓ 否
BitmapSrc (不为 null) → BitmapImage
  ↓ 否
Text (不为 null) → Text
  ↓ 否
默认 → Icon
```

**背景色差异**：
- `Icon` 和 `Text` 类型使用 `AvatarBg`（默认灰色占位背景）
- `BitmapImage` 和 `SvgImage` 类型使用透明背景（图片自身提供视觉内容）

### 文字自适应缩放

当 `ContentType = Text` 时，控件会自动计算文字是否超出容器宽度，并在必要时缩小文字：

1. 计算文字实际渲染宽度 `textWidth`
2. 计算可用宽度 `availableWidth = Width - Gap × 2`
3. 计算缩放比 `scale = min(availableWidth / textWidth, 1.0)`
4. 如果 `scale < 1.0`，应用 `Scale + Translate` 变换使文字居中缩小

`Gap` 属性控制文字距离左右两侧的像素间距（默认 4px），值越大留白越多，文字可用空间越小。

### 自定义尺寸机制

Avatar 使用 `CustomizableSizeType` 枚举，比标准 `SizeType` 多一个 `Custom` 值：

| 操作 | 效果 |
|---|---|
| 设置 `Size = 64` | `SizeType` 自动切换为 `Custom`，`Width = Height = 64` |
| 设置 `Size = NaN`（恢复） | `SizeType` 还原为之前的值（Large/Middle/Small） |
| 设置 `SizeType = Large` | 使用 Token 定义的大号尺寸 |

当 `SizeType = Custom` 时，图标尺寸为 `Size / 2`（有 Icon 或 Src 时）或固定 18px（文字模式）。

### 圆角计算

| 形状 | 圆角方式 |
|---|---|
| `Circle` | 代码动态设置 `CornerRadius = Width / 2`（完全圆角） |
| `Square` + `Large` | 使用 `SharedToken.BorderRadiusLG` |
| `Square` + `Middle` | 使用 `SharedToken.BorderRadius` |
| `Square` + `Small` | 使用 `SharedToken.BorderRadiusSM` |
| `Square` + `Custom` | 使用 `SharedToken.BorderRadius` |

### AvatarGroup 头像组

AvatarGroup 是一个自定义布局容器，实现了头像的重叠排列和折叠显示。

**重叠布局算法**：
- `MeasureOverride`: `totalWidth = count × childWidth - (count - 1) × GroupOverlapping`
- `ArrangeOverride`: 逐个排列，每个头像偏移 `childWidth - GroupOverlapping`

**折叠机制**：
1. 当 `MaxDisplayCount` 有值且子 Avatar 数量超出时，创建一个显示 `+N` 的折叠计数头像
2. 折叠计数头像包裹在 `FlyoutHost` 中，Flyout 内的 StackPanel 包含被折叠的 Avatar
3. `FoldAvatarFlyoutTriggerType` 控制弹出方式：`Hover`（默认）或 `Click`
4. Click 模式下折叠计数头像自动切换为手形光标

**属性级联**：
AvatarGroup 通过绑定表达式将以下属性自动传递给每个子 Avatar：
- `BorderThickness`
- `IsMotionEnabled`
- `Shape`
- `Size`
- `SizeType`

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 图标头像 `icon` | ✅ ReactNode | ✅ `Icon` (PathIcon) | ✅ 完全对齐 |
| 图片头像 `src` | ✅ 字符串 URL | ✅ `Src` (SVG) / `BitmapSrc` (IImage) | ✅ 完全对齐（拆分为两个属性以区分 SVG 和 Bitmap） |
| 文字头像 | ✅ children | ✅ `Text` (string, `[Content]`) | ✅ 完全对齐 |
| 形状 `shape` | ✅ `circle` / `square` | ✅ `Shape` (AvatarShape) | ✅ 完全对齐 |
| 尺寸 `size` | ✅ 数字 / `large` / `small` / `default` | ✅ `SizeType` + `Size` | ✅ 完全对齐 |
| 文字间距 `gap` | ✅ 数字（默认 4） | ✅ `Gap` 属性（默认 4.0） | ✅ 完全对齐 |
| 头像组 `Avatar.Group` | ✅ 组容器 | ✅ `AvatarGroup` | ✅ 完全对齐 |
| 最大显示数 `maxCount` | ✅ 数字 | ✅ `MaxDisplayCount` | ✅ 完全对齐 |
| 折叠弹出 | ✅ Tooltip/Popover | ✅ `FlyoutHost` + Flyout | ✅ 完全对齐 |
| `srcSet` | ✅ 响应式图片源集 | ❌ 不适用 | — Avalonia 无此概念 |
| `alt` | ✅ 图片替代文字 | ❌ 暂未支持 | ⚠️ 无障碍待改进 |
| `onError` | ✅ 图片加载失败回调 | ❌ 暂未支持 | ⚠️ 可通过事件扩展 |
| `draggable` | ✅ 图片拖拽 | ❌ 不适用 | — 桌面端通过 DragDrop 系统处理 |
| `crossOrigin` | ✅ CORS 设置 | ❌ 不适用 | — 非 Web 平台 |

---

## 继承关系

### Avatar

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.Commons.AbstractAvatar (implements IMotionAwareControl)
        └── AtomUI.Desktop.Controls.Avatar
```

`Avatar` 的桌面端实现极为简洁（仅 12 行），所有行为均定义在跨平台基类 `AbstractAvatar` 中，桌面端仅注册 Token Scope：

```csharp
public class Avatar : AbstractAvatar
{
    public Avatar()
    {
        this.RegisterTokenResourceScope(AvatarToken.ScopeProvider);
    }
}
```

### AvatarGroup

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.AvatarGroup (implements IMotionAwareControl)
```

`AvatarGroup` 不继承自 `AbstractAvatar`，而是一个独立的 `TemplatedControl`，通过自定义的 `MeasureOverride` / `ArrangeOverride` 实现重叠布局。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化呈现、Background/Foreground/BorderBrush 基础视觉属性 |
| `AbstractAvatar`（基类层） | 多内容类型支持（Icon/Bitmap/SVG/Text）、内容类型自动检测、文字自适应缩放、自定义尺寸机制、形状圆角计算 |
| `Avatar`（桌面平台层） | 注册 `AvatarToken` Scope，接入桌面端 Design Token 系统 |
| `AvatarGroup`（桌面平台层） | 重叠布局、折叠显示、属性级联、Flyout 弹出 |

**实现的共享接口：**

| 接口 | 定义位置 | 控件 | 作用 |
|---|---|---|---|
| `IMotionAwareControl` | `AtomUI.Core` | Avatar / AvatarGroup | 全局动画开关（`IsMotionEnabled` 属性） |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Avatar/AbstractAvatar.cs` | 跨平台抽象基类（290 行） |
| 形状枚举 | `src/AtomUI.Controls/Avatar/AvatarShape.cs` | `Circle` / `Square` |
| 伪类常量 | `src/AtomUI.Controls/Avatar/AvatarPseudoClass.cs` | `:large` / `:middle` / `:small` / `:custom-size` |
| 控件类 | `src/AtomUI.Desktop.Controls/Avatar/Avatar.cs` | 桌面端 Avatar（12 行） |
| 控件类 | `src/AtomUI.Desktop.Controls/Avatar/AvatarGroup.cs` | 桌面端 AvatarGroup（350 行） |
| Token 定义 | `src/AtomUI.Desktop.Controls/Avatar/AvatarToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Avatar/Themes/AvatarTheme.axaml` | Avatar ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Avatar/Themes/AvatarGroupTheme.axaml` | AvatarGroup ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Avatar/Themes/AvatarThemes.axaml` | ResourceDictionary 合并 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/AvatarShowCase.axaml` | 使用范例 |
| Gallery ViewModel | `controlgallery/AtomUIGallery/ShowCases/ViewModels/DataDisplay/AvatarViewModel.cs` | 示例 ViewModel |

---

## 模板结构

### Avatar 模板

Avatar 的 ControlTemplate 采用扁平 Panel 布局，四种内容展示器并列放置，通过样式选择器切换可见性：

```
Panel#RootLayout
├── Border#Frame                         ← 主框架（背景色、圆角、边框）
├── IconPresenter#IconPresenter          ← 图标展示器（ContentType=Icon 时可见）
├── Image#ImagePresenter                 ← 位图展示器（ContentType=BitmapImage 时可见）
├── Svg#SvgPresenter                     ← SVG 展示器（ContentType=SvgImage 时可见）
└── TextBlock#PART_TextPresenter         ← 文字展示器（ContentType=Text 时可见，支持 RenderTransform 缩放）
```

**设计要点：**
- **内容互斥**：四种展示器默认全部 `IsVisible="False"`，仅由 `ContentType` 样式选择器激活其中一个。
- **图标/文字使用灰色背景**：`AvatarBg` Token 提供的占位背景色。
- **图片使用透明背景**：图片自身提供完整视觉内容。
- **文字缩放**：`PART_TextPresenter` 的 `RenderTransform` 绑定到 `TextRenderTransform` 属性，实现自适应缩放。
- **Frame 层独立**：`Border#Frame` 独立于内容展示器，仅承载背景和边框。

### AvatarGroup 模板

AvatarGroup 无 AXAML 模板定义——它通过代码直接操作 `LogicalChildren` 和 `VisualChildren` 集合管理子控件，并重写 `MeasureOverride` / `ArrangeOverride` 实现重叠布局。ControlTheme 仅设置默认属性值（间距、重叠宽度、折叠头像颜色、边框厚度）。

### 模板部件

| 部件名 | 类型 | 说明 |
|---|---|---|
| `PART_TextPresenter` | `TextBlock` | 文字内容展示器，基类监听其 `SizeChanged` 事件以触发缩放计算 |

