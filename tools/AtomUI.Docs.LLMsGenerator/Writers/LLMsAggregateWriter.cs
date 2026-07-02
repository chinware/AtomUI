using System.Text;

namespace AtomUI.Docs.LLMsGenerator.Writers;

public static class LLMsAggregateWriter
{
    public static string Write(string title, IReadOnlyList<GeneratedLLMsFile> files, string fileName)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {title}");
        builder.AppendLine();
        builder.AppendLine(LLMsWriterConstants.GeneratedMarker);
        builder.AppendLine();

        foreach (var file in files.Where(file => file.Path.EndsWith($"/{fileName}", StringComparison.Ordinal)))
        {
            var relativePath = "./" + Path.GetRelativePath("docs/AI/llms", file.Path).Replace(Path.DirectorySeparatorChar, '/');
            builder.AppendLine($"Source: {relativePath}");
            builder.AppendLine();
            builder.AppendLine(file.Content.Trim());
            builder.AppendLine();
        }

        return builder.ToString().ReplaceLineEndings("\n").TrimEnd() + "\n";
    }
}
