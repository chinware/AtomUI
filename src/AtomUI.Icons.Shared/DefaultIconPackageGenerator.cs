using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Controls;

namespace AtomUI.Icons;

public abstract class DefaultIconPackageGenerator : AbstractIconPackageGenerator
{
    public DefaultIconPackageGenerator(string sourcePath, string targetPath, string? generatedIconsDir = null)
        : base(sourcePath, targetPath, generatedIconsDir)
    {
    }
    
    protected override IEnumerable<IconFileInfo> ScanIconFilesRecursively(string sourcePath)
    {
        foreach (var svgFilePath in Directory.EnumerateFiles(sourcePath,  "*.svg"))
        {
            var name = NormalizeClassName(Path.GetFileNameWithoutExtension(svgFilePath));
            var parentDir = Path.GetFileName(Path.GetDirectoryName(svgFilePath));
            Debug.Assert(!string.IsNullOrEmpty(parentDir));
            var iconTheme = IconThemeType.Filled;
            if (Enum.TryParse<IconThemeType>(parentDir, true, out var value))
            {
                iconTheme = value;
            }
            yield return new IconFileInfo()
            {
                Name      = name,
                FilePath  =  svgFilePath,
                ThemeType = iconTheme
            };
        }
        
        foreach (var subPath in Directory.EnumerateDirectories(sourcePath))
        {
            foreach (var svgFilePath in ScanIconFilesRecursively(subPath))
            {
                yield return svgFilePath;
            }
        }
    }

    private static string NormalizeClassName(string name)
    {
        name = Regex.Replace(name, @"-([a-zA-Z0-9])",
            match => match.Groups[1].ToString().ToUpper());
        name = name[0].ToString().ToUpper() + name.Substring(1);
        return name;
    }

    protected override async Task GenerateIconPackageKindAsync()
    {
        await using var output = new FileStream(Path.Combine(GeneratedIconsPath, $"{PackageName}IconKind.g.cs"), FileMode.Create, FileAccess.Write);
        var sourceText  = new StringBuilder();
        sourceText.AppendLine("///");
        sourceText.AppendLine("/// This code is auto generated. Do not amend.");
        sourceText.AppendLine("///");
        sourceText.AppendLine($"namespace {PackageNamespace};");
        sourceText.AppendLine($"public enum {PackageName}IconKind");
        sourceText.AppendLine("{");
        for (var i = 0; i < IconFiles.Count; ++i)
        {
            var info = IconFiles[i];
            sourceText.AppendLine($"    {info.Name}{info.ThemeType} = {i + 1},");
        }
        
        sourceText.AppendLine("}");
        await output.WriteAsync(Encoding.UTF8.GetBytes(sourceText.ToString()));
    }

    protected override async Task GenerateIconProviderFactoryAsync()
    {
        await using var output = new FileStream(Path.Combine(TargetPath, $"{PackageName}IconProvider.Factory.g.cs"),
            FileMode.Create, FileAccess.Write);
        var sourceText = new StringBuilder();
        sourceText.AppendLine("// This code is auto generated. Do not modify.");
        sourceText.AppendLine("using System;");
        sourceText.AppendLine("using System.Diagnostics.CodeAnalysis;");
        sourceText.AppendLine("using AtomUI.Controls;");
        sourceText.AppendLine("");
        sourceText.AppendLine($"namespace {PackageNamespace};");
        sourceText.AppendLine("");
        sourceText.AppendLine($"public partial class {PackageName}IconProvider");
        sourceText.AppendLine("{");
        sourceText.AppendLine("    [UnconditionalSuppressMessage(\"Trimming\", \"IL2063\",");
        sourceText.AppendLine("        Justification = \"Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.\")]");
        sourceText.AppendLine("    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]");
        sourceText.AppendLine($"    private static Type GetIconType({PackageName}IconKind kind)");
        sourceText.AppendLine("    {");
        sourceText.AppendLine("        switch (kind)");
        sourceText.AppendLine("        {");
        foreach (var info in IconFiles)
        {
            var iconClassName = $"{info.Name}{info.ThemeType}";
            sourceText.AppendLine($"            case {PackageName}IconKind.{iconClassName}: return typeof({iconClassName});");
        }
        sourceText.AppendLine("            default: throw new InvalidOperationException($\"Icon kind {kind} does not exist\");");
        sourceText.AppendLine("        }");
        sourceText.AppendLine("    }");
        sourceText.AppendLine("");
        sourceText.AppendLine($"    private static Icon CreateIcon({PackageName}IconKind kind)");
        sourceText.AppendLine("    {");
        sourceText.AppendLine("        return kind switch");
        sourceText.AppendLine("        {");
        foreach (var info in IconFiles)
        {
            var iconClassName = $"{info.Name}{info.ThemeType}";
            sourceText.AppendLine($"            {PackageName}IconKind.{iconClassName} => new {iconClassName}(),");
        }
        sourceText.AppendLine("            _ => throw new InvalidOperationException($\"Icon kind {kind} does not exist\")");
        sourceText.AppendLine("        };");
        sourceText.AppendLine("    }");
        sourceText.AppendLine("}");
        await output.WriteAsync(Encoding.UTF8.GetBytes(sourceText.ToString()));
    }
}
