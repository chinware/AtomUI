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
    public void Migrated_ShowCases_Use_Shared_Scenario_Controller()
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
        return Directory
               .EnumerateFiles(Path.Combine(GetRepoRoot(), "controlgallery/AtomUIGallery/ShowCases"),
                                "*ShowCase.axaml",
                                SearchOption.AllDirectories)
               .Where(path =>
               {
                   var source = File.ReadAllText(path);
                   return source.Contains("Name=\"ScenarioContentHost\"", StringComparison.Ordinal) &&
                          source.Contains("Name=\"ScenarioTabs\"", StringComparison.Ordinal);
               })
               .Order(StringComparer.Ordinal)
               .ToArray();
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
