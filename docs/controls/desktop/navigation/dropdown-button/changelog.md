# DropdownButton Changelog

本文档记录 DropdownButton 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record DropdownButton as the semantic owner, with DropdownFlyout/MenuFlyout used only as relay adapters.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-03

- Design
  - Inherit Button `IconWidth` / `IconHeight` semantics for the user and loading icons across desktop and Browser templates.
  - Keep `OpenIndicator` sizing independent from the inherited user/loading icon sizing contract.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `DropdownButton`.
  - Align generated output paths with `controls/dropdown-button/index-cn.md` and `controls/dropdown-button/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete DropdownButton desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish DropdownButton desktop architecture documentation under `docs/controls/desktop/navigation/dropdown-button/overview.md`.
  - Add DropdownButton implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add DropdownButton control-level changelog.
  - Document that DropdownButton does not require a dedicated Token document and records its theme dependencies in the overview.
