# Window Changelog

本文档记录 Window 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-28

- API
  - Add `LeftAddOn`, `LeftAddOnTemplate`, `RightAddOn` and `RightAddOnTemplate` as `Window` owners of the default `WindowTitleBar` content contract.
- Docs
  - Define the default title-bar add-on entry points and keep `Window.TitleBar` as template-owned internal state.

## 2026-07-23

- Implementation
  - Rename the shared Linux chrome manager to `AbstractLinuxWindowChromeManager` and the fallback backend manager to `GenericLinuxWindowChromeManager`.
- Docs
  - Synchronize Window and WindowTitleBar implementation docs with the renamed Linux chrome manager structure.

## 2026-07-22

- Implementation
  - Move platform chrome managers under `Window/Chrome` and Window helper implementations under `Window/Utils`.
  - Move the Wayland input-region reflection bridge into `AtomUI.Native/Linux` while keeping X11 shadow input-region policy in `X11WindowChromeManager`.
- Docs
  - Clarify that Window visual helpers are internal observable implementation details, not public API.

## 2026-07-21

- Architecture
  - Define one Windows, macOS and Linux initial theme-surface contract owned by the shared Window show lifecycle.
  - Keep `WindowTheme` as the only long-lived background owner and limit pre-show initialization to a synchronous scoped Snapshot read with disposable Template-priority values.
- Maintenance
  - Reserve platform chrome managers for native geometry and capability projection; require platform-specific surface fallback only after managed first-frame state is proven correct on that backend.

## 2026-07-20

- Architecture
  - Define complete layer, visible frame and content bounds as separate Window geometry contracts.
  - Make `FrameShadowThickness` plus the actual drawn-host presence the platform capability projection consumed by Dialog and Drawer.
- Maintenance
  - Keep native backend differences inside Window chrome managers and remove speculative same-value OS branches from Window tokens.

## 2026-07-09

- Docs
  - Clarify `TitleBarFrameLayer` as the title-bar background/decorative layer, not the user interaction entry.
  - Document that title-bar buttons, menus and search boxes should be hosted by a custom `TitleBar`.
  - Clarify the responsibility split between `TitleBarFrameLayer`, `TitleBar`, and Avalonia `WindowDrawnDecorations` CSD roles.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Window`.
  - Align generated output paths with `controls/window/index-cn.md` and `controls/window/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Window desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Window desktop architecture documentation under `docs/controls/desktop/window/window/overview.md`.
  - Add Window implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Window control-level changelog.
  - Add Window Token documentation covering WindowToken.
