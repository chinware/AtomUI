# Grid Changelog

本文档记录 Grid 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Grid`.
  - Align generated output paths with `controls/grid/index-cn.md` and `controls/grid/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Grid desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Grid desktop architecture documentation under `docs/controls/desktop/layout/grid/overview.md`.
  - Add Grid implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Grid control-level changelog.
  - Document that Grid does not require a dedicated Token document and records its theme dependencies in the overview.
