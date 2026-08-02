# Select

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Select 是 AtomUI 桌面数据录入体系中的选项选择控件，用于在受控选项集合中完成单选、多选和标签化选择。它由输入壳体、已选结果区域、过滤输入、候选弹层、候选列表、异步加载和 Form / CompactSpace 集成组成，面向表单、筛选器、配置项和可搜索选项输入场景。

Select 的职责是选择一个或多个 `ISelectOption`，或在 `Tags` 模式下基于用户输入临时创建动态选项。它不负责远程服务协议、权限过滤、业务对象持久化、复杂树形选择、级联选择或富文本标签编辑；这些能力应由业务层或 TreeSelect / Cascader 等专用控件承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Select` |
| 状态 | Stable |

## 何时使用

Select 的设计语言来自输入框、已选结果和候选弹层的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 选择模式 | 控件选择单个选项、多个选项或允许用户创建标签。 | `Mode=Single/Multiple/Tags`。 |
| 选项数据 | 候选项可来自本地集合、内容子项或异步 loader。 | `OptionsSource`、`Options`、`OptionsLoader`。 |
| 选择结果 | 单选结果显示为文本，多选和 tags 显示为可关闭标签。 | `SelectedOption`、`SelectedOptions`、`MaxTagCount`。 |
| 搜索过滤 | 输入搜索文本后过滤候选项。 | `IsFilterEnabled`、`FilterValue`、`Filter`、`FilterValueSelector`。 |
| 输入表面 | 控件边框、背景、尺寸、状态和附加内容。 | `StyleVariant`、`Status`、`SizeType`、Addon。 |
| 弹层语义 | 候选列表按 placement 打开，并可匹配控件宽度。 | `IsDropDownOpen`、`Placement`、`IsPopupMatchSelectWidth`。 |
| 异步加载 | 打开控件时按上下文加载候选数据。 | `OptionsLoader`、`OptionsAsyncLoadContext`、`IsLoading`。 |

`Custom` 尺寸不是 Select 的第四套专属 Token。主题层把 `SizeType=Custom` 归入 `Middle` 的默认字体和 padding 分支；用户显式设置尺寸属性时由 Avalonia 属性优先级决定最终布局。

## 公共 API

Select 的公共 API 分布在 `AbstractSelect` 和 `Select` 两层。`AbstractSelect` 提供输入壳体、弹层、清除、状态、尺寸、Addon、Form 和 CompactSpace 契约；`Select` 提供选项、选择、过滤、分组、异步加载和模式契约。

选项与选择 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Mode` | `SelectMode` | 选择模式，默认 `Single`。 |
| `OptionsSource` | `IEnumerable<ISelectOption>?` | 外部候选项集合。Select 只读取该集合，不把 Tags 运行时动态选项写回该集合。 |
| `Options` | `ItemCollection` | XAML 内容子项入口。它表达用户声明的静态候选项，不承载 Tags 模式运行时动态选项。 |
| `OptionTemplate` | `IDataTemplate?` | 候选项显示模板，默认显示 `ISelectOption.Header`。 |
| `SelectedOption` | `ISelectOption?` | 单选模式当前选项，默认 `TwoWay` 绑定并启用 Avalonia data validation。 |
| `SelectedOptions` | `IList<ISelectOption>?` | 多选和 Tags 模式当前选项集合，默认 `TwoWay` 绑定并启用 Avalonia data validation。 |
| `DefaultValues` | `IList<object>?` | 加载后按值匹配默认选中项。 |
| `DefaultValueCompareFn` | `Func<object, ISelectOption, bool>?` | 默认值匹配自定义比较函数。 |
| `SelectionChanged` | event | `SelectedOption` 或 `SelectedOptions` 改变时触发。 |

过滤与列表 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsFilterEnabled` | `bool` | 是否启用搜索过滤。`Tags` 模式始终形成有效过滤能力。 |
| `FilterValue` | `object?` | 当前过滤输入值。 |
| `Filter` | `IValueFilter?` | 候选项过滤器。为空时使用 ListView 默认过滤行为。 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | 从候选项中提取过滤文本。默认使用 `HeaderFilterPropertySelector`。 |
| `IsGroupEnabled` | `bool` | 是否启用候选项分组。 |
| `GroupPropertySelector` | `DefaultFilterValueSelector?` | 分组值 selector。 |
| `IsDefaultActiveFirstOption` | `bool` | 当前 public surface 的一部分；维护时需要先确认候选激活路径再改变其语义。 |
| `IsHideSelectedOptions` | `bool` | 多选候选列表中隐藏已选项。 |
| `AutoScrollToSelectedOptions` | `bool` | 候选列表打开或选择同步时滚动到已选项。 |
| `DisplayPageSize` | `int` | 候选弹层可视行数，用于计算最大高度，默认 `10`。 |
| `MaxCount` | `int` | 多选最大可选数量，默认 `int.MaxValue`。 |

选择绑定语义：

- `SelectedOption` 是 `Mode=Single` 的唯一选择状态入口；用户选择、清除、Form 写值和 ViewModel 写值都通过该属性同步。
- `SelectedOptions` 是 `Mode=Multiple/Tags` 的唯一选择状态入口；用户选择变化会回写绑定源，绑定源替换集合或对 `INotifyCollectionChanged` 集合执行 `Add` / `Remove` / `Reset` 时，Select 会同步标签、候选列表和计数状态。
- `OptionsSource` 始终只是候选项来源。即使 `SelectedOptions` 绑定到可变集合，Tags 模式运行时动态选项也只进入选择集合和内部有效候选集合，不写回 `OptionsSource`。

弹层与异步 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsDefaultOpen` | `bool` | 当前 public surface 的一部分；默认打开行为需与 `IsDropDownOpen` 状态保持兼容。 |
| `IsDropDownOpen` | `bool` | 候选弹层打开状态。 |
| `Placement` | `SelectPopupPlacement` | 候选弹层展开位置，默认 `BottomEdgeAlignedLeft`。 |
| `IsPopupMatchSelectWidth` | `bool` | 弹层最小宽度是否匹配 Select 宽度，默认 `true`。 |
| `ShouldUseOverlayPopup` | `bool` | 是否使用 overlay popup 宿主，默认 `true`。 |
| `OptionsLoader` | `ISelectOptionsAsyncLoader?` | 异步候选加载器。 |
| `OptionsAsyncLoadContext` | `object?` | 传给异步 loader 的上下文。 |
| `AsyncLoadTimeout` | `TimeSpan` | 异步加载超时时间，默认 `10` 秒。 |
| `IsLoading` | `bool` | 异步加载或 loading 状态，只读公开。 |
| `OptionsLoading` / `OptionsLoaded` | event | 开始加载和加载完成通知。 |
| `DropDownOpening` / `DropDownClosing` | event | 弹层打开/关闭前通知，可取消。 |
| `DropDownOpened` / `DropDownClosed` | event | 弹层打开/关闭后通知。 |

输入表面 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SizeType` | `CustomizableSizeType` | 输入尺寸密度，支持 `Large/Middle/Small/Custom`。 |
| `StyleVariant` | `InputControlStyleVariant` | 输入表面样式。 |
| `Status` | `InputControlStatus` | 手动输入反馈状态；native validation error 以 `DataValidationErrors` 为最高优先级。 |
| `PlaceholderText` / `PlaceholderForeground` | `string?` / `IBrush?` | 空选择时的占位文本和颜色。 |
| `IsAllowClear` / `ClearIcon` | `bool` / `PathIcon?` | 清除入口和图标。 |
| `SuffixIcon` / `SuffixLoadingIcon` | `PathIcon?` | 普通展开指示和 loading 指示。 |
| `LeftAddOn` / `RightAddOn` | `object?` | 外部左右 AddOn。 |
| `ContentLeftAddOn` / `ContentRightAddOn` | `object?` | 内部左右内容。 |
| `IsMotionEnabled` | `bool` | 输入壳体、handle、候选列表和 popup 动效开关。 |
| `IsShowOverflowTip` | `bool` | 选中结果文本或多选 tag 视觉溢出时是否显示完整内容 tooltip，默认 `true`。 |
| `OverflowTipDelay` | `int` | 溢出 tooltip 打开前的延迟时间，单位毫秒，默认 `1200`。 |
| `OverflowTipPlacement` | `PlacementMode` | 溢出 tooltip 相对选中结果文本或多选 tag 的位置，默认 `TopEdgeAlignedLeft`。 |

多选标签 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `MaxTagCount` | `int?` | 最大直接展示标签数量。 |
| `IsResponsiveTagMode` | `bool` | 使用响应式标签布局。 |
| `MaxTagPlaceholder` | `string?` | 当前 public surface 的一部分；默认模板使用剩余数量标签表达折叠信息。 |
| `IsShowMaxCountIndicator` | `bool` | 是否展示最大数量指示。 |

候选项契约：

| 类型 | 语义 |
| --- | --- |
| `ISelectOption.Header` | 候选项显示内容。 |
| `ListItemData.Content` | 候选项值和默认值匹配的主要输入。 |
| `ListItemData.ItemKey` | 候选项稳定标识，优先用于选项替换后的选择映射。 |
| `ListItemData.IsEnabled` | 候选项启用状态。 |
| `ISelectOption.IsDynamicAdded` | Tags 模式下用户输入生成的运行时动态选项标记。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddOnDecoratedBox` | `SelectAddOnDecoratedBox` | 输入壳体、Addon、variant、status、CompactSpace 和 hover/pressed 状态承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选模式结果显示和搜索输入。 |
| `SelectedOptionsBox` | `SelectResultOptionsBox` | 多选和 Tags 模式已选标签与搜索输入。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PopupFrame` | `Border` | 懒创建的候选弹层外壳。 |
| `PART_CandidateList` | `SelectCandidateList` | 懒创建的候选项列表、键盘导航和提交/取消。 |

Select 的稳定伪类包括 `:dropdownopen`，同时通过标准 `:pressed`、`:disabled` 和 AddOnDecoratedBox 相关状态表达输入表面视觉。

## 事件与命令

| `SelectionChanged` | event | `SelectedOption` 或 `SelectedOptions` 改变时触发。 |
| `OptionsLoading` / `OptionsLoaded` | event | 开始加载和加载完成通知。 |
| `DropDownOpening` / `DropDownClosing` | event | 弹层打开/关闭前通知，可取消。 |
| `DropDownOpened` / `DropDownClosed` | event | 弹层打开/关闭后通知。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml:38`

Gallery key：`ExamplesContent` / item `0`

```axaml
<WrapPanel ItemSpacing="10" LineSpacing="10">
    <atom:Select Mode="Single" PlaceholderText="请选择" Width="120"
                 DefaultValues="lucy"
                 OptionsSource="{Binding BasicSelectedOptions}" />

    <atom:Select Mode="Single" PlaceholderText="请选择" IsEnabled="False" Width="120"
                 DefaultValues="lucy"
                 OptionsSource="{Binding SingleLucyOptions}" />

    <atom:Select Name="AsyncLoadSelect"
                 Mode="Single"
                 PlaceholderText="请选择"
                 Width="120"
                 DefaultValues="lucy"
                 OptionsSource="{Binding SingleLucyOptions}"
                 OptionsLoader="{Binding SelectOptionsAsyncLoader}" />

    <atom:Select Mode="Single" PlaceholderText="请选择" IsAllowClear="True" Width="120"
                 OptionsSource="{Binding SingleLucyOptions}" />

    <atom:Select Name="DefaultSelectedSelect"
                 Mode="Single"
                 PlaceholderText="请选择"
                 IsAllowClear="True"
                 Width="120"
                 OptionsSource="{Binding BasicSelectedOptions}"
                 SelectedOption="{Binding DefaultSelectedOption}" />

</WrapPanel>
```

### 双向绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml:79`

Gallery key：`ExamplesContent` / item `1`

```axaml
<WrapPanel ItemSpacing="24" LineSpacing="16">
    <StackPanel Spacing="8" MinWidth="260">
        <TextBlock Text="SelectedOption"
                   FontWeight="SemiBold" />
        <atom:Select Mode="Single"
                     PlaceholderText="请选择"
                     IsAllowClear="True"
                     Width="220"
                     OptionsSource="{Binding BasicSelectedOptions}"
                     SelectedOption="{Binding BoundSelectedOption}" />
        <WrapPanel ItemSpacing="8">
            <atom:Button SizeType="Small"
                         Command="{Binding SetBoundSelectedOptionCommand}"
                         Content="设为 Lucy" />
            <atom:Button SizeType="Small"
                         Command="{Binding ClearBoundSelectedOptionCommand}"
                         Content="清空" />
        </WrapPanel>
        <TextBlock Text="ViewModel 值" />
        <TextBlock Text="{Binding BoundSelectedOptionText}" />
    </StackPanel>

    <StackPanel Spacing="8" MinWidth="320">
        <TextBlock Text="SelectedOptions"
                   FontWeight="SemiBold" />
        <atom:Select Mode="Multiple"
                     PlaceholderText="请选择人员"
                     IsAllowClear="True"
                     IsFilterEnabled="True"
                     Width="300"
                     OptionsSource="{Binding BasicSelectedOptions}"
                     SelectedOptions="{Binding BoundSelectedOptions}" />
        <WrapPanel ItemSpacing="8">
            <atom:Button SizeType="Small"
                         Command="{Binding SetBoundSelectedOptionsCommand}"
                         Content="设为 Jack + Yiminghe" />
            <atom:Button SizeType="Small"
                         Command="{Binding ClearBoundSelectedOptionsCommand}"
                         Content="清空" />
        </WrapPanel>
        <TextBlock Text="ViewModel 值" />
        <TextBlock Text="{Binding BoundSelectedOptionsText}"
                   TextWrapping="Wrap" />
    </StackPanel>
</WrapPanel>
```

### 带搜索框的选择器

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml:135`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:Select Mode="Single"
```

### 自定义搜索

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml:150`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:Select Name="CustomSearchSelect"
```

## 状态模型

Select 的核心状态流：

```text
OptionsSource / Options / OptionsLoader
      ↓
用户选项源
      ↓
Effective options = 用户选项源 + Tags 运行时动态选项
      ↓
SelectCandidateList
      ↓
SelectedOption / SelectedOptions
      ↓
SingleFilterInput / SelectedOptionsBox
      ↓
Form value + SelectionChanged
```

选择模式语义：

- `Single` 使用 `SelectedOption` 作为唯一表单值，内部单行过滤输入负责展示当前 `Header`。
- `Multiple` 使用 `SelectedOptions` 作为表单值，已选项以 `SelectTag` 展示。
- `Tags` 以 `Multiple` 为基础，始终启用有效过滤，并在过滤结果为空且输入非空时创建 `IsDynamicAdded=true` 的运行时动态选项。

弹层交互优先级：

```text
Disabled / invisible / window deactivated
> DropDownClosing cancellation
> DropDownOpening cancellation
> IsDropDownOpen
> pointer / keyboard open request
```

键盘行为：

- 弹层打开时按键优先交给 `SelectCandidateList.HandleKeyDown()`。
- `Enter` 在单选模式提交候选项，在多选模式切换候选项选中状态。
- `Escape` 取消候选并关闭弹层。
- `F4` 或 `Alt+Up/Down` 切换弹层。
- 弹层关闭时，`Up/Down`、`Enter`、`Space` 可打开弹层。
- 多选和 Tags 模式下，过滤输入为空时 `Backspace/Delete` 删除最后一个已选项。

清除行为通过 `SelectHandle.ClearRequestedEvent` 冒泡到 Select，并调用 `ClearValue()` 清空 `SelectedOption` 和 `SelectedOptions`。

## 主题与 Design Token

Select 的默认视觉由 Select 专属主题、AddOnDecoratedBox、ListView 和 PopupHost 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `SelectTheme.axaml` | 根模板、输入壳体、单选输入、多选结果区域、handle、popup 和基础 selector。 |
| `SelectAddOnDecoratedBoxTheme.axaml` | 输入框 variant、status、dropdown open、hover、pressed 和多选 padding。 |
| `SelectResultOptionsBoxTheme.axaml` | 多选标签布局、响应式标签布局和搜索输入承载。 |
| `SelectCandidateListTheme.axaml` | 候选列表基础 ListView 主题和默认候选模板。 |
| `SelectCandidateListItemTheme.axaml` | 候选项 active、selected、disabled 和隐藏已选项视觉。 |
| `SelectTagTheme.axaml` | 多选标签高度、背景、关闭按钮和禁用态。 |
| `SelectHandleTheme.axaml` | 展开、loading、清除、过滤指示和 Form feedback 图标。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `SelectToken` | 多选标签、候选项、popup padding 和输入 padding。 |

选中结果的完整内容提示复用 `OverflowTip` attached behavior。模板只在单选文本和多选 tag 上声明 `IsShowOverflowTip` / `OverflowTipDelay` / `OverflowTipPlacement`，实际 tooltip 只在文本视觉宽度不足时写入 `ToolTip.Tip`，且不会覆盖用户手动设置的 tooltip。

候选弹层内容采用懒创建模型。`PART_Popup` 属于模板稳定 part；`PopupFrame` 和 `PART_CandidateList` 在打开前由 C# 创建并设置 `TemplatedParent`，关闭或重新套用模板时释放引用和事件订阅。

Token 来源：

SelectToken 是 Select 的控件级 Token scope，描述多选标签、候选项、候选弹层 padding 和 Select 输入内容 padding。输入壳体的通用边框、圆角、状态色、focus ring、disabled 背景和 AddOn 结构来自 SharedToken、AddOnDecoratedBoxToken 和 PopupHostToken。

SelectToken 不承载以下状态：

- `OptionsSource`、`Options`、`SelectedOption`、`SelectedOptions`、`DefaultValues` 等数据状态。
- `FilterValue`、`IsDropDownOpen`、`IsLoading`、`Status`、`Mode` 等运行状态。
- 候选项 hover、pressed、selected、disabled 的当前实例状态。
- Popup 当前宽度、高度、offset、placement 或异步加载结果。

## AOT 与裁剪注意事项

Select 不依赖运行时反射发现模板结构。模板协作通过固定 template part、显式类型、属性绑定和事件完成。

资源和生命周期边界：

- `SubscriptionsOnOpen` 只在 popup 打开期间持有可见性订阅，popup 关闭时清空。
- `_deactivationSubscription` 在 attach 时创建，detach 时释放。
- `_selectHandleInputStateBindings` 每次模板接入前释放旧绑定，仅持有 AddOnDecoratedBox → SelectHandle 的 hover / pressed sibling part 状态转发。
- `_candidateList` 的事件订阅和 `ItemsSource` 必须在 `ClearPopupContent()` 中释放。
- `SelectHandle` 订阅 `FormFeedback.ValidateStatus` 时必须在 feedback 变化和 logical detach 时释放。
- 异步加载通过 `AsyncSearchLoadCoordinator` 处理超时、取消和跳过旧结果。

AOT 边界：

- `SelectToken` 通过 token generator 显式注册，生成 `SelectTokenKind` 和 `SelectTokenResourceExtension`。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- `OptionTemplate`、AddOn 模板和 EmptyIndicator 模板是 XAML 模板入口，不依赖运行时成员扫描。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs`：输入壳体、弹层状态、公共输入属性、Form / CompactSpace / Motion 接口和 popup 生命周期。
- `src/AtomUI.Desktop.Controls/Select/Select.cs`：public Select API、protected 扩展 hook、用户选项源同步、有效候选选项同步、选择同步、过滤输入、Tags 动态选项、键盘和指针处理。
- `src/AtomUI.Desktop.Controls/Select/Select.AsyncOptionsLoad.cs`：异步候选加载和私有加载完成流程。
- `src/AtomUI.Desktop.Controls/Select/SelectOption.cs`：`ISelectOption` 和默认 `SelectOption`。
- `src/AtomUI.Desktop.Controls/Select/SelectCandidateList.cs`：候选列表、候选导航、提交取消、最大选择数和隐藏已选项。
- `src/AtomUI.Desktop.Controls/Select/SelectCandidateListItem.cs`：候选项容器状态。
- `src/AtomUI.Desktop.Controls/Select/SelectResultOptionsBox.cs`：多选结果标签和过滤输入承载。
- `src/AtomUI.Desktop.Controls/Select/SelectHandle.cs`：右侧展开、loading、清除和 Form feedback 图标。
- `src/AtomUI.Desktop.Controls/Tooltip/OverflowTip.cs`：共享溢出 tooltip attached behavior，供单选结果和多选 tag 复用。
- `src/AtomUI.Desktop.Controls/Select/DataLoad/*`：异步候选加载接口、结果和事件参数。
- `src/AtomUI.Desktop.Controls/Select/SelectToken.cs`：Select 控件 Token。
- `src/AtomUI.Desktop.Controls/Select/Themes/*.axaml`：Select 根模板、候选列表、结果标签、handle、输入壳体和 token 样式。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/select/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/select/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/select/token.md`
- 变更记录：`docs/controls/desktop/data-entry/select/changelog.md`
- 语义结构：`./semantic-cn.md`
