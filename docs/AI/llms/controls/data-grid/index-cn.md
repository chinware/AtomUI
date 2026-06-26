# DataGrid

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

DataGrid 是 AtomUI 桌面控件体系中的数据表格控件，用于列模型、行选择、排序、过滤、编辑、冻结列和分页展示。

DataGrid 不负责简单列表、树视图或业务数据访问层。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls.DataGrid`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls.DataGrid` |
| .NET 命名空间 | `AtomUI.Desktop.Controls.DataGrid` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid` |
| 状态 | Stable |

## 何时使用

DataGrid 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | DataGrid 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | DataGrid 是 AtomUI 桌面控件体系中的数据表格控件，用于列模型、行选择、排序、过滤、编辑、冻结列和分页展示。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate` 等 32 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | DataGrid Token + ControlTheme。 |

## 公共 API

DataGrid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate`、`ColumnHeaderHeight`、`ContentHeight` 等 32 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ClipboardCopyMode`、`CurrentSortDirection`、`FilterMode`、`Index`、`IsFilterActivated`、`IsHideOnSinglePage`、`IsHoverMode`、`IsMultipleFilterEnabled`、`IsSelected`、`IsSorterTooltipVisible` 等 20 项 | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `AscendingIndicatorVisible`、`DescendingIndicatorVisible`、`IsDeleteEnabled`、`IsDetailsVisible`、`IsEditEnabled`、`IsFrameBorderVisible`、`IsFrozen`、`IsLeaf`、`IsMotionEnabled`、`IsOperating` 等 19 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `BottomPaginationAlign`、`ColumnWidth`、`HorizontalAlignment`、`HorizontalScrollBarVisibility`、`MaxColumnWidth`、`MinColumnWidth`、`RowHeight`、`SeparatorBrush`、`SizeType`、`SublevelIndent` 等 14 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `CellTheme`、`CollectionView`、`CustomOperatingIndicator`、`EmptyIndicator`、`Footer`、`FormatString`、`GridLinesVisibility`、`Level`、`Maximum`、`Minimum` 等 15 项 | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `SelectionChanged`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AtomUIDataGridThemesProvider`、`CollectionViewGroupComparer`、`CollectionViewGroupRoot`、`ControlTokenTypePool`、`DataGrid`、`DataGridAbstractTextColumn`、`DataGridAutoGeneratingColumnEventArgs`、`DataGridBeginningEditEventArgs`、`DataGridBoundColumn`、`DataGridCell`、`DataGridCellCollection`、`DataGridCellCoordinates`、`DataGridCellEditEndedEventArgs`、`DataGridCellEditEndingEventArgs` 等 95 项。
- 枚举：`DataGridClipboardCopyMode`、`DataGridEditAction`、`DataGridEditingUnit`、`DataGridFilterMode`、`DataGridGridLinesVisibility`、`DataGridHeadersVisibility`、`DataGridLangResourceKind`、`DataGridLengthUnitType`、`DataGridPaginationVisibility`、`DataGridRowDetailsVisibilityMode` 等 15 项。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Ascending` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_BottomGridLine` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_Descending` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_FocusVisual` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Frame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_HeaderPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_HorizontalIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_IndicatorIconButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_RightGridLine` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_SortIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_VerticalIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_VerticalSeparator` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `DataGridPseudoClass.EmptyColumns`、`DataGridPseudoClass.EmptyRows`、`DataGridPseudoClass.FirstColumnHeader`、`DataGridPseudoClass.GroupItemLeaf`、`DataGridPseudoClass.LastColumnHeader`、`DataGridPseudoClass.MiddleColumnHeader`、`DataGridPseudoClass.RowHover`、`EmptyColumns=:empty-columns`、`EmptyRows=:empty-rows`、`FirstColumnHeader=:first-column-header`、`GroupItemLeaf=:is-leaf-item`、`LastColumnHeader=:last-column-header` 等 14 项。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

DataGrid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `SelectionChanged`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`AtomUIDataGridThemesProvider`、`CollectionViewGroupComparer`、`CollectionViewGroupRoot`、`ControlTokenTypePool`、`DataGrid`、`DataGridAbstractTextColumn`、`DataGridAutoGeneratingColumnEventArgs`、`DataGridBeginningEditEventArgs`、`DataGridBoundColumn`、`DataGridCell`、`DataGridCellCollection`、`DataGridCellCoordinates`、`DataGridCellEditEndedEventArgs`、`DataGridCellEditEndingEventArgs` 等 95 项。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础表格

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml:140`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:DataGrid x:Name="BasicCaseGrid"
               IsHideOnSinglePage="True"
               x:DataType="vm:DataGridBaseInfo"
                   AttachedToVisualTree="HandleExampleDataGridAttached">
    <atom:DataGrid.Columns>
        <atom:DataGridTemplateColumn Header="姓名">
            <atom:DataGridTemplateColumn.CellTemplate>
                <DataTemplate x:DataType="vm:DataGridBaseInfo">
                    <atom:HyperLinkTextBlock Text="{Binding Name}" />
                </DataTemplate>
            </atom:DataGridTemplateColumn.CellTemplate>
        </atom:DataGridTemplateColumn>
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" CanUserResize="True" />
        <atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" />
        <atom:DataGridTemplateColumn Header="标签">
            <atom:DataGridTemplateColumn.CellTemplate>
                <DataTemplate x:DataType="vm:DataGridBaseInfo">
                    <ItemsControl ItemsSource="{Binding Tags}">
                        <ItemsControl.ItemsPanel>
                            <ItemsPanelTemplate>
                                <StackPanel Orientation="Horizontal" Spacing="5" />
                            </ItemsPanelTemplate>
                        </ItemsControl.ItemsPanel>
                        <ItemsControl.ItemTemplate>
                            <DataTemplate x:DataType="vm:TagInfo">
                                <atom:Tag Text="{Binding Name}" TagColor="{Binding Color}"></atom:Tag>
                            </DataTemplate>
                        </ItemsControl.ItemTemplate>
                    </ItemsControl>
                </DataTemplate>
            </atom:DataGridTemplateColumn.CellTemplate>
        </atom:DataGridTemplateColumn>
        <atom:DataGridTemplateColumn Header="操作">
            <atom:DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal" Spacing="15">
                        <atom:HyperLinkTextBlock Text="邀请" />
                        <atom:HyperLinkTextBlock Text="修改" />
                        <atom:HyperLinkTextBlock Text="删除" />
                    </StackPanel>
                </DataTemplate>
            </atom:DataGridTemplateColumn.CellTemplate>
        </atom:DataGridTemplateColumn>
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 选择

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml:196`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:RadioButton x:Name="ExtendedSelection" IsChecked="True" IsCheckedChanged="HandleSelectionModeCheckedChanged" Content="多选" />
        <atom:RadioButton x:Name="SingleSelection" IsCheckedChanged="HandleSelectionModeCheckedChanged" Content="单选" />
    </StackPanel>
    <atom:DataGrid x:Name="SelectionDataGrid" SelectionMode="Extended" SelectTriggerType="Cell"
                           x:DataType="vm:DataGridBaseInfo"
                   AttachedToVisualTree="HandleExampleDataGridAttached">
        <atom:DataGrid.Columns>
            <atom:DataGridSelectionColumn />
            <atom:DataGridTemplateColumn Header="姓名">
                <atom:DataGridTemplateColumn.CellTemplate>
                    <DataTemplate x:DataType="vm:DataGridBaseInfo">
                        <atom:HyperLinkTextBlock Text="{Binding Name}" />
                    </DataTemplate>
                </atom:DataGridTemplateColumn.CellTemplate>
            </atom:DataGridTemplateColumn>
            <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
            <atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" />
            <atom:DataGridTemplateColumn Header="标签">
                <atom:DataGridTemplateColumn.CellTemplate>
                    <DataTemplate x:DataType="vm:DataGridBaseInfo">
                        <ItemsControl ItemsSource="{Binding Tags}">
                            <ItemsControl.ItemsPanel>
                                <ItemsPanelTemplate>
                                    <StackPanel Orientation="Horizontal" Spacing="5" />
                                </ItemsPanelTemplate>
                            </ItemsControl.ItemsPanel>
                            <ItemsControl.ItemTemplate>
                                <DataTemplate x:DataType="vm:TagInfo">
                                    <atom:Tag Text="{Binding Name}" TagColor="{Binding Color}"></atom:Tag>
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </DataTemplate>
                </atom:DataGridTemplateColumn.CellTemplate>
            </atom:DataGridTemplateColumn>
            <atom:DataGridTemplateColumn Header="操作">
                <atom:DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <StackPanel Orientation="Horizontal" Spacing="15">
                            <atom:HyperLinkTextBlock Text="邀请" />
                            <atom:HyperLinkTextBlock Text="修改" />
                            <atom:HyperLinkTextBlock Text="删除" />
                        </StackPanel>
                    </DataTemplate>
                </atom:DataGridTemplateColumn.CellTemplate>
            </atom:DataGridTemplateColumn>
        </atom:DataGrid.Columns>
    </atom:DataGrid>
</StackPanel>
```

### 拖拽调整列宽

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml:258`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:DataGrid x:Name="DragResizeColumn" IsHideOnSinglePage="True" CanUserResizeColumns="True"
                           x:DataType="vm:DataGridBaseInfo"
                   AttachedToVisualTree="HandleExampleDataGridAttached">
    <atom:DataGrid.Columns>
        <atom:DataGridTemplateColumn Header="姓名" CanUserResize="True">
            <atom:DataGridTemplateColumn.CellTemplate>
                <DataTemplate x:DataType="vm:DataGridBaseInfo">
                    <atom:HyperLinkTextBlock Text="{Binding Name}" />
                </DataTemplate>
            </atom:DataGridTemplateColumn.CellTemplate>
        </atom:DataGridTemplateColumn>
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" CanUserResize="True"
                                 CanUserSort="True" />
        <atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" />
        <atom:DataGridTemplateColumn Header="标签">
            <atom:DataGridTemplateColumn.CellTemplate>
                <DataTemplate x:DataType="vm:DataGridBaseInfo">
                    <ItemsControl ItemsSource="{Binding Tags}">
                        <ItemsControl.ItemsPanel>
                            <ItemsPanelTemplate>
                                <StackPanel Orientation="Horizontal" Spacing="5" />
                            </ItemsPanelTemplate>
                        </ItemsControl.ItemsPanel>
                        <ItemsControl.ItemTemplate>
                            <DataTemplate x:DataType="vm:TagInfo">
                                <atom:Tag Text="{Binding Name}" TagColor="{Binding Color}"></atom:Tag>
                            </DataTemplate>
                        </ItemsControl.ItemTemplate>
                    </ItemsControl>
                </DataTemplate>
            </atom:DataGridTemplateColumn.CellTemplate>
        </atom:DataGridTemplateColumn>
        <atom:DataGridTemplateColumn Header="操作">
            <atom:DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal" Spacing="15">
                        <atom:HyperLinkTextBlock Text="邀请" />
                        <atom:HyperLinkTextBlock Text="修改" />
                        <atom:HyperLinkTextBlock Text="删除" />
                    </StackPanel>
                </DataTemplate>
            </atom:DataGridTemplateColumn.CellTemplate>
        </atom:DataGridTemplateColumn>
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 尺寸

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml:314`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Spacing="10">
    <atom:DataGrid x:Name="LargeSizeDataGrid" SizeType="Large"
                           x:DataType="vm:DataGridBaseInfo"
                   AttachedToVisualTree="HandleExampleDataGridAttached">
        <atom:DataGrid.Columns>
            <atom:DataGridTextColumn Header="姓名"
                                     Binding="{Binding Name}" />
            <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
            <atom:DataGridTextColumn Header="地址"
                                     Binding="{Binding Address}" />
        </atom:DataGrid.Columns>
    </atom:DataGrid>

    <atom:DataGrid x:Name="MiddleSizeDataGrid" SizeType="Middle"
                           x:DataType="vm:DataGridBaseInfo"
                   AttachedToVisualTree="HandleExampleDataGridAttached">
        <atom:DataGrid.Columns>
            <atom:DataGridTextColumn Header="姓名"
                                     Binding="{Binding Name}" />
            <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
            <atom:DataGridTextColumn Header="地址"
                                     Binding="{Binding Address}" />
        </atom:DataGrid.Columns>
    </atom:DataGrid>

    <atom:DataGrid x:Name="SmallSizeDataGrid" SizeType="Small"
                           x:DataType="vm:DataGridBaseInfo"
                   AttachedToVisualTree="HandleExampleDataGridAttached">
        <atom:DataGrid.Columns>
            <atom:DataGridTextColumn Header="姓名"
                                     Binding="{Binding Name}" />
            <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
            <atom:DataGridTextColumn Header="地址"
                                     Binding="{Binding Address}" />
        </atom:DataGrid.Columns>
    </atom:DataGrid>
</StackPanel>
```

## 状态模型

DataGrid 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、collection/filter、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

DataGrid 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AtomUIDataGridThemesProvider.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DataGridCellTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridColumnGroupHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridColumnHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridHeaderViewItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridOperationButtons.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `DataGridRowExpanderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowGroupHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowReorderHandle.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DataGridTopLeftColumnHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridFilterIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DataGridFilterMenuItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridFilterTreeItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridMenuFilterFlyoutPresenterTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `DataGridSortIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DataGridTreeFilterFlyoutPresenterTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |

DataGrid 使用 `DataGridToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、collection/filter、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

DataGrid Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DataGridToken`，scope id 为 `DataGrid`，源码位于 `src/AtomUI.Desktop.Controls.DataGrid/DataGridToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls.DataGrid`：18 个文件，代表文件 `AtomUIDataGridThemesProvider.axaml`、`AtomUIDataGridThemesProvider.cs`、`DataGrid.Cells.cs`、`DataGrid.Columns.cs`、`DataGrid.Privates.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Cell`：4 个文件，代表文件 `DataGridCell.cs`、`DataGridCellCollection.cs`、`DataGridCellCoordinates.cs`、`DataGridCellsPresenter.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Column`：31 个文件，代表文件 `DataGridAbstractTextColumn.cs`、`DataGridBoundColumn.cs`、`DataGridCheckBoxColumn.cs`、`DataGridColumn.Privates.cs`、`DataGridColumn.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters`：7 个文件，代表文件 `DataGridFilterIndicator.cs`、`DataGridFilterItem.cs`、`DataGridFilterValuesSelectedEventArgs.cs`、`DataGridMenuFilterFlyout.cs`、`DataGridMenuFilterFlyoutPresenter.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Data`：15 个文件，代表文件 `CollectionViewGroupRoot.cs`、`DataGridCollectionView.cs`、`DataGridCollectionViewGroup.cs`、`DataGridCollectionViewGroupInternal.cs`、`DataGridCurrentChangingEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/EventArgs`：18 个文件，代表文件 `DataGridAutoGeneratingColumnEventArgs.cs`、`DataGridBeginningEditEventArgs.cs`、`DataGridCellEditEndedEventArgs.cs`、`DataGridCellEditEndingEventArgs.cs`、`DataGridCellEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.LanguageGenerator`：2 个文件，代表文件 `LanguageProviderPool.g.cs`、`LanguageResourceConst.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ResourceHost.ScopedResourceHostGenerator`：1 个文件，代表文件 `GenerateScopedResourceHostAttribute.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator`：2 个文件，代表文件 `ControlTokenTypePool.g.cs`、`TokenResourceConst.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Properties`：1 个文件，代表文件 `AssemblyInfo.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Row`：7 个文件，代表文件 `DataGridDetailsPresenter.cs`、`DataGridRow.Privates.cs`、`DataGridRow.cs`、`DataGridRowGroupHeader.cs`、`DataGridRowGroupInfo.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Themes`：21 个文件，代表文件 `DataGridCellTheme.axaml`、`DataGridColumnGroupHeaderTheme.axaml`、`DataGridColumnHeaderTheme.axaml`、`DataGridColumnHeaderTheme.cs`、`DataGridHeaderViewItemTheme.axaml` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Utils`：8 个文件，代表文件 `DataGridFrozenGrid.cs`、`DataGridHelper.cs`、`DataGridValueConverter.cs`、`KeyboardHelper.cs`、`Range.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Utils/Converters`：2 个文件，代表文件 `DataGridPaginationVisibilityConvertor.cs`、`DataGridUniformBorderThicknessToScalarConverter.cs`。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/data-grid/overview.md`
- 实现文档：`docs/controls/desktop/data-display/data-grid/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/data-grid/token.md`
- 变更记录：`docs/controls/desktop/data-display/data-grid/changelog.md`
- 语义结构：`./semantic-cn.md`
