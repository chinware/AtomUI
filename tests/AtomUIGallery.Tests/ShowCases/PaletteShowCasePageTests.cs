using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class PaletteShowCasePageTests
{
    [Fact]
    public void Palette_ShowCase_Uses_Document_Layout_With_Lazy_Palette_Scenarios()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Palette/Views/PaletteShowCase.axaml");

        source.ShouldContain("PaletteShowCaseLangResource PageSubtitle");
        source.ShouldContain("PaletteShowCaseLangResource PageDescription");
        source.ShouldContain("PaletteShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("PaletteShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("PaletteShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("PaletteShowCaseLangResource ComponentCategory");
        source.ShouldContain("PaletteShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("PaletteShowCaseLangResource P2HeaderLight");
        source.ShouldContain("PaletteShowCaseLangResource P2HeaderDark");
        source.ShouldContain("Tag=\"Light\"");
        source.ShouldContain("Tag=\"Dark\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("LightPaletteContentTemplate");
        source.ShouldContain("DarkPaletteContentTemplate");
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:PaletteShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        CountOccurrences(source, "<gallery:ColorListControl").ShouldBe(4);
        CountOccurrences(source, "PaletteMetaInfo=\"{Binding Left}\"").ShouldBe(2);
        CountOccurrences(source, "PaletteMetaInfo=\"{Binding Right}\"").ShouldBe(2);
        CountOccurrences(source, "IsDark=\"True\"").ShouldBe(2);
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:ScrollViewer");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Palette_ShowCase_Lazy_Loads_Light_And_Dark_Content()
    {
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Palette/Views/PaletteShowCase.axaml.cs");

        codeBehindSource.ShouldContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldContain("LightPaletteContentTemplate");
        codeBehindSource.ShouldContain("DarkPaletteContentTemplate");
        codeBehindSource.ShouldContain("ContentTemplate = template");
        codeBehindSource.ShouldContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldContain("_scenarioController.Detach()");
        codeBehindSource.ShouldNotContain("new ColorListControl");
    }

    [Fact]
    public void Palette_ShowCase_Localization_Includes_Page_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Palette/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Palette/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Palette/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ComponentCategory");
            source.ShouldContain("ComponentStatusStable");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("PageDescription");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("InfoPackageLabel");
            source.ShouldContain("InfoBaseClassLabel");
            source.ShouldContain("P2HeaderLight");
            source.ShouldContain("P2HeaderDark");
        }
    }

    [Fact]
    public void Palette_ShowCase_Scenarios_Match_Approved_Gallery_Content()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Palette/Views/PaletteShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Palette/Views/PaletteShowCase.axaml.cs");
        var approved         = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/PaletteShowCaseExamples.snapshot");

        var normalized = NormalizePaletteScenarios(pageSource, codeBehindSource);
        CountPaletteScenarios(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string NormalizePaletteScenarios(string pageSource, string codeBehindSource)
    {
        var scenarios = new List<string>();
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "Light", "P2HeaderLight", isDark: false);
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "Dark", "P2HeaderDark", isDark: true);
        return string.Join("\n", scenarios);
    }

    private static void AddScenarioIfPresent(
        ICollection<string> scenarios,
        string pageSource,
        string codeBehindSource,
        string scenario,
        string resourceKey,
        bool isDark)
    {
        var hasOldInlineScenario =
            pageSource.Contains(resourceKey, StringComparison.Ordinal) &&
            pageSource.Contains("PaletteMetaInfo=\"{Binding Left}\"", StringComparison.Ordinal) &&
            pageSource.Contains("PaletteMetaInfo=\"{Binding Right}\"", StringComparison.Ordinal) &&
            (!isDark || pageSource.Contains("IsDark=\"True\"", StringComparison.Ordinal));

        var hasLazyScenario =
            pageSource.Contains(resourceKey, StringComparison.Ordinal) &&
            pageSource.Contains($"Tag=\"{scenario}\"", StringComparison.Ordinal) &&
            codeBehindSource.Contains($"{scenario}Scenario", StringComparison.Ordinal) &&
            pageSource.Contains("PaletteMetaInfo=\"{Binding Left}\"", StringComparison.Ordinal) &&
            pageSource.Contains("PaletteMetaInfo=\"{Binding Right}\"", StringComparison.Ordinal) &&
            (!isDark || pageSource.Contains("IsDark=\"True\"", StringComparison.Ordinal));

        if (hasOldInlineScenario || hasLazyScenario)
        {
            scenarios.Add($"{scenario}:{resourceKey}:ColorListControl:Left:Right:IsDark={isDark}");
        }
    }

    private static int CountPaletteScenarios(string source)
    {
        return source.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static string ComputeSha256(string source)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string ReadSnapshotHash(string source)
    {
        return source
            .Split('\n')
            .First(line => line.StartsWith("sha256:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim();
    }

    private static int ReadSnapshotCount(string source)
    {
        return int.Parse(source
            .Split('\n')
            .First(line => line.StartsWith("count:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim());
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

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
