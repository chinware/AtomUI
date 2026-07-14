# Menu Changelog

本文档记录 Menu 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-14

- Docs
  - Define a single-owner hover intent model for delayed submenu open and close behavior.
  - Require cancellable or invalidatable delayed callbacks across `Menu`, `ContextMenu` and the default `MenuFlyoutPresenter`.
  - Document close, window deactivation and detach cleanup boundaries without changing the existing public interaction-handler contract.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Menu`.
  - Align generated output paths with `controls/menu/index-cn.md` and `controls/menu/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Menu desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Menu desktop architecture documentation under `docs/controls/desktop/navigation/menu/overview.md`.
  - Add Menu implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Menu control-level changelog.
  - Add Menu Token documentation covering MenuToken.
