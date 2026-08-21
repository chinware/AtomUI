# Cascader

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Cascader 是 AtomUI 桌面数据录入体系中的级联选择控件，用于在有明确父子层级的数据中按列逐级展开并选择一个或多个选项。它面向地区、组织、分类、资源路径、权限域等“路径就是业务含义”的选择场景。

Cascader 由外层输入控件和内部级联弹层组成。外层 `Cascader` 负责输入表面、popup、选择结果、清除、Form、CompactSpace 和 AddOn 集成；内部 `CascaderView` 负责级联列、展开收起、过滤结果、异步加载和多选勾选。

Cascader 不负责树节点编辑、拖拽排序、远程协议封装、业务权限过滤、复杂图形化层级展示或任意树操作。这些能力应由业务层、TreeView、TreeSelect 或专用数据控件承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader` |
| 状态 | Stable |

## 何时使用

Cascader 的设计语言来自输入框、路径选择和多列层级弹层的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 层级数据 | 选项按 parent / children 组成路径。 | `OptionsSource`、`Options`、`ICascaderOption.Children`。 |
| 逐列展开 | 每一级子选项以独立列展示，同级只保持一个展开分支。 | `CascaderViewLevelList`、`ExpandTrigger`。 |
| 选择模式 | 支持单选和多选勾选。 | `IsMultiple`、`SelectedOption`、`SelectedOptions`。 |
| 勾选展示 | 多选结果可展示全部、父级或叶子节点。 | `ShowCheckedStrategy`、`EffectiveSelectedOptions`。 |
| 父级可选 | 非叶子节点可作为选择结果。 | `IsAllowSelectParent`。 |
| 异步加载 | 展开未加载节点时按节点加载子项。 | `DataLoader`、`LoadingIcon`、内部 `CascaderView.AsyncLoadTimeout`。 |
| 搜索过滤 | 输入过滤值后展示完整路径结果。 | `Filter`、`FilterValue`、`FilterHighlightStrategy`。 |
| 输入表面 | 继承输入家族的尺寸、variant、status、Addon、清除和 Form feedback。 | `SizeType`、`StyleVariant`、`Status`、Addon、`SelectHandle`。 |

`SizeType=Custom` 不是 Cascader 的第四套专属 Token。主题层把 `Custom` 归入 `Middle` 默认字体和 padding 分支；用户显式设置高度等尺寸属性时由 Avalonia 属性优先级决定最终布局。

## 公共 API

Cascader 通过 `OptionsSource` 和 `Options` 接收 `ICascaderOption` 数据。`CascaderOption` 是轻量默认选项模型，适合代码直接构造和普通层级数据。`BindableCascaderOption` 是绑定型选项模型，继承 `AvaloniaObject` 并暴露 DirectProperty，用于选项属性需要承载 XAML binding、`DynamicResource` 或 owner scoped resource 的场景。

### 3.1 数据模型契约

| 类型 | 语义 |
| --- | --- |
| `ICascaderOption` | Cascader 数据节点契约，包含 header、icon、enabled、checked、checkbox enabled、expanded、leaf、value、children 和 parent。 |
| `CascaderOption` | 默认轻量选项模型，不继承 `AvaloniaObject`，保持普通数据对象定位。 |
| `BindableCascaderOption` | 绑定型选项模型，支持 Avalonia binding target 和 scoped resource host 生命周期。 |
| `ICascaderItemDataLoader` | 节点异步加载边界，按目标节点返回子选项集合和加载状态。 |

`ICascaderOption.Header` 是默认显示内容；`ItemKey` 优先作为稳定 identity；`Value` 是 identity 兜底和业务值承载；`Children` 是层级结构入口；`ParentNode` 用于路径回溯、展开路径和显示路径计算。

### 3.2 选项与选择 API

| API | 类型 | 语义 |
| --- | --- | --- |
| `OptionsSource` | `IEnumerable<ICascaderOption>?` | 外部选项集合。变化时同步到内部 `Options`。 |
| `Options` | `ItemCollection` | XAML 内容子项入口，也是 CascaderView 实际数据入口。 |
| `OptionTemplate` | `IDataTemplate?` | 选项显示模板，默认显示 `ICascaderOption.Header` 并继续以 option 为 DataContext。 |
| `SelectedOption` | `ICascaderOption?` | 单选当前选项，默认 `TwoWay` binding，并启用 Avalonia data validation。 |
| `SelectedOptions` | `IList<ICascaderOption>?` | 多选当前选项集合，默认 `TwoWay` binding，并启用 Avalonia data validation。 |
| `DefaultSelectOptionPath` | `TreeNodePath?` | 单选默认路径。路径段按 `ItemKey` 优先、`Value` 兜底匹配。 |
| `SelectedOptionPath` | internal `string?` | 单选显示路径文本，由已选节点 header 路径组成。 |
| `Clear()` | method | 清空单选、多选和显示路径。 |

### 3.3 行为 API

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsMultiple` | `bool` | 是否启用多选勾选。启用后 `CascaderView.IsCheckable=true`。 |
| `ShowCheckedStrategy` | `TreeSelectCheckedStrategy` | 多选 tag 展示策略，不改变真实 `SelectedOptions`。 |
| `MaxCount` | inherited `int` | 多选最大可选数量。达到上限后未选项进入最大数限制状态。 |
| `IsAllowSelectParent` | `bool` | 是否允许非叶子节点作为选择结果。 |
| `ExpandTrigger` | `CascaderViewExpandTrigger` | 级联展开触发方式，默认 `Click`。 |
| `DataLoader` | `ICascaderItemDataLoader?` | 展开未加载节点时的异步数据加载器。 |
| `CascaderView.AsyncLoadTimeout` | internal view API | 内部弹层 view 的异步加载超时边界，不是外层 `Cascader` 当前 public property。 |
| `Filter` | `IValueFilter?` | 路径过滤器；为空时初始化为 contains 过滤。 |
| `FilterValue` | inherited `object?` | 当前过滤输入值。 |
| `FilterHighlightStrategy` | `TextBlockHighlightStrategy` | 过滤命中高亮策略。 |
| `FilterHighlightForeground` | `IBrush?` | 过滤命中高亮前景色。 |

### 3.4 输入表面 API

Cascader 继承 `AbstractSelect` 的输入表面契约：

| API | 语义 |
| --- | --- |
| `SizeType` | 输入尺寸密度，支持 `Large / Middle / Small / Custom`。 |
| `StyleVariant` | 输入表面样式。 |
| `Status` | 手动输入反馈状态；warning 仅在无 native validation error 时驱动 `:warning`，error 视觉优先由 `DataValidationErrors` 驱动。 |
| `PlaceholderText` / `PlaceholderForeground` | 空选择时的占位文本和颜色。 |
| `IsFilterEnabled` | 是否显示过滤输入并将输入同步为 `FilterValue`。 |
| `IsShowOverflowTip` | 单选路径或多选 tag 视觉溢出时是否显示完整内容 tooltip，默认 `true`。 |
| `OverflowTipDelay` | 溢出 tooltip 打开前的延迟时间，单位毫秒，默认 `1200`。 |
| `OverflowTipPlacement` | 溢出 tooltip 相对单选路径或多选 tag 的位置，默认 `TopEdgeAlignedLeft`。 |
| `IsAllowClear` / `SuffixIcon` / `SuffixLoadingIcon` | 清除、展开指示和 loading 指示入口。 |
| `LeftAddOn` / `RightAddOn` / `ContentLeftAddOn` / `ContentRightAddOn` | 外部和内部 AddOn 内容。 |
| `IsDropDownOpen` / `PopupPlacement` / `ShouldUseOverlayPopup` / `IsPopupMatchSelectWidth` | popup 打开、定位、宿主和宽度匹配契约。 |
| `DropDownOpening` / `DropDownOpened` / `DropDownClosing` / `DropDownClosed` | 继承自 `AbstractSelect` 的 popup 生命周期事件。 |

### 3.5 内部 view 事件契约

`CascaderView` 是内部弹层控件，当前事件用于 `Cascader` 和测试协作，不作为外层 Cascader 的主用户事件模型：

| 事件 | 语义 |
| --- | --- |
| `SelectedOptionsChanged` | 内部多选集合变化。 |
| `ItemAsyncLoaded` | 单个节点异步加载完成。 |
| `ItemExpanded` / `ItemCollapsed` | 内部节点展开 / 收起。 |
| `ItemClicked` / `ItemDoubleClicked` | 内部节点点击 / 双击。 |
| `OptionSelected` | 内部单选选项提交。 |

### 3.6 稳定 template part

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddOnDecoratedBox` | `CascaderAddOnDecoratedBox` | 输入壳体、Addon、variant、status、CompactSpace、多选状态和 content padding 承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选过滤输入，过滤启用时显示。 |
| `SelectedOptionsBox` | `SelectTagAwareTextBox` | 多选结果 tag 展示和多选过滤输入承载。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 级联弹层宿主。 |
| `PopupFrame` | `Border` | 弹层外壳、最大高度、最小宽度和 popup padding。 |
| `PART_CascaderView` | `CascaderView` | 级联弹层内容、列、过滤和选项交互。 |
| `PART_ItemsPanel` | `StackPanel` | CascaderView 内部横向列容器。 |
| `PART_RootLevelList` | `CascaderViewLevelList` | CascaderView root 列。 |
| `PART_FilterList` | `CascaderViewFilterList` | CascaderView 过滤结果列表。 |

稳定伪类来自 `AbstractSelect` 和 `CascaderViewItem`：

- `:dropdownopen`、native validation `:error`、AtomUI warning `:warning`、`:pressed`。
- AddOnDecoratedBox variant 伪类：`:outlined`、`:filled`、`:borderless`。
- `CascaderViewItem` 使用 `:expanded`、`:checked`、`:selected` 和 checkbox toggle type 伪类。

## 事件与命令

| `DropDownOpening` / `DropDownOpened` / `DropDownClosing` / `DropDownClosed` | 继承自 `AbstractSelect` 的 popup 生命周期事件。 |
### 3.5 内部 view 事件契约
`CascaderView` 是内部弹层控件，当前事件用于 `Cascader` 和测试协作，不作为外层 Cascader 的主用户事件模型：
| 事件 | 语义 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### {gallery:CascaderShowCaseLangResource BasicCascaderViewTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml:626`

Gallery key：`ExamplesContent` / item `15`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:CascaderView OptionsSource="{Binding BasicCascaderViewNodes}" />

    <atom:CascaderView OptionsSource="{Binding BasicCheckableCascaderViewNodes}"
                       IsCheckable="True" />

    <atom:CascaderView />
</StackPanel>
```

### {gallery:CascaderShowCaseLangResource GenerateByTemplateTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml:644`

Gallery key：`ExamplesContent` / item `16`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:CascaderView
        Name="BasicCascaderView"
        OptionsSource="{Binding BasicCascaderViewNodes}">
        <atom:CascaderView.OptionTemplate>
            <TreeDataTemplate ItemsSource="{Binding Children}"
                              x:DataType="atom:ICascaderOption">
                <atom:TextBlock Text="{Binding Header}" />
            </TreeDataTemplate>
        </atom:CascaderView.OptionTemplate>
    </atom:CascaderView>

    <atom:CascaderView
        Name="BasicCheckableCascaderView"
        OptionsSource="{Binding BasicCheckableCascaderViewNodes}"
        IsCheckable="True">
        <atom:CascaderView.OptionTemplate>
            <TreeDataTemplate x:DataType="atom:ICascaderOption"
                              ItemsSource="{Binding Children}">
                <atom:TextBlock Text="{Binding Header}" />
            </TreeDataTemplate>
        </atom:CascaderView.OptionTemplate>
    </atom:CascaderView>
</StackPanel>
```

### {gallery:CascaderShowCaseLangResource LoadOptionsLazilyTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml:678`

Gallery key：`ExamplesContent` / item `17`

```axaml
<atom:CascaderView
    Name="AsyncLoadCascaderView"
    OptionsSource="{Binding AsyncLoadCascaderViewNodes}"
    DataLoader="{Binding AsyncCascaderNodeLoader}">
    <atom:CascaderView.OptionTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:ICascaderOption">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:CascaderView.OptionTemplate>
</atom:CascaderView>
```

## 状态模型

核心状态流：

```text
OptionsSource / Options
      ↓
CascaderView.Options
      ↓
CascaderViewLevelList
      ↓
CascaderViewItem
      ↓
SelectedOption / SelectedOptions
      ↓
SelectedOptionPath / EffectiveSelectedOptions
      ↓
input display + Form value
```

单选模式：

- `IsMultiple=false` 时使用 `SelectedOption` 作为真实值。
- 叶子节点点击后提交选择并关闭 popup。
- `IsAllowSelectParent=true` 时，非叶子节点也可提交为选择结果。
- `SelectedOptionPath` 根据当前选项的 parent 链生成 header 路径，用于输入框展示。
- `DefaultSelectOptionPath` 只在当前选择为空或强制刷新路径时应用。

多选模式：

- `IsMultiple=true` 时使用 `SelectedOptions` 作为真实值，并让内部 `CascaderView` 进入 checkable 模式。
- `SelectedOptions` 保留真实勾选集合，`ShowCheckedStrategy` 只计算 `EffectiveSelectedOptions`，用于 tag 展示。
- `SelectedOptions` 支持外部集合替换，也支持 `INotifyCollectionChanged` 集合的原地 `Add`、`Remove`、`Replace`、`Move` 和 `Reset`；这些变化会同步刷新 tag、计数、空状态、Form value 和内部 `CascaderView` 勾选状态。
- `MaxCount` 达到上限时，未选项通过 `IsMaxSelectReached` 进入受限状态；已选项仍可取消。
- 关闭单个 tag 时，目标节点及其子孙会从 `SelectedOptions` 中移除。

展开和异步加载：

- 展开按节点 parent 链从 root 到目标逐级执行。
- 同一级只保持一个已展开分支，展开新分支会折叠同级旧分支。
- 未加载且非 leaf 的节点在有 `DataLoader` 时进入 loading，加载完成后把返回子项加入目标 `Children`。
- 控件 detached 时取消待处理异步加载，避免离开视觉树后继续处理结果。

过滤：

- `FilterValue` 非空且控件 loaded 时，CascaderView 收集所有叶子路径并按 `Filter` 过滤。
- 过滤结果显示完整路径文本，选中过滤结果后回写目标 option。
- 过滤模式下，`Up` / `Down` 在可用结果间循环移动内部候选高亮，不修改 `SelectedOption`；`Enter` 提交当前候选，尚无候选时提交第一个可用结果，没有可用结果时保持选择和 popup 状态不变。路径中任一祖先 disabled 时，该过滤结果也不可作为候选或提交。
- 过滤列表拥有独立于树列的 active candidate owner；过滤结果重建、过滤清空、popup 关闭和容器回收时清除旧候选。树列与过滤列不会同时保留两个候选视觉。
- 清空过滤值或关闭 popup 后，过滤列表、过滤计数和缓存路径会被清理。

树形键盘导航：

- popup 打开且未过滤时，`Up` / `Down` 在当前已展开列的可见 enabled item 间循环移动内部候选；候选高亮与真实选择相互独立。
- 普通树列的 active candidate 由 `CascaderView` 单一持有；鼠标移动到 enabled item 时迁移该候选并继续执行 `ExpandTrigger=Hover` 的展开逻辑，但不提前提交选择、不滚动列表。`Enter` 使用同一 active candidate 作为选择或展开目标。
- `Right` 从当前候选或第一个可见 enabled item 开始，展开可展开节点并把候选移到下一列的第一个 enabled child。
- `Left` 优先把子级候选移回父级；候选已位于展开的根级非叶节点时折叠该节点。
- `Enter` 提交 enabled、非 loading 的叶子候选；`IsAllowSelectParent=true` 时也可提交父节点，否则沿用 `Right` 的展开并进入子级行为。

Form：

- 单选 Form value 为 `SelectedOption`。
- 多选 Form value 为 `SelectedOptions`。
- Form 校验错误写入同一份 Avalonia `DataValidationErrors`；`SelectedOption` 和 `SelectedOptions` 不维护独立错误状态。
- Form clear 会按当前 `IsMultiple` 清空对应选择状态。

## 主题与 Design Token

Cascader 的默认视觉由 Cascader 根主题、输入壳体、PopupHost、CascaderView、CascaderViewItem、SelectTagAwareTextBox、SelectHandle 和 CascaderToken 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `CascaderTheme.axaml` | 根模板、输入壳体、单选结果、多选 tag、handle、popup 和 CascaderView 绑定。 |
| `CascaderViewTheme.axaml` | 弹层内部级联列、空状态、过滤列表和默认 option template。 |
| `CascaderViewItemTheme.axaml` | option 行、checkbox、icon、header、展开 / loading icon、hover / expanded / disabled 状态。 |
| `CascaderViewLevelListTheme.axaml` | 单列列表宽度、高度、padding 和滚动行为。 |
| `CascaderToken` | Cascader 输入宽度、列宽、弹层高度、选项高度、padding、状态色和过滤高亮。 |
| `PopupHostToken` | popup margin、阴影和圆角。 |
| SharedToken | 字体、输入高度、图标尺寸、placeholder、disabled、motion 和全局 spacing。 |

单选路径和多选 tag 的完整内容提示复用共享 `OverflowTip` attached behavior。主题只把 `IsShowOverflowTip`、`OverflowTipDelay`、`OverflowTipPlacement` 和当前展示文本传给显示节点；tooltip 仅在视觉溢出时写入，且不覆盖用户手动声明的 `ToolTip.Tip`。

主题不可破坏的视觉边界：

- 输入壳体必须继续由 `CascaderAddOnDecoratedBox` 承载，保持 variant、status、CompactSpace 和 content padding 语义。
- `PART_SelectHandle` 的 hover / pressed / dropdown open / clear / loading 状态必须与输入壳体保持同步。
- `OptionTemplate` 的 DataContext 必须保持为 `ICascaderOption`，不能改为 header 文本。
- Popup 宽度和空状态宽度匹配语义必须保持：普通级联列使用列宽，空状态需要匹配输入宽度。
- 选项行的 checkbox、icon、header、expand / loading icon 间距由 Token 管理，不应在单个模板节点中写死。
- disabled、expanded、pointerover、checked、loading 和 active candidate 状态 selector 不能被绕过；`pointerover` 只触发 active candidate 迁移，不独立绘制第二个候选背景。

Token 来源：

`CascaderToken` 描述 Cascader 输入弹层和级联选项的尺寸、列宽、选项状态色、padding、过滤高亮和内部元素间距。它只表达控件主题常量，不表达实例选择状态、当前展开路径、当前过滤值、当前 loading 状态或最大选择数量状态。

## AOT 与裁剪注意事项

绑定型选项使用 source generator 生成 scoped resource host，不手写 `IResourceHost` / `IThemeVariantHost` 样板代码，不使用运行时反射扫描或字符串 binding。

模板内稳定关系优先放在 AXAML 中表达。右侧 count、content add-on 和 handle 状态使用 compiled ancestor binding；C# 属性同步只用于运行时生成容器和绑定型非 Visual 数据对象。

容器生命周期是绑定型选项的释放边界。`CascaderViewItem` 持有的 resource host attachment、option property subscription、container property subscription 和 children collection subscription 必须在容器 clear 或 detach 时释放。

异步加载由 `AsyncExpandLoadCoordinator` 合并同一节点并发请求并应用超时。`CascaderView` detached 时必须调用 `CancelAll()`，避免已离开视觉树的控件继续处理加载结果。

过滤路径缓存以当前 options 为边界，清除过滤或 options source 变化时必须失效，不能跨数据源复用旧路径。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs`：public API、输入壳体、popup、选择结果、默认路径、Form / CompactSpace 集成和外层交互。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderView.cs`：弹层内容、选项源、选择、过滤入口、空状态、默认展开路径和模板接入。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderView.ExpandAndCollapse.cs`：展开、收起、同级互斥展开、子列创建和默认展开路径。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Check.cs`：多选勾选、子树勾选、父级半选和 `SelectedOptions` 同步。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderView.AsyncItemDataLoad.cs`：异步加载、超时、取消、并发加载合并和 loaded 事件。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Filter.cs`：过滤路径收集、过滤结果列表、过滤计数和高亮文本。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderViewLevelList.cs`：单级列表、容器准备、虚拟化上下文保存和清理。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderViewItem.cs`：选项容器、checked / expanded / selected 事件、leaf 判断和绑定型选项同步。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderOption.cs`：`ICascaderOption`、默认轻量 `CascaderOption` 和层级 helper。
- `src/AtomUI.Desktop.Controls/Cascader/BindableCascaderOption.cs`：绑定型选项模型，使用 scoped resource host generator。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderAddOnDecoratedBox.cs`：Cascader 输入壳体扩展。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderViewPanel.cs`：弹层 frame 布局与边框测量。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderViewFilterList.cs`、`CascaderViewFilterListItemData.cs`：过滤结果列表和路径数据。
- `src/AtomUI.Desktop.Controls/Tooltip/OverflowTip.cs`：共享溢出 tooltip attached behavior，供单选路径和多选 tag 复用。
- `src/AtomUI.Desktop.Controls/Cascader/Themes/*.axaml`：Cascader、CascaderView、CascaderViewItem、level list 和 filter list 主题。
- `src/AtomUI.Desktop.Controls/Cascader/CascaderToken.cs`：Cascader 专属 Token。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/cascader/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/cascader/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/cascader/token.md`
- 变更记录：`docs/controls/desktop/data-entry/cascader/changelog.md`
- 语义结构：`./semantic-cn.md`
