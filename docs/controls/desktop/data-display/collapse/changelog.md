# Collapse Changelog

本文档记录 Collapse 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-13

- Design
  - Define Avalonia selection model as the sole expansion-state owner for normal and accordion modes.
  - Define structural separator ownership so item and content borders no longer depend on selection or motion timing.
- Compatibility
  - Preserve the existing public API, stable template parts, resource keys, Token names and theme values.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Collapse`.
  - Align generated output paths with `controls/collapse/index-cn.md` and `controls/collapse/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Collapse desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Collapse desktop architecture documentation under `docs/controls/desktop/data-display/collapse/overview.md`.
  - Add Collapse implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Collapse control-level changelog.
  - Add Collapse Token documentation covering CollapseToken.
