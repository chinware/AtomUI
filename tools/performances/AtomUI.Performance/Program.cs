using System.Text;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;

namespace AtomUI.Performance;

internal static partial class Program
{
    internal const int DefaultCount = 60;

    [STAThread]
    public static int Main(string[] args)
    {
        var options = PerfOptions.Parse(args);
        SetupAvalonia();

        AddOnDecoratedBoxPerfProbe.IsEnabled = true;
        if (options.VerifyAccessories ||
            options.VerifyEffectiveBrushes ||
            options.VerifyAddonStates ||
            options.VerifyAntDesignMetadata ||
            options.VerifyIconHiddenSlots ||
            options.VerifyIconProviderCache ||
            options.VerifyButtonStates ||
            options.VerifySpaceStates ||
            options.VerifySelectStates ||
            options.VerifyAutoCompleteStates ||
            options.VerifyAvatarStates ||
            options.VerifyBadgeStates ||
            options.VerifyButtonSpinnerStates ||
            options.VerifyCardStates ||
            options.VerifyCarouselStates ||
            options.VerifyCascaderStates ||
            options.VerifyCheckBoxStates ||
            options.VerifyCollapseStates ||
            options.VerifyComboBoxStates ||
            options.VerifyDatePickerStates ||
            options.VerifyDescriptionsStates ||
            options.VerifyDialogStates ||
            options.VerifyDrawerStates ||
            options.VerifyExpanderStates ||
            options.VerifyEmptyStates ||
            options.VerifyFloatButtonStates ||
            options.VerifyFlyoutStates ||
            options.VerifyFormStates ||
            options.VerifyGroupBoxStates ||
            options.VerifyImagePreviewerStates ||
            options.VerifyInputStates ||
            options.VerifyListBoxStates ||
            options.VerifyListViewStates ||
            options.VerifyMarqueeLabelStates ||
            options.VerifyMentionsStates ||
            options.VerifyMenuStates ||
            options.VerifyNavMenuStates ||
            options.VerifyMessageStates ||
            options.VerifyMessageBoxStates ||
            options.VerifyNotificationStates ||
            options.VerifyNumericUpDownStates ||
            options.VerifyOptionButtonGroupStates ||
            options.VerifyPaginationStates ||
            options.VerifyPopupConfirmStates ||
            options.VerifyProgressBarStates ||
            options.VerifyRadioButtonStates ||
            options.VerifyQRCodeStates ||
            options.VerifyRateStates ||
            options.VerifyResultStates ||
            options.VerifyScrollViewerStates ||
            options.VerifySkeletonStates ||
            options.VerifySegmentedStates ||
            options.VerifySpinStates ||
            options.VerifySwitchStates ||
            options.VerifyTabControlStates ||
            options.VerifyStepsStates ||
            options.VerifyStatisticStates ||
            options.VerifySplitViewStates ||
            options.VerifySplitterStates ||
            options.VerifySliderStates ||
            options.VerifyTreeViewStates ||
            options.VerifyTransferStates ||
            options.VerifyTextBlockStates)
        {
            var verified = true;
            if (options.VerifyAccessories)
            {
                Console.WriteLine("--verify-accessories is currently disabled (Avalonia 12 accessory architecture rewrite pending).");
            }
            if (options.VerifyEffectiveBrushes)
            {
                Console.WriteLine("--verify-effective-brushes is currently disabled (AddOnDecoratedBox InnerBox*Brush API removed in Avalonia 12 migration).");
            }
            if (options.VerifyAddonStates)
            {
                verified &= RunAddonStateVerification();
            }
            if (options.VerifyAntDesignMetadata)
            {
                verified &= RunAntDesignMetadataVerification();
            }
            if (options.VerifyIconHiddenSlots)
            {
                verified &= RunIconHiddenSlotsVerification();
            }
            if (options.VerifyIconProviderCache)
            {
                verified &= RunIconProviderCacheVerification();
            }
            if (options.VerifyButtonStates)
            {
                verified &= RunButtonStateVerification();
            }
            if (options.VerifySpaceStates)
            {
                verified &= RunSpaceStateVerification();
            }
            if (options.VerifySelectStates)
            {
                verified &= RunSelectStateVerification();
            }
            if (options.VerifyAutoCompleteStates)
            {
                verified &= RunAutoCompleteStateVerification();
            }
            if (options.VerifyAvatarStates)
            {
                verified &= RunAvatarStateVerification();
            }
            if (options.VerifyBadgeStates)
            {
                verified &= RunBadgeStateVerification();
            }
            if (options.VerifyButtonSpinnerStates)
            {
                verified &= RunButtonSpinnerStateVerification();
            }
            if (options.VerifyCardStates)
            {
                verified &= RunCardStateVerification();
            }
            if (options.VerifyCarouselStates)
            {
                verified &= RunCarouselStateVerification();
            }
            if (options.VerifyCascaderStates)
            {
                verified &= RunCascaderStateVerification();
            }
            if (options.VerifyCheckBoxStates)
            {
                verified &= RunCheckBoxStateVerification();
            }
            if (options.VerifyCollapseStates)
            {
                verified &= RunCollapseStateVerification();
            }
            if (options.VerifyComboBoxStates)
            {
                verified &= RunComboBoxStateVerification();
            }
            if (options.VerifyDatePickerStates)
            {
                verified &= RunDatePickerStateVerification();
            }
            if (options.VerifyDescriptionsStates)
            {
                verified &= RunDescriptionsStateVerification();
            }
            if (options.VerifyDialogStates)
            {
                verified &= RunDialogStateVerification();
            }
            if (options.VerifyDrawerStates)
            {
                verified &= RunDrawerStateVerification();
            }
            if (options.VerifyExpanderStates)
            {
                verified &= RunExpanderStateVerification();
            }
            if (options.VerifyEmptyStates)
            {
                verified &= RunEmptyStateVerification();
            }
            if (options.VerifyFloatButtonStates)
            {
                verified &= RunFloatButtonStateVerification();
            }
            if (options.VerifyFlyoutStates)
            {
                verified &= RunFlyoutStateVerification();
            }
            if (options.VerifyFormStates)
            {
                verified &= RunFormStateVerification();
            }
            if (options.VerifyGroupBoxStates)
            {
                verified &= RunGroupBoxStateVerification();
            }
            if (options.VerifyImagePreviewerStates)
            {
                verified &= RunImagePreviewerStateVerification();
            }
            if (options.VerifyInputStates)
            {
                verified &= RunInputStateVerification();
            }
            if (options.VerifyListBoxStates)
            {
                verified &= RunListBoxStateVerification();
            }
            if (options.VerifyListViewStates)
            {
                verified &= RunListViewStateVerification();
            }
            if (options.VerifyMarqueeLabelStates)
            {
                verified &= RunMarqueeLabelStateVerification();
            }
            if (options.VerifyMentionsStates)
            {
                verified &= RunMentionsStateVerification();
            }
            if (options.VerifyMenuStates)
            {
                verified &= RunMenuStateVerification();
            }
            if (options.VerifyNavMenuStates)
            {
                verified &= RunNavMenuStateVerification();
            }
            if (options.VerifyMessageStates)
            {
                verified &= RunMessageStateVerification();
            }
            if (options.VerifyMessageBoxStates)
            {
                verified &= RunMessageBoxStateVerification();
            }
            if (options.VerifyNotificationStates)
            {
                verified &= RunNotificationStateVerification();
            }
            if (options.VerifyNumericUpDownStates)
            {
                verified &= RunNumericUpDownStateVerification();
            }
            if (options.VerifyOptionButtonGroupStates)
            {
                verified &= RunOptionButtonGroupStateVerification();
            }
            if (options.VerifyPaginationStates)
            {
                verified &= RunPaginationStateVerification();
            }
            if (options.VerifyPopupConfirmStates)
            {
                verified &= RunPopupConfirmStateVerification();
            }
            if (options.VerifyProgressBarStates)
            {
                verified &= RunProgressBarStateVerification();
            }
            if (options.VerifyRadioButtonStates)
            {
                verified &= RunRadioButtonStateVerification();
            }
            if (options.VerifyQRCodeStates)
            {
                verified &= RunQRCodeStateVerification();
            }
            if (options.VerifyRateStates)
            {
                verified &= RunRateStateVerification();
            }
            if (options.VerifyResultStates)
            {
                verified &= RunResultStateVerification();
            }
            if (options.VerifyScrollViewerStates)
            {
                verified &= RunScrollViewerStateVerification();
            }
            if (options.VerifySkeletonStates)
            {
                verified &= RunSkeletonStateVerification();
            }
            if (options.VerifySegmentedStates)
            {
                verified &= RunSegmentedStateVerification();
            }
            if (options.VerifySpinStates)
            {
                verified &= RunSpinStateVerification();
            }
            if (options.VerifySwitchStates)
            {
                verified &= RunSwitchStateVerification();
            }
            if (options.VerifyTabControlStates)
            {
                verified &= RunTabControlStateVerification();
            }
            if (options.VerifyStepsStates)
            {
                verified &= RunStepsStateVerification();
            }
            if (options.VerifyStatisticStates)
            {
                verified &= RunStatisticStateVerification();
            }
            if (options.VerifySplitViewStates)
            {
                verified &= RunSplitViewStateVerification();
            }
            if (options.VerifySplitterStates)
            {
                verified &= RunSplitterStateVerification();
            }
            if (options.VerifySliderStates)
            {
                verified &= RunSliderStateVerification();
            }
            if (options.VerifyTreeViewStates)
            {
                verified &= RunTreeViewStateVerification();
            }
            if (options.VerifyTransferStates)
            {
                verified &= RunTransferStateVerification();
            }
            if (options.VerifyTextBlockStates)
            {
                verified &= RunTextBlockStateVerification();
            }
            return verified ? 0 : 1;
        }

        var scenarios = CreateScenarios(options.Suite);
        foreach (var scenario in scenarios)
        {
            RunWarmup(scenario, Math.Min(5, options.Count));
        }

        var results = scenarios
            .Select(scenario => MeasureScenario(scenario, options.Count))
            .ToList();

        Console.WriteLine(RenderTable(results));

        if (!string.IsNullOrWhiteSpace(options.MarkdownOutputPath))
        {
            var markdown = RenderMarkdown(results, options);
            var fullPath = Path.GetFullPath(options.MarkdownOutputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, markdown, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine();
            Console.WriteLine($"Wrote markdown baseline: {fullPath}");
        }

        return 0;
    }

    private static void SetupAvalonia()
    {
        AppBuilder.Configure<PerfApplication>()
                  .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                  .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime());
    }

    private static IReadOnlyList<PerfScenario> CreateScenarios(string suite)
    {
        return suite.ToLowerInvariant() switch
        {
            "icon" => CreateIconScenarios(),
            "avatar" => CreateAvatarScenarios(),
            "badge" => CreateBadgeScenarios(),
            "button" => CreateButtonScenarios(),
            "buttonspinner" => CreateButtonSpinnerScenarios(),
            "card" => CreateCardScenarios(),
            "carousel" => CreateCarouselScenarios(),
            "cascader" => CreateCascaderScenarios(),
            "checkbox" => CreateCheckBoxScenarios(),
            "collapse" => CreateCollapseScenarios(),
            "combobox" => CreateComboBoxScenarios(),
            "datepicker" => CreateDatePickerScenarios(),
            "descriptions" => CreateDescriptionsScenarios(),
            "dialog" => CreateDialogScenarios(),
            "drawer" => CreateDrawerScenarios(),
            "expander" => CreateExpanderScenarios(),
            "empty" => CreateEmptyScenarios(),
            "flyouts" => CreateFlyoutScenarios(),
            "floatbutton" => CreateFloatButtonScenarios(),
            "form" => CreateFormScenarios(),
            "groupbox" => CreateGroupBoxScenarios(),
            "imagepreviewer" => CreateImagePreviewerScenarios(),
            "input" => CreateInputScenarios(),
            "listbox" => CreateListBoxScenarios(),
            "listview" => CreateListViewScenarios(),
            "marqueelabel" => CreateMarqueeLabelScenarios(),
            "mentions" => CreateMentionsScenarios(),
            "menu" => CreateMenuScenarios(),
            "navmenu" => CreateNavMenuScenarios(),
            "message" => CreateMessageScenarios(),
            "messagebox" => CreateMessageBoxScenarios(),
            "notification" => CreateNotificationScenarios(),
            "numericupdown" => CreateNumericUpDownScenarios(),
            "optionbuttongroup" => CreateOptionButtonGroupScenarios(),
            "pagination" => CreatePaginationScenarios(),
            "popupconfirm" => CreatePopupConfirmScenarios(),
            "progressbar" => CreateProgressBarScenarios(),
            "radiobutton" => CreateRadioButtonScenarios(),
            "qrcode" => CreateQRCodeScenarios(),
            "rate" => CreateRateScenarios(),
            "result" => CreateResultScenarios(),
            "scrollviewer" => CreateScrollViewerScenarios(),
            "skeleton" => CreateSkeletonScenarios(),
            "segmented" => CreateSegmentedScenarios(),
            "slider" => CreateSliderScenarios(),
            "spin" => CreateSpinScenarios(),
            "switch" => CreateSwitchScenarios(),
            "tabcontrol" => CreateTabControlScenarios(),
            "steps" => CreateStepsScenarios(),
            "statistic" => CreateStatisticScenarios(),
            "splitview" => CreateSplitViewScenarios(),
            "splitter" => CreateSplitterScenarios(),
            "space" => CreateSpaceScenarios(),
            "select" => CreateSelectScenarios(),
            "autocomplete" => CreateAutoCompleteScenarios(),
            "textblock" => CreateTextBlockScenarios(),
            "popup" => CreatePopupScenarios(),
            "tag" => CreateTagScenarios(),
            "separator" => CreateSeparatorScenarios(),
            _ => CreateAddOnScenarios()
        };
    }
}
