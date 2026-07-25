# Changelog

All notable changes to AtomUI are documented in this file.

`AtomUI` follows Semantic Versioning 2.0.0.

## 6.1.1

`2026-07-25`

- Breaking Changes
  - ListView: replace the settable Avalonia `ISelectionModel` contract with a read-only `IListViewSelection` facade. Code that directly set `Selection`, `SelectedItem`, `SelectedItems` or `SelectedValue` should migrate to `SelectedIndex` or `Selection.Select` / `Selection.Deselect` / `Selection.Clear` / `Selection.SelectAll`; `SelectionChanged` now uses `ListViewSelectionChangedEventArgs`, and the old `ListCollectionViewChangedEventArgs` type has been removed. `ListItemData.IsSelected` has been removed, and `ListItemData`, `GroupListItemData` and `SelectOption` are now classes instead of records. See [6.1.1 API change examples](docs/release-notes/6.1.1-api-changes.md).
- Window and Dialog
  - Fix macOS modal dialog window title-bar and resize-region behavior so only one outer resizer is shown and owner-outside modal activation no longer causes visible flashing.
  - Fix Wayland dialog/window resize constraints, sudden resize growth, fractional-scale borders and mask alignment.
  - Fix Windows CSD dialog/window resize handling, background priming, frame dark mode and minimum-height constraints so native bounds stay aligned with drawn decorations.
  - Consolidate platform chrome boundaries while keeping Windows, macOS, Wayland, X11 and other Linux decoration behavior separated.
- WindowTitleBar
  - Add `WindowTitleBar.TitleAlignment` and `Window.TitleAlignment` with `Auto`, `Left`, `Center`, `WindowCenter` and `Right` modes.
  - Fix Windows/Linux logo ordering before the left add-on, Windows caption-button cursors and title-bar padding.
- ListView, Select and Transfer
  - Fix selection state and data-view index mismatches across filtering, grouping, pagination and duplicate item occurrences.
  - Add `ItemKeySelector` and `SelectedIndexes` so ListView can preserve selection by stable business keys and source indexes.
- DataGrid
  - Fix pagination state replay after the DataGrid template is reapplied.
- ImagePreviewer
  - Fix preview dialog resize motion, title centering and background stability.
- Drawer, ScrollViewer and Base Layout
  - Fix Drawer masks so they cover the visible window frame.
  - Fix fractional-scale sub-pixel overflow causing auto scrollbars to appear incorrectly.
  - Fix SplitButton child bounds across arrange passes.
- Gallery, Documentation and Build
  - Remove built-in Gallery API and design-token metadata table sidecars, and stop referencing those Gallery tables from LLMS output.
  - Stop tracking reproducible generated files and fix CRLF output in generated theme asset manifests.

## 6.1.0

`2026-07-20`

- Breaking Changes
  - Dialog and MessageBox: rebuild the static display and host lifecycle around async sessions. Custom code that referenced `IDialogHost`, `IDialogHostProvider`, `IDialogActionResult` or `IMessageBoxActionResult`, or depended on callback-style host close APIs, should migrate to `ShowDialogAsync`, `ShowDialogModalAsync`, `ShowMessageBoxAsync`, `ShowMessageBoxModalAsync`, `Dialog.OpenAsync`, `Dialog.BeforeCloseAsync` and the returned `Task<object?>`. See [6.1.0 API change examples](docs/release-notes/6.1.0-api-changes.md).
  - Steps: replace the selection-based contract with controlled step state. Migrate `CurrentStep` to `Current`, `InitialStep` to `Initial`, `CurrentStepStatus` / `StepsItemStatus` to `Status` / `StepsStatus`, `Style` / `ItemIndicatorType` to `Type`, `LabelPlacement` to `TitlePlacement`, and `ProgressValue` / `IsShowItemProgress` to nullable `Percent`; move item descriptions from `Description` / `DescriptionTemplate` to `Content` / `ContentTemplate`, and handle clickable step changes with `CurrentChangeRequested` instead of relying on selection mutation. See [6.1.0 API change examples](docs/release-notes/6.1.0-api-changes.md).
- Theme
  - Add resolver-based theme definition loading for built-in assets, application assets and explicitly enabled user theme directories using the XML theme definition format.
  - Add `IThemeDefinitionResolver`, `IThemeManager.AvailableThemes`, `IThemeManager.CurrentTheme`, `ThemeCatalogDiagnostics`, `ThemeCatalogChanged` and `ReloadThemesAsync` so applications can discover, switch and reload theme catalogs atomically.
  - Add `WithApplicationId`, `UseUserThemeDirectory()` and `UseUserThemeDirectory(string directory)`; the default user directory is the application data folder under `{ApplicationId}/Themes` when explicitly enabled.
  - Rebuild theme parsing, binding, catalog compilation, snapshot caching and token resource publication so root and scoped themes publish complete snapshots and preserve the previous state when parsing or compilation fails.
  - Generate shared control token resource metadata for AXAML and isolate control token scopes so primary, link and text-button states follow the active theme consistently.
- Gallery Themes
  - Add built-in theme definitions including Daybreak Blue, Polar Green, Sunset Orange, Golden Purple and Magenta, and expose them through a Theme Settings submenu with color swatches.
  - Enable Gallery theme switching through the theme manager and enable user theme directory loading for the desktop Gallery.
- Window and Native
  - Upgrade the Avalonia dependency to `12.1.0` and add native Wayland support.
  - Stabilize Windows and Linux drawn decorations, including Windows 10 CSD frame rendering, dark-theme title bars, caption buttons, live resize, Wayland rounded CSD clipping and Linux client-frame visual layers.
  - Separate complete-window masks, visible-frame bounds and shadow extents for Dialog and Drawer overlays so masks cover drawn title bars while placement remains inside the visible frame.
- Dialog, Drawer and MessageBox
  - Unify Dialog and MessageBox around one session, presenter and surface lifecycle, including UI dispatcher construction, async open/close, close veto, owner close and deterministic teardown.
  - Improve overlay and window presenter sizing, drag, resize, maximize, mask routing, focus restoration, cancellation and reentrancy behavior.
  - Reduce overlay dialog transition work.
- Navigation and Selection Controls
  - Steps: add `OutlineDot`, controlled navigation request events, deterministic step numbering/status/connector semantics, pointer-click wave behavior and new layout panels for horizontal, vertical and navigation layouts.
  - Cascader: support keyboard candidate navigation and stabilize expansion, filtering, selected option synchronization and popup state.
  - Menu and NavMenu: add submenu hover intent, support popups from detached title bars, add lifecycle-safe node commands and fix pointer or layout-induced submenu transitions.
  - Collapse and Expander: improve separator, accordion, content visibility and motion cleanup behavior.
- Data Entry, Display and General Controls
  - Add custom EmptyIndicator support for CascaderView, ListView and ListBox.
  - Add TransferItemDecorator item count handling for list and tree transfer views.
  - Fix AddOnDecoratedBox template-only add-on content, template-owned embedded TextBox inputs, embedded input padding, DatePicker and TimePicker input sizing, and NumericUpDown default input padding coverage.
  - Remove the ColorPicker spectrum color name tooltip and stabilize ImagePreviewer search and theme resources.
  - Fix Row/Splitter layout recovery and exact grid line wrapping.
- Feedback and Overlay Controls
  - Notification: render plain notifications without type icons, align close button hover/pressed states, theme the progress gradient from primary colors, set the default auto-close duration to 4.5 seconds and reduce internal card spacing.
  - Tour: correct target placement and animate the highlighted region.
- Icons, Gallery, Generator and Documentation
  - Add new SVG icons for AI, application and social/service scenarios.
  - Add a Windows rendering fallback for Gallery and refresh Gallery baselines, generated documentation and theme design documentation.
  - Add generated theme schema and control token resource metadata used by XML theme binding, token publication and optional control packages.
  - Update package metadata to `6.1.0`.

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
