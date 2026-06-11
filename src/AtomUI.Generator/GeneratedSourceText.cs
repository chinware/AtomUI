using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator;

internal static class GeneratedSourceText
{
    public static SourceText From(string source)
    {
        return SourceText.From(NormalizeLineEndings(source), Encoding.UTF8);
    }

    private static string NormalizeLineEndings(string source)
    {
        return source.Replace("\r\n", "\n")
                     .Replace("\r", "\n");
    }
}
