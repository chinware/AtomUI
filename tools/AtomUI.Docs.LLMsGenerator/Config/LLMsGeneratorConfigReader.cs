using System.Text.Json;

namespace AtomUI.Docs.LLMsGenerator.Config;

public static class LLMsGeneratorConfigReader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public static LLMsGeneratorConfig Read(string configPath)
    {
        if (string.IsNullOrWhiteSpace(configPath))
        {
            throw new ArgumentException("LLMS config path is required.", nameof(configPath));
        }

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException("LLMS config file was not found.", configPath);
        }

        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<LLMsGeneratorConfig>(json, JsonOptions);
        if (config is null)
        {
            throw new InvalidOperationException($"Unable to read LLMS config file: {configPath}");
        }

        Validate(config, configPath);
        return config;
    }

    private static void Validate(LLMsGeneratorConfig config, string configPath)
    {
        if (config.SchemaVersion != 1)
        {
            throw new InvalidOperationException(
                $"Unsupported LLMS config schemaVersion '{config.SchemaVersion}' in {configPath}.");
        }

        Require(config.ProjectId, nameof(config.ProjectId), configPath);
        Require(config.DisplayName, nameof(config.DisplayName), configPath);
        Require(config.DefaultLanguage, nameof(config.DefaultLanguage), configPath);
        Require(config.OutputRoot, nameof(config.OutputRoot), configPath);

        if (config.Languages.Count == 0)
        {
            throw new InvalidOperationException($"LLMS config '{configPath}' must define at least one language.");
        }

        if (!config.Languages.Contains(config.DefaultLanguage, StringComparer.Ordinal))
        {
            throw new InvalidOperationException(
                $"LLMS config '{configPath}' defaultLanguage must be present in languages.");
        }

        if (config.ControlSets.Count == 0)
        {
            throw new InvalidOperationException($"LLMS config '{configPath}' must define at least one control set.");
        }

        foreach (var controlSet in config.ControlSets)
        {
            Require(controlSet.Id, $"{nameof(config.ControlSets)}[].{nameof(controlSet.Id)}", configPath);
            Require(controlSet.Platform, $"{nameof(config.ControlSets)}[].{nameof(controlSet.Platform)}", configPath);
            Require(controlSet.DocsRoot, $"{nameof(config.ControlSets)}[].{nameof(controlSet.DocsRoot)}", configPath);
        }
    }

    private static void Require(string value, string fieldName, string configPath)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"LLMS config '{configPath}' must define '{fieldName}'.");
        }
    }
}
