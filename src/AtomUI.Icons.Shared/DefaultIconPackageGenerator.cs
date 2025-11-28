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
            var name = Path.GetFileNameWithoutExtension(svgFilePath);
            name = Regex.Replace(name, @"-([a-zA-Z0-9])",
                match => match.Groups[1].ToString().ToUpper());
            name = name[0].ToString().ToUpper() + name.Substring(1);
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
}