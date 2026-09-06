# DataGrid Changelog

本文档记录 DataGrid 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-09-06

- Design
  - Define one active viewport request scope per generation, retain shared block leases before superseding the prior scope, and cancel orphaned visible or prefetch work so rapid thumb input cannot accumulate historical requests ahead of the final target.
  - Define foreground-visible scheduling ahead of scope-bound prefetch, generation-level bootstrap ownership, cooperative Source cancellation as the latency contract, and deterministic cleanup for scope, lease, work-item CTS and inflight state.
- API
  - Keep the public data-entry name `ItemsSource`, redefine its type as `IDataGridSource?`, and remove the transitional `Source` name without adding a compatibility alias or restoring the legacy enumerable/CollectionView path.
  - Name the immutable pagination value `DataGridPageRequest`, expose it consistently through `DataGrid.PageRequest`, `DataGrid.AppliedPageRequest`, and `DataGridFetchRequest.PageRequest`, and remove the provisional pagination-request names without compatibility aliases.
- Implementation
  - Replace the mutable ItemsSource/CollectionView execution path with immutable `DataGridQuery`, range-based `IDataGridSource`, atomic presentation snapshots, stable-key selection/current state and optional key-based mutation capabilities.
  - Keep the existing RowsPresenter/DisplayData container virtualizer, add bounded range cache/pins, two-request concurrency, latest-wins cancellation, snapshot-expiry recovery and sparse O(log M) variable-height lookup.
  - Add the empty-query local range fast path, synchronous Source/cache-hit commit path, exact fixed-height first realization and auto-sized nested-grid bootstrap without Source I/O in layout.
  - Remove per-cell sort subscriptions and project sort/filter/loading state from the single committed Query/presentation owner.
  - Make schema display access explicit with `DataGridFieldDisplayAccessor`; auto-generated columns now use disposable compiled bindings and never interpret protocol `FieldId` values as CLR property paths.
  - Keep committed content fully opaque while `LoadState` is `Refreshing`; only initial `Loading` or explicit `IsOperating` drives the existing Spin, so sort and other query refreshes do not produce a transient fade.
- Gallery and validation
  - Add the deterministic million-row fake-remote showcase with latency, cancellation, failure/reload and snapshot-expiry controls.
  - Give the Basic Paging example explicit Auto-column `MinWidth` baselines so its first committed page exposes natural horizontal overflow without preloading later ranges, and cover scrollbar stability across page changes with an attached headless Gallery regression.
  - Add Query/Source, selection, mutation, lifecycle, virtualization, RowDetails, Gallery and million-row performance coverage plus a dedicated repeatable DataGrid benchmark.
- Docs
  - Synchronize the public overview, implementation guide, formal Query/Range Source design, generated LLMS sources and Data Display navigation with the final contract.
  - Clarify that virtualized Auto sizing measures realized cells only and that stable first-page content extent must come from declared column geometry rather than offscreen Source I/O, Star compression or forced scrollbar visibility.

## 2026-09-05

- Design
  - Define immutable `DataGridQuery`, `IDataGridSource` range requests, Source schema, stable row/group keys and snapshot-based atomic presentation as the single DataGrid data model for local and remote sources.
  - Separate long source-data indices, int window-data indices and int display slots; keep the existing RowsPresenter/DisplayData/container pools as the only visual virtualizer.
  - Define latest-wins DesiredViewport/CommittedViewport coordination, bounded block cache and prefetch, sparse variable-height metrics, pin ownership, full failure rollback and zero Source I/O in layout/container hot paths.
  - Define declarative key/query/interval selection and key-relative optional mutation capabilities for data outside the loaded range.
- Docs
  - Add the dedicated Query and Range Source design and synchronize the DataGrid overview, implementation ownership, LLMS source map and Data Display navigation.

## 2026-09-04

- Implementation
  - Keep `DataGridCell` pseudo-classes authoritative with row state: push `:selected` to all cells when `DataGridRow.IsSelected` changes, and initialize pseudo-classes at cell creation, instead of relying only on opportunistic recycle/current-cell refresh paths.
  - In the cell ControlTheme, make the sorted-column background (`BodySortBg`) yield to the row selection background when the cell is in a selected row, matching Ant Design where the selected-row cell background outranks `td.ant-table-column-sort`; unselected rows keep the sort tint. Fixes [#454](https://github.com/AtomUI/AtomUI/issues/454).

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
