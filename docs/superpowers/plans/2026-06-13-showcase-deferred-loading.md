# ShowCase Deferred Loading Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add opt-in deferred loading for heavy Gallery `ShowCaseItem` demo content and enable it for `LineEditShowCase`.

**Architecture:** `ShowCaseItem` owns optional deferred content declaration and one-shot materialization. `ShowCasePanel` owns the viewport-driven policy with one panel-level `EffectiveViewportChanged` listener and batches materialization for near-viewport items. Existing panels keep eager behavior because deferred loading is disabled by default.

**Tech Stack:** .NET 10, Avalonia 12, AtomUIGallery controls, xUnit, Gallery token themes.

---

### Task 1: Add RED tests

**Files:**
- Modify: `tests/AtomUIGallery.Tests/Controls/ShowCasePanelStructureTests.cs`
- Modify: `tests/AtomUIGallery.Tests/ShowCases/LineEditShowCasePageTests.cs`

- [ ] Add a runtime test proving `ShowCaseItem.DeferredContentTemplate` is not built before `MaterializeDeferredContent()`.
- [ ] Add structure tests proving `ShowCasePanel` exposes opt-in deferred loading properties and uses one panel-level `EffectiveViewportChanged` subscription.
- [ ] Add LineEdit tests proving the Examples panel opts in and the demo content is moved into `DeferredContentTemplate`.
- [ ] Run focused tests and confirm they fail for missing properties/markup.

### Task 2: Implement deferred item content

**Files:**
- Modify: `controlgallery/AtomUIGallery/Controls/ShowCaseItem.axaml.cs`
- Modify: `controlgallery/AtomUIGallery/Controls/ShowCaseItemTheme.axaml`
- Modify: `controlgallery/AtomUIGallery/Controls/ShowCaseItemToken.cs`

- [ ] Add `IsDeferredContentEnabled`, `DeferredContentTemplate`, `DeferredContent`, `DeferredPlaceholderHeight`, and read-only materialization state.
- [ ] Add `MaterializeDeferredContent()` as a one-shot method.
- [ ] Add token-backed placeholder styling in the item theme.
- [ ] Verify runtime test passes.

### Task 3: Implement panel-level deferred loading

**Files:**
- Modify: `controlgallery/AtomUIGallery/Controls/ShowCasePanel.axaml.cs`
- Modify: `controlgallery/AtomUIGallery/Controls/ShowCasePanelTheme.axaml`

- [ ] Add `IsDeferredLoadingEnabled`, `InitialDeferredLoadItemCount`, `DeferredLoadBatchSize`, and `DeferredLoadViewportBuffer`.
- [ ] Subscribe once to `EffectiveViewportChanged` when deferred loading is enabled.
- [ ] Materialize all deferred content when disabled; materialize initial and near-viewport items when enabled.
- [ ] Keep existing browser progressive shell mounting behavior intact.

### Task 4: Enable LineEdit trial

**Files:**
- Modify: `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml`
- Modify: `tests/AtomUIGallery.Tests/ShowCases/LineEditShowCasePageTests.cs`
- Modify: `tests/AtomUIGallery.Tests/ShowCases/LineEditShowCaseExamples.snapshot` only if the preservation extractor changes format.

- [ ] Set `IsDeferredLoadingEnabled="True"` on the LineEdit examples panel.
- [ ] Convert each LineEdit `ShowCaseItem` demo body into `DeferredContentTemplate` without changing demo controls.
- [ ] Update the preservation extractor so it still compares demo content, not shell markup.

### Task 5: Verify

- [ ] Run `dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo --filter FullyQualifiedName~ShowCasePanelStructureTests /nr:false`.
- [ ] Run `dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo --filter FullyQualifiedName~LineEditShowCasePageTests /nr:false`.
- [ ] Run `dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false`.
- [ ] Run `dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --nologo -v:minimal /nr:false`.
- [ ] Run `dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo /nr:false`.
- [ ] Run `git diff --check`.
