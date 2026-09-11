using System.Text.RegularExpressions;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Localization;

/// <summary>
/// 所有 Semantic Part 示例条目共用同一句标题，避免同一个概念在 Gallery 里出现多种说法。
/// 这些条目的标题固定在四个资源键上（SemanticPartStyleTitle / StyleClassTitle /
/// SemanticStylingTitle / SemanticStylesTitle），这里以它们为唯一定位规则，断言每种语言的
/// 取值完全一致：新增页面若漏改标题或另起说法，都会在这里失败。
/// </summary>
public class SemanticPartShowCaseTitleTests
{
    private static readonly HashSet<string> SemanticTitleKeys = new(StringComparer.Ordinal)
    {
        "SemanticPartStyleTitle",
        "StyleClassTitle",
        "SemanticStylingTitle",
        "SemanticStylesTitle"
    };

    private static readonly Dictionary<string, string> CanonicalTitles = new(StringComparer.Ordinal)
    {
        ["en-US"] = "Custom Semantic Part styling",
        ["zh-CN"] = "自定义语义结构的样式",
        ["zh-TW"] = "自訂語義結構的樣式",
        ["pt-BR"] = "Estilo personalizado de Semantic Part"
    };

    [Fact]
    public void Semantic_Part_ShowCase_Items_Share_One_Title_Per_Language()
    {
        var root = FindRepositoryRoot();
        var showCases = Path.Combine(root, "controlgallery/AtomUIGallery/ShowCases");

        var items = EnumerateSemanticShowCaseItems(showCases).ToList();
        items.Count.ShouldBeGreaterThanOrEqualTo(53);

        foreach (var (page, resourceKey, localizationDirectory) in items)
        {
            foreach (var (language, expected) in CanonicalTitles)
            {
                var path = Path.Combine(localizationDirectory, $"{language}.xlf");
                File.Exists(path).ShouldBeTrue($"Missing localization file: {path}");

                var values = ReadUnits(path);
                values.ShouldContainKey(resourceKey);
                values[resourceKey].ShouldBe(
                    expected,
                    $"Semantic Part title '{resourceKey}' in '{page}' diverges in '{language}'.");
            }
        }
    }

    private static IEnumerable<(string Page, string ResourceKey, string LocalizationDirectory)> EnumerateSemanticShowCaseItems(
        string showCasesRoot)
    {
        var itemPattern  = new Regex(@"<gallery:ShowCaseItem\b(?<body>.*?)(?:/>|</gallery:ShowCaseItem>)", RegexOptions.Singleline);
        var titlePattern = new Regex("Title=\"\\{gallery:\\w+LangResource (?<key>\\w+)\\}\"");

        foreach (var file in Directory.EnumerateFiles(showCasesRoot, "*ShowCase.axaml", SearchOption.AllDirectories))
        {
            var source = File.ReadAllText(file);
            var viewsDirectory = Path.GetDirectoryName(file)!;
            var localizationDirectory = Path.Combine(viewsDirectory, "..", "Localization");
            var page = Path.GetFileName(Path.GetDirectoryName(viewsDirectory)!);

            foreach (Match item in itemPattern.Matches(source))
            {
                var title = titlePattern.Match(item.Groups["body"].Value);
                if (title.Success && SemanticTitleKeys.Contains(title.Groups["key"].Value))
                {
                    yield return (page, title.Groups["key"].Value, localizationDirectory);
                }
            }
        }
    }

    private static IReadOnlyDictionary<string, string> ReadUnits(string path)
    {
        XNamespace xliff = "urn:oasis:names:tc:xliff:document:2.0";
        return XDocument.Load(path)
                        .Descendants(xliff + "unit")
                        .ToDictionary(
                            unit => (string?)unit.Attribute("id") ?? throw new InvalidDataException($"XLIFF unit in '{path}' has no key."),
                            unit => unit.Descendants(xliff + "target").SingleOrDefault()?.Value ??
                                    unit.Descendants(xliff + "source").Single().Value,
                            StringComparer.Ordinal);
    }

    private static string FindRepositoryRoot()
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

        throw new DirectoryNotFoundException("The AtomUI repository root could not be located.");
    }
}
