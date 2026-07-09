# Window Changelog

本文档记录 Window 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-09

- Docs
  - Clarify `TitleBarFrameLayer` as the title-bar background/decorative layer, not the user interaction entry.
  - Document that title-bar buttons, menus and search boxes should be hosted by a custom `TitleBar`.
  - Clarify the responsibility split between `TitleBarFrameLayer`, `TitleBar`, and Avalonia `WindowDrawnDecorations` CSD roles.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Window`.
  - Align generated output paths with `controls/window/index-cn.md` and `controls/window/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Window desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Window desktop architecture documentation under `docs/controls/desktop/window/window/overview.md`.
  - Add Window implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Window control-level changelog.
  - Add Window Token documentation covering WindowToken.
