# Separator Changelog

本文档记录 Separator 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Separator`.
  - Align generated output paths with `controls/separator/index-cn.md` and `controls/separator/semantic-cn.md`.

## 2026-06-24

- Fix
  - Reduce untitled horizontal Separator block margin tokens to `1px` so plain divider lines do not create large vertical gaps in compact surfaces such as Drawer headers.

- Docs
  - Complete Separator desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Separator desktop architecture documentation under `docs/controls/desktop/general/separator/overview.md`.
  - Add Separator implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Separator control-level changelog.
  - Add Separator Token documentation covering SeparatorToken.
