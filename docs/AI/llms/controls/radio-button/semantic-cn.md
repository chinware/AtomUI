# RadioButton 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `radio-root` | `RadioButton` | 承载普通单选内容、checked、disabled 和 Wave 状态。 | `Content`、`IsChecked` | RadioButtonToken | stable |
| `radio-group` | `RadioButtonGroup` | 承载普通单选集合、CheckedItem 和排列方向。 | `CheckedItem`、`Orientation`、`ItemsSource` | SharedToken | stable |
| `indicator` | `RadioIndicator` | 绘制普通单选圆环、圆点和状态动效。 | `IsChecked`、`IsEnabled` | RadioButtonToken | internal-observable |
| `option-group` | `OptionButtonGroup` | 承载按钮式单选集合、Orientation、共享边框和选中边框。 | `Orientation`、`ButtonStyle`、`SelectedItem`、`SizeType` | OptionButtonToken + SharedToken | stable |
| `option-item` | `OptionButton` | 承载按钮式选项内容、图标、checked 状态和有效圆角。 | `Content`、`Icon`、`IsChecked` | OptionButtonToken | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioButtonTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <DockPanel>
        <RadioIndicator Name="Indicator" />
        <ContentPresenter Name="ContentPresenter" />
    </DockPanel>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
RadioButton
  -> RadioButtonGroup (control theme, RadioButtonGroupTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> RadioButton (control theme, RadioButtonTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> DockPanel (template-stable)
           -> RadioIndicator#Indicator (internal-observable)
           -> ContentPresenter#ContentPresenter (internal-observable)
  -> RadioIndicator (control theme, RadioIndicatorTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#{x:Static atom:WaveSpiritDecorator.WaveSpiritPart} (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `RadioButton` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RadioButtonGroup` | control theme | `RadioButtonGroupTheme.axaml` | 用户代码 / 控件宿主 | `BorderBrush`, `BorderThickness`, `CornerRadius`, `ItemsPanel` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `RadioButtonGroupTheme.axaml` | RadioButtonGroup | `BorderBrush`, `BorderThickness`, `CornerRadius`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `RadioButtonGroupTheme.axaml` | RadioButtonGroup | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RadioButton` | control theme | `RadioButtonTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `RadioButtonTheme.axaml` | RadioButton | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `RadioButtonTheme.axaml` | RadioButton | `Content`, `ContentTemplate`, `IsChecked`, `IsEnabled`, `IsMotionEnabled`, `IsWaveSpiritEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Indicator` | template node (RadioIndicator) | `RadioButtonTheme.axaml` | RadioButton | `IsChecked`, `IsEnabled`, `IsMotionEnabled`, `IsWaveSpiritEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `RadioButtonTheme.axaml` | RadioButton | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RadioIndicator` | control theme | `RadioIndicatorTheme.axaml` | RadioButton | `IsMotionEnabled`, `IsWaveSpiritEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `RadioIndicatorTheme.axaml` | RadioIndicator | `IsMotionEnabled`, `IsWaveSpiritEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:WaveSpiritDecorator.WaveSpiritPart}` | template node (WaveSpiritDecorator) | `RadioIndicatorTheme.axaml` | RadioIndicator | `IsMotionEnabled`, `IsWaveSpiritEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Icon`、`Items`、`ItemsSource`、`ItemTemplate` | 定义选项内容、图标、数据和模板入口。 |
| 选择与集合 | `IsChecked`、`CheckedItem`、`SelectedIndex`、`SelectedItem` | 维护普通单选组和按钮式单选组的当前值。 |
| 交互与状态 | `IsEnabled`、`IsMotionEnabled`、`IsWaveSpiritEnabled`、`ButtonStyle` | 表达可用性、动效和 Outline/Solid 状态视觉。 |
| 视觉与布局 | `Orientation`、`ItemSpacing`、`LineSpacing`、`SizeType`、`CornerRadius`、`BorderThickness` | 控制普通组排列、按钮组方向、尺寸和组合几何。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | checked/selected、disabled、pointer、motion、ButtonStyle 和方向组合状态。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | RadioButtonToken、OptionButtonToken 与对应 ControlTheme。 |

## State Flow

RadioButton 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `RadioButtonGroup.CheckedItem` 是单选组的外部值 owner，默认 `BindingMode.TwoWay` 并启用 Avalonia data validation；用户选择和 ViewModel 更新必须收敛到同一份当前项状态。
- `OptionButtonGroup` 以 SelectingItemsControl 的选择状态作为按钮组选中 source of truth，用户 checked、键盘导航和外部 `SelectedIndex` / `SelectedItem` 必须收敛到同一选择。
- `OptionButtonGroup.Orientation` 是排列方向、组合圆角、分隔线方向和方向键导航的唯一 owner，默认值为 `Horizontal`。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

RadioButton 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `RadioButtonGroupTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `RadioButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `RadioButtonThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `RadioIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `OptionButtonGroupTheme.axaml` | 定义按钮组 ItemsPresenter、方向布局入口、尺寸和组级边框资源。 |
| `OptionButtonTheme.axaml` | 定义按钮内容、Outline/Solid、checked/disabled、方向对齐和 Wave 视觉。 |
| `OptionButtonBoxThemes.axaml` | 聚合 OptionButtonGroup 与 OptionButton 的主题资源。 |

普通单选控件使用 `RadioButtonToken`，按钮式选项使用 `OptionButtonToken`。Token 只表达组件视觉语义，不承载 checked/selected、Orientation、GroupPositionTrait 或 EffectiveCornerRadius 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

RadioButton 和 OptionButton Token 只表达组件级视觉变量，例如指示器尺寸、内容 Padding、字体和交互状态颜色。Token 不承载 checked/selected、Orientation、GroupPositionTrait、EffectiveCornerRadius 或容器 Bounds 等运行时状态。

当前 Token scope：

- `RadioButtonToken`，scope id 为 `RadioButton`，源码位于 `src/AtomUI.Desktop.Controls/RadioButton/RadioButtonToken.cs`。
- `OptionButtonToken`，scope id 为 `OptionButton`，源码位于 `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButtonToken.cs`。

## Customization Boundaries

维护 RadioButton 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- `OptionButtonGroup.Orientation` 默认保持 `Horizontal`；纵向能力不得改变横向尺寸、圆角、边框和选择语义。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 RadioButton 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
- OptionButtonGroup Orientation 默认 Horizontal，横向可观察行为保持；纵向宽度遵循 Avalonia Alignment 和 Width。
- Custom 尺寸不使用专属 selector，实例值和 owner-scoped Style 可以接管对应尺寸维度。
