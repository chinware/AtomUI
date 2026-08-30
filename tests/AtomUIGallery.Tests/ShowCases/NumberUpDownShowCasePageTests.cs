using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class NumberUpDownShowCasePageTests
{
    [Fact]
    public void NumberUpDown_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml");

        source.ShouldContain("NumberUpDownShowCaseLangResource PageSubtitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource PageDescription");
        source.ShouldNotContain("NumberUpDownShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("NumberUpDownShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("NumberUpDownShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("NumberUpDownShowCaseLangResource ComponentCategory");
        source.ShouldContain("NumberUpDownShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("NumberUpDownShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("NumberUpDownShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("NumberUpDownShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        source.ShouldContain("Selector=\"atom|NumericUpDown\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:NumberUpDownShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(16);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(16);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:NumberUpDownViewModel\"").ShouldBe(17);
        source.ShouldContain("NumberUpDownShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource SpinnerModeTitle");
        source.ShouldContain("Title=\"{gallery:NumberUpDownShowCaseLangResource SpinnerModeTitle}\"\n        BadgeText=\"v6.0.5\"");
        CountOccurrences(source, "BadgeText=\"v6.0.5\"").ShouldBe(1);
        source.ShouldContain("Spacing=\"{atom:SharedTokenResource UniformlyMargin}\"");
        source.ShouldContain("Mode=\"Spinner\"");
        source.ShouldContain("NumberUpDownShowCaseLangResource HideHandleTitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource HideHandleDescription");
        source.ShouldContain("Title=\"{gallery:NumberUpDownShowCaseLangResource HideHandleTitle}\"\n        BadgeText=\"v6.0.7\"");
        CountOccurrences(source, "BadgeText=\"v6.0.7\"").ShouldBe(1);
        CountOccurrences(source, "ShowButtonSpinner=\"False\"").ShouldBe(2);
        source.ShouldContain("FormatString=\"0\"");
        source.ShouldContain("PlaceholderText=\"Outlined\"");
        source.ShouldContain("PlaceholderText=\"Filled\"");
        source.ShouldContain("StyleVariant=\"Filled\"");
        source.ShouldContain("NumberUpDownShowCaseLangResource StringModeTitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource DecimalStepTitle");
        source.ShouldContain("Name=\"CustomSizeTypeNumberUpDown\"");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("Height=\"38\"");
        source.ShouldContain("FontSize=\"15\"");
        source.ShouldContain("IsCustomFontSize=\"True\"");
        source.ShouldContain("NumberUpDownShowCaseLangResource StatusTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void NumberUpDown_ShowCase_Declares_The_Semantic_Previews_And_Style_Example()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml");
        var english = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Localization/en-US.xlf");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"NumberUpDownSemanticPreview\"");
        source.ShouldContain("Name=\"SpinnerModeSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #NumberUpDownSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #SpinnerModeSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:NumericUpDown}\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(10);
        foreach (var path in new[] { "root", "prefix", "input", "suffix", "clear" })
        {
            CountOccurrences(source, $"Path=\"{path}\"").ShouldBe(2);
        }

        source.ShouldContain("SourceKey=\"numericupdown-semantic-part\"");
        source.ShouldContain("BadgeText=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        source.ShouldContain("NumberUpDownShowCaseLangResource SemanticPartStyleTitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource SemanticPartStyleDescription");
        source.ShouldContain("Selector=\"atom|NumericUpDown.semantic-fixed\"");
        source.ShouldContain("Selector=\"atom|NumericUpDown.semantic-conditional\"");
        CountOccurrences(source, "<atom:NumericUpDownInputStyle").ShouldBe(1);
        CountOccurrences(source, "<atom:NumericUpDownPrefixStyle").ShouldBe(1);
        CountOccurrences(source, "<atom:NumericUpDownSuffixStyle").ShouldBe(1);
        CountOccurrences(source, "<atom:NumericUpDownClearStyle").ShouldBe(1);
        source.ShouldContain("x:SetterTargetType=\"{x:Type atom:TextBox}\"");
        source.ShouldContain("x:SetterTargetType=\"ContentPresenter\"");
        source.ShouldContain("x:SetterTargetType=\"StackPanel\"");
        source.ShouldContain("x:SetterTargetType=\"Button\"");
        CountOccurrences(source, "Classes=\"semantic-fixed\"").ShouldBe(1);
        CountOccurrences(source, "Classes=\"semantic-conditional\"").ShouldBe(1);

        english.ShouldContain("<unit id=\"SemanticRootDescription\">");
        english.ShouldContain("<unit id=\"SemanticPartStyleTitle\">");
    }

    [Fact]
    public void NumberUpDown_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/NumberUpDownShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractNumberUpDownExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractNumberUpDownExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string NormalizeMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.Normalize(source);
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
