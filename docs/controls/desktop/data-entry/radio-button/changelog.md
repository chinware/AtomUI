# RadioButton Changelog

本文档记录 RadioButton 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-06

- Behavior
  - Make `RadioButtonGroup.CheckedItem` default to TwoWay binding and enable Avalonia data validation.
  - Keep user selection, Form value and ViewModel state on the same `CheckedItem` source of truth.
- Gallery
  - Add a `v6.0.8` RadioButtonGroup binding example showing user selection and ViewModel updates staying synchronized.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `RadioButton`.
  - Align generated output paths with `controls/radio-button/index-cn.md` and `controls/radio-button/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete RadioButton desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish RadioButton desktop architecture documentation under `docs/controls/desktop/data-entry/radio-button/overview.md`.
  - Add RadioButton implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add RadioButton control-level changelog.
  - Add RadioButton Token documentation covering RadioButtonToken.
