# TabControl

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

TabControl 是 AtomUI 桌面控件体系中的标签页内容控件，用于在多个内容页面之间切换、关闭、拖动排序和溢出导航。

TabControl 不负责单纯页签条、路由系统或布局分割容器。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/TabControl`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl` |
| 状态 | Stable |

## 何时使用

TabControl 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | TabControl 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | TabControl 是 AtomUI 桌面控件体系中的标签页内容控件，用于在多个内容页面之间切换、关闭、拖动排序和溢出导航。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CloseIcon`、`ContentPadding`、`ContentTemplate`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent` 等 15 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、reorder、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | TabControl Token + ControlTheme。 |

## 公共 API

TabControl 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`ContentPadding`、`ContentTemplate`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent`、`HeaderStartExtraContentTemplate`、`HorizontalContentAlignment` 等 15 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsSelected`、`IsTabReorderEnabled`、`TabActivationTrigger`、`SelectedIndex`、`SelectedItem`、`ItemsSource` | 维护选择触发时机、集合顺序、拖动排序和内容页状态。 |
| 交互与状态 | `IsAutoHideCloseButton`、`IsClosable`、`IsMotionEnabled`、`IsShowAddTabButton`、`IsTabAutoHideCloseButton`、`IsTabClosable` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `SizeType`、`TabAlignmentCenter`、`TabStripPlacement` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `AddTabButton`、`TabScrollViewer` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `AddTabRequest`、`CloseTab`、`Closed`、`Closing`、`TabReordering`、`TabReordered`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`BaseOverflowMenuItem`、`BaseTabControl`、`BaseTabScrollViewer`、`BaseTabStrip`、`CardTabControl`、`CardTabStrip`、`CloseTabRequestEventArgs`、`TabClosedEventArgs`、`TabClosingEventArgs`、`TabControl`、`TabControlOverflowMenuItem`、`TabControlScrollViewer`、`TabItem`、`TabItemData` 等 22 项。
- 枚举：`TabActivationTrigger`、`TabSharp`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddTabButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_AlignWrapper` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_CardTabStripScrollViewer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ItemCloseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_ScrollEndEdgeIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_ScrollMenuIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_ScrollStartEdgeIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_SelectedItemIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_TabsContainer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `Bottom=:bottom`、`Left=:left`、`Right=:right`、`TabPseudoClass.Bottom`、`TabPseudoClass.Left`、`TabPseudoClass.Right`、`TabPseudoClass.Top`、`Top=:top`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

TabControl 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `AddTabRequest`、`CloseTab`、`Closed`、`Closing`、`TabReordering`、`TabReordered`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`BaseOverflowMenuItem`、`BaseTabControl`、`BaseTabScrollViewer`、`BaseTabStrip`、`CardTabControl`、`CardTabStrip`、`CloseTabRequestEventArgs`、`TabClosedEventArgs`、`TabClosingEventArgs`、`TabControl`、`TabControlOverflowMenuItem`、`TabControlScrollViewer`、`TabItem`、`TabItemData` 等 22 项。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Views/TabControlShowCase.axaml:131`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:TabControl Name="TestControl">
        <atom:TabItem Header="标签页 1" Content="标签页内容 1" />
        <atom:TabItem Header="标签页 2" Content="标签页内容 2" />
        <atom:TabItem Header="标签页 3" Content="标签页内容 3" />
    </atom:TabControl>
</StackPanel>
```

### 通过 ItemSource 生成标签项

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Views/TabControlShowCase.axaml:148`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:TabControl ItemsSource="{Binding TabItemDataSource}">
        <atom:TabControl.ItemTemplate>
            <DataTemplate x:DataType="views:MyTabItemData">
                <Border Padding="10">
                    <TextBlock Text="{Binding Content}" />
                </Border>
            </DataTemplate>
        </atom:TabControl.ItemTemplate>
    </atom:TabControl>
</StackPanel>
```

### 禁用标签

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Views/TabControlShowCase.axaml:240`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:CardTabControl HeaderStartEdgePadding="">
        <atom:TabItem Header="标签页 1" Content="标签页内容 1" />
        <atom:TabItem Header="标签页 2" IsEnabled="False" Content="标签页内容 2" />
        <atom:TabItem Header="标签页 3" Content="标签页内容 3" />
    </atom:CardTabControl>

    <atom:TabControl>
        <atom:TabItem Header="标签页 1" Content="标签页内容 1" />
        <atom:TabItem Header="标签页 2" IsEnabled="False" Content="标签页内容 2" />
        <atom:TabItem Header="标签页 3" Content="标签页内容 3" />
    </atom:TabControl>
</StackPanel>
```

### 居中显示

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Views/TabControlShowCase.axaml:263`

Gallery key：`ExamplesContent` / item `5`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:TabControl TabAlignmentCenter="True">
        <atom:TabItem Header="标签页 1" Content="标签页内容 1" />
        <atom:TabItem Header="标签页 2" Content="标签页内容 2" />
        <atom:TabItem Header="标签页 3" Content="标签页内容 3" />
    </atom:TabControl>

    <atom:CardTabControl TabAlignmentCenter="True">
        <atom:TabItem Header="标签页 1" Content="标签页内容 1" />
        <atom:TabItem Header="标签页 2" Content="标签页内容 2" />
        <atom:TabItem Header="标签页 3" Content="标签页内容 3" />
    </atom:CardTabControl>

</StackPanel>
```

## 状态模型

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
- `IsTabClosable` 是生成 `TabItem` 的模板级默认值；overflow 菜单使用容器最终生效的 `IsClosable`，因此控件级默认、单项覆盖和 overflow 呈现必须保持同一语义。
- 拖动排序开启后，排序结果必须提交到 `ItemsSource` 或 `Items` 的逻辑集合顺序；拖动过程采用 Chrome 式轨道内实时让位预览，被拖 Tab 只沿 Tab 轨道主轴移动并覆盖在兄弟 Tab 上方，其他 Tab 通过临时 transform 让出目标位置，不能直接把 `ItemsPresenter.Panel.Children` 当作排序数据源。
- `TabStripPlacement=Top/Bottom` 时主轴为 X 轴，被拖 Tab 的 Y 位移必须保持为 0；`TabStripPlacement=Left/Right` 时主轴为 Y 轴，被拖 Tab 的 X 位移必须保持为 0。目标位置由被拖 Tab 的前进边缘跨过被覆盖兄弟 Tab 主轴中线决定：向后拖动使用 trailing edge，向前拖动使用 leading edge，相当于覆盖兄弟 Tab 约一半宽度或高度即触发让位，而不是等待被拖 Tab 视觉中心跨过兄弟中心。
- overflow 菜单项是对应 `TabItem` 的临时替代呈现，不拥有独立的关闭语义；其 `IsClosable` 必须复制源 Tab 的有效值，关闭请求必须回到 `BaseTabControl.CloseTab` 统一处理。

## 主题与 Design Token

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
- `BaseOverflowMenuItemTheme` 必须根据 `IsClosable` 控制 `PART_ItemCloseButton` 的可见性：不可关闭项隐藏关闭按钮，可关闭项显示关闭按钮；该规则对 `TabControl`、`CardTabControl` 及其对应 overflow item 统一生效。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

TabControl Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TabControlToken`，scope id 为 `TabControl`，源码位于 `src/AtomUI.Desktop.Controls/TabControl/TabControlToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- 拖动 move 帧内只更新轻量 transform、目标 index 和自动滚动请求；主轴约束和目标 index 计算必须是纯几何计算，不得在 pointer move 中反复移动集合、重建 item 容器或重新应用模板。
- 拖动预览 transform、绘制层级、计时器和订阅应按交互会话缓存并在会话结束释放；不得因一次拖动永久保留视觉对象或数据 item。
- 拖动排序不得引入运行时反射、动态类型扫描或 AOT 不友好的事件发现路径。
- 图标槽对齐不得为无图标 Tab 创建额外图标控件、动态占位对象或 C# 运行时模板分支；应复用静态 AXAML 槽位、现有资源绑定和内部布尔状态，避免增加模板实例化和 container recycle 成本。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/TabControl`：20 个文件，代表文件 `BaseOverflowMenuItem.cs`、`BaseTabControl.cs`、`BaseTabScrollViewer.cs`、`CardTabControl.cs`、`CardTabControl.SemanticParts.cs`、`TabControl.SemanticParts.cs`、`TabItem.SemanticParts.cs`、`ScrollContentPresenterReflectionExtensions.cs` 等。
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip`：9 个文件，代表文件 `BaseTabStrip.cs`、`CardTabStrip.cs`、`CardTabStrip.SemanticParts.cs`、`TabStrip.cs`、`TabStrip.SemanticParts.cs`、`TabStripItem.cs`、`TabStripItem.SemanticParts.cs`、`TabStripOverflowMenuItem.cs` 等。
- `src/AtomUI.Desktop.Controls/TabControl/Themes`：26 个文件（含 `TabStrip` 子目录），代表文件 `BaseOverflowMenuItemTheme.axaml`、`BaseTabControlTheme.axaml`、`BaseTabControlTheme.cs`、`BaseTabItemTheme.axaml`、`BaseTabItemTheme.cs`、`CardTabControlTheme.axaml`、`CardTabItemTheme.axaml` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。
- Tab 拖动排序属于 TabControl 家族的集合与选择协作路径；实现应落在 `BaseTabControl`、Tab item 容器、滚动视口和内部拖动协作对象之间，不能把排序状态散落到 Gallery、theme 或业务数据对象中。
- overflow 菜单属于滚动视口创建的临时呈现层；`BaseOverflowMenuItem` 只承载菜单视觉和请求转发，`BaseTabControl` 仍是关闭状态、事件和集合变更的唯一 owner。
- 垂直页签图标对齐属于 TabControl 家族的 owner 级布局状态；`Left` / `Right` placement 下由 owner 统一判断同组是否存在图标，再把内部保留图标槽状态投射到 item container，不能通过 Gallery 手工补空图标或新增 public API。
- 默认 Line Tab 的 `Left` / `Right` placement 应保持紧凑的垂直节奏，减少无意义高度浪费；相邻间距和 item 自身垂直 padding 都应按 Line 紧凑模型处理。Card Tab 使用独立 `CardGutter` 和 Card padding 视觉节奏，本规则不得改变 Card 外观。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/tab-control/overview.md`
- 实现文档：`docs/controls/desktop/navigation/tab-control/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/navigation/tab-control/semantic-part.md`
- Token 文档：`docs/controls/desktop/navigation/tab-control/token.md`
- 变更记录：`docs/controls/desktop/navigation/tab-control/changelog.md`
- 语义结构：`./semantic-cn.md`
