using AtomUI.Theme;
using AtomUI.Theme.Catalog;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.TokenSystem;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeManagerTests
{
    [Fact]
    public void ScanThemes_Reports_Throwing_Control_Token_Activation_Before_Catalog_Publication()
    {
        var manager = CreateManager(typeof(ThrowingConstructorCompilerButtonToken));

        var exception = Should.Throw<ThemeLoadException>(() => manager.ScanThemes());

        exception.Message.ShouldContain(nameof(ThrowingConstructorCompilerButtonToken));
        exception.Message.ShouldContain(ThrowingConstructorCompilerButtonToken.ExceptionMessage);
    }

    [Fact]
    public void ScanThemes_Reports_Non_Control_Token_Registration_Before_Catalog_Publication()
    {
        var manager = CreateManager(typeof(InvalidCompilerToken));

        var exception = Should.Throw<ThemeLoadException>(() => manager.ScanThemes());

        exception.Message.ShouldContain(nameof(InvalidCompilerToken));
        exception.Message.ShouldContain(nameof(AbstractControlDesignToken));
    }

    [Fact]
    public void ScanThemes_Reports_Duplicate_Control_Token_Ids_Before_Catalog_Publication()
    {
        var manager = CreateManager(typeof(CompilerButtonToken), typeof(DuplicateCompilerButtonToken));

        var exception = Should.Throw<ThemeLoadException>(() => manager.ScanThemes());

        exception.Message.ShouldContain(CompilerButtonToken.ID);
    }

    [Fact]
    public void ScanThemes_Activates_Each_Registered_Control_Token_Once_For_Schema_Preflight()
    {
        ActivationCountingCompilerButtonToken.ResetActivationCount();
        var manager = CreateManager(typeof(ActivationCountingCompilerButtonToken));

        manager.CreateControlTokenSchemas();

        ActivationCountingCompilerButtonToken.ActivationCount.ShouldBe(1);
    }

    [Fact]
    public void CreateControlTokenSchemas_Rejects_Inherited_Id_Before_Compile_Request_Creation()
    {
        var manager = CreateManager(typeof(CompilerButtonToken));
        var schemas = manager.CreateControlTokenSchemas();
        var catalog = new ThemeCatalog(
            [new TestThemeSource(
                "themes/Brand.xml",
                """
                <Theme Name="Brand" IsDefault="true">
                  <ControlTokens>
                    <ControlToken Id="Button">
                      <Token Name="Id" Value="Overridden" />
                    </ControlToken>
                  </ControlTokens>
                </Theme>
                """)],
            new HashSet<string>(StringComparer.Ordinal),
            schemas,
            [new ControlTokenRegistration(typeof(CompilerButtonToken))]);

        schemas[CompilerButtonToken.ID].ShouldContain(nameof(CompilerButtonToken.Height));
        schemas[CompilerButtonToken.ID].ShouldNotContain(nameof(AbstractControlDesignToken.Id));
        var descriptor = catalog.GetDescriptor("Brand").ShouldNotBeNull();
        descriptor.IsAvailable.ShouldBeFalse();
        descriptor.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Code == "ATMTHM009" &&
            diagnostic.Message.Contains(nameof(AbstractControlDesignToken.Id), StringComparison.Ordinal));
        Should.Throw<ThemeLoadException>(() =>
            catalog.CreateCompileRequest("Brand", [ThemeAlgorithm.Default]));
    }

    private static ThemeManager CreateManager(params Type[] tokenTypes)
    {
        var manager = new ThemeManager();
        foreach (var tokenType in tokenTypes)
        {
            manager.RegisterControlTokenType(tokenType);
        }

        return manager;
    }

    private sealed class TestThemeSource : IThemeCatalogSource
    {
        private readonly string _xml;

        public TestThemeSource(string definitionFilePath, string xml)
        {
            Id                 = Path.GetFileNameWithoutExtension(definitionFilePath);
            DefinitionFilePath = definitionFilePath;
            _xml               = xml;
        }

        public string Id { get; }
        public string DefinitionFilePath { get; }
        public bool IsBuiltIn => false;
        public bool IsRequiredBuiltInDefault => false;
        public int SourcePriority => 0;

        public Stream OpenRead()
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(_xml), writable: false);
        }
    }
}
