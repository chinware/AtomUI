using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.DesignTokens;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeStartupTests
{
    [Fact]
    public void Control_Package_Registration_Adds_Descriptors_Assets_Themes_And_Languages_Atomically()
    {
        HeadlessTestApp.Run(() =>
        {
            var descriptor = ThemeCompilerTests.CreateCompilerButtonDescriptor();
            var provisional = new ControlThemeAssetDescriptor(
                new Uri("avares://Tests/Themes/Button.axaml"),
                descriptor.Identity,
                [descriptor.Identity],
                null,
                1);
            var registry = TypedThemeSnapshotCacheTests.CreateRegistry([descriptor]);
            var asset = new ControlThemeAssetDescriptor(
                provisional.AssetUri,
                provisional.OwnerIdentity,
                provisional.ReferencedControlIdentities,
                provisional.SemanticPart,
                ThemeSchemaRegistry.ComputeResourceKeySchemaFingerprint(
                    provisional,
                    registry.GlobalTokens));
            var provider = new TestControlThemesProvider("Tests.Controls");
            var package = new ControlPackageRegistration(
                provider.Id,
                [descriptor],
                [asset],
                provider,
                Array.Empty<AtomUI.Theme.Language.LanguageProvider>());
            var builder = new ThemeManagerBuilder();

            builder.AddControlPackage(package);
            var manager = builder.Build();
            manager.InitializeApplication(Application.Current!);

            manager.CurrentSnapshot!.Registry.Controls
                   .ShouldContain(control => control.Identity == descriptor.Identity);
            manager.Resources.MergedDictionaries.ShouldContain(provider.ControlThemes.Single());
            Should.Throw<ThemeResourceRegisterException>(() => builder.AddControlPackage(package));
        });
    }

    [Fact]
    public void InitializeApplication_Mounts_A_Fully_Populated_Manager_On_The_First_Frame()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            var builder = new ThemeManagerBuilder();
            builder.WithInitialTheme(IThemeManager.DEFAULT_THEME_ID);
            var manager = builder.Build();

            manager.InitializeApplication(application);

            manager.CurrentTheme.ShouldNotBeNull();
            manager.CurrentSnapshot.ShouldNotBeNull();
            manager.RootContext.Snapshot.ShouldBeSameAs(manager.CurrentSnapshot);
            manager.Resources.MergedDictionaries
                   .OfType<ThemeTokenResourceProvider>()
                   .ShouldHaveSingleItem()
                   .ShouldBeSameAs(manager.RootContext.ResourceProvider);
            application.Styles.ShouldContain(manager);
            application.RequestedThemeVariant.ShouldBe(ThemeVariant.Light);
            manager.AvailableThemes.ShouldContain(theme =>
                theme.Id == IThemeManager.DEFAULT_THEME_ID && theme.IsDefault);
            manager.OfType<Style>().ShouldContain(style =>
                style.Setters.OfType<Setter>().Any(setter =>
                    setter.Property == ThemeScope.ContextProperty &&
                    ReferenceEquals(setter.Value, manager.RootContext)));
        });
    }

    [Fact]
    public async Task Runtime_Request_Preserves_Startup_Config_As_The_Lower_Layer()
    {
        await HeadlessTestApp.RunAsync(async () =>
        {
            var application = Application.Current!;
            var builder = new ThemeManagerBuilder();
            builder.WithInitialTheme(
                IThemeManager.DEFAULT_THEME_ID,
                new ThemeConfigBuilder()
                    .WithToken(nameof(DesignToken.BorderRadius), "12")
                    .Build());
            var manager = builder.Build();
            manager.InitializeApplication(application);

            var result = await manager.ApplyThemeAsync(
                new ThemeRequest(
                    IThemeManager.DEFAULT_THEME_ID,
                    new ThemeConfigBuilder()
                        .WithToken(nameof(DesignToken.ColorPrimary), "#00b96b")
                        .Build(),
                    ThemeTransitionReason.UserRequest),
                TestContext.Current.CancellationToken);

            result.Status.ShouldBe(ThemeTransitionStatus.Committed);
            var snapshot = manager.CurrentSnapshot.ShouldNotBeNull();
            snapshot.Global<CornerRadius>(nameof(DesignToken.BorderRadius))
                    .ShouldBe(new CornerRadius(12));
            snapshot.Global<Color>(nameof(DesignToken.ColorPrimary))
                    .ShouldBe(Color.Parse("#00b96b"));
        });
    }

    [Fact]
    public void Root_ThemeConfigProvider_Resolves_Context_Before_TopLevel_Styles_Are_Applied()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            application.UseAtomUI();
            var manager = AvaloniaLocator.Current.GetService(typeof(ThemeManager))
                                         .ShouldBeOfType<ThemeManager>();
            var provider = new ThemeConfigProvider
            {
                Child = new Border()
            };
            var window = new Window
            {
                Content = provider
            };

            var providerContext = provider.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull();
            providerContext.Manager.ShouldBeSameAs(manager);
            providerContext.RegistrationId.ShouldBeGreaterThan(0);
            window.GetValue(ThemeScope.ContextProperty)
                  .ShouldBeSameAs(manager.RootContext);

            window.Content = null;
            window.Close();
        });
    }

    [Fact]
    public void Root_ThemeContext_Is_Resolved_For_Ordinary_TopLevel_Content()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            application.UseAtomUI();
            var manager = AvaloniaLocator.Current.GetService(typeof(ThemeManager))
                                         .ShouldBeOfType<ThemeManager>();
            var content = new Border();
            var window = new Window
            {
                Content = content
            };

            new ThemeTokenResolver().Capture(content)
                                    .ShouldBeSameAs(manager.RootContext.Snapshot);
            window.GetValue(ThemeScope.ContextProperty)
                  .ShouldBeSameAs(manager.RootContext);

            window.Content = null;
            window.Close();
        });
    }

    private sealed class TestControlThemesProvider : ControlThemesProvider
    {
        internal TestControlThemesProvider(string id)
        {
            Id = id;
            ControlThemes.Add(new ResourceDictionary());
        }
    }
}
