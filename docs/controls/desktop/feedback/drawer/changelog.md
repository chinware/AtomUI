# Drawer Changelog

本文档记录 Drawer 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

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
