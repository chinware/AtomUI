# Badge Changelog

本文档记录 Badge 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-12

- Semantic Part
  - Add independent descriptors for `CountBadge`, `DotBadge`, and `RibbonBadge` with stable `root`, `indicator`, and ribbon `content` contracts.
  - Preserve Count/Dot owner-scoped styling across Avalonia 12 `AdornerLayer` by separating visual host from logical/style owner.
  - Add static semantic markers to all applicable Badge themes and generated selector-class usage for the runtime Ribbon indicator.
- Gallery
  - Add a truly deferred Semantic Parts Tab with independent Count, Dot, and Ribbon previews.
  - Register only each Count/Dot owner's runtime Adorner as an additional preview root; keep Ribbon on the inline path.
  - Extend `GalleryShowCaseHost` to manage multiple independent previews in one deferred content root.
  - Add a full-width, deferred `v6.1.3` example that demonstrates owner-scoped Semantic Part styling for Count and Ribbon badges.
- Tests
  - Cover descriptor metadata, marker placement, logical descendant selectors, Adorner detach cleanup, deferred Gallery creation, and precise cross-root preview registration.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Badge`.
  - Align generated output paths with `controls/badge/index-cn.md` and `controls/badge/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Badge desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Badge desktop architecture documentation under `docs/controls/desktop/data-display/badge/overview.md`.
  - Add Badge implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Badge control-level changelog.
  - Add Badge Token documentation covering BadgeToken.
