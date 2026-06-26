# TabStrip Changelog

本文档记录 TabStrip 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `TabStrip`.
  - Align generated output paths with `controls/tab-strip/index-cn.md` and `controls/tab-strip/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete TabStrip desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish TabStrip desktop architecture documentation under `docs/controls/desktop/navigation/tab-strip/overview.md`.
  - Add TabStrip implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add TabStrip control-level changelog.
  - Document that TabStrip does not require a dedicated Token document and records its theme dependencies in the overview.
