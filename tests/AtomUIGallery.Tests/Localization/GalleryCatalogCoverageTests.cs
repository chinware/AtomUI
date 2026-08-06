using System.Reflection;
using System.Xml.Linq;
using AtomUI;
using AtomUI.Localization;
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
    public void Gallery_Registers_All_Application_Catalogs_And_Compiled_Translations()
    {
        var assembly = typeof(BaseGalleryApplication).Assembly;
        var catalogTypes = assembly.GetTypes()
            .Where(type => type is { IsEnum: true, Namespace: "AtomUIGallery.Localization" } &&
                           type.Name.EndsWith("LangResourceKind", StringComparison.Ordinal))
            .OrderBy(static type => type.FullName, StringComparer.Ordinal)
            .ToArray();

        catalogTypes.Length.ShouldBe(78);
        foreach (var catalogType in catalogTypes)
        {
            catalogType.GetCustomAttribute<LanguageCatalogAttribute>()
                       .ShouldNotBeNull()
                       .ContractVersion.ShouldBe(1);
            var ids = Enum.GetValues(catalogType)
                          .Cast<object>()
                          .Select(Convert.ToInt32)
                          .Order()
                          .ToArray();
            ids.ShouldBe(Enumerable.Range(1, ids.Length));

            var extensionName = catalogType.Name[..^"Kind".Length] + "Extension";
            var extensionType = assembly.GetType($"{catalogType.Namespace}.{extensionName}")
                                        .ShouldNotBeNull();
            extensionType.IsSealed.ShouldBeTrue();
            extensionType.BaseType.ShouldBe(
                typeof(LanguageResourceExtension<>).MakeGenericType(catalogType));
        }

        assembly.GetTypes().ShouldNotContain(static type =>
            type.Name == "en_US" || type.Name == "zh_CN" || type.Name == "zh_TW");

        var files = LoadLanguageFiles(catalogTypes);
        files.Length.ShouldBe(234);
        files.GroupBy(static file => file.CatalogType)
             .ShouldAllBe(static group => group.Count() == 3);

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
                        var kind = Enum.Parse(file.CatalogType, entry.Name);
                        Convert.ToInt32(kind).ShouldBe(entry.Id);
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
        var root = GetRepoPath("controlgallery/AtomUIGallery");
        XNamespace xliff = "urn:oasis:names:tc:xliff:document:2.0";

        return Directory.GetFiles(root, "*.xlf", SearchOption.AllDirectories)
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
                        var text = targetLanguage is null
                            ? segment.Element(xliff + "source").ShouldNotBeNull().Value
                            : segment.Element(xliff + "target").ShouldNotBeNull().Value;
                        return new CatalogEntry(
                            int.Parse(unit.Attribute("id")!.Value),
                            unit.Attribute("name")!.Value,
                            text);
                    })
                    .ToArray();
                return new CatalogLanguageFile(catalogType, language, entries);
            })
            .ToArray();
    }

    private static string GetRepoPath(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"Repository path '{relativePath}' was not found.");
    }

    private sealed record CatalogLanguageFile(
        Type CatalogType,
        LanguageTag Language,
        CatalogEntry[] Entries);

    private sealed record CatalogEntry(int Id, string Name, string Text);
}
