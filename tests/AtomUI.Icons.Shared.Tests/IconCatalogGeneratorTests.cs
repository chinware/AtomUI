using System.Text;
using AtomUI.Icons;
using Shouldly;
using Xunit;

namespace AtomUI.Icons.Shared.Tests;

public class IconCatalogGeneratorTests
{
    [Fact]
    public async Task GenerateAsyncWritesAotSafeIconCatalog()
    {
        var rootPath   = Path.Combine(Path.GetTempPath(), "AtomUI.Icons.Shared.Tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(rootPath, "Assets");
        var targetPath = Path.Combine(rootPath, "Output");

        try
        {
            Directory.CreateDirectory(Path.Combine(sourcePath, "outlined"));
            Directory.CreateDirectory(targetPath);
            await File.WriteAllTextAsync(Path.Combine(sourcePath, "outlined", "home.svg"), "<svg />", TestContext.Current.CancellationToken);

            var generator = new TestIconPackageGenerator(sourcePath, targetPath);

            await generator.GenerateAsync(TestContext.Current.CancellationToken);

            var catalogPath = Path.Combine(targetPath, "TestIconCatalog.g.cs");
            File.Exists(catalogPath).ShouldBeTrue();

            var catalogSource = await File.ReadAllTextAsync(catalogPath, TestContext.Current.CancellationToken);
            catalogSource.ShouldContain("public readonly record struct TestIconInfo");
            catalogSource.ShouldContain("public static class TestIconCatalog");
            catalogSource.ShouldContain("public static IReadOnlyList<TestIconInfo> GetIcons()");
            catalogSource.ShouldContain("new(\"HomeOutlined\", IconThemeType.Outlined, TestIconKind.HomeOutlined, typeof(HomeOutlined), static () => new HomeOutlined())");
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
        }
    }

    private sealed class TestIconPackageGenerator : DefaultIconPackageGenerator
    {
        public TestIconPackageGenerator(string sourcePath, string targetPath)
            : base(sourcePath, targetPath)
        {
            PackageName      = "Test";
            PackageNamespace = "Test.Icons";
        }

        protected override async Task GenerateIconPackageClass(IconFileInfo iconFileInfo, Stream output)
        {
            var className = $"{iconFileInfo.Name}{iconFileInfo.ThemeType}";
            var sourceText = $$"""
                               namespace {{PackageNamespace}};

                               public class {{className}} : AtomUI.Controls.Icon
                               {
                               }
                               """;
            await output.WriteAsync(Encoding.UTF8.GetBytes(sourceText));
        }
    }
}
