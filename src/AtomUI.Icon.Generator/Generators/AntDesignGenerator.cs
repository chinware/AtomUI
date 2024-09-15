using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace AtomUI.Icon.Generators;

/// <summary>
/// 目前仅能针对 AntDesign 暂时不可移植
/// </summary>
[Generator]
public class AntDesignGenerator : IIncrementalGenerator
{
    private readonly SvgParser _svgParser;
    private List<string> _twotoneTplPrimaryColors;
    private readonly List<string> _twotoneTplSecondaryColors;

    public AntDesignGenerator()
    {
        _svgParser = new SvgParser();
        _twotoneTplPrimaryColors = new List<string>
        {
            "#333"
        };
        _twotoneTplSecondaryColors = new List<string>
        {
            "#E6E6E6",
            "#D9D9D9",
            "#D8D8D8"
        };
    }

    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var textFiles = initContext.AdditionalTextsProvider
                                   .Where(static file =>
                                       file.Path.EndsWith(".svg"));

        var svgFileInfos = textFiles.Select((text, cancellationToken) =>
        {
            var name = Path.GetFileNameWithoutExtension(text.Path);
            // 暂时只支持拉丁字符
            name = Regex.Replace(name, @"-([a-zA-Z0-9])",
                match => match.Groups[1].ToString().ToUpper());
            name = name[0].ToString().ToUpper() + name.Substring(1);
            var parentDir = Path.GetFileName(Path.GetDirectoryName(text.Path));
            return new SvgFileInfo
            {
                Name        = name,
                FileContent = text.GetText(cancellationToken)!.ToString(),
                FilePath    = text.Path,
                ThemeType   = parentDir!
            };
        }).Collect();
        var assembleInfo =
            initContext.CompilationProvider.Select((compilation, token) => { return compilation.Assembly; });

        var mergeInfo = assembleInfo.Combine(svgFileInfos);

        initContext.RegisterSourceOutput(mergeInfo, (ctx, mergeInfo) =>
        {
            var packageName = mergeInfo.Left.Name;
            var fileInfos   = mergeInfo.Right;
            GenerateIconKind(packageName, fileInfos, ctx);
            GenerateIconPackage(packageName, fileInfos, ctx);
        });
    }

    private void GenerateIconKind(string ns, ImmutableArray<SvgFileInfo> fileInfos, SourceProductionContext ctx)
    {
        var packageName = GetPackageNameFromNs(ns);
        var sourceText  = new StringBuilder();
        sourceText.AppendLine("///");
        sourceText.AppendLine("/// This code is auto generated. Do not amend.");
        sourceText.AppendLine("///");
        sourceText.AppendLine($"namespace {ns};");
        sourceText.AppendLine($"public enum {packageName}IconKind");
        sourceText.AppendLine("{");
        for (var i = 0; i < fileInfos.Length; ++i)
        {
            var info = fileInfos[i];
            sourceText.AppendLine($"   {info.Name}{info.ThemeType} = {i + 1},");
        }

        sourceText.AppendLine("}");
        ctx.AddSource($"{packageName}IconKind.g.cs", sourceText.ToString());
    }

    private void GenerateIconPackage(string ns, ImmutableArray<SvgFileInfo> fileInfos, SourceProductionContext ctx)
    {
        var packageName = GetPackageNameFromNs(ns);
        var sourceText  = new StringBuilder();
        sourceText.AppendLine("///");
        sourceText.AppendLine("/// This code is auto generated. Do not amend.");
        sourceText.AppendLine("///");
        sourceText.AppendLine("using Avalonia;");
        sourceText.AppendLine($"namespace {ns};");
        sourceText.AppendLine($"public partial class {packageName}IconPackage");
        sourceText.AppendLine("{");
        foreach (var info in fileInfos)
        {
            var svgParsedInfo = _svgParser.Parse(info.FileContent);
            var viewBox       = svgParsedInfo.ViewBox;
            sourceText.Append($"   public static readonly IconInfo {info.Name}{info.ThemeType}");
            sourceText.Append(" = new IconInfo(");
            sourceText.Append($"\"{info.Name}{info.ThemeType}\", ");
            if (info.ThemeType == "TwoTone")
            {
                sourceText.Append($"new Rect({viewBox.X}, {viewBox.Y}, {viewBox.Width}, {viewBox.Height}), ");
                sourceText.Append("new List<GeometryData>{");
                // 需要判断主要颜色和次要颜色
                for (var i = 0; i < svgParsedInfo.PathInfos.Count; i++)
                {
                    var pathInfo  = svgParsedInfo.PathInfos[i];
                    var isPrimary = true;
                    if (pathInfo.FillColor != null && _twotoneTplSecondaryColors.Contains(pathInfo.FillColor))
                    {
                        isPrimary = false;
                    }

                    sourceText.Append($"new GeometryData(\"{pathInfo.Data}\", {isPrimary.ToString().ToLower()})");
                    if (i != svgParsedInfo.PathInfos.Count - 1)
                    {
                        sourceText.Append(", ");
                    }
                }
            }
            else
            {
                sourceText.Append($"IconThemeType.{info.ThemeType}, ");
                sourceText.Append($"new Rect({viewBox.X}, {viewBox.Y}, {viewBox.Width}, {viewBox.Height}), ");
                sourceText.Append("new List<GeometryData>{");
                for (var i = 0; i < svgParsedInfo.PathInfos.Count; i++)
                {
                    var pathInfo = svgParsedInfo.PathInfos[i];
                    sourceText.Append($"new GeometryData(\"{pathInfo.Data}\", true)");
                    if (i != svgParsedInfo.PathInfos.Count - 1)
                    {
                        sourceText.Append(", ");
                    }
                }
            }

            sourceText.Append("});\n");
        }

        sourceText.Append("\n");

        sourceText.AppendLine("   private partial void SetupIconPool()");
        sourceText.AppendLine("   {");
        // 生成初始化 icon pool 代码
        for (var i = 0; i < fileInfos.Length; ++i)
        {
            var info = fileInfos[i];
            sourceText.AppendLine(
                $"      _iconPool.Add((int)AntDesignIconKind.{info.Name}{info.ThemeType}, {info.Name}{info.ThemeType});");
        }

        sourceText.AppendLine("   }");
        sourceText.AppendLine("}");

        ctx.AddSource($"{packageName}IconPackage.g.cs", sourceText.ToString());
    }

    private string GetPackageNameFromNs(string ns)
    {
        var parts = ns.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Last();
    }
}