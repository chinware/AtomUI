# GalleryBase Foundation Completion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Complete the product-neutral GalleryBase foundation so AtomUI Gallery consumes shared configuration, route registration, and navigation runtime instead of owning those base concerns.

**Architecture:** GalleryBase will own neutral configuration, navigation tree, route registry, route validation, and a reusable navigation ViewModel. AtomUIGallery will become the first product module by explicitly registering branding, navigation nodes, route factories, and ReactiveUI view mappings while keeping product pages and assets in the product project.

**Tech Stack:** .NET 10, Avalonia, AtomUI Desktop Controls, ReactiveUI, xUnit v3, Shouldly.

---

### Task 1: GalleryBase Configuration, Navigation, And Route Tests

**Files:**
- Create: `tests/AtomUIGallery.Tests/Toolkits/GalleryBaseFoundationTests.cs`

- [ ] **Step 1: Write failing tests**

Add tests that cover:

```csharp
[Fact]
public void Route_Registry_Creates_ViewModel_And_Rejects_Duplicate_Routes()
{
    var registry = new GalleryRouteRegistry();
    registry.Map("Overview", screen => new TestRouteViewModel(screen), () => new TestRouteView());
    registry.CreateViewModel("Overview", new TestScreen()).ShouldBeOfType<TestRouteViewModel>();
    Should.Throw<GalleryConfigurationException>(() =>
        registry.Map("Overview", screen => new TestRouteViewModel(screen), () => new TestRouteView()));
}

[Fact]
public void Configuration_Requires_Default_Route_To_Be_Registered_Page()
{
    var options = new GalleryBaseOptions();
    options.Navigation.DefaultRoute = "Overview";
    options.Navigation.AddPage("Overview", "Overview");
    options.BuildConfiguration().DefaultRoute.ShouldBe("Overview");
}
```

- [ ] **Step 2: Run test to verify it fails**

Run:

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false --filter GalleryBaseFoundationTests
```

Expected: fail because `GalleryRouteRegistry`, `GalleryBaseOptions`, and `GalleryConfigurationException` do not exist.

### Task 2: GalleryBase Core Foundation Types

**Files:**
- Create: `src/AtomUI.Toolkits.GalleryBase/Configuration/GalleryConfigurationException.cs`
- Create: `src/AtomUI.Toolkits.GalleryBase/Configuration/GalleryBaseOptions.cs`
- Create: `src/AtomUI.Toolkits.GalleryBase/Configuration/GalleryBaseConfiguration.cs`
- Create: `src/AtomUI.Toolkits.GalleryBase/Navigation/GalleryNavigationBuilder.cs`
- Create: `src/AtomUI.Toolkits.GalleryBase/Navigation/GalleryNavigationNode.cs`
- Create: `src/AtomUI.Toolkits.GalleryBase/Routing/GalleryRouteRegistry.cs`
- Create: `src/AtomUI.Toolkits.GalleryBase/Routing/GalleryRouteDescriptor.cs`
- Modify: `src/AtomUI.Toolkits.GalleryBase/ThemeManagerBuilderExtensions.cs`

- [ ] **Step 1: Implement minimal core types**

Implement `EntityKey` based route keys, duplicate validation, immutable configuration snapshots, and `UseGalleryBase(Action<GalleryBaseOptions>?)`.

- [ ] **Step 2: Run tests to verify green**

Run:

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false --filter GalleryBaseFoundationTests
```

Expected: pass.

### Task 3: GalleryBase Navigation Runtime Tests

**Files:**
- Modify: `tests/AtomUIGallery.Tests/Toolkits/GalleryBaseFoundationTests.cs`

- [ ] **Step 1: Write failing navigation ViewModel tests**

Add tests that verify:

```csharp
[Fact]
public void Navigation_ViewModel_Does_Not_Navigate_Group_Or_Current_Route()
{
    var screen = new TestScreen();
    var configuration = CreateConfiguration();
    var viewModel = new GalleryNavigationViewModel(screen, configuration);
    viewModel.NavigateToCommand.Execute("Overview").Subscribe();
    viewModel.NavigateToCommand.Execute("Overview").Subscribe();
    screen.CreatedRoutes.ShouldBe(new[] { "Overview" });
}
```

- [ ] **Step 2: Run test to verify it fails**

Run the same filtered test command. Expected: fail because `GalleryNavigationViewModel` does not exist.

### Task 4: GalleryBase Navigation Runtime

**Files:**
- Create: `src/AtomUI.Toolkits.GalleryBase/Navigation/GalleryNavigationViewModel.cs`
- Create: `src/AtomUI.Toolkits.GalleryBase/Navigation/GalleryNavigationMenuAdapter.cs`

- [ ] **Step 1: Implement navigation ViewModel and NavMenu adapter**

Implement default route navigation, `SelectedItem`, `CanNavigateTo`, duplicate-current-route guard, group no-op behavior, and F5/F6 diagnostic commands.

- [ ] **Step 2: Run tests to verify green**

Run:

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false --filter GalleryBaseFoundationTests
```

Expected: pass.

### Task 5: AtomUIGallery Product Module Tests

**Files:**
- Create: `tests/AtomUIGallery.Tests/Toolkits/AtomUIGalleryModuleTests.cs`

- [ ] **Step 1: Write failing product module tests**

Add tests that verify AtomUI Gallery configuration:

```csharp
[Fact]
public void AtomUI_Gallery_Module_Registers_All_Navigation_Pages_As_Routes()
{
    var configuration = AtomUIGalleryModule.CreateConfiguration();
    foreach (var node in configuration.NavigationNodes.SelectMany(Walk))
    {
        if (node.IsRoute)
        {
            configuration.Routes.ContainsRoute(node.Key).ShouldBeTrue(node.Key.ToString());
        }
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run:

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false --filter AtomUIGalleryModuleTests
```

Expected: fail because `AtomUIGalleryModule` does not exist.

### Task 6: AtomUIGallery Product Module And View Registration

**Files:**
- Create: `controlgallery/AtomUIGallery/AtomUIGalleryModule.cs`
- Modify: `controlgallery/AtomUIGallery/ThemeManagerBuilderExtensions.cs`
- Modify: `controlgallery/AtomUIGallery/ShowCases/ShowCaseRegister.cs`
- Modify: `controlgallery/AtomUIGallery.Desktop/Program.cs`
- Modify: `controlgallery/AtomUIGallery.Browser/Program.cs`

- [ ] **Step 1: Implement product module**

Move product route factories and view mappings into `AtomUIGalleryModule`. Keep `ShowCaseViewModule` as a compatibility wrapper that delegates to the module.

- [ ] **Step 2: Run product module tests**

Run:

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false --filter AtomUIGalleryModuleTests
```

Expected: pass.

### Task 7: AtomUIGallery Navigation Consumption

**Files:**
- Modify: `controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs`
- Modify: `controlgallery/AtomUIGallery/Workspace/ViewModels/WorkspaceWindowViewModel.cs`
- Modify: `controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml`
- Modify: `controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml.cs`
- Modify: `controlgallery/AtomUIGallery.Browser/BrowserGalleryView.cs`
- Modify: `tests/AtomUIGallery.Tests/Workspace/CaseNavigationViewModelDiagnosticsTests.cs`
- Modify: `tests/AtomUIGallery.Tests/Workspace/BrowserGalleryConventionsTests.cs`

- [ ] **Step 1: Replace hardcoded navigation factories**

Make `CaseNavigationViewModel` inherit or wrap `GalleryNavigationViewModel`, using the product configuration instead of hardcoded route factories.

- [ ] **Step 2: Replace hardcoded NavMenu tree**

Make `CaseNavigation` populate `NavMenu` from `GalleryNavigationMenuAdapter`, keeping visual properties unchanged.

- [ ] **Step 3: Run workspace tests**

Run:

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false --filter "CaseNavigation|BrowserGalleryConventions"
```

Expected: pass after test expectations are updated for the GalleryBase runtime.

### Task 8: Verification

**Files:**
- No new files unless tests reveal necessary targeted fixes.

- [ ] **Step 1: Run targeted and project verification**

Run:

```bash
dotnet build src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj --nologo -v:minimal /nr:false
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --nologo -v:minimal /nr:false
dotnet build controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj --nologo -v:minimal /nr:false
git diff --check
```

Expected: all commands pass.
