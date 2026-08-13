using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AtomUI.Localization;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using Avalonia.Media;

namespace AtomUI.LinkedRegistration.Fixtures;

internal static class FixtureHost
{
    private const string DesktopPackageId = "AtomUI.Desktop.Controls";

    internal static int Run(
        string fixtureName,
        Action<IAtomUIBuilder> register,
        params Type[] usageMarkers)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fixtureName);
        ArgumentNullException.ThrowIfNull(register);
        ArgumentNullException.ThrowIfNull(usageMarkers);

        var theme = new RecordingThemeManagerBuilder();
        var localization = new RecordingLocalizationBuilder();
        var builder = new RecordingAtomUIBuilder(theme, localization);

        register(builder);
        GC.KeepAlive(usageMarkers);

        WriteSnapshot(Console.OpenStandardOutput(), fixtureName, theme, localization);
        return 0;
    }

    private static void WriteSnapshot(
        Stream output,
        string fixtureName,
        RecordingThemeManagerBuilder theme,
        RecordingLocalizationBuilder localization)
    {
        var packages = theme.Packages.OrderBy(static package => package.Id, StringComparer.Ordinal)
                            .ToArray();
        var desktopPackage = packages.SingleOrDefault(static package => package.Id == DesktopPackageId);
        var catalogIds = localization.Catalogs.Select(static catalog => catalog.CatalogId)
                                     .OrderBy(static id => id, StringComparer.Ordinal)
                                     .ToArray();
        var translationBundleIds = localization.TranslationBundles
                                               .Select(static bundle =>
                                                   $"{bundle.CatalogId}|{bundle.Language.Value}|" +
                                                   $"{bundle.SourceKind}|{bundle.SourceIdentity}")
                                               .OrderBy(static id => id, StringComparer.Ordinal)
                                               .ToArray();
        var packageCoreFingerprint = CreatePackageCoreFingerprint(
            packages,
            catalogIds,
            translationBundleIds,
            theme.InitializerCount);

        using var writer = new Utf8JsonWriter(output, new JsonWriterOptions { Indented = false });
        writer.WriteStartObject();
        writer.WriteString("Fixture", fixtureName);
        writer.WriteString("PackageCoreFingerprint", packageCoreFingerprint);
        writer.WriteNumber("DesktopControlCount", desktopPackage?.Controls.Count ?? 0);
        writer.WriteNumber("DesktopThemeAssetCount", desktopPackage?.ThemeAssets.Count ?? 0);
        writer.WriteNumber(
            "DesktopProviderResourceCount",
            desktopPackage?.ControlThemesProvider.ControlThemes.Count ?? 0);
        writer.WriteNumber("InitializerCount", theme.InitializerCount);

        writer.WriteStartArray("Packages");
        foreach (var package in packages)
        {
            writer.WriteStartObject();
            writer.WriteString("Id", package.Id);
            writer.WriteString("ProviderId", package.ControlThemesProvider.Id);
            writer.WriteNumber("ProviderResourceCount", package.ControlThemesProvider.ControlThemes.Count);

            writer.WriteStartArray("ControlIdentities");
            foreach (var control in package.Controls)
            {
                writer.WriteStringValue(control.Identity.ToString());
            }
            writer.WriteEndArray();

            writer.WriteStartArray("ThemeAssetUris");
            foreach (var asset in package.ThemeAssets)
            {
                writer.WriteStringValue(asset.AssetUri.AbsoluteUri);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        writer.WriteStartArray("CatalogIds");
        foreach (var catalogId in catalogIds)
        {
            writer.WriteStringValue(catalogId);
        }
        writer.WriteEndArray();

        writer.WriteStartArray("TranslationBundleIds");
        foreach (var bundleId in translationBundleIds)
        {
            writer.WriteStringValue(bundleId);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
        writer.Flush();
        output.WriteByte((byte)'\n');
    }

    private static string CreatePackageCoreFingerprint(
        IReadOnlyList<ControlPackageRegistration> packages,
        IReadOnlyList<string> catalogIds,
        IReadOnlyList<string> translationBundleIds,
        int initializerCount)
    {
        var source = new StringBuilder();
        foreach (var package in packages)
        {
            source.Append("package=").Append(package.Id)
                  .Append("|provider=").Append(package.ControlThemesProvider.Id).Append('\n');
        }
        foreach (var catalogId in catalogIds)
        {
            source.Append("catalog=").Append(catalogId).Append('\n');
        }
        foreach (var bundleId in translationBundleIds)
        {
            source.Append("bundle=").Append(bundleId).Append('\n');
        }
        source.Append("initializers=").Append(initializerCount).Append('\n');

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source.ToString())));
    }

    private sealed class RecordingAtomUIBuilder(
        IThemeManagerBuilder theme,
        ILocalizationBuilder localization) : IAtomUIBuilder
    {
        public IThemeManagerBuilder Theme { get; } = theme;
        public ILocalizationBuilder Localization { get; } = localization;
    }

    private sealed class RecordingThemeManagerBuilder : IThemeManagerBuilder
    {
        internal List<ControlPackageRegistration> Packages { get; } = [];
        internal int InitializerCount { get; private set; }

        public void AddThemeDefinitionResolver(IThemeDefinitionResolver resolver)
        {
        }

        public void AddControlPackage(ControlPackageRegistration package)
        {
            Packages.Add(package);
        }

        public void AddInitializer(Action<IThemeManager> initializer)
        {
            ArgumentNullException.ThrowIfNull(initializer);
            InitializerCount++;
        }

        public void WithInitialTheme(string themeId, ThemeConfig? config = null)
        {
        }

        public void WithFollowSystemThemes(ThemeRequest light, ThemeRequest dark)
        {
        }

        public void WithApplicationId(string applicationId)
        {
        }

        public void UseUserThemeDirectory()
        {
        }

        public void UseUserThemeDirectory(string directory)
        {
        }

        public void WithDefaultFontFamily(FontFamily fontFamily)
        {
        }

        public void WithDefaultFontFamily(string fontFamily)
        {
        }
    }

    private sealed class RecordingLocalizationBuilder : ILocalizationBuilder
    {
        internal List<LanguageCatalogDescriptor> Catalogs { get; } = [];
        internal List<TranslationBundleDescriptor> TranslationBundles { get; } = [];

        public void AddCatalog(LanguageCatalogDescriptor descriptor)
        {
            Catalogs.Add(descriptor);
        }

        public void AddTranslationBundle(TranslationBundleDescriptor descriptor)
        {
            TranslationBundles.Add(descriptor);
        }

        public void AddLanguageDefinition(LanguageDefinition definition)
        {
        }

        public void ConfigureLanguages(
            LanguageTag defaultLanguage,
            IEnumerable<LanguageTag> supportedLanguages)
        {
        }
    }
}
