using System.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.DesignTokens;
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

        properties.ShouldBe([nameof(IThemeManager.AvailableThemes), nameof(IThemeManager.CurrentTheme)]);
        events.ShouldBe([nameof(IThemeManager.ThemeChangeFailed), nameof(IThemeManager.ThemeChanged)]);
        typeof(IThemeManager).GetProperty(nameof(IThemeManager.AvailableThemes))!.PropertyType
                             .ShouldBe(typeof(IReadOnlyList<ThemeInfo>));
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
}
