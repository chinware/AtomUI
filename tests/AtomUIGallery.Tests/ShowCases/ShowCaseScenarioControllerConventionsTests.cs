using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ShowCaseScenarioControllerConventionsTests
{
    [Fact]
    public void Standard_ShowCases_Render_Examples_Directly_Without_Api_Or_Token_Tabs()
    {
        var standardShowCases = GetStandardDocumentedShowCaseFiles();
        standardShowCases.Count.ShouldBeGreaterThan(0);

        var failures = new List<string>();
        foreach (var pagePath in standardShowCases)
        {
            var relativePagePath = Path.GetRelativePath(GetRepoRoot(), pagePath);
            var source           = File.ReadAllText(pagePath);

            if (!source.Contains("<gallery:GalleryStickyTabsHost", StringComparison.Ordinal))
            {
                failures.Add($"{relativePagePath}: must keep the document-style GalleryStickyTabsHost shell.");
            }

            if (!source.Contains("<gallery:ShowCasePanel Name=\"ExamplesContent\"", StringComparison.Ordinal))
            {
                failures.Add($"{relativePagePath}: must render ExamplesContent as the direct showcase body.");
            }

            foreach (var removedMarker in new[]
                     {
                         "<gallery:GalleryStickyTabsHost.StickyContent>",
                         "Name=\"ScenarioTabs\"",
                         "Name=\"ScenarioContentHost\"",
                         "Tag=\"Api\"",
                         "Tag=\"DesignToken\"",
                         "ScenarioApi",
                         "ScenarioDesignToken"
                     })
            {
                if (source.Contains(removedMarker, StringComparison.Ordinal))
                {
                    failures.Add($"{relativePagePath}: must not declare obsolete scenario marker {removedMarker}.");
                }
            }

            if (source.Contains("ItemsSource=\"{Binding ApiRows}\"", StringComparison.Ordinal) ||
                source.Contains("ItemsSource=\"{Binding DesignTokenRows}\"", StringComparison.Ordinal))
            {
                failures.Add($"{relativePagePath}: must keep API and token DataGrids out of the main page XAML.");
            }
        }

        failures.ShouldBeEmpty();
    }

    [Fact]
    public void Standard_ShowCase_CodeBehind_Does_Not_Create_Api_Or_Token_Scenarios()
    {
        var standardShowCases = GetStandardDocumentedShowCaseFiles();
        standardShowCases.Count.ShouldBeGreaterThan(0);

        var failures = new List<string>();
        foreach (var pagePath in standardShowCases)
        {
            var relativePagePath = Path.GetRelativePath(GetRepoRoot(), pagePath);
            var codePath         = Path.ChangeExtension(pagePath, ".axaml.cs");
            if (!File.Exists(codePath))
            {
                failures.Add($"{relativePagePath}: code-behind file was not found.");
                continue;
            }

            var codeSource = File.ReadAllText(codePath);
            foreach (var removedMarker in new[]
                     {
                         "GalleryShowCaseScenarioController",
                         "_scenarioController",
                         "ApiScenario",
                         "DesignTokenScenario",
                         "CreateScenarioContent",
                         "ApiDataGrid()",
                         "DesignTokenDataGrid()"
                     })
            {
                if (codeSource.Contains(removedMarker, StringComparison.Ordinal))
                {
                    failures.Add($"{relativePagePath}: code-behind must not keep obsolete scenario marker {removedMarker}.");
                }
            }
        }

        failures.ShouldBeEmpty();
    }

    [Fact]
    public void Remaining_Scenario_ShowCases_Use_Shared_Scenario_Controller()
    {
        var helperSource = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseScenarioController.cs");

        helperSource.ShouldContain("SelectionChanged += HandleScenarioSelectionChanged");
        helperSource.ShouldContain("_lazyScenarioContentCache");
        helperSource.ShouldContain("EnsureSelectedScenarioContent");
        helperSource.ShouldContain("ClearLazyScenarioContent");
        helperSource.ShouldContain("UpdateDataContext");
        helperSource.ShouldContain("Attach(object? dataContext)");

        var failures = new List<string>();
        foreach (var pagePath in GetMigratedScenarioShowCaseFiles())
        {
            var relativePagePath = Path.GetRelativePath(GetRepoRoot(), pagePath);
            var codePath         = Path.ChangeExtension(pagePath, ".axaml.cs");
            if (!File.Exists(codePath))
            {
                failures.Add($"{relativePagePath}: code-behind file was not found.");
                continue;
            }

            var codeSource = File.ReadAllText(codePath);
            if (!codeSource.Contains("GalleryShowCaseScenarioController", StringComparison.Ordinal))
            {
                failures.Add($"{relativePagePath}: must use GalleryShowCaseScenarioController.");
            }

            if (!codeSource.Contains("_scenarioController.Attach(DataContext)", StringComparison.Ordinal))
            {
                failures.Add($"{relativePagePath}: must delegate attached scenario selection to the shared controller.");
            }

            if (!codeSource.Contains("_scenarioController.Detach()", StringComparison.Ordinal))
            {
                failures.Add($"{relativePagePath}: must clear lazy scenario content through the shared controller.");
            }

            if (!codeSource.Contains("_scenarioController.UpdateDataContext(DataContext)", StringComparison.Ordinal))
            {
                failures.Add($"{relativePagePath}: must synchronize DataContext through the shared controller.");
            }

            foreach (var duplicatedMember in new[]
                     {
                         "_lazyScenarioContentCache",
                         "HandleScenarioSelectionChanged",
                         "EnsureSelectedScenarioContent",
                         "ClearLazyScenarioContent",
                         "private Control ResolveScenarioContent("
                     })
            {
                if (codeSource.Contains(duplicatedMember, StringComparison.Ordinal))
                {
                    failures.Add($"{relativePagePath}: must not keep duplicated {duplicatedMember} implementation.");
                }
            }
        }

        failures.ShouldBeEmpty();
    }

    private static IReadOnlyList<string> GetMigratedScenarioShowCaseFiles()
    {
        var standardShowCases = new HashSet<string>(GetStandardDocumentedShowCaseFiles(), StringComparer.Ordinal);

        return Directory
               .EnumerateFiles(Path.Combine(GetRepoRoot(), "controlgallery/AtomUIGallery/ShowCases"),
                                "*ShowCase.axaml",
                                SearchOption.AllDirectories)
               .Where(path => !standardShowCases.Contains(path))
               .Where(path =>
               {
                   var source = File.ReadAllText(path);
                   return source.Contains("Name=\"ScenarioContentHost\"", StringComparison.Ordinal) &&
                          source.Contains("Name=\"ScenarioTabs\"", StringComparison.Ordinal);
               })
               .Order(StringComparer.Ordinal)
               .ToArray();
    }

    private static IReadOnlyList<string> GetStandardDocumentedShowCaseFiles()
    {
        return Directory
               .EnumerateFiles(Path.Combine(GetRepoRoot(), "controlgallery/AtomUIGallery/ShowCases"),
                                "*ShowCase.axaml",
                                SearchOption.AllDirectories)
               .Where(IsMainShowCasePage)
               .Where(path =>
               {
                   var controlName = Path.GetFileNameWithoutExtension(path)[..^"ShowCase".Length];
                   var viewsPath   = Path.GetDirectoryName(path)!;
                   return File.Exists(Path.Combine(viewsPath, $"{controlName}ApiDataGrid.axaml")) &&
                          File.Exists(Path.Combine(viewsPath, $"{controlName}DesignTokenDataGrid.axaml"));
               })
               .Order(StringComparer.Ordinal)
               .ToArray();
    }

    private static bool IsMainShowCasePage(string path)
    {
        var fileName    = Path.GetFileNameWithoutExtension(path);
        var controlName = Directory.GetParent(path)?.Parent?.Name;
        return string.Equals(fileName, $"{controlName}ShowCase", StringComparison.Ordinal);
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = Path.Combine(GetRepoRoot(), relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "controlgallery/AtomUIGallery/ShowCases");
            if (Directory.Exists(candidate))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return AppContext.BaseDirectory;
    }
}
