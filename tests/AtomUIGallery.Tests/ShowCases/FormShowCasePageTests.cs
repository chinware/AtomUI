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
        source.ShouldContain("FormThemes.axaml");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:FormShowCaseLangResource PageDescription}\"");
        var examples = ExtractFormExampleItems(source);
        CountShowCaseItemElements(examples).ShouldBe(21);
        CountOccurrences(examples, "IsDeferredContentEnabled=\"True\"").ShouldBe(21);
        CountOccurrences(examples, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(21);
        CountOccurrences(examples, "DataTemplate x:DataType=\"viewModels:FormViewModel\"").ShouldBe(21);
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

    [Fact]
    public void Form_ShowCase_Declares_The_Semantic_Preview_Matching_Upstream_Example()
    {
        var source  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Views/FormShowCase.axaml");
        var english = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Localization/en-US.xlf");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"FormItemSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #PasswordSemanticItem}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:FormItem}\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(6);
        foreach (var path in new[] { "root", "label", "content", "help", "helpItem", "extra" })
        {
            CountOccurrences(source, $"Path=\"{path}\"").ShouldBe(1);
        }

        // The semantic demo form must mirror the upstream Semantic DOM example:
        // Username only carries help text; Password carries two error messages
        // plus the extra hint, laid out with labelCol 8 / wrapperCol 16.
        source.ShouldContain("LabelColInfo=\"8*\"");
        source.ShouldContain("WrapperColInfo=\"16*\"");
        source.ShouldContain("AttachedToVisualTree=\"HandleSemanticDemoFormAttached\"");
        source.ShouldContain("Help=\"{gallery:FormShowCaseLangResource SemanticHelpUse4To16Characters}\"");
        source.ShouldContain("Extra=\"{gallery:FormShowCaseLangResource SemanticExtraPasswordMustContainLettersAndNumbers}\"");
        source.ShouldContain("FormShowCaseLangResource P2MessagePleaseInputYourPassword");
        source.ShouldContain("FormShowCaseLangResource SemanticMessageUseAtLeast8Characters");
        source.ShouldContain("<atom:LineEdit PasswordChar=\"*\" />");

        english.ShouldContain("Use 4 to 16 characters.");
        english.ShouldContain("Use at least 8 characters.");
        english.ShouldContain("Password must contain letters and numbers.");
    }

    [Fact]
    public void Form_ShowCase_Declares_The_Semantic_Styling_Example_Matching_Upstream_Demo()
    {
        var source  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Views/FormShowCase.axaml");
        var english = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Localization/en-US.xlf");

        // Mirrors the upstream "Custom semantic dom styling" demo, rendered as the
        // LAST ShowCaseItem of the Examples list: two card forms sharing
        // Username/Email/Submit+reset content, the second one on the filled
        // variant with a blue root border and blue labels.
        source.ShouldContain("FormShowCaseLangResource SemanticStylingTitle");
        source.ShouldContain("FormShowCaseLangResource SemanticStylingDescription");
        source.ShouldContain("SourceKey=\"form-semantic-part\"");
        source.ShouldContain("BadgeText=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        source.ShouldContain("Span=\"Full\"");
        source.IndexOf("SourceKey=\"form-semantic-part\"", StringComparison.Ordinal)
              .ShouldBeGreaterThan(source.LastIndexOf("IsOccupyEntireRow=\"True\"", StringComparison.Ordinal));
        // 2 existing demos plus the 2 semantic styling forms use the 4/20 grid.
        CountOccurrences(source, "LabelColInfo=\"4*\"").ShouldBe(4);
        CountOccurrences(source, "WrapperColInfo=\"20*\"").ShouldBe(4);
        source.ShouldContain("Classes=\"semantic-card semantic-object\"");
        source.ShouldContain("Classes=\"semantic-card semantic-function\"");
        source.ShouldContain("StyleVariant=\"Filled\"");
        // The card shell is styled with control-level properties relayed by
        // the Form template root (the NumericUpDown root-relay precedent);
        // part styling must use the generated dedicated Semantic Part
        // styles, never hand-written part selectors.
        source.ShouldContain("Selector=\"atom|Form.semantic-card\"");
        source.ShouldContain("<Setter Property=\"BoxShadow\" Value=\"0 2 8 0 #1A000000\" />");
        source.ShouldContain("Selector=\"atom|Form.semantic-function\"");
        CountOccurrences(source, "<atom:FormItemLabelStyle x:SetterTargetType=\"TextBlock\">").ShouldBe(2);
        CountOccurrences(source, "<atom:FormItemContentStyle x:SetterTargetType=\"ContentPresenter\">").ShouldBe(1);
        source.ShouldNotContain("/template/ TextBlock.semantic-label");
        source.ShouldNotContain("/template/ ContentPresenter.semantic-content");
        source.ShouldNotContain("/template/ Border#Frame");
        // The two cards stack vertically; each stretches horizontally up to the
        // same MaxWidth so the 4*/20* star grid yields identical label columns.
        CountOccurrences(source, "MaxWidth=\"800\"").ShouldBe(2);
        // Regression: neither card may pin HorizontalAlignment — a Left
        // alignment hugs content width and defeats the shared MaxWidth cap.
        ExtractSemanticStylingRegion(source).ShouldNotContain("HorizontalAlignment");
        source.ShouldContain("Value=\"#1677FF\"");
        CountOccurrences(source, "<atom:SubmitButton Content=\"{gallery:FormShowCaseLangResource SemanticStyleSubmitButtonText}\" />").ShouldBe(2);
        CountOccurrences(source, "<atom:ResetButton Content=\"{gallery:FormShowCaseLangResource SemanticStyleResetButtonText}\" />").ShouldBe(2);
        source.ShouldContain("FormShowCaseLangResource SemanticStyleMessagePleaseEnterUsername");
        source.ShouldContain("FormShowCaseLangResource SemanticStyleMessagePleaseEnterEmail");
        source.ShouldContain("FormShowCaseLangResource SemanticStyleUsernamePlaceholder");
        source.ShouldContain("FormShowCaseLangResource SemanticStyleEmailPlaceholder");

        english.ShouldContain("Custom semantic styling");
        english.ShouldContain("You can customize the semantic style of Form by passing objects/functions through `classNames` and `styles`.");
        english.ShouldNotContain("semantic dom");
        english.ShouldContain("Please enter username!");
        english.ShouldContain("Please enter email!");
        english.ShouldContain("Please enter username");
        english.ShouldContain("Please enter email");
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

    private static string ExtractSemanticStylingRegion(string source)
    {
        const string startMarker = "Classes=\"semantic-card semantic-object\"";
        const string endMarker   = "</gallery:ShowCaseItem.DeferredContentTemplate>";

        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        start.ShouldBeGreaterThanOrEqualTo(0);

        var end = source.IndexOf(endMarker, start, StringComparison.Ordinal);
        end.ShouldBeGreaterThan(start);

        return source[start..end];
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
