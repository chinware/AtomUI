# Popup Changelog

本文档记录 Popup 控件级 API、Theme、Token、实现结构和设计契约变化，不替代仓库根目录 `CHANGELOG.md`。

## 2026-08-21

- API
  - Add `Popup.SurfaceBackground`; the Popup Theme defaults it to Shared Token `ColorBgElevated`, while `null` selects a content-owned surface.
- Architecture
  - Extend the shared `ShadowsAwareContainer` frame renderer to draw an optional surface and the existing shadow for both native `PopupRoot` and `OverlayPopupHost`.
  - Keep `PopupRoot.Background=null` so native transparent window composition and shadow buffers remain unchanged.
- Compatibility
  - Keep all framework-owned Flyout, ToolTip, ContextMenu, menu, selector, Picker, Tour and ColorPicker surfaces content-owned through explicit `SurfaceBackground=null` declarations.
  - Change only direct Popup's default visual contract from transparent to the theme elevated surface; transparent direct Popups opt out explicitly.
- Verification
  - Add primitive, runtime-family, source-inventory and permanent TestApp contracts; record Windows/macOS as tested and Linux X11/Wayland as untested.
