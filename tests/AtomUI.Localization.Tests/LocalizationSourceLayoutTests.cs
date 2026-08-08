using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LocalizationSourceLayoutTests
{
    private static readonly string[] s_expectedFiles =
    [
        "src/AtomUI.Localization/Services/IGeneratedApplicationLanguageBootstrap.cs",
        "src/AtomUI.Localization/Services/ILanguageManager.cs",
        "src/AtomUI.Localization/Services/ILocalizationBuilder.cs",
        "src/AtomUI.Localization/Services/ILocalizer.cs",
        "src/AtomUI.Localization/Services/LanguageManager.cs",
        "src/AtomUI.Localization/Services/LocalizationBuilder.cs",
        "src/AtomUI.Localization/Services/LocalizationHost.cs",
        "src/AtomUI.Localization/Services/LocalizationLogger.cs",
        "src/AtomUI.Localization/Services/Localizer.cs",
        "src/AtomUI.Localization/Resources/LanguageResourceExtension.cs",
        "src/AtomUI.Localization/Resources/LanguageResourceKeys.cs",
        "src/AtomUI.Localization/Resources/LanguageResourceProvider.cs",
        "src/AtomUI.Localization/LanguageContext.cs",
        "src/AtomUI.Localization/LanguageFallbackResolver.cs",
        "src/AtomUI.Localization/LanguageRevision.cs",
        "src/AtomUI.Localization/LanguageSnapshot.cs",
        "src/AtomUI.Localization/LanguageSnapshotBuilder.cs"
    ];

    private static readonly string[] s_retiredTypeNames =
    [
        "LocalizationRuntime",
        "LanguageRuntimeContext",
        "LanguageRuntimeRevision",
        "LanguageRuntimeResourceKeys",
        "LocalizationRuntimeLogger"
    ];

    [Fact]
    public void Source_Uses_The_A1_Compact_Responsibility_Grouping()
    {
        var root = FindRepoRoot();
        Directory.Exists(Path.Combine(root, "src", "AtomUI.Localization", "Runtime"))
                 .ShouldBeFalse("The retired Runtime directory must not remain as a catch-all area.");

        foreach (var relativePath in s_expectedFiles)
        {
            File.Exists(Path.Combine(root, relativePath))
                .ShouldBeTrue($"Expected localization source file '{relativePath}' was not found.");
        }

        var sourceRoots = new[]
        {
            Path.Combine(root, "src", "AtomUI.Localization"),
            Path.Combine(root, "src", "AtomUI.Core")
        };
        var violations = sourceRoots
                         .SelectMany(static sourceRoot =>
                             Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories))
                         .SelectMany(path => s_retiredTypeNames
                             .Where(retiredName => File.ReadAllText(path).Contains(
                                 retiredName,
                                 StringComparison.Ordinal))
                             .Select(retiredName => $"{Path.GetRelativePath(root, path)}: {retiredName}"))
                         .ToArray();

        violations.ShouldBeEmpty();
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(
                    directory.FullName,
                    "src",
                    "AtomUI.Localization",
                    "AtomUI.Localization.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the AtomUI repository root.");
    }
}
