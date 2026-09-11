# SplitButton Changelog

本文档记录 SplitButton 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record SplitButton as the semantic owner, with Flyout used only as the relay adapter.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `SplitButton`.
  - Align generated output paths with `controls/split-button/index-cn.md` and `controls/split-button/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete SplitButton desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish SplitButton desktop architecture documentation under `docs/controls/desktop/general/split-button/overview.md`.
  - Add SplitButton implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add SplitButton control-level changelog.
  - Document that SplitButton does not require a dedicated Token document and records its theme dependencies in the overview.
