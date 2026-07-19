# Modal Changelog

本文档记录 Modal 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-18

- Architecture
  - Replace the multi-owner host lifecycle with one `DialogSession`, two async presenters and one shared `DialogSurface` implementation.
  - Make `MessageBox` a direct Dialog specialization without a hidden Dialog or parallel host lifecycle.
- Behavior
  - Make `OpenAsync` represent the complete opening, active and teardown lifetime; reject concurrent instance opens.
  - Unify normal and forced close sources, deterministic post-commit teardown, opening/closing motion, nested focus restoration and owner close behavior.
  - Preserve natural Overlay/Window sizing, runtime size updates, placement, standard/custom buttons, loading and semantic MessageBox styles.
  - Fix real modal mask pointer routing while allowing modeless background input.
  - Construct generic static API views on the UI Dispatcher and remove Gallery dispatcher-yield workarounds from direct click handlers.
- Lifecycle
  - Pair presenter bindings, events, logical/resource parents, content references and button subscriptions with deterministic release paths.
  - Disconnect Surface composition children before host removal so retained Content or CustomButton controls cannot retain a closed Presenter/Surface.
  - Add Overlay and Window WeakReference coverage for Session, Presenter, Surface, user Content and retained custom controls.
- API
  - Remove synchronous/callback display APIs and obsolete host/action-result types.

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
