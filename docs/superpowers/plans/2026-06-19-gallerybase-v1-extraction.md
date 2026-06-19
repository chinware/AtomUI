# GalleryBase V1 Extraction Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Extract the reusable Gallery controls into `AtomUI.Toolkits.GalleryBase` and make `AtomUIGallery` consume that library without changing Gallery behavior.

**Architecture:** V1 extracts the controls, token themes, runtime options, and supporting models only. Navigation and Shell stay in `AtomUIGallery` for this pass, but they should import GalleryBase controls through the new namespace. GalleryBase exposes the new neutral XAML namespace and the legacy Gallery namespace so existing product XAML can migrate incrementally.

**Tech Stack:** .NET 10, Avalonia, AtomUI Desktop controls, AtomUI.Generator, xUnit/Shouldly.

---

### Task 1: Add Extraction Guard Tests

**Files:**
- Create: `tests/AtomUIGallery.Tests/Toolkits/GalleryBaseExtractionTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Toolkits;

public class GalleryBaseExtractionTests
{
    [Fact]
    public void GalleryBase_Project_Is_Solution_Module_And_Owns_Gallery_Controls()
    {
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj")).ShouldBeTrue();
        ReadRepoFile("AtomUI.slnx").ShouldContain("src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj");

        var project = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj");
        project.ShouldContain("AtomUI.Desktop.Controls.csproj");
        project.ShouldContain("AtomUI.Generator.csproj");
        project.ShouldNotContain("controlgallery/AtomUIGallery");

        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/ShowCaseItem.axaml.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/ShowCasePanel.axaml.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsHost.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseScenarioController.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseRuntimeOptions.cs")).ShouldBeTrue();
    }
}
```

- [ ] **Step 2: Run focused tests and verify RED**

Run: `dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo --filter FullyQualifiedName~GalleryBaseExtractionTests /nr:false`

Expected: fail because `AtomUI.Toolkits.GalleryBase` does not exist yet.

### Task 2: Create GalleryBase Project

**Files:**
- Create: `src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj`
- Create: `src/AtomUI.Toolkits.GalleryBase/Properties/AssemblyInfo.cs`
- Modify: `AtomUI.slnx`

- [ ] **Step 1: Add project with AtomUI Desktop controls and generator references**
- [ ] **Step 2: Add neutral and legacy XAML namespace definitions**
- [ ] **Step 3: Add project to `AtomUI.slnx`**

### Task 3: Move Gallery Controls And Models

**Files:**
- Move: `controlgallery/AtomUIGallery/Controls/*` to `src/AtomUI.Toolkits.GalleryBase/Controls/*`
- Move: `controlgallery/AtomUIGallery/Models/*` to `src/AtomUI.Toolkits.GalleryBase/Models/*`
- Modify: moved namespaces and XAML `x:Class` declarations

- [ ] **Step 1: Move files mechanically**
- [ ] **Step 2: Rename namespaces to `AtomUI.Toolkits.GalleryBase.*`**
- [ ] **Step 3: Update theme model namespace references**

### Task 4: Make AtomUIGallery Consume GalleryBase

**Files:**
- Modify: `controlgallery/AtomUIGallery/AtomUIGallery.csproj`
- Modify: `controlgallery/AtomUIGallery/ShowCases/ShowCaseControlAliases.cs`
- Modify: `controlgallery/AtomUIGallery/ThemeManagerBuilderExtensions.cs`
- Modify: `controlgallery/AtomUIGallery/Properties/AssemblyInfo.cs`
- Modify: `controlgallery/AtomUIGallery/Workspace/**/*.cs`
- Modify: `controlgallery/AtomUIGallery.Browser/BrowserGalleryView.cs`
- Modify: tests importing `AtomUIGallery.Controls`

- [ ] **Step 1: Add project reference to GalleryBase**
- [ ] **Step 2: Replace `AtomUIGallery.Controls` imports with GalleryBase controls**
- [ ] **Step 3: Keep product language resource XML namespace mappings in `AtomUIGallery`**

### Task 5: Verify

- [ ] **Step 1: Run focused extraction tests**

Run: `dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo --filter FullyQualifiedName~GalleryBaseExtractionTests /nr:false`

Expected: pass.

- [ ] **Step 2: Run full Gallery tests**

Run: `dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false`

Expected: pass.

- [ ] **Step 3: Build Gallery desktop app**

Run: `dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --nologo -v:minimal /nr:false`

Expected: pass.
