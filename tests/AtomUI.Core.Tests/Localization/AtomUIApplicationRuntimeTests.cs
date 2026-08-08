using AtomUI.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Localization;

public class AtomUIApplicationRuntimeTests
{
    [Fact]
    public void Runtime_Mounts_One_Stable_Provider_And_Exposes_Exact_Service_Instances()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            using var runtime = CreateRuntime(application);

            runtime.InitializeApplication();

            application.Resources.MergedDictionaries
                       .Count(provider => ReferenceEquals(provider, runtime.ResourceProvider))
                       .ShouldBe(1);
            runtime.ResourceProvider.Owner.ShouldBeSameAs(application);
            AtomUIApplicationRuntimeStore.Get(application).ShouldBeSameAs(runtime);
            runtime.LanguageManager.ShouldBeSameAs(runtime.LocalizationHost.LanguageManager);
            runtime.Localizer.ShouldBeSameAs(runtime.LocalizationHost.Localizer);
        });
    }

    [Fact]
    public void Runtime_Projects_FlowDirection_Through_The_Same_Language_Provider()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            using var runtime = CreateRuntime(
                application,
                builder => builder.UseLanguages(
                    LanguageTags.ArSA,
                    [LanguageTags.ArSA, LanguageTags.EnUS]));
            runtime.InitializeApplication();
            var provider = runtime.ResourceProvider;
            var window = new Window
            {
                Content = new Border()
            };
            window.Show();

            window.FlowDirection.ShouldBe(FlowDirection.RightToLeft);
            runtime.LanguageManager.ChangeLanguage(LanguageTags.EnUS);

            runtime.ResourceProvider.ShouldBeSameAs(provider);
            window.FlowDirection.ShouldBe(FlowDirection.LeftToRight);
            window.Close();
        });
    }

    [Fact]
    public void Runtime_Rejects_Duplicate_Initialization_For_The_Same_Application()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            using var first = CreateRuntime(application);
            using var second = CreateRuntime(application);
            first.InitializeApplication();

            var exception = Should.Throw<InvalidOperationException>(second.InitializeApplication);

            exception.Message.ShouldContain("already initialized");
            application.Resources.MergedDictionaries
                       .Count(provider => ReferenceEquals(provider, second.ResourceProvider))
                       .ShouldBe(0);
        });
    }

    [Fact]
    public void Dispose_Unmounts_Application_Resources_Styles_And_Services()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            var runtime = CreateRuntime(application);
            runtime.InitializeApplication();
            var provider = runtime.ResourceProvider;
            var flowDirectionStyle = runtime.FlowDirectionStyle;

            runtime.Dispose();

            application.Resources.MergedDictionaries.Contains(provider).ShouldBeFalse();
            application.Styles.Contains(flowDirectionStyle).ShouldBeFalse();
            AtomUIApplicationRuntimeStore.Get(application).ShouldBeNull();
            Should.Throw<InvalidOperationException>(() =>
                runtime.LanguageManager.ChangeLanguage(LanguageTags.EnUS));
        });
    }

    private static AtomUIApplicationRuntime CreateRuntime(
        Application application,
        Action<IAtomUIBuilder>? configure = null)
    {
        var builder = new AtomUIBuilder(application);
        configure?.Invoke(builder);
        return new AtomUIApplicationRuntime(
            application,
            builder.ThemeManagerBuilder.Build(),
            builder.LocalizationBuilder.Build());
    }
}
