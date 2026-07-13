# NavMenu Changelog

本文档记录 NavMenu 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

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
