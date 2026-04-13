# Expander 展开器

## 概述

展开器（Expander）是一个可展开/收起的面板控件，用于在有限的空间中组织和隐藏内容。用户可以通过点击头部区域或展开图标来控制内容区域的显示与隐藏。Expander 本质上是 Ant Design [Collapse](https://ant.design/components/collapse-cn) 组件的单面板简化版本——当只需要一个可折叠面板时，使用 Expander 比 Collapse 更加简洁。

AtomUI 的 `Expander` 控件在 Avalonia 内置 `Expander` 的基础上进行了全面增强，支持 Ant Design 5.0 的折叠面板设计规范，包括多种展开方向、尺寸切换、无边框/幽灵模式、触发方式、自定义图标位置、附加内容区域等特性。

---

## 设计原理

### Ant Design 的折叠面板设计哲学

Ant Design 认为折叠面板是管理复杂信息架构的重要工具——它通过「渐进式披露」（Progressive Disclosure）减少用户的认知负荷：

| 设计维度 | 设计意图 |
|---|---|
| **空间效率** | 将次要信息折叠起来，让用户首先聚焦于标题摘要，按需展开查看详情 |
| **视觉层次** | 通过头部背景色、边框、间距等视觉手段建立清晰的区域划分 |
| **展开动画** | 使用平滑的展开/收起动画过渡，帮助用户理解空间变化 |
| **多种风格** | 提供默认、无边框、幽灵三种风格，适应不同的容器背景和视觉需求 |
| **灵活触发** | 支持点击头部或仅点击图标两种触发方式 |

### Avalonia Expander 基础能力

AtomUI 的 `Expander` 继承自 Avalonia 框架内置的 `Avalonia.Controls.Expander`，通过 `using` 别名避免命名冲突：

```csharp
using AvaloniaExpander = Avalonia.Controls.Expander;

public class Expander : AvaloniaExpander, IMotionAwareControl { ... }
```

**Avalonia Expander 提供的核心能力：**

| 能力 | 说明 |
|---|---|
| `Header` / `HeaderTemplate` | 头部内容和模板 |
| `Content` / `ContentTemplate` | 折叠区域内容和模板 |
| `IsExpanded` | 展开/收起状态 |
| `ExpandDirection` | 展开方向（Up/Down/Left/Right） |
| `ContentTransition` | 内容切换过渡动画 |

### AtomUI 的扩展设计

AtomUI 在 Avalonia Expander 之上做了以下增强：

| 增强项 | 说明 |
|---|---|
| **Ant Design 视觉规范** | 头部背景色、边框处理、圆角、间距完全对齐 Ant Design Collapse 面板规范 |
| **三种尺寸** | 通过 `SizeType` 支持 Large / Middle / Small 三种尺寸 |
| **展开图标控制** | 支持自定义图标（`ExpandIcon`）、图标位置（`ExpandIconPosition`）、隐藏图标（`IsShowExpandIcon`） |
| **附加内容区域** | 通过 `AddOnContent` 在头部右侧添加额外操作区（如设置图标） |
| **无边框模式** | `IsBorderless` 移除整体边框 |
| **幽灵模式** | `IsGhostStyle` 使头部背景透明 |
| **触发方式** | `TriggerType` 控制点击头部或仅点击图标触发展开 |
| **自定义间距** | `HeaderPadding` / `ContentPadding` 覆盖默认间距 |
| **展开/收起动画** | 使用 `ExpandMotion` / `CollapseMotion` 实现平滑的尺寸变化动画 |

---

## 功能详解

### 展开方向

通过继承自 Avalonia 的 `ExpandDirection` 属性控制四个方向的展开：

| 方向 | 说明 |
|---|---|
| `Down`（默认） | 向下展开，头部在上方 |
| `Up` | 向上展开，头部在下方 |
| `Left` | 向左展开，头部在右侧（旋转 90°） |
| `Right` | 向右展开，头部在左侧（旋转 90°） |

水平方向（Left/Right）时，头部会自动通过 `LayoutTransformControl` 旋转 90°。

### 尺寸系统

Expander 支持三种尺寸，影响头部内边距、内容内边距和字号：

| 尺寸 | 头部内边距 Token | 内容内边距 Token | 字号 |
|---|---|---|---|
| `Large` | `HeaderPaddingLG` | `ContentPaddingLG` | `FontSizeLG` |
| `Middle` | `HeaderPadding` | `ContentPadding` | `FontSize` |
| `Small` | `HeaderPaddingSM` | `ContentPaddingSM` | `FontSize` |

### 视觉风格

| 风格 | 属性 | 效果 |
|---|---|---|
| 默认 | 无特殊设置 | 带边框、头部灰色背景、内容白色背景 |
| 无边框 | `IsBorderless="True"` | 移除外边框，内容区域使用头部背景色 |
| 幽灵 | `IsGhostStyle="True"` | 头部背景透明 |

### 触发方式

通过 `TriggerType` 属性控制展开/收起的触发区域：

| 触发类型 | 鼠标指针 | 触发区域 |
|---|---|---|
| `Header`（默认） | 整个头部区域显示手型光标 | 点击头部任意位置 |
| `Icon` | 仅展开图标显示手型光标 | 仅点击展开图标 |

### 展开图标

- **默认图标**：`RightOutlined`（右箭头）
- **自定义图标**：通过 `ExpandIcon` 属性设置
- **隐藏图标**：`IsShowExpandIcon="False"`
- **图标位置**：`ExpandIconPosition` 控制图标在左侧（`Start`）或右侧（`End`）

展开状态下，图标会根据展开方向自动旋转 90°。

### 附加内容

通过 `AddOnContent` 和 `AddOnContentTemplate` 属性，可以在头部右侧（图标和标题之后）添加额外内容，如操作图标或按钮。

### 自定义间距

当设置 `HeaderPadding` 或 `ContentPadding` 时，会激活 `:custom-header-padding` / `:custom-content-padding` 伪类，覆盖 Token 控制的默认间距。

---

## 与 Ant Design 对齐程度

| 特性 | Ant Design Collapse | AtomUI Expander | 对齐情况 |
|---|---|---|---|
| 展开/收起 | ✅ `activeKey` | ✅ `IsExpanded` | ✅ 完全对齐 |
| 三种尺寸 | ✅ `size` | ✅ `SizeType` | ✅ 完全对齐 |
| 自定义图标 | ✅ `expandIcon` | ✅ `ExpandIcon` | ✅ 完全对齐 |
| 图标位置 | ✅ `expandIconPosition` | ✅ `ExpandIconPosition` | ✅ 完全对齐 |
| 隐藏图标 | ✅ `showArrow` | ✅ `IsShowExpandIcon` | ✅ 完全对齐 |
| 无边框 | ✅ `bordered` | ✅ `IsBorderless` | ✅ 完全对齐 |
| 幽灵模式 | ✅ `ghost` | ✅ `IsGhostStyle` | ✅ 完全对齐 |
| 触发方式 | ✅ `collapsible` | ✅ `TriggerType` | ✅ 完全对齐 |
| 附加内容 | ✅ `extra` | ✅ `AddOnContent` | ✅ 完全对齐 |
| 展开方向 | ❌ 仅垂直 | ✅ Up/Down/Left/Right | ✅ 超越（支持四向） |
| 自定义间距 | — | ✅ `HeaderPadding` / `ContentPadding` | ✅ 扩展 |
| 展开动画 | ✅ 内置 | ✅ `ExpandMotion` / `CollapseMotion` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.ContentControl
  └── Avalonia.Controls.HeaderedContentControl
        └── Avalonia.Controls.Expander
              └── AtomUI.Desktop.Controls.Expander
                    ├── implements IMotionAwareControl
                    └── RegisterTokenResourceScope(ExpanderToken.ScopeProvider)
```

**注意**：与大多数 AtomUI 控件不同，Expander 没有 `Abstract*` 基类层——它直接继承 Avalonia 的 `Expander`，因为 Avalonia 已经提供了足够完善的基础能力。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Expander/Expander.cs` | Expander 控件实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Expander/ExpanderToken.cs` | 组件级 Design Token |
| 伪类 | `src/AtomUI.Desktop.Controls/Expander/ExpanderPseudoClass.cs` | 伪类常量定义 |
| 模板常量 | `src/AtomUI.Desktop.Controls/Expander/Themes/ExpanderThemeConstants.cs` | 模板部件名称常量 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Expander/Themes/ExpanderTheme.axaml` | ControlTheme 定义 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ExpanderShowCase.axaml` | 使用范例 |

---

## 模板结构

Expander 的 ControlTemplate 由 `DockPanel` 组织头部和内容两大区域：

```
Border#PART_Frame (外边框，圆角裁剪)
└── DockPanel#PART_MainLayout
    ├── LayoutTransformControl#PART_HeaderLayoutTransform (头部容器，支持旋转)
    │   └── Border#PART_HeaderDecorator (头部装饰，背景色 + 边框)
    │       └── Grid#PART_HeaderLayout (4列网格布局)
    │           ├── IconButton#PART_ExpandButton (Col 0 或 3，展开/收起图标)
    │           ├── ContentPresenter#PART_HeaderPresenter (Col 1，头部内容)
    │           └── ContentPresenter#PART_AddOnContentPresenter (Col 2，附加内容)
    └── LayoutAwareMotionActor#PART_ContentMotionActor (动画容器)
        └── ContentPresenter#PART_ContentPresenter (折叠内容)
```

**关键绑定：**

| 模板部件 | 绑定属性 | 说明 |
|---|---|---|
| `Border#PART_Frame` | `EffectiveBorderThickness` | 根据 `IsBorderless`/`IsGhostStyle` 动态计算 |
| `IconButton#PART_ExpandButton` | `Icon` → `ExpandIcon`、`IsVisible` → `IsShowExpandIcon` | 展开图标 |
| `ContentPresenter#PART_HeaderPresenter` | `Header` / `HeaderTemplate` | 头部内容 |
| `ContentPresenter#PART_AddOnContentPresenter` | `AddOnContent` / `AddOnContentTemplate` | 附加内容 |
| `ContentPresenter#PART_ContentPresenter` | `Content` / `ContentTemplate`、`Padding` → `ContentPadding` | 折叠内容 |
