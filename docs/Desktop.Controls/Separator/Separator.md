# Separator 分割线

## 概述

分割线（Separator）用于在页面内容之间创建视觉分隔，帮助用户理解内容的层级和分组关系。它可以是水平或垂直方向，水平分割线还支持在线条中间嵌入标题文本，用于内容分组和区域划分。

AtomUI 的 `Separator` 控件完整实现了 [Ant Design 5.0 Divider](https://ant.design/components/divider-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验。

---

## 设计原理

### Ant Design 的分割线设计哲学

Ant Design 对分割线的定位是：**「区隔内容的分割线」**。分割线在界面中扮演着"无声的组织者"角色——它不承载交互，但通过视觉断裂引导用户理解内容结构。Ant Design 为分割线建立了以下设计维度：

**两种方向：**

| 方向 | 设计意图 | 典型用途 |
|---|---|---|
| **水平分割线** | 在垂直布局中创建段落分隔，是最常见的使用方式。可在线条中嵌入标题文本，用于显式标注分隔含义 | 表单分组、段落分隔、内容区域划分 |
| **垂直分割线** | 在水平布局中创建行内元素分隔，高度自适应行高 | 操作链接之间的分隔符（如 `编辑 | 删除`） |

**三种线条变体：**

| 变体 | 视觉效果 | 语义 |
|---|---|---|
| **实线（Solid）** | 连续直线 | 默认、最常见的分隔方式 |
| **虚线（Dashed）** | `[4, 2]` 间隔的虚线 | 较弱的分隔暗示，视觉上更轻量 |
| **点线（Dotted）** | `[1, 1]` 间隔的点线 | 最轻量的分隔方式 |

**标题位置系统：**

| 位置 | 效果 |
|---|---|
| `Center`（默认） | 标题居中，两侧等长线条 |
| `Left` | 标题靠左，左侧短线条（或由 `OrientationMargin` 控制） |
| `Right` | 标题靠右，右侧短线条 |

### Avalonia Separator 基础能力

AtomUI 的 `Separator` 通过 `AbstractSeparator` 继承自 Avalonia 的 `Avalonia.Controls.Separator`。理解 Avalonia Separator 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia Separator 的核心职责：**

Avalonia 的 `Separator` 继承自 `TemplatedControl`，是一个简单的视觉分隔控件。它的继承链为：

```
Control → TemplatedControl → Separator
```

Avalonia 的原生 `Separator` 功能非常基础——它仅作为一个占位元素存在，具体的分割线视觉效果完全由主题模板决定。它没有内置的方向控制、标题嵌入、线条变体等能力。

**Avalonia Separator 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Margin` | 外间距，控制分割线与周围内容的距离 |
| `IsEnabled` | 是否启用 |
| `Background` | 背景色 |
| `Foreground` | 前景色 |
| `FontSize` | 字体大小（用于文本类分割线） |
| `FontWeight` | 字体粗细 |

### AtomUI 的扩展设计

AtomUI `Separator` 在 Avalonia Separator 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **两种方向** | `Orientation` 属性（Horizontal / Vertical） | 对齐 Ant Design 的水平/垂直分割线 |
| **三种线条变体** | `Variant` 枚举 + 自绘 `DashStyle` / `DotStyle` | 对齐 Ant Design 5.22+ 的 `variant` 属性 |
| **标题嵌入** | `Title` 属性 + 自绘分段线条 | 对齐 Ant Design 的 `children` 标题功能 |
| **标题位置** | `TitlePosition` 枚举（Left / Center / Right） | 对齐 Ant Design 的 `orientation` 属性 |
| **标题边距** | `OrientationMargin` + `OrientationMarginPercent` | 对齐 Ant Design 的 `orientationMargin` 属性 |
| **普通文字模式** | `IsPlain` 属性 | 对齐 Ant Design 的 `plain` 属性 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 控制水平分割线的上下间距 |
| **自定义颜色** | `LineColor` / `TitleColor` 属性 | 支持自定义分割线和标题文本颜色 |
| **自绘渲染** | `OnRender` 直接绘制线条 | 精确控制线条宽度、虚线样式和标题间距 |
| **Design Token** | `SeparatorToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 方向（Orientation）

通过 `Orientation` 属性控制分割线方向：

| 方向 | 布局行为 | 尺寸计算 |
|---|---|---|
| `Horizontal`（默认） | 水平全宽线条，可嵌入标题 | 宽度撑满可用空间，高度由线宽和标题文本高度决定 |
| `Vertical` | 垂直短线条，适用于行内分隔 | 宽度 = `LineWidth + VerticalMarginInline`，高度 = 1em（或撑满可用高度） |

AtomUI 还提供了便捷子类 `VerticalSeparator`，默认方向为垂直，无需手动设置 `Orientation`：

```csharp
public class VerticalSeparator : Separator
{
    static VerticalSeparator()
    {
        OrientationProperty.OverrideDefaultValue<VerticalSeparator>(Orientation.Vertical);
    }
    protected override Type StyleKeyOverride => typeof(Separator);
}
```

### 标题嵌入（Title）

当 `Orientation` 为水平且设置了 `Title` 属性时：
- 标题文本渲染在线条中间（或左侧/右侧，由 `TitlePosition` 控制）
- 线条在标题两侧分段绘制，中间留出 `TextPaddingInline`（1em）的间距
- 标题区域保证至少 25% 的线条比例（`SEPARATOR_LINE_MIN_PROPORTION = 0.25`），避免标题文本过长时线条完全消失
- `:has-title` 伪类自动激活

### 标题位置与边距

| 属性 | 作用 |
|---|---|
| `TitlePosition` | 标题在线条中的位置：`Center`（居中）、`Left`（偏左）、`Right`（偏右） |
| `OrientationMargin` | 标题距左/右边缘的精确像素值。设置后覆盖默认的百分比计算 |
| `OrientationMarginPercent` | Token 控制的默认百分比（0.05 = 5%），当 `OrientationMargin` 未设置（`NaN`）时生效 |

### 线条变体（Variant）

通过 `Variant` 属性控制线条样式，底层使用 Avalonia 的 `IDashStyle`：

| 变体 | DashStyle 参数 | 视觉效果 |
|---|---|---|
| `Solid`（默认） | `null` | 连续实线 |
| `Dashed` | `[4, 2]` offset=0 | 等间隔虚线 |
| `Dotted` | `[1, 1]` offset=0 | 等间隔点线 |

### 自绘渲染机制

Separator 不使用 `Border` 等控件来绘制线条，而是通过重写 `OnRender` 直接绘制。这样的设计有以下优势：
- 精确控制线条宽度（支持亚像素级别，通过 `EdgeMode.Aliased` 避免抗锯齿模糊）
- 精确控制虚线/点线样式参数
- 精确控制标题与线条之间的间距
- 避免 Border 在不同 DPI 下的亚像素对齐问题

### 尺寸（SizeType）

通过 `SizeType` 控制水平分割线的垂直间距（上下 Margin）：

| 尺寸 | Token | 效果 |
|---|---|---|
| `Small` | `HorizontalMarginBlockSM` | 紧凑间距（`UniformlyMarginXS`） |
| `Middle`（默认） | `HorizontalMarginBlock` | 标准间距（`UniformlyMargin`） |
| `Large` | `HorizontalMarginBlockLG` | 宽松间距（`UniformlyMarginLG`） |

### 普通文字模式（IsPlain）

| IsPlain | 标题字号 | 标题字重 |
|---|---|---|
| `false`（默认） | `FontSizeLG` | 500（Medium） |
| `true` | `FontSize` | Normal |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 水平分割线 | ✅ 默认 | ✅ `Orientation=Horizontal` | ✅ 完全对齐 |
| 垂直分割线 | ✅ `type="vertical"` | ✅ `Orientation=Vertical` / `VerticalSeparator` | ✅ 完全对齐 |
| 带标题 | ✅ `children` | ✅ `Title` 属性 | ✅ 完全对齐 |
| 标题位置 | ✅ `orientation` (left/right/center) | ✅ `TitlePosition` (Left/Right/Center) | ✅ 完全对齐 |
| 标题边距 | ✅ `orientationMargin` | ✅ `OrientationMargin` | ✅ 完全对齐 |
| 虚线/点线变体 | ✅ `variant` (5.22.0+) | ✅ `Variant` (Solid/Dashed/Dotted) | ✅ 完全对齐 |
| 普通文字 | ✅ `plain` | ✅ `IsPlain` | ✅ 完全对齐 |
| 尺寸 | — | ✅ `SizeType` (Small/Middle/Large) | ✅ AtomUI 扩展 |
| 自定义颜色 | ✅ `style` | ✅ `LineColor` / `TitleColor` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.Separator
        └── AtomUI.Controls.Commons.AbstractSeparator (ISizeTypeAware)  ← 设备无关基类
              └── AtomUI.Desktop.Controls.Separator                     ← 桌面实现
                    └── AtomUI.Desktop.Controls.VerticalSeparator       ← 垂直分割线便捷类
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.Separator` | 基础模板化控件结构 |
| `AbstractSeparator`（基类层） | 方向、标题、标题位置、线条变体、线宽、普通文字模式、尺寸、自定义颜色、`OnRender` 自绘逻辑、`MeasureOverride` / `ArrangeOverride` 布局计算 |
| `Separator`（桌面层） | 注册 `SeparatorToken.ScopeProvider`，由主题控制间距和默认颜色 |
| `VerticalSeparator` | 仅覆盖 `Orientation` 默认值为 `Vertical`，使用 `Separator` 的 StyleKey |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸，控制水平分割线的垂直间距 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Separator/AbstractSeparator.cs` | 设备无关基类（方向、标题、自绘逻辑） |
| 枚举 | `src/AtomUI.Controls/Separator/SeparatorEnums.cs` | 枚举定义（SeparatorTitlePosition、SeparatorVariant） |
| 伪类 | `src/AtomUI.Controls/Separator/SeparatorPseudoClass.cs` | 伪类常量 |
| 控件类 | `src/AtomUI.Desktop.Controls/Separator/Separator.cs` | 桌面分割线 + VerticalSeparator |
| Token 定义 | `src/AtomUI.Desktop.Controls/Separator/SeparatorToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Separator/Themes/SeparatorTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Separator/Themes/SeparatorTheme.cs` | 主题 Code-behind |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml` | 使用范例 |

---

## 模板结构

Separator 的 ControlTemplate 非常简洁，因为主要的线条绘制通过 `OnRender` 自绘完成：

```
TemplatedControl (Separator 自身)
  └── TextBlock#PART_Title (标题文本，仅水平分割线 + 有标题时可见)
```

**自绘设计理由：**
- **线条不使用 Border**：Separator 通过 `DrawingContext.DrawLine()` 直接绘制线条，可以精确控制线宽、虚线参数和抗锯齿模式（`EdgeMode.Aliased`），避免 Border 在不同 DPI 下的亚像素对齐问题。
- **标题通过 ArrangeOverride 定位**：当有标题时，`TextBlock#PART_Title` 通过 `ArrangeOverride` 手动定位到计算出的标题区域，线条在 `OnRender` 中在标题两侧分段绘制。

### 模板部件

| 部件名 | 控件类型 | 说明 |
|---|---|---|
| `PART_Title` | `TextBlock` | 标题文本，仅在 `Orientation=Horizontal` 且 `Title` 不为空时可见 |
