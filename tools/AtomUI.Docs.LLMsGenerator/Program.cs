using AtomUI.Docs.LLMsGenerator.Catalog;
using AtomUI.Docs.LLMsGenerator.Config;
using AtomUI.Docs.LLMsGenerator.Reader;
using AtomUI.Docs.LLMsGenerator.Verification;
using AtomUI.Docs.LLMsGenerator.Writers;

namespace AtomUI.Docs.LLMsGenerator;

internal static class Program
{
    private const string DefaultConfigPath = "docs/generated/llms.config.json";

    public static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            PrintUsage();
            return args.Length == 0 ? 1 : 0;
        }

        try
        {
            var command = args[0];
            var configPath = ReadConfigPath(args);
            var repositoryRoot = Directory.GetCurrentDirectory();
            var config = LLMsGeneratorConfigReader.Read(Path.GetFullPath(configPath));

            return command switch
            {
                "generate" => RunGenerate(repositoryRoot, config),
                "verify" => RunVerify(repositoryRoot, config),
                _ => Fail($"Unknown command '{command}'.")
            };
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static int RunGenerate(string repositoryRoot, LLMsGeneratorConfig config)
    {
        var models = ReadModels(repositoryRoot, config);
        var sourceDiagnostics = models.SelectMany(LLMsVerifier.ValidateSource).ToArray();
        if (sourceDiagnostics.Length > 0)
        {
            foreach (var diagnostic in sourceDiagnostics)
            {
                Console.Error.WriteLine(diagnostic);
            }

            return 1;
        }

        var files = LLMsDocumentationWriter.WriteAll(models);
        LLMsDocumentationWriter.WriteFiles(repositoryRoot, files);
        Console.WriteLine($"Generated {models.Count} controls and {files.Count} LLMS files for {config.ProjectId}.");
        return 0;
    }

    private static int RunVerify(string repositoryRoot, LLMsGeneratorConfig config)
    {
        var controls = DiscoverControls(repositoryRoot, config);
        var missingFiles = controls.SelectMany(EnumerateRequiredSourceFiles)
                                   .Where(path => !File.Exists(path))
                                   .Select(path => $"Missing source document: {path}");
        var models = ControlDocReader.ReadAll(repositoryRoot, controls);
        var files = LLMsDocumentationWriter.WriteAll(models);
        var diagnostics = missingFiles
            .Concat(models.SelectMany(LLMsVerifier.ValidateSource))
            .Concat(LLMsVerifier.VerifyGeneratedFiles(repositoryRoot, files))
            .ToArray();

        if (diagnostics.Length > 0)
        {
            foreach (var diagnostic in diagnostics)
            {
                Console.Error.WriteLine(diagnostic);
            }

            return 1;
        }

        Console.WriteLine($"Verified {models.Count} controls and {files.Count} LLMS files for {config.ProjectId}.");
        return 0;
    }

    private static IReadOnlyList<ControlDocModel> ReadModels(string repositoryRoot, LLMsGeneratorConfig config)
    {
        var controls = DiscoverControls(repositoryRoot, config);
        return ControlDocReader.ReadAll(repositoryRoot, controls);
    }

    private static IReadOnlyList<ControlDocumentInfo> DiscoverControls(
        string repositoryRoot,
        LLMsGeneratorConfig config)
    {
        return config.ControlSets
            .SelectMany(controlSet => ControlInventory.Discover(
                repositoryRoot,
                controlSet,
                config.OutputRoot,
                config.DefaultLanguage))
            .ToArray();
    }

    private static IEnumerable<string> EnumerateRequiredSourceFiles(ControlDocumentInfo control)
    {
        yield return control.OverviewPath;
        yield return control.ImplementationPath;
        yield return control.ChangelogPath;
    }

    private static string ReadConfigPath(IReadOnlyList<string> args)
    {
        for (var index = 1; index < args.Count; index++)
        {
            if (args[index] == "--config")
            {
                if (index + 1 >= args.Count)
                {
                    throw new ArgumentException("Missing value for --config.");
                }

                return args[index + 1];
            }
        }

        return DefaultConfigPath;
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        PrintUsage();
        return 1;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- generate --config docs/generated/llms.config.json");
        Console.WriteLine("  dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- verify --config docs/generated/llms.config.json");
    }
}
