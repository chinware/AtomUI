using System.Reflection;
using System.Xml.Linq;
using AtomUI;
using AtomUI.Controls.Localization;
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Localization;
using AtomUI.Toolkits.GalleryBase.Localization;
using AtomUIGallery.Localization;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Localization;

public class GalleryCatalogCoverageTests
{
    private static readonly MethodInfo LocalizerGetMethod = typeof(ILocalizer)
        .GetMethods()
        .Single(method => method.Name == nameof(ILocalizer.Get));

    static GalleryCatalogCoverageTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Official_Catalogs_Preserve_Their_Enum_Xliff_And_Compiled_Translation_Contracts()
    {
        var assemblies = new[]
        {
            typeof(CommonLangResourceKind).Assembly,
            typeof(DatePickerLangResourceKind).Assembly,
            typeof(ColorPickerLangResourceKind).Assembly,
            typeof(DataGridLangResourceKind).Assembly,
            typeof(GalleryShowCaseHeaderLangResourceKind).Assembly,
            typeof(BaseGalleryApplication).Assembly
        }.Distinct().ToArray();
        var catalogTypes = assemblies
            .SelectMany(static assembly => assembly.GetTypes())
            .Where(static type => type is { IsEnum: true } &&
                                  type.GetCustomAttribute<LanguageCatalogAttribute>() is not null)
            .OrderBy(static type => type.FullName, StringComparer.Ordinal)
            .ToArray();

        catalogTypes.Length.ShouldBe(92);
        var memberOrderBaseline = LoadCatalogMemberOrderBaseline();
        memberOrderBaseline.Keys.ShouldBe(
            catalogTypes.Select(static type => type.FullName!),
            ignoreOrder: true);
        foreach (var catalogType in catalogTypes)
        {
            catalogType.GetCustomAttribute<LanguageCatalogAttribute>()
                       .ShouldNotBeNull()
                       .ContractVersion.ShouldBe(2);
            Enum.GetNames(catalogType).ShouldBe(memberOrderBaseline[catalogType.FullName!]);

            var extensionName = catalogType.Name[..^"Kind".Length] + "Extension";
            var extensionType = catalogType.Assembly
                                           .GetType($"{catalogType.Namespace}.{extensionName}")
                                           .ShouldNotBeNull();
            extensionType.IsSealed.ShouldBeTrue();
            extensionType.BaseType.ShouldBe(
                typeof(LanguageResourceExtension<>).MakeGenericType(catalogType));
        }

        assemblies.SelectMany(static assembly => assembly.GetTypes())
                  .ShouldNotContain(static type =>
                      type.Name == "en_US" || type.Name == "zh_CN" || type.Name == "zh_TW");

        var files = LoadLanguageFiles(catalogTypes);
        files.Length.ShouldBe(276);
        files.GroupBy(static file => file.CatalogType)
             .ShouldAllBe(static group => group.Count() == 3);
        AssertXliffContracts(catalogTypes, files);

        var application = Application.Current.ShouldNotBeNull();
        var manager = application.GetLanguageManager().ShouldNotBeNull();
        var localizer = application.GetLocalizer().ShouldNotBeNull();
        try
        {
            foreach (var language in new[] { LanguageTags.EnUS, LanguageTags.ZhCN, LanguageTags.ZhTW })
            {
                manager.ChangeLanguage(language);
                foreach (var file in files.Where(file => file.Language == language))
                {
                    var get = LocalizerGetMethod.MakeGenericMethod(file.CatalogType);
                    foreach (var entry in file.Entries)
                    {
                        var kind = Enum.Parse(file.CatalogType, entry.Key);
                        get.Invoke(localizer, [kind]).ShouldBe(entry.Text);
                    }
                }
            }
        }
        finally
        {
            manager.ChangeLanguage(LanguageTags.EnUS);
        }
    }

    private static CatalogLanguageFile[] LoadLanguageFiles(IReadOnlyCollection<Type> catalogTypes)
    {
        var typesByName = catalogTypes.ToDictionary(
            static type => type.FullName!,
            StringComparer.Ordinal);
        var root = GetRepoRoot();
        XNamespace xliff = "urn:oasis:names:tc:xliff:document:2.0";

        return new[] { "src", "controlgallery" }
            .SelectMany(directory => Directory.GetFiles(
                Path.Combine(root, directory),
                "*.xlf",
                SearchOption.AllDirectories))
            .Where(path => !path.Contains(
                Path.Combine("src", "LanguagePacks") + Path.DirectorySeparatorChar,
                StringComparison.Ordinal))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(path =>
            {
                var document = XDocument.Load(path, LoadOptions.PreserveWhitespace);
                var rootElement = document.Root.ShouldNotBeNull();
                var file = rootElement.Element(xliff + "file").ShouldNotBeNull();
                var catalogName = file.Attribute("id")!.Value;
                var catalogType = typesByName[catalogName];
                var targetLanguage = rootElement.Attribute("trgLang")?.Value;
                var language = LanguageTag.Parse(
                    targetLanguage ?? rootElement.Attribute("srcLang")!.Value);
                var entries = file.Elements(xliff + "unit")
                    .Select(unit =>
                    {
                        var segment = unit.Element(xliff + "segment").ShouldNotBeNull();
                        var source = segment.Element(xliff + "source").ShouldNotBeNull().Value;
                        var target = segment.Element(xliff + "target");
                        var text = targetLanguage is null ? source : target.ShouldNotBeNull().Value;
                        unit.Attribute("name").ShouldBeNull();
                        return new CatalogEntry(
                            unit.Attribute("id").ShouldNotBeNull().Value,
                            source,
                            text,
                            target?.Attribute("state")?.Value);
                    })
                    .ToArray();
                return new CatalogLanguageFile(catalogName, catalogType, language, entries);
            })
            .ToArray();
    }

    private static void AssertXliffContracts(
        IReadOnlyCollection<Type> catalogTypes,
        IReadOnlyCollection<CatalogLanguageFile> files)
    {
        foreach (var catalogType in catalogTypes)
        {
            var catalogFiles = files.Where(file => file.CatalogType == catalogType).ToArray();
            catalogFiles.Select(static file => file.Language)
                        .ShouldBe([LanguageTags.EnUS, LanguageTags.ZhCN, LanguageTags.ZhTW], ignoreOrder: true);
            catalogFiles.ShouldAllBe(file => file.CatalogId == catalogType.FullName);

            var sourceEntries = catalogFiles.Single(file => file.Language == LanguageTags.EnUS).Entries;
            var enumKeys = Enum.GetNames(catalogType)
                               .OrderBy(static key => key, StringComparer.Ordinal)
                               .ToArray();
            sourceEntries.Select(static entry => entry.Key)
                         .OrderBy(static key => key, StringComparer.Ordinal)
                         .ShouldBe(enumKeys);

            foreach (var targetFile in catalogFiles.Where(file => file.Language != LanguageTags.EnUS))
            {
                targetFile.Entries.Select(static entry => (entry.Key, entry.Source))
                          .OrderBy(static entry => entry.Key, StringComparer.Ordinal)
                          .ShouldBe(sourceEntries.Select(static entry => (entry.Key, entry.Source))
                                                 .OrderBy(static entry => entry.Key, StringComparer.Ordinal));
                targetFile.Entries.ShouldAllBe(static entry =>
                    entry.TargetState == "translated" && !string.IsNullOrEmpty(entry.Text));
            }
        }
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("AtomUI repository root was not found.");
    }

    private static IReadOnlyDictionary<string, string[]> LoadCatalogMemberOrderBaseline()
    {
        var path = Path.Combine(
            GetRepoRoot(),
            "tests",
            "AtomUIGallery.Tests",
            "Localization",
            "CatalogMemberOrder.baseline");
        return File.ReadLines(path)
                   .Where(static line => !string.IsNullOrWhiteSpace(line))
                   .Select(static line => line.Split('|', 2))
                   .ToDictionary(
                       static parts => parts[0],
                       static parts => parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries),
                       StringComparer.Ordinal);
    }

    private sealed record CatalogLanguageFile(
        string CatalogId,
        Type CatalogType,
        LanguageTag Language,
        CatalogEntry[] Entries);

    private sealed record CatalogEntry(
        string Key,
        string Source,
        string Text,
        string? TargetState);
}
