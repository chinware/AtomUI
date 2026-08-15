# Skeleton Changelog

本文档记录 Skeleton 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Fixed
  - Pause active shimmer when Skeleton is effectively invisible, including visibility inherited from hidden Visual ancestors.
- Docs
  - Document the effective-visibility lifecycle and hidden-page performance invariant.

## 2026-08-15

- Semantic Parts
  - Add Ant Design-aligned Semantic Part descriptors for `Skeleton` and its public element owners.
  - Add static template markers and generated owner-scoped style contracts without runtime discovery.
  - Document the dual static content layers used by normal and active Skeleton states.
- Gallery
  - Add the Semantic Parts Preview and Ant Design-aligned custom styling example using AtomUI `v6.1.3` version metadata.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Skeleton`.
  - Align generated output paths with `controls/skeleton/index-cn.md` and `controls/skeleton/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Skeleton desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Skeleton desktop architecture documentation under `docs/controls/desktop/feedback/skeleton/overview.md`.
  - Add Skeleton implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Skeleton control-level changelog.
  - Add Skeleton Token documentation covering SkeletonToken.
