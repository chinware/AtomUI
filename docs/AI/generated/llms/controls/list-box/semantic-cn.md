# ListBox 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

ListBox 公开 `root` 与 `item` 两个职责区域，与上游稳定 Semantic DOM（`Listy` 组件的
`classNames` / `styles` 均为 `{ root?, item?, groupHeader? }`）对齐。上游 `groupHeader` 不适用于 ListBox——ListBox
是轻量选择列表，只有选择、过滤与 CandidateList 基座职责，没有分组功能，不虚构 Part。`root` 由生成器为带非 root
Part 的 owner 隐式加入，不生成 Style；`item` 是运行时由 ListBox 创建的容器 Part。

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `ListBox` |
| Part | `root` |
| Selector | ListBox 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `ListBox` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | ListBox owner |
| 职责 | 根语义区域，即滚动容器，承载字体、行高、相对定位、外框与外框闭合边界；对应上游 `.ant-listy`。 |
| 相关 API | `ItemsSource`、`ItemTemplate`、`SizeType`、`IsBorderless`、`IsSelectable`、`SelectionMode` |
| 相关 Token | `ListBoxToken`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `ListBox` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `> .semantic-item` |
| Style Type | `ListBoxItemStyle` |
| ContractType | `ListBoxItem` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `ListBoxItem` 容器 |
| 职责 | 条目元素，设置内间距、底部分割线与悬浮背景；对应上游 `.ant-listy-item`。 |
| 相关 API | `SizeType`、`ItemHoverBg`、`ItemSelectedBg` |
| 相关 Token | `ItemPaddingSM`、`ItemPadding`、`ItemPaddingLG`、`ItemHoverBgColor`、`ColorSplit`、`ControlItemBgHover` |
| 稳定性 | stable since 6.0 |

`root` 是隐式 Part，不声明 `.semantic-root` marker。`item` 的 marker `.semantic-item` 在 `ListBoxItem` 创建路径
一次性添加，`PrepareContainerForItemOverride` 幂等补齐（覆盖用户直接提供容器与 `CandidateListItem` 派生容器的
路径）；`ListBoxItem` 只有一种身份，不存在切换。

`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过 `x:SetterTargetType` 提供 AXAML 编译期类型
上下文；它不参与 `.semantic-*` 的身份匹配。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/ListBox/Themes/ListBoxTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <Panel>
        <ScrollViewer Name="PART_ScrollViewer">
            <ItemsPresenter Name="ItemsPresenter" />
        </ScrollViewer>
        <ContentPresenter Name="EmptyIndicator" />
        <Empty Name="DefaultEmptyIndicator" />
    </Panel>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
ListBox
  -> ListBoxItem (item container control theme, ListBoxItemTheme.axaml)
     -> Panel (template-stable)
        -> PixelAlignedBorder#Frame (template-stable)
           -> DockPanel (template-stable)
              -> IconTemplatePresenter#SelectedIndicator (internal-observable)
              -> Panel (template-stable)
                 -> ContentPresenter#ContentPresenter (internal-observable)
                 -> HighlightableTextBlock (template-stable)
        -> PixelAlignedBorder#SplitLineFrame (template-stable)
  -> ListBox (control theme, ListBoxTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> Panel (template-stable)
           -> ScrollViewer#PART_ScrollViewer (template-stable)
              -> ItemsPresenter#ItemsPresenter (internal-observable)
           -> ContentPresenter#EmptyIndicator (internal-observable)
           -> Empty#DefaultEmptyIndicator (template-stable)
  -> CandidateListItem (item container control theme, CandidateListItemTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ListBox` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ListBoxItem` | item container control theme | `ListBoxItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `Content`, `ContentTemplate`, `ContentText`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `ListBoxItemTheme.axaml` | ListBoxItem | `Background`, `BorderBrush`, `Content`, `ContentTemplate`, `ContentText`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (PixelAlignedBorder) | `ListBoxItemTheme.axaml` | ListBoxItem | `Background`, `Content`, `ContentTemplate`, `ContentText`, `CornerRadius`, `FilterHighlightForeground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `ListBoxItemTheme.axaml` | ListBoxItem | `Content`, `ContentTemplate`, `ContentText`, `FilterHighlightForeground`, `FilterHighlightStrategy`, `FilterHighlightWords` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedIndicator` | template node (IconTemplatePresenter) | `ListBoxItemTheme.axaml` | ListBoxItem | `IsSelectedIndicatorVisible`, `SelectedIndicator` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `ListBoxItemTheme.axaml` | ListBoxItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `IsFiltering`, `VerticalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SplitLineFrame` | template node (PixelAlignedBorder) | `ListBoxItemTheme.axaml` | ListBoxItem | `BorderBrush`, `EffectiveBorderThickness`, `IsSplitLineEffectiveVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ListBox` | control theme | `ListBoxTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `CornerRadius`, `EffectiveBorderThickness`, `EmptyIndicator`, `EmptyIndicatorPadding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `ListBoxTheme.axaml` | ListBox | `Background`, `BorderBrush`, `CornerRadius`, `EffectiveBorderThickness`, `EmptyIndicator`, `EmptyIndicatorPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `ListBoxTheme.axaml` | ListBox | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsDefaultEmptyIndicatorVisible`, `IsEffectiveEmptyVisible`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScrollViewer` | template node (ScrollViewer) | `ListBoxTheme.axaml` | ListBox | `IsEffectiveEmptyVisible`, `ItemsPanel`, `ScrollViewer`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `ListBoxTheme.axaml` | ListBox | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `EmptyIndicator` | template node (ContentPresenter) | `ListBoxTheme.axaml` | ListBox | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsEffectiveEmptyVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DefaultEmptyIndicator` | template node (Empty) | `ListBoxTheme.axaml` | ListBox | `EmptyIndicatorPadding`, `IsDefaultEmptyIndicatorVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CandidateListItem` | item container control theme | `CandidateListItemTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |

## Template Parts

| API | 语义 |
| --- | --- |
| `IsSelectable` | 是否允许用户交互更新选择。关闭时清空当前选择。 |
| `SelectionMode` / `SelectedIndex` / `SelectedItem` / `SelectedItems` / `Selection` | 继承 Avalonia `ListBox` 的选择模型。 |
| `SizeType` | 控制整体圆角、空状态 padding 和条目高度、padding。 |
| `IsBorderless` | 是否隐藏 root 边框。 |
| `ItemHoverBg` / `ItemSelectedBg` | 条目 hover 和 selected 背景入口。 |
| `IsShowSelectedIndicator` | 是否在选中条目右侧显示选中标记。 |
| `SelectedIndicator` | 选中标记图标模板入口。 |
| `IsMotionEnabled` | 是否启用条目背景和前景过渡动效。 |
| `EmptyIndicator` / `EmptyIndicatorTemplate` / `EmptyIndicatorPadding` / `IsShowEmptyIndicator` | 空状态展示入口。 |
| `Filter` / `FilterValue` / `FilterValueSelector` | 文本过滤入口。 |
| `FilterHighlightStrategy` | 过滤文本高亮和隐藏未命中项的策略入口。 |
| `FilterResultCount` | 当前过滤命中数量。 |
| `IsFiltering` | 当前是否处于过滤状态。 |
| `FilterHighlightForeground` | 过滤高亮前景色入口。 |

## Pseudo Classes

伪类契约主要继承 Avalonia `ListBoxItem`：

- `:pointerover` 表示条目 hover。
- `:selected` 表示条目被选中。
- `:disabled` 表示条目不可用。

## State Flow

ListBox 的状态模型由选择状态、交互状态、过滤状态、空状态、尺寸状态和动效状态组成。

选择行为：

- `IsSelectable=false` 时，ListBox 不响应 pointer 或 keyboard 选择更新，并清空 `SelectedIndex`、`SelectedItem` 和 `SelectedItems`。
- `SelectionMode` 继承 Avalonia `ListBox` 语义，单选使用 `SelectedItem`，多选使用 `SelectedItems`。
- 方向键按 Avalonia navigation direction 移动选择，`Shift` 支持范围选择，平台 select-all 手势在多选模式下选择全部。
- `IsShowSelectedIndicator=true` 时，选中容器右侧显示 `SelectedIndicator`。

点击行为：

- `ListBoxItem.Clicked` 是 routed event，ListBox 通过 class handler 收敛到 `ItemClicked`。
- 派生控件可重写 `NotifyListBoxItemClicked` 承接点击行为，例如 CandidateList 在单选场景中提交候选项。

过滤行为：

- `Filter`、`FilterValue` 和 `FilterValueSelector` 共同决定条目是否命中。
- `IsFiltering=true` 时条目进入过滤展示状态，并通过 `FilterHighlightStrategy` 决定高亮和隐藏未命中项。
- `FilterResultCount` 表示命中数量；过滤模式下空状态依据 `FilterResultCount == 0` 判断。
- 过滤展示主要面向文本内容。复杂自定义 `ItemTemplate` 需要维护者明确处理过滤态展示入口，避免过滤态绕过用户模板。

空状态行为：

- `ItemCount == 0` 时显示空状态。
- `IsFiltering && FilterResultCount == 0` 时显示过滤空状态。
- `IsShowEmptyIndicator=false` 时不显示空状态内容。

动效行为：

- ListBoxItem 初始化阶段禁用 transitions，loaded 后启用，避免初始状态产生非预期动画。
- `IsMotionEnabled=false` 时不应用背景和前景过渡。

## Theme and Token Boundaries

ListBox 主题按 root 和 item 两层组织。

```text
ListBoxTheme
  Frame（ClipContentToCornerRadius=True）
  PART_ScrollViewer
  ItemsPresenter
  EmptyIndicator

ListBoxItemTheme
  Frame
  SelectedIndicator
  ContentPresenter
  HighlightableTextBlock
  SplitLineFrame
```

视觉规则：

- root 外框由 `BorderBrush`、`BorderThickness`、`CornerRadius` 和 `IsBorderless` 共同决定。默认外框颜色是 `ColorSplit`，与条目分割线同色，表达“外框即列表闭合线”。
- root `Frame` 开启 `ClipContentToCornerRadius`：内容被裁剪到外框圆角内边缘，条目 hover / selected 背景等溢出圆角内边缘的内容被裁剪，不在圆角口袋区溢出；外框环由 `Frame` 自身一次绘制（`ColorSplit`），无叠加节点。
- `SizeType` 控制 root 圆角、空状态 padding、条目最小高度和条目 padding。条目表面保持直角，主题不设置条目圆角。
- 条目直接贴合 root 外框内边缘：`ContentPadding` 与 `ItemMargin` 均为 `Thickness(0)`，条目之间不留垂直间距，列表紧凑感由条目高度与分割线表达。
- 条目底部分割线默认是 1 DIP `ColorSplit` 底边线，由条目 `BorderThickness` / `BorderBrush` 驱动。最后一项的底部分割线被抑制，由 root 外框下边缘承担闭合线；`IsBorderless` 时保留（外框消失后由分割线承担闭合线）。
- 默认条目背景透明，hover 使用 `ItemHoverBg`，selected 使用 `ItemSelectedBg`；hover / selected 背景延伸到外框内边缘，溢出圆角口袋区的部分由 `Frame` 的圆角内容裁剪约束。
- disabled 内容使用 SharedToken disabled 文本色。
- 选中指示器默认使用 默认 `CheckOutlined`，颜色使用 SharedToken 主色，尺寸使用 SharedToken icon size。
- 空状态默认使用 `Empty` 的 simple preset image。

ListBoxToken 提供内容 padding、条目文字颜色、条目状态背景、条目 padding、条目 margin、选中指示器 margin 和过滤高亮色。Token 详情见 [ListBox Token 设计](token.md)。

Token 边界：

ListBoxToken 是 ListBox 的组件级设计变量层。它把全局颜色、尺寸、间距和状态色转换为 ListBox root、ListBoxItem、selected indicator 和 filter highlighter 可消费的语义值。

ListBoxToken 服务以下主题和控件：

- `ListBoxTheme.axaml`
- `ListBoxItemTheme.axaml`
- `CandidateListTheme.axaml`
- `CandidateListItemTheme.axaml`
- `CascaderViewFilterListTheme.axaml`
- `ListBox` / `ListBoxItem` / `CandidateList` / `CandidateListItem`

ListBoxToken 不承载 `SelectedItem`、`SelectedItems`、`IsSelected`、`IsFiltering`、`FilterValue`、`FilterResultCount`、`IsEffectiveEmptyVisible`、`VirtualIndex` 等实例状态。这些状态由 C# 状态模型、容器属性和主题 selector 处理。

## Customization Boundaries

维护 ListBox 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 ListBox / ListBoxItem public API、事件和 Avalonia 属性语义。
- 不破坏 Avalonia `ListBox` 的 `SelectionMode`、`SelectedItem`、`SelectedItems`、`Selection` 和 keyboard navigation 语义。
- `IsSelectable=false` 必须阻止用户选择更新，并清空当前选择。
- `ItemClicked` 必须继续从 ListBoxItem routed event 收敛，保持 CandidateList 的提交入口稳定。
- `PART_ScrollViewer`、`ItemsPresenter`、`EmptyIndicator`、`SelectedIndicator` 和 `ContentPresenter` 的职责不得被无兼容说明地改变。
- 选中指示器、过滤高亮和空状态节点应留在 AXAML 静态模板中，通过 `IsVisible` 和状态属性控制，不作为普通性能优化迁移到 C# 动态创建。
- 虚拟化容器回收时，容器本地值必须对称清理，避免旧 item 的 disabled、filter、indicator 或 content 状态泄漏到新 item。
- 筛选命中数量和空状态契约不得只依赖当前已实现容器。
- 语义区域边界：`ListBox` 只发布 `root` / `item` 两个 Part；`item` 的 marker 在容器创建与 prepare 路径一次性幂等
  建立、不随状态切换，静态模板不增删 marker，默认主题不消费 `.semantic-*` selector；selection、filter 高亮与
  empty 不属于 Part。
- root 外框与条目分割线必须保持同一 `ColorSplit` 基线；root `Frame` 的 `ClipContentToCornerRadius` 圆角内容裁剪保证条目状态背景不溢出圆角内边缘。
- 条目表面保持直角与贴边：不恢复条目圆角，不重新引入 `ContentPadding` / `ItemMargin` 垂直间距。
- 最后一项分割线抑制规则由控件状态机决定：最后一项不显示底部分割线，由 root 外框下边缘闭合；`IsBorderless` 时
  保留；集合追加与删除后必须重新同步所有已实现容器，Semantic Style 不能绕过该规则。
- Token 只表达组件设计语义，不承载选择、过滤、空状态或虚拟化运行时状态。

维护不变量：

内部重构必须保持以下不变量：

- `ListBox.cs` 保留 public API、事件、容器生命周期和虚拟化上下文入口。
- `ListBoxItem.cs` 保持容器角色，不承载业务数据源请求、排序、分页或跨列表全局状态。
- `PrepareContainerForItemOverride` 新增状态同步时，必须在 `ClearContainerForItemOverride` 或 `NotifyClearContainerForVirtualizingContext` 对称清理。
- `FilterValueSelector` 是复杂数据项的过滤值入口，不应在 ListBox 内部硬编码业务数据类型。
- `FilterHighlightStrategy` 只控制过滤展示，不改变 selection model。
- 选中指示器可见性只能由 `IsShowSelectedIndicator && IsSelected` 推导。
- `ItemClicked` 派发顺序必须允许 CandidateList 在 public event 前执行 `NotifyListBoxItemClicked`。
- `IsBorderless` 只影响边框厚度，不改变 root padding、corner radius 或 scroll behavior。
- Semantic Part 边界：`ListBox` 只发布 `root` / `item` 两个 Part；`item` 的 marker 在容器创建与 prepare 路径一次性
  幂等建立、不随状态切换，静态模板不增删 marker，默认主题不消费 `.semantic-*` selector；selection、filter 高亮与
  empty 不属于 Part。
- 分割线状态机：`IsSplitLineVisible` 只由 ListBox 按容器视图位置与 `IsBorderless` 计算，容器不得自行决定；集合变化
  与 borderless 变化必须重新同步已实现容器；Semantic Style 不能绕过最后一项抑制。
- 条目模板保持 `SplitLineFrame` 绑定 `EffectiveBorderThickness` / `IsSplitLineEffectiveVisible`，默认分割线为 1 DIP
  `ColorSplit`；root 外框与分割线共享同一 `ColorSplit` 基线。
- root 模板保持 `Frame` 开启 `ClipContentToCornerRadius`：内容被裁剪到外框圆角内边缘，条目状态背景不溢出圆角
  口袋区；外框环由 `Frame` 自身一次绘制（`ColorSplit`，与分割线同色），不得再叠加任何覆盖节点。裁剪应用前必须通过
  `SupportsGeometryClipHitTesting` 探测平台几何命中能力，无法正确判定圆角几何包含的平台降级为不应用裁剪。
- 条目表面保持直角与贴边：不恢复条目圆角，`ContentPadding` / `ItemMargin` 保持 `Thickness(0)`。
- Token 变更必须同步 `ListBoxTokenKind`、AXAML 引用和 token.md 语义说明。
