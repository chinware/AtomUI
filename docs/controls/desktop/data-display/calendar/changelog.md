# Calendar Changelog

本文档记录 Calendar 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-30

- Breaking / Rewrite
  - 按 Ant Design 6 Calendar 语义彻底重写 Calendar 控件，抛弃旧 WPF/Avalonia Calendar 移植体系（`BaseCalendarButton`、`CalendarDayButton`、`SelectedDatesCollection`、`BlackoutDates`、Decade 面板等全部删除）。
  - 全新 Public API：`Value`/`Mode`/`Fullscreen`/`ShowWeek`/`ValidRange`/`DisabledDate`/`CellTemplate`/`FullCellTemplate`/`HeaderTemplate` + `ValueChanged`/`Selected`/`PanelChanged` 事件。
  - `CalendarMode` 收敛为 `Month`/`Year`；`CalendarDateRange` 收敛为只服务 ValidRange（`end<start` 抛异常）。
  - 内部新增纯面板 `CalendarView`（命名空间 `AtomUI.Desktop.Controls.Internal.Calendar`），与 DatePicker 的 CalendarView 子系统完全隔离。
  - Token 收敛为六个语义（`FullBg`/`FullPanelBg`/`ItemActiveBg`/`YearControlWidth`/`MonthControlWidth`/`MiniContentHeight`），使用独立的 `CalendarControlToken`（scope id `CalendarControl`）；旧 `CalendarToken` 保留给 DatePicker。
  - 事件顺序固定为 `PanelChanged -> ValueChanged -> Selected`；程序设值不触发用户事件。
  - 新增 roving focus、方向键导航、Grid/GridItem/SelectionItem Automation。
  - Gallery 重写为九个示例，控件文档与 Token 文档同步更新。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Calendar`.
  - Align generated output paths with `controls/calendar/index-cn.md` and `controls/calendar/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Calendar desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Calendar desktop architecture documentation under `docs/controls/desktop/data-display/calendar/overview.md`.
  - Add Calendar implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Calendar control-level changelog.
  - Add Calendar Token documentation covering CalendarToken.
