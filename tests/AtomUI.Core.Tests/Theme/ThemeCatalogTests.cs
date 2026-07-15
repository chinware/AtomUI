using System.Globalization;
using System.Text;
using AtomUI.Theme;
using AtomUI.Theme.Catalog;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Language;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Shouldly;
using Xunit;
using AtomUITheme = AtomUI.Theme.Theme;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeCatalogTests
{
    private static readonly IReadOnlySet<string> s_sharedTokenNames =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "ColorPrimary",
            "BorderRadius"
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> s_componentOwnTokenNames =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["Button"] = new HashSet<string>(StringComparer.Ordinal)
            {
                "BorderColor",
                "Height"
            }
        };

    [Fact]
    public void Theme_Facade_Loads_Preparsed_Descriptor_Through_Compiler_Without_Reopening_For_Variants()
    {
        var source = new TestThemeSource("themes/Brand.xml", ThemeXmlWithCompilerButton("Brand"));
        var catalog = CreateCatalog(
            [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            source);
        var descriptor = catalog.GetDescriptor("Brand").ShouldNotBeNull();

        var defaultTheme = new AtomUITheme(
            descriptor,
            catalog,
            new ThemeCompiler(),
            [ThemeAlgorithm.Default]);
        var darkTheme = new AtomUITheme(
            descriptor,
            catalog,
            new ThemeCompiler(),
            [ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);

        defaultTheme.Load();
        darkTheme.Load();

        source.OpenCount.ShouldBe(1);
    }

    [Fact]
    public void Theme_Facade_Exposes_Compiled_Tokens_And_Resources_Through_ITheme()
    {
        var source = new TestThemeSource("themes/Brand.xml", ThemeXmlWithCompilerButton("Brand"));
        var catalog = CreateCatalog(
            [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            source);
        var descriptor = catalog.GetDescriptor("Brand").ShouldNotBeNull();
        ITheme theme = new AtomUITheme(
            descriptor,
            catalog,
            new ThemeCompiler(),
            [ThemeAlgorithm.Default]);

        ((AtomUITheme)theme).Load();

        theme.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        theme.ThemeResource[SharedTokenKind.ColorPrimary]
             .ShouldBeOfType<ImmutableSolidColorBrush>()
             .Color
             .ShouldBe(Color.Parse("#00b96b"));
        theme.GetControlToken(CompilerButtonToken.ID)
             .ShouldBeOfType<CompilerButtonToken>()
             .Height
             .ShouldBe(48);
        theme.ThemeResource[CompilerButtonTokenKind.Height].ShouldBe(48d);
    }

    [Fact]
    public void Theme_Facade_Keeps_Component_Shared_Overrides_Out_Of_Global_Resources()
    {
        var source = new TestThemeSource(
            "themes/Brand.xml",
            """
            <Theme Name="Brand" IsDefault="true">
              <SharedTokens>
                <Token Name="ColorPrimary" Value="#00b96b" />
              </SharedTokens>
              <ControlTokens>
                <ControlToken Id="Button">
                  <Token Name="ColorPrimary" IsShared="true" Value="#ff4d4f" />
                </ControlToken>
              </ControlTokens>
            </Theme>
            """);
        var catalog = CreateCatalog(
            [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            source);
        var descriptor = catalog.GetDescriptor("Brand").ShouldNotBeNull();
        var theme = new AtomUITheme(
            descriptor,
            catalog,
            new ThemeCompiler(),
            [ThemeAlgorithm.Default]);

        theme.Load();

        theme.ThemeResource[SharedTokenKind.ColorPrimary]
             .ShouldBeOfType<ImmutableSolidColorBrush>()
             .Color
             .ShouldBe(Color.Parse("#00b96b"));
        theme.GetControlToken(CompilerButtonToken.ID)!
             .GetSharedResourceDeltaDictionary()[SharedTokenKind.ColorPrimary]
             .ShouldBe(Color.Parse("#ff4d4f"));
    }

    [Fact]
    public void Theme_Facade_Failed_Hydration_Does_Not_Publish_Partial_State_And_Can_Retry()
    {
        var source = new TestThemeSource("themes/Brand.xml", ThemeXml("Brand", isDefault: true));
        var catalog = CreateCatalog(source);
        var descriptor = catalog.GetDescriptor("Brand").ShouldNotBeNull();
        var theme = new RetryAfterHydrationFailureTheme(descriptor, catalog);

        Should.Throw<ArgumentException>(() => theme.Load());

        theme.ThemeResource.ShouldBeEmpty();
        theme.GetControlToken(CompilerButtonToken.ID).ShouldBeNull();
        theme.IsLoaded.ShouldBeFalse();

        theme.Load();

        theme.IsLoaded.ShouldBeTrue();
        theme.GetControlToken(CompilerButtonToken.ID).ShouldNotBeNull();
    }

    [Fact]
    public void Theme_Builder_Extension_Preserves_Base_Id_For_The_Concrete_Builder()
    {
        var builder = new ThemeManagerBuilder();

        builder.WithDefaultTheme("Brand", ThemeAlgorithm.Compact, ThemeAlgorithm.Dark);

        builder.ThemeId.ShouldBe("Brand-Dark-Compact");
        builder.ExplicitDefaultThemeBaseId.ShouldBe("Brand");
    }

    [Fact]
    public void Theme_Builder_Extension_Keeps_The_Interface_Builder_Behavior()
    {
        IThemeManagerBuilder builder = new RecordingThemeManagerBuilder();

        builder.WithDefaultTheme("Brand", ThemeAlgorithm.Compact, ThemeAlgorithm.Dark);

        ((RecordingThemeManagerBuilder)builder).ThemeId.ShouldBe("Brand-Dark-Compact");
    }

    [Fact]
    public void Catalog_Parses_One_File_Once_For_All_Variants()
    {
        var source = new TestThemeSource("themes/Brand.xml", ThemeXml("Brand"));
        var catalog = CreateCatalog(source);

        catalog.GetDescriptor("Brand").ShouldNotBeNull();
        catalog.CreateCompileRequest("Brand", [ThemeAlgorithm.Default]);
        catalog.CreateCompileRequest("Brand", [ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);

        source.OpenCount.ShouldBe(1);
    }

    [Fact]
    public void Catalog_Uses_The_Lower_Priority_Custom_Descriptor()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("custom/Brand.xml", ThemeXml("Custom"), sourcePriority: 0),
            new TestThemeSource("app-data/Brand.xml", ThemeXml("App Data"), sourcePriority: 1));

        var descriptor = catalog.GetDescriptor("Brand").ShouldNotBeNull();

        descriptor.Definition!.DisplayName.ShouldBe("Custom");
        descriptor.SourcePriority.ShouldBe(0);
    }

    [Fact]
    public void Catalog_Records_A_Stable_Diagnostic_For_Duplicate_Ids()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("custom/Brand.xml", ThemeXml("Custom"), sourcePriority: 0),
            new TestThemeSource("assets/Brand.xml", ThemeXml("Built in"), sourcePriority: 3, isBuiltIn: true));

        var diagnostic = catalog.Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM011");
        diagnostic.Severity.ShouldBe(ThemeDiagnosticSeverity.Warning);
        diagnostic.FilePath.ShouldBe("assets/Brand.xml");
        diagnostic.Message.ShouldBe(
            "Theme id 'Brand' from 'custom/Brand.xml' (priority 0) wins over duplicate source 'assets/Brand.xml' (priority 3).");
    }

    [Fact]
    public void Catalog_Sorts_Paths_Ordinally_Within_A_Source_Group()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("themes/zeta.xml", ThemeXml("Zeta"), sourcePriority: 4),
            new TestThemeSource("themes/Alpha.xml", ThemeXml("Alpha"), sourcePriority: 4));

        catalog.Descriptors.Select(static descriptor => descriptor.Id)
               .ShouldBe(["Alpha", "zeta"]);
    }

    [Fact]
    public void Explicit_Builder_Default_Selection_Is_Tracked_And_Wins_Over_IsDefault_Fallback()
    {
        var builder = new ThemeManagerBuilder();
        builder.HasExplicitDefaultTheme.ShouldBeFalse();
        builder.WithDefaultTheme("Selected");

        var catalog = CreateCatalog(
            new TestThemeSource("themes/Selected.xml", ThemeXml("Selected", isDefault: false)),
            new TestThemeSource("themes/Fallback.xml", ThemeXml("Fallback", isDefault: true)));

        builder.HasExplicitDefaultTheme.ShouldBeTrue();
        catalog.ResolveDefaultDescriptor(builder.ExplicitDefaultThemeBaseId)
               .Id.ShouldBe("Selected");
    }

    [Fact]
    public void Catalog_Uses_The_Unique_Available_IsDefault_Descriptor_When_No_Explicit_Selection_Exists()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("themes/First.xml", ThemeXml("First", isDefault: false)),
            new TestThemeSource("themes/Fallback.xml", ThemeXml("Fallback", isDefault: true)));

        catalog.ResolveDefaultDescriptor(null).Id.ShouldBe("Fallback");
    }

    [Fact]
    public void Catalog_Rejects_Zero_Available_IsDefault_Descriptors_Without_An_Explicit_Selection()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("themes/First.xml", ThemeXml("First", isDefault: false)));

        Should.Throw<ThemeLoadException>(() => catalog.ResolveDefaultDescriptor(null));
    }

    [Fact]
    public void Catalog_Rejects_Multiple_Available_IsDefault_Descriptors_Without_An_Explicit_Selection()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("themes/First.xml", ThemeXml("First", isDefault: true)),
            new TestThemeSource("themes/Second.xml", ThemeXml("Second", isDefault: true)));

        Should.Throw<ThemeLoadException>(() => catalog.ResolveDefaultDescriptor(null));
    }

    [Fact]
    public void Catalog_Keeps_An_Unselected_Invalid_Custom_Theme_As_Unavailable()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("custom/Broken.xml", "<Theme", sourcePriority: 0),
            new TestThemeSource("themes/Fallback.xml", ThemeXml("Fallback", isDefault: true), sourcePriority: 1));

        var broken = catalog.GetDescriptor("Broken").ShouldNotBeNull();

        broken.IsAvailable.ShouldBeFalse();
        catalog.ResolveDefaultDescriptor(null).Id.ShouldBe("Fallback");
    }

    [Fact]
    public void Catalog_Rejects_An_Explicitly_Selected_Unavailable_Theme()
    {
        var catalog = CreateCatalog(
            new TestThemeSource("custom/Broken.xml", "<Theme"),
            new TestThemeSource("themes/Fallback.xml", ThemeXml("Fallback", isDefault: true)));

        Should.Throw<ThemeLoadException>(() => catalog.ResolveDefaultDescriptor("Broken"));
    }

    [Fact]
    public void Catalog_Identifies_And_Rejects_An_Invalid_Required_BuiltIn_Default()
    {
        var source = new TestThemeSource(
            "avares://AtomUI.Core/Assets/Themes/DaybreakBlue.xml",
            "<Theme",
            isBuiltIn: true,
            isRequiredBuiltInDefault: true);
        var catalog = CreateCatalog(source);

        var descriptor = catalog.GetDescriptor(IThemeManager.DEFAULT_THEME_ID).ShouldNotBeNull();

        descriptor.DefinitionFilePath.ShouldBe(source.DefinitionFilePath);
        descriptor.IsAvailable.ShouldBeFalse();
        Should.Throw<ThemeLoadException>(() => catalog.EnsureRequiredBuiltInThemesAvailable());
    }

    [Fact]
    public void Catalog_Rejects_A_Missing_Required_Core_DaybreakBlue_Source()
    {
        var catalog = CreateCatalog(
            new TestThemeSource(
                "avares://AtomUI.Core/Assets/Themes/Other.xml",
                ThemeXml("Other", isDefault: true),
                isBuiltIn: true));

        var exception = Should.Throw<ThemeLoadException>(() => catalog.EnsureRequiredBuiltInThemesAvailable());

        exception.Message.ShouldContain(IThemeManager.DEFAULT_THEME_ID);
    }

    [Fact]
    public void Catalog_Records_Source_Open_Failures_As_Unavailable_Descriptors()
    {
        var source = new ThrowingThemeSource("custom/Broken.xml", "source is unavailable");
        var catalog = CreateCatalog(source);

        var descriptor = catalog.GetDescriptor("Broken").ShouldNotBeNull();
        var diagnostic = descriptor.Diagnostics.ShouldHaveSingleItem();

        descriptor.IsAvailable.ShouldBeFalse();
        diagnostic.Code.ShouldBe("ATMTHM012");
        diagnostic.Severity.ShouldBe(ThemeDiagnosticSeverity.Error);
        diagnostic.FilePath.ShouldBe("custom/Broken.xml");
        diagnostic.Message.ShouldContain("source is unavailable");
    }

    [Fact]
    public void CreateCompileRequest_Filters_Unregistered_Optional_Component_Definitions()
    {
        var source = new TestThemeSource(
            "themes/Brand.xml",
            """
            <Theme Name="Brand" IsDefault="true">
              <ControlTokens>
                <ControlToken Id="OptionalWidget" EnableAlgorithm="true">
                  <Token Name="Accent" Value="x" />
                </ControlToken>
              </ControlTokens>
            </Theme>
            """);
        var catalog = CreateCatalog(source);

        var descriptor = catalog.GetDescriptor("Brand").ShouldNotBeNull();
        var request = catalog.CreateCompileRequest("Brand", [ThemeAlgorithm.Default]);
        var result = new ThemeCompiler().Compile(request);

        descriptor.Definition!.ControlTokens.ContainsKey("OptionalWidget").ShouldBeTrue();
        descriptor.Diagnostics.ShouldContain(diagnostic => diagnostic.Code == "ATMTHM010");
        request.Definition.ControlTokens.ShouldBeEmpty();
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void Catalog_And_Compile_Requests_Copy_Mutable_Inputs()
    {
        var source = new TestThemeSource("themes/Brand.xml", ThemeXml("Brand"));
        var catalog = CreateCatalog(source);
        var algorithms = new List<ThemeAlgorithm> { ThemeAlgorithm.Default };

        var request = catalog.CreateCompileRequest("Brand", algorithms);
        algorithms.Add(ThemeAlgorithm.Dark);

        request.Algorithms.ShouldBe([ThemeAlgorithm.Default]);
        Should.Throw<NotSupportedException>(() =>
            ((IList<ThemeAlgorithm>)request.Algorithms).Add(ThemeAlgorithm.Compact));
        Should.Throw<NotSupportedException>(() =>
            ((IList<ThemeDescriptor>)catalog.Descriptors).Add(catalog.GetDescriptor("Brand")!));
    }

    [Fact]
    public void ThemeDescriptor_Copies_And_Exposes_Read_Only_Diagnostics()
    {
        var diagnostics = new List<ThemeDefinitionDiagnostic>
        {
            new("TEST001", ThemeDiagnosticSeverity.Warning, "Brand.xml", 0, 0, "ThemeCatalog", "Warning")
        };
        var descriptor = new ThemeDescriptor("Brand", "Brand.xml", false, 0, null, diagnostics);
        diagnostics.Clear();

        descriptor.Diagnostics.ShouldHaveSingleItem().Code.ShouldBe("TEST001");
        Should.Throw<NotSupportedException>(() =>
            ((IList<ThemeDefinitionDiagnostic>)descriptor.Diagnostics).Add(
                new ThemeDefinitionDiagnostic("TEST002", ThemeDiagnosticSeverity.Warning, "Brand.xml", 0, 0, "ThemeCatalog", "Warning")));
    }

    private static ThemeCatalog CreateCatalog(params IThemeCatalogSource[] sources)
    {
        return CreateCatalog(Array.Empty<ControlTokenRegistration>(), sources);
    }

    private static ThemeCatalog CreateCatalog(
        IReadOnlyList<ControlTokenRegistration> registrations,
        params IThemeCatalogSource[] sources)
    {
        return new ThemeCatalog(
            sources,
            s_sharedTokenNames,
            s_componentOwnTokenNames,
            registrations);
    }

    private static string ThemeXml(string displayName, bool isDefault = false)
    {
        return $"<Theme Name=\"{displayName}\" IsDefault=\"{isDefault.ToString().ToLowerInvariant()}\" />";
    }

    private static string ThemeXmlWithCompilerButton(string displayName)
    {
        return $"""
                <Theme Name="{displayName}" IsDefault="true">
                  <SharedTokens>
                    <Token Name="ColorPrimary" Value="#00b96b" />
                  </SharedTokens>
                  <ControlTokens>
                    <ControlToken Id="Button">
                      <Token Name="Height" Value="48" />
                    </ControlToken>
                  </ControlTokens>
                </Theme>
                """;
    }

    private sealed class TestThemeSource : IThemeCatalogSource
    {
        private readonly string _xml;

        public TestThemeSource(
            string definitionFilePath,
            string xml,
            int sourcePriority = 0,
            bool isBuiltIn = false,
            bool isRequiredBuiltInDefault = false)
        {
            Id                       = Path.GetFileNameWithoutExtension(definitionFilePath);
            DefinitionFilePath       = definitionFilePath;
            SourcePriority           = sourcePriority;
            IsBuiltIn                = isBuiltIn;
            IsRequiredBuiltInDefault = isRequiredBuiltInDefault;
            _xml                     = xml;
        }

        public string Id { get; }
        public string DefinitionFilePath { get; }
        public bool IsBuiltIn { get; }
        public bool IsRequiredBuiltInDefault { get; }
        public int SourcePriority { get; }
        public int OpenCount { get; private set; }

        public Stream OpenRead()
        {
            OpenCount++;
            return new MemoryStream(Encoding.UTF8.GetBytes(_xml), writable: false);
        }
    }

    private sealed class ThrowingThemeSource : IThemeCatalogSource
    {
        private readonly string _message;

        public ThrowingThemeSource(string definitionFilePath, string message)
        {
            Id                       = Path.GetFileNameWithoutExtension(definitionFilePath);
            DefinitionFilePath       = definitionFilePath;
            _message                 = message;
        }

        public string Id { get; }
        public string DefinitionFilePath { get; }
        public bool IsBuiltIn => false;
        public bool IsRequiredBuiltInDefault => false;
        public int SourcePriority => 0;

        public Stream OpenRead()
        {
            throw new IOException(_message);
        }
    }

    private sealed class RetryAfterHydrationFailureTheme : AtomUITheme
    {
        private int _compileCount;

        public RetryAfterHydrationFailureTheme(ThemeDescriptor descriptor, ThemeCatalog catalog)
            : base(descriptor, catalog, new ThemeCompiler(), [ThemeAlgorithm.Default])
        {
        }

        protected override ThemeCompileResult Compile(ThemeCompileRequest request)
        {
            _compileCount++;
            return new ThemeCompileResult(
                _compileCount == 1 ? CreateDuplicateTokenSnapshot() : CreateValidSnapshot(),
                Array.Empty<ThemeDefinitionDiagnostic>(),
                null);
        }

        private static ThemeSnapshot CreateDuplicateTokenSnapshot()
        {
            var components = new Dictionary<ComponentTokenIdentity, ComponentThemeSnapshot>
            {
                [new ComponentTokenIdentity("First", CompilerButtonToken.ID)] = CreateComponent(),
                [new ComponentTokenIdentity("Second", CompilerButtonToken.ID)] = CreateComponent()
            };
            return CreateSnapshot(components);
        }

        private static ThemeSnapshot CreateValidSnapshot()
        {
            var components = new Dictionary<ComponentTokenIdentity, ComponentThemeSnapshot>
            {
                [new ComponentTokenIdentity(null, CompilerButtonToken.ID)] = CreateComponent()
            };
            return CreateSnapshot(components);
        }

        private static ComponentThemeSnapshot CreateComponent()
        {
            return new ComponentThemeSnapshot(
                new DesignToken(),
                new Dictionary<object, object?>(),
                new CompilerButtonToken(),
                new Dictionary<object, object?>());
        }

        private static ThemeSnapshot CreateSnapshot(
            IReadOnlyDictionary<ComponentTokenIdentity, ComponentThemeSnapshot> components)
        {
            return new ThemeSnapshot(
                "Brand",
                1,
                [ThemeAlgorithm.Default],
                false,
                new DesignToken(),
                new Dictionary<object, object?>(),
                components);
        }
    }

    private sealed class RecordingThemeManagerBuilder : IThemeManagerBuilder
    {
        public IList<Type> ControlDesignTokens { get; } = new List<Type>();
        public IList<IThemeAssetPathProvider> ThemeAssetPathProviders { get; } = new List<IThemeAssetPathProvider>();
        public IList<IControlThemesProvider> ControlThemesProviders { get; } = new List<IControlThemesProvider>();
        public IList<LanguageProvider> LanguageProviders { get; } = new List<LanguageProvider>();
        public IList<EventHandler> InitializedHandlers { get; } = new List<EventHandler>();
        public LanguageVariant LanguageVariant { get; private set; } = LanguageVariant.en_US;
        public string ThemeId { get; private set; } = IThemeManager.DEFAULT_THEME_ID;

        public void AddControlToken(Type tokenType)
        {
            ControlDesignTokens.Add(tokenType);
        }

        public void AddControlThemesProvider(IThemeAssetPathProvider themeAssetPathProvider)
        {
            ThemeAssetPathProviders.Add(themeAssetPathProvider);
        }

        public void AddControlThemesProvider(IControlThemesProvider controlThemesProvider)
        {
            ControlThemesProviders.Add(controlThemesProvider);
        }

        public void AddLanguageProviders(LanguageProvider languageProvider)
        {
            LanguageProviders.Add(languageProvider);
        }

        public void WithDefaultTheme(string themeId)
        {
            ThemeId = themeId;
        }

        public void WithDefaultFontFamily(FontFamily fontFamily)
        {
        }

        public void WithDefaultFontFamily(string fontFamily)
        {
        }

        public void WithDefaultCultureInfo(CultureInfo cultureInfo)
        {
            LanguageVariant = LanguageVariant.FromCultureInfo(cultureInfo);
        }

        public void WithDefaultLanguageVariant(LanguageVariant languageVariant)
        {
            LanguageVariant = languageVariant;
        }

        public void WithThemeVariantCalculatorFactory(IThemeVariantCalculatorFactory factory)
        {
        }
    }
}
