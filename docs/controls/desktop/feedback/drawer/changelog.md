# Drawer Changelog

本文档记录 Drawer 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-20

- Architecture
  - Keep Drawer containers in the owning Window `TopLevel` and its `ScopeAwareAdornerLayer`; reserve `WindowDrawnDecorations` overlay for chrome visuals.
  - Share the Window-owned reference-counted drawn chrome suppression lease with Overlay Dialog without sharing container, motion or close state.
- Behavior
  - Keep ComboBox, Select, DatePicker, Tooltip and Flyout content popups visible, interactive and light-dismissible through the owning TopLevel popup layers.
- Lifecycle
  - Remove the Drawer container directly from its scope layer and release host geometry/chrome leases on close or disposal, preserving reopen behavior without retaining the closed visual subtree.
  - Migrate an open Drawer atomically when `OpenOn` changes: move its container to the new scope layer, rebind visible-frame geometry, transfer the Window chrome lease and propagate the effective target to open nested Drawers.
- Validation
  - Reuse the canonical popup entry inventory, primitive matrix and control-family matrix instead of limiting Drawer content coverage to named selector examples.
  - Cover live `OpenOn` migration from Window to local target, between Windows and across nested open Drawers, including old-layer and suppression-lease cleanup.
  - Record Windows and macOS as tested for the final shared layering scheme; Linux X11/Wayland remains untested.

## 2026-07-20

- Architecture
  - Select the drawn decorations Drawer host by actual host capability instead of an OS or CSD gate.
  - Share the Window visible-frame geometry used by frame clipping and Overlay Dialog while preserving Drawer-owned container, motion and nested push lifecycle.
- Behavior
  - Use managed/drawn title-bar space on every platform and in both CSD and non-CSD Window paths while excluding transparent frame-shadow buffers.
  - Keep nested Drawer open/close on the containing scope layer when a drawn decorations overlay has no discoverable TopLevel visual ancestor.
- Docs
  - Document drawn-host fallback, visible-frame sizing and cross-platform maintenance invariants.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Drawer`.
  - Align generated output paths with `controls/drawer/index-cn.md` and `controls/drawer/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Drawer desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Drawer desktop architecture documentation under `docs/controls/desktop/feedback/drawer/overview.md`.
  - Add Drawer implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Drawer control-level changelog.
  - Add Drawer Token documentation covering DrawerToken.
