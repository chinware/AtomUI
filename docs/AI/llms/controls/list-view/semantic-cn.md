# ListView 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `ListView` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/ListView/Themes/ListViewTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <DockPanel>
        <ContentPresenter Name="TopPaginationPresenter" />
        <ContentPresenter Name="BottomPaginationPresenter" />
        <Spin>
            <Panel>
                <ScrollViewer Name="{x:Static atom:ListViewThemeConstants.ScrollViewerPart}">
                    <ItemsPresenter Name="ItemsPresenter" />
                </ScrollViewer>
                <ContentPresenter Name="EmptyIndicator" />
                <Empty Name="DefaultEmptyIndicator" />
            </Panel>
        </Spin>
    </DockPanel>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
ListView
  -> ListViewItem (item container control theme, ListViewItemTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel (template-stable)
           -> IconTemplatePresenter#SelectedIndicator (internal-observable)
           -> ContentPresenter#ContentPresenter (internal-observable)
  -> ListView (control theme, ListViewTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> DockPanel (template-stable)
           -> ContentPresenter#TopPaginationPresenter (internal-observable)
           -> ContentPresenter#BottomPaginationPresenter (internal-observable)
           -> Spin (template-stable)
              -> Panel (template-stable)
                 -> ScrollViewer#{x:Static atom:ListViewThemeConstants.ScrollViewerPart} (template-stable)
                    -> ItemsPresenter#ItemsPresenter (internal-observable)
                 -> ContentPresenter#EmptyIndicator (internal-observable)
                 -> Empty#DefaultEmptyIndicator (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ListView` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ListViewItem` | item container control theme | `ListViewItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `HorizontalContentAlignment`, `IsSelectedIndicatorVisible` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `ListViewItemTheme.axaml` | ListViewItem | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `HorizontalContentAlignment`, `IsSelectedIndicatorVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `ListViewItemTheme.axaml` | ListViewItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `IsSelectedIndicatorVisible`, `SelectedIndicator`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedIndicator` | template node (IconTemplatePresenter) | `ListViewItemTheme.axaml` | ListViewItem | `IsSelectedIndicatorVisible`, `SelectedIndicator` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `ListViewItemTheme.axaml` | ListViewItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ListView` | control theme | `ListViewTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BottomPagination`, `CornerRadius`, `CustomOperatingIndicator`, `CustomOperatingIndicatorTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `ListViewTheme.axaml` | ListView | `Background`, `BorderBrush`, `BottomPagination`, `CornerRadius`, `CustomOperatingIndicator`, `CustomOperatingIndicatorTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `ListViewTheme.axaml` | ListView | `BottomPagination`, `CustomOperatingIndicator`, `CustomOperatingIndicatorTemplate`, `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TopPaginationPresenter` | template node (ContentPresenter) | `ListViewTheme.axaml` | ListView | `TopPagination` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `BottomPaginationPresenter` | template node (ContentPresenter) | `ListViewTheme.axaml` | ListView | `BottomPagination` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `ListViewTheme.axaml` | ListView | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsDefaultEmptyIndicatorVisible`, `IsEffectiveEmptyVisible`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:ListViewThemeConstants.ScrollViewerPart}` | template node (ScrollViewer) | `ListViewTheme.axaml` | ListView | `IsEffectiveEmptyVisible`, `ItemsPanel`, `ScrollViewer`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `ListViewTheme.axaml` | ListView | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `EmptyIndicator` | template node (ContentPresenter) | `ListViewTheme.axaml` | ListView | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsEffectiveEmptyVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DefaultEmptyIndicator` | template node (Empty) | `ListViewTheme.axaml` | ListView | `EmptyIndicatorPadding`, `IsDefaultEmptyIndicatorVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| API | 语义 |
| --- | --- |
| `ItemsSource` / `Items` | 数据入口。`ItemsSource` 会归一为 `IListCollectionView`。 |
| `SortDescriptions` | 排序描述集合，交给 `IListCollectionView.SortDescriptions` 执行。 |
| `Filter` / `FilterValue` / `FilterValueSelector` | 过滤谓词、过滤条件和值选择器。 |
| `IsFiltering` | 当前是否存在有效过滤描述。 |
| `TotalItemCount` | 当前视图总数据量，用于空状态、伪类和分页同步。 |

## Pseudo Classes

ListView 的公共契约由数据视图 API、选择 API、分页 API、视觉 API、事件 API、template part、伪类和主题入口组成。

数据视图 API：

| API | 语义 |
| --- | --- |
| `ItemsSource` / `Items` | 数据入口。`ItemsSource` 会归一为 `IListCollectionView`。 |
| `SortDescriptions` | 排序描述集合，交给 `IListCollectionView.SortDescriptions` 执行。 |
| `Filter` / `FilterValue` / `FilterValueSelector` | 过滤谓词、过滤条件和值选择器。 |
| `IsFiltering` | 当前是否存在有效过滤描述。 |
| `TotalItemCount` | 当前视图总数据量，用于空状态、伪类和分页同步。 |

## State Flow

ListView 的状态模型由数据视图状态、选择状态、分页状态、过滤状态、分组状态、空状态、操作状态、尺寸状态和动效状态组成。

数据视图状态：

- `ItemsSource` 设置后归一为具备 entry bridge 的 `IListCollectionView`，ListView 监听其 `CollectionChanged`、`PropertyChanged`、`PageChanging` 和 `PageChanged`。不具备 entry bridge 的自定义 view 从其 `SourceCollection` 重新归一。
- `SortDescriptions`、`Filter` / `FilterValue` / `FilterValueSelector`、`IsGroupEnabled` / `GroupPropertySelector` 分别写入 collection view 的排序、过滤和分组描述。
- `TotalItemCount` 和 `IsEmptyDataSource` 来自 collection view，用于伪类、空状态和分页器同步。

选择行为：

- `IsSelectable=false` 时，ListView 不响应 pointer 或 keyboard 选择更新，并清空 canonical selection。
- 数据源中的每一次出现拥有独立 internal EntryId；item 引用、`Equals`、`GetHashCode` 和显示值不参与源条目标识与选择映射。
- `ListViewSelectionModel` 持有 selected EntryIds、anchor 和 active entry；`SelectedIndex(es)`、`SelectedItem(s)`、`SelectedValue` 和容器 `IsSelected` 都是同一状态的投影。
- 公开选择索引始终表达原始数据源的 source index。排序、过滤、分组和分页通过 entry projection 映射容器，不通过 item `IndexOf` 反查源位置。
- 组标题是没有 EntryId 和 source index 的视图合成节点，所有选择入口均跳过组标题。
- Reset 或 ItemsSource 替换只通过唯一、非空 item key 恢复选择；没有稳定 key 的选择清除。
- `SelectionMode.AlwaysSelected` 在存在数据且丢失选择时恢复到首项。
- 完整选择、集合变化和恢复语义见 [ListView 选择模型设计](selection-model-design.md)。

键盘和文本搜索行为：

- 方向键按 Avalonia navigation direction 移动选择，`WrapSelection` 控制边界循环。
- 多选模式下，平台 select-all 手势调用 `Selection.SelectAll()`。
- Space / Enter 会尝试从事件源更新选择。
- `IsTextSearchEnabled=true` 时，文本输入按 `TextSearch.TextBinding` 或 `DisplayMemberBinding` 做前缀搜索，搜索词由短定时器清空。

过滤行为：

- `Filter != null && FilterValue != null` 时，ListView 进入过滤态并向 collection view 写入 `ListFilterDescription`。
- `FilterValueSelector` 优先用于提取过滤值；未提供时默认读取 `IListItemData.Content`。
- 过滤改变后触发 `FilterContextChanged`，并刷新空状态。

分组行为：

- `IsGroupEnabled=true` 时，ListView 用 `GroupPropertySelector` 构造 `ListGroupDescription` 并写入 collection view。
- 默认 `GroupPropertySelector` 读取 `IGroupHeader.Group`，因此推荐分组数据项实现 `IListItemData` / `IGroupHeader` 并提供稳定、非空的 `Group` 值。
- collection view 会为每个 group key 插入一个 `GroupListItemData` 作为组标题项，其 `Content` 来自 `groupKey.ToString()`，`IsGroupItem=true`。
- 组标题项使用 `GroupItemTemplate`，普通数据项仍使用 `ItemTemplate`。
- 组标题项参与当前视图枚举和容器生成，但它表达的是视觉分隔，不是业务数据项；pointer selection 路径会跳过 `IsGroupItem=true` 的容器。
- 分组开启后，selection model 仍以业务 source entries 为选择范围；当前 view projection 只增加没有 EntryId 的组标题节点。
- 与分页同时使用时，collection view 先按完整结果建立临时分组顺序，再按当前页重建对外可枚举的 group 结构。

分页行为：

- `PageSize=0` 表示不分页；`PageSize>0` 时 collection view 只枚举当前页。
- `TopPagination` 和 `BottomPagination` 接收当前 `Total`、`PageSize`、`CurrentPage`、对齐、启用状态、动效和单页隐藏策略。
- 分页器的 `CurrentPageChanged` 请求转换为 `IListCollectionView.MoveToPage(page - 1)`。
- `PaginationVisibility` 只控制分页器可见性，不改变 collection view 的分页数据。

空状态和操作态：

- `IsShowEmptyIndicator && TotalItemCount == 0` 时显示空状态。
- 空状态显示时隐藏滚动列表区域。
- `IsOperating=true` 时，root 模板内的 `Spin` 覆盖列表和空状态，用于表达外部操作中状态。

动效行为：

- ListViewItem 初始化阶段禁用 transitions，loaded 后启用，避免容器首次准备时出现非预期动画。
- 虚拟化容器保存、恢复和清理期间会临时关闭 motion，避免回收状态产生过渡。

## Theme and Token Boundaries

ListView 主题按 root、分页、操作态、滚动内容、空状态和 item 两层组织。

```text
ListViewTheme
  Frame
  TopPaginationPresenter
  BottomPaginationPresenter
  Spin
    PART_ScrollViewer
      ItemsPresenter
    EmptyIndicator

ListViewItemTheme
  Frame
    SelectedIndicator
    ContentPresenter
```

视觉规则：

- root 边框由 `BorderBrush`、`BorderThickness`、`CornerRadius` 和 `IsBorderless` 共同决定。
- `SizeType` 控制 root 圆角、空状态 padding、条目最小高度、条目 padding 和条目圆角。
- 默认条目背景透明，hover 使用 `ItemHoverBg`，selected 使用 `ItemSelectedBg`。
- 组标题使用 `GroupHeaderColor`，不应用普通条目的 hover / selected 状态背景。
- selected indicator 默认使用 默认 `CheckOutlined`，颜色和尺寸来自 SharedToken。
- 空状态默认使用 `Empty` 的 simple preset image。
- 分页器 margin 使用 ListViewToken 的 `PaginationMargin`。

ListViewToken 提供内容 padding、条目文字颜色、条目状态背景、条目 padding、条目 margin、分页器 margin、组标题色和选中指示器 margin。Token 详情见 [ListView Token 设计](token.md)。

Token 边界：

ListViewToken 是 ListView 的组件级设计变量层。它把全局颜色、尺寸、间距和状态色转换为 ListView root、ListViewItem、组标题、分页器间距和 selected indicator 可消费的语义值。

ListViewToken 服务以下主题和控件：

- `ListViewTheme.axaml`
- `ListViewItemTheme.axaml`
- `ListView`
- `ListViewItem`

ListViewToken 不承载 `ItemsSource`、`SelectedIndex`、`SelectedItem`、`SelectedItems`、`IsSelected`、`IsFiltering`、`FilterValue`、`PageIndex`、`PageSize`、`IsOperating`、`IsEffectiveEmptyVisible`、`VirtualIndex` 等实例状态。这些状态由 C# 状态模型、collection view、容器属性和 theme selector 处理。

## Customization Boundaries

维护 ListView 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 ListView / ListViewItem public API、事件、Avalonia 属性语义和默认值。
- `ItemsSource` 到 `IListCollectionView` 的归一化必须保持排序、过滤、分组、分页、entry projection 和选择映射可用。
- 只有具备 entry bridge 的用户 view 可以直接接入，且不由 ListView 当作自建 view 释放；其他 view 使用其 `SourceCollection` 创建由 ListView 持有的 AtomUI view。
- `SortDescriptions`、`FilterDescriptions` 和 `GroupDescriptions` 的重建必须保持 collection view 状态一致，不留下重复描述。
- `IsSelectable=false` 必须阻止用户选择更新，并清空当前选择。
- 组标题项必须保持不可通过普通 pointer selection 路径选中。
- 源条目标识必须来自 source entry，不能以 item equality、对象引用、source index 或 view index 替代 EntryId。
- 公开选择索引必须始终使用 source index；view index 和页内索引只能作为当前投影位置。
- 过滤、排序、分组和分页不得替换已选 entry；隐藏 entry 再次可见时必须恢复选中投影。
- Reset 和 ItemsSource 替换不得按 item equality 或相同索引猜测旧选择。
- `PaginationVisibility` 只表达分页器可见性，不改变分页数据和分页器 motion 状态语义。
- `PART_ScrollViewer`、`ItemsPresenter`、`EmptyIndicator`、分页 presenter、`SelectedIndicator` 和 `ContentPresenter` 的职责不得被无兼容说明地改变。
- 选中指示器、空状态和操作态节点应留在 AXAML 静态模板中，通过状态属性控制，不作为普通性能优化迁移到 C# 动态创建。
- 虚拟化容器回收时，容器本地值必须对称保存、恢复和清理，避免 disabled、group item、selected indicator 或 content 状态串扰。
- Token 只表达组件设计语义，不承载选择、过滤、分页、空状态或虚拟化运行时状态。

维护不变量：

内部重构必须保持以下不变量：

- `ListView.cs` 保留 public API、事件、ItemsSource 归一、容器生命周期和 collection view 配置入口。
- `ListView.Selecting.cs` 保持选择模型接入、entry/index 映射、SelectedValue 和键盘 / 文本搜索职责，不把选择状态写入数据项或容器作为 owner。
- `ListViewSelectionModel` 是 selected EntryIds、anchor 和 active entry 的唯一 owner；公开属性不得维护平行选择状态。
- 标识映射路径不得使用 item `Equals`、对象引用、`IList.IndexOf` 或 `IListCollectionView.IndexOf` 解析 source entry。
- `SelectedItem`、`SelectedItems` 和 `SelectedValue` 只能从 canonical selection 投影，不作为反向选择输入。
- `ListView.Pagination.cs` 只处理分页器接入和 collection view page 状态同步，不执行数据请求。
- `ListViewItem.cs` 保持条目容器角色，不承载排序、过滤、分页或跨列表全局状态。
- collection view 替换时必须解绑旧 view 事件；只有具备 entry bridge 的 view 可以直接接入，且只有 ListView 自建 view 才能由 ListView dispose。
- `ConfigureFilterDescription`、`ConfigureSortDescriptions` 和 `ConfigureGroupInfo` 重建描述前必须清理旧描述。
- `IsGroupEnabled` 切换后只刷新 view projection，并保持组标题项没有 EntryId 且不进入业务选择结果。
- `GroupPropertySelector` 必须返回稳定 group key；需要兜底分组时由 selector 返回明确 key，不依赖 collection view 自动处理 `null`。
- `GroupListItemData` 是展示层合成项，不得写回用户源集合或被当作业务数据模型扩展点。
- 分组与分页组合时必须保持 `PrepareTemporaryGroups` 和 `PrepareGroupsForCurrentPage` 的顺序，不得只对当前页局部数据直接推断全局分组顺序。
- `PrepareContainerForItemOverride` 新增状态同步时，必须在 `ClearContainerForItemOverride` 或 virtualizing clear 路径中对称清理。
- EntryId 映射必须同时覆盖 selection change、container prepared、container index changed、auto scroll 和 selected indicator 同步。
- Add、Remove、Move 和 Replace 必须按 collection change index 更新 entries，不按 item equality 定位变化目标。
- Reset 和 ItemsSource 替换只通过唯一非空 item key 恢复选择，不按 item equality 或旧索引回退。
- 分页器替换时必须解除旧 `CurrentPageChanged` 和 relay binding。
- Token 变更必须同步 `ListViewTokenKind`、AXAML 引用和 token.md 语义说明。
