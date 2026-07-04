# DataGrid Changelog

本文档记录 DataGrid 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-04

- Docs
  - Define and implement the DataGrid column filter binding model: `Filters` as bindable filter item source and `SelectedFilterValues` as the single selected-state owner.
  - Document column `DataContext` binding support, generated accessor requirements for filter item DTOs, filter mode enums, `FilterDescriptions` projection rules, flyout checked-state synchronization invariants and explicit `Binding.DataType` usage when row `x:DataType` is active.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `DataGrid`.
  - Align generated output paths with `controls/data-grid/index-cn.md` and `controls/data-grid/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete DataGrid desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish DataGrid desktop architecture documentation under `docs/controls/desktop/data-display/data-grid/overview.md`.
  - Add DataGrid implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add DataGrid control-level changelog.
  - Add DataGrid Token documentation covering DataGridToken.
