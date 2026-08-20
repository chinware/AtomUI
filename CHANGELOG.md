# Changelog

All notable changes to AtomUI are documented in this file.

`AtomUI` follows Semantic Versioning 2.0.0.

## 6.1.5

`2026-08-20`

- Dialog and MessageBox
  - Add `Dialog.IsMaskClosable` (default `true`) and `MessageBoxOptions.IsMaskClosable` to control whether pressing the modal mask closes the dialog. `IsClosable` continues to gate only the header close button; with `IsMaskClosable=false`, mask presses are ignored.
- ToolTip
  - Add `TextWrapping` (default `Wrap`) and `TextTrimming` attached properties so long tip text wraps within `ToolTipMaxWidth` instead of being clipped.
  - Allow a `ToolTip` instance used as `Tip` to override host presentation properties; presentation properties set on the instance take precedence and unset ones fall back to the host.
  - Treat `IsOpen` as a declarative desired-open state and reconcile it with tip readiness and host attachment instead of edge-triggered open-on-set behavior.
- Window and WindowTitleBar
  - Unify title-bar host behavior and CSD geometry across logical-tree title bars; content-area title bars receive caption, drag and double-click behavior without owning size hints.
  - Preserve `WindowDecorations.Full` when the AtomUI title bar is hidden, hiding only the drawn title-bar visual and avoiding a title-bar-height blank band.
- Candidate interaction
  - Unify pointer and keyboard candidate navigation for Select, AutoComplete, ComboBox, Cascader and Mentions so the highlighted item and `Enter` commit target stay consistent; pointer movement remains non-committing.

## 6.1.4

`2026-08-18`

- Breaking Changes
  - SearchEdit: replace `SearchButtonClick` with `SearchRequested`. Migrate handlers to `SearchRequestedEventArgs`, which exposes the query snapshot and `Button` / `EnterKey` trigger; Enter now requests a search by default and can be disabled with `IsSearchOnEnterEnabled=false`.
  - Localization: remove `ContractVersion` from `LanguageCatalogAttribute`, `LanguageCatalogDescriptor` and `TranslationBundleDescriptor`, including the descriptor constructor arguments; remove `AtomUILanguageContractVersion` and `AtomUIRequireVerifiedLanguageContract`; and replace the custom build-task path override `AtomUILocalizationBuildTasksAssembly` with `AtomUIBuildTasksAssembly`. Custom catalogs and language packages must use stable catalog keys and source fingerprints, then rebuild against 6.1.4. See [6.1.4 API change examples](docs/releases/6.1.4-api-changes.md).
- AOT, Generator and Build
  - Add compile-time linked registration for trimmed, NativeAOT and WebAssembly AOT applications. AtomUI now generates a static registration plan from C# and AXAML usage, preserves required control-package resources and uses bounded package-level fallbacks without runtime assembly scanning.
  - Add build-generated linked-registration sidecars and package manifests, keep publish analyzers out of ordinary builds, avoid implicit publish work in Gallery builds and reduce duplicate generated descriptor factories.
  - Add `ControlPackageRegistrationEntryAttribute` and package-granularity registration for third-party control packages, with opt-in directory granularity for independently trimmable control families.
  - Centralize the NuGet release manifest, include `AtomUI.Desktop.Controls.Extras` in extension package build, pack and artifact verification, and skip theme-asset generation when a project has no AXAML inputs.
- Window and Navigation
  - Add `Window.IsMinimizeCaptionButtonVisible` and `IsMaximizeCaptionButtonVisible`, completing independent visibility controls for minimize, maximize, close, fullscreen and pin caption buttons while keeping platform capabilities and window state authoritative.
  - Fix the Wayland startup title-bar flash, avoid retaining the complete Ant Design icon catalog in trimmed Window applications, and add tokenized spacing between the title-bar logo and left add-on.
  - Add configurable collapsed NavMenu tooltips with node-level content, header fallback, placement and delay controls.
  - Preserve `TabControl.HeaderTemplate` and `CardTabStrip.ItemTemplate` rendering when tab headers move into overflow menus. #430
- Data Entry and DataGrid
  - Unify pointer hover and keyboard navigation around one active Select candidate so the highlighted item and Enter commit target remain consistent across Single, Multiple and Tags modes.
  - Fix non-editable ComboBox keyboard focus after reopening so consecutive Down and Enter selections continue to update the selected item. #428
  - Fix DataGrid star columns when `ItemsSource` is empty by resolving widths from the active finite header viewport.
- Gallery
  - Add responsive content breakpoints, metadata layout and sidebar collapse behavior to the desktop and browser Gallery shells.
  - Add real-time Icon Gallery filtering and one-click icon-name copy with localized success or failure feedback.

## 6.1.3

`2026-08-08`

- Breaking Changes
  - Theme: custom theme and control packages must migrate from manually maintained token/theme registration to generated control-package descriptors and `Themes/**/*.axaml` asset manifests registered through `Application.UseAtomUI(...)` before the theme schema freezes. Replace string algorithm IDs in theme builders, attributes, descriptors and state with `ThemeAlgorithm` enum values.
  - Localization: replace the removed `LanguageCode`, `LanguageVariant`, language provider/pool and ThemeManager language registration APIs with `LanguageTag`, catalog enums, XLIFF 2.1 resources, `UseLanguages()` and generated module registration. Catalog enum member names and XLIFF unit IDs are now the stable contract identity; applications and static language packages must be rebuilt together and must not mix the former numeric-ID catalogs with key-based catalogs.
  - Upload: remove `Upload.IsOpenFileDialogOnClick` in favor of `UploadDropZone.IsOpenFileDialogOnClick` and `SourceKind`; replace `Upload.Accepts` with `AllowedFileTypes`; remove the old drop events and URI-only file contract; and migrate custom integrations to the typed source, input-batch, admission and completion contracts with `Files` as the single state owner.
  - Timeline: add horizontal `Orientation`, replace `TimelineMode.Left` / `Right` with logical `Start` / `End`, and rename `IndicatorLeftModeMargin` / `IndicatorRightModeMargin` to `IndicatorStartModeMargin` / `IndicatorEndModeMargin`.
  - Tag: replace `IsBordered` with `TagVariant` (`Filled`, `Solid` or `Outlined`), and use the new `CheckableTag` / `CheckableTagGroup` selection APIs for checkable tag scenarios.
  - Separator: change `SizeType` from `SizeType` to `CustomizableSizeType` and implement `ICustomizableSizeTypeAware`; `Custom` spacing is owned by the control instance or its containing style. See [6.1.3 API change examples](docs/releases/6.1.3-api-changes.md) for migration guidance.
- Calendar and DatePicker
  - Add the new Calendar control with date, month and year views, Mini and Fullscreen densities, custom headers and cells, week-number display, notices, range bars, disabled-date composition, keyboard navigation and automation support.
  - Add switchable DatePicker modes and `MinDate` / `MaxDate` constraints.
- Localization, Generator and Build
  - Replace runtime language providers with generated XLIFF 2.1 catalogs, strongly typed resource extensions, application bootstrap code and immutable language snapshots while preserving existing `{atom:XxxLangResource Key}` XAML usage.
  - Add static language-package build contracts, generated registration for active and dormant modules, verified and deferred package manifests, partial overrides, catalog validation and NativeAOT-safe registration without runtime assembly scanning or XLIFF parsing.
  - Add official Portuguese (Brazil) (`pt-BR`) packages for AtomUI modules and language switching in the Gallery.
- Theme
  - Compile theme definitions and generated control-theme manifests into complete immutable root and scoped snapshots, publish changes atomically, and cache equivalent definitions by normalized content digest.
  - Align Ant Design token semantics, complete control token snapshots and composite input states, including generated exact control identities, resource keys and effective Global/Own token access.
- Upload
  - Rebuild picker, directory picker, drag-and-drop and programmatic input around one cross-platform admission pipeline with typed file-source ownership, serialized batches and deterministic terminal states.
  - Add directory policies, traversal limits, count-overflow policy, snapshot error reporting and consistent `IsMultipleEnabled` behavior across picker and drop input.
  - Harden replacement, cancellation, cleanup and upload progress finalization so accepted sources remain valid until execution exits and updates stop after terminal status.
- Navigation and Selection Controls
  - Add `Menu.IsScrollEnabled` for scrollable popup menus.
  - Add hierarchical NavMenu composition, collapsible sidebar headers, improved inline-collapsed presentation and stale-selection cleanup when items are replaced.
  - Add vertical and round Segmented variants, keyboard navigation and dynamic option loading.
  - Add vertical OptionButtonGroup layout and horizontal Timeline layouts with logical Start/End/Alternate placement.
- Data Entry, Display and General Controls
  - Add Tag Filled/Solid/Outlined variants, preset/status/custom color handling, CheckableTag and single/multiple-selection CheckableTagGroup.
  - Add multi-handle Slider values with per-handle disabled state.
  - Add Button `IconWidth` and `IconHeight`, and Steps `ItemHeaderForeground`, `ItemSubHeaderForeground` and `ItemRailBackground` semantic styling properties.
  - Improve TextBox and TextArea OverflowTip decisions with text viewport metrics, restore Separator preset spacing, and add a Gallery form-in-Drawer example.
  - Add LunarCalendar with lunar date projection, solar terms, traditional festivals, holiday/workday markers and configurable weekend highlighting.
- Window, DataGrid, Gallery and Dependencies
  - Add the Window title-bar add-on facade and route ImagePreviewer toolbar requests correctly from client-side decoration overlays.
  - Fix DataGrid row-reorder reset crashes, collection moves and reorder lifecycle handling, while reducing allocations in the reorder path.
  - Include the Gallery Developer Tools runtime dependencies required by packaged builds.
  - Upgrade Avalonia from `12.1.0` to `12.1.1`; this patch dependency upgrade does not require AtomUI source migration.

## 6.1.2

`2026-07-27`

- Theme and Gallery
  - Add the shared `ThemePreference` enum with `Light`, `Dark` and `System` values.
  - Add Light, Dark and Follow System appearance modes to the Gallery. Follow System updates the Gallery theme when the operating system appearance changes.
- Dialog, Drawer and Window
  - Fix overlay Dialog and Drawer masks so they cover drawn window chrome without changing the window content geometry.
- Splitter
  - Fix measurement when the cross axis is unconstrained so Splitter returns a finite desired size.

## 6.1.1

`2026-07-25`

- Breaking Changes
  - ListView: replace the settable Avalonia `ISelectionModel` contract with a read-only `IListViewSelection` facade. Code that directly set `Selection`, `SelectedItem`, `SelectedItems` or `SelectedValue` should migrate to `SelectedIndex` or `Selection.Select` / `Selection.Deselect` / `Selection.Clear` / `Selection.SelectAll`; `SelectionChanged` now uses `ListViewSelectionChangedEventArgs`, and the old `ListCollectionViewChangedEventArgs` type has been removed. `ListItemData.IsSelected` has been removed, and `ListItemData`, `GroupListItemData` and `SelectOption` are now classes instead of records. See [6.1.1 API change examples](docs/releases/6.1.1-api-changes.md).
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
  - Dialog and MessageBox: rebuild the static display and host lifecycle around async sessions. Custom code that referenced `IDialogHost`, `IDialogHostProvider`, `IDialogActionResult` or `IMessageBoxActionResult`, or depended on callback-style host close APIs, should migrate to `ShowDialogAsync`, `ShowDialogModalAsync`, `ShowMessageBoxAsync`, `ShowMessageBoxModalAsync`, `Dialog.OpenAsync`, `Dialog.BeforeCloseAsync` and the returned `Task<object?>`. See [6.1.0 API change examples](docs/releases/6.1.0-api-changes.md).
  - Steps: replace the selection-based contract with controlled step state. Migrate `CurrentStep` to `Current`, `InitialStep` to `Initial`, `CurrentStepStatus` / `StepsItemStatus` to `Status` / `StepsStatus`, `Style` / `ItemIndicatorType` to `Type`, `LabelPlacement` to `TitlePlacement`, and `ProgressValue` / `IsShowItemProgress` to nullable `Percent`; move item descriptions from `Description` / `DescriptionTemplate` to `Content` / `ContentTemplate`, and handle clickable step changes with `CurrentChangeRequested` instead of relying on selection mutation. See [6.1.0 API change examples](docs/releases/6.1.0-api-changes.md).
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
  - ImagePreviewer: replace `SourceUri`, `SourceUris`, and `FallbackSourceUri` with `Source`, `Sources`, and `FallbackSource` based on `IImagePreviewSource`. Use `UriImagePreviewSource` for URI, local file and Avalonia resource images, and `StreamImagePreviewSource` for lazy stream sources. See [6.0.8 API change examples](docs/releases/6.0.8-api-changes.md).
  - Upload: replace the old `IsUploadDirectoryEnabled`, `IsShowUploadTrigger`, and `DefaultTaskList` composition model with composable `UploadTrigger`, `UploadDropZone`, and `Files`. See [6.0.8 API change examples](docs/releases/6.0.8-api-changes.md).
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
