# DatePicker Changelog

本文档记录 DatePicker 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-01

- Docs
  - Add CalendarView system optimization design covering one-way state flow, panel models, renderer ownership, generated container lifecycle and verification matrix.
  - Add CalendarView system optimization implementation plan with test-first development tasks and verification checkpoints.
  - Link CalendarView optimization design from DatePicker overview and implementation docs.
- Changed
  - Add the first CalendarView state/model/controller foundation and mirror Calendar/RangeCalendar property state into a normalized `CalendarViewState`.
  - Isolate CalendarItem pointer tracking behind an explicit lifecycle helper and remove fixed `Children[7]` month bounds dependency.
  - Rebuild CalendarView month, year and decade button rendering around panel models, renderer-owned visual updates and explicit generated container lifecycle.
  - Remove CalendarView range highlight grid scans and selection-range suppression side effects.
  - Align DatePicker and RangeDatePicker input width reservation with the effective date/time format so selected and hover preview text no longer resize the input.
  - Make range picker inner layout stretch to the reserved range width while preserving constrained shrink behavior.
  - Split CalendarView committed range state from hover preview state while keeping hover preview endpoints visually aligned with range endpoints.
  - Preserve the active range part before endpoint values reach the calendar so opening or selecting the end part no longer lets the start date rewind the displayed months.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `DatePicker`.
  - Align generated output paths with `controls/date-picker/index-cn.md` and `controls/date-picker/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete DatePicker desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish DatePicker desktop architecture documentation under `docs/controls/desktop/data-entry/date-picker/overview.md`.
  - Add DatePicker implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add DatePicker control-level changelog.
  - Add DatePicker Token documentation covering DatePickerToken.
