using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUIForm = AtomUI.Desktop.Controls.Form;

namespace AtomUIGallery.Tests.ShowCases;

public class FormShowCasePageTests
{
    [Fact]
    public void Form_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Views/FormShowCase.axaml");

        source.ShouldContain("FormShowCaseLangResource PageSubtitle");
        source.ShouldContain("FormShowCaseLangResource PageDescription");
        source.ShouldNotContain("FormShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("FormShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("FormShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("FormShowCaseLangResource ComponentCategory");
        source.ShouldContain("FormShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("FormShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("FormShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("FormShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldContain("FormThemes.axaml");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:FormShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(20);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(20);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(20);
        CountOccurrences(source, "DataTemplate x:DataType=\"viewModels:FormViewModel\"").ShouldBe(20);
        CountOccurrences(source, "IsOccupyEntireRow=\"True\"").ShouldBeGreaterThanOrEqualTo(18);
        source.ShouldContain("FormShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("FormShowCaseLangResource FormLayoutTitle");
        source.ShouldContain("FormShowCaseLangResource FormVariantsTitle");
        source.ShouldContain("FormShowCaseLangResource NoBlockRuleTitle");
        source.ShouldContain("FormShowCaseLangResource DynamicFormItemTitle");
        source.ShouldContain("FormShowCaseLangResource RegistrationTitle");
        source.ShouldContain("FormShowCaseLangResource CustomizedFormControlsTitle");
        source.ShouldContain("OptionCheckedChanged=\"HandleFormLayoutOptionCheckedChanged\"");
        source.ShouldContain("SelectionChanged=\"HandleFormStyleVariantChanged\"");
        source.ShouldContain("OptionCheckedChanged=\"HandleFormRequiredMarkChanged\"");
        source.ShouldContain("OptionCheckedChanged=\"HandleFormSizeTypeChanged\"");
        source.ShouldContain("Click=\"HandleAddFormItem\"");
        source.ShouldContain("Click=\"HandleAddFormItemAtHead\"");
        source.ShouldContain("Click=\"HandleFillClicked\"");
        source.ShouldContain("Submitted=\"HandleNoBlockFormSubmitted\"");
        source.ShouldContain("Validated=\"HandleNoBlockFormValidated\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Form_ShowCase_Required_Layout_Demo_Items_Have_Validators()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Views/FormShowCase.axaml");

        var layoutDemo = ExtractRequiredLayoutDemo(source);
        CountOccurrences(layoutDemo, "IsRequired=\"True\"").ShouldBe(6);
        CountOccurrences(layoutDemo, "<atom:FormValidatorProvider>").ShouldBe(6);
        CountOccurrences(
                layoutDemo,
                "<atom:FormStringNotEmptyValidator Message=\"{gallery:FormShowCaseLangResource P2MessagePleaseInput}\" />")
            .ShouldBe(6);
    }

    [Fact]
    public void Donation_Form_Item_Does_Not_Validate_On_Initial_Default_Unit_Selection()
    {
        AvaloniaTestApp.EnsureInitialized();

        var donation = new AtomUIGallery.ShowCases.Form.Donation();
        var formItem = new FormItem
        {
            LabelText  = "Donation",
            FieldName  = "donation",
            IsRequired = true,
            Content    = donation,
            Validators = [new FormNotNullValidator { Message = "required" }]
        };
        var form = new AtomUIForm
        {
            Width = 420
        };
        form.Items.Add(formItem);

        var valueChangedCount = 0;
        ((IFormItemAware)donation).ValueChanged += (_, _) => valueChangedCount++;

        var window = new Avalonia.Controls.Window
        {
            Width   = 520,
            Height  = 240,
            Content = form
        };

        try
        {
            window.Show();
            RunLayoutJobs();

            valueChangedCount.ShouldBe(0);
            donation.Value.ShouldBeNull();
            donation.Status.ShouldBe(InputControlStatus.Default);
            DataValidationErrors.GetHasErrors(donation).ShouldBeFalse();
            formItem.ValidateStatus.ShouldBe(FormValidateStatus.Default);
            formItem.ValidateResult.ShouldBe(FormValidateResult.Success);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Form_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Views/FormShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/FormShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractFormExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractFormExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string ExtractRequiredLayoutDemo(string source)
    {
        const string firstFormMarker = "<atom:Form FormLayout=\"Horizontal\">";

        var firstFormStart = source.IndexOf(firstFormMarker, StringComparison.Ordinal);
        firstFormStart.ShouldBeGreaterThanOrEqualTo(0);

        var stackPanelCloseStart = source.IndexOf("</StackPanel>", firstFormStart, StringComparison.Ordinal);
        stackPanelCloseStart.ShouldBeGreaterThan(firstFormStart);

        return source[firstFormStart..stackPanelCloseStart];
    }

    private static string NormalizeMarkup(string source)
    {
        source = Regex.Replace(
            source,
            @"\s*AttachedToVisualTree=""Handle(BasicForm|LayoutCaseForm|FormSliderItem)Attached""",
            string.Empty,
            RegexOptions.CultureInvariant);
        source = Regex.Replace(
            source,
            @"\s*DetachedFromVisualTree=""HandleLayoutCaseFormDetached""",
            string.Empty,
            RegexOptions.CultureInvariant);

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

    private static int CountShowCaseItemElements(string source)
    {
        return Regex.Matches(source, @"<gallery:ShowCaseItem(\s|>)", RegexOptions.CultureInvariant).Count;
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

    private static void RunLayoutJobs()
    {
        Dispatcher.UIThread.RunJobs();
        Dispatcher.UIThread.RunJobs();
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
