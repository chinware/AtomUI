# WindowTitleBar Changelog

本文档记录 WindowTitleBar 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-23

- Design
  - Define public `WindowTitleBarButton` and `WindowTitleBarToggleButton` controls for `LeftAddOn` and `RightAddOn` content.
  - Share managed caption visual tokens and active/inactive state with system caption buttons while keeping Window operations, native element roles and platform chrome internal.
- API
  - Add `WindowTitleBarButton : IconButton` and `WindowTitleBarToggleButton : ToggleIconButton` as additive public AXAML controls.
- Theme
  - Register independent AddOn themes that reuse `WindowTitleBarToken` dimensions, active/inactive colors, interaction states and motion policy without entering the system caption contract.
- Integration
  - Add Gallery Workspace coverage for appearance and Wave Spirit actions, including checked-state synchronization with the existing menu.
- Verification
  - Cover host-state projection, standalone fallback, icon-pair switching, theme asset registration and pointer-input isolation in `WindowTitleBarButtonTests`.

## 2026-08-19

- Architecture
  - Define a shared Window host projection for every `WindowTitleBar` in an AtomUI Window logical tree.
  - Make `Window` own projection definitions and lease creation while each `WindowTitleBar` owns and disposes its connection lease.
  - Include drag and double-click interaction subscriptions in each host projection lease while keeping default-title-bar content projection separate.
  - Keep title-bar height hints and CSD geometry owned only by the default title bar; content-area title bars receive caption, drag and double-click behavior without becoming geometry owners.
- Platform
  - Keep CSD windows on `WindowDecorations.Full` when the AtomUI title bar is hidden, and hide only the drawn title-bar visual so native window state transitions remain platform-owned.
- Verification
  - Extend the caption button lifecycle contract to cover content-area title bars, real pointer double-click toggling, multiple title bars per Window, host switching and lease release.

## 2026-08-18

- Design
  - Define a conditional gap between effective Windows/Linux Logo and LeftAddOn content while keeping macOS Leading and Logo/Title grouping unchanged.
- Token
  - Add the dedicated `LogoAndLeftAddOnSpacing` semantic with a default `SpacingXXS` value of 4 logical pixels.
- Architecture
  - Separate managed caption button requested visibility from Window capability and platform support.
  - Keep Window as the operation and state owner while CaptionButtonGroup derives presentation state and forwards declarative actions.
- Docs
  - Add the caption button configuration design covering Public API defaults, platform/state matrices, template flow, lifecycle and verification boundaries.
  - Synchronize the Leading composition, title safe-region formula, Token contract and conditional-spacing verification requirements.

## 2026-07-28

- Integration
  - Project the existing left and right add-on content and template properties from the default `Window` facade.
- Docs
  - Clarify default Window ownership and preserve `WindowTitleBar` layout and interaction responsibilities.

## 2026-07-23

- Theme
  - Place visible Windows and Linux `PART_Logo` content at the physical leading edge before `PART_LeftAddOn`, while keeping macOS Logo/Title grouping unchanged.
- Tests
  - Cover Windows/Linux template Logo ordering and verify explicit title alignments plus dynamic add-on changes against the shared safe-region formula.
- Docs
  - Synchronize the platform-specific Logo role model and Token spacing semantics.

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
