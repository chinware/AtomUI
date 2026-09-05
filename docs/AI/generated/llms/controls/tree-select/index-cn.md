# TreeSelect

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

TreeSelect 是 AtomUI 桌面数据录入体系中的树形选择控件，用于在层级数据中完成单选、多选和勾选式选择。它组合 Select 家族的输入壳体、过滤输入、弹层和 Form / CompactSpace 集成能力，并使用 TreeView 家族承载树节点、展开、勾选、图标、连线和异步加载。

TreeSelect 的职责是从 `ITreeItemNode` 层级数据中选择一个或多个节点。它不负责远程数据协议、业务权限过滤、级联业务规则、树节点编辑、拖放排序或复杂数据持久化；这些能力应由业务层、TreeView 或专用数据控件承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect` |
| 状态 | Stable |

## 何时使用

TreeSelect 的设计语言来自输入框、树形候选弹层和已选结果的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 树形数据 | 候选项按父子层级展示。 | `ItemsSource`、`Items`、`ItemTemplate`。 |
| 选择模式 | 控件支持单选、多选和 checkbox 勾选。 | `IsMultiple`、`IsTreeCheckable`、`IsTreeCheckStrictly`。 |
| 结果展示 | 单选显示节点文本，多选以 tag 展示有效选中节点。 | `SelectedItem`、`SelectedItems`、`ShowCheckedStrategy`。 |
| 搜索过滤 | 输入过滤值后高亮、加粗、展开路径或隐藏不匹配节点。 | `IsFilterEnabled`、`FilterValue`、`Filter`、`FilterStrategy`。 |
| 树视觉 | 控制图标、叶子图标、连线、展开指示和旋转。 | `IsShowIcon`、`IsShowLeafIcon`、`IsShowLine`、`IsShowTreeLine`。 |
| 输入表面 | 继承 Select 家族的边框、尺寸、状态和 AddOn。 | `SizeType`、`StyleVariant`、`Status`、Addon。 |
| 弹层语义 | 候选树在 popup 中打开，可匹配输入宽度。 | `IsDropDownOpen`、`PopupPlacement`、`IsPopupMatchSelectWidth`。 |

`SizeType=Custom` 不是 TreeSelect 的第四套专属 Token。主题层把 `Custom` 归入 `Middle` 的默认字体和 padding 分支；用户显式设置高度等尺寸属性时由 Avalonia 属性优先级决定最终布局。

## 公共 API

TreeSelect 继承 `AbstractSelect` 的输入壳体、弹层、清除、状态、尺寸、Addon、Form 和 CompactSpace 契约；自身提供树数据、树选择、过滤和勾选策略契约。

树数据 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `ItemsSource` | `IEnumerable<ITreeItemNode>?` | 外部树节点集合。变化时同步到内部 `Items`。 |
| `Items` | `ItemCollection` | XAML 内容子项入口，也是候选树实际数据入口。 |
| `ItemTemplate` | `IDataTemplate?` | 树节点显示模板，默认显示 `ITreeItemNode.Header`。 |
| `DataLoader` | `ITreeItemNodeLoader?` | 展开节点时加载子节点的数据加载边界。 |
| `TreeDefaultExpandedPaths` | `IList<TreeNodePath>?` | 默认展开路径集合。 |
| `IsDefaultExpandAll` | `bool` | 默认展开全部节点。 |

选择 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SelectedItem` | `ITreeItemNode?` | 单选模式当前节点；默认 `BindingMode.TwoWay`，启用 Avalonia `DataValidationErrors`。 |
| `SelectedItems` | `IList<ITreeItemNode>?` | 多选或勾选模式当前节点集合；默认 `BindingMode.TwoWay`，启用 Avalonia `DataValidationErrors`，支持 `INotifyCollectionChanged` 原地变更刷新。 |
| `IsMultiple` | `bool` | 是否使用多选结果模型。 |
| `IsTreeCheckable` | `bool` | 是否使用 checkbox 勾选。启用后 TreeSelect 使用多选结果模型。 |
| `IsTreeCheckStrictly` | `bool` | 勾选时父子节点是否严格独立。 |
| `ShowCheckedStrategy` | `TreeSelectCheckedStrategy` | 多选 tag 的展示策略，决定展示全部、父节点或叶子节点。 |
| `MaxCount` | `int` | 继承自 `AbstractSelect` 的最大可选数量。 |
| `Clear()` | method | 清空 `SelectedItem` 和 `SelectedItems`。 |

过滤与树视觉 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Filter` | `IValueFilter?` | 树节点过滤器；为空时初始化为 contains 过滤。 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | 从节点中提取过滤文本。 |
| `FilterStrategy` | `TreeFilterStrategy` | 过滤后的高亮、加粗、展开路径和隐藏不匹配策略。 |
| `FilterHighlightForeground` | `IBrush?` | 过滤命中高亮前景色。 |
| `AutoScrollToSelectedItem` | `bool` | 候选树打开或同步时滚动到选中节点。 |
| `IsShowOverflowTip` | `bool` | 单选结果文本或多选 tag 视觉溢出时是否显示完整内容 tooltip，默认 `true`。 |
| `OverflowTipDelay` | `int` | 溢出 tooltip 打开前的延迟时间，单位毫秒，默认 `1200`。 |
| `OverflowTipPlacement` | `PlacementMode` | 溢出 tooltip 相对单选结果文本或多选 tag 的位置，默认 `TopEdgeAlignedLeft`。 |
| `IsShowIcon` / `IsShowLeafIcon` | `bool` | 是否显示节点图标和叶子节点图标。 |
| `IsShowLine` / `IsShowTreeLine` | `bool` | 是否显示层级连线。 |
| `IsSwitcherRotation` | `bool` | 展开图标是否旋转。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_InputControlFrame` | `TreeSelectAddOnDecoratedBox` | `InputControlFrame` / AddOnDecoratedBox 组合、Addon、EffectiveStatus、CompactSpace 和多选布局承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选模式结果显示和搜索输入。 |
| `SelectedItemsBox` | `SelectTagAwareTextBox` | 多选和勾选模式已选 tag 展示。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PopupFrame` | `Border` | 懒创建的候选弹层外壳。 |
| `PART_TreeView` | `TreeSelectTreeView` | 懒创建的树候选控件。 |

## 事件与命令

TreeSelect 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml:102`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:TreeSelect Name="BasicTreeSelect"
```

### 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml:120`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="16">
    <StackPanel Spacing="8">
        <TextBlock Text="单选绑定"
                   FontWeight="SemiBold" />
        <atom:TreeSelect Name="BindingSingleTreeSelect"
                         HorizontalAlignment="Stretch"
                         IsDefaultExpandAll="True"
                         IsAllowClear="True"
                         IsFilterEnabled="True"
                         ItemsSource="{Binding BindingSingleTreeNodes}"
                         SelectedItem="{Binding BoundSelectedItem}"
                         PlaceholderText="请选择" />
        <WrapPanel ItemSpacing="8">
            <atom:Button SizeType="Small"
                         Command="{Binding SetBoundSelectedItemCommand}"
                         Content="选择你的叶子" />
            <atom:Button SizeType="Small"
                         Command="{Binding ClearBoundSelectedItemCommand}"
                         Content="清空" />
        </WrapPanel>
        <TextBlock Text="ViewModel 值：" />
        <TextBlock Text="{Binding BoundSelectedItemText}" />
    </StackPanel>

    <StackPanel Spacing="8">
        <TextBlock Text="多选绑定"
                   FontWeight="SemiBold" />
        <atom:TreeSelect Name="BindingMultipleTreeSelect"
                         HorizontalAlignment="Stretch"
                         IsDefaultExpandAll="True"
                         IsAllowClear="True"
                         IsFilterEnabled="True"
                         IsMultiple="True"
                         ItemsSource="{Binding BindingMultipleTreeNodes}"
                         SelectedItems="{Binding BoundSelectedItems}"
                         PlaceholderText="请选择" />
        <WrapPanel ItemSpacing="8">
            <atom:Button SizeType="Small"
                         Command="{Binding SetBoundSelectedItemsCommand}"
                         Content="选择两个节点" />
            <atom:Button SizeType="Small"
                         Command="{Binding ClearBoundSelectedItemsCommand}"
                         Content="清空" />
        </WrapPanel>
        <TextBlock Text="ViewModel 值：" />
        <TextBlock Text="{Binding BoundSelectedItemsText}"
                   TextWrapping="Wrap" />
    </StackPanel>
</StackPanel>
```

### 多选

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml:178`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:TreeSelect Name="MultiSelectionTreeSelect"
```

### 由树数据生成

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml:195`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:TreeSelect Name="ItemsSourceTreeSelect"
```

## 状态模型

TreeSelect 的核心状态流：

```text
ItemsSource / Items
      ↓
TreeSelectTreeView
      ↓
SelectedItem / SelectedItems
      ↓
SingleFilterInput / SelectedItemsBox
      ↓
Form value + MaxCount state
```

选择模式语义：

- 单选模式使用 `SelectedItem` 作为表单值，候选节点点击后关闭弹层。
- 多选模式使用 `SelectedItems` 作为表单值，候选树使用多选 selection，并通过 tag 展示结果。
- `IsTreeCheckable=true` 使用 checkbox 作为节点切换入口，TreeView selection 不再作为主要选择入口。
- `ShowCheckedStrategy` 只影响多选 tag 展示集合，不改变 `SelectedItems` 的真实值。
- `SelectedItems` 是用户拥有的受控集合。集合引用替换和 `ObservableCollection` 等 `INotifyCollectionChanged` 原地 `Add`、`Remove`、`Reset` 都必须刷新 tag、`SelectedCount`、Form value changed、候选树 selection / checked items 和最大选择数状态。

过滤行为：

- `FilterValue` 来自单选搜索输入。
- `FilterStrategy` 控制命中高亮、加粗、展开路径和隐藏不匹配节点。
- 弹层关闭后，单选模式清空过滤值并恢复结果显示。

清除行为通过 `SelectHandle.ClearRequestedEvent` 冒泡到 TreeSelect，并调用 `Clear()` 清空当前选择。

## 主题与 Design Token

TreeSelect 的默认视觉由 TreeSelect 专属主题、`InputControlFrame` / Select 家族 AddOnDecoratedBox、TreeView 和 PopupHost 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `TreeSelectTheme.axaml` | 根模板、输入壳体、单选输入、多选结果区域、handle、popup 和基础 selector。 |
| `SelectTagAwareTextBox` / `SelectTag` 主题 | 多选 tag 布局、高度、关闭按钮和禁用态。 |
| `TreeView` / `TreeViewItem` 主题 | 树节点缩进、展开、勾选、图标、连线和过滤视觉。 |
| `InputControlFrameTheme` / `SharedToken` | 输入壳体边框、圆角、effective status、focus 和 CompactSpace 视觉。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `TreeSelectToken` | TreeSelect 候选弹层最小宽度。 |
| `SelectToken` | TreeSelect 复用的 popup padding、多选 tag 和输入内容 padding。 |

单选结果文本和多选 tag 的完整内容提示复用共享 `OverflowTip` attached behavior。主题通过 `IsShowOverflowTip`、`OverflowTipDelay` 和 `OverflowTipPlacement` 控制提示开关、延迟和位置，实际 tooltip 仅在文本视觉溢出时托管到 `ToolTip`。

右侧 count、content add-on 和 handle 的稳定 template part 状态由 AXAML compiled ancestor binding 表达。C# 中只保留 frame layout part 到 SelectHandle 的 hover / pressed sibling 状态转发，因为该关系不是 templated parent 绑定，不能用 `TemplateBinding` 表达；该转发不改变 `InputControlFrame` 对输入表面状态的唯一 ownership。

Token 来源：

TreeSelectToken 是 TreeSelect 的控件级 Token scope，目前只承载 TreeSelect 候选弹层的最小宽度下限。输入表面的通用边框、圆角、effective status、focus ring、disabled 背景和 AddOn 结构来自 `InputControlFrameTheme`、SharedToken 和 PopupHostToken；多选 tag、popup padding 和输入内容 padding 复用 SelectToken。

TreeSelectToken 不承载以下状态：

- `ItemsSource`、`Items`、`SelectedItem`、`SelectedItems` 等数据状态。
- `FilterValue`、`IsDropDownOpen`、`IsTreeCheckable`、`IsMultiple`、`Status` 等运行状态。
- 树节点 hover、pressed、selected、checked、expanded、disabled 的当前实例状态。
- Popup 当前宽度、高度、offset、placement 或异步加载结果。

## AOT 与裁剪注意事项

TreeSelect 不依赖运行时反射发现模板结构。模板协作通过固定 template part、显式类型、AXAML binding、C# indexer binding 和事件完成。

资源和生命周期边界：

- `_selectHandleInputStateBindings` 每次模板接入前释放旧绑定，仅持有 frame layout part → SelectHandle 的 hover / pressed sibling 状态转发，不承载输入表面状态归一。
- `_treeView` 的事件订阅和 `ItemsSource` 必须在 `ClearPopupContent()` 中释放。
- 懒创建的 `PopupFrame` 和 `TreeSelectTreeView` 必须设置 `TemplatedParent`，并在清理时置空。
- `Items.CollectionChanged` 是控件实例持有自身集合的订阅，生命周期与控件实例一致。
- `_selectedItemsCollectionChangedSource` 只订阅当前 `SelectedItems` 中实现 `INotifyCollectionChanged` 的集合；`SelectedItems` 替换、控件 detach 时必须释放旧订阅。
- TreeView 异步加载由 TreeView 家族处理，TreeSelect 不直接持有异步任务状态。

AOT 边界：

- `TreeSelectToken` 通过 token generator 显式注册，生成 `TreeSelectTokenKind` 和 `TreeSelectTokenResourceExtension`。
- `ItemTemplate` 是 XAML 模板入口，不依赖运行时成员扫描。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.cs`：public TreeSelect API、生命周期、弹层树懒创建、选择同步、过滤输入、Form 映射和私有算法。
- `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.SemanticParts.cs`：TreeSelect 的 13 个 Semantic Part 声明（见 [TreeSelect Semantic Part 契约](semantic-part.md)）。
- `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelectAddOnDecoratedBox.cs`：TreeSelect 输入布局扩展，复用 `InputControlFrame` 并承载多选和选择空状态。
- `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelectTreeView.cs`：TreeSelect 候选树，使用 TreeView 样式键并创建专用容器。
- `src/AtomUI.Desktop.Controls/TreeSelect/TreeViewSelectTreeViewItem.cs`：候选树节点容器，承接最大选择数状态。
- `src/AtomUI.Desktop.Controls/TreeSelect/Converters/*`：树节点显示转换辅助。
- `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelectToken.cs`：TreeSelect 控件 Token。
- `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectTheme.axaml`：根模板、输入壳体、右侧内容、结果区域、popup 和 selector。
- `src/AtomUI.Desktop.Controls/Tooltip/OverflowTip.cs`：共享溢出 tooltip attached behavior，供单选结果和多选 tag 复用。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/tree-select/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/tree-select/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-entry/tree-select/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-entry/tree-select/token.md`
- 变更记录：`docs/controls/desktop/data-entry/tree-select/changelog.md`
- 语义结构：`./semantic-cn.md`
