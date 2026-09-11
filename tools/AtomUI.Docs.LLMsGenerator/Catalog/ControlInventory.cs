using AtomUI.Docs.LLMsGenerator.Config;

namespace AtomUI.Docs.LLMsGenerator.Catalog;

public static class ControlInventory
{
    public const string DefaultOutputRoot = "docs/AI/generated/llms";
    public const string DefaultLanguage = "cn";

    public static IReadOnlyList<ControlDocumentInfo> Discover(
        string repositoryRoot,
        LLMsControlSetConfig controlSet,
        string outputRoot = DefaultOutputRoot,
        string language = DefaultLanguage)
    {
        if (string.IsNullOrWhiteSpace(repositoryRoot))
        {
            throw new ArgumentException("Repository root is required.", nameof(repositoryRoot));
        }

        ArgumentNullException.ThrowIfNull(controlSet);

        var docsRoot = ResolvePath(repositoryRoot, controlSet.DocsRoot);
        if (!Directory.Exists(docsRoot))
        {
            throw new DirectoryNotFoundException($"Control docs root was not found: {docsRoot}");
        }

        var categoryDirectories = Directory.GetDirectories(docsRoot)
                                           .Where(directory => !IsHidden(directory))
                                           .Select(directory => new
                                           {
                                               Name = Path.GetFileName(directory),
                                               Path = directory
                                           })
                                           .Where(directory => !string.IsNullOrWhiteSpace(directory.Name))
                                           .ToDictionary(
                                               directory => directory.Name!,
                                               directory => directory.Path,
                                               StringComparer.Ordinal);

        var orderedCategoryNames = GetOrderedCategories(categoryDirectories.Keys, controlSet.CategoryOrder);
        var controls = new List<ControlDocumentInfo>();

        foreach (var categoryName in orderedCategoryNames)
        {
            var categoryDirectory = categoryDirectories[categoryName];
            var controlDirectories = Directory.GetDirectories(categoryDirectory)
                                              .Where(directory => !IsHidden(directory))
                                              .OrderBy(Path.GetFileName, StringComparer.Ordinal);

            foreach (var controlDirectory in controlDirectories)
            {
                var controlName = Path.GetFileName(controlDirectory);
                if (string.IsNullOrWhiteSpace(controlName))
                {
                    continue;
                }

                controls.Add(new ControlDocumentInfo
                {
                    ControlSetId = controlSet.Id,
                    Platform = controlSet.Platform,
                    Category = categoryName,
                    Name = controlName,
                    DirectoryPath = controlDirectory,
                    GalleryRoot = ResolvePath(repositoryRoot, controlSet.GalleryRoot),
                    SourceRoots = controlSet.SourceRoots
                                             .Select(sourceRoot => ResolvePath(repositoryRoot, sourceRoot))
                                             .ToArray(),
                    OverviewPath = Path.Combine(controlDirectory, "overview.md"),
                    ImplementationPath = Path.Combine(controlDirectory, "implementation.md"),
                    TokenPath = GetOptionalFilePath(controlDirectory, "token.md"),
                    ChangelogPath = Path.Combine(controlDirectory, "changelog.md"),
                    OutputIndexPath = BuildOutputPath(outputRoot, controlName, $"index-{language}.md"),
                    OutputSemanticPath = BuildOutputPath(outputRoot, controlName, $"semantic-{language}.md")
                });
            }
        }

        return controls;
    }

    private static IReadOnlyList<string> GetOrderedCategories(
        IEnumerable<string?> categoryNames,
        IReadOnlyList<string> configuredOrder)
    {
        var availableCategories = categoryNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .ToHashSet(StringComparer.Ordinal);

        var orderedCategories = new List<string>();
        foreach (var categoryName in configuredOrder)
        {
            if (availableCategories.Remove(categoryName))
            {
                orderedCategories.Add(categoryName);
            }
        }

        orderedCategories.AddRange(availableCategories.OrderBy(name => name, StringComparer.Ordinal));
        return orderedCategories;
    }

    private static string? GetOptionalFilePath(string directory, string fileName)
    {
        var path = Path.Combine(directory, fileName);
        return File.Exists(path) ? path : null;
    }

    private static string BuildOutputPath(string outputRoot, string controlName, string fileName)
    {
        return string.Join("/", outputRoot.TrimEnd('/'), "controls", controlName, fileName);
    }

    private static string ResolvePath(string repositoryRoot, string path)
    {
        return Path.IsPathFullyQualified(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(repositoryRoot, path));
    }

    private static bool IsHidden(string directory)
    {
        var name = Path.GetFileName(directory);
        return name.StartsWith(".", StringComparison.Ordinal);
    }
}

public sealed class ControlDocumentInfo
{
    public string ControlSetId { get; init; } = string.Empty;

    public string Platform { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string DirectoryPath { get; init; } = string.Empty;

    public string GalleryRoot { get; init; } = string.Empty;

    public IReadOnlyList<string> SourceRoots { get; init; } = [];

    public string OverviewPath { get; init; } = string.Empty;

    public string ImplementationPath { get; init; } = string.Empty;

    public string? TokenPath { get; init; }

    public string ChangelogPath { get; init; } = string.Empty;

    public string OutputIndexPath { get; init; } = string.Empty;

    public string OutputSemanticPath { get; init; } = string.Empty;
}
