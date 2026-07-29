# Tag Changelog

本文档记录 Tag 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-29

- Design/API
  - Define `TagVariant` with `Filled`, `Solid` and `Outlined`; the default is `Filled`.
  - Remove `IsBordered` from the Tag contract. The old borderless and inverse compatibility behavior is not part of the new design.
  - Keep Tag color input orthogonal to Variant and define the Ant Design v6 `Default`, `Preset`, `Status` and `Custom` color matrix.
  - Define `CheckableTag` as a ToggleButton-based binary selection tag with Content, Icon, command, motion and Form semantics, without ordinary Tag color, Variant or close APIs.
  - Define `CheckableTagGroup` with primitive/structured Options, cancellable single selection, multiple selection, TwoWay `CheckedItem(s)`, one-time defaults, mode conversion and unified change events.
- Architecture
  - Keep Group business values separate from the internal SelectionModel, option wrappers and CheckableTag containers through an internal `SelectingItemsControl` host without a public `ListBox` fallback.
  - Define collection replacement, in-place collection notifications, template reapplication, container recycle and detach release invariants.
- Theme/Token
  - Define preset palette mapping, semantic status mapping, custom HSL light-background calculation and default Solid text contrast.
  - Keep `info` as the AtomUI input alias for `processing` and use the same Info semantic tokens.
  - Do not add global Tag configuration or put Variant into `ThemeConfig`.
  - Reuse Tag family and SharedToken visual semantics for CheckableTag and keep CheckableTagGroup free of component-specific Tokens.
- Docs
  - Add the CheckableTag and CheckableTagGroup selection-model design and synchronize the Tag overview, implementation, Token and LLMS source map.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Tag`.
  - Align generated output paths with `controls/tag/index-cn.md` and `controls/tag/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Tag desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Tag desktop architecture documentation under `docs/controls/desktop/data-display/tag/overview.md`.
  - Add Tag implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Tag control-level changelog.
  - Add Tag Token documentation covering TagToken.
