using AtomUI.Localization.Build;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class XliffMergeEngineTests
{
    [Fact]
    public void Merge_Preserves_Translations_And_Marks_Source_Changes_For_Review()
    {
        var source = SourceDocument(
            Unit(1, "Heading", "Sign in", null, null, ["Current developer context"]),
            Unit(2, "Body", "Welcome back", null, null, []),
            Unit(3, "Count", "{0} items", null, null, []));
        var existing = TargetDocument(
            Unit(1, "Title", "Sign in", "登录", "translated", ["Translator note"]),
            Unit(2, "Body", "Welcome", "欢迎", "final", []),
            Unit(4, "Removed", "Removed", "已移除", "reviewed", ["Keep history"]));

        var merged = XliffMergeEngine.Merge(source, existing, "zh-CN");

        merged.TargetLanguage.ShouldBe("zh-CN");
        merged.File.Units.Select(static unit => unit.Id).ShouldBe([1, 2, 3, 4]);

        var renamed = merged.File.Units[0];
        renamed.Name.ShouldBe("Heading");
        renamed.Target.ShouldBe("登录");
        renamed.TargetState.ShouldBe("translated");
        renamed.Notes.ShouldBe(["Current developer context", "Translator note"]);

        var changed = merged.File.Units[1];
        changed.Source.ShouldBe("Welcome back");
        changed.Target.ShouldBe("欢迎");
        changed.TargetState.ShouldBe("initial");

        var added = merged.File.Units[2];
        added.Target.ShouldBe(string.Empty);
        added.TargetState.ShouldBe("initial");
        added.IsObsolete.ShouldBeFalse();

        var removed = merged.File.Units[3];
        removed.Target.ShouldBe("已移除");
        removed.IsObsolete.ShouldBeTrue();
    }

    [Fact]
    public void Merge_Rejects_Mismatched_Catalog_Or_Target_Language()
    {
        var source = SourceDocument(Unit(1, "Title", "Title", null, null, []));
        var otherCatalog = new XliffDocumentModel(
            "en-US",
            "zh-CN",
            new XliffFileModel(
                "Other.Catalog",
                [Unit(1, "Title", "Title", "标题", "translated", [])]));
        var wrongLanguage = TargetDocument(Unit(1, "Title", "Title", "Title", "translated", []));

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
        int id,
        string name,
        string source,
        string? target,
        string? targetState,
        IReadOnlyList<string> notes)
    {
        CompositeFormatContractParser.TryParse(source, out var placeholders, out _).ShouldBeTrue();
        return new XliffUnitModel(
            id,
            name,
            source,
            target,
            targetState,
            notes,
            placeholders,
            1,
            1);
    }
}
