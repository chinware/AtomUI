# TabStrip 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `TabStrip` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、排序项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `reorder` | `拖动排序区域` | 承载拖动源、实时让位预览、自动滚动和集合顺序提交。 | `IsTabReorderEnabled`、`TabReordering`、`TabReordered` | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/TabControl/Themes/TabStrip/TabStripTheme.axaml`

```xml
<Border Name="Frame">
    <Panel Name="AlignWrapper">
        <Border>
            <DockPanel Name="HeaderLayout">
                <ContentPresenter Name="HeaderStartExtraContent" />
                <ContentPresenter Name="HeaderEndExtraContent" />
                <TabStripScrollViewer Name="PART_TabsContainer">
                    <Panel>
                        <ItemsPresenter Name="PART_ItemsPresenter" />
                        <Border Name="PART_SelectedItemIndicator" />
                    </Panel>
                </TabStripScrollViewer>
            </DockPanel>
        </Border>
    </Panel>
</Border>
```

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent`、`HeaderStartExtraContentTemplate`、`Icon` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsTabReorderEnabled`、`TabActivationTrigger`、`SelectedIndex`、`SelectedItem`、`ItemsSource` | 维护页签选择触发时机、集合顺序和拖动排序状态。 |
| 交互与状态 | `IsAutoHideCloseButton`、`IsClosable`、`IsMotionEnabled`、`IsShowAddTabButton`、`IsTabAutoHideCloseButton`、`IsTabClosable` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `SizeType`、`TabAlignmentCenter`、`TabStripPlacement` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、reorder、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## State Flow

TabStrip 的状态流按以下路径收敛：

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
- `TabActivationTrigger=PointerPressed` 表达按下立即激活；该模式仍必须通过统一选择入口更新 `SelectedIndex`、`SelectedItem`、伪类和主题状态。
- `PointerReleased` 模式下，按下 Tab A、移动到 Tab B 或 Tab 外松开不应激活新 Tab；拖动排序进入 active reorder 后，释放事件不得再触发 Tab 激活。
- 拖动排序开启后，排序结果必须提交到 `ItemsSource` 或 `Items` 的逻辑集合顺序；拖动过程采用 Chrome 式轨道内实时让位预览，被拖 Tab 只沿 Tab 轨道主轴移动并覆盖在兄弟 Tab 上方，其他 Tab 通过临时 transform 让出目标位置，不能直接把 `ItemsPresenter.Panel.Children` 当作排序数据源。
- `TabStripPlacement=Top/Bottom` 时主轴为 X 轴，被拖 Tab 的 Y 位移必须保持为 0；`TabStripPlacement=Left/Right` 时主轴为 Y 轴，被拖 Tab 的 X 位移必须保持为 0。目标位置由被拖 Tab 的前进边缘跨过被覆盖兄弟 Tab 主轴中线决定：向后拖动使用 trailing edge，向前拖动使用 leading edge，相当于覆盖兄弟 Tab 约一半宽度或高度即触发让位，而不是由 pointer 的非主轴偏移决定。

## Theme and Token Boundaries

TabStrip 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

当前控件未抽取到专属 AXAML 主题文件；视觉契约主要来自继承控件、共享主题和资源 key。

TabStrip 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

- TabStrip 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## Customization Boundaries

维护 TabStrip 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- 不把拖动排序实现为视觉容器重排；排序必须由集合 owner 提交，选择、overflow 菜单和滚动状态都从同一个集合顺序推导。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 TabStrip 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 拖动排序释放时必须修改逻辑集合顺序，拖动中允许用 `RenderTransform` 和临时 `ZIndex` 做实时视觉预览，但不能只调整 `Panel.Children`、`ZIndex` 或 transform 作为最终排序结果。
- 选中项必须跟随同一个逻辑 item，不能跟随旧 index；重排后指示条、overflow 菜单和关闭状态必须从新顺序统一推导。
- `TabActivationTrigger` 只能改变 pointer 激活提交时机，不能改变键盘选择、access key、关闭后选择、程序化选择或拖动排序后的选中项回放语义。
- `PointerReleased` 候选激活状态必须由控件 owner 持有并按 pointer 会话释放，不能让旧 `TabStripItem` 或旧 pointer 引用跨 template reapply / detach 存活。
- 所有拖动临时状态必须在提交、取消、capture lost、template reapply 和 detach 时释放，不能保留旧容器或旧 adorner。
- 垂直图标槽对齐不能改变 `Top` / `Bottom` 的紧凑布局；不能新增 public API、Token 或 Gallery-only workaround；`TabControl`、`TabStrip`、`CardTabControl` 和 `CardTabStrip` 的同组混合有图标/无图标布局必须使用同一套 owner 推导规则。
- 默认 Line TabStrip 的 `Left` / `Right` spacing / padding 调整不得影响 Card TabStrip、拖动排序阈值、选中指示条定位或 overflow 计算；选中指示条高度必须继续跟随 Line item 的真实 bounds。
- 切换 `TabStripPlacement` 后当前选中项必须继续跟随同一个逻辑 item，不能因 container 重新准备或旧 `IsSelected` 状态回流而改变。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
