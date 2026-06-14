using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class IconShowCasePageTests
{
    [Fact]
    public void Icon_ShowCase_Uses_Document_Layout_With_Lazy_Icon_Galleries()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml");

        source.ShouldContain("IconShowCaseLangResource PageSubtitle");
        source.ShouldContain("IconShowCaseLangResource PageDescription");
        source.ShouldContain("IconShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("IconShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("IconShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("IconShowCaseLangResource ComponentCategory");
        source.ShouldContain("IconShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("IconShowCaseLangResource P2HeaderOutlined");
        source.ShouldContain("IconShowCaseLangResource P2HeaderFilled");
        source.ShouldContain("IconShowCaseLangResource P2HeaderTwoTone");
        source.ShouldContain("Tag=\"Outlined\"");
        source.ShouldContain("Tag=\"Filled\"");
        source.ShouldContain("Tag=\"TwoTone\"");
        source.ShouldContain("RowDefinitions=\"Auto,Auto,*\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("Grid.Row=\"1\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\"");
        source.ShouldContain("Grid.Row=\"2\"");
        source.ShouldContain("Margin=\"28,10,28,28\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:IconShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldNotContain("<atom:ScrollViewer");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<gallery:IconGallery IconThemeType=");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Icon_ShowCase_Lazy_Loads_IconGallery_For_Selected_Theme()
    {
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml.cs");

        codeBehindSource.ShouldContain("ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged");
        codeBehindSource.ShouldContain("EnsureSelectedScenarioContent");
        codeBehindSource.ShouldContain("ScenarioContentHost.Content = content");
        codeBehindSource.ShouldContain("new IconGallery()");
        codeBehindSource.ShouldContain("IconThemeType = IconThemeType.Outlined");
        codeBehindSource.ShouldContain("IconThemeType = IconThemeType.Filled");
        codeBehindSource.ShouldContain("IconThemeType = IconThemeType.TwoTone");
        codeBehindSource.ShouldContain("VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch");
        codeBehindSource.ShouldNotContain("Height        = 640");
        codeBehindSource.ShouldContain("_lazyScenarioContentCache");
        codeBehindSource.ShouldContain("ClearLazyScenarioContent");
    }

    [Fact]
    public void Icon_ShowCase_Localization_Includes_Page_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ComponentCategory");
            source.ShouldContain("ComponentStatusStable");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("PageDescription");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("InfoPackageLabel");
            source.ShouldContain("InfoBaseClassLabel");
            source.ShouldContain("P2HeaderOutlined");
            source.ShouldContain("P2HeaderFilled");
            source.ShouldContain("P2HeaderTwoTone");
        }
    }

    [Fact]
    public void Icon_ShowCase_Themes_Match_Approved_Gallery_Content()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml.cs");
        var approved         = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/IconShowCaseExamples.snapshot");

        var normalized = NormalizeIconThemeScenarios(pageSource, codeBehindSource);
        CountIconThemeScenarios(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string NormalizeIconThemeScenarios(string pageSource, string codeBehindSource)
    {
        var scenarios = new List<string>();
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "Outlined", "P2HeaderOutlined");
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "Filled", "P2HeaderFilled");
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "TwoTone", "P2HeaderTwoTone");
        return string.Join("\n", scenarios);
    }

    private static void AddScenarioIfPresent(
        ICollection<string> scenarios,
        string pageSource,
        string codeBehindSource,
        string theme,
        string resourceKey)
    {
        var hasOldInlineGallery =
            pageSource.Contains(resourceKey, StringComparison.Ordinal) &&
            pageSource.Contains($"IconThemeType=\"{theme}\"", StringComparison.Ordinal);

        var hasLazyGallery =
            pageSource.Contains(resourceKey, StringComparison.Ordinal) &&
            pageSource.Contains($"Tag=\"{theme}\"", StringComparison.Ordinal) &&
            codeBehindSource.Contains($"IconThemeType.{theme}", StringComparison.Ordinal);

        if (hasOldInlineGallery || hasLazyGallery)
        {
            scenarios.Add($"{theme}:{resourceKey}");
        }
    }

    private static int CountIconThemeScenarios(string source)
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
