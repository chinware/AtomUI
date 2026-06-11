using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AtomUI.Generator.Language;

internal class LanguageProviderConstructorSourceWriter
{
    private readonly SourceProductionContext _context;
    private readonly List<LanguageInfo> _languageInfos;

    public LanguageProviderConstructorSourceWriter(SourceProductionContext context, List<LanguageInfo> languageInfos)
    {
        _context       = context;
        _languageInfos = languageInfos.Where(info => info.ShouldGenerateExplicitConstructor)
                                      .OrderBy(info => info.Namespace)
                                      .ThenBy(info => info.ClassName)
                                      .ToList();
    }

    public void Write()
    {
        if (_languageInfos.Count == 0)
        {
            return;
        }

        _context.AddSource("LanguageProviderConstructors.g.cs", GeneratedSourceText.From(BuildSourceText()));
    }

    private string BuildSourceText()
    {
        var builder = new StringBuilder();

        foreach (var languageInfo in _languageInfos)
        {
            AppendLanguageProviderConstructor(builder, languageInfo);
            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static void AppendLanguageProviderConstructor(StringBuilder builder, LanguageInfo languageInfo)
    {
        builder.AppendLine($"namespace {languageInfo.Namespace}");
        builder.AppendLine("{");
        builder.AppendLine($"    {languageInfo.Accessibility} partial class {languageInfo.ClassName}");
        builder.AppendLine("    {");
        builder.AppendLine($"        public {languageInfo.ClassName}()");
        builder.AppendLine($"            : base(global::AtomUI.Theme.Language.LanguageCode.{languageInfo.LanguageCode}, {ToStringLiteral(languageInfo.LanguageId)})");
        builder.AppendLine("        {");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");
    }

    private static string ToStringLiteral(string value)
    {
        return SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(value))
                            .ToFullString();
    }
}
