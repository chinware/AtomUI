using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class MigratedShowCaseDeferredCreationTests
{
    [Fact]
    public void Migrated_ShowCases_Defer_Every_Example_Item_Content()
    {
        var migratedShowCases = GetMigratedShowCaseFiles();
        migratedShowCases.Count.ShouldBeGreaterThan(0);

        var failures = new List<string>();
        foreach (var file in migratedShowCases)
        {
            var source       = File.ReadAllText(file);
            var relativePath = Path.GetRelativePath(GetRepoRoot(), file);
            var examples     = ExtractExamplesPanel(source, relativePath);
            var itemCount    = CountShowCaseItems(examples);

            if (!examples.Contains("IsDeferredLoadingEnabled=\"True\"", StringComparison.Ordinal))
            {
                failures.Add($"{relativePath}: ExamplesContent must set IsDeferredLoadingEnabled=\"True\".");
            }

            var deferredItemCount = CountOccurrences(examples, "IsDeferredContentEnabled=\"True\"");
            if (deferredItemCount != itemCount)
            {
                failures.Add(
                    $"{relativePath}: expected {itemCount} deferred ShowCaseItems, found {deferredItemCount}.");
            }

            var deferredTemplateCount = CountOccurrences(examples, "<gallery:ShowCaseItem.DeferredContentTemplate>");
            if (deferredTemplateCount != itemCount)
            {
                failures.Add(
                    $"{relativePath}: expected {itemCount} DeferredContentTemplate blocks, found {deferredTemplateCount}.");
            }

            var typedDeferredTemplateCount = Regex
                                             .Matches(examples,
                                                      "<gallery:ShowCaseItem\\.DeferredContentTemplate>\\s*<DataTemplate\\s+x:DataType=\"",
                                                      RegexOptions.Singleline)
                                             .Count;
            if (typedDeferredTemplateCount != itemCount)
            {
                failures.Add(
                    $"{relativePath}: expected {itemCount} typed deferred DataTemplates, found {typedDeferredTemplateCount}.");
            }
        }

        failures.ShouldBeEmpty();
    }

    private static IReadOnlyList<string> GetMigratedShowCaseFiles()
    {
        return Directory
               .EnumerateFiles(Path.Combine(GetRepoRoot(), "controlgallery/AtomUIGallery/ShowCases"),
                                "*ShowCase.axaml",
                                SearchOption.AllDirectories)
               .Where(path =>
               {
                   var source = File.ReadAllText(path);
                   return source.Contains("<gallery:GalleryStickyTabsHost", StringComparison.Ordinal) &&
                          source.Contains("<gallery:ShowCaseItem", StringComparison.Ordinal);
               })
               .Order(StringComparer.Ordinal)
               .ToArray();
    }

    private static string ExtractExamplesPanel(string source, string relativePath)
    {
        const string panelMarker = "<gallery:ShowCasePanel Name=\"ExamplesContent\"";

        var panelStart = source.IndexOf(panelMarker, StringComparison.Ordinal);
        panelStart.ShouldBeGreaterThanOrEqualTo(0, $"{relativePath}: ExamplesContent panel was not found.");

        var panelOpenEnd = source.IndexOf('>', panelStart);
        panelOpenEnd.ShouldBeGreaterThan(panelStart, $"{relativePath}: ExamplesContent panel opening tag was not closed.");

        const string panelCloseMarker = "</gallery:ShowCasePanel>";
        var panelCloseStart = source.IndexOf(panelCloseMarker, panelOpenEnd, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(panelOpenEnd, $"{relativePath}: ExamplesContent panel was not closed.");

        return source[panelStart..panelCloseStart];
    }

    private static int CountOccurrences(string source, string value)
    {
        var count      = 0;
        var startIndex = 0;
        while (true)
        {
            var matchIndex = source.IndexOf(value, startIndex, StringComparison.Ordinal);
            if (matchIndex < 0)
            {
                return count;
            }

            count++;
            startIndex = matchIndex + value.Length;
        }
    }

    private static int CountShowCaseItems(string source)
    {
        return Regex.Matches(source, "<gallery:ShowCaseItem(?=\\s|>)").Count;
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
