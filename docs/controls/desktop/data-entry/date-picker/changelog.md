# DatePicker Changelog

本文档记录 DatePicker 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-03

- Implementation
  - Use the shared internal `InfoPickerTextBox` for DatePicker input visuals, keeping padding and text-presenter spacing in the child theme instead of crossing the child template from DatePicker themes.

## 2026-07-30

- Docs
  - Define the inclusive `MinDate` / `MaxDate` contract shared by DatePicker and RangeDatePicker, including PickerMode normalization and invalid-boundary convergence.
  - Define presenter-owned range synchronization so Calendar bounds are applied before valid selected values and out-of-range controlled values are not written back.
  - Specify visible disabled cells, bounded panel navigation, Today/Now/Confirm guards, dual-panel range behavior, and reuse of existing disabled-state tokens.

## 2026-07-16

- Changed
  - Make DatePicker and RangeDatePicker preferred input width ignore placeholder text and reserve from a stable formatted value baseline.
  - Keep default DatePicker and RangeDatePicker width at an Ant Design style input baseline for Date, Week, Month, Quarter and Year modes when `Format` is not set.
  - Render long TextBox placeholder text with character ellipsis inside the reserved input area.
- Docs
  - Clarify that DatePicker and RangeDatePicker default input width is reserved from a formatted value baseline, not from placeholder text.
  - Document that non-Date picker modes keep a stable Ant Design style input baseline unless an explicit `Format` is provided.
  - Document that long placeholder text is ellipsized inside the reserved input area and must not resize the control.

## 2026-07-05

- API
  - Make `DatePicker.SelectedDateTime` default to `BindingMode.TwoWay` while preserving Avalonia data validation support.
- Gallery
  - Add a `v6.0.8` `SelectedDateTime` binding example showing ViewModel synchronization without explicit `Mode=TwoWay`.
- Docs
  - Document `SelectedDateTime` as the single-value controlled Form value contract.

## 2026-07-04

- Docs
  - Document the `PickerDisplayDate` panel display anchor contract for keeping committed values, default selected values and popup display position separate.
  - Clarify that internal CalendarView `DisplayDate` state is not the outer DatePicker selected value contract.

## 2026-07-02

- API
  - Add `DatePickerMode` and `PickerMode` to `DatePicker` and `RangeDatePicker` for Date, Week, Month, Quarter and Year selection granularity.
- Changed
  - Propagate picker granularity through presenter templates into CalendarView state, panel model generation and selection normalization.
  - Add a dedicated quarter panel model rendering Q1-Q4 instead of using the month panel as a substitute.
  - Render `PickerMode=Quarter` with a compact one-row quarter panel instead of inheriting the 12-slot year panel height.
  - Render `PickerMode=Week` as an 8-column week panel with an ISO week number column and continuous selected-week and hover-week row highlights.
  - Render range committed and hover preview states for Month, Quarter and Year picker cells with the same endpoint and middle-cell model as Date mode.
  - Route Month, Quarter and Year panel hover through `Calendar.NotifyHoverDateChanged` so range inputs and popup preview update from the same CalendarButton interaction.
  - Keep `RangeDatePicker` popup panels dual-pane for Week, Month, Quarter and Year picker modes.
  - Normalize non-date committed values to week start, month start, quarter start or year start while keeping `DateTime?` as the value contract.
  - Make time selection effective only for `PickerMode=Date`.
  - Reserve input width by picker granularity so selected and hover text do not resize the input.
- Gallery
  - Update the basic DatePicker showcase to display Date, Week, Month, Quarter and Year modes.
- Docs
  - Document the PickerMode granularity model and CalendarView target panel mapping.

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
