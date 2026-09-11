# ComboBox Changelog

本文档记录 ComboBox 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record ComboBox as the semantic owner for ComboBox.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-23

- Behavior
  - Route Form validation through `FormStatus` and the shared input-frame effective-status pipeline without overwriting explicit `Status`.
- Architecture
  - Pair the external Form feedback status subscription with logical attach/detach so a reused ComboBox reconnects without requiring template reapplication.
- Theme
  - Forward `FormStatus`, focus-within and native validation from `ComboBox` to its decorated input frame.
- Docs
  - Link ComboBox to the shared input-control architecture and document the validation-state ownership boundary.

## 2026-08-19

- Behavior
  - Make pointer movement and keyboard navigation update the same internal active candidate without committing `SelectedItem`.
  - Keep pointer migration non-scrolling and make `Enter` commit the current visual candidate.
- Theme
  - Neutralize inherited pointer-over candidate styling and preserve selected-item visual precedence.
- Tests
  - Add mixed pointer/keyboard navigation and pointer-to-`Enter` commit regression coverage.

## 2026-08-18

- Behavior
  - Keep keyboard input on a non-editable ComboBox when reopening its popup so consecutive `Down` and `Enter` selections continue to update the selected item.

## 2026-07-05

- Behavior
  - Use `SelectedItem` as the ComboBox Form value for `SetFormValue`, `GetFormValue`, and `ClearFormValue`.
  - Add non-editable selected content overflow tooltip support through `IsShowOverflowTip`, `OverflowTipDelay` and `OverflowTipPlacement`.
  - Align non-editable selected content overflow tooltip against the outer `AddOnDecoratedBox` instead of the padded inner presenter.
- Gallery
  - Add a `v6.0.8` `SelectedItem` binding example showing the ViewModel/Form value contract.
- Docs
  - Document the `SelectedItem` Form value owner in ComboBox overview and implementation notes.
  - Document the non-editable selected content overflow tooltip boundary.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `ComboBox`.
  - Align generated output paths with `controls/combo-box/index-cn.md` and `controls/combo-box/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete ComboBox desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish ComboBox desktop architecture documentation under `docs/controls/desktop/navigation/combo-box/overview.md`.
  - Add ComboBox implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add ComboBox control-level changelog.
  - Add ComboBox Token documentation covering ComboBoxToken.
