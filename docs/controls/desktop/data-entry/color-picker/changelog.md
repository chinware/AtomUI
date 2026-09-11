# ColorPicker Changelog

本文档记录 ColorPicker 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record AbstractColorPicker as the semantic owner for ColorPicker.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-07-06

- API
  - Make `ColorPicker.Value` and `GradientColorPicker.Value` public controlled values with default `TwoWay` binding and Avalonia data validation.
- Behavior
  - Align clear/Form clear behavior so `Value` becomes `null` instead of only clearing trigger visuals.
- Gallery
  - Add a `v6.0.8` value binding example for solid and gradient pickers.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `ColorPicker`.
  - Align generated output paths with `controls/color-picker/index-cn.md` and `controls/color-picker/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete ColorPicker desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish ColorPicker desktop architecture documentation under `docs/controls/desktop/data-entry/color-picker/overview.md`.
  - Add ColorPicker implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add ColorPicker control-level changelog.
  - Add ColorPicker Token documentation covering ColorPickerToken.
