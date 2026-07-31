# Calendar Changelog

本文档记录 Calendar 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

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

## 2026-07-30

- Breaking / Rewrite
  - 按 Calendar 当前行为规则彻底重写控件，抛弃旧 WPF/Avalonia Calendar 移植体系（`BaseCalendarButton`、`CalendarDayButton`、`SelectedDatesCollection`、`BlackoutDates`、Decade 面板等全部删除）。
  - 全新 Public API：`Value`/`Mode`/`Fullscreen`/`ShowWeek`/`ValidRange`/`DisabledDate`/`CellTemplate`/`FullCellTemplate`/`HeaderTemplate` + `ValueChanged`/`Selected`/`PanelChanged` 事件。
  - `CalendarMode` 收敛为 `Month`/`Year`；`CalendarDateRange` 收敛为只服务 ValidRange（`end<start` 抛异常）。
  - 内部新增纯面板 `CalendarView`（命名空间 `AtomUI.Desktop.Controls.Internal.Calendar`），与 DatePicker 的 CalendarView 子系统完全隔离。
  - Token 收敛为七个语义（`FullBg`/`FullPanelBg`/`ItemActiveBg`/`YearControlWidth`/`MonthControlWidth`/`MiniContentHeight`/`FullCellMinHeight`），使用独立的 `CalendarControlToken`（scope id `CalendarControl`）；旧 `CalendarToken` 保留给 DatePicker。
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
