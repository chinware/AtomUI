using AtomUI.Build.Tasks;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class CollectLanguageCatalogsTaskTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"atomui-catalog-collection-tests-{Guid.NewGuid():N}");

    public CollectLanguageCatalogsTaskTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Execute_Normalizes_Deduplicates_And_Annotates_Catalog_Templates()
    {
        var path = Write("DatePicker.catalog.xlf", CatalogXliff);
        var item = CatalogItem(path);
        var engine = new RecordingBuildEngine();
        var task = new CollectLanguageCatalogsTask
        {
            BuildEngine = engine,
            CatalogFiles = [item, CatalogItem(Path.Combine(_directory, ".", Path.GetFileName(path)))]
        };

        task.Execute().ShouldBeTrue();
        engine.Errors.ShouldBeEmpty();
        var catalog = task.Catalogs.ShouldHaveSingleItem();
        catalog.ItemSpec.ShouldBe(Path.GetFullPath(path));
        catalog.GetMetadata("AtomUILanguageModuleId").ShouldBe("AtomUI.Desktop.Controls");
        catalog.GetMetadata("AtomUILanguageCatalogId")
               .ShouldBe("AtomUI.Desktop.Controls.DatePickerLang.DatePickerLangResourceKind");
        catalog.GetMetadata("AtomUILanguageSourceFingerprint")
               .ShouldMatch("^[0-9a-f]{64}$");
    }

    [Fact]
    public void Execute_Rejects_Different_Files_For_The_Same_Catalog_Identity()
    {
        var first = Write("first.catalog.xlf", CatalogXliff);
        var second = Write("second.catalog.xlf", CatalogXliff.Replace("Today", "Current day"));
        var engine = new RecordingBuildEngine();
        var task = new CollectLanguageCatalogsTask
        {
            BuildEngine = engine,
            CatalogFiles = [CatalogItem(first), CatalogItem(second)]
        };

        task.Execute().ShouldBeFalse();
        var error = engine.Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILOC006");
        error.Message.ShouldNotBeNull().ShouldContain("more than one template");
    }

    public void Dispose()
    {
        Directory.Delete(_directory, recursive: true);
    }

    private static TestTaskItem CatalogItem(string path)
    {
        return new TestTaskItem(
            path,
            ("AtomUILanguageModuleId", "AtomUI.Desktop.Controls"));
    }

    private string Write(string fileName, string content)
    {
        var path = Path.Combine(_directory, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    private const string CatalogXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="AtomUI.Desktop.Controls.DatePickerLang.DatePickerLangResourceKind">
            <unit id="Today">
              <segment><source>Today</source></segment>
            </unit>
          </file>
        </xliff>
        """;
}
