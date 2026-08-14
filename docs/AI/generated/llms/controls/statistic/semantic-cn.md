# Statistic 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Statistic 公开 `root`、`header`、`title`、`content`、`value`、`prefix` 和 `suffix`。该契约只属于 `Statistic`；
`AbstractStatistic`、`TimerStatistic` 和 `StatisticCountUp` 不继承或发布这组 descriptor。

| Part | Selector | Style Type | ContractType | Cardinality | AtomUI 节点 | 稳定性 |
| --- | --- | --- | --- | --- | --- | --- |
| `root` | Statistic 本身 | 不适用 | `Statistic` | `Single` | owner | stable since 6.0 |
| `header` | `.semantic-header` | `StatisticHeaderStyle` | `Border` | `Single` | `Border#HeaderLayout` | stable since 6.0 |
| `title` | `.semantic-title` | `StatisticTitleStyle` | `ContentPresenter` | `Single` | `ContentPresenter#HeaderPresenter` | stable since 6.0 |
| `content` | `.semantic-content` | `StatisticContentStyle` | `StackPanel` | `Single` | `StackPanel#ContentLayout` | stable since 6.0 |
| `value` | `.semantic-value` | `StatisticValueStyle` | `ContentPresenter` | `Single` | 数值 `ContentPresenter` | stable since 6.0 |
| `prefix` | `.semantic-prefix` | `StatisticPrefixStyle` | `ContentPresenter` | `Single` | 前缀 `ContentPresenter` | stable since 6.0 |
| `suffix` | `.semantic-suffix` | `StatisticSuffixStyle` | `ContentPresenter` | `Single` | 后缀 `ContentPresenter` | stable since 6.0 |

六个 selector Part 的 `SelectorRoute` 均为 `/template/ .semantic-<name>`，`Customization` 为 `Selector`，
`CrossVisualRoot=false`，`RuntimeCreated=false`。root 的 `Customization` 为 `Root`，不生成 `.semantic-root` 或 Style Type。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Statistic/Themes/StatisticTheme.axaml`

```xml
<DashedBorder Name="Frame">
    <StackPanel Name="RootLayout">
        <Border Name="HeaderLayout">
            <ContentPresenter Name="HeaderPresenter" />
        </Border>
        <Skeleton>
            <StackPanel Name="ContentLayout">
                <ContentPresenter Name="ValuePrefixAddOn" />
                <ContentPresenter Name="ContentPresenter" />
                <ContentPresenter Name="ValueSuffixAddOn" />
            </StackPanel>
        </Skeleton>
    </StackPanel>
</DashedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Statistic
  -> StatisticCountUp (control theme, StatisticCountUpTheme.axaml)
     -> TextBlock (template-stable)
  -> Statistic (control theme, StatisticTheme.axaml)
     -> DashedBorder#Frame (template-stable)
        -> StackPanel#RootLayout (template-stable)
           -> Border#HeaderLayout (template-stable)
              -> ContentPresenter#HeaderPresenter (internal-observable)
           -> Skeleton (template-stable)
              -> StackPanel#ContentLayout (template-stable)
                 -> ContentPresenter#ValuePrefixAddOn (internal-observable)
                 -> ContentPresenter#ContentPresenter (internal-observable)
                 -> ContentPresenter#ValueSuffixAddOn (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Statistic` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StatisticCountUp` | control theme | `StatisticCountUpTheme.axaml` | 用户代码 / 控件宿主 | `FormattedValue` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Statistic` | control theme | `StatisticTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (DashedBorder) | `StatisticTheme.axaml` | Statistic | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (StackPanel) | `StatisticTheme.axaml` | Statistic | `Content`, `ContentTemplate`, `Header`, `HeaderTemplate`, `IsLoading`, `ValuePrefixAddOn` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (Border) | `StatisticTheme.axaml` | Statistic | `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderPresenter` | template node (ContentPresenter) | `StatisticTheme.axaml` | Statistic | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentLayout` | template node (StackPanel) | `StatisticTheme.axaml` | Statistic | `Content`, `ContentTemplate`, `ValuePrefixAddOn`, `ValuePrefixAddOnTemplate`, `ValueSuffixAddOn`, `ValueSuffixAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ValuePrefixAddOn` | template node (ContentPresenter) | `StatisticTheme.axaml` | Statistic | `ValuePrefixAddOn`, `ValuePrefixAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `StatisticTheme.axaml` | Statistic | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ValueSuffixAddOn` | template node (ContentPresenter) | `StatisticTheme.axaml` | Statistic | `ValueSuffixAddOn`, `ValueSuffixAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AnimatingValue`、`ContentFontSize`、`ContentForeground`、`EndValue`、`Value`、`ValuePrefixAddOn`、`ValuePrefixAddOnTemplate`、`ValueSuffixAddOn`、`ValueSuffixAddOnTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `GroupSeparator` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsLoading` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 动效与异步 | `RefreshDuration` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 视觉与格式 | `DecimalSeparator`、`Format`、`GroupSeparator`、`Precision`、`StrokeDashArray` | 维护数值格式、root 虚线边框和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | loading/async、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Statistic Token + ControlTheme。 |

## State Flow

Statistic 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- loading/async、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Statistic 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractStatisticTheme.axaml` | 提供 Statistic 家族共享的字体、颜色、图标和内容 selector 基线，不拥有 `Statistic` 的叶子模板。 |
| `StatisticCountUpTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `StatisticTheme.axaml` | 提供 `Statistic` 叶子模板、root 表面投影、静态 Semantic marker、间距和 loading 状态视觉。 |
| `TimerStatisticTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Statistic 使用 `StatisticToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 loading/async、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不删除或重命名 `root`、`header`、`title`、`content`、`value`、`prefix`、`suffix`，不改变 selector class、ContractType 或 cardinality。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Statistic Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `StatisticToken`，scope id 为 `Statistic`，源码位于 `src/AtomUI.Desktop.Controls/Statistic/StatisticToken.cs`。

## Customization Boundaries

维护 Statistic 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级；Semantic 示例必须保持与对应公开上游 6.6.0 示例一致。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Statistic 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- `root`、`header`、`title`、`content`、`value`、`prefix`、`suffix` 的名称、selector、ContractType、cardinality 和静态 marker 身份。
- `Statistic` 叶子模板与 `TimerStatistic` 独立模板的边界；不得把 Statistic 契约无意发布到 TimerStatistic 或 StatisticCountUp。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
