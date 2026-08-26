# DataGrid Changelog

本文档记录 DataGrid 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Design
  - Define `DataGrid` as the internal pinned-filter semantic owner and select one eligible filter column in DisplayIndex order.
- Implementation
  - Relay pinned state through Header -> FilterIndicator -> Menu/Tree Flyout -> Popup without reflection or runtime discovery.
  - Close and release the previous target on column, presenter, template, or lifecycle replacement while keeping an already open filter Flyout open after ordinary unpin.

## 2026-08-18

- Implementation
  - Make finite star-column resolution independent of `DataGridRowsPresenter` visibility by storing the active column viewport on `DataGrid` and accepting empty-state widths from the normal or group header presenter.
  - Separate initial Auto measurement completion from star-width distribution, while preserving existing min/max, resize, frozen-column, scrollbar and filler handling through `AdjustColumnWidths`.
- Docs
  - Add the dedicated DataGrid column sizing design covering width-mode semantics, DataGrid-owned star resolution, presenter viewport ownership, empty-state template integration, filler boundaries, compatibility, and verification.
  - Synchronize the architecture and implementation documents with the shared finite-viewport column sizing model.

## 2026-07-23

- Implementation
  - Make pagination state projection independent of `ItemsSource`, `PageSize`, and template-application order.
  - Replay CollectionView pagination state before subscribing newly acquired top and bottom pagination parts.
- Docs
  - Define the CollectionView-owned pagination flow, template-part lifecycle, and replay invariants.

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
