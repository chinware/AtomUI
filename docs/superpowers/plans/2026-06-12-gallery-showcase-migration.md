# Gallery ShowCase Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor every control ShowCase page to follow the new Gallery ShowCase design pattern, with exactly one ShowCase migrated, verified, and accepted at a time.

**Architecture:** ButtonShowCase is the reference implementation. Each standard migrated control page uses `GalleryStickyTabsHost` for page-level scrolling and sticky scenario navigation, `TabStrip + ContentControl` for scenarios, `ShowCasePanel` masonry cards for examples, deferred `ShowCaseItem` demo content creation, and lazily loaded `DataGrid` UserControls for API and Design Token reference tabs. Special pages that own their own large internal viewport, such as Icon, must avoid a nested page scroll host and let the inner content control own scrolling. The migration must preserve existing demo content inside every `ShowCaseItem`.

**Tech Stack:** .NET 10, Avalonia, AtomUI Desktop controls, AtomUIGallery, xUnit, Gallery custom controls and token-based themes.

---

## Progress Contract

This file is the progress source of truth for the all-ShowCase migration.

- A ShowCase is not complete until its row is changed to `Accepted`.
- Only one ShowCase row may be `In Progress` at any time.
- A ShowCase may be implemented only after its row is selected as the current task.
- Do not migrate multiple ShowCases in one implementation pass.
- Do not mark the next ShowCase `In Progress` until the current ShowCase is accepted by the user.
- Every migration must keep `ShowCaseItem` internal demo content unchanged.
- Every migrated ShowCase must use deferred `ShowCaseItem` demo content creation; enabling only `ShowCasePanel.IsDeferredLoadingEnabled` is not enough.
- Any requested change to actual demo content must be handled as a separate task outside this migration plan.
- Do not modify `Window`, `NavMenu`, or global shell behavior to solve a ShowCase layout problem.
- Do not add private per-page visual systems when the behavior belongs in Gallery controls or Gallery tokens.

## Status Values

| Status | Meaning | Who Updates |
|---|---|---|
| `Not Started` | No migration work has started for this ShowCase. | Codex |
| `In Progress` | The ShowCase is the only active migration target. | Codex |
| `Implemented` | Code changes are done and local verification passed. | Codex |
| `Accepted` | User reviewed the result and approved the ShowCase. | User confirmation, then Codex updates this file |
| `Blocked` | Migration cannot continue without a decision or prerequisite fix. | Codex |

## Global Acceptance Gate

Every ShowCase must pass this gate before its progress row can become `Implemented`.

| Gate | Required Evidence |
|---|---|
| Structure | Root page uses `GalleryStickyTabsHost`; scenarios use `TabStrip + ContentControl`; Examples use `ShowCasePanel IsScrollEnabled="False"`. |
| Deferred item creation | Examples use `ShowCasePanel IsDeferredLoadingEnabled="True"`; every demo `ShowCaseItem` uses `IsDeferredContentEnabled="True"` and `DeferredContentTemplate`; each deferred `DataTemplate` declares the current ShowCase ViewModel `x:DataType`; no migrated demo content remains as direct `ShowCaseItem.Content`. |
| Demo preservation | Snapshot confirms `ShowCaseItem` internal demo content did not change during layout migration; any former code-behind-only behavior wiring moved into explicit VM binding/event forwarding is recorded in the snapshot as the equivalent runtime structure. |
| Lazy reference tabs | API and Design Token are separate UserControls and are created only when their tabs are selected. |
| DataGrid behavior | API and Design Token use DataGrid, keep the first key/name column fixed when needed, and own their horizontal scrolling. |
| Spacing and scrolling | Header, TabStrip, and content left/right edges align; page scrollbar stays at the far right; no nested page scrollbar appears in Examples. Special full-viewport pages must have exactly one vertical scrollbar, owned by the control that actually scrolls the large content. |
| Build and tests | Focused Gallery tests pass, then Gallery Desktop builds. |
| User review | A screenshot or live app check is reviewed before marking `Accepted`. |

## Standard Verification Commands

Run focused tests during each ShowCase migration:

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false
```

Run full Gallery Desktop build before asking for acceptance:

```bash
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --nologo -v:minimal /nr:false
```

Run desktop controls regression tests after complex interactive ShowCases:

```bash
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo /nr:false
```

Run whitespace validation before each commit:

```bash
git diff --check
```

## Reference Files

| File | Responsibility |
|---|---|
| `docs/gallery/gallery-showcase-design-pattern.md` | Normative Gallery ShowCase design pattern. |
| `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml` | Reference migrated ShowCase page. |
| `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml.cs` | Reference scenario switching and lazy cache implementation. |
| `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonApiDataGrid.axaml` | Reference lazy API DataGrid view. |
| `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonDesignTokenDataGrid.axaml` | Reference lazy Design Token DataGrid view. |
| `tests/AtomUIGallery.Tests/ShowCases/ButtonShowCasePageTests.cs` | Reference structure and lazy loading tests. |
| `tests/AtomUIGallery.Tests/ShowCases/ButtonShowCaseExamples.snapshot` | Reference demo content snapshot. |
| `controlgallery/AtomUIGallery/Controls/GalleryStickyTabsHost.cs` | Page host that owns document scrolling and sticky tabs. |
| `controlgallery/AtomUIGallery/Controls/ShowCasePanel.cs` | Examples host; migrated pages set `IsScrollEnabled="False"`. |
| `controlgallery/AtomUIGallery/Controls/ShowCaseMasonryPanel.cs` | Masonry layout implementation for ShowCase examples. |

## Per-ShowCase Migration Steps

Use this exact sequence for each ShowCase row.

- [ ] **Step 1: Select one ShowCase row**

  Change its status from `Not Started` to `In Progress`.

- [ ] **Step 2: Snapshot current examples**

  Add or update the selected ShowCase snapshot under `tests/AtomUIGallery.Tests/ShowCases/`. The snapshot scope is the Examples `ShowCaseItem` content only, not the new page shell.

- [ ] **Step 3: Add failing structure tests**

  Add tests for the selected ShowCase under `tests/AtomUIGallery.Tests/ShowCases/`. The tests must assert `GalleryStickyTabsHost`, `TabStrip`, `ScenarioContentHost`, `ShowCasePanel IsScrollEnabled=False`, `ShowCasePanel IsDeferredLoadingEnabled=True`, every demo `ShowCaseItem` uses `IsDeferredContentEnabled=True`, `DeferredContentTemplate`, typed deferred `DataTemplate`, no code-behind dependency on template-internal `Name` fields, and lazy API/Design Token creation.

- [ ] **Step 4: Migrate the root page**

  Wrap the root ShowCase in `GalleryStickyTabsHost`, add Header, move existing examples into the Examples scenario, enable deferred loading on `ShowCasePanel`, move each original demo subtree into typed `ShowCaseItem.DeferredContentTemplate`, and keep all original `ShowCaseItem` demo content behavior unchanged. Former code-behind field wiring must become VM binding or template-local event forwarding because deferred templates have their own NameScope.

- [ ] **Step 5: Add lazy DataGrid views**

  Add the selected ShowCase's API and Design Token DataGrid views in its `Views` folder, following the Button reference naming pattern.

- [ ] **Step 6: Wire scenario switching**

  Update the selected ShowCase root code-behind to cache lazy views, synchronize `DataContext`, and clear cache on detach.

- [ ] **Step 7: Run verification**

  Run the standard verification commands listed above. For low-risk single-page ShowCases, `AtomUIGallery.Tests` plus Gallery Desktop build is sufficient. For complex interactive ShowCases, also run desktop controls regression tests.

- [ ] **Step 8: Mark implemented and request review**

  Change the ShowCase status to `Implemented`, summarize changed files, include verification results, and ask for user acceptance. Stop here until the user accepts or requests changes.

- [ ] **Step 9: Mark accepted**

  Only after user approval, change the ShowCase status to `Accepted`. Then select the next row.

## Strict Sequential Plan

There is no grouped execution. The sections below are only a recommended queue order to reduce risk. Execution is strictly one root ShowCase row at a time.

If a selected root ShowCase folder contains sub-scenario files such as `CardBasicShowCase.axaml` or `FormValidationShowCase.axaml`, those files are part of that one selected root ShowCase migration. They must not be used as a reason to start another root ShowCase before user acceptance.

| Queue Section | Goal | Commit Rule | Acceptance Rule |
|---|---|---|---|
| P0 Foundation | Keep the migration plan, test conventions, and Button reference stable. | Commit only after the active row is accepted. | User confirms the active row. |
| P1 Low-Risk Single-Page Controls | Migrate simple pages with one root ShowCase and limited interaction. | Commit only after the active ShowCase is accepted, unless the user asks to defer commits. | User accepts the active ShowCase before any next ShowCase starts. |
| P2 Medium Controls | Migrate pages with richer states, grouped examples, or multiple sub-showcase files. | Commit only after the active ShowCase is accepted, unless the user asks to defer commits. | User accepts the active ShowCase before any next ShowCase starts. |
| P3 High-Risk Complex Controls | Migrate DataGrid, Form, Menu, Tree, Select, layout playground, and popup-heavy controls. | Commit only after the active ShowCase is accepted, unless the user asks to defer commits. | User accepts the active ShowCase before any next ShowCase starts. |
| P4 Special Pages | Migrate non-standard pages such as Icon, Palette, CustomizeTheme, ImagePreviewer, and Tour. | Commit only after the active ShowCase is accepted, unless the user asks to defer commits. | User accepts the active ShowCase before any next ShowCase starts. |
| P5 Cleanup | Remove duplicated migration scaffolding, update docs, run full verification. | Commit only after the active cleanup row is accepted. | User confirms the active cleanup row. |

## Progress Table

### P0 Foundation

| ID | Status | ShowCase | Files | Acceptance |
|---|---|---|---|---|
| P0.1 | Accepted | ButtonShowCase reference implementation | `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml`; `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml.cs`; `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonApiDataGrid.axaml`; `controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonDesignTokenDataGrid.axaml`; `tests/AtomUIGallery.Tests/ShowCases/ButtonShowCasePageTests.cs`; `tests/AtomUIGallery.Tests/ShowCases/ButtonShowCaseExamples.snapshot` | Existing reference. Commit `289f0f674 gallery: redesign gallery shell and showcase pages`. |
| P0.2 | Accepted | Migration progress plan | `docs/superpowers/plans/2026-06-12-gallery-showcase-migration.md` | Accepted by user request to start the first ShowCase migration. |
| P0.3 | Not Started | Shared migration test helper review | `tests/AtomUIGallery.Tests/ShowCases/` | Decide whether to extract reusable helpers after two more ShowCases are migrated. |

### P1 Low-Risk Single-Page Controls

| ID | Status | ShowCase | Component Folder | Current Views | Verification |
|---|---|---|---|---|---|
| P1.1 | Accepted | Avatar | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Avatar` | `AvatarShowCase.axaml`; `AvatarApiDataGrid.axaml`; `AvatarDesignTokenDataGrid.axaml` | Accepted by user; API/Token DataGrid spacing and star description column fixed; `AtomUIGallery.Tests` passed; `AtomUI.Desktop.Controls.DataGrid.Tests` passed; `AtomUI.Desktop.Controls.Tests` passed; Gallery Desktop build passed; Desktop launch smoke check passed. |
| P1.2 | Accepted | Badge | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge` | `BadgeShowCase.axaml`; `BadgeApiDataGrid.axaml`; `BadgeDesignTokenDataGrid.axaml` | Accepted by user; structure + snapshot + lazy tabs passed; sticky tabs keep the real TabStrip inline and draw a non-hit-test overlay mirror while pinned so Badge adorners cannot cover the TabStrip. |
| P1.3 | Accepted | Calendar | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar` | `CalendarShowCase.axaml`; `CalendarApiDataGrid.axaml`; `CalendarDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.4 | Accepted | Descriptions | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Descriptions` | `DescriptionsShowCase.axaml`; `DescriptionsApiDataGrid.axaml`; `DescriptionsDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.5 | Accepted | Empty | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Empty` | `EmptyShowCase.axaml`; `EmptyApiDataGrid.axaml`; `EmptyDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.6 | Accepted | GroupBox | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/GroupBox` | `GroupBoxShowCase.axaml`; `GroupBoxApiDataGrid.axaml`; `GroupBoxDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.7 | Accepted | QRCode | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode` | `QRCodeShowCase.axaml`; `QRCodeApiDataGrid.axaml`; `QRCodeDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.8 | Accepted | Segmented | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented` | `SegmentedShowCase.axaml`; `SegmentedApiDataGrid.axaml`; `SegmentedDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.9 | Accepted | Statistic | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Statistic` | `StatisticShowCase.axaml`; `StatisticApiDataGrid.axaml`; `StatisticDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.10 | Accepted | Tag | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag` | `TagShowCase.axaml`; `TagApiDataGrid.axaml`; `TagDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.11 | Accepted | Timeline | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline` | `TimelineShowCase.axaml`; `TimelineApiDataGrid.axaml`; `TimelineDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.12 | Accepted | Tooltip | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tooltip` | `TooltipShowCase.axaml`; `TooltipApiDataGrid.axaml`; `TooltipDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed. |
| P1.13 | Accepted | Alert | `controlgallery/AtomUIGallery/ShowCases/Feedback/Alert` | `AlertShowCase.axaml`; `AlertApiDataGrid.axaml`; `AlertDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery Desktop build passed; shared sticky host clipping bug fixed. |
| P1.14 | Accepted | Result | `controlgallery/AtomUIGallery/ShowCases/Feedback/Result` | `ResultShowCase.axaml`; `ResultApiDataGrid.axaml`; `ResultDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.15 | Accepted | Skeleton | `controlgallery/AtomUIGallery/ShowCases/Feedback/Skeleton` | `SkeletonShowCase.axaml`; `SkeletonApiDataGrid.axaml`; `SkeletonDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.16 | Accepted | Spin | `controlgallery/AtomUIGallery/ShowCases/Feedback/Spin` | `SpinShowCase.axaml`; `SpinApiDataGrid.axaml`; `SpinDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.17 | Accepted | Watermark | `controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark` | `WatermarkShowCase.axaml`; `WatermarkApiDataGrid.axaml`; `WatermarkDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.18 | Accepted | Separator | `controlgallery/AtomUIGallery/ShowCases/General/Separator` | `SeparatorShowCase.axaml`; `SeparatorApiDataGrid.axaml`; `SeparatorDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.19 | Accepted | Breadcrumb | `controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb` | `BreadcrumbShowCase.axaml`; `BreadcrumbApiDataGrid.axaml`; `BreadcrumbDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.20 | Accepted | Pagination | `controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination` | `PaginationShowCase.axaml`; `PaginationApiDataGrid.axaml`; `PaginationDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.21 | Accepted | Rate | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Rate` | `RateShowCase.axaml`; `RateApiDataGrid.axaml`; `RateDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.22 | Accepted | Slider | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider` | `SliderShowCase.axaml`; `SliderApiDataGrid.axaml`; `SliderDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.23 | Accepted | ToggleSwitch | `controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch` | `ToggleSwitchShowCase.axaml`; `ToggleSwitchApiDataGrid.axaml`; `ToggleSwitchDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |
| P1.24 | Accepted | Splitter | `controlgallery/AtomUIGallery/ShowCases/Layout/Splitter` | `SplitterShowCase.axaml`; `SplitterApiDataGrid.axaml`; `SplitterDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed. |

### P2 Medium Controls

| ID | Status | ShowCase | Component Folder | Current Views | Verification |
|---|---|---|---|---|---|
| P2.1 | Accepted | Card | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Card` | `CardShowCase.axaml`; `CardApiDataGrid.axaml`; `CardDesignTokenDataGrid.axaml`; `CardBasicShowCase.axaml`; `CardAdvancedShowCase.axaml`; `CardLayoutShowCase.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; `AtomUIGallery.Tests` passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.2 | Accepted | Carousel | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Carousel` | `CarouselShowCase.axaml`; `CarouselApiDataGrid.axaml`; `CarouselDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; `AtomUIGallery.Tests` passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.3 | Accepted | Collapse | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Collapse` | `CollapseShowCase.axaml`; `CollapseApiDataGrid.axaml`; `CollapseDesignTokenDataGrid.axaml`; `CollapseBasicShowCase.axaml`; `CollapseAppearanceShowCase.axaml`; `CollapseBehaviorShowCase.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; `AtomUIGallery.Tests` passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.4 | Accepted | Expander | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Expander` | `ExpanderShowCase.axaml`; `ExpanderApiDataGrid.axaml`; `ExpanderDesignTokenDataGrid.axaml`; `ExpanderBasicShowCase.axaml`; `ExpanderAppearanceShowCase.axaml`; `ExpanderBehaviorShowCase.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; `AtomUIGallery.Tests` passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.5 | Accepted | List | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/List` | `ListShowCase.axaml`; `ListApiDataGrid.axaml`; `ListDesignTokenDataGrid.axaml`; `ListBasicShowCase.axaml`; `ListAdvancedShowCase.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; `AtomUIGallery.Tests` passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.6 | Accepted | InfoFlyout | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout` | `InfoFlyoutShowCase.axaml`; `InfoFlyoutApiDataGrid.axaml`; `InfoFlyoutDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; `AtomUIGallery.Tests` passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.7 | Accepted | AutoComplete | `controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete` | `AutoCompleteShowCase.axaml`; `AutoCompleteApiDataGrid.axaml`; `AutoCompleteDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; `AtomUIGallery.Tests` passed; Gallery Desktop build passed; `AtomUI.Desktop.Controls.Tests` passed; `git diff --check` passed. |
| P2.8 | Accepted | CheckBox | `controlgallery/AtomUIGallery/ShowCases/DataEntry/CheckBox` | `CheckBoxShowCase.axaml`; `CheckBoxApiDataGrid.axaml`; `CheckBoxDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.9 | Accepted | ColorPicker | `controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker` | `ColorPickerShowCase.axaml`; `ColorPickerApiDataGrid.axaml`; `ColorPickerDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed. |
| P2.10 | Accepted | DatePicker | `controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker` | `DatePickerShowCase.axaml`; `DatePickerApiDataGrid.axaml`; `DatePickerDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed. |
| P2.11 | Accepted | LineEdit | `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit` | `LineEditShowCase.axaml`; `LineEditApiDataGrid.axaml`; `LineEditDesignTokenDataGrid.axaml`; `LineEditBasicShowCase.axaml`; `LineEditSearchShowCase.axaml`; `LineEditStateShowCase.axaml`; `LineEditTextAreaShowCase.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by review. |
| P2.12 | Accepted | Mentions | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions` | `MentionsShowCase.axaml`; `MentionsApiDataGrid.axaml`; `MentionsDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P2.13 | Accepted | NumberUpDown | `controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown` | `NumberUpDownShowCase.axaml`; `NumberUpDownApiDataGrid.axaml`; `NumberUpDownDesignTokenDataGrid.axaml`; old Basic/Range/Style/Addon sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P2.14 | Accepted | RadioButton | `controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton` | `RadioButtonShowCase.axaml`; `RadioButtonApiDataGrid.axaml`; `RadioButtonDesignTokenDataGrid.axaml`; old Basic/Groups/Options/Styles sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P2.15 | Accepted | TimePicker | `controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker` | `TimePickerShowCase.axaml`; `TimePickerApiDataGrid.axaml`; `TimePickerDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P2.16 | Accepted | Upload | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload` | `UploadShowCase.axaml`; `UploadApiDataGrid.axaml`; `UploadDesignTokenDataGrid.axaml`; old Basic/Pictures/Constraints sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P2.17 | Accepted | ProgressBar | `controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar` | `ProgressBarShowCase.axaml`; `ProgressBarApiDataGrid.axaml`; `ProgressBarDesignTokenDataGrid.axaml`; old Basic/Advanced/Layout sub-showcase files merged into root and removed | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.18 | Accepted | FloatButton | `controlgallery/AtomUIGallery/ShowCases/General/FloatButton` | `FloatButtonShowCase.axaml`; `FloatButtonApiDataGrid.axaml`; `FloatButtonDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed. |
| P2.19 | Accepted | SplitButton | `controlgallery/AtomUIGallery/ShowCases/General/SplitButton` | `SplitButtonShowCase.axaml`; `SplitButtonApiDataGrid.axaml`; `SplitButtonDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to continue the migration. |
| P2.20 | Accepted | ButtonSpinner | `controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner` | `ButtonSpinnerShowCase.axaml`; `ButtonSpinnerApiDataGrid.axaml`; `ButtonSpinnerDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the remaining ShowCase migration. |
| P2.21 | Accepted | ComboBox | `controlgallery/AtomUIGallery/ShowCases/Navigation/ComboBox` | `ComboBoxShowCase.axaml`; `ComboBoxApiDataGrid.axaml`; `ComboBoxDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the remaining ShowCase migration. |
| P2.22 | Accepted | DropdownButton | `controlgallery/AtomUIGallery/ShowCases/Navigation/DropdownButton` | `DropdownButtonShowCase.axaml`; `DropdownButtonApiDataGrid.axaml`; `DropdownButtonDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the remaining ShowCase migration. |
| P2.23 | Accepted | TabControl | `controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl` | `TabControlShowCase.axaml`; `TabControlApiDataGrid.axaml`; `TabControlDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; accepted by user request to start the remaining ShowCase migration. |
| P2.24 | Accepted | TabStrip | `controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip` | `TabStripShowCase.axaml`; `TabStripApiDataGrid.axaml`; `TabStripDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the remaining ShowCase migration. |

### P3 High-Risk Complex Controls

| ID | Status | ShowCase | Component Folder | Current Views | Verification |
|---|---|---|---|---|---|
| P3.1 | Accepted | DataGrid | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid` | `DataGridShowCase.axaml`; `DataGridApiDataGrid.axaml`; `DataGridDesignTokenDataGrid.axaml`; old Basic/Structure/Interaction/Fixed/Editing/Filtering/Paging/Drag sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; DataGrid controls tests passed; `git diff --check` passed; accepted by user request to continue the next ShowCase. |
| P3.2 | Accepted | TreeView | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/TreeView` | `TreeViewShowCase.axaml`; `TreeViewApiDataGrid.axaml`; `TreeViewDesignTokenDataGrid.axaml`; old Basic/Advanced sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.3 | Accepted | Cascader | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader` | `CascaderShowCase.axaml`; `CascaderApiDataGrid.axaml`; `CascaderDesignTokenDataGrid.axaml`; old Basic/Multiple/Advanced/View sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.4 | Accepted | Form | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Form` | `FormShowCase.axaml`; `FormApiDataGrid.axaml`; `FormDesignTokenDataGrid.axaml`; old Basic/Controls/Dynamic/Layout/Preset/State/Validation sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.5 | Accepted | Select | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Select` | `SelectShowCase.axaml`; `SelectApiDataGrid.axaml`; `SelectDesignTokenDataGrid.axaml`; old Basic/Options/Appearance sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.6 | Accepted | Transfer | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Transfer` | `TransferShowCase.axaml`; `TransferApiDataGrid.axaml`; `TransferDesignTokenDataGrid.axaml`; old Basic/Advanced/TreeStatus sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.7 | Accepted | TreeSelect | `controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect` | `TreeSelectShowCase.axaml`; `TreeSelectApiDataGrid.axaml`; `TreeSelectDesignTokenDataGrid.axaml`; old Basic/Behavior/Appearance sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.8 | Accepted | FlexPanel | `controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel` | `FlexPanelShowCase.axaml`; `FlexPanelApiDataGrid.axaml`; `FlexPanelDesignTokenDataGrid.axaml`; old Basic/Alignment/Item/Combination/Playground sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; FlexPanel runtime regressions covered for bare Avalonia `Slider`, deferred template logical-tree lookup, and blue demo tile white foreground. |
| P3.9 | Accepted | Grid | `controlgallery/AtomUIGallery/ShowCases/Layout/Grid` | `GridShowCase.axaml`; `GridApiDataGrid.axaml`; `GridDesignTokenDataGrid.axaml`; old Basic/Spacing/Alignment/Order/ColInfo sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.10 | Accepted | Space | `controlgallery/AtomUIGallery/ShowCases/Layout/Space` | `SpaceShowCase.axaml`; `SpaceApiDataGrid.axaml`; `SpaceDesignTokenDataGrid.axaml`; old Basic/Align/CompactButton/CompactForm/Size sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P3.11 | Accepted | Menu | `controlgallery/AtomUIGallery/ShowCases/Navigation/Menu` | `MenuShowCase.axaml`; `MenuApiDataGrid.axaml`; `MenuDesignTokenDataGrid.axaml`; old Basic/Features/ItemsSource/Context/Navigation sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; accepted by user request to start the next ShowCase. |
| P3.12 | Accepted | Steps | `controlgallery/AtomUIGallery/ShowCases/Navigation/Steps` | `StepsShowCase.axaml`; `StepsApiDataGrid.axaml`; `StepsDesignTokenDataGrid.axaml`; old Basic/Interactive/Vertical/DotClickable/Navigation/Progress/Inline sub-showcase files merged into root and removed | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; accepted by user request to start the next ShowCase. |
| P3.13 | Accepted | Drawer | `controlgallery/AtomUIGallery/ShowCases/Feedback/Drawer` | `DrawerShowCase.axaml`; `DrawerApiDataGrid.axaml`; `DrawerDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; nested drawer stale-container regression fixed at control level. |
| P3.14 | Accepted | Message | `controlgallery/AtomUIGallery/ShowCases/Feedback/Message` | `MessageShowCase.axaml`; `MessageApiDataGrid.axaml`; `MessageDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed. |
| P3.15 | Accepted | Modal | `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal` | `ModalShowCase.axaml`; `ModalApiDataGrid.axaml`; `ModalDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed. |
| P3.16 | Accepted | Notification | `controlgallery/AtomUIGallery/ShowCases/Feedback/Notification` | `NotificationShowCase.axaml`; `NotificationApiDataGrid.axaml`; `NotificationDesignTokenDataGrid.axaml` | Accepted by user request to start the next ShowCase; structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed. |
| P3.17 | Accepted | PopupConfirm | `controlgallery/AtomUIGallery/ShowCases/Feedback/PopupConfirm` | `PopupConfirmShowCase.axaml`; `PopupConfirmApiDataGrid.axaml`; `PopupConfirmDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |

### P4 Special Pages

| ID | Status | ShowCase | Component Folder | Current Views | Verification |
|---|---|---|---|---|---|
| P4.1 | Accepted | CustomizeTheme | `controlgallery/AtomUIGallery/ShowCases/General/CustomizeTheme` | `CustomizeThemeShowCase.axaml`; `CustomizeThemeApiDataGrid.axaml`; `CustomizeThemeDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P4.2 | Accepted | Icon | `controlgallery/AtomUIGallery/ShowCases/General/Icon` | `IconShowCase.axaml` | Special full-viewport layout keeps Header/TabStrip outside scroll and lets IconGallery own the only vertical scrollbar; structure + icon theme snapshot + lazy IconGallery creation passed; accepted by user request to start the next ShowCase. |
| P4.3 | Accepted | Palette | `controlgallery/AtomUIGallery/ShowCases/General/Palette` | `PaletteShowCase.axaml` | Special palette page uses GalleryStickyTabsHost page scrolling with lazy Light/Dark palette content templates; Palette structure + snapshot tests passed; Gallery tests passed; Gallery Desktop build passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P4.4 | Accepted | ImagePreviewer | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer` | `ImagePreviewerShowCase.axaml`; `ImagePreviewerApiDataGrid.axaml`; `ImagePreviewerDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; accepted by user request to start the next ShowCase. |
| P4.5 | Implemented | Tour | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tour` | `TourShowCase.axaml`; `TourApiDataGrid.axaml`; `TourDesignTokenDataGrid.axaml` | Structure + snapshot + lazy tabs passed; Gallery tests passed; Gallery Desktop build passed; desktop controls tests passed; `git diff --check` passed; awaiting user acceptance. |

### P5 Cleanup And Final Verification

| ID | Status | Scope | Files | Acceptance |
|---|---|---|---|---|
| P5.1 | Not Started | Remove duplicated per-page migration code if a shared helper is justified | `controlgallery/AtomUIGallery/ShowCases/**/Views/*ShowCase.axaml.cs`; `tests/AtomUIGallery.Tests/ShowCases/` | Only extract after repeated code appears in at least three migrated ShowCases. |
| P5.2 | Not Started | Update Gallery design docs | `docs/gallery/gallery-showcase-design-pattern.md`; `docs/superpowers/plans/2026-06-12-gallery-showcase-migration.md` | Docs match final implementation and list all accepted ShowCases. |
| P5.3 | Not Started | Full verification | Entire Gallery and desktop controls test scope | `AtomUIGallery.Tests`, `AtomUI.Desktop.Controls.Tests`, Gallery Desktop build, and `git diff --check` all pass. |

## Non-Component Pages

These pages are intentionally excluded from the control ShowCase migration table because they are Gallery landing/community pages, not control reference pages.

| Page | Folder | Handling |
|---|---|---|
| Overview | `controlgallery/AtomUIGallery/ShowCases/General/Overview` | Keep under Gallery landing page work. |
| Community | `controlgallery/AtomUIGallery/ShowCases/General/Community` | Keep under Gallery community page work. |
| AboutUs | `controlgallery/AtomUIGallery/ShowCases/General/AboutUs` | Keep under Gallery informational page work. |

## Review Protocol

For every ShowCase, Codex must report:

- ShowCase ID and status.
- Files changed.
- Whether `ShowCaseItem` demo snapshot changed.
- Verification commands and results.
- Known visual risk, if any.
- Whether the row is ready for user acceptance.

The user then either accepts the ShowCase or gives corrections. The next ShowCase cannot start until the current row is accepted and this progress table is updated.
