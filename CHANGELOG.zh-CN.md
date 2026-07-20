# 更新日志

AtomUI 的重要变更记录在此文件中。

`AtomUI` 遵循 Semantic Versioning 2.0.0。

英文版本见 [CHANGELOG.md](CHANGELOG.md)。

## 6.1.0

`2026-07-20`

- 破坏性变更
  - Dialog 和 MessageBox：围绕异步 Session 重建静态展示和宿主生命周期。直接引用 `IDialogHost`、`IDialogHostProvider`、`IDialogActionResult` 或 `IMessageBoxActionResult`，或依赖 callback 式宿主关闭 API 的自定义代码，应迁移到 `ShowDialogAsync`、`ShowDialogModalAsync`、`ShowMessageBoxAsync`、`ShowMessageBoxModalAsync`、`Dialog.OpenAsync`、`Dialog.BeforeCloseAsync` 和返回的 `Task<object?>`。迁移示例见 [6.1.0 API 变更示例](docs/release-notes/6.1.0-api-changes.md)。
  - Steps：将基于 Selection 的契约替换为受控步骤状态。请将 `CurrentStep` 迁移到 `Current`，`InitialStep` 迁移到 `Initial`，`CurrentStepStatus` / `StepsItemStatus` 迁移到 `Status` / `StepsStatus`，`Style` / `ItemIndicatorType` 迁移到 `Type`，`LabelPlacement` 迁移到 `TitlePlacement`，`ProgressValue` / `IsShowItemProgress` 迁移到可空 `Percent`；步骤项描述从 `Description` / `DescriptionTemplate` 迁移到 `Content` / `ContentTemplate`，可点击步骤变更改为处理 `CurrentChangeRequested`，不再依赖 Selection 直接变更。迁移示例见 [6.1.0 API 变更示例](docs/release-notes/6.1.0-api-changes.md)。
- Theme
  - 新增基于 resolver 的主题定义加载能力，支持内置资源、应用资源和显式启用的用户主题目录，并统一使用 XML 主题定义格式。
  - 新增 `IThemeDefinitionResolver`、`IThemeManager.AvailableThemes`、`IThemeManager.CurrentTheme`、`ThemeCatalogDiagnostics`、`ThemeCatalogChanged` 和 `ReloadThemesAsync`，应用可以发现、切换并原子刷新主题 Catalog。
  - 新增 `WithApplicationId`、`UseUserThemeDirectory()` 和 `UseUserThemeDirectory(string directory)`；显式启用时，默认用户主题目录为应用数据目录下的 `{ApplicationId}/Themes`。
  - 重建主题解析、绑定、Catalog 编译、Snapshot 缓存和 Token 资源发布流程，让根主题和局部主题都发布完整 Snapshot，并在解析或编译失败时保留上一版状态。
  - 生成 AXAML 共享控件 Token 资源元数据，并隔离控件 Token 作用域，使 primary、link 和 text button 状态随当前主题稳定变化。
- Gallery 主题
  - 新增 Daybreak Blue、Polar Green、Sunset Orange、Golden Purple 和 Magenta 等内置主题定义，并通过带色块的主题设置子菜单暴露。
  - Gallery 通过 Theme manager 完成主题切换，并为桌面 Gallery 启用用户主题目录加载。
- Window 和 Native
  - 将 Avalonia 依赖升级到 `12.1.0`，并新增原生 Wayland 支持。
  - 稳定 Windows 和 Linux 绘制装饰，包括 Windows 10 CSD frame 渲染、深色主题标题栏、caption 按钮、实时 resize、Wayland 圆角 CSD 裁剪和 Linux client-frame 视觉层。
  - 为 Dialog 和 Drawer overlay 分离完整窗口 mask、可见 frame bounds 和阴影范围，让 mask 覆盖绘制标题栏，同时 placement 保持在可见 frame 内。
- Dialog、Drawer 和 MessageBox
  - 将 Dialog 和 MessageBox 统一到同一套 Session、Presenter 和 Surface 生命周期，覆盖 UI Dispatcher 构造、异步打开/关闭、关闭拦截、owner close 和确定性 teardown。
  - 优化 overlay 与 window presenter 的尺寸、拖动、resize、最大化、mask 路由、焦点恢复、取消和重入行为。
  - 降低 overlay dialog transition 开销。
- 导航和选择控件
  - Steps：新增 `OutlineDot`、受控导航请求事件、确定性的步骤编号/status/connector 语义、pointer click wave 行为，以及用于水平、垂直和导航布局的新 layout panel。
  - Cascader：支持键盘候选导航，并稳定展开、过滤、选中 option 同步和 popup 状态。
  - Menu 和 NavMenu：新增子菜单 hover intent，支持 detached title bar 中的 popup，新增生命周期安全的节点命令，并修复 pointer 或布局变化导致的子菜单状态跳变。
  - Collapse 和 Expander：优化 separator、accordion、内容可见性和 motion cleanup 行为。
- 数据录入、数据显示和通用控件
  - 为 CascaderView、ListView 和 ListBox 新增自定义 EmptyIndicator 支持。
  - 为 TransferItemDecorator 新增 list 和 tree transfer 视图的 item count 处理。
  - 修复 AddOnDecoratedBox 仅模板提供 add-on 内容、模板拥有的 EmbeddedTextBox 输入框、embedded 输入 padding、DatePicker 和 TimePicker 输入尺寸，以及 NumericUpDown 默认输入 padding 覆盖。
  - 移除 ColorPicker spectrum 颜色名称 tooltip，并稳定 ImagePreviewer 搜索和主题资源。
  - 修复 Row/Splitter 布局恢复和精确 grid line 换行问题。
- 反馈和浮层控件
  - Notification：普通通知默认不显示 type icon，对齐关闭按钮 hover/pressed 状态，使用 primary 色生成进度条渐变，将默认自动关闭时长设为 4.5 秒，并缩小内部卡片间距。
  - Tour：修正目标 placement，并为高亮区域添加动画。
- Icons、Gallery、Generator 和文档
  - 新增面向 AI、应用和社交/服务场景的 SVG 图标。
  - 为 Gallery 新增 Windows 渲染 fallback，并刷新 Gallery baseline、生成文档和主题设计文档。
  - 新增 XML 主题绑定、Token 发布和可选控件包使用的生成式主题 schema 与控件 Token 资源元数据。
  - 将包元数据更新到 `6.1.0`。

## 6.0.8

`2026-07-10`

- 破坏性变更
  - ImagePreviewer：将 `SourceUri`、`SourceUris`、`FallbackSourceUri` 统一替换为基于 `IImagePreviewSource` 的 `Source`、`Sources`、`FallbackSource`；URI、本地文件和 Avalonia 资源请使用 `UriImagePreviewSource`，按需数据流请使用 `StreamImagePreviewSource`。迁移示例见 [6.0.8 API 变更示例](docs/release-notes/6.0.8-api-changes.md)。
  - Upload：移除旧的 `IsUploadDirectoryEnabled`、`IsShowUploadTrigger` 和 `DefaultTaskList` 组合方式，改为由 `UploadTrigger`、`UploadDropZone` 和 `Files` 组成可组合上传入口。迁移示例见 [6.0.8 API 变更示例](docs/release-notes/6.0.8-api-changes.md)。
- 数据录入和选择控件
  - 新增 OtpLineEdit 一次性验证码输入控件，支持 `Text` 双向绑定、`Length`、`InputMode`、`Formatter`、遮罩、分隔符、`Completed` 事件、四种 `StyleVariant`、Form 和 `DataValidationErrors` 集成。
  - 将 Form 验证统一接入 Avalonia `DataValidationErrors`，并新增 `ValidateTrigger`；默认改为值变更时验证，也可配置为失焦触发。
  - 为 Select、Cascader、TreeSelect、ListView、CheckBoxGroup、RadioButtonGroup、Rate、DatePicker、TimePicker、Transfer、ColorPicker、Slider、Dialog、Tour、ImagePreviewer 等控件补齐或调整默认双向绑定与数据验证语义。
  - 为 Select、ComboBox 和 Cascader 新增溢出内容提示能力，支持 `IsShowOverflowTip`、`OverflowTipDelay` 和 `OverflowTipPlacement`。
  - 为 DatePicker 和 TimePicker 新增弹层展示锚点属性，并补充范围选择绑定示例。
- Upload
  - 重构 Upload 为文件状态协调器，新增 `UploadFileItem`、`UploadTrigger`、`UploadDropZone`、`UploadFileValueMode`、`AutoUpload`、`ListMaxHeight`、`ListScrollBarVisibility`、`SuccessAutoRemoveDelay`、`PendingText` 和 `TriggerContent` 等能力。
  - 优化文件选择、目录选择、拖拽上传、上传列表滚动、图片列表预览、删除状态和成功自动移除行为。
- ImagePreviewer
  - 新增基于 `IImagePreviewSource` 的 URI 与懒加载数据流图片源模型，支持 `CoverIndex`、`MaxConcurrentLoads` 和 `PreloadCount`。
  - 修复多图加载中单张失败会过早切换 fallback 的问题；现在只有整组图片全部失败时才使用 `FallbackSource`。
  - 优化多图预览标题、封面加载、预览窗口导航图标和 20 张远程图片示例。
- DataGrid
  - 新增列过滤数据模型，支持 `Filters`、`SelectedFilterValues`、`FilterTextMemberPath`、`FilterValueMemberPath`、`FilterChildrenMemberPath`、`FilterPresenterMode`、`FilterSelectionMode` 和 `FilterApplyMode`。
  - 优化过滤选择、树形过滤、筛选值绑定和 Gallery 示例。
- TabControl 和 TabStrip
  - 新增 Tab 拖动排序能力，支持 `IsTabReorderEnabled`、`TabActivationTrigger`、`TabReordering` 和 `TabReordered`。
  - 优化 Chrome 风格拖动预览、滚动时锚点保持、选中指示条同步、左右 placement 下无图标 Tab 的布局和默认 Line Tab 垂直紧凑间距。
- TreeView、Cascader 和 NavMenu
  - 修复 TreeView `ItemsSource` 拖拽移动崩溃，并改进拖拽、选中项双向绑定、表单值和 descendants bring-into-view 行为。
  - 修复 Cascader 选择同步和展开状态崩溃问题，并补齐 `SelectedOption` / `SelectedOptions` 双向绑定。
  - 新增 NavMenu popup frame，改进弹出层宿主、定位和关闭行为。
- Dialog、FloatButton 和基础视觉
  - 为 Dialog 新增 `BeforeCloseAsync`，支持异步关闭校验，并统一关闭请求处理。
  - 为 FloatButton 新增命令支持，并优化滚动容器场景下的浮动层定位和 controlled `IsOpen` 行为。
  - 优化 Avatar 图片裁剪、控件边框按布局缩放渲染、Button 边框渲染和 Space 预设间距。
- Gallery、文档和构建
  - 扁平化标准 Showcase 示例结构，补充 Tab 拖动、Upload、ImagePreviewer、OtpLineEdit、选择绑定和范围选择示例。
  - 更新控件设计文档、Window 标题栏定制说明、Feature Request issue 规范、README 包版本和 Gallery banner。
  - 清理多处未使用 localization using，移除 Labs 包并更新工程文档。

## 6.0.7

`2026-07-03`

- DatePicker
  - 新增日期、周、月、季度和年份等选择模式，并优化范围面板行为和输入框首选宽度计算。
  - 优化范围选择状态、hover 渲染和 CalendarView 生命周期处理。
- DataGrid
  - 优化行详情高度估算和展开行虚拟化稳定性。
  - 修复空或 null `ItemsSource` 场景下点击表头/标题按钮导致崩溃的问题。
  - 优化选择列初始状态处理，以及带边框表格的外框圆角行为。
  - 修复 DataGrid 模板单元格中的 AutoComplete 焦点保持问题。
- Select
  - 修复 Tags 模式动态选项创建会修改用户 `OptionsSource` 的问题。
  - 优化 Tags 键盘交互：Enter 提交当前动态候选项，Escape 关闭弹层，Up/Down 可从搜索输入框导航候选项。
  - 修复多选和 Tags 在 IME 预编辑文本渲染期间 placeholder 显示不正确的问题。
- ImagePreviewer
  - 新增通过 `ImageSourceUri` 加载本地和远程图片源的能力。
  - 新增预览窗口标题解析、`PreviewTitleIcon`、加载 skeleton 和本地化错误状态。
- Gallery 和文档
  - 新增可复用的 Gallery 源码展示组件、代码查看器懒加载、可复制代码选区和生成式代码片段目录支持。
  - 新增 Gallery API 表格和 Design Token 表格文本可选中能力。
  - 新增 LLMS 文档生成，并刷新控件文档覆盖范围。
  - 新增组织介绍文档页面，并将 README 包示例更新到 `6.0.7`。
- Splash 和 Window
  - 新增 Splash 控件和服务，支持 owner window、Gallery 版本标签和更稳定的显示逻辑。
  - 新增 Windows 窗口 chrome 管理，支持 DWM 阴影处理并优化 resize 伪影。
- 控件
  - 新增 Extras 和 Labs 补充控件模块。
  - 新增 Splitter 分隔线样式 API 和 Gallery 示例覆盖。
  - 优化 NumberUpDown handle 定制和模式行为。
  - 优化 Button 阴影渲染、SplitButton resize 布局稳定性、Drawer 右键行为、Message 和 Notification 反馈层级、弹出层阴影以及 Separator 紧凑间距。
  - 优化 Form validator 兼容性，并新增 `ExtraExtraExtraLarge` 断点布局处理。
- Localization 和 Build
  - 更新语言 provider 生成方式，并为 GalleryBase 新增中文语言支持。
  - 新增 AppImage launcher 验证支持，并更新 Gallery 和 NuGet 包发布工作流。

## 6.0.6

`2026-06-24`

- 兼容性说明
  - 多个尺寸感知控件现在使用可定制尺寸模型，除内置尺寸类型外也支持 `Custom` 尺寸行为。直接依赖旧尺寸类型契约的项目应验证源码兼容性。
- 新增
  - 为 Button 系列、DropdownButton、NumericUpDown、Mentions、Select、TreeSelect、ToggleSwitch、SpinIndicator 以及相关尺寸感知控件新增可定制尺寸支持。
  - 新增 Button 图标位置配置能力。
  - 新增 Slider 桌面控件、文档和 Gallery 示例。
  - 新增 `Col.Flex` 支持，并优化 Row/Grid 响应式布局行为。
  - 新增 ComboBox 可编辑过滤、候选项键盘导航、Enter/Escape 行为和空结果反馈。
  - 新增 NavMenu 键盘导航、激活项反馈和内联折叠模式。
  - 新增初始主题算法配置，让应用首帧即可按深色主题渲染。
  - 新增 TreeView/Cascader 可绑定节点或选项支持，覆盖面向绑定的数据场景。
  - 新增可复用的 GalleryBase toolkit 包，并集成 Gallery shell 和路由基础能力。
- 变更
  - 优化 Button、SplitButton、LineEdit、SearchEdit、DatePicker、TimePicker、ColorPicker、Select、TreeSelect 等控件的自定义尺寸布局和 Gallery 示例。
  - 优化 Form 校验流程，避免提交型校验过早显示错误。
  - 优化 Collapse 手风琴状态处理、padding 计算和内容可见性行为。
  - 优化 Expander 指示器间距、内容可见性和布局行为。
  - 优化 Descriptions、Segmented、Steps、ProgressBar、Skeleton、Card、AvatarGroup、Transfer、Upload、Pagination 和 Breadcrumb 的实现结构和正确性。
  - 优化 SpinIndicator 动画处理、默认对齐和可定制尺寸渲染。
  - 优化 RibbonBadge 定位和 adorner 可见性行为。
  - 优化 CompactSpace 内部协作和布局处理。
  - 将 Avalonia 依赖从 `12.0.4` 升级到 `12.0.5`。
- 修复
  - 修复 DatePicker 和 TimePicker 输入框首选宽度计算问题。
  - 修复 ListBox/ListView 过滤项状态处理问题。
  - 修复 NavMenu 在内联折叠模式下的选择保持问题。
  - 修复 Gallery AppImage installer 资源路径问题。
  - 修复 Gallery macOS DMG 工作流，避免使用未使用的 Homebrew taps。
  - 修复 Breadcrumb 生成项状态清理和 `IBreadcrumbItemData.Content` 映射问题。
- 文档
  - 新增或补全文档覆盖：LineEdit、SearchEdit、ListView、NumericUpDown、Select、Slider、ToggleSwitch、TreeSelect、Descriptions、Expander、Segmented、ProgressBar、Card、Cascader、Form 等桌面控件。
  - 新增控件优化 skill 文档，包含 API 布局、文件拆分、生命周期和根因优化规则。
  - 更新 Gallery 示例，覆盖自定义尺寸、键盘导航和新文档控件场景。

## 6.0.5

`2026-06-19`

- 破坏性变更
  - 响应式布局 API 统一到 `AtomUI.Controls.Shared.MediaQuery`。
  - 新增共享的 `ResponsiveInt`、`ResponsiveDouble`、`ResponsiveGutter` 和 `ResponsiveValueMap` 模型。
  - 新增 `xxxl` 断点和对应的 `ScreenXXXL` token 支持。
  - Grid、Descriptions 和 Masonry 现在共享同一套 mobile-first 响应式解析和 fallback 规则。
  - 依赖旧控件级响应式解析的自定义代码，尤其是依赖 `DescriptionsMediaBreakInfo` 的代码，应迁移到共享响应式值类型。
- Gallery
  - 重构 Gallery shell 和 showcase 页面结构。
  - 新增 sticky showcase tabs、延迟加载 showcase、场景诊断和快照归一化。
  - 为多数已迁移 showcase 页面新增本地化 API 表格和 Design Token 表格。
  - 对齐 Browser Gallery 路由和导航到共享桌面 Gallery 组件。
  - 新增 Gallery 版本展示和 Community Telegram 区域。
- 新控件和布局
  - 新增 BorderBeam，包含几何、颜色 stop、token、Gallery、文档和测试。
  - 新增 Masonry，支持响应式列、gutter、加载 skeleton、图片 demo、Gallery 示例和布局测试。
  - 为 Grid、Descriptions 和 Masonry 新增共享响应式布局支持。
- 桌面控件
  - Button：新增 `Color`、`Variant` 和 `CustomBackground` 支持，包括渐变背景以及更新后的 Gallery/API 文档。
  - NumericUpDown：新增 inline spinner 模式并优化 handle 样式。
  - ButtonSpinner：优化禁用状态处理，并新增样式 variant 支持。
  - NavMenu：新增 `ClearSelection`、`IsItemBackgroundEnabled`，并优化选择协作、内联间距和背景行为。
  - TextArea：优化 resize 行为。
  - GroupBox：修复背景透明时边框不可见的问题。
  - Pagination：优化 page size 校验和自定义 page size 选项。
  - Dialog 和 Menu：防止 dialog close/open 流程重入，并优化 menu 关闭行为。
  - HyperLinkButton：让图标颜色跟随前景色。
- NativeAOT、诊断和 DataGrid
  - 强化 NativeAOT 场景下的生成式数据成员访问器。
  - 新增 AOT 数据成员路径 analyzer 和编译器诊断规范。
  - 修复 DataGrid 排序路径解析，并新增排序回归测试。
  - 修复 Gallery Design Token 表格空状态。
  - 为非 Visual `AvaloniaObject` 使用场景实现 scoped resource 和 theme host，避免动态资源滞留泄漏。
- Native Window 和平台稳定性
  - 优化 Linux 窗口初始化、缩放、frame extents、window chrome 管理、弹出层支持和标题栏菜单关闭行为。
  - 优化 macOS 标题栏 logo 可见性。
  - 优化 Windows Win32 composition 选项以提升性能和稳定性。
  - 修正 popup、dialog、title bar 和 Gallery 文本问题。
- Build 和 Release
  - 恢复桌面诊断，并稳定生成源码的换行。
  - 优化 NativeAOT Gallery 发布工作流和验证。
  - 更新 `PublishToLocal.ps1` 版本提取逻辑，改为使用 `AtomUIVersion`。
  - 更新 AtomUI 6.0 发布定位相关包元数据。
