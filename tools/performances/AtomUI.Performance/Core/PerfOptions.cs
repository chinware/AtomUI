namespace AtomUI.Performance;

internal sealed record PerfOptions(
    int Count,
    string Suite,
    string? MarkdownOutputPath,
    bool VerifyAccessories,
    bool VerifyEffectiveBrushes,
    bool VerifyAddonStates,
    bool VerifyAntDesignMetadata,
    bool VerifyIconHiddenSlots,
    bool VerifyIconProviderCache,
    bool VerifyButtonStates,
    bool VerifySpaceStates)
{
    public static PerfOptions Parse(string[] args)
    {
        var count                  = Program.DefaultCount;
        var suite                  = "addon";
        string? markdownOutput     = null;
        var verifyAccessories      = false;
        var verifyEffectiveBrushes = false;
        var verifyAddonStates      = false;
        var verifyAntDesignMetadata = false;
        var verifyIconHiddenSlots  = false;
        var verifyIconProviderCache = false;
        var verifyButtonStates    = false;
        var verifySpaceStates     = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--count" when i + 1 < args.Length && int.TryParse(args[i + 1], out var parsedCount):
                    count = parsedCount;
                    i++;
                    break;
                case "--markdown" when i + 1 < args.Length:
                    markdownOutput = args[i + 1];
                    i++;
                    break;
                case "--suite" when i + 1 < args.Length:
                    suite = args[i + 1];
                    i++;
                    break;
                case "--verify-accessories":
                    verifyAccessories = true;
                    break;
                case "--verify-effective-brushes":
                    verifyEffectiveBrushes = true;
                    break;
                case "--verify-addon-states":
                    verifyAddonStates = true;
                    break;
                case "--verify-antdesign-metadata":
                    verifyAntDesignMetadata = true;
                    break;
                case "--verify-icon-hidden-slots":
                    verifyIconHiddenSlots = true;
                    break;
                case "--verify-icon-provider-cache":
                    verifyIconProviderCache = true;
                    break;
                case "--verify-button-states":
                    verifyButtonStates = true;
                    break;
                case "--verify-space-states":
                    verifySpaceStates = true;
                    break;
            }
        }

        return new PerfOptions(
            Math.Max(1, count),
            suite,
            markdownOutput,
            verifyAccessories,
            verifyEffectiveBrushes,
            verifyAddonStates,
            verifyAntDesignMetadata,
            verifyIconHiddenSlots,
            verifyIconProviderCache,
            verifyButtonStates,
            verifySpaceStates);
    }
}
