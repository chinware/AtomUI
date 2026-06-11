using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Language;

internal class LanguageProviderPoolClassSourceWriter
{
    private readonly SourceProductionContext _context;
    private readonly List<LanguageInfo> _languageInfos;
    private readonly Dictionary<string, List<LanguageInfo>> _languagesById;

    public LanguageProviderPoolClassSourceWriter(SourceProductionContext context, List<LanguageInfo> classes)
    {
        _context       = context;
        _languageInfos = classes.OrderBy(info => info.Namespace).ThenBy(info => info.ClassName).ToList();
        _languagesById = _languageInfos.GroupBy(info => info.LanguageId)
                                        .ToDictionary(group => group.Key, group => group.ToList());
    }

    public void Write()
    {
        var sourceText = SourceText.From(BuildSourceText(), Encoding.UTF8);
        _context.AddSource("LanguageProviderPool.g.cs", sourceText);
    }

    private string BuildSourceText()
    {
        var builder = new StringBuilder();
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using AtomUI.Theme.Language;");
        builder.AppendLine("using Avalonia.Controls;");
        builder.AppendLine();
        builder.AppendLine("namespace AtomUI.Theme.Language");
        builder.AppendLine("{");

        foreach (var languageInfo in _languageInfos)
        {
            AppendLanguageProviderClass(builder, languageInfo);
            builder.AppendLine();
        }

        AppendLanguageProviderPoolClass(builder);

        builder.AppendLine("}");
        return builder.ToString();
    }

    private void AppendLanguageProviderClass(StringBuilder builder, LanguageInfo languageInfo)
    {
        var providerClassName = GetGeneratedProviderClassName(languageInfo);
        var resourceKindType  = $"global::{languageInfo.ResourceKindTypeFullName}";
        var providerType      = $"global::{languageInfo.ProviderTypeFullName}";

        builder.AppendLine($"    internal sealed class {providerClassName} : LanguageProvider");
        builder.AppendLine("    {");
        builder.AppendLine($"        public {providerClassName}()");
        builder.AppendLine($"            : base(LanguageCode.{languageInfo.LanguageCode}, {ToStringLiteral(languageInfo.LanguageId)})");
        builder.AppendLine("        {");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        protected override System.Type GetResourceKindType()");
        builder.AppendLine("        {");
        builder.AppendLine($"            return typeof({resourceKindType});");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        public override void BuildResourceDictionary(IResourceDictionary dictionary)");
        builder.AppendLine("        {");
        builder.AppendLine("            var resourceKindType = GetResourceKindType();");
        builder.AppendLine("            try");
        builder.AppendLine("            {");

        foreach (var key in GetResourceKeys(languageInfo))
        {
            if (languageInfo.Items.ContainsKey(key))
            {
                builder.AppendLine($"                dictionary[{resourceKindType}.{key}] = {providerType}.{key};");
            }
            else
            {
                var message = $"Language item: {key} does not exist in {languageInfo.ProviderTypeFullName}";
                builder.AppendLine($"                throw new System.Exception({ToStringLiteral(message)});");
            }
        }

        builder.AppendLine("            }");
        builder.AppendLine("            catch (System.Exception)");
        builder.AppendLine("            {");
        builder.AppendLine("                LogBuildResourceDictionaryError(resourceKindType);");
        builder.AppendLine("                throw;");
        builder.AppendLine("            }");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private void AppendLanguageProviderPoolClass(StringBuilder builder)
    {
        builder.AppendLine("    internal class LanguageProviderPool");
        builder.AppendLine("    {");
        builder.AppendLine("        internal static IList<LanguageProvider> GetLanguageProviders()");
        builder.AppendLine("        {");
        builder.AppendLine($"            List<LanguageProvider> languageProviders = new List<LanguageProvider>({_languageInfos.Count});");

        foreach (var languageInfo in _languageInfos)
        {
            builder.AppendLine($"            languageProviders.Add(new {GetGeneratedProviderClassName(languageInfo)}());");
        }

        builder.AppendLine("            return languageProviders;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private IEnumerable<string> GetResourceKeys(LanguageInfo languageInfo)
    {
        if (!_languagesById.TryGetValue(languageInfo.LanguageId, out var languageInfos))
        {
            return Array.Empty<string>();
        }

        return languageInfos.SelectMany(info => info.Items.Keys)
                            .Distinct()
                            .OrderBy(key => key);
    }

    private static string GetGeneratedProviderClassName(LanguageInfo languageInfo)
    {
        return $"{NormalizeIdentifierPart(languageInfo.LanguageId)}{NormalizeIdentifierPart(languageInfo.LanguageCode)}LanguageProvider";
    }

    private static string NormalizeIdentifierPart(string value)
    {
        var builder   = new StringBuilder();
        var upperNext = true;

        foreach (var ch in value)
        {
            if (!char.IsLetterOrDigit(ch))
            {
                upperNext = true;
                continue;
            }

            if (builder.Length == 0 && char.IsDigit(ch))
            {
                builder.Append('_');
            }

            builder.Append(upperNext ? char.ToUpperInvariant(ch) : ch);
            upperNext = false;
        }

        return builder.Length == 0 ? "Language" : builder.ToString();
    }

    private static string ToStringLiteral(string value)
    {
        return SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(value))
                            .ToFullString();
    }
}
