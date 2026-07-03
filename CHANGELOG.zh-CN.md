# 更新日志

AtomUI 的重要变更记录在此文件中。

`AtomUI` 遵循 Semantic Versioning 2.0.0。

英文版本见 [CHANGELOG.md](CHANGELOG.md)。

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
