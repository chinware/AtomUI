# Collapse

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Collapse 是 AtomUI 桌面控件体系中的折叠面板控件，用于在多个可展开内容区之间展示和隐藏内容。

Collapse 不负责树控件、菜单或 Tabs 内容切换。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Collapse`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Collapse` |
| 状态 | Stable |

## 何时使用

Collapse 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Collapse 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Collapse 是 AtomUI 桌面控件体系中的折叠面板控件，用于在多个可展开内容区之间展示和隐藏内容。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AddOnContent`、`AddOnContentTemplate`、`ContentPadding`、`ExpandIcon`、`ExpandIconPosition`、`HeaderPadding`、`IsShowExpandIcon`、`ItemContentPadding` 等 9 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Collapse Token + ControlTheme。 |

## 公共 API

Collapse 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AddOnContent`、`AddOnContentTemplate`、`ContentPadding`、`ExpandIcon`、`ExpandIconPosition`、`HeaderPadding`、`IsShowExpandIcon`、`ItemContentPadding`、`ItemHeaderPadding` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsSelected` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAccordion`、`IsBorderless`、`IsGhostStyle`、`IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `TriggerType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Collapse`、`CollapseItem`。
- 枚举：`CollapseExpandIconPosition`、`CollapseTriggerType`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddOnContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_ContentMotionActor` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_ExpandButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Frame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_HeaderDecorator` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_HeaderPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_ItemsPresenter` | `ItemsPresenter` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_MainLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Collapse 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 无边框

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Collapse/Views/CollapseAppearanceShowCase.axaml:12`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Collapse IsBorderless="True">
    <atom:CollapseItem Header="这是面板标题 1">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
    <atom:CollapseItem Header="这是面板标题 2">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
    <atom:CollapseItem Header="这是面板标题 3">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
</atom:Collapse>
```

### 幽灵折叠面板

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Collapse/Views/CollapseAppearanceShowCase.axaml:28`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Collapse IsGhostStyle="True">
    <atom:CollapseItem Header="这是面板标题 1">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
    <atom:CollapseItem Header="这是面板标题 2">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
    <atom:CollapseItem Header="这是面板标题 3">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
</atom:Collapse>
```

### 自定义标题和内容间距

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Collapse/Views/CollapseAppearanceShowCase.axaml:44`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Collapse ItemHeaderPadding="5" ItemContentPadding="5">
        <atom:CollapseItem Header="这是面板标题 1">
            <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
        </atom:CollapseItem>
        <atom:CollapseItem Header="这是面板标题 2">
            <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
        </atom:CollapseItem>
        <atom:CollapseItem Header="这是面板标题 3">
            <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
        </atom:CollapseItem>
    </atom:Collapse>

    <atom:Collapse ItemHeaderPadding="0" ItemContentPadding="0" IsGhostStyle="True">
        <atom:CollapseItem Header="这是面板标题 1">
            <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
        </atom:CollapseItem>
        <atom:CollapseItem Header="这是面板标题 2">
            <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
        </atom:CollapseItem>
        <atom:CollapseItem Header="这是面板标题 3">
            <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
        </atom:CollapseItem>
    </atom:Collapse>
</StackPanel>
```

### 折叠面板

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Collapse/Views/CollapseBasicShowCase.axaml:12`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Collapse>
    <atom:CollapseItem Header="这是面板标题 1">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
    <atom:CollapseItem Header="这是面板标题 2">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
    <atom:CollapseItem Header="这是面板标题 3">
        <atom:TextBlock TextWrapping="Wrap" Text="狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。" />
    </atom:CollapseItem>
</atom:Collapse>
```

## 状态模型

Collapse 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

`Collapse` 继续使用 Avalonia `SelectingItemsControl` 的 selection model 作为唯一展开状态 owner，`CollapseItem.IsSelected` 是该状态投影到容器后的公开绑定入口。控件不得维护 active-key 集合、当前展开项缓存或另一套展开状态。

状态维护规则：

- 普通模式使用 `Multiple | Toggle`：每个 item 可独立展开和收起。
- 手风琴模式使用 `Single | Toggle`：打开目标项时关闭旧项，点击当前项时允许全部收起。
- 普通模式切换到手风琴模式时，按视觉索引保留第一个已展开项，保证切换后的单一展开状态确定且稳定。
- Header、Icon、keyboard 和 pointer 输入最终进入同一个 selection 操作，不在输入处理器中直接维护展开状态。
- Disabled 或不可交互状态优先屏蔽 pointer、keyboard 和 motion，不改变 selection。
- 内容可见性、箭头方向和动效目标只从 `IsSelected` 派生；模板节点之间不得双向同步展开状态。
- 模板重套用、items reset/replace/clear 和模式切换后必须保持 selection model、容器与内容视觉一致。

## 主题与 Design Token

Collapse 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CollapseItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CollapseTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Collapse 使用 `CollapseToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、motion、visual option 运行时状态。

Collapse 的分隔线采用结构化所有权：

- `PART_Frame` 绘制外框、圆角并裁剪整体内容。
- 非末 `CollapseItem` 的 item shell 固定绘制底部分隔线。
- 默认 bordered 模式下，`PART_ContentFrame` 固定绘制内容顶部边线。
- Borderless 模式保留 item 间分隔线，但不绘制外框和内容顶部边线。
- Ghost 模式不绘制外框、item 分隔线和内容顶部边线。
- 分隔线厚度不得依赖 `IsSelected`、动效进行状态或动效完成时机。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Collapse Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `CollapseToken`，scope id 为 `Collapse`，源码位于 `src/AtomUI.Desktop.Controls/Collapse/CollapseToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 单项展开不得遍历全部容器重算边框。
- 模式切换只处理当前 selection indexes；item 位置只在集合结构或容器索引变化时更新。
- 边框、背景、padding 和 icon placement 使用 AXAML、selector 和 template binding 表达。
- 不新增运行时反射、字符串 binding、全局事件、timer、active-key 缓存或长期状态对象。
- template reapply 必须解绑旧展开按钮；detach 和新动效开始前必须取消旧 content motion。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/Collapse/Collapse.cs`
- `src/AtomUI.Desktop.Controls/Collapse/CollapseItem.cs`
- `src/AtomUI.Desktop.Controls/Collapse/CollapseToken.cs`
- `src/AtomUI.Desktop.Controls/Collapse/ICollapseItemData.cs`
- `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/collapse/overview.md`
- 实现文档：`docs/controls/desktop/data-display/collapse/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/collapse/token.md`
- 变更记录：`docs/controls/desktop/data-display/collapse/changelog.md`
- 语义结构：`./semantic-cn.md`
