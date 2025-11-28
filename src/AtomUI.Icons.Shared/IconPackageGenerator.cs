namespace AtomUI.Icons;

public abstract class AbstractIconPackageGenerator
{
    public string PackageName { get; protected set; } = string.Empty;
    public string PackageNamespace { get; protected set; } = string.Empty;
    
    public const string DefaultSavedDirectory = "GeneratedIcons";
    protected string SourcePath;
    protected string TargetPath;
    protected string GeneratedIconsPath;
    protected List<IconFileInfo> IconFiles;
    protected readonly SvgParser SvgParser;

    public AbstractIconPackageGenerator(string sourcePath, string targetPath, string? generatedIconsDir = null)
    {
        SourcePath         = sourcePath;
        TargetPath         = targetPath;
        GeneratedIconsPath = Path.Combine(TargetPath, generatedIconsDir ?? DefaultSavedDirectory);
        IconFiles          = new List<IconFileInfo>();
        SvgParser          = new SvgParser();
    }

    protected virtual void PrepareEnvironment()
    {
        if (!Directory.Exists(SourcePath))
        {
            throw new FileNotFoundException($"Icons file path {SourcePath} does not exist.");
        }

        if (!Directory.Exists(TargetPath))
        {
            throw new FileNotFoundException($"Icons file path {TargetPath} does not exist.");
        }

        if (Directory.Exists(GeneratedIconsPath))
        {
            Directory.Delete(GeneratedIconsPath, true);
        }
        Directory.CreateDirectory(GeneratedIconsPath);
    }
    
    protected void ScanIconFiles()
    {
        Console.WriteLine($"Beginning ScanIconFiles: {SourcePath}");
        IconFiles.AddRange(ScanIconFilesRecursively(SourcePath));
    }

    protected abstract IEnumerable<IconFileInfo> ScanIconFilesRecursively(string filePath);

    protected abstract Task GenerateIconPackageKindAsync();

    protected virtual async Task GenerateIconPackageClassesAsync()
    {
        foreach (var iconFileInfo in IconFiles)
        {
            await using var stream = new FileStream(GenerateIconClassFileName(iconFileInfo), FileMode.Create, FileAccess.Write);
            await GenerateIconPackageClass(iconFileInfo, stream);
        }
    }

    protected virtual string GenerateIconClassFileName(IconFileInfo iconFileInfo)
    {
        return Path.Combine(GeneratedIconsPath, $"{iconFileInfo.Name}{iconFileInfo.ThemeType}.g.cs");
    }
    
    protected abstract Task GenerateIconPackageClass(IconFileInfo iconFileInfo, Stream output);
    
    public async Task GenerateAsync()
    {
        PrepareEnvironment();
        ScanIconFiles();
        if (IconFiles.Count > 0)
        {
            Console.WriteLine($"Found {IconFiles.Count} icon files.");
            await GenerateIconPackageKindAsync();
            Console.WriteLine("Generate Icon PackageKind enum successfully.");
            await GenerateIconPackageClassesAsync();
            Console.WriteLine("Generate Icon classes successfully.");
        }
    }
}