using System.Xml.Linq;
using AtomUI.Build.Tasks;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class LanguagePackageTasksTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"atomui-language-package-tests-{Guid.NewGuid():N}");

    public LanguagePackageTasksTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Prepare_Writes_A_Deterministic_Manifest_For_Content_Only_Translations()
    {
        var languagePath = Write("ja-JP.xlf", JapaneseXliff);
        var manifestPath = Path.Combine(_directory, "AtomUI.LanguagePack.xml");
        var languageItem = LanguageItem(languagePath);
        var engine = new RecordingBuildEngine();
        var task = new PrepareLanguagePackageTask
        {
            BuildEngine = engine,
            PackageId = "AtomUI.Desktop.Controls.I18n.JaJP",
            LanguageFiles = [languageItem],
            PackageFiles = [languageItem],
            OutputManifestPath = manifestPath
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();
        File.Exists(manifestPath).ShouldBeTrue();

        var manifest = XDocument.Load(manifestPath);
        var root = manifest.Root.ShouldNotBeNull();
        ((string?)root.Attribute("packageId")).ShouldBe("AtomUI.Desktop.Controls.I18n.JaJP");
        ((string?)root.Attribute("language")).ShouldBe("ja-JP");
        var catalog = root.Elements("catalog").ShouldHaveSingleItem();
        ((string?)catalog.Attribute("moduleId")).ShouldBe("AtomUI.Desktop.Controls");
        ((string?)catalog.Attribute("catalogId"))
            .ShouldBe("AtomUI.Desktop.Controls.DatePickerLang.DatePickerLangResourceKind");
        ((string?)catalog.Attribute("contractVersion")).ShouldBe("1");
        ((string?)catalog.Attribute("path")).ShouldBe("Localization/DatePicker/ja-JP.xlf");
        ((string?)catalog.Attribute("sourceFingerprint"))
            .ShouldNotBeNull()
            .ShouldMatch("^[0-9a-f]{64}$");
    }

    [Theory]
    [InlineData("RuntimeInitializer.dll", "runtime assembly")]
    [InlineData("Initializer.cs", "runtime source")]
    [InlineData("download.atomlang", ".atomlang")]
    [InlineData("build/Install.targets", "build logic")]
    [InlineData("build/Install.props", "build logic")]
    [InlineData("install.ps1", "script")]
    [InlineData("install.sh", "script")]
    [InlineData("install.cmd", "script")]
    [InlineData("install.bat", "script")]
    public void Prepare_Rejects_Executable_Content_In_A_Static_Language_Package(
        string forbiddenFileName,
        string messageFragment)
    {
        var languagePath = Write("ja-JP.xlf", JapaneseXliff);
        var forbiddenPath = Write(forbiddenFileName, "forbidden content");
        var languageItem = LanguageItem(languagePath);
        var engine = new RecordingBuildEngine();
        var task = new PrepareLanguagePackageTask
        {
            BuildEngine = engine,
            PackageId = "AtomUI.Desktop.Controls.I18n.JaJP",
            LanguageFiles = [languageItem],
            PackageFiles = [languageItem, new TestTaskItem(forbiddenPath)],
            OutputManifestPath = Path.Combine(_directory, "invalid.xml")
        };

        task.Execute().ShouldBeFalse();
        var error = engine.Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILOC009");
        error.Message.ShouldNotBeNull().ShouldContain(messageFragment);
        File.Exists(task.OutputManifestPath).ShouldBeFalse();
    }

    [Fact]
    public void Prepare_Rejects_Build_Logic_Placed_By_PackagePath()
    {
        var languagePath = Write("ja-JP.xlf", JapaneseXliff);
        var forbiddenPath = Write("Install.targets", "forbidden content");
        var languageItem = LanguageItem(languagePath);
        var engine = new RecordingBuildEngine();
        var task = new PrepareLanguagePackageTask
        {
            BuildEngine = engine,
            PackageId = "AtomUI.Desktop.Controls.I18n.JaJP",
            LanguageFiles = [languageItem],
            PackageFiles =
            [
                languageItem,
                new TestTaskItem(
                    forbiddenPath,
                    ("Pack", "true"),
                    ("PackagePath", "buildTransitive/Install.targets"))
            ],
            OutputManifestPath = Path.Combine(_directory, "invalid.xml")
        };

        task.Execute().ShouldBeFalse();
        var error = engine.Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILOC009");
        error.Message.ShouldNotBeNull().ShouldContain("build logic");
        File.Exists(task.OutputManifestPath).ShouldBeFalse();
    }

    [Fact]
    public void Prepare_Validates_The_Final_PackagePath_Instead_Of_The_Source_Extension()
    {
        var languagePath = Write("ja-JP.xlf", JapaneseXliff);
        var sourcePath = Write("payload.dll", "documentation payload");
        var languageItem = LanguageItem(languagePath);
        var engine = new RecordingBuildEngine();
        var task = new PrepareLanguagePackageTask
        {
            BuildEngine = engine,
            PackageId = "AtomUI.Desktop.Controls.I18n.JaJP",
            LanguageFiles = [languageItem],
            PackageFiles =
            [
                languageItem,
                new TestTaskItem(
                    sourcePath,
                    ("Pack", "true"),
                    ("PackagePath", "contentFiles/any/any/payload.txt"))
            ],
            OutputManifestPath = Path.Combine(_directory, "valid.xml")
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();
        File.Exists(task.OutputManifestPath).ShouldBeTrue();
    }

    [Fact]
    public void GenerateProps_Emits_Only_Declarative_Static_Language_Items()
    {
        var languagePath = Write("ja-JP.xlf", JapaneseXliff);
        var outputPath = Path.Combine(_directory, "AtomUI.Desktop.Controls.I18n.JaJP.props");
        var engine = new RecordingBuildEngine();
        var task = new GenerateLanguagePackagePropsTask
        {
            BuildEngine = engine,
            PackageId = "AtomUI.Desktop.Controls.I18n.JaJP",
            LanguageFiles = [LanguageItem(languagePath)],
            OutputPath = outputPath
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();

        var props = XDocument.Load(outputPath);
        var item = props.Descendants("AtomUILanguage").ShouldHaveSingleItem();
        ((string?)item.Attribute("Include"))
            .ShouldBe("$(MSBuildThisFileDirectory)../contentFiles/any/any/Localization/DatePicker/ja-JP.xlf");
        ((string?)item.Attribute("AtomUILanguageSourceKind")).ShouldBe("StaticLanguagePack");
        ((string?)item.Attribute("AtomUILanguageSourceIdentity"))
            .ShouldBe("AtomUI.Desktop.Controls.I18n.JaJP");
        ((string?)item.Attribute("AtomUILanguageModuleId")).ShouldBe("AtomUI.Desktop.Controls");
        ((string?)item.Attribute("AtomUILanguageContractVersion")).ShouldBe("1");
        ((string?)item.Attribute("AtomUILanguagePackagePath"))
            .ShouldBe("Localization/DatePicker/ja-JP.xlf");
        ((string?)item.Attribute("AtomUILanguageSourceFingerprint"))
            .ShouldNotBeNull()
            .ShouldMatch("^[0-9a-f]{64}$");
        props.Descendants("UsingTask").ShouldBeEmpty();
        props.Descendants("Target").ShouldBeEmpty();
    }

    [Fact]
    public void GenerateProps_Emits_Module_BuiltIn_Source_Items()
    {
        var languagePath = Write("en-US.xlf", ModuleEnglishXliff);
        var outputPath = Path.Combine(_directory, "AtomUI.Desktop.Controls.Localization.props");
        var engine = new RecordingBuildEngine();
        var task = new GenerateLanguagePackagePropsTask
        {
            BuildEngine = engine,
            PackageId = "AtomUI.Desktop.Controls",
            SourceKind = "ModuleBuiltIn",
            RequireTargetLanguage = false,
            LanguageFiles = [LanguageItem(languagePath)],
            OutputPath = outputPath
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();

        var props = XDocument.Load(outputPath);
        var item = props.Descendants("AtomUILanguage").ShouldHaveSingleItem();
        ((string?)item.Attribute("AtomUILanguageSourceKind")).ShouldBe("ModuleBuiltIn");
    }

    public void Dispose()
    {
        Directory.Delete(_directory, recursive: true);
    }

    private TestTaskItem LanguageItem(string path)
    {
        return new TestTaskItem(
            path,
            ("AtomUILanguageModuleId", "AtomUI.Desktop.Controls"),
            ("AtomUILanguageContractVersion", "1"),
            ("AtomUILanguagePackagePath", "Localization/DatePicker/ja-JP.xlf"));
    }

    private string Write(string fileName, string content)
    {
        var path = Path.Combine(_directory, fileName);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(path, content);
        return path;
    }

    private const string JapaneseXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="ja-JP">
          <file id="AtomUI.Desktop.Controls.DatePickerLang.DatePickerLangResourceKind">
            <unit id="Today">
              <segment><source>Today</source><target state="reviewed">今日</target></segment>
            </unit>
          </file>
        </xliff>
        """;

    private const string ModuleEnglishXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="AtomUI.Desktop.Controls.DatePickerLang.DatePickerLangResourceKind">
            <unit id="Today">
              <segment><source>Today</source></segment>
            </unit>
          </file>
        </xliff>
        """;
}
