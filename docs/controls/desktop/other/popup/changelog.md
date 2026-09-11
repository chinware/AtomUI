# Popup Changelog

本文档记录 Popup 控件级 API、Theme、Token、实现结构和设计契约变化，不替代仓库根目录 `CHANGELOG.md`。

## 2026-08-25

- Design
  - Define the internal-only pinned-open contract across semantic owners, Flyout adapters, and the physical Popup.
- Implementation
  - Gate pinned opening on content, anchor, attachment, effective visibility/enabled state, TopLevel, and placement validity.
  - Reject ordinary close sources without starting close motion, while lifecycle teardown clears hosts, pending requests, bindings, subscriptions, trackers, and timers.
  - Keep an already open Popup open after unpinning, but cancel and clean a not-yet-open pending request without transient business-state close notifications.

## 2026-08-24

- Lifecycle
  - Separate ordinary animated close from placement-target and logical-owner teardown; invalid popup sessions now close synchronously instead of retaining an Avalonia host without a valid anchor.
  - Require an animated close to preserve every owner established for the current open session: an existing logical owner must remain attached, while the effective PlacementTarget must remain attached, transformable and owned by the captured TopLevel. Direct Popup sessions that started without a logical owner continue to animate against an explicit valid PlacementTarget.
- Motion
  - Replace independent close-motion flags with the shared internal `AtomUI.MotionScene.MotionExecutionState` flow (`Idle` / `Pending` / `Playing` / `Completing`), coalesce repeated close requests and clear motion/session state from the `Closed` boundary.
- Verification
  - Add lifecycle regressions for placement-target detach, Popup logical detach, cross-TopLevel target replacement and ordinary animated close behavior.

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
  - Add primitive, runtime-family and source-inventory contracts; record Windows/macOS as tested and Linux X11/Wayland as untested.
