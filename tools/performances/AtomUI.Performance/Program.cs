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
            options.VerifyImagePreviewerStates)
        {
            var verified = true;
            if (options.VerifyAccessories)
            {
                verified &= RunAccessoryLifecycleVerification();
            }
            if (options.VerifyEffectiveBrushes)
            {
                verified &= RunEffectiveBrushVerification();
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
            "space" => CreateSpaceScenarios(),
            "select" => CreateSelectScenarios(),
            "autocomplete" => CreateAutoCompleteScenarios(),
            _ => CreateAddOnScenarios()
        };
    }
}
