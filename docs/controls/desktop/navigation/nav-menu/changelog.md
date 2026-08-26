# NavMenu Changelog

本文档记录 NavMenu 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-08-26

- Architecture
  - Separate transient popup/submenu closing from persistent selection clearing: pointer outside, window deactivation, platform focus loss, non-client click, inline-collapsed transitions, and mode changes preserve `SelectedItem` and selected path.
  - Keep public `Close()` compatible as the explicit operation that closes all submenus and clears selection.
- Verification
  - Cover platform focus loss, mode replacement, popup reopen projection, and the public `Close()` selection-clearing boundary.

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record NavMenu/NavMenuItem as the semantic owner for NavMenu.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-19

- API
  - 节点 `Tooltip` 支持直接传入 `ToolTip` 实例：实例上显式设置的呈现类附加属性（位置、颜色、箭头、文本换行等）在打开时优先于菜单级与宿主配置，未设置的回落；纯文本用法不变。
- Verification
  - 覆盖 ToolTip 实例经节点、binder、容器到 `PART_Header` 的原样透传。

## 2026-08-18

- API
  - 新增 `NavMenuNode.Tooltip` / `INavMenuNode.Tooltip` 独立节点提示内容，未设置时回退到节点 `Header`。
  - 新增节点级 `IsTooltipEnabled`，以及菜单级 `IsCollapsedTooltipEnabled`、`CollapsedTooltipPlacement`、`CollapsedTooltipShowDelay`、`CollapsedTooltipBetweenShowDelay` 折叠提示策略。
- Architecture
  - 由 `NavMenuItem` 统一计算 `EffectiveCollapsedTooltip`，只允许有效 inline collapsed 状态下的顶层叶子节点生成提示内容。
  - 将节点真实 `Header` 单独投影为 `NodeHeader`，避免 generated container 的节点对象进入首字符转换或 Tooltip fallback。
  - 节点 Tooltip、节点开关和菜单策略 binding 归入现有 container `CompositeDisposable`，在 rebind、clear 和 recycle 时统一释放并清空。
- Theme
  - 将 `ToolTip.Tip`、placement 和 delay 附加到实际 `VerticalNavMenuItemHeader`，保持 `ToolTip` 服务的视觉宿主边界。
  - 无图标折叠项的首字符改为从节点 `Header` 投影取得，不再对整个节点调用 `ToString()`。
- Verification
  - 覆盖显式 Tooltip、Header fallback、节点/菜单禁用、submenu 抑制、模式切换、运行期更新、自定义节点通知、动态资源和容器回收。

## 2026-08-05

- API
  - 定义 `INavMenuEntry` 作为节点、分组和分隔线的共同结构契约，`INavMenuNode` 保持唯一可交互节点语义。
  - 定义 `NavMenuGroup`、`NavMenuDivider` 和 `NavMenuNode.Entries`，支持在根、子菜单、popup 和分组中任意层级组合结构 entry。
  - 保留 `NavMenuNode.Children : IList<INavMenuNode>`，将其定义为 `Entries` 的实时语义节点兼容视图；`INavMenuNode.Entries : IEnumerable<INavMenuEntry>` 通过协变接口默认 `Entries => Children` 保持既有自定义节点兼容，`NavMenuNode.Entries` 继续提供 `IList<INavMenuEntry>` 可写入口。
  - 定义 `NavMenu.Header`、`HeaderTemplate`、`Footer`、`FooterTemplate` 和 `ItemSpacing`；继续以 `IsInlineCollapsed` 作为唯一折叠状态源，不增加相反语义的 `Expanded` 属性。
- Architecture
  - 定义 `NavMenuItem`、`NavMenuGroupItem`、`NavMenuDividerItem` 三类内部容器，并由统一 entry container coordinator 管理类型分派、独立 recycle key、prepare 和 clear。
  - 定义 direct `Items`、`ItemsSource`、节点 `Entries` 和分组 `Entries` 的统一 entry source 校验；初始装载、source replacement、Add、Replace、Reset 均只接受 `INavMenuNode`、`NavMenuGroup` 或 `NavMenuDivider`，仅实现 marker 的未知种类也在容器生成前确定性失败。
  - 为内置 `NavMenuNode` / `NavMenuGroup` 定义唯一弱 structural owner，禁止同一有状态实例在 entry 树中重复挂载，并在 Remove、Replace、Clear 和根 source removal 后释放；无状态 `NavMenuDivider` 保持可复用。
  - 每个内置 entry owner 和根 `NavMenu` 都协调自己的 structural scope：递归穿过 custom node，遇到 built-in child 后由 child 自身协调器接管。custom node 的内置后代继承最近 scope owner，使离线树和根菜单中的 custom wrapper 都无法绕过唯一性；动态 source 重新执行 owner-cycle 校验，拒绝指回当前 built-in owner 或祖先。协调器只弱订阅当前 scope 的 custom source，避免纯 built-in 深树形成 O(N²) 祖先订阅和重复扫描。
  - `Entries` 的 Add、Insert、Replace 和批量初始化在 mutation 前预检完整 prospective tree；成功写入先提交全部 structural ownership，再调用自定义 parent callback、投影父级并发送通知，后续项冲突、callback 重入或同步观察者重入都不能留下部分写入或抢占 entry。
  - 将逻辑树父级与导航语义父级分离；分组对 `ParentNode`、`Level`、`IsTopLevel`、selection path、default path、Accordion 和 keyboard navigation 保持透明。
  - 定义无扁平列表分配的 semantic navigator，按已生成容器处理同层和 inline 可见树漫游，跳过分组、分隔线和不可交互项。
  - 将 generated container clear 定义为 selection 与 interaction 的统一失效出口，清除 realized selection、keyboard active、pointer press/release 目标和指向旧容器的 hover 延迟任务。
  - 明确模板重应用与容器回收的状态边界：`NavMenuItem.OnApplyTemplate` 只替换 template part、订阅和局部 motion 资源，不清理打开路径或选中路径；节点状态只由对应 coordinator 和真实 container clear 路径维护。
  - selection coordinator 区分已应用节点身份和临时 realized container 引用；容器回收只失效临时引用，视觉树 detach 保留已应用节点身份，后续选择按语义路径解析并清除当前旧容器，避免模板重建或重新挂载后出现多个 selected leaf。
  - generated node container 在 prepare 完成后统一从 selection coordinator 投影 `IsSelected` / `IsInSelectedPath`，使任意深度 collapsed popup 延迟生成、关闭重开或容器复用时恢复同一持久选择，不依赖 popup 打开事件或 dispatcher 刷新时机。
  - generated node container clear 完整复位 `IsSelected`、`IsInSelectedPath` 和 `IsSubMenuOpen`，避免 recycle 把旧节点视觉状态转移给新节点。
  - keyboard active 有效性同时检查 effective visible/effective enabled 与语义父链打开状态，避免 popup 关闭后提交仍被框架保留的 child container。
- Theme
  - 定义 Inline/Vertical 固定 Header/Footer 与中间滚动 entry 区，Horizontal 左 Header、右 Footer 和中间菜单区。
  - 定义 root inline collapsed 隐藏分组标题、popup 分组标题保持可见、Horizontal 根分组透明和 divider orientation 规则。
  - 明确 root inline collapsed 的透明分组后代继续继承折叠视觉并使用 `CollapsedIconSize` 居中；Footer 在折叠态退出布局，Header 保持可见以承载展开入口。
  - 根 ItemsPanel 通过 `TemplateBinding` 消费公开 `ItemSpacing`；后代 ItemsPanel 消费内部可继承的 `EntryItemSpacing`，既保持 Horizontal 根层默认 `0`，也使 popup、submenu 和 group 正确使用垂直 spacing Token；该路径不使用穿透子控件模板的 selector 或逐容器 binding。
- Token
  - 复用 `GroupTitleColor`、`DarkGroupTitleColor`、`GroupTitleLineHeight`、`GroupTitleFontSize` 表达非交互分组标题，不新增专属 divider token。
  - 明确 `CollapsedIconSize` 默认映射全局 `IconSizeLG`，相对普通 `ItemIconSize=IconSize` 使用大一档图标尺寸。
  - 明确 `VerticalItemsPanelSpacing=0` 保持默认 block margin 视觉；显式 `NavMenu.ItemSpacing` 是实例级额外 panel spacing，可覆盖根与后代默认 ItemsPanel，但不改写 Token。
  - 将 `MenuPopupMaxHeight` 默认值定义为八个标准菜单项高度；短菜单保持自然高度，长菜单在固定可见范围内滚动。
- Verification
  - 定义任意层级结构 entry、集合全动作、非法数据确定性失败、路径、选择、键盘、collapsed、Header/Footer、spacing、资源释放、容器回收隔离和纯节点快路径验证矩阵。
  - 覆盖 active container 移除后的 Enter、pressed container 移除后的 pointer release、pending open/close 目标回收，以及 popup 关闭后隐藏 child 不得被 Enter 提交。
  - 覆盖开启宽度 motion 后连续两轮 inline collapsed 折叠/展开，验证任意深度的 selected item、祖先 selected path 和 cached inline open path 在模板重应用后完整恢复。
  - 覆盖连续折叠/展开后切换 sibling、视觉树 detach 后程序化切换 sibling，以及 node container clear 的完整选择状态复位。
  - 覆盖与 Gallery 相同的三级 collapsed popup 路径，验证当前 leaf 在两轮 popup 关闭重开和容器重新生成后持续保持唯一 selected 状态。
  - 覆盖 node/group 同 owner 与跨 owner 重复拒绝、释放后重挂载、root source 重复、跨根迁移、弱 owner 回收和 divider 复用。
  - 覆盖离线和根菜单中的 custom wrapper 共享内置后代、嵌套 observable source 动态获取/释放 owner、动态重复、动态 owner/祖先环拒绝、custom source weak subscription 回收、批量初始化原子失败、parent callback 与集合通知同步重入，以及纯 built-in 深树不产生祖先重复订阅。
  - Menu Gallery 增加结构化 NavMenu 示例，覆盖固定 Header/Footer、根与嵌套分组、分隔线和显式 `ItemSpacing`，并由页面结构测试与 approved snapshot 保护。

## 2026-07-13

- API
  - 新增 `NavMenuNode` / `INavMenuNode` 的 `Command`、`CommandParameter` 节点命令契约，明确节点只承载配置，实际执行和 `CanExecute` 生命周期由 `NavMenuItem` 负责。
  - `INavMenuNode` 的新增命令成员提供 `null` 默认实现，保持既有自定义节点实现兼容。
  - 明确 `CommandParameter` 不隐式回退到 `ItemKey`，业务 key 由调用方显式绑定或赋值。
- Implementation
  - 将 `NavMenuNode` 资源宿主设计收敛到 `[GenerateScopedResourceHost]`，由 generator 生成 `IResourceHost` / `IThemeVariantHost` 和可释放 attachment token。
  - scoped resource-host token 增加 host generation 身份，避免 `A -> B -> A` 重入时旧 token 提前释放当前 owner。
  - 通过 container `CompositeDisposable` 将节点命令投影到当前 `NavMenuItem`，并在 container clear、recycle 和 source replacement 时同时释放命令 binding 与 resource-host attachment。
  - `NavMenuItem` 在 UI Dispatcher 上按周期合并 `CanExecuteChanged`，避免同步 `ReactiveCommand` 的瞬时 `false -> true` 让共享命令叶子触发 disabled 颜色闪动；pending operation 在 command / parameter 替换和 detach 时取消。
- Tests
  - 覆盖 pointer / keyboard 单次执行、`CanExecute`、同步 `ReactiveCommand` 瞬时状态合并、命令替换、container clear、ItemsSource replacement、custom node、owner resource 更新、repeated attach 和 WeakReference 回收。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `NavMenu`.
  - Align generated output paths with `controls/nav-menu/index-cn.md` and `controls/nav-menu/semantic-cn.md`.

## 2026-06-23

- Docs
  - 补充 NavMenu inline collapsed 设计模型，明确 public `Mode` 保持 `Inline`，内部通过 effective mode 切换到 vertical popup 交互。
  - 在 `overview.md` 和 `implementation.md` 中记录 inline open path cache、折叠期间 popup 临时状态、展开恢复和 selected path 保持规则。
  - 在 `overview.md` 中补充 NavMenu 键盘导航设计，明确 keyboard active/focus、open path 和 `SelectedItem` 三类状态分离。
  - 在 `implementation.md` 中补充 keyboard navigation coordinator 的职责边界、按键语义、active 视觉、popup 分支关闭和生命周期失效规则。
  - 明确 keyboard active 初始解析优先使用当前可见 `SelectedItem` 容器作为方向键移动锚点，无选中项或选中项不可见时回退到第一个可导航节点。
- API
  - 记录 `IsInlineCollapsed` 和 `InlineCollapsedWidth` 的 NavMenu 契约：折叠不改写 public `Mode`，本地 `InlineCollapsedWidth` 覆盖 token 默认宽度。
  - 新增 `IsInlineCollapsed` 和 `InlineCollapsedWidth` 公共属性，并在 Gallery API 表中登记。
  - 明确键盘 active 项不作为公共 API 暴露，业务侧仍通过 `SelectedItem`、`NavMenuItemClick` 和 `NavMenuNodeSelected` 获取已提交选择。
- Theme
  - 明确 inline collapsed header 视觉由 theme 表达：顶层 icon 使用 `CollapsedIconSize` 居中，标题和箭头收起，无 icon 顶层项显示标题首字符；root 宽度由控件内部 coercion 约束。
  - 落地 inline collapsed 视觉状态：root 宽度使用 `InlineCollapsedWidth` 对 `Width` / `MinWidth` 做 effective value coercion，顶层 header 通过 AXAML selector 切换折叠视觉，popup 子项保持普通 vertical 视觉。
  - 为 inline collapsed 的根宽度变化补充内部 `InlineCollapsedLayoutWidth` motion，避免 root `Width` coercion 绕过 `DoubleTransition` 后产生瞬间跳变，并避免使用 animation-priority relay binding 或 `MaxWidth` 夹宽度。
  - 明确 keyboard active 视觉使用 `ItemActiveBg` 语义，且优先级低于 selected，不复用 `IsSelected` 或 `IsInSelectedPath`；`IsInSelectedPath` 不应屏蔽 active 背景。
- Token
  - 记录 `NavMenuToken.InlineCollapsedWidth` 作为 inline collapsed 宽度默认值，初始设计值为 `48`。
  - 新增 `NavMenuToken.InlineCollapsedWidth`，并在 Gallery Token 表中登记。
  - 明确 `CollapsedWidth` 保留兼容，新的 inline collapsed 宽度语义优先使用 `InlineCollapsedWidth`。
- Implementation
  - 新增 internal `EffectiveMode` 和 inline collapsed open path cache，使 `Mode=Inline && IsInlineCollapsed=true` 复用 vertical popup 交互，同时保留 public `Mode`、`SelectedItem` 和 selected path。

## 2026-06-19

- Docs
  - 新增 `implementation.md`，记录 NavMenu 容器绑定、interaction handler、selection coordinator、默认路径 replay、popup 和背景块实现边界。
  - 将 `overview.md` 收敛为模式语义、公共契约、行为状态、视觉主题模型和验证入口。
  - 在 Navigation 分类入口中登记 NavMenu 实现原理文档。
  - 按控件文档规范补齐 NavMenu 桌面版架构设计、Token 设计和控件级 changelog。
  - 在 Navigation 分类入口中登记 NavMenu 文档。
- Theme
  - 记录 `IsItemBackgroundEnabled` 对 item 背景块和 inline submenu 背景块的控制边界。
  - 明确 root background、popup background、header background 和 inline submenu background 的职责分离。
- Token
  - 记录 NavMenuToken 的颜色、间距、popup、horizontal、dark 和 danger 分类。
  - 明确 `VerticalChildItemsMargin` 只服务背景模式下的 inline submenu 背景块外距。
