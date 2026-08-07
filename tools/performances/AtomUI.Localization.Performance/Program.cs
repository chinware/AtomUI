namespace AtomUI.Localization.Performance;

internal static class Program
{
    private const int DefaultCount = 60;

    public static int Main(string[] args)
    {
        try
        {
            var options = Parse(args);
            return LocalizationScenarios.Run(
                options.Count,
                options.MarkdownOutputPath,
                options.VerifyStates);
        }
        catch (ArgumentException exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 2;
        }
    }

    private static LocalizationPerformanceOptions Parse(string[] args)
    {
        var count = DefaultCount;
        string? markdownOutputPath = null;
        var verifyStates = false;
        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--count" when index + 1 < args.Length &&
                                     int.TryParse(args[index + 1], out var parsedCount) &&
                                     parsedCount > 0:
                    count = parsedCount;
                    index++;
                    break;
                case "--markdown" when index + 1 < args.Length:
                    markdownOutputPath = args[++index];
                    break;
                case "--suite" when index + 1 < args.Length &&
                                     args[index + 1].Equals("localization", StringComparison.OrdinalIgnoreCase):
                    index++;
                    break;
                case "--verify-localization-states":
                    verifyStates = true;
                    break;
                default:
                    throw new ArgumentException($"Unsupported localization performance argument '{args[index]}'.");
            }
        }

        return new LocalizationPerformanceOptions(count, markdownOutputPath, verifyStates);
    }

    private sealed record LocalizationPerformanceOptions(
        int Count,
        string? MarkdownOutputPath,
        bool VerifyStates);
}
