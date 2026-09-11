using AtomUI.Build.Tasks.LocalizationBuild;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class LanguageFileValidationTests
{
    [Fact]
    public void ValidateTarget_Reports_Stale_Source_And_Missing_Complete_Unit()
    {
        var source = Parse(SourceXliff(
            ("Title", "Hello"),
            ("Body", "Body")));
        var target = Parse(TargetXliff(
            "ja-JP",
            ("Title", "Changed", "こんにちは", "final")));

        var diagnostics = LanguageFileValidation.ValidateTarget(
            source,
            target,
            new LanguageFileValidationOptions(requireCompleteBundle: true, minimumTargetState: "final"));

        diagnostics.ShouldContain(item =>
            item.Kind == LanguageFileValidationDiagnosticKind.CatalogMismatch &&
            item.Message.Contains("Title", StringComparison.Ordinal));
        diagnostics.ShouldContain(item =>
            item.Kind == LanguageFileValidationDiagnosticKind.InvalidTranslation &&
            item.Message.Contains("Body", StringComparison.Ordinal));
    }

    [Fact]
    public void ValidateTarget_Allows_Partial_Override_But_Still_Validates_Present_Units()
    {
        var source = Parse(SourceXliff(
            ("Title", "Hello"),
            ("Body", "Body")));
        var target = Parse(TargetXliff(
            "ja-JP",
            ("Title", "Hello", "こんにちは", "final")));

        var diagnostics = LanguageFileValidation.ValidateTarget(
            source,
            target,
            new LanguageFileValidationOptions(requireCompleteBundle: false, minimumTargetState: "final"));

        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateTarget_Reports_Unpublishable_Target_With_Actionable_Message()
    {
        var source = Parse(SourceXliff(("Title", "Hello")));
        var target = Parse(TargetXliff(
            "ja-JP",
            ("Title", "Hello", " ", "translated")));

        var diagnostics = LanguageFileValidation.ValidateTarget(
            source,
            target,
            new LanguageFileValidationOptions(requireCompleteBundle: true, minimumTargetState: "translated"));

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Kind.ShouldBe(LanguageFileValidationDiagnosticKind.InvalidTranslation);
        diagnostic.Message.ShouldContain("must contain a target");
    }

    [Fact]
    public void ValidateTarget_Reports_Actual_And_Required_Target_State()
    {
        var source = Parse(SourceXliff(("Title", "Hello")));
        var target = Parse(TargetXliff(
            "ja-JP",
            ("Title", "Hello", "こんにちは", "reviewed")));

        var diagnostics = LanguageFileValidation.ValidateTarget(
            source,
            target,
            new LanguageFileValidationOptions(requireCompleteBundle: true, minimumTargetState: "final"));

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Kind.ShouldBe(LanguageFileValidationDiagnosticKind.InvalidTranslation);
        diagnostic.Message.ShouldContain("reviewed");
        diagnostic.Message.ShouldContain("final");
    }

    [Fact]
    public void ValidateTargetContent_Validates_Deferred_Target_Without_Source_Contract()
    {
        var target = Parse(TargetXliff(
            "ja-JP",
            ("Title", "Hello", "こんにちは", "reviewed")));

        var diagnostics = LanguageFileValidation.ValidateTargetContent(
            target,
            new LanguageFileValidationOptions(requireCompleteBundle: true, minimumTargetState: "final"));

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Kind.ShouldBe(LanguageFileValidationDiagnosticKind.InvalidTranslation);
        diagnostic.Message.ShouldContain("reviewed");
        diagnostic.Message.ShouldContain("final");
    }

    private static XliffDocumentModel Parse(string content)
    {
        var result = Xliff21Parser.Parse(content);
        result.Errors.ShouldBeEmpty();
        return result.Document.ShouldNotBeNull();
    }

    private static string SourceXliff(params (string Key, string Source)[] units)
    {
        return $$"""
                 <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
                   <file id="Test.Product.Catalog">
                 {{string.Join(Environment.NewLine, units.Select(static unit => $$"""
                     <unit id="{{unit.Key}}"><segment><source>{{unit.Source}}</source></segment></unit>
                 """))}}
                   </file>
                 </xliff>
                 """;
    }

    private static string TargetXliff(
        string language,
        params (string Key, string Source, string Target, string State)[] units)
    {
        return $$"""
                 <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="{{language}}">
                   <file id="Test.Product.Catalog">
                 {{string.Join(Environment.NewLine, units.Select(static unit => $$"""
                     <unit id="{{unit.Key}}"><segment><source>{{unit.Source}}</source><target state="{{unit.State}}">{{unit.Target}}</target></segment></unit>
                 """))}}
                   </file>
                 </xliff>
                 """;
    }
}
