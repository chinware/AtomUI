# Calendar Changelog

本文档记录 Calendar 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-02

- Fixed
  - `Fullscreen=false` 改为无内部外框的卡片内容布局，`MiniContentHeight` 约束包含 WeekHeader 与六行日期的完整 CalendarView。
  - 默认 Header 的 Month/Year 切换改用 `OptionButtonGroup`，并对齐 Header、body 分隔线与纵向 Padding。
  - Header 控件组在卡片容器中保持右对齐；Year 模式月份 Cell 使用 `YearMonthCellWidth` 铺开选中背景，匹配 Ant Design 面板布局。
  - Mini 日期选中态改为主色实心与浅色文本，today、outside、disabled 和星期标题对齐各自的 SharedToken 状态语义；Fullscreen 选中态保持独立规则。
- Gallery
  - 卡片示例使用外部 300 宽边框容器，容器在 Showcase 内容区左对齐，并同步 Card 标题与三种语言说明文案。

## 2026-08-03

- Fixed
  - 默认 Header 年/月选择器的下拉列表视口按 Ant Design Select 的 256 高度（8 个 32 高选项）配置，弹层内边距保持由 ComboBox Token 控制。

## 2026-07-31

- Fixed
  - 将 Year 面板修正为 3×4，并将纵向键盘步长修正为 3 个月。
  - 周序号 Cell 现在继承行首禁用状态，可选择行首日期并报告 `Date` 来源。
  - 修复 `DateTime.MinValue/MaxValue` 附近的网格、焦点与 Header 年份选项溢出。
  - 模板替换、模式收缩和 VisualTree detach 统一释放池化 Cell 的 owner、model、context 与模板引用。
  - WeekHeader 改为与 CellHost 同列的 Grid；CellTemplate 使用独立内容行；Fullscreen/Mini 使用不同根布局、Cell 视觉和可见焦点描边。
  - CalendarView Automation 补齐 `ISelectionProvider`，Cell 返回 SelectionContainer 并使用完整本地化名称。
- API
  - `Value` 与 `Mode` 默认绑定模式改为 `TwoWay`。
  - Calendar 文案和日期文化跟随 AtomUI 全局语言服务。
- Gallery
  - Fullscreen 示例改用真实宽布局，HeaderTemplate 绑定 `CalendarHeaderContext.Value`，事件示例移除 Loaded 匿名订阅。

- Docs
  - 将 Calendar 行为设计归档到 `docs/controls/desktop/data-display/calendar/`，并同步 overview、implementation、Token 与分类入口的导航。
  - 明确 CellTemplate/FullCellTemplate 优先级、月份首尾禁用、周序号选择、方向键跳过禁用 Cell、语言资源和 Template 生命周期契约。
  - 将 Automation 文档修正为 Avalonia 可移植的 `Table`/`ListItem` 与 SelectionItem 契约，不再宣称跨平台 Grid/GridItem provider。
  - 新增范围条专项设计，定义 `RangeBars` / `CalendarRangeBar` 契约、Calendar body overlay 绘制层、可见行坐标算法、非 Visual 资源宿主生命周期和验证边界。
  - Calendar 专属 Token 增加 `RangeBarHeight`，作为范围条 overlay 的默认条高；单条范围条仍通过实例属性覆盖颜色和高度。

## 2026-07-30

- Breaking / Rewrite
  - 按 Calendar 当前行为规则彻底重写控件，抛弃旧 WPF/Avalonia Calendar 移植体系（`BaseCalendarButton`、`CalendarDayButton`、`SelectedDatesCollection`、`BlackoutDates`、Decade 面板等全部删除）。
  - 全新 Public API：`Value`/`Mode`/`Fullscreen`/`ShowWeek`/`ValidRange`/`DisabledDate`/`CellTemplate`/`FullCellTemplate`/`HeaderTemplate` + `ValueChanged`/`Selected`/`PanelChanged` 事件。
  - `CalendarMode` 收敛为 `Month`/`Year`；`CalendarDateRange` 收敛为只服务 ValidRange（`end<start` 抛异常）。
  - 内部新增纯面板 `CalendarView`（命名空间 `AtomUI.Desktop.Controls.Internal.Calendar`），与 DatePicker 的 CalendarView 子系统完全隔离。
  - Token 收敛为独立的 `CalendarControlToken` 语义（`FullBg`/`FullPanelBg`/`ItemActiveBg`/`YearControlWidth`/`MonthControlWidth`/`MiniContentHeight`/`FullCellMinHeight`），旧 `CalendarToken` 保留给 DatePicker。
  - 事件顺序固定为 `PanelChanged -> ValueChanged -> Selected`；程序设值不触发用户事件。
  - 新增 roving focus、方向键导航和基于 Avalonia `Table`/`ListItem`/`ISelectionItemProvider` 的 Automation 契约。
  - Gallery 重写为九个示例，控件文档与 Token 文档同步更新。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Calendar`。
  - Align generated output paths with `controls/calendar/index-cn.md` and `controls/calendar/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Calendar desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Calendar desktop architecture documentation under `docs/controls/desktop/data-display/calendar/overview.md`.
  - Add Calendar implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Calendar control-level changelog.
  - Add Calendar Token documentation covering `CalendarControlToken`。
