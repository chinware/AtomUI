using System.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.DesignTokens;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeLegacyApiRemovalTests
{
    [Fact]
    public void IThemeManager_Exposes_Only_The_Final_Theme_Runtime_Contract()
    {
        var properties = typeof(IThemeManager)
                         .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                         .Select(static property => property.Name)
                         .OrderBy(static name => name, StringComparer.Ordinal)
                         .ToArray();
        var events = typeof(IThemeManager)
                     .GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Select(static @event => @event.Name)
                     .OrderBy(static name => name, StringComparer.Ordinal)
                     .ToArray();
        var methods = typeof(IThemeManager)
                      .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                      .Where(static method => !method.IsSpecialName)
                      .Select(static method => method.Name)
                      .OrderBy(static name => name, StringComparer.Ordinal)
                      .ToArray();

        properties.ShouldBe([
            nameof(IThemeManager.AvailableThemes),
            nameof(IThemeManager.CurrentTheme),
            nameof(IThemeManager.SemanticParts),
            nameof(IThemeManager.ThemeCatalogDiagnostics)
        ]);
        events.ShouldBe([
            nameof(IThemeManager.ThemeCatalogChanged),
            nameof(IThemeManager.ThemeChangeFailed),
            nameof(IThemeManager.ThemeChanged)
        ]);
        methods.ShouldBe([
            nameof(IThemeManager.ApplyThemeAsync),
            nameof(IThemeManager.ReloadThemesAsync)
        ]);
        typeof(IThemeManager).GetProperty(nameof(IThemeManager.AvailableThemes))!.PropertyType
                             .ShouldBe(typeof(IReadOnlyList<ThemeInfo>));
    }

    [Fact]
    public void Theme_Builder_And_Resolver_Public_Api_Matches_The_Planned_Contract()
    {
        var builderMethods = typeof(IThemeManagerBuilder)
                             .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                             .Select(static method => method.Name)
                             .OrderBy(static name => name, StringComparer.Ordinal)
                             .ToArray();

        builderMethods.ShouldBe([
            nameof(IThemeManagerBuilder.AddControlPackage),
            nameof(IThemeManagerBuilder.AddInitializer),
            nameof(IThemeManagerBuilder.AddThemeDefinitionResolver),
            nameof(IThemeManagerBuilder.UseUserThemeDirectory),
            nameof(IThemeManagerBuilder.UseUserThemeDirectory),
            nameof(IThemeManagerBuilder.WithApplicationId),
            nameof(IThemeManagerBuilder.WithDefaultFontFamily),
            nameof(IThemeManagerBuilder.WithDefaultFontFamily),
            nameof(IThemeManagerBuilder.WithFollowSystemThemes),
            nameof(IThemeManagerBuilder.WithInitialTheme)
        ]);
        typeof(IThemeDefinitionResolver).GetProperty(nameof(IThemeDefinitionResolver.Id)).ShouldNotBeNull();
        typeof(IThemeDefinitionResolver).GetProperty(nameof(IThemeDefinitionResolver.SupportsReload)).ShouldNotBeNull();
        typeof(IThemeDefinitionResolver).GetMethod(nameof(IThemeDefinitionResolver.Resolve)).ShouldNotBeNull();
        typeof(IThemeDefinitionSource).GetMethod(nameof(IThemeDefinitionSource.OpenRead)).ShouldNotBeNull();
    }

    [Fact]
    public void ThemeInfo_Exposes_Optional_ReadOnly_AccentColor_Without_Removing_The_Existing_Constructor()
    {
        var accentColorProperty = typeof(ThemeInfo).GetProperty(nameof(ThemeInfo.AccentColor));

        accentColorProperty.ShouldNotBeNull();
        accentColorProperty.PropertyType.ShouldBe(typeof(Color?));
        accentColorProperty.SetMethod.ShouldBeNull();
        typeof(ThemeInfo).GetConstructor([
            typeof(string),
            typeof(string),
            typeof(ThemeAppearance),
            typeof(bool)
        ]).ShouldNotBeNull();
        typeof(ThemeInfo).GetConstructor([
            typeof(string),
            typeof(string),
            typeof(ThemeAppearance),
            typeof(bool),
            typeof(Color?)
        ]).ShouldNotBeNull();
    }

    [Fact]
    public void ThemeConfigProvider_Exposes_Immutable_Config_And_Inherited_Child_Only()
    {
        var declaredProperties = typeof(ThemeConfigProvider)
                                 .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                 .Select(static property => property.Name)
                                 .OrderBy(static name => name, StringComparer.Ordinal)
                                 .ToArray();

        declaredProperties.ShouldBe([nameof(ThemeConfigProvider.Config)]);
        typeof(ThemeConfigProvider).GetProperty("Child").ShouldNotBeNull();
    }

    [Fact]
    public void ThemeScope_Exposes_Only_The_Stable_Context_Property()
    {
        var fields = typeof(ThemeScope)
                     .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Select(static field => field.Name)
                     .OrderBy(static name => name, StringComparer.Ordinal)
                     .ToArray();

        fields.ShouldBe([nameof(ThemeScope.ContextProperty)]);
    }

    [Fact]
    public void Legacy_Theme_Types_Are_Physically_Removed()
    {
        var assembly = typeof(IThemeManager).Assembly;
        var removedTypeNames = new[]
        {
            "AtomUI.Theme.Theme",
            "AtomUI.Theme.ITheme",
            "AtomUI.Theme.ThemeCoordinator",
            "AtomUI.Theme.ThemeTransitionEventArgs",
            "AtomUI.Theme.Compilation.DesignTokenClone",
            "AtomUI.Theme.Compilation.ThemeCompileRequest",
            "AtomUI.Theme.Compilation.ThemeSnapshotMaterializer",
            "AtomUI.Theme.Configuration.TokenSetter",
            "AtomUI.Theme.Configuration.ControlTokenInfoSetter",
            "AtomUI.Theme.IThemeConfigProvider",
            "AtomUI.Theme.Resources.ControlSharedTokenResourceExtension",
            "AtomUI.Theme.Resources.ControlTokenIdentity",
            "AtomUI.Theme.Resources.ThemeResourceKeyCache",
            "AtomUI.Theme.Definitions.ThemeCatalog",
            "AtomUI.Theme.Definitions.ThemeDescriptor",
            "AtomUI.Theme.Definitions.ThemeDefinitionModel",
            "AtomUI.Theme.Definitions.ThemeDefinitionParser",
            "AtomUI.Theme.Definitions.ThemeDefinitionParseResult",
            "AtomUI.Theme.DesignTokens.ControlTokenConfigInfo",
            "AtomUI.Theme.DesignTokens.IDesignToken",
            "AtomUI.Theme.DesignTokens.IControlDesignToken",
            "AtomUI.Theme.DesignTokens.TokenConfigBuckets"
        };

        foreach (var typeName in removedTypeNames)
        {
            assembly.GetType(typeName, throwOnError: false).ShouldBeNull(typeName);
        }
    }

    [Fact]
    public void Compile_Time_Token_Builders_Do_Not_Expose_Legacy_Runtime_Apis()
    {
        var removedMethods = new HashSet<string>(StringComparer.Ordinal)
        {
            "BuildResourceDictionary",
            "BuildSharedResourceDeltaDictionary",
            "Clone",
            "GetCustomTokens",
            "GetSharedResourceDeltaDictionary",
            "GetTokenKindType",
            "GetTokenProperties",
            "GetTokenValue",
            "HasCustomTokenConfig",
            "HasToken",
            "LoadConfig",
            "SetCustomTokens",
            "SetHasCustomTokenConfig",
            "SetTokenValue"
        };

        var remainingMethods = typeof(AbstractDesignToken)
                               .GetMethods(BindingFlags.Public |
                                           BindingFlags.NonPublic |
                                           BindingFlags.Instance |
                                           BindingFlags.Static |
                                           BindingFlags.DeclaredOnly)
                               .Concat(typeof(AbstractControlDesignToken)
                                       .GetMethods(BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.DeclaredOnly))
                               .Concat(typeof(DesignToken)
                                       .GetMethods(BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.DeclaredOnly))
                               .Select(static method => method.Name)
                               .Where(removedMethods.Contains)
                               .Distinct(StringComparer.Ordinal)
                               .OrderBy(static name => name, StringComparer.Ordinal)
                               .ToArray();

        remainingMethods.ShouldBeEmpty();
    }

    [Fact]
    public void Control_Token_Base_Exposes_Only_Effective_Global_Token_To_Derived_Tokens()
    {
        var type = typeof(AbstractControlDesignToken);

        type.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ShouldBeNull();
        type.GetField("SharedToken", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ShouldBeNull();
        type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                [typeof(string)],
                modifiers: null)
            .ShouldBeNull();

        var effectiveGlobalToken = type.GetProperty(
            "EffectiveGlobalToken",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        effectiveGlobalToken.ShouldNotBeNull();
        effectiveGlobalToken.PropertyType.ShouldBe(typeof(DesignToken));
        effectiveGlobalToken.GetMethod.ShouldNotBeNull();
        effectiveGlobalToken.GetMethod.IsFamily.ShouldBeTrue();

        type.GetMethod(
                "AssignEffectiveGlobalToken",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ShouldNotBeNull();
        type.GetMethod(
                "AssignSharedToken",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ShouldBeNull();
    }
}
