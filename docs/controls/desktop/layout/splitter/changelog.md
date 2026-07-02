# Splitter Changelog

本文档记录 Splitter 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-02

- Runtime
  - Add `LineThickness` and `LineCornerRadius` public style APIs on `Splitter`, propagate them through `SplitterPanel`, `SplitterHandle` and `SplitterDragBar`, and bind them to `PART_HandleLine` / `PART_Grip`.
  - Bind Splitter root frame appearance to `Background`, `BorderBrush`, `BorderThickness` and `CornerRadius`.
- Gallery
  - Add Splitter API rows and a line style ShowCase item for root frame and visible divider customization.
- Docs
  - Clarify Splitter public contract, internal collaboration types and resize event model.
  - Document the style boundary for root frame appearance, visible split line thickness, split line corner radius and user-owned panel content.
  - Define the maintenance rule that line style APIs must be exposed from `Splitter` and propagated to internal handle parts instead of exposing `SplitterHandle` or `SplitterDragBar`.
  - Clarify the semantic difference between `HandleSize`, `SplitBarHandleSize`, `SplitBarSize` and `HandleLineThickness`.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Splitter`.
  - Align generated output paths with `controls/splitter/index-cn.md` and `controls/splitter/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Splitter desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Splitter desktop architecture documentation under `docs/controls/desktop/layout/splitter/overview.md`.
  - Add Splitter implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Splitter control-level changelog.
  - Add Splitter Token documentation covering SplitterToken.
