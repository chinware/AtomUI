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
            options.VerifySelectStates)
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
            "button" => CreateButtonScenarios(),
            "space" => CreateSpaceScenarios(),
            "select" => CreateSelectScenarios(),
            _ => CreateAddOnScenarios()
        };
    }
}
