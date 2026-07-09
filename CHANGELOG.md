# Changelog

All notable changes to AtomUI are documented in this file.

`AtomUI` follows Semantic Versioning 2.0.0.

## 6.0.8

`2026-07-10`

- Breaking Changes
  - ImagePreviewer: replace `SourceUri`, `SourceUris`, and `FallbackSourceUri` with `Source`, `Sources`, and `FallbackSource` based on `IImagePreviewSource`. Use `UriImagePreviewSource` for URI, local file and Avalonia resource images, and `StreamImagePreviewSource` for lazy stream sources. See [6.0.8 API change examples](docs/release-notes/6.0.8-api-changes.md).
  - Upload: replace the old `IsUploadDirectoryEnabled`, `IsShowUploadTrigger`, and `DefaultTaskList` composition model with composable `UploadTrigger`, `UploadDropZone`, and `Files`. See [6.0.8 API change examples](docs/release-notes/6.0.8-api-changes.md).
- Data Entry and Selection Controls
  - Add OtpLineEdit for one-time password input, including `Text` two-way binding, `Length`, `InputMode`, `Formatter`, masking, separators, `Completed`, four `StyleVariant` surfaces, Form integration and `DataValidationErrors` support.
  - Route Form validation through Avalonia `DataValidationErrors` and add `ValidateTrigger`; validation now defaults to value changes and can be configured to run on blur.
  - Add or refine default two-way binding and data validation semantics for Select, Cascader, TreeSelect, ListView, CheckBoxGroup, RadioButtonGroup, Rate, DatePicker, TimePicker, Transfer, ColorPicker, Slider, Dialog, Tour and ImagePreviewer.
  - Add overflow tooltip support for Select, ComboBox and Cascader through `IsShowOverflowTip`, `OverflowTipDelay` and `OverflowTipPlacement`.
  - Add popup display anchor properties for DatePicker and TimePicker, and add range selection binding examples.
- Upload
  - Refactor Upload into a file-state coordinator and add `UploadFileItem`, `UploadTrigger`, `UploadDropZone`, `UploadFileValueMode`, `AutoUpload`, `ListMaxHeight`, `ListScrollBarVisibility`, `SuccessAutoRemoveDelay`, `PendingText` and `TriggerContent`.
  - Improve file selection, directory selection, drag-and-drop upload, upload list scrolling, picture list preview, delete state and success auto-removal behavior.
- ImagePreviewer
  - Add the `IImagePreviewSource` URI and lazy-stream source model with `CoverIndex`, `MaxConcurrentLoads` and `PreloadCount`.
  - Fix fallback behavior so a single failed item is skipped and `FallbackSource` is used only when the whole source batch fails.
  - Improve group preview titles, cover loading, preview title-bar navigation icons and the 20-remote-image showcase.
- DataGrid
  - Add the column filter model with `Filters`, `SelectedFilterValues`, `FilterTextMemberPath`, `FilterValueMemberPath`, `FilterChildrenMemberPath`, `FilterPresenterMode`, `FilterSelectionMode` and `FilterApplyMode`.
  - Improve filter selection, tree filters, selected filter value binding and Gallery examples.
- TabControl and TabStrip
  - Add drag reordering for tabs with `IsTabReorderEnabled`, `TabActivationTrigger`, `TabReordering` and `TabReordered`.
  - Improve Chrome-style drag preview, scroll anchoring while dragging, selected indicator synchronization, no-icon layout for left/right placements and compact vertical spacing for default Line tabs.
- TreeView, Cascader and NavMenu
  - Fix TreeView `ItemsSource` drag-and-drop crashes and improve drag moves, selected item two-way binding, Form values and descendant bring-into-view behavior.
  - Fix Cascader selection synchronization and expand-state crashes, and add `SelectedOption` / `SelectedOptions` two-way binding.
  - Add NavMenu popup frame support for improved popup hosting, placement and dismissal behavior.
- Dialog, FloatButton and Base Visuals
  - Add `BeforeCloseAsync` for Dialog async close validation and unify close request handling.
  - Add command support to FloatButton and improve scrollbar-safe floating overlays and controlled `IsOpen` behavior.
  - Improve Avatar image clipping, layout-scaled control border rendering, Button border rendering and Space preset spacing.
- Gallery, Documentation and Build
  - Flatten standard Showcase examples and add examples for tab reordering, Upload, ImagePreviewer, OtpLineEdit, selection binding and range selection.
  - Update control design documents, Window title bar customization guidance, feature request issue guidelines, README package versions and Gallery banners.
  - Remove unused localization imports, remove the Labs package and update project documentation.

## 6.0.7

`2026-07-03`

- DatePicker
  - Add picker modes for date, week, month, quarter and year scenarios, with improved range panels and preferred input width calculation.
  - Improve range selection state, hover rendering and CalendarView lifecycle handling.
- DataGrid
  - Improve row details height estimation and expanded row virtualization stability.
  - Fix crashes when header/title interactions happen with empty or null `ItemsSource`.
  - Improve selection column initial state handling and frame corner radius behavior for bordered tables.
  - Fix AutoComplete focus preservation inside DataGrid template cells.
- Select
  - Fix Tags mode dynamic option creation so runtime tags do not mutate user `OptionsSource`.
  - Improve Tags keyboard behavior so Enter commits the active dynamic candidate, Escape closes the popup, and Up/Down navigate candidates from the search input.
  - Fix multi-select and Tags placeholders while IME preedit text is rendered.
- ImagePreviewer
  - Add local and remote image source loading through `ImageSourceUri`.
  - Add preview window title resolution, `PreviewTitleIcon`, loading skeletons and localized error states.
- Gallery and Documentation
  - Add reusable Gallery source-code display components, lazy code viewer loading, copyable code selections and generated snippet catalog support.
  - Add selectable Gallery API and design token table text.
  - Add LLMS documentation generation and refresh control documentation coverage.
  - Add organization documentation pages and update README package examples to `6.0.7`.
- Splash and Window
  - Add Splash control and service with owner-window handling, version tags in Gallery and improved show behavior.
  - Add Windows window chrome management with DWM shadow handling and improve resize artifact behavior.
- Controls
  - Add Extras and Labs modules for supplemental controls.
  - Add Splitter line styling APIs and Gallery showcase coverage.
  - Improve NumberUpDown handle customization and mode behavior.
  - Improve Button shadow rendering, SplitButton resize layout stability, Drawer right-click behavior, Message and Notification feedback layering, popup shadow handling and separator compact spacing.
  - Improve Form validator compatibility and add `ExtraExtraExtraLarge` breakpoint layout handling.
- Localization and Build
  - Update language provider generation and add Chinese language support for GalleryBase.
  - Add AppImage launcher validation support and update release workflows for Gallery and NuGet packages.

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
