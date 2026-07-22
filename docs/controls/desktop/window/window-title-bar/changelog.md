# WindowTitleBar Changelog

本文档记录 WindowTitleBar 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-22

- API
  - Add `WindowTitleBarTitleAlignment`, `WindowTitleBar.TitleAlignment` and the `Window.TitleAlignment` owner projection.
- Theme
  - Use one full-frame `WindowTitleBarLayoutPanel` with Leading, Title and Trailing roles in the default, ImagePreviewer and fullscreen title hosts.
  - Treat native chrome insets, managed operations, add-on margins and conditional title spacing as single-source layout inputs.
  - Preserve `TitleBarPadding` as managed content spacing after the native chrome safe extent without shifting the full-frame title center.
- Implementation
  - Add stateless platform strategies and publish native chrome insets from the existing Window platform metric path.
  - Recalculate zero, hidden and dynamically replaced left/right add-on content without cached widths or ghost spacing.
- Docs
  - Synchronize the title alignment model, layout formulas, derived hosts and LLMS inputs.

## 2026-07-21

- Docs
  - Rewrite the control design, implementation and Token documents from the WindowTitleBar source, Themes, Window host and ImagePreviewer integration.
  - Define the cross-platform title alignment contract, platform Strategy ownership, CSD matrix, full-frame Template contract and shared safe-region algorithm.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `WindowTitleBar`.
  - Align generated output paths with `controls/window-title-bar/index-cn.md` and `controls/window-title-bar/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete WindowTitleBar desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish WindowTitleBar desktop architecture documentation under `docs/controls/desktop/window/window-title-bar/overview.md`.
  - Add WindowTitleBar implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add WindowTitleBar control-level changelog.
  - Add WindowTitleBar Token documentation covering WindowTitleBarToken.
