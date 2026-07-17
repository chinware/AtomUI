# TreeSelect 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `TreeSelect` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectTheme.axaml`

```xml
<Panel>
    <TreeSelectAddOnDecoratedBox Name="{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}">
        <Panel>
            <TextBlock Name="PlaceholderText" />
            <SelectFilterTextBox Name="PART_SingleFilterInput" />
            <SelectTagAwareTextBox Name="SelectedItemsBox" />
        </Panel>
    </TreeSelectAddOnDecoratedBox>
    <Popup Name="PART_Popup" />
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
TreeSelect
  -> TreeSelectAddOnDecoratedBox (control theme, TreeSelectAddOnDecoratedBoxTheme.axaml)
  -> TreeSelect (control theme, TreeSelectTheme.axaml)
     -> Panel (template-stable)
        -> TreeSelectAddOnDecoratedBox#{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart} (internal-observable)
           -> Panel (template-stable)
              -> TextBlock#PlaceholderText (template-stable)
              -> SelectFilterTextBox#PART_SingleFilterInput (template-stable)
              -> SelectTagAwareTextBox#SelectedItemsBox (template-stable)
        -> Popup#PART_Popup (template-stable)
  -> TreeViewSelectTreeViewItem (item container control theme, TreeSelectTreeViewItemTheme.axaml)
     -> StackPanel (template-stable)
        -> TreeViewItemHeader#Header (template-stable)
        -> LayoutAwareMotionActor#PART_ItemsPresenterMotionActor (template-stable)
           -> ItemsPresenter#ItemsPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `TreeSelect` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TreeSelectAddOnDecoratedBox` | control theme | `TreeSelectAddOnDecoratedBoxTheme.axaml` | TreeSelect | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TreeSelect` | control theme | `TreeSelectTheme.axaml` | 用户代码 / 控件宿主 | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataValidationErrors`, `EffectiveSelectedItems` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `TreeSelectTheme.axaml` | TreeSelect | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataValidationErrors`, `EffectiveSelectedItems` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (TreeSelectAddOnDecoratedBox) | `TreeSelectTheme.axaml` | TreeSelect | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataValidationErrors`, `EffectiveSelectedItems` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PlaceholderText` | template node (TextBlock) | `TreeSelectTheme.axaml` | TreeSelect | `IsPlaceholderTextVisible`, `PlaceholderForeground`, `PlaceholderText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SingleFilterInput` | template node (SelectFilterTextBox) | `TreeSelectTheme.axaml` | TreeSelect | `FontFamily`, `FontSize`, `FontStyle`, `FontWeight`, `IsShowOverflowTip`, `OverflowTipDelay` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedItemsBox` | template node (SelectTagAwareTextBox) | `TreeSelectTheme.axaml` | TreeSelect | `EffectiveSelectedItems`, `Height`, `IsDropDownOpen`, `IsFilterEnabled`, `IsResponsiveTagMode`, `IsShowOverflowTip` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Popup` | template node (Popup) | `TreeSelectTheme.axaml` | TreeSelect | `IsDropDownOpen`, `PopupPlacement`, `ShouldUseOverlayPopup` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TreeViewSelectTreeViewItem` | item container control theme | `TreeSelectTreeViewItemTheme.axaml` | TreeSelect | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `StackPanel` | template node (StackPanel) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Header` | template node (TreeViewItemHeader) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenterMotionActor` | template node (LayoutAwareMotionActor) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddOnDecoratedBox` | `TreeSelectAddOnDecoratedBox` | 输入壳体、Addon、variant、status、CompactSpace 和多选状态承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选模式结果显示和搜索输入。 |
| `SelectedItemsBox` | `SelectTagAwareTextBox` | 多选和勾选模式已选 tag 展示。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PopupFrame` | `Border` | 懒创建的候选弹层外壳。 |
| `PART_TreeView` | `TreeSelectTreeView` | 懒创建的树候选控件。 |

## Pseudo Classes

- Template part、Token 名称、伪类和主题 selector 不得在未授权情况下重命名或删除。

## State Flow

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

## Theme and Token Boundaries

TreeSelect 的默认视觉由 TreeSelect 专属主题、Select 家族输入壳体、TreeView 和 PopupHost 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `TreeSelectTheme.axaml` | 根模板、输入壳体、单选输入、多选结果区域、handle、popup 和基础 selector。 |
| `SelectTagAwareTextBox` / `SelectTag` 主题 | 多选 tag 布局、高度、关闭按钮和禁用态。 |
| `TreeView` / `TreeViewItem` 主题 | 树节点缩进、展开、勾选、图标、连线和过滤视觉。 |
| `AddOnDecoratedBoxToken` | 输入壳体边框、圆角、状态、focus 和 CompactSpace 视觉。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `TreeSelectToken` | TreeSelect 候选弹层最小宽度。 |
| `SelectToken` | TreeSelect 复用的 popup padding、多选 tag 和输入内容 padding。 |

单选结果文本和多选 tag 的完整内容提示复用共享 `OverflowTip` attached behavior。主题通过 `IsShowOverflowTip`、`OverflowTipDelay` 和 `OverflowTipPlacement` 控制提示开关、延迟和位置，实际 tooltip 仅在文本视觉溢出时托管到 `ToolTip`。

右侧 count、content add-on 和 handle 的稳定 template part 状态由 AXAML compiled ancestor binding 表达。C# 中只保留 AddOnDecoratedBox hover / pressed 到 SelectHandle 的 sibling 状态转发，因为该关系不是 templated parent 绑定，不能用 `TemplateBinding` 表达。

Token 边界：

TreeSelectToken 是 TreeSelect 的组件级 Token scope，目前只承载 TreeSelect 候选弹层的最小宽度下限。输入壳体的通用边框、圆角、状态色、focus ring、disabled 背景和 AddOn 结构来自 SharedToken、AddOnDecoratedBoxToken 和 PopupHostToken；多选 tag、popup padding 和输入内容 padding 复用 SelectToken。

TreeSelectToken 不承载以下状态：

- `ItemsSource`、`Items`、`SelectedItem`、`SelectedItems` 等数据状态。
- `FilterValue`、`IsDropDownOpen`、`IsTreeCheckable`、`IsMultiple`、`Status` 等运行状态。
- 树节点 hover、pressed、selected、checked、expanded、disabled 的当前实例状态。
- Popup 当前宽度、高度、offset、placement 或异步加载结果。

## Customization Boundaries

维护 TreeSelect 时必须保持以下不变量：

- 单选模式使用 `SelectedItem`，多选和勾选模式使用 `SelectedItems`。
- `SelectedItem` 和 `SelectedItems` 必须保持默认双向绑定，并通过 Avalonia `DataValidationErrors` 承接 binding / Form error。
- `SelectedItems` 原地变更必须与集合替换走同一套展示、Form 和候选树同步路径，不能依赖用户重新赋值。
- `IsTreeCheckable=true` 必须继续把 TreeSelect 归入多选结果模型。
- `ShowCheckedStrategy` 只能影响 `EffectiveSelectedItems`，不能改写真实 `SelectedItems`。
- `ItemsSource` 变化必须尽量按节点路径 identity 保留已有选择。
- `Clear()` 必须同时清空单选和多选状态。
- Popup 打开、关闭和 light-dismiss 语义必须继续由 `AbstractSelect` 管理。
- 重新套用模板或 detach 时必须释放旧 popup 内容、TreeView 事件订阅和模板 part relay binding。
- 右侧稳定 template part 优先使用 AXAML binding，不能把可静态表达的绑定重新搬回 C#。
- `SizeType=Custom` 必须继续以 `Middle` 作为未显式覆盖时的默认视觉基线。
- Template part、Token 名称、伪类和主题 selector 不得在未授权情况下重命名或删除。

维护不变量：

内部重构必须保持以下不变量：

- public/protected API、Avalonia property、template part、Token 和 selector 契约不变。
- 构造函数之后必须先放 TreeSelect 自身 public/protected API，再放 Form 接口区，private 实现方法必须位于接口区之后。
- 可用 AXAML 表达的模板绑定不能回退为 `BindUtils.RelayBind`。
- C# relay binding 必须有与获取路径匹配的释放路径。
- `SelectedItem` / `SelectedItems` 与 TreeView selection / checked items 的同步不能形成递归事件。
- `SelectedItems` 集合引用替换和原地变更必须刷新同一组 value-state，避免 tag、`SelectedCount`、Form 值和 popup TreeView 状态不同步。
- `ShowCheckedStrategy` 只能派生展示集合，不能改写真实选择集合。
- `ItemsSource` 替换时的选择保留必须继续使用节点路径 identity。
- popup 内容清理必须断开事件、ItemsSource、TemplatedParent 和 popup child 引用。
