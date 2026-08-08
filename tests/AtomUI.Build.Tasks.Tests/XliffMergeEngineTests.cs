using AtomUI.Build.Tasks.LocalizationBuild;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class XliffMergeEngineTests
{
    [Fact]
    public void Merge_Preserves_Translations_And_Marks_Source_Changes_For_Review()
    {
        var source = SourceDocument(
            Unit("Title", "Sign in", null, null, ["Current developer context"]),
            Unit("Body", "Welcome back", null, null, []),
            Unit("Count", "{0} items", null, null, []));
        var existing = TargetDocument(
            Unit("Title", "Sign in", "登录", "translated", ["Translator note"]),
            Unit("Body", "Welcome", "欢迎", "final", []),
            Unit("Removed", "Removed", "已移除", "reviewed", ["Keep history"]));

        var merged = XliffMergeEngine.Merge(source, existing, "zh-CN");

        merged.TargetLanguage.ShouldBe("zh-CN");
        merged.File.Units.Select(static unit => unit.Key).ShouldBe(["Body", "Count", "Removed", "Title"]);

        var renamed = merged.File.Units[0];
        renamed.Key.ShouldBe("Body");
        renamed.Target.ShouldBe("欢迎");
        renamed.TargetState.ShouldBe("initial");

        var changed = merged.File.Units[1];
        changed.Key.ShouldBe("Count");
        changed.Target.ShouldBe(string.Empty);
        changed.TargetState.ShouldBe("initial");

        var added = merged.File.Units[2];
        added.Key.ShouldBe("Removed");
        added.Target.ShouldBe("已移除");
        added.IsObsolete.ShouldBeTrue();

        var removed = merged.File.Units[3];
        removed.Key.ShouldBe("Title");
        removed.Target.ShouldBe("登录");
        removed.TargetState.ShouldBe("translated");
        removed.Notes.ShouldBe(["Current developer context", "Translator note"]);
    }

    [Fact]
    public void Merge_Rejects_Mismatched_Catalog_Or_Target_Language()
    {
        var source = SourceDocument(Unit("Title", "Title", null, null, []));
        var otherCatalog = new XliffDocumentModel(
            "en-US",
            "zh-CN",
            new XliffFileModel(
                "Other.Catalog",
                [Unit("Title", "Title", "标题", "translated", [])]));
        var wrongLanguage = TargetDocument(Unit("Title", "Title", "Title", "translated", []));

        Should.Throw<ArgumentException>(() => XliffMergeEngine.Merge(source, otherCatalog, "zh-CN"))
              .Message.ShouldContain("Catalog");
        Should.Throw<ArgumentException>(() => XliffMergeEngine.Merge(source, wrongLanguage, "ja-JP"))
              .Message.ShouldContain("target language");
    }

    private static XliffDocumentModel SourceDocument(params XliffUnitModel[] units)
    {
        return new XliffDocumentModel(
            "en-US",
            null,
            new XliffFileModel("Test.Product.LoginLangResourceKind", units));
    }

    private static XliffDocumentModel TargetDocument(params XliffUnitModel[] units)
    {
        return new XliffDocumentModel(
            "en-US",
            "zh-CN",
            new XliffFileModel("Test.Product.LoginLangResourceKind", units));
    }

    private static XliffUnitModel Unit(
        string key,
        string source,
        string? target,
        string? targetState,
        IReadOnlyList<string> notes)
    {
        CompositeFormatContractParser.TryParse(source, out var placeholders, out _).ShouldBeTrue();
        return new XliffUnitModel(
            key,
            null,
            source,
            target,
            targetState,
            null,
            notes,
            placeholders,
            1,
            1);
    }
}
