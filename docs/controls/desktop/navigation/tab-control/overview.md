# TabControl 桌面版架构设计

本文档定义 `TabControl` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [TabControl 桌面版实现原理](implementation.md)，TabControl Token 的专项设计见 [TabControl Token 设计](token.md)，设计和契约变化记录见 [TabControl Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl` |
| 控件状态 | Stable |

TabControl 是 AtomUI 桌面控件体系中的标签页内容控件，用于在多个内容页面之间切换、关闭、拖动排序和溢出导航。

TabControl 不负责单纯页签条、路由系统或布局分割容器。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/TabControl`

## 2. 设计语言

TabControl 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | TabControl 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | TabControl 是 AtomUI 桌面控件体系中的标签页内容控件，用于在多个内容页面之间切换、关闭、拖动排序和溢出导航。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CloseIcon`、`ContentPadding`、`ContentTemplate`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent` 等 15 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、reorder、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | TabControl Token + ControlTheme。 |

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

TabControl 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `BaseOverflowMenuItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabControlTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `BaseTabItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabScrollViewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CardTabControlTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CardTabItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TabControlTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TabControlThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `TabItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabStripItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BaseTabStripTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CardTabStripItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CardTabStripTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TabStripItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TabStripTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

TabControl 使用 `TabControlToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

TabControl 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `BaseOverflowMenuItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `BaseTabControl`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `BaseTabControlTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `BaseTabItemTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `BaseTabScrollViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `BaseTabStrip`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `BaseTabStripItemTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `BaseTabStripTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `CardTabControl`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `CardTabStrip`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabControl`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabControlOverflowMenuItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `TabControlScrollViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabControlToken`：组件 Token scope，负责从全局 token 派生控件语义变量。
- `TabItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `TabItemData`：数据、状态或行为协作类型，维护集合同步和事件路径。
- `TabScrollContentPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `TabStrip`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabStripItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `TabStripOverflowMenuItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `TabStripScrollViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabsContainerPanel`：布局面板，负责测量、排列、虚拟化或集合内容布局。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme、控件文档和 Gallery ShowCase 示例保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 TabControl 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- 不把拖动排序实现为视觉容器重排；排序必须由集合 owner 提交，选择、内容、overflow 菜单和滚动状态都从同一个集合顺序推导。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

TabControl 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

`TabActivationTrigger` 定义鼠标 pointer 触发选择的提交时机。默认 `PointerReleased` 适合避免误触：左键按下 Tab 时只记录候选项和 pointer，只有同一个 pointer 在同一个 Tab 上释放且未进入拖动排序时才提交选择。`PointerPressed` 则保持按下立即激活的桌面快速切换体验。

选择触发配置只影响 pointer 选择时机，不改变键盘导航、access key、程序化设置 `SelectedIndex` / `SelectedItem`、关闭 Tab 后自动选择和拖动排序后的选中项回放。实现中不得绕过 Avalonia `SelectingItemsControl.UpdateSelectionFromEvent`，否则会导致 item container、content presenter、overflow 菜单和伪类状态不同步。

### 8.2 Tab 拖动排序模型

TabControl 的拖动排序是选择与集合模型的扩展能力，由 `IsTabReorderEnabled` 控制。关闭该属性时，Tab 保持原有点击选择、关闭和溢出导航行为；开启后，左键按下并超过拖动阈值才进入排序状态，普通点击仍按原选择语义处理。

排序必须以逻辑 item 为单位提交，不以生成的 `TabItem` 视觉容器作为数据源。`ItemsSource` 为可写 `IList` 时移动 `ItemsSource`；没有 `ItemsSource` 时移动 `Items`；只读或不可写数据源不执行排序提交。拖动释放前触发 `TabReordering`，事件可取消；提交成功后触发 `TabReordered`。事件参数应表达拖动 item、原 index、目标 index 和取消状态，不要求业务层读取内部容器。

实时目标位置按 `TabStripPlacement` 选择主轴：`Top` / `Bottom` 使用 X 轴，`Left` / `Right` 使用 Y 轴。拖拽中的源 Tab 不做自由二维漂浮，只在主轴上跟随 pointer：横向轨道的 Y 位移为 0，纵向轨道的 X 位移为 0。兄弟 Tab 的让位位移和排序判断也只使用主轴坐标，目标 index 由源 Tab 的前进边缘跨过被覆盖兄弟 Tab 主轴中线决定：向后拖动使用 trailing edge，向前拖动使用 leading edge。`HeaderStartExtraContent`、`HeaderEndExtraContent`、`PART_AddTabButton` 和 overflow 菜单项不参与排序目标计算；溢出场景下可在真实 Tab 视口边缘自动滚动，但不支持在 overflow 菜单内部直接拖动。

拖动视觉反馈必须接近 Chrome 浏览器标签行为：超过拖动阈值后源 Tab 提升到兄弟 Tab 上方并沿轨道移动；源 Tab 前进边缘未跨过被覆盖兄弟 Tab 的主轴中线前，兄弟 Tab 保持原位，不做让位也不切换排序目标；跨过半宽或半高阈值后，目标区间内的兄弟 Tab 通过临时 transform 按整格宽度或高度平滑让位。拖动中源 Tab 必须保持不透明背景；Line 模式使用当前激活面背景，Card 模式使用卡片激活面背景，避免覆盖时文字和图标叠穿。拖动中不使用插入线作为主反馈，不反复移动逻辑集合。释放时只提交一次真实集合 move；取消、capture lost、template reapply 或 detach 时恢复所有临时 transform、绘制层级和过渡设置。

选中状态必须跟随同一个逻辑 item，而不是跟随旧 index。重排完成后，`SelectedItem`、`SelectedIndex`、`SelectedContent`、选中指示条、关闭按钮状态和 overflow 菜单都应从新的集合顺序重新推导，不能用延迟刷新或强制重设选择掩盖状态同步问题。拖动预览期间，如果选中 Tab 是被拖源或正在让位的兄弟 Tab，`PART_SelectedItemIndicator` 必须叠加对应临时 transform 的主轴位移，使指示条跟随当前视觉位置，而不是停留在旧 layout bounds。

### 8.3 弹层与宿主模型

TabControl 涉及弹层、窗口或 overlay 宿主时，打开状态、取消事件、定位和宿主释放必须保持一致。重复打开、关闭、窗口失活和 template reapply 都必须释放旧宿主引用。

### 8.4 动效模型

TabControl 的动效只表达状态变化反馈，不应改变 public API 语义。初始加载、禁用态和卸载路径应能抑制或取消动效，避免保留旧控件实例。

### 8.5 视觉选项模型

TabControl 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [TabControl 桌面版实现原理](implementation.md)
- [TabControl Token 设计](token.md)
- [TabControl Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `TabControl` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、排序项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `reorder` | `拖动排序区域` | 承载拖动源、实时让位预览、自动滚动和集合顺序提交。 | `IsTabReorderEnabled`、`TabReordering`、`TabReordered` | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `token.md` + Gallery ShowCase | 生成 `controls/tab-control/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/tab-control/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | token.md、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、reorder、motion、visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| 拖动排序 | 覆盖 Top/Bottom 横向排序、Left/Right 纵向排序、选中项保持、可写/只读 ItemsSource、取消事件、overflow 自动滚动和 close/add 按钮排除。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Token 类型、生成数据和 token.md和文档同步。 |
| Gallery | 走查对应 ShowCase 示例和源码片段入口。 |
