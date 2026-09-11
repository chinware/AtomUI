using System.Text.RegularExpressions;
using AtomUI.Docs.LLMsGenerator.Reader;
using AtomUI.Docs.LLMsGenerator.Writers;

namespace AtomUI.Docs.LLMsGenerator.Verification;

public static partial class LLMsVerifier
{
    private static readonly Regex ForbiddenExternalProjectRegex = new(
        string.Join('|', ["Ant\\s+Design", "ant\\s+design", "ant\\.design", "ant" + "d"]),
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static IReadOnlyList<string> ValidateSource(ControlDocModel model)
    {
        var diagnostics = new List<string>();

        if (!model.HasExplicitMetadata)
        {
            diagnostics.Add($"{model.ControlName}: overview.md lacks package / namespace / Gallery / status metadata.");
        }

        if (!model.HasExplicitExportSourceTable)
        {
            diagnostics.Add($"{model.ControlName}: overview.md lacks LLMS export source table.");
        }

        if (!model.HasExplicitSemanticParts)
        {
            diagnostics.Add($"{model.ControlName}: overview.md lacks LLMS semantic parts.");
        }

        if (!model.HasTokenDoc && !model.HasExplicitTokenSourceDescription)
        {
            diagnostics.Add($"{model.ControlName}: token.md is absent and overview.md does not explain token absence or reuse.");
        }

        if (model.OutputIndexPath != $"docs/AI/generated/llms/controls/{model.ControlName}/index-cn.md")
        {
            diagnostics.Add($"{model.ControlName}: generated output path is not controls/<control>/index-cn.md.");
        }

        if (model.OutputSemanticPath != $"docs/AI/generated/llms/controls/{model.ControlName}/semantic-cn.md")
        {
            diagnostics.Add($"{model.ControlName}: generated output path is not controls/<control>/semantic-cn.md.");
        }

        return diagnostics;
    }

    public static IReadOnlyList<string> ValidateGeneratedContent(string path, string content)
    {
        var diagnostics = new List<string>();
        if (!content.Contains(LLMsWriterConstants.GeneratedMarker, StringComparison.Ordinal))
        {
            diagnostics.Add($"{path}: generated marker is missing.");
        }

        if (content.Contains("components/", StringComparison.Ordinal) ||
            content.Contains("./components", StringComparison.Ordinal))
        {
            diagnostics.Add($"{path}: stale component path found.");
        }

        if (LanguageDirectoryRegex().IsMatch(content))
        {
            diagnostics.Add($"{path}: stale language-in-directory path found.");
        }

        if (PlaceholderRegex().IsMatch(content))
        {
            diagnostics.Add($"{path}: placeholder marker found.");
        }

        if (ForbiddenExternalProjectRegex.IsMatch(content))
        {
            diagnostics.Add($"{path}: forbidden external project name found.");
        }

        return diagnostics;
    }

    public static IReadOnlyList<string> VerifyGeneratedFiles(
        string repositoryRoot,
        IReadOnlyList<GeneratedLLMsFile> expectedFiles)
    {
        var diagnostics = new List<string>();
        var expectedPaths = expectedFiles.Select(file => file.Path).ToHashSet(StringComparer.Ordinal);

        foreach (var file in expectedFiles)
        {
            diagnostics.AddRange(ValidateGeneratedContent(file.Path, file.Content));

            var absolutePath = Path.Combine(repositoryRoot, file.Path);
            if (!File.Exists(absolutePath))
            {
                diagnostics.Add($"{file.Path}: generated file is missing.");
                continue;
            }

            var actual = File.ReadAllText(absolutePath).ReplaceLineEndings("\n");
            if (actual != file.Content.ReplaceLineEndings("\n"))
            {
                diagnostics.Add($"{file.Path}: generated file is stale.");
            }
        }

        var outputRoot = Path.Combine(repositoryRoot, "docs/AI/generated/llms");
        if (Directory.Exists(outputRoot))
        {
            foreach (var path in Directory.GetFiles(outputRoot, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(repositoryRoot, path).Replace(Path.DirectorySeparatorChar, '/');
                if (IsGeneratedLLMsPath(relativePath) && !expectedPaths.Contains(relativePath))
                {
                    diagnostics.Add($"{relativePath}: stale generated file found.");
                }
            }
        }

        return diagnostics;
    }

    private static bool IsGeneratedLLMsPath(string path)
    {
        return path is "docs/AI/generated/llms/llms.txt" or "docs/AI/generated/llms/llms-full-cn.txt" or "docs/AI/generated/llms/llms-semantic-cn.md" ||
               path.StartsWith("docs/AI/generated/llms/controls/", StringComparison.Ordinal);
    }

    [GeneratedRegex(@"-[a-z][a-z]/(?:index|semantic)-[a-z][a-z]\.md", RegexOptions.IgnoreCase)]
    private static partial Regex LanguageDirectoryRegex();

    [GeneratedRegex(@"\b(?:TODO|TBD)\b", RegexOptions.IgnoreCase)]
    private static partial Regex PlaceholderRegex();

}
