# Timeline Changelog

本文档记录 Timeline 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-28

- API
  - Define `Orientation` as the Timeline primary-axis selector with `Vertical` as the default.
  - Replace `TimelineMode.Left` and `TimelineMode.Right` with the logical `Start` and `End` values; keep `Alternate` and use `Start` as the default.
  - Keep placement ownership on Timeline without adding an item-level placement API or a second placement enum.
- Design
  - Define the Vertical/Horizontal x Start/End/Alternate layout matrix, logical RTL mapping, visual-order-based Reverse behavior, equal-width horizontal items and item-local text wrapping.
  - Establish visible visual order as the single source for Alternate parity, first/last state, Label layout and Pending adjacency.
- Theme
  - Reuse the existing Timeline, TimelineItem, TimelineStackPanel, TimelineItemPanel and TimelineIndicator composition for both orientations.
  - Define orientation-specific Item spacing and horizontal Indicator rendering without adding a second ControlTemplate or internal horizontal scrollbar.
- Token
  - Replace physical `IndicatorLeftModeMargin` and `IndicatorRightModeMargin` naming with `IndicatorStartModeMargin` and `IndicatorEndModeMargin`.
  - Keep horizontal cross-axis gap on SharedToken instead of duplicating it in TimelineToken.
- Docs
  - Add `orientation-layout-design.md` and synchronize the Timeline overview, implementation and Token boundaries.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Timeline`.
  - Align generated output paths with `controls/timeline/index-cn.md` and `controls/timeline/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Timeline desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Timeline desktop architecture documentation under `docs/controls/desktop/data-display/timeline/overview.md`.
  - Add Timeline implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Timeline control-level changelog.
  - Add Timeline Token documentation covering TimelineToken.
