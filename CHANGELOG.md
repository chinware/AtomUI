# Changelog

All notable changes to AtomUI are documented in this file.

`AtomUI` follows Semantic Versioning 2.0.0.

## 6.0.6

`2026-06-24`

- Compatibility Notes
  - Several size-aware controls now use the customizable size model, allowing `Custom` size behavior in addition to built-in size types. Projects that directly depend on old size-type contracts should verify source compatibility.
- Added
  - Add customizable size support across Button family, DropdownButton, NumericUpDown, Mentions, Select, TreeSelect, ToggleSwitch, SpinIndicator and related size-aware controls.
  - Add configurable Button icon placement.
  - Add Slider desktop control with documentation and Gallery coverage.
  - Add `Col.Flex` support and improve Row/Grid responsive layout behavior.
  - Add editable ComboBox filtering, candidate keyboard navigation, Enter/Escape behavior and empty-result feedback.
  - Add NavMenu keyboard navigation, active item feedback and inline collapsed mode.
  - Add initial theme algorithm configuration so applications can render the first frame with dark theme.
  - Add bindable TreeView/Cascader node or option support for binding-oriented data scenarios.
  - Add reusable GalleryBase toolkit package and integrate Gallery shell/routing foundations.
- Changed
  - Improve custom size layout and Gallery showcases for Button, SplitButton, LineEdit, SearchEdit, DatePicker, TimePicker, ColorPicker, Select, TreeSelect and other controls.
  - Improve Form validation flow so submit-oriented validation does not show errors too early.
  - Improve Collapse accordion state handling, padding calculation and content visibility behavior.
  - Improve Expander indicator spacing, content visibility and layout behavior.
  - Improve Descriptions, Segmented, Steps, ProgressBar, Skeleton, Card, AvatarGroup, Transfer, Upload, Pagination and Breadcrumb implementation structure and correctness.
  - Improve SpinIndicator animation handling, default alignment and customizable size rendering.
  - Improve RibbonBadge positioning and adorner visibility behavior.
  - Improve CompactSpace internal collaboration and layout handling.
  - Upgrade Avalonia dependency from `12.0.4` to `12.0.5`.
- Fixed
  - Fix DatePicker and TimePicker preferred input width calculation.
  - Fix ListBox/ListView filtered item state handling.
  - Fix NavMenu selection preservation in inline collapsed mode.
  - Fix Gallery AppImage installer asset paths.
  - Fix Gallery macOS DMG workflow by avoiding unused Homebrew taps.
  - Fix Breadcrumb generated item state cleanup and `IBreadcrumbItemData.Content` mapping.
- Documentation
  - Add or complete control documentation for LineEdit, SearchEdit, ListView, NumericUpDown, Select, Slider, ToggleSwitch, TreeSelect, Descriptions, Expander, Segmented, ProgressBar, Card, Cascader, Form and other desktop controls.
  - Add control optimization skill documentation, including API layout, file splitting, lifecycle and root-cause optimization rules.
  - Update Gallery showcase examples for custom size, keyboard navigation and newly documented control scenarios.

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
