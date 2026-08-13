# Badge

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Badge 用于在目标内容附近显示数量、状态点或角标信息。桌面家族包含三个可实例化的 public owner：

| Owner | 职责 |
| --- | --- |
| `CountBadge` | 显示非负数量、溢出文本以及零值可见性。 |
| `DotBadge` | 显示状态点，可在独立模式下附带状态文本。 |
| `RibbonBadge` | 在目标内容边缘显示带文本的 Ribbon。 |

Badge 不负责通知中心、Tooltip、业务状态存储或目标内容本身的主题定义。`DecoratedTarget` 始终由应用拥有，Badge 只负责组合和定位。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge` |
| 状态 | Stable |

## 何时使用

Badge 以“附加信息不改变主体内容语义”为核心。数量、状态点和 Ribbon 都是目标内容的补充视觉；没有目标时也可以作为独立信息单元使用。

| 维度 | CountBadge | DotBadge | RibbonBadge |
| --- | --- | --- | --- |
| 信息密度 | 紧凑数值或溢出文本。 | 最小状态信号，可选说明文本。 | 突出的短文本标签。 |
| 视觉锚点 | 目标边角或独立徽标。 | 目标边角或独立状态行。 | 目标的 Start/End 上边缘。 |
| 状态表达 | 数量、零值、溢出和尺寸。 | 语义状态色或自定义颜色。 | 文本、颜色、位置和显示状态。 |
| 定制原则 | 公开完整 indicator，不公开背景与文本拆分。 | 公开状态点 indicator，不把 standalone Label 纳入契约。 | 公开完整 indicator 与文本 content，不公开折角几何。 |

Semantic Part 表达跨版本稳定的产品职责，不等同于内部 Adorner、MotionActor、Border、Label 或绘制节点清单。

## 公共 API

### 3.1 Public owner

`AbstractCountBadge`、`AbstractDotBadge`、`AbstractRibbonBadge` 是共享 API 与状态基类；桌面用户实例化 `CountBadge`、`DotBadge`、`RibbonBadge`。内部 Adorner、指示器和动效类型不是用户可实例化的公共控件，也不是 Semantic Part descriptor owner。

#### CountBadge

| 契约组 | 成员 | 语义与默认值 |
| --- | --- | --- |
| 内容 | `DecoratedTarget` | 可选的被装饰 `Control`，同时是 XAML content property；默认 `null`。 |
| 数值 | `Count`、`OverflowCount`、`IsZeroVisible` | `Count` 默认 `0` 且被约束为非负值；`OverflowCount` 默认 `99` 且被约束为非负值；零值默认不显示。 |
| 外观 | `BadgeColor`、`Size` | `BadgeColor` 接受预设色名或可解析颜色字符串；`Size` 默认为 `Default`，另支持 `Small`。 |
| 定位 | `Offset` | 调整指示器相对目标或独立布局位置；默认 `(0,0)`。 |
| 状态 | `BadgeIsVisible`、`IsMotionEnabled` | 控制徽标可见性与动效；`BadgeIsVisible` 默认 `true`。 |

#### DotBadge

| 契约组 | 成员 | 语义与默认值 |
| --- | --- | --- |
| 内容 | `DecoratedTarget`、`Text` | `DecoratedTarget` 是可选 XAML content property；`Text` 只在独立模式显示；二者默认 `null`。 |
| 状态 | `Status`、`BadgeIsVisible`、`IsMotionEnabled` | `Status` 可选，支持 `Default`、`Success`、`Processing`、`Error`、`Warning`；`BadgeIsVisible` 默认 `true`。 |
| 外观 | `DotColor` | 接受预设色名或可解析颜色字符串；显式颜色作为状态色之外的实例颜色入口。 |
| 定位 | `Offset` | 调整状态点相对目标的位置；默认 `(0,0)`。 |

#### RibbonBadge

| 契约组 | 成员 | 语义与默认值 |
| --- | --- | --- |
| 内容 | `DecoratedTarget`、`Text` | `DecoratedTarget` 是可选 XAML content property；`Text` 为 Ribbon 文本；二者默认 `null`。 |
| 外观 | `RibbonColor` | 接受预设色名或可解析颜色字符串。 |
| 定位 | `Placement`、`Offset` | `Placement` 默认为 `End`，另支持 `Start`；`Offset` 默认 `(0,0)`。 |
| 状态 | `BadgeIsVisible` | 控制 Ribbon 是否存在；默认 `true`。 |

Badge 家族没有控件专属 public 事件；状态变化通过 Avalonia 属性、继承事件和绑定系统表达。

### 3.2 Semantic Parts

Badge 的三个可实例化 owner 各自拥有独立 descriptor，支持范围如下：

| Owner | Parts | 职责摘要 |
| --- | --- | --- |
| `CountBadge` | `root`、`indicator` | 数量状态 owner 与完整数量徽标视觉。 |
| `DotBadge` | `root`、`indicator` | 状态点 owner 与状态点视觉；不公开 standalone 说明文本。 |
| `RibbonBadge` | `root`、`indicator`、`content` | Ribbon owner、完整 Ribbon 表面与文本区域。 |

完整的 Selector、`ContractType`、cardinality、跨根与运行时元数据、逐 Part 定制说明和排除边界见
[Badge Semantic Part 契约](semantic-part.md)。同名 `indicator` 不表示三个 owner 共享运行时节点、状态或 Theme。

## 事件与命令

Badge 家族没有控件专属 public 事件；状态变化通过 Avalonia 属性、继承事件和绑定系统表达。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:131`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="0" IsZeroVisible="True">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### 封顶数字

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:154`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="99">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="100">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="99" OverflowCount="10">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="1000" OverflowCount="999">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### 偏移量

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:189`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5" Offset="10, 10">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### 尺寸

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:205`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="5" Size="Small">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

## 状态模型

Badge 状态从 public owner 单向投影到运行时视觉：

```text
Public API
  -> owner visibility / mode / effective text and color
  -> runtime Adorner properties
  -> ControlTheme / Measure / Arrange / Render
```

主要状态规则：

- `Count=0 && IsZeroVisible=false` 时，CountBadge 将徽标归一为隐藏；`Count > OverflowCount` 时显示 `<OverflowCount>+`。
- DotBadge 有 `DecoratedTarget` 时只显示状态点；独立模式可以同时显示状态点和 `Text`。
- RibbonBadge 隐藏时只移除 Ribbon 视觉，不隐藏 `DecoratedTarget`。
- Count/Dot 启用退出动效时，indicator 可以在隐藏请求后短暂保留；动效完成后才从宿主移除。
- Dot 在 standalone 与 target mode 间切换时会重建内部 Adorner，但公开 `indicator` 身份不变。
- Semantic marker 不表达 visible、status、placement 或 motion phase；节点存在时 marker 保持不变。完整状态与 Part 数量矩阵见
  [Badge Semantic Part 契约](semantic-part.md)。

## 主题与 Design Token

Badge owner 本身没有 ControlTemplate。三个 owner 在运行时创建内部视觉宿主，并把 public 属性单向投影给该宿主。

| Owner | 无 `DecoratedTarget` | 有 `DecoratedTarget` |
| --- | --- | --- |
| `CountBadge` | 数量视觉作为 owner 的普通视觉和逻辑子树。 | 目标作为 owner 子节点；数量视觉显示在 Avalonia `AdornerLayer`。 |
| `DotBadge` | 状态点与可选文本作为 owner 的普通视觉和逻辑子树。 | 目标作为 owner 子节点；状态点显示在 Avalonia `AdornerLayer`。 |
| `RibbonBadge` | Ribbon 视觉作为 owner 的普通视觉和逻辑子树。 | 目标与 Ribbon 都由 owner 在同一 inline visual tree 中排列，不进入原生 `AdornerLayer`。 |

CountBadge 和 DotBadge 的跨 VisualRoot 模式只改变 indicator 的 visual parent。其 logical/style owner 仍必须是对应 Badge owner，使资源、实例 `Styles` 和 owner-scoped selector 保持可达。

Badge 的默认视觉由三个内部 Token scope 与四个 ControlTheme 共同提供：

| 资源 | 职责 |
| --- | --- |
| `CountBadgeToken` | 数量徽标高度、字体、颜色、Padding、圆角和阴影。 |
| `DotBadgeToken` | 状态点尺寸、颜色、阴影和独立文本间距。 |
| `RibbonBadgeToken` | Ribbon 偏移、折角、文本 Padding 和行高。 |
| `CountBadgeAdornerTheme.axaml` | 数量 indicator 的模板、尺寸变体和默认视觉。 |
| `DotBadgeAdornerTheme.axaml` | 状态点、独立文本和 target mode 模板。 |
| `DotBadgeIndicatorTheme.axaml` | 状态点绘制所需的默认属性。 |
| `RibbonBadgeAdornerTheme.axaml` | Ribbon content 模板及绘制参数。 |

Token 只表达组件视觉语义，不保存数量、状态、可见性、目标引用或 motion phase。AtomUI 内置主题不得使用 `.semantic-*` 实现默认视觉。

Token 来源：

Badge Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `BadgeToken`，scope id 为 `Badge`，源码位于 `src/AtomUI.Desktop.Controls/Badge/BadgeToken.cs`。

## AOT 与裁剪注意事项

Badge Semantic Part 的默认运行时成本仅包括 descriptor 静态数据和既有视觉节点上的静态 class：

- descriptor、Part 名称、selector class 和类型 identity 由 Generator 静态产生，不使用反射发现。
- AXAML marker 在模板初始化时执行一次 `Classes.Set`，不创建 Binding 或持久状态同步。
- Ribbon indicator 在 Adorner factory 中使用生成常量执行一次 class 添加。
- logical parent 调整复用既有 attach/detach 路径，不增加 VisualTree 扫描、布局监听或全局事件。
- Control 包不查询 Semantic Part registry，也不创建 Gallery Preview、highlight Adorner 或 descriptor ViewModel。
- AtomUI 默认 ControlTheme 不使用 `.semantic-*` selector，因此未声明用户 Semantic Style 时不创建对应 class activator。
- 应用声明 Semantic Style 后，Avalonia 只为实际候选节点维护 selector 激活；同一 Part 的 Setter 应合并到一个 Style，并在批量 Badge 场景验证 listener 释放。

运行时逻辑不得使用 `Type.GetType`、`Assembly.GetTypes`、动态代码生成、字符串属性路径或反射扫描寻找 Part。Gallery 对已实例化 marker 的查找属于延迟创建的工具层，不得进入 Badge 包。

## 源码索引

### 2.1 Public owner 与共享状态

| 源码 | 职责 |
| --- | --- |
| `src/AtomUI.Controls/Badge/AbstractCountBadge.cs` | CountBadge public 属性、零值归一、运行时宿主、AdornerLayer retry 和 attach/detach。 |
| `src/AtomUI.Controls/Badge/AbstractDotBadge.cs` | DotBadge public 属性、standalone/target 模式切换、运行时宿主和 AdornerLayer 生命周期。 |
| `src/AtomUI.Controls/Badge/AbstractRibbonBadge.cs` | RibbonBadge public 属性、inline child 管理、测量和排列。 |
| `src/AtomUI.Desktop.Controls/Badge/CountBadge.cs` | 桌面 public owner、Count Adorner factory 和 Token 投影入口。 |
| `src/AtomUI.Desktop.Controls/Badge/DotBadge.cs` | 桌面 public owner、Dot Adorner factory 和 Token 投影入口。 |
| `src/AtomUI.Desktop.Controls/Badge/RibbonBadge.cs` | 桌面 public owner、Ribbon Adorner factory 和颜色投影入口。 |

### 2.2 Internal runtime visual 与 Theme

| 源码 | 职责 |
| --- | --- |
| `AbstractCountBadgeAdorner.cs` / `CountBadgeAdorner.cs` | 数量文本计算、显示隐藏动效、定位、阴影和主题宿主。 |
| `AbstractDotBadgeAdorner.cs` / `DotBadgeAdorner.cs` | 状态点动效、模式布局、定位和主题宿主。 |
| `DotBadgeIndicator.cs` | 使用 `DrawingContext` 绘制状态点与阴影。 |
| `AbstractRibbonBadgeAdorner.cs` / `RibbonBadgeAdorner.cs` | Ribbon 测量、排列、背景与折角绘制。 |
| `CountBadgeToken.cs` / `DotBadgeToken.cs` / `RibbonBadgeToken.cs` | 三种视觉的内部 Token scope。 |
| `CountBadgeAdornerTheme.axaml` | 数量 indicator 模板与尺寸变体。 |
| `DotBadgeAdornerTheme.axaml` | standalone 和 target mode 两套状态点模板。 |
| `DotBadgeIndicatorTheme.axaml` | 状态点绘制属性的默认值。 |
| `RibbonBadgeAdornerTheme.axaml` | Ribbon 文本模板与绘制参数。 |

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/badge/overview.md`
- 实现文档：`docs/controls/desktop/data-display/badge/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-display/badge/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-display/badge/token.md`
- 变更记录：`docs/controls/desktop/data-display/badge/changelog.md`
- 语义结构：`./semantic-cn.md`
