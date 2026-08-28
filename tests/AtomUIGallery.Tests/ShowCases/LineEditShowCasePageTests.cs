using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class LineEditShowCasePageTests
{
    [Fact]
    public void LineEdit_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");

        source.ShouldContain("LineEditShowCaseLangResource PageSubtitle");
        source.ShouldContain("LineEditShowCaseLangResource PageDescription");
        source.ShouldNotContain("LineEditShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("LineEditShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("LineEditShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("LineEditShowCaseLangResource ComponentCategory");
        source.ShouldContain("LineEditShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("LineEditShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("LineEditShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("LineEditShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
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
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:LineEditShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(22);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(22);
        source.ShouldContain("LineEditShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("LineEditShowCaseLangResource TextBoxTitle");
        source.ShouldContain("LineEditShowCaseLangResource InputSizesTitle");
        source.ShouldContain("LineEditShowCaseLangResource P2PlaceholderTextCustom");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("Height=\"38\"");
        source.ShouldContain("FontSize=\"15\"");
        source.ShouldContain("LineEditShowCaseLangResource InputStatusTitle");
        source.ShouldContain("LineEditShowCaseLangResource SearchBoxTitle");
        source.ShouldContain("LineEditShowCaseLangResource SearchEditSizeTypeTitle");
        source.ShouldContain("LineEditShowCaseLangResource SearchEditSizeTypeDescription");
        source.ShouldContain("Name=\"CustomSizeTypeSearchEdit\"");
        source.ShouldContain("LineEditShowCaseLangResource TextAreaTitle");
        source.ShouldContain("LineEditShowCaseLangResource OtpLineEditTwoWayBindingTitle");
        source.ShouldContain("LineEditShowCaseLangResource OtpLineEditFormTitle");
        source.ShouldContain("LineEditShowCaseLangResource OtpLineEditAntDesignTitle");
        source.ShouldContain("LineEditShowCaseLangResource SemanticPartStyleTitle");
        source.ShouldNotContain("SourceKey=\"line-edit-otp-basic\"");
        source.ShouldContain("SourceKey=\"line-edit-otp-two-way\"");
        source.ShouldContain("SourceKey=\"line-edit-otp-form\"");
        source.ShouldContain("SourceKey=\"line-edit-otp-ant-design\"");
        CountOccurrences(source, "BadgeText=\"v6.0.8\"").ShouldBe(3);
        CountOccurrences(source, "BadgeText=\"5.16.0\"").ShouldBe(0);
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void LineEdit_ShowCase_Declares_The_Semantic_Preview_And_Style_Example()
    {
        var source = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var english = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Localization/en-US.xlf");
        var semanticExample = ExtractShowCaseItem(source, "line-edit-semantic-part");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"LineEditSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #LineEditSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:LineEdit}\"");
        source.ShouldContain("Name=\"LineEditSemanticOwner\"");
        source.ShouldContain("SizeType=\"Middle\"");
        source.ShouldContain("StyleVariant=\"Outlined\"");
        source.ShouldContain("IsAllowClear=\"True\"");
        source.ShouldContain("IsShowCount=\"True\"");
        foreach (var path in new[] { "root", "prefix", "input", "suffix", "clear", "count" })
        {
            source.ShouldContain($"Path=\"{path}\"");
        }

        semanticExample.ShouldContain("SourceKey=\"line-edit-semantic-part\"");
        semanticExample.ShouldContain("BadgeText=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        semanticExample.ShouldContain("Selector=\"atom|LineEdit.style-class-base\"");
        semanticExample.ShouldContain("Selector=\"atom|LineEdit.style-class-object\"");
        semanticExample.ShouldContain("Selector=\"atom|LineEdit.style-class-object:focus-within\"");
        semanticExample.ShouldContain("<Setter Property=\"BorderBrush\" Value=\"#D9D9D9\" />");
        semanticExample.ShouldContain("<Setter Property=\"BorderBrush\" Value=\"#A9A9A9\" />");
        semanticExample.ShouldContain("Selector=\"atom|LineEdit.style-class-fn\"");
        semanticExample.ShouldContain("Selector=\"atom|LineEdit.style-class-password\"");
        semanticExample.ShouldContain("Selector=\"atom|TextArea.style-class-textarea\"");
        semanticExample.ShouldContain("Selector=\"atom|OtpLineEdit.style-class-otp\"");
        semanticExample.ShouldContain("Selector=\"atom|SearchEdit.style-class-search\"");
        semanticExample.ShouldNotContain("BorderThickness");
        semanticExample.ShouldNotContain("CornerRadius");
        semanticExample.ShouldContain("Selector=\"atom|TextArea.style-class-textarea /template/ atom|TextBlock.semantic-count\"");
        semanticExample.ShouldContain("Selector=\"atom|SearchEdit.style-class-search /template/ TextPresenter.semantic-input\"");
        semanticExample.ShouldContain("Selector=\"atom|SearchEdit.style-class-search /template/ .semantic-scope-input-frame /template/ atom|Button.semantic-button\"");
        CountOccurrences(semanticExample, "<Setter Property=\"Foreground\" Value=\"#4DA8DA\" />").ShouldBe(2);
        semanticExample.ShouldContain("<Setter Property=\"TextElement.Foreground\" Value=\"#4DA8DA\" />");
        semanticExample.ShouldContain("<Setter Property=\"CaretBrush\" Value=\"#4DA8DA\" />");
        CountOccurrences(semanticExample, "<Setter Property=\"BorderBrush\" Value=\"#4DA8DA\" />").ShouldBe(2);
        CountOccurrences(semanticExample, "#696FC7").ShouldBe(1);
        CountOccurrences(semanticExample, "#BDE3C3").ShouldBe(2);
        CountOccurrences(semanticExample, "#F5D3C4").ShouldBe(1);
        CountOccurrences(semanticExample, "#6E8CFB").ShouldBe(1);
        CountOccurrences(semanticExample, "#4DA8DA").ShouldBeGreaterThanOrEqualTo(3);
        CountOccurrences(semanticExample, "<atom:LineEdit").ShouldBe(3);
        semanticExample.ShouldContain("Classes=\"style-class-base style-class-fn\"");
        semanticExample.ShouldContain("Classes=\"style-class-base style-class-password\"");
        semanticExample.ShouldContain("SizeType=\"Middle\"");
        semanticExample.ShouldContain("Length=\"6\"");
        semanticExample.ShouldContain("Separator=\"*\"");
        semanticExample.ShouldContain("<Setter Property=\"CellWidth\" Value=\"32\" />");
        semanticExample.ShouldContain("<Setter Property=\"CellBorderBrush\" Value=\"#6E8CFB\" />");
        semanticExample.ShouldContain("SizeType=\"Large\"");

        foreach (var staleCaption in new[] { "SemanticPartTextStyleTitle", "SemanticPartPrefixStyleTitle", "SemanticPartClearStyleTitle", "SemanticPartAccentStyleTitle" })
        {
            source.ShouldNotContain(staleCaption);
            english.ShouldNotContain($"unit id=\"{staleCaption}\"");
        }
        english.ShouldContain("<source>Custom semantic dom styling</source>");

        english.ShouldContain("unit id=\"SemanticRootDescription\"");
        english.ShouldContain("unit id=\"SemanticPrefixDescription\"");
        english.ShouldContain("unit id=\"SemanticInputDescription\"");
        english.ShouldContain("unit id=\"SemanticSuffixDescription\"");
        english.ShouldContain("unit id=\"SemanticClearDescription\"");
        english.ShouldContain("unit id=\"SemanticCountDescription\"");
        english.ShouldContain("unit id=\"SemanticPartStyleTitle\"");
        english.ShouldContain("unit id=\"SemanticPartStyleDescription\"");
    }

    [Fact]
    public void LineEdit_ShowCase_Includes_TextBox_Demo()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var demo   = ExtractShowCaseItem(source, "LineEditShowCaseLangResource TextBoxTitle");

        demo.ShouldContain("LineEditShowCaseLangResource TextBoxDescription");
        demo.ShouldContain("<atom:TextBox");
        demo.ShouldContain("PlaceholderText=\"{gallery:LineEditShowCaseLangResource P2PlaceholderTextTextBox}\"");
        demo.ShouldContain("PlaceholderText=\"{gallery:LineEditShowCaseLangResource P2PlaceholderTextInputWithClearIcon}\"");
        demo.ShouldContain("PlaceholderText=\"{gallery:LineEditShowCaseLangResource P2PlaceholderTextInputPassword}\"");
        demo.ShouldContain("Width=\"360\"");
        demo.ShouldContain("HorizontalAlignment=\"Left\"");
        demo.ShouldContain("IsAllowClear=\"True\"");
        demo.ShouldContain("PasswordChar=\"•\"");
        demo.ShouldContain("IsEnableRevealButton=\"True\"");
        CountOccurrences(demo, "<atom:TextBox").ShouldBe(3);
        demo.ShouldNotContain("<atom:LineEdit");
    }

    [Fact]
    public void LineEdit_ShowCase_Demonstrates_TemplateOnly_InnerRight_ToolTip()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var demo   = ExtractShowCaseItem(source, "LineEditShowCaseLangResource PrefixAndSuffixTitle");

        demo.ShouldContain("<atom:LineEdit.InnerRightContentTemplate>");
        demo.ShouldContain("<DataTemplate>");
        demo.ShouldContain("<antdicons:InfoCircleOutlined");
        demo.ShouldContain("atom:ToolTip.Tip=\"{gallery:LineEditShowCaseLangResource PrefixAndSuffixToolTip}\"");
        demo.ShouldNotContain("InnerRightContent=\"{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined");
    }

    [Fact]
    public void LineEdit_ShowCase_Embeds_OtpLineEdit_Without_Standalone_Page()
    {
        var source           = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var moduleSource     = ReadRepoFile("controlgallery/AtomUIGallery/AtomUIGalleryModule.cs");
        var otpTwoWayDemo    = ExtractShowCaseItem(source, "LineEditShowCaseLangResource OtpLineEditTwoWayBindingTitle");
        var otpFormDemo      = ExtractShowCaseItem(source, "LineEditShowCaseLangResource OtpLineEditFormTitle");
        var standalonePage      = GetRepoPath("controlgallery/AtomUIGallery/ShowCases/DataEntry/OtpLineEdit/Views/OtpLineEditShowCase.axaml");
        var standaloneViewModel = GetRepoPath("controlgallery/AtomUIGallery/ShowCases/DataEntry/OtpLineEdit/ViewModels/OtpLineEditViewModel.cs");

        File.Exists(standalonePage).ShouldBeFalse("OtpLineEdit should be demonstrated inside LineEdit instead of owning a standalone Gallery page.");
        File.Exists(standaloneViewModel).ShouldBeFalse("OtpLineEdit should not own a standalone Gallery ViewModel.");
        moduleSource.ShouldNotContain("using AtomUIGallery.ShowCases.OtpLineEdit;");
        moduleSource.ShouldNotContain("OtpLineEditViewModel.ID");
        moduleSource.ShouldNotContain("DataEntry_OtpLineEdit");

        otpTwoWayDemo.ShouldContain("Text=\"{Binding OtpLineEditBoundValue, Mode=TwoWay}\"");
        otpTwoWayDemo.ShouldContain("Command=\"{Binding SetOtpLineEditBoundValueCommand}\"");
        otpTwoWayDemo.ShouldContain("Command=\"{Binding ClearOtpLineEditBoundValueCommand}\"");
        otpTwoWayDemo.ShouldContain("Text=\"{Binding OtpLineEditBoundValueSummary}\"");
        otpTwoWayDemo.ShouldContain("BadgeText=\"v6.0.8\"");
        otpTwoWayDemo.ShouldNotContain("IsAllowClear=\"True\"");

        otpFormDemo.ShouldContain("<atom:FormItem.Validators>");
        otpFormDemo.ShouldContain("<atom:FormValidatorProvider>");
        otpFormDemo.ShouldContain("<atom:FormStringNotEmptyValidator Message=\"{gallery:LineEditShowCaseLangResource OtpLineEditFormValidationMessage}\" />");
        otpFormDemo.ShouldContain("</atom:FormValidatorProvider>");
        otpFormDemo.ShouldContain("BadgeText=\"v6.0.8\"");
        otpFormDemo.ShouldNotContain("IsAllowClear=\"True\"");
    }

    [Fact]
    public void LineEdit_ShowCase_Includes_AntDesign_Otp_Demo()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var demo   = ExtractShowCaseItem(source, "LineEditShowCaseLangResource OtpLineEditAntDesignTitle");

        demo.ShouldContain("SourceKey=\"line-edit-otp-ant-design\"");
        demo.ShouldContain("BadgeText=\"v6.0.8\"");
        demo.ShouldNotContain("Span=\"Full\"");
        CountOccurrences(demo, "<atom:OtpLineEdit ").ShouldBe(7);
        demo.ShouldContain("With formatter (Upcase)");
        demo.ShouldContain("With Disabled");
        demo.ShouldContain("With Length (8)");
        demo.ShouldContain("With variant");
        demo.ShouldContain("With custom display character");
        demo.ShouldContain("With custom separator");
        demo.ShouldContain("With custom function separator");
        demo.ShouldContain("Formatter=\"{Binding OtpLineEditUppercaseFormatter}\"");
        demo.ShouldContain("IsEnabled=\"False\"");
        demo.ShouldContain("Length=\"8\"");
        demo.ShouldContain("StyleVariant=\"Filled\"");
        demo.ShouldNotContain("StyleVariant=\"Outlined\"");
        demo.ShouldNotContain("StyleVariant=\"Borderless\"");
        demo.ShouldNotContain("StyleVariant=\"Underlined\"");
        demo.ShouldContain("IsMasked=\"True\"");
        demo.ShouldContain("Separator=\"/\"");
        demo.ShouldContain("Separator=\"—\"");
        demo.ShouldContain("SeparatorInterval=\"1\"");
        demo.ShouldContain("SeparatorTemplate");
        demo.ShouldContain("OtpLineEditSeparatorBrushConverter");
        demo.ShouldContain("CellIndex");
    }

    [Fact]
    public void LineEdit_ShowCase_SearchEdit_SizeType_Item_Demonstrates_All_Size_Modes()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var item   = ExtractShowCaseItem(source, "LineEditShowCaseLangResource SearchEditSizeTypeTitle");

        CountOccurrences(item, "<atom:SearchEdit").ShouldBe(4);
        item.ShouldContain("Name=\"CustomSizeTypeSearchEdit\"");
        item.ShouldContain("SizeType=\"Large\"");
        item.ShouldContain("SizeType=\"Middle\"");
        item.ShouldContain("SizeType=\"Small\"");
        item.ShouldContain("SizeType=\"Custom\"");
        item.ShouldContain("Height=\"38\"");
        item.ShouldContain("FontSize=\"15\"");
    }

    [Fact]
    public void LineEdit_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/LineEditShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractLineEditExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractLineEditExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return StripDeferredLoadingMarkup(source[firstItemStart..panelCloseStart]);
    }

    private static string ExtractShowCaseItem(string source, string titleMarker)
    {
        var titleIndex = source.IndexOf(titleMarker, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        const string itemStartMarker = "<gallery:ShowCaseItem";
        const string itemEndMarker   = "</gallery:ShowCaseItem>";

        var itemStart = source.LastIndexOf(itemStartMarker, titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var itemEnd = source.IndexOf(itemEndMarker, titleIndex, StringComparison.Ordinal);
        itemEnd.ShouldBeGreaterThan(titleIndex);

        return source[itemStart..(itemEnd + itemEndMarker.Length)];
    }

    private static string StripDeferredLoadingMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.StripDeferredLoadingMarkup(source);
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
        var path = GetRepoPath(relativePath);
        return File.Exists(path)
            ? path
            : Path.Combine(AppContext.BaseDirectory, relativePath);
    }

    private static string GetRepoPath(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
