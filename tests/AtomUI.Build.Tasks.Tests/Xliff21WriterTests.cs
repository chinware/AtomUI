using System.Xml.Linq;
using AtomUI.Build.Tasks.LocalizationBuild;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class Xliff21WriterTests
{
    [Fact]
    public void Write_Produces_Deterministic_Utf8_Xliff_With_Stable_Unit_Order()
    {
        var document = new XliffDocumentModel(
            "en-US",
            "zh-CN",
            new XliffFileModel(
                "Test.Product.LoginLangResourceKind",
                [
                    Unit("Body", "Use <safe> & sound", "安全使用", "reviewed", ["Translator & reviewer"]),
                    Unit("Title", "Sign in", "登录", "translated", ["Window title"]),
                    Unit("Retired", "Retired", "已停用", "final", [], isObsolete: true)
                ]));

        var first = Xliff21Writer.Write(document);
        var second = Xliff21Writer.Write(document);

        first.ShouldBe(second);
        first.ShouldStartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
        first.IndexOf("id=\"Body\"", StringComparison.Ordinal)
             .ShouldBeLessThan(first.IndexOf("id=\"Retired\"", StringComparison.Ordinal));
        first.ShouldNotContain("name=\"");
        first.ShouldContain("Use &lt;safe&gt; &amp; sound");
        first.ShouldContain("Translator &amp; reviewer");

        XNamespace ns = "urn:oasis:names:tc:xliff:document:2.0";
        var xml = XDocument.Parse(first);
        var root = xml.Root.ShouldNotBeNull();
        ((string?)root.Attribute("version")).ShouldBe("2.1");
        ((string?)root.Attribute("srcLang")).ShouldBe("en-US");
        ((string?)root.Attribute("trgLang")).ShouldBe("zh-CN");
        var retired = root.Descendants(ns + "unit")
                          .Single(unit => (string?)unit.Attribute("id") == "Retired");
        ((string?)retired.Attribute("translate")).ShouldBe("no");
    }

    [Fact]
    public void Write_RoundTrips_An_Initial_Empty_Target_With_Formatted_Source()
    {
        var document = new XliffDocumentModel(
            "en-US",
            "ja-JP",
            new XliffFileModel(
                "Test.Product.ItemsLangResourceKind",
                [Unit("Count", "{0} items", string.Empty, "initial", [])]));

        var result = Xliff21Parser.Parse(Xliff21Writer.Write(document));

        result.Errors.ShouldBeEmpty();
        var unit = result.Document.ShouldNotBeNull().File.Units.ShouldHaveSingleItem();
        unit.Target.ShouldBe(string.Empty);
        unit.TargetState.ShouldBe("initial");
    }

    [Fact]
    public void Write_RoundTrips_Target_SubState()
    {
        var document = new XliffDocumentModel(
            "en-US",
            "zh-CN",
            new XliffFileModel(
                "Test.Product.LoginLangResourceKind",
                [Unit("Title", "Sign in", "登录", "translated", [], "needs-review")]));

        var result = Xliff21Parser.Parse(Xliff21Writer.Write(document));

        result.Errors.ShouldBeEmpty();
        result.Document.ShouldNotBeNull().File.Units.ShouldHaveSingleItem()
              .TargetSubState.ShouldBe("needs-review");
    }

    [Fact]
    public void Repository_Xliff_Files_Use_Canonical_Writer_Output()
    {
        var repositoryRoot = FindRepositoryRoot();
        var paths = new[] { "src", "controlgallery", "tests" }
                    .SelectMany(directory => Directory.GetFiles(
                        Path.Combine(repositoryRoot, directory),
                        "*.xlf",
                        SearchOption.AllDirectories))
                    .OrderBy(static path => path, StringComparer.Ordinal)
                    .ToArray();

        paths.ShouldNotBeEmpty();
        foreach (var path in paths)
        {
            var content = File.ReadAllText(path);
            var parsed = Xliff21Parser.Parse(content);
            parsed.Errors.ShouldBeEmpty($"XLIFF file '{path}' must be valid before formatting.");
            Xliff21Writer.Write(parsed.Document.ShouldNotBeNull())
                         .ShouldBe(content, $"XLIFF file '{path}' is not in canonical form.");
        }
    }

    private static XliffUnitModel Unit(
        string key,
        string source,
        string? target,
        string? targetState,
        IReadOnlyList<string> notes,
        string? targetSubState = null,
        bool isObsolete = false)
    {
        CompositeFormatContractParser.TryParse(source, out var placeholders, out _).ShouldBeTrue();
        return new XliffUnitModel(
            key,
            null,
            source,
            target,
            targetState,
            targetSubState,
            notes,
            placeholders,
            1,
            1,
            isObsolete);
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
