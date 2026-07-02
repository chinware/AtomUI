namespace AtomUI.Docs.LLMsGenerator.Markdown;

public static class MarkdownSectionParser
{
    public static MarkdownDocument Parse(string markdown, string path)
    {
        ArgumentNullException.ThrowIfNull(markdown);

        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        var headings = ReadHeadings(lines);
        var title = headings.FirstOrDefault(heading => heading.Level == 1)?.Text ?? string.Empty;
        var sections = BuildSections(lines, headings);

        return new MarkdownDocument(path, title, sections);
    }

    private static IReadOnlyList<MarkdownHeading> ReadHeadings(IReadOnlyList<string> lines)
    {
        var headings = new List<MarkdownHeading>();
        var inFence = false;
        var fenceMarker = string.Empty;

        for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
        {
            var line = lines[lineIndex];
            var trimmedStart = line.TrimStart();

            if (TryReadFenceMarker(trimmedStart, out var marker))
            {
                if (!inFence)
                {
                    inFence = true;
                    fenceMarker = marker;
                }
                else if (marker == fenceMarker)
                {
                    inFence = false;
                    fenceMarker = string.Empty;
                }

                continue;
            }

            if (inFence)
            {
                continue;
            }

            if (TryReadHeading(trimmedStart, out var level, out var text))
            {
                headings.Add(new MarkdownHeading(level, text, lineIndex));
            }
        }

        return headings;
    }

    private static IReadOnlyList<MarkdownSection> BuildSections(
        IReadOnlyList<string> lines,
        IReadOnlyList<MarkdownHeading> headings)
    {
        var sectionHeadings = headings.Where(heading => heading.Level > 1).ToArray();
        var sections = new List<MarkdownSection>(sectionHeadings.Length);

        foreach (var heading in sectionHeadings)
        {
            var contentStartLine = heading.LineIndex + 1;
            var contentEndLine = lines.Count;
            var nextBoundary = headings.FirstOrDefault(next =>
                next.LineIndex > heading.LineIndex && next.Level <= heading.Level);

            if (nextBoundary is not null)
            {
                contentEndLine = nextBoundary.LineIndex;
            }

            var content = string.Join('\n', lines.Skip(contentStartLine).Take(contentEndLine - contentStartLine));
            sections.Add(new MarkdownSection(heading.Text, heading.Level, heading.LineIndex + 1, content));
        }

        return sections;
    }

    private static bool TryReadFenceMarker(string trimmedStart, out string marker)
    {
        marker = string.Empty;

        if (trimmedStart.StartsWith("```", StringComparison.Ordinal))
        {
            marker = "```";
            return true;
        }

        if (trimmedStart.StartsWith("~~~", StringComparison.Ordinal))
        {
            marker = "~~~";
            return true;
        }

        return false;
    }

    private static bool TryReadHeading(string trimmedStart, out int level, out string text)
    {
        level = 0;
        text = string.Empty;

        while (level < trimmedStart.Length && level < 6 && trimmedStart[level] == '#')
        {
            level++;
        }

        if (level == 0 || level >= trimmedStart.Length || trimmedStart[level] != ' ')
        {
            return false;
        }

        text = trimmedStart[(level + 1)..].Trim();
        if (text.Length == 0)
        {
            return false;
        }

        return true;
    }

    private sealed record MarkdownHeading(int Level, string Text, int LineIndex);
}

public sealed class MarkdownDocument
{
    public MarkdownDocument(string path, string title, IReadOnlyList<MarkdownSection> sections)
    {
        Path = path;
        Title = title;
        Sections = sections;
    }

    public string Path { get; }

    public string Title { get; }

    public IReadOnlyList<MarkdownSection> Sections { get; }

    public MarkdownSection GetRequiredSection(string heading)
    {
        var matches = Sections.Where(section => section.Heading == heading).ToArray();
        return matches.Length switch
        {
            0 => throw new MarkdownSectionMissingException(Path, heading),
            1 => matches[0],
            _ => throw new MarkdownSectionDuplicateException(Path, heading)
        };
    }
}

public sealed class MarkdownSection
{
    public MarkdownSection(string heading, int level, int lineNumber, string content)
    {
        Heading = heading;
        Level = level;
        LineNumber = lineNumber;
        Content = content;
    }

    public string Heading { get; }

    public int Level { get; }

    public int LineNumber { get; }

    public string Content { get; }
}

public sealed class MarkdownSectionMissingException : Exception
{
    public MarkdownSectionMissingException(string path, string heading)
        : base($"Markdown document '{path}' is missing required section '{heading}'.")
    {
    }
}

public sealed class MarkdownSectionDuplicateException : Exception
{
    public MarkdownSectionDuplicateException(string path, string heading)
        : base($"Markdown document '{path}' contains duplicate section '{heading}'.")
    {
    }
}
