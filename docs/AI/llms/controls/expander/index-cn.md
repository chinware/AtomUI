# Expander

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Expander 是桌面端数据展示类单面板折叠容器，用于在有限空间内展示一段可展开或收起的内容。它以 Avalonia `Expander` 为基础，扩展 AtomUI 的尺寸、展开图标、附加内容、触发区域、边框模式、Ghost 模式、展开方向、动效和 Token 体系。

Expander 的职责是管理一个 Header 与一个 Content 区域之间的展开关系。它不是多面板集合控件，不负责手风琴互斥展开、列表虚拟化、数据项生成、表单校验、远程加载或复杂主从详情关系。需要多面板协调时应使用 Collapse；需要静态分组时应使用 GroupBox；需要列表或树形数据展示时应使用 ListView、TreeView 或 DataGrid。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Expander` |
| 状态 | Stable |

## 何时使用

Expander 的设计语言来自 参考设计体系的轻量折叠面板：Header 表达当前内容主题，展开图标表达折叠状态和方向，Content 承载可延迟阅读的内容。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 层级收纳 | Header 始终可见，Content 按状态显示或隐藏。 | `Header`、`Content`、`IsExpanded`。 |
| 展开方向 | 内容可以向下、向上、向左或向右展开。 | `ExpandDirection`。 |
| 触发区域 | 可配置整行 Header 触发或仅图标触发。 | `TriggerType`。 |
| 图标语义 | 展开图标表示折叠状态、方向和可操作入口。 | `ExpandIcon`、`IsShowExpandIcon`、`ExpandIconPosition`。 |
| 视觉强度 | 普通、Borderless 和 Ghost 模式表达不同容器强度。 | `IsBorderless`、`IsGhostStyle`。 |
| 密度 | 尺寸和自定义 padding 控制 Header/Content 的空间密度。 | `SizeType`、`HeaderPadding`、`ContentPadding`。 |

Expander 应保持数据展示控件的克制视觉。Header 不是工具栏，AddOnContent 只承担轻量辅助入口，不改变折叠容器的主语义。

## 公共 API

Expander 的公共契约分为继承的 Avalonia `Expander` 契约和 AtomUI 扩展契约。

继承契约：

| API | 语义 |
| --- | --- |
| `Header` / `HeaderTemplate` | Header 区域内容和模板。 |
| `Content` / `ContentTemplate` | 可展开内容区域和模板。 |
| `IsExpanded` | 展开状态。 |
| `ExpandDirection` | 展开方向，支持 `Down`、`Up`、`Left`、`Right`。 |
| `IsEnabled` | 禁用后 Header 和展开按钮进入禁用状态。 |
| `BorderThickness` | 普通模式根边框厚度来源。 |

AtomUI 扩展契约：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SizeType` | `CustomizableSizeType` | 控件尺寸，支持 `Large`、`Middle`、`Small`、`Custom`。`Custom` 默认沿用 Middle 主题分支，显式 `HeaderPadding` / `ContentPadding` 可覆盖默认 spacing。 |
| `IsShowExpandIcon` | `bool` | 是否显示展开图标。 |
| `ExpandIcon` | `PathIcon?` | 自定义展开图标；为空时使用 `RightOutlined`。 |
| `AddOnContent` / `AddOnContentTemplate` | `object?` / `IDataTemplate?` | Header 右侧辅助内容及模板。 |
| `IsGhostStyle` | `bool` | 使用 Ghost 视觉，移除容器边框并使用内容背景作为 Header 背景。 |
| `IsBorderless` | `bool` | 使用无边框视觉，移除容器边框。 |
| `TriggerType` | `ExpanderTriggerType` | 触发区域，`Header` 表示 Header 区域可触发，`Icon` 表示仅图标可触发。 |
| `ExpandIconPosition` | `ExpanderIconPosition` | 展开图标位置，支持 `Start` 和 `End`。 |
| `HeaderPadding` | `Thickness?` | 显式 Header 内边距；为空时由 SizeType 和 ExpanderToken 决定。 |
| `ContentPadding` | `Thickness?` | 显式 Content 内边距；为空时由 SizeType 和 ExpanderToken 决定。 |
| `IsMotionEnabled` | `bool` | 是否启用展开/收起动效。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Frame` | `Border` | 根边框、裁剪和整体布局承载。 |
| `PART_MainLayout` | `DockPanel` | Header 与 Content 的 dock 布局。 |
| `PART_HeaderLayoutTransform` | `LayoutTransformControl` | 横向展开方向下旋转 Header。 |
| `PART_HeaderDecorator` | `Border` | Header 背景、边框和 padding 承载，也是 Header 点击范围。 |
| `PART_HeaderLayout` | `Grid` | 展开图标、Header、AddOnContent 的列布局。 |
| `PART_ExpandButton` | `IconButton` | 展开图标显示和图标触发入口。 |
| `PART_HeaderPresenter` | `ContentPresenter` | Header 内容和模板承载。 |
| `PART_AddOnContentPresenter` | `ContentPresenter` | AddOnContent 内容和模板承载。 |
| `PART_ContentMotionActor` | `LayoutAwareMotionActor` | Content 展开/收起动效承载。 |
| `PART_ContentPresenter` | `ContentPresenter` | Content 内容、模板和 padding 承载。 |

稳定伪类和主题状态：

| 伪类 / selector 状态 | 语义 |
| --- | --- |
| `:expanded` | 继承展开状态，用于展开图标旋转。 |
| `:up` / `:down` / `:left` / `:right` | 展开方向状态。 |
| `:custom-header-padding` | `HeaderPadding` 非空，Header padding 和图标间距使用显式值。 |
| `:custom-content-padding` | `ContentPadding` 非空，Content padding 使用显式值。 |
| `[IsBorderless=True]` / `[IsGhostStyle=True]` | 视觉强度分支。 |
| `[TriggerType=Header]` / `[TriggerType=Icon]` | Cursor 和点击路径分支。 |

Expander 没有专用 routed event 或 command。展开状态通过继承的 `IsExpanded` 表达。

## 事件与命令

Expander 没有专用 routed event 或 command。展开状态通过继承的 `IsExpanded` 表达。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 无边框

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Expander/Views/ExpanderAppearanceShowCase.axaml:12`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Expander IsBorderless="True" Header="这是面板标题 1">
    <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
</atom:Expander>
```

### 幽灵展开器

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Expander/Views/ExpanderAppearanceShowCase.axaml:20`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Expander IsGhostStyle="True" Header="这是面板标题 1">
    <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
</atom:Expander>
```

### 自定义标题和内容间距

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Expander/Views/ExpanderAppearanceShowCase.axaml:28`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Expander Header="这是面板标题 1" HeaderPadding="5" ContentPadding="5">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:Expander>

    <atom:Expander Header="这是面板标题 1" IsGhostStyle="True" HeaderPadding="5" ContentPadding="5">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:Expander>
</StackPanel>
```

### 展开器

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Expander/Views/ExpanderBasicShowCase.axaml:12`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Expander Header="这是面板标题 1">
    <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
</atom:Expander>
```

## 状态模型

核心状态流：

```text
Header pointer / ExpandButton click
        ↓
TriggerType gate
        ↓
IsExpanded
        ↓
Content motion / stable visibility
        ↓
PART_ContentMotionActor.IsVisible
```

触发语义：

- `TriggerType=Header` 时，鼠标左键按下且命中 `PART_HeaderDecorator` 会切换 `IsExpanded`。
- `TriggerType=Icon` 时，Header 点击不切换状态，只能通过 `PART_ExpandButton.Click` 切换。
- 禁用状态下模板将 `IsEnabled` 传递给 `PART_ExpandButton`，主题同步禁用前景。

展开方向：

- `ExpandDirection=Down`：Header 停留顶部，Content 向下展开。
- `ExpandDirection=Up`：Header dock 到底部，Content 向上展开。
- `ExpandDirection=Left` / `Right`：Header 通过 `LayoutTransformControl` 旋转，整体对齐到对应侧。
- 展开图标在 `:expanded` 下按展开方向旋转，必须与内容运动方向一致。

动效状态：

- `IsMotionEnabled=false` 时直接同步 `PART_ContentMotionActor.IsVisible` 和透明度。
- `IsMotionEnabled=true` 时使用 `ExpandMotion` / `CollapseMotion` 运行布局感知动效。
- 展开状态在动画中再次变化时，当前 motion 会取消并以最新 `IsExpanded` 重新归一到最终可见状态。

自定义 padding：

```text
HeaderPadding != null
  → :custom-header-padding
  → PART_HeaderDecorator.Padding = HeaderPadding
  → EffectiveExpandButtonMargin 按 HeaderPadding.Left/Right 推导

ContentPadding != null
  → :custom-content-padding
  → PART_ContentPresenter.Padding = ContentPadding
```

自定义 HeaderPadding 分支必须保持展开图标与 Header 文本的水平间距和垂直居中关系，不应继续套用默认尺寸 token 的图标间距。

## 主题与 Design Token

Expander 的默认视觉由 `ExpanderTheme.axaml` 和 `ExpanderToken` 共同定义。

主题职责：

- 提供 Header、Content、MotionActor 和 Frame 的稳定模板结构。
- 根据 `SizeType` 选择 Header/Content padding 和字体大小。
- 根据 `ExpandDirection` 设置 Header dock、旋转和图标旋转。
- 根据 `IsExpanded` 设置 Header 边框状态。
- 根据 `IsBorderless` / `IsGhostStyle` 调整根边框和背景。
- 根据 `TriggerType` 设置可点击区域 cursor。
- 根据自定义 padding 伪类覆盖 Header/Content padding 和展开图标间距。

Token 关系：

```text
SharedToken
   ↓
ExpanderToken
   ↓
ExpanderTheme
   ↓
Frame + Header + ExpandButton + Content
```

SharedToken 提供全局边框、字体、动效时长、图标大小和基础颜色。ExpanderToken 提供 Header/Content padding、Header/Content 背景、圆角和展开图标默认外边距。

Token 来源：

ExpanderToken 是 Expander 的组件级 Token scope，描述 Header/Content 的默认 padding、背景、整体圆角和展开图标默认外边距。

ExpanderToken 不承载以下状态：

- `Header`、`Content`、`AddOnContent` 等实例内容。
- `IsExpanded`、`ExpandDirection`、`TriggerType`、`ExpandIconPosition` 等实例行为状态。
- `HeaderPadding` / `ContentPadding` 的显式用户覆盖值。
- `EffectiveBorderThickness`、`HeaderBorderThickness`、`EffectiveExpandButtonMargin` 等运行时派生状态。
- motion 运行状态、cancellation 或临时 transform。

## AOT 与裁剪注意事项

资源边界：

- `ExpanderToken` 通过 token generator 注册，主题通过 `ExpanderTokenResource` 消费。
- SharedToken 提供图标大小、动效开关、动效时长、边框厚度和基础颜色。
- 默认图标使用显式 `new RightOutlined()`，不做运行时图标扫描。

生命周期边界：

- `_expandButton.Click` 在模板重套用时先解除旧订阅，再订阅新 part。
- `_contentMotionCancellation` 在新 motion、稳定状态、模板重套用和 detach 时取消并释放。
- motion actor 临时值必须在状态归一时清理。

AOT 边界：

- 不新增字符串 path binding、运行时反射扫描或动态类型创建。
- 模板内固定关系优先使用 AXAML `TemplateBinding`、selector 和 TokenResource。
- C# 只处理 AXAML 无法表达的运行时状态流，例如 motion cancellation 和 HeaderPadding 派生 margin。

性能边界：

- Header/Content 默认视觉由静态 AXAML 提供，不在运行时动态构造模板视觉。
- 展开/收起只操作单个 `LayoutAwareMotionActor`，不遍历复杂子树。
- 自定义 padding 的 margin 计算是常量时间，不进入渲染热路径。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Expander/Expander.cs`：公共 API、内部 effective state、template part 接入、触发区域、默认图标、边框状态、动效状态机和自定义 padding 度量。
- `src/AtomUI.Desktop.Controls/Expander/ExpanderPseudoClass.cs`：方向、自定义 padding 和展开状态相关伪类常量。
- `src/AtomUI.Desktop.Controls/Expander/ExpanderToken.cs`：Expander 组件 Token。
- `src/AtomUI.Desktop.Controls/Expander/Themes/ExpanderTheme.axaml`：控件模板、SizeType 分支、方向分支、图标位置分支、触发分支、Borderless/Ghost 分支和 token 引用。
- `tests/AtomUI.Desktop.Controls.Tests/Expander/ExpanderBehaviorTests.cs`：Expander 行为和布局回归测试。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/expander/overview.md`
- 实现文档：`docs/controls/desktop/data-display/expander/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/expander/token.md`
- 变更记录：`docs/controls/desktop/data-display/expander/changelog.md`
- 语义结构：`./semantic-cn.md`
