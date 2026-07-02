using AtomUI.Docs.LLMsGenerator.Reader;

namespace AtomUI.Docs.LLMsGenerator.Writers;

public static class LLMsDocumentationWriter
{
    public static IReadOnlyList<GeneratedLLMsFile> WriteAll(IReadOnlyList<ControlDocModel> models)
    {
        var files = new List<GeneratedLLMsFile>();
        foreach (var model in models)
        {
            files.Add(new GeneratedLLMsFile(model.OutputIndexPath, LLMsControlWriter.Write(model)));
            files.Add(new GeneratedLLMsFile(model.OutputSemanticPath, LLMsSemanticWriter.Write(model)));
        }

        files.Add(new GeneratedLLMsFile("docs/AI/llms/llms.txt", LLMsIndexWriter.Write(models)));
        files.Add(new GeneratedLLMsFile("docs/AI/llms/llms-full-cn.txt",
            LLMsAggregateWriter.Write("AtomUI Desktop Controls Full CN", files, "index-cn.md")));
        files.Add(new GeneratedLLMsFile("docs/AI/llms/llms-semantic-cn.md",
            LLMsAggregateWriter.Write("AtomUI Desktop Controls Semantic CN", files, "semantic-cn.md")));
        return files;
    }

    public static void WriteFiles(string repositoryRoot, IReadOnlyList<GeneratedLLMsFile> files)
    {
        PruneStaleGeneratedFiles(repositoryRoot, files);

        foreach (var file in files)
        {
            var path = Path.Combine(repositoryRoot, file.Path);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, file.Content);
        }
    }

    private static void PruneStaleGeneratedFiles(string repositoryRoot, IReadOnlyList<GeneratedLLMsFile> files)
    {
        var expectedPaths = files.Select(file => file.Path).ToHashSet(StringComparer.Ordinal);
        var outputRoot = Path.Combine(repositoryRoot, "docs/AI/llms");
        if (!Directory.Exists(outputRoot))
        {
            return;
        }

        foreach (var path in Directory.GetFiles(outputRoot, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(repositoryRoot, path).Replace(Path.DirectorySeparatorChar, '/');
            if (IsGeneratedLLMsPath(relativePath) && !expectedPaths.Contains(relativePath))
            {
                File.Delete(path);
            }
        }

        var controlsRoot = Path.Combine(outputRoot, "controls");
        if (!Directory.Exists(controlsRoot))
        {
            return;
        }

        foreach (var directory in Directory.GetDirectories(controlsRoot, "*", SearchOption.AllDirectories)
                                           .OrderByDescending(directory => directory.Length))
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory);
            }
        }
    }

    private static bool IsGeneratedLLMsPath(string path)
    {
        return path is "docs/AI/llms/llms.txt" or "docs/AI/llms/llms-full-cn.txt" or "docs/AI/llms/llms-semantic-cn.md" ||
               path.StartsWith("docs/AI/llms/controls/", StringComparison.Ordinal);
    }
}
