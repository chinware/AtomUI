# TimePicker Changelog

本文档记录 TimePicker 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-23

- Architecture
  - Align TimePicker and RangeTimePicker input surfaces with the shared `AbstractTextInput` / `InputControlFrame` model.
  - Keep time-panel state in TimePicker while projecting native validation, FormStatus and explicit status through the shared frame.

## 2026-08-03

- Implementation
  - Use the shared internal `InfoPickerTextBox` for TimePicker input visuals, keeping padding and text-presenter spacing in the child theme instead of crossing the child template from TimePicker themes.

## 2026-07-16

- Changed
  - Make TimePicker and RangeTimePicker preferred input width ignore user placeholder text and reserve the larger value between the widest formatted value text and the Ant Design TimePicker locale baseline (`Select time`, or `Start time` / `End time` for ranges).
  - Render long TextBox placeholder text with character ellipsis inside the reserved input area.
- Docs
  - Clarify that TimePicker and RangeTimePicker default input width follows Ant Design's `picker === 'time'` locale placeholder branch, not DatePicker's `Select quarter` baseline.
  - Document that long placeholder text is ellipsized inside the reserved input area and must not resize the control.

## 2026-07-05

- API
  - Make `TimePicker.SelectedTime` bind `TwoWay` by default while keeping Avalonia data validation enabled.
- Gallery
  - Add a `v6.0.8` `SelectedTime` binding example that shows ViewModel synchronization without explicit `Mode=TwoWay`.
- Docs
  - Document `SelectedTime` as the controlled Form value and native validation projection point.

## 2026-07-04

- API
  - Add `PickerDisplayTime` as the popup panel display anchor for `TimePicker`, separate from `SelectedTime` and `DefaultTime`.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `TimePicker`.
  - Align generated output paths with `controls/time-picker/index-cn.md` and `controls/time-picker/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete TimePicker desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish TimePicker desktop architecture documentation under `docs/controls/desktop/data-entry/time-picker/overview.md`.
  - Add TimePicker implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add TimePicker control-level changelog.
  - Add TimePicker Token documentation covering TimePickerToken.
