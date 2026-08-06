using AtomUI.Build.Tasks;
using AtomUI.Localization.Build;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class ExportLanguageTemplatesTaskTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"atomui-language-export-tests-{Guid.NewGuid():N}");

    public ExportLanguageTemplatesTaskTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Execute_Deterministically_Merges_Existing_Targets_And_Notes()
    {
        var sourcePath = Write("en-US.xlf", SourceXliff);
        var outputPath = Write("ja-JP.xlf", ExistingTargetXliff);
        var sourceItem = new TestTaskItem(
            sourcePath,
            ("AtomUILanguageTemplateOutputPath", outputPath));
        var engine = new RecordingBuildEngine();
        var task = new ExportLanguageTemplatesTask
        {
            BuildEngine = engine,
            SourceFiles = [sourceItem],
            TargetLanguage = "ja-JP"
        };

        task.Execute().ShouldBeTrue();
        var first = File.ReadAllText(outputPath);
        task.Execute().ShouldBeTrue();
        var second = File.ReadAllText(outputPath);

        first.ShouldBe(second);
        engine.Errors.ShouldBeEmpty();
        task.ExportedFiles.ShouldHaveSingleItem().ItemSpec.ShouldBe(outputPath);
        var parsed = Xliff21Parser.Parse(first);
        parsed.Errors.ShouldBeEmpty();
        var units = parsed.Document.ShouldNotBeNull().File.Units;
        units.Select(static unit => unit.Id).ShouldBe([1, 2, 3]);
        units[0].Name.ShouldBe("Heading");
        units[0].Target.ShouldBe("ログイン");
        units[0].TargetState.ShouldBe("translated");
        units[0].Notes.ShouldBe(["Current context", "Translator note"]);
        units[1].Source.ShouldBe("Welcome back");
        units[1].Target.ShouldBe("ようこそ");
        units[1].TargetState.ShouldBe("initial");
        units[2].Target.ShouldBe(string.Empty);
        units[2].TargetState.ShouldBe("initial");
    }

    public void Dispose()
    {
        Directory.Delete(_directory, recursive: true);
    }

    private string Write(string fileName, string content)
    {
        var path = Path.Combine(_directory, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    private const string SourceXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="Test.Product.LoginLangResourceKind">
            <unit id="1" name="Heading">
              <notes><note>Current context</note></notes>
              <segment><source>Sign in</source></segment>
            </unit>
            <unit id="2" name="Body">
              <segment><source>Welcome back</source></segment>
            </unit>
            <unit id="3" name="Count">
              <segment><source>{0} items</source></segment>
            </unit>
          </file>
        </xliff>
        """;

    private const string ExistingTargetXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="ja-JP">
          <file id="Test.Product.LoginLangResourceKind">
            <unit id="1" name="Title">
              <notes><note>Translator note</note></notes>
              <segment><source>Sign in</source><target state="translated">ログイン</target></segment>
            </unit>
            <unit id="2" name="Body">
              <segment><source>Welcome</source><target state="final">ようこそ</target></segment>
            </unit>
          </file>
        </xliff>
        """;
}
