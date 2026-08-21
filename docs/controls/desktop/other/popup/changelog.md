# Popup Changelog

本文档记录 Popup 控件级 API、Theme、Token、实现结构和设计契约变化，不替代仓库根目录 `CHANGELOG.md`。

## 2026-08-21

- API
  - Add optional `Popup.SurfaceBackground` with a `null` default; a non-null brush explicitly selects a host-owned surface.
- Architecture
  - Extend the shared `ShadowsAwareContainer` frame renderer to draw an optional surface and the existing shadow for both native `PopupRoot` and `OverlayPopupHost`.
  - Keep `PopupRoot.Background=null` so native transparent window composition and shadow buffers remain unchanged.
- Compatibility
  - Keep Direct Popup, Flyout, ToolTip, ContextMenu, menu, selector, Picker, Tour and ColorPicker surfaces content-owned through the shared primitive default.
  - Remove redundant per-consumer `SurfaceBackground=null` declarations, keep the Direct Popup host transparent, and make its regression-demo Child own the visible surface.
- Verification
  - Add primitive, runtime-family, source-inventory and permanent TestApp contracts; record Windows/macOS as tested and Linux X11/Wayland as untested.
