# Changelog

All notable changes to AtomUI are documented in this file.

`AtomUI` follows Semantic Versioning 2.0.0.

## 6.0.5

`2026-06-19`

- Breaking Changes
  - Responsive layout APIs are now unified under `AtomUI.Controls.Shared.MediaQuery`.
  - Add shared `ResponsiveInt`, `ResponsiveDouble`, `ResponsiveGutter`, and `ResponsiveValueMap` models.
  - Add the `xxxl` breakpoint and corresponding `ScreenXXXL` token support.
  - Grid, Descriptions, and Masonry now share the same mobile-first responsive parsing and fallback rules.
  - Custom code depending on old per-control responsive parsing, especially `DescriptionsMediaBreakInfo`, should migrate to the shared responsive value types.
- Gallery
  - Redesign the Gallery shell and showcase page structure.
  - Add sticky showcase tabs, deferred showcase loading, scenario diagnostics, and snapshot normalization.
  - Add localized API and Design Token tables for most migrated showcase pages.
  - Align Browser Gallery routing and navigation with shared desktop Gallery components.
  - Add Gallery version display and Community Telegram section.
- New Controls and Layout
  - Add BorderBeam with geometry, color stop, token, Gallery, documentation, and tests.
  - Add Masonry with responsive columns, gutter support, loading skeleton and image demos, Gallery coverage, and layout tests.
  - Add shared responsive layout support for Grid, Descriptions, and Masonry.
- Desktop Controls
  - Button: add `Color`, `Variant`, and `CustomBackground` support, including gradient backgrounds and updated Gallery/API documentation.
  - NumericUpDown: add inline spinner mode and improve handle styling.
  - ButtonSpinner: improve disabled state handling and add style variant support.
  - NavMenu: add `ClearSelection`, `IsItemBackgroundEnabled`, improved selection coordination, inline spacing, and background behavior.
  - TextArea: improve resize behavior.
  - GroupBox: fix border visibility when background is transparent.
  - Pagination: improve page size validation and custom page size options.
  - Dialog and Menu: prevent dialog reentrancy during close/open flows and improve menu close behavior.
  - HyperLinkButton: make icon color follow foreground.
- NativeAOT, Diagnostics, and DataGrid
  - Harden generated data member accessors for NativeAOT scenarios.
  - Add AOT data member path analyzer and compiler diagnostics guidelines.
  - Fix DataGrid sorting path resolution and add sorting regression tests.
  - Fix empty Gallery design token table state.
  - Implement scoped resource and theme hosts for non-visual `AvaloniaObject` usages to prevent dynamic resource retention leaks.
- Native Window and Platform Stability
  - Improve Linux window initialization, scaling, frame extents, window chrome management, popup support, and title bar menu dismissal.
  - Improve macOS title bar logo visibility.
  - Improve Windows Win32 composition options for performance and stability.
  - Correct popup, dialog, title bar, and Gallery text issues.
- Build and Release
  - Restore desktop diagnostics and stable generated source line endings.
  - Improve NativeAOT Gallery publishing workflow and validation.
  - Update `PublishToLocal.ps1` version extraction to use `AtomUIVersion`.
  - Update package metadata for AtomUI 6.0 release positioning.
