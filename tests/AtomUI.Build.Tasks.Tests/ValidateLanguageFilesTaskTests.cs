using AtomUI.Build.Tasks;
using Microsoft.Build.Framework;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class ValidateLanguageFilesTaskTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"atomui-localization-task-tests-{Guid.NewGuid():N}");

    public ValidateLanguageFilesTaskTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Execute_Reports_Xliff_Parse_Errors_With_Source_Location()
    {
        var path = Write("invalid.xlf", CreateXliff(version: "2.0"));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles = [Item(path, "ModuleBuiltIn")]
        };

        task.Execute().ShouldBeFalse();

        var error = engine.Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILOC005");
        error.File.ShouldBe(path);
        error.LineNumber.ShouldBeGreaterThan(0);
        error.Message.ShouldNotBeNull().ShouldContain("version");
    }

    [Fact]
    public void Execute_Requires_Complete_Module_Bundles_But_Allows_Partial_Overrides()
    {
        var source = Write("en-US.xlf", CreateXliff(targetLanguage: null));
        var target = Write("zh-CN.xlf", CreateXliff(targetLanguage: "zh-CN", includeSecondTarget: false));
        var partialOverride = Write(
            "zh-CN.override.xlf",
            CreateXliff(targetLanguage: "zh-CN", includeSecondUnit: false));
        var moduleEngine = new RecordingBuildEngine();
        var moduleTask = new ValidateLanguageFilesTask
        {
            BuildEngine = moduleEngine,
            LanguageFiles =
            [
                Item(source, "ModuleBuiltIn"),
                Item(target, "ModuleBuiltIn")
            ]
        };

        moduleTask.Execute().ShouldBeFalse();
        moduleEngine.Errors.Any(error =>
            error.Code == "ATOMUILOC007" &&
            error.Message?.Contains("unit 'Body'", StringComparison.Ordinal) == true)
            .ShouldBeTrue();

        var overrideEngine = new RecordingBuildEngine();
        var overrideTask = new ValidateLanguageFilesTask
        {
            BuildEngine = overrideEngine,
            LanguageFiles =
            [
                Item(source, "ModuleBuiltIn"),
                Item(partialOverride, "ApplicationOverride")
            ]
        };

        overrideTask.Execute().ShouldBeTrue();
        overrideEngine.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Execute_Rejects_Duplicate_Translation_Sources_At_The_Same_Priority()
    {
        var first = Write("first.xlf", CreateXliff(targetLanguage: "zh-CN"));
        var second = Write("second.xlf", CreateXliff(targetLanguage: "zh-CN"));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles =
            [
                Item(first, "StaticLanguagePack", "Pack.One"),
                Item(second, "StaticLanguagePack", "Pack.Two")
            ]
        };

        task.Execute().ShouldBeFalse();
        var error = engine.Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILOC006");
        error.Message.ShouldNotBeNull().ShouldContain("same priority");
        error.Message.ShouldNotBeNull().ShouldContain("Pack.One");
        error.Message.ShouldNotBeNull().ShouldContain("Pack.Two");
    }

    [Fact]
    public void Execute_Accepts_Disjoint_Application_Override_Fragments()
    {
        var source = Write("en-US.xlf", CreateXliff(targetLanguage: null));
        var titleOverride = Write(
            "title.override.xlf",
            CreateXliff(targetLanguage: "zh-CN", includeSecondUnit: false));
        var bodyOverride = Write(
            "body.override.xlf",
            CreateXliff(targetLanguage: "zh-CN", includeFirstUnit: false));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles =
            [
                Item(source, "ModuleBuiltIn"),
                Item(titleOverride, "ApplicationOverride", "TestApp.Title"),
                Item(bodyOverride, "ApplicationOverride", "TestApp.Body")
            ]
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Execute_Rejects_Overlapping_Application_Override_Units()
    {
        var source = Write("en-US.xlf", CreateXliff(targetLanguage: null));
        var first = Write(
            "first.override.xlf",
            CreateXliff(targetLanguage: "zh-CN", includeSecondUnit: false));
        var second = Write(
            "second.override.xlf",
            CreateXliff(targetLanguage: "zh-CN", includeSecondUnit: false));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles =
            [
                Item(source, "ModuleBuiltIn"),
                Item(first, "ApplicationOverride", "TestApp.One"),
                Item(second, "ApplicationOverride", "TestApp.Two")
            ]
        };

        task.Execute().ShouldBeFalse();
        var error = engine.Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILOC006");
        error.Message.ShouldNotBeNull().ShouldContain("unit 'Title'");
        error.Message.ShouldNotBeNull().ShouldContain("TestApp.One");
        error.Message.ShouldNotBeNull().ShouldContain("TestApp.Two");
    }

    [Fact]
    public void Execute_Accepts_A_Complete_Publishable_Bundle()
    {
        var source = Write("en-US.xlf", CreateXliff(targetLanguage: null));
        var target = Write("zh-CN.xlf", CreateXliff(targetLanguage: "zh-CN"));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles =
            [
                Item(source, "ModuleBuiltIn"),
                Item(target, "ModuleBuiltIn")
            ]
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("translated", "translated", true)]
    [InlineData("translated", "reviewed", true)]
    [InlineData("translated", "final", true)]
    [InlineData("reviewed", "translated", false)]
    [InlineData("reviewed", "reviewed", true)]
    [InlineData("reviewed", "final", true)]
    [InlineData("final", "translated", false)]
    [InlineData("final", "reviewed", false)]
    [InlineData("final", "final", true)]
    public void Execute_Enforces_The_Minimum_Target_State(
        string minimumTargetState,
        string actualTargetState,
        bool expectedSuccess)
    {
        var source = Write("en-US.xlf", CreateXliff(targetLanguage: null, includeSecondUnit: false));
        var target = Write(
            "zh-CN.xlf",
            CreateXliff(
                targetLanguage: "zh-CN",
                includeSecondUnit: false,
                firstTargetState: actualTargetState));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles =
            [
                Item(source, "ModuleBuiltIn"),
                Item(target, "StaticLanguagePack")
            ],
            MinimumTargetState = minimumTargetState
        };

        task.Execute().ShouldBe(expectedSuccess);
        if (expectedSuccess)
        {
            engine.Errors.ShouldBeEmpty();
        }
        else
        {
            var error = engine.Errors.ShouldHaveSingleItem();
            error.Code.ShouldBe("ATOMUILOC007");
            error.Message.ShouldNotBeNull().ShouldContain(actualTargetState);
            error.Message.ShouldNotBeNull().ShouldContain(minimumTargetState);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("initial")]
    [InlineData("approved")]
    public void Execute_Rejects_An_Invalid_Minimum_Target_State(string minimumTargetState)
    {
        var source = Write("en-US.xlf", CreateXliff(targetLanguage: null));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles = [Item(source, "ModuleBuiltIn")],
            MinimumTargetState = minimumTargetState
        };

        task.Execute().ShouldBeFalse();
        var error = engine.Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILOC009");
        error.Message.ShouldNotBeNull().ShouldContain("translated, reviewed, or final");
    }

    [Theory]
    [InlineData("<target state=\"translated\">标题</target>", "<target state=\"translated\">   </target>")]
    [InlineData("<target state=\"translated\">标题</target>", "<target state=\"translated\" subState=\"needs-review\">标题</target>")]
    public void Execute_Rejects_Unpublishable_Target_Content_Or_SubState(
        string currentTarget,
        string invalidTarget)
    {
        var source = Write("en-US.xlf", CreateXliff(targetLanguage: null));
        var target = Write(
            "zh-CN.xlf",
            CreateXliff(targetLanguage: "zh-CN").Replace(currentTarget, invalidTarget));
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles =
            [
                Item(source, "ModuleBuiltIn"),
                Item(target, "ModuleBuiltIn")
            ]
        };

        task.Execute().ShouldBeFalse();
        engine.Errors.Any(error =>
            error.Code == "ATOMUILOC007" &&
            (error.Message ?? string.Empty).Contains("non-empty target", StringComparison.Ordinal))
            .ShouldBeTrue();
    }

    [Fact]
    public void Execute_Accepts_An_Empty_English_Source_Unit()
    {
        var source = Write(
            "en-US-empty.xlf",
            """
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="Test.Product.CalendarLangResourceKind">
                <unit id="YearSuffix">
                  <segment><source></source></segment>
                </unit>
              </file>
            </xliff>
            """);
        var engine = new RecordingBuildEngine();
        var task = new ValidateLanguageFilesTask
        {
            BuildEngine = engine,
            LanguageFiles = [Item(source, "ModuleBuiltIn")]
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();
    }

    public void Dispose()
    {
        Directory.Delete(_directory, recursive: true);
    }

    private TestTaskItem Item(
        string path,
        string sourceKind,
        string sourceIdentity = "Test.Product")
    {
        return new TestTaskItem(
            path,
            ("AtomUILanguageModuleId", "Test.Product"),
            ("AtomUILanguageSourceKind", sourceKind),
            ("AtomUILanguageSourceIdentity", sourceIdentity));
    }

    private string Write(string fileName, string content)
    {
        var path = Path.Combine(_directory, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    private static string CreateXliff(
        string? targetLanguage = "zh-CN",
        bool includeSecondTarget = true,
        bool includeSecondUnit = true,
        string version = "2.1",
        bool includeFirstUnit = true,
        string firstTargetState = "translated",
        string secondTargetState = "reviewed")
    {
        var targetAttribute = targetLanguage is null ? string.Empty : $" trgLang=\"{targetLanguage}\"";
        var firstTarget = targetLanguage is null
            ? string.Empty
            : $"<target state=\"{firstTargetState}\">标题</target>";
        var secondTarget = targetLanguage is null || !includeSecondTarget
            ? string.Empty
            : $"<target state=\"{secondTargetState}\">正文</target>";
        var secondUnit = includeSecondUnit
            ? $$"""
                <unit id="Body">
                  <segment><source>Body</source>{{secondTarget}}</segment>
                </unit>
                """
            : string.Empty;
        var firstUnit = includeFirstUnit
            ? $$"""
                <unit id="Title">
                  <segment><source>Title</source>{{firstTarget}}</segment>
                </unit>
                """
            : string.Empty;
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0"
                   version="{{version}}"
                   srcLang="en-US"{{targetAttribute}}>
              <file id="Test.Product.LoginLangResourceKind">
                {{firstUnit}}
                {{secondUnit}}
              </file>
            </xliff>
            """;
    }
}
