using AtomUI.Theme;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeStartupTests
{
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
        });
    }
}
