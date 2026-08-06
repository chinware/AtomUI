# TabControl 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `TabControl` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、排序项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `reorder` | `拖动排序区域` | 承载拖动源、实时让位预览、自动滚动和集合顺序提交。 | `IsTabReorderEnabled`、`TabReordering`、`TabReordered` | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/TabControl/Themes/TabControlTheme.axaml`

```xml
<Border Name="Frame">
    <DockPanel>
        <Panel Name="PART_AlignWrapper">
            <Border>
                <DockPanel Name="HeaderLayout">
                    <ContentPresenter Name="HeaderStartExtraContent" />
                    <ContentPresenter Name="HeaderEndExtraContent" />
                    <TabControlScrollViewer Name="PART_TabsContainer">
                        <Panel>
                        </Panel>
                    </TabControlScrollViewer>
                </DockPanel>
            </Border>
        </Panel>
        <ContentPresenter />
    </DockPanel>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
TabControl
  -> BaseOverflowMenuItem (item container control theme, BaseOverflowMenuItemTheme.axaml)
     -> Border#Frame (template-stable)
        -> Grid (template-stable)
           -> ContentPresenter#ItemTextPresenter (internal-observable)
           -> IconButton#PART_ItemCloseButton (template-stable)
  -> TabItem (item container control theme, BaseTabItemTheme.axaml)
     -> Border#Frame (template-stable)
        -> StackPanel (template-stable)
           -> IconPresenter#ItemIconPresenter (internal-observable)
           -> ContentPresenter#ContentPresenter (internal-observable)
           -> IconButton#PART_ItemCloseButton (template-stable)
  -> BaseTabScrollViewer (control theme, BaseTabScrollViewerTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> Border#PART_ScrollStartEdgeIndicator (template-stable)
        -> Border#PART_ScrollEndEdgeIndicator (template-stable)
        -> DockPanel#ScrollViewLayout (template-stable)
           -> IconButton#PART_ScrollMenuIndicator (template-stable)
           -> TabScrollContentPresenter#ScrollViewContent (internal-observable)
  -> TabItem (item container control theme, CardTabItemTheme.axaml)
     -> Panel (template-stable)
        -> PixelAlignedBorder#Frame (template-stable)
           -> StackPanel (template-stable)
              -> IconPresenter#ItemIconPresenter (internal-observable)
              -> ContentPresenter#ContentPresenter (internal-observable)
              -> IconButton#PART_ItemCloseButton (template-stable)
        -> Rectangle#LineMask (template-stable)
  -> TabControl (control theme, TabControlTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel (template-stable)
           -> Panel#PART_AlignWrapper (template-stable)
              -> Border (template-stable)
                 -> DockPanel#HeaderLayout (template-stable)
                    -> ContentPresenter#HeaderStartExtraContent (internal-observable)
                    -> ContentPresenter#HeaderEndExtraContent (internal-observable)
                    -> TabControlScrollViewer#PART_TabsContainer (template-stable)
                       -> Panel (template-stable)
                          -> ItemsPresenter#PART_ItemsPresenter (template-stable)
                          -> Border#PART_SelectedItemIndicator (template-stable)
           -> ContentPresenter (internal-observable)
  -> TabItem (item container control theme, TabItemTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `TabControl` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `BaseOverflowMenuItem` | item container control theme | `BaseOverflowMenuItemTheme.axaml` | TabControl | `Background`, `CornerRadius`, `Header`, `HeaderTemplate`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `BaseOverflowMenuItemTheme.axaml` | BaseOverflowMenuItem | `Background`, `CornerRadius`, `Header`, `HeaderTemplate`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemTextPresenter` | template node (ContentPresenter) | `BaseOverflowMenuItemTheme.axaml` | BaseOverflowMenuItem | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ItemCloseButton` | template node (IconButton) | `BaseOverflowMenuItemTheme.axaml` | BaseOverflowMenuItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TabItem` | item container control theme | `BaseTabItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `CloseButtonOpacity`, `CloseIcon`, `Foreground`, `Header`, `HeaderTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `BaseTabItemTheme.axaml` | TabItem | `Background`, `CloseButtonOpacity`, `CloseIcon`, `Header`, `HeaderTemplate`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `BaseTabItemTheme.axaml` | TabItem | `CloseButtonOpacity`, `CloseIcon`, `Header`, `HeaderTemplate`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemIconPresenter` | template node (IconPresenter) | `BaseTabItemTheme.axaml` | TabItem | `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `BaseTabItemTheme.axaml` | TabItem | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ItemCloseButton` | template node (IconButton) | `BaseTabItemTheme.axaml` | TabItem | `CloseButtonOpacity`, `CloseIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `BaseTabScrollViewer` | control theme | `BaseTabScrollViewerTheme.axaml` | TabControl | `HorizontalSnapPointsAlignment`, `HorizontalSnapPointsType`, `Padding`, `TabStripPlacement`, `VerticalSnapPointsAlignment`, `VerticalSnapPointsType` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (Panel) | `BaseTabScrollViewerTheme.axaml` | BaseTabScrollViewer | `HorizontalSnapPointsAlignment`, `HorizontalSnapPointsType`, `Padding`, `TabStripPlacement`, `VerticalSnapPointsAlignment`, `VerticalSnapPointsType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScrollStartEdgeIndicator` | template node (Border) | `BaseTabScrollViewerTheme.axaml` | BaseTabScrollViewer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScrollEndEdgeIndicator` | template node (Border) | `BaseTabScrollViewerTheme.axaml` | BaseTabScrollViewer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ScrollViewLayout` | template node (DockPanel) | `BaseTabScrollViewerTheme.axaml` | BaseTabScrollViewer | `HorizontalSnapPointsAlignment`, `HorizontalSnapPointsType`, `Padding`, `TabStripPlacement`, `VerticalSnapPointsAlignment`, `VerticalSnapPointsType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScrollMenuIndicator` | template node (IconButton) | `BaseTabScrollViewerTheme.axaml` | BaseTabScrollViewer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ScrollViewContent` | template node (TabScrollContentPresenter) | `BaseTabScrollViewerTheme.axaml` | BaseTabScrollViewer | `HorizontalSnapPointsAlignment`, `HorizontalSnapPointsType`, `Padding`, `TabStripPlacement`, `VerticalSnapPointsAlignment`, `VerticalSnapPointsType` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TabItem` | item container control theme | `CardTabItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CloseButtonOpacity`, `CloseIcon`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `CardTabItemTheme.axaml` | TabItem | `Background`, `BorderBrush`, `BorderThickness`, `CloseButtonOpacity`, `CloseIcon`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (PixelAlignedBorder) | `CardTabItemTheme.axaml` | TabItem | `Background`, `BorderBrush`, `BorderThickness`, `CloseButtonOpacity`, `CloseIcon`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `CardTabItemTheme.axaml` | TabItem | `CloseButtonOpacity`, `CloseIcon`, `Header`, `HeaderTemplate`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemIconPresenter` | template node (IconPresenter) | `CardTabItemTheme.axaml` | TabItem | `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `CardTabItemTheme.axaml` | TabItem | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ItemCloseButton` | template node (IconButton) | `CardTabItemTheme.axaml` | TabItem | `CloseButtonOpacity`, `CloseIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `LineMask` | template node (Rectangle) | `CardTabItemTheme.axaml` | TabItem | `Background`, `LineMaskMargin` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TabControl` | control theme | `TabControlTheme.axaml` | 用户代码 / 控件宿主 | `ContentPadding`, `EffectiveHeaderPadding`, `HeaderEndExtraContent`, `HeaderEndExtraContentTemplate`, `HeaderStartExtraContent`, `HeaderStartExtraContentTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `TabControlTheme.axaml` | TabControl | `ContentPadding`, `EffectiveHeaderPadding`, `HeaderEndExtraContent`, `HeaderEndExtraContentTemplate`, `HeaderStartExtraContent`, `HeaderStartExtraContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `TabControlTheme.axaml` | TabControl | `ContentPadding`, `EffectiveHeaderPadding`, `HeaderEndExtraContent`, `HeaderEndExtraContentTemplate`, `HeaderStartExtraContent`, `HeaderStartExtraContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_AlignWrapper` | template node (Panel) | `TabControlTheme.axaml` | TabControl | `EffectiveHeaderPadding`, `HeaderEndExtraContent`, `HeaderEndExtraContentTemplate`, `HeaderStartExtraContent`, `HeaderStartExtraContentTemplate`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (DockPanel) | `TabControlTheme.axaml` | TabControl | `HeaderEndExtraContent`, `HeaderEndExtraContentTemplate`, `HeaderStartExtraContent`, `HeaderStartExtraContentTemplate`, `IsMotionEnabled`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderStartExtraContent` | template node (ContentPresenter) | `TabControlTheme.axaml` | TabControl | `HeaderStartExtraContent`, `HeaderStartExtraContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `HeaderEndExtraContent` | template node (ContentPresenter) | `TabControlTheme.axaml` | TabControl | `HeaderEndExtraContent`, `HeaderEndExtraContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_TabsContainer` | template node (TabControlScrollViewer) | `TabControlTheme.axaml` | TabControl | `IsMotionEnabled`, `ItemsPanel`, `SelectedIndicatorRenderTransform`, `TabStripPlacement` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `TabControlTheme.axaml` | TabControl | `ItemsPanel`, `SelectedIndicatorRenderTransform` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `TabControlTheme.axaml` | TabControl | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`ContentPadding`、`ContentTemplate`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent`、`HeaderStartExtraContentTemplate`、`HorizontalContentAlignment` 等 15 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsSelected`、`IsTabReorderEnabled`、`TabActivationTrigger`、`SelectedIndex`、`SelectedItem`、`ItemsSource` | 维护选择触发时机、集合顺序、拖动排序和内容页状态。 |
| 交互与状态 | `IsAutoHideCloseButton`、`IsClosable`、`IsMotionEnabled`、`IsShowAddTabButton`、`IsTabAutoHideCloseButton`、`IsTabClosable` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `SizeType`、`TabAlignmentCenter`、`TabStripPlacement` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `AddTabButton`、`TabScrollViewer` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、reorder、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | TabControl Token + ControlTheme。 |

## State Flow

TabControl 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。
- Tab 激活触发由 `TabActivationTrigger` 控制，默认值为 `PointerReleased`；按下时只记录候选 Tab，只有鼠标在同一个 Tab 上松开才激活。
- `TabActivationTrigger=PointerPressed` 表达按下立即激活；该模式仍必须通过统一选择入口更新 `SelectedIndex`、`SelectedItem`、内容页、伪类和主题状态。
- `PointerReleased` 模式下，按下 Tab A、移动到 Tab B 或 Tab 外松开不应激活新 Tab；拖动排序进入 active reorder 后，释放事件不得再触发 Tab 激活。
- 拖动排序开启后，排序结果必须提交到 `ItemsSource` 或 `Items` 的逻辑集合顺序；拖动过程采用 Chrome 式轨道内实时让位预览，被拖 Tab 只沿 Tab 轨道主轴移动并覆盖在兄弟 Tab 上方，其他 Tab 通过临时 transform 让出目标位置，不能直接把 `ItemsPresenter.Panel.Children` 当作排序数据源。
- `TabStripPlacement=Top/Bottom` 时主轴为 X 轴，被拖 Tab 的 Y 位移必须保持为 0；`TabStripPlacement=Left/Right` 时主轴为 Y 轴，被拖 Tab 的 X 位移必须保持为 0。目标位置由被拖 Tab 的前进边缘跨过被覆盖兄弟 Tab 主轴中线决定：向后拖动使用 trailing edge，向前拖动使用 leading edge，相当于覆盖兄弟 Tab 约一半宽度或高度即触发让位，而不是等待被拖 Tab 视觉中心跨过兄弟中心。

## Theme and Token Boundaries

TabControl 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `BaseOverflowMenuItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabControlTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `BaseTabItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabScrollViewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CardTabControlTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CardTabItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TabControlTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TabItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabStripItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabStripTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CardTabStripItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CardTabStripTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TabStripItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TabStripTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

TabControl 使用 `TabControlToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

TabControl Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TabControlToken`，scope id 为 `TabControl`，源码位于 `src/AtomUI.Desktop.Controls/TabControl/TabControlToken.cs`。

## Customization Boundaries

维护 TabControl 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- 不把拖动排序实现为视觉容器重排；排序必须由集合 owner 提交，选择、内容、overflow 菜单和滚动状态都从同一个集合顺序推导。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 TabControl 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 拖动排序释放时必须修改逻辑集合顺序，拖动中允许用 `RenderTransform` 和临时 `ZIndex` 做实时视觉预览，但不能只调整 `Panel.Children`、`ZIndex` 或 transform 作为最终排序结果。
- 选中项必须跟随同一个逻辑 item，不能跟随旧 index；重排后内容页、指示条、overflow 菜单和关闭状态必须从新顺序统一推导。
- `TabActivationTrigger` 只能改变 pointer 激活提交时机，不能改变键盘选择、access key、关闭后选择、程序化选择或拖动排序后的选中项回放语义。
- `PointerReleased` 候选激活状态必须由控件 owner 持有并按 pointer 会话释放，不能让旧 `TabItem` 或旧 pointer 引用跨 template reapply / detach 存活。
- 所有拖动临时状态必须在提交、取消、capture lost、template reapply 和 detach 时释放，不能保留旧容器或旧 adorner。
- 垂直图标槽对齐不能改变 `Top` / `Bottom` 的紧凑布局；不能新增 public API、Token 或 Gallery-only workaround；`TabControl`、`TabStrip`、`CardTabControl` 和 `CardTabStrip` 的同组混合有图标/无图标布局必须使用同一套 owner 推导规则。
- 默认 Line Tab 的 `Left` / `Right` spacing / padding 调整不得影响 Card Tab、拖动排序阈值、选中指示条定位或 overflow 计算；选中指示条高度必须继续跟随 Line item 的真实 bounds。
- 切换 `TabStripPlacement` 后当前选中项必须继续跟随同一个逻辑 item，不能因 container 重新准备或旧 `IsSelected` 状态回流而改变。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
