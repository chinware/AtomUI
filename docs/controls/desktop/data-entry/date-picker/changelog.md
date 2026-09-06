# DatePicker Changelog

本文档记录 DatePicker 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-09-06

- Architecture
  - Publish the DatePicker family Semantic Part contract with two owners: `DatePicker` (twelve parts) and `RangeDatePicker` (thirteen parts, adding `secondaryInput`), aligned with the Ant Design `DatePicker` / `RangePicker` semantic structure; see `DatePicker.SemanticParts.cs`, `RangeDatePicker.SemanticParts.cs` and `semantic-part.md`.
  - Anchor trigger-part markers in the shared `InfoPickerInputTheme.axaml` (single owner) and the `RangeDatePickerTheme.axaml` host template (range owner), reusing the `AddOnDecoratedBoxTheme` scope anchors for `prefix` / `suffix` and adding the `semantic-prefix` projection presenter for `ContentLeftAddOn`.
  - Resolve `clear` through the shared `PickerClearUpButtonTheme.axaml` template (`CrossNestedOwners=true`); the marker stays inert for the TimePicker family until its own contract lands.
  - Carry the popup part markers on the runtime-assembled presenter subtree: `popup.container` / `popup.footer` on the three presenter theme templates, `popup.header` / `popup.body` / `popup.content` on the single-month and dual-month CalendarItem themes, and `popup.cell` injected in the `CalendarDayButton` constructor so month-grid rebuilds and container recycling keep the marker.
- API
  - Make `InfoPickerInput.IsPopupPinnedOpen` and `IsPickerOpen` public (matching the Select family `IsDropDownOpen` precedent) so Semantic Parts previews can open and pin the picker popup.
  - Align the InfoPickerInput pinned-open overlay suppression with the Select family: the light-dismiss flag is assigned as a local value before the popup opens (Avalonia reads it only at open time), and the pinned popup no longer keeps an `OverlayInputPassThroughElement`, so a lingering overlay can no longer make everything outside the input box inert. The same alignment is applied to Mentions and AbstractColorPicker.
  - Relay the owner root `BorderBrush` onto `AddOnDecoratedBox` as a `LocalValue` (unset value clears the relay and restores the shared state machine), mirroring the NumericUpDown root-border relay so root-level Semantic customization can recolor the trigger frame.
- Theme
  - Make the shared `ArrowDecoratedBoxTheme` content decorator consume `BorderBrush` / `BorderThickness` with a zero default thickness, so the popup-root Semantic Part can paint the popup frame border while every built-in popup (Date/Time pickers, ColorPicker, ToolTip, flyouts, DataGrid filter flyouts) keeps its borderless visual.
  - Add the `:bordered` pseudo-class to `AbstractArrowDecoratedBox` (set when `BorderThickness` is non-default) and hide the floating arrow through the ControlTheme while bordered: the built-in visuals cannot fuse the arrow with a custom border, so a bordered popup renders as a clean panel.
  - Cap `PART_InfoInputBox` with a per-size-type `MaxHeight` (`FontHeightLG` / `FontHeight` / `FontHeightSM`, `Custom` stays natural) so Semantic Style setters on the inputs participate in natural measurement without breaking the AddOnDecoratedBox size-type height baseline.
- Gallery
  - Add the DatePicker Semantic Parts tab with a pinned-open range preview (dual month, both inputs and prefix) and localized part descriptions; migrate the showcase host to `GalleryShowCaseHost` per the standard Semantic Part page model. The preview stage asks for a 560px floor (`PreviewStageMinHeight`) with a top-aligned anchor so the tall dual-month popup opens downwards instead of flipping over the page tabs.
  - Fix the shared `SemanticPartPreviewLayoutPanel` height contract: the template content row now passes the host height clamp through to the panel, the stage floor moves into the panel (`PreviewStageMinHeight`, shrinkable under the clamp), so a tall preview can no longer push the parts pane past the visible viewport and the part list can always scroll to its last item.
  - Add the "Custom Semantic Part styling" example with version-gated badge (`GalleryVersionInfo.DisplayVersion`), covering single-picker input italic / prefix / suffix / popup-root styles and range-picker root border / input / secondary-input / cell / popup-footer / popup-root styles; replace the stale hardcoded example badges.
- Tests
  - Add `DatePickerSemanticPartTests` covering descriptor shape for both owners, host / shared / runtime-assembly marker inventories, default-theme non-consumption, generated style hits for both owners, popup marker exposure (single, dual-month, timed single-month), cell marker survival across rebuild and reopen, footer visibility marker stability, and the Small-size semantic padding height-baseline regression.
  - Update `DatePickerShowCasePageTests`, `DatePickerShowCaseExamples.snapshot` and the catalog order / coverage baselines for the migrated host, semantic preview template, added example and new localization entries.

## 2026-09-04

- Behavior
  - Apply the shared InfoPickerInput pinned light-dismiss suppression before the first DatePicker or RangeDatePicker Popup open and restore the template default after unpinning.
- Tests
  - Cover a DatePicker pin request made before template materialization, including physical open state and unpin restoration.

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record InfoPickerInput as the semantic owner for DatePicker.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-23

- Architecture
  - Align DatePicker and RangeDatePicker input surfaces with the shared `AbstractTextInput` / `InputControlFrame` model.
  - Keep picker and calendar state in DatePicker while projecting native validation, FormStatus and explicit status through the shared frame.

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
