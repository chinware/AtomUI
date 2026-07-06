# Pagination Changelog

本文档记录 Pagination 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-06

- API
  - 将 `CurrentPage` 和 `PageSize` 设为默认 `TwoWay` 受控分页状态。
- Implementation
  - 内部页码和页大小更新改为 `SetCurrentValue`，避免破坏外部 binding owner。
- Gallery
  - 增加默认双向绑定示例，标记为 `v6.0.8`。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Pagination`.
  - Align generated output paths with `controls/pagination/index-cn.md` and `controls/pagination/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Pagination desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Pagination desktop architecture documentation under `docs/controls/desktop/navigation/pagination/overview.md`.
  - Add Pagination implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Pagination control-level changelog.
  - Add Pagination Token documentation covering PaginationToken.
