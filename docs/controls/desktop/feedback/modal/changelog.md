# Modal Changelog

本文档记录 Modal 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-06

- API
  - Make `Dialog.IsOpen` default to `BindingMode.TwoWay` so controlled dialog open state updates the bound ViewModel without explicit binding mode.
  - Add `DialogOptions.BeforeCloseAsync`, `DialogClosingContext` and `DialogCloseReason` for static Dialog close-before validation.
- Behavior
  - Route Dialog button, keyboard, host, owner, placement-target and programmatic close requests through one async close pipeline.
- Docs
  - Document `Dialog.IsOpen` as controlled open state and clarify that it is not a Form validation value.
  - Update the close-before validation design from planned behavior to implemented API and pipeline invariants.

## 2026-07-03

- Docs
  - Add the approved design for `DialogOptions.BeforeCloseAsync` as a future close-before-validation API for static Modal dialogs.
  - Document the intended close request pipeline, event order, async validation behavior and compatibility boundaries.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Modal`.
  - Align generated output paths with `controls/modal/index-cn.md` and `controls/modal/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Modal desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Modal desktop architecture documentation under `docs/controls/desktop/feedback/modal/overview.md`.
  - Add Modal implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Modal control-level changelog.
  - Add Modal Token documentation covering DialogToken.
