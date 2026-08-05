using AtomUI.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Localization;

public class ApplicationLocalizationStartupTests
{
    [Fact]
    public void UseAtomUI_Publishes_Default_Application_Runtime_Services()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;

            application.GetLanguageManager().ShouldBeNull();
            application.GetLocalizer().ShouldBeNull();
            application.UseAtomUI();

            var runtime = AtomUIApplicationRuntimeStore.Get(application).ShouldNotBeNull();
            application.GetLanguageManager().ShouldBeSameAs(runtime.LanguageManager);
            application.GetLocalizer().ShouldBeSameAs(runtime.Localizer);
            application.GetThemeManager().ShouldBeSameAs(runtime.ThemeManager);
            runtime.LanguageManager.Current.CurrentLanguage.ShouldBe(LanguageTags.EnUS);
            runtime.LanguageManager.Current.Revision.ShouldBe(0);
        });
    }

    [Fact]
    public void UseAtomUI_Uses_Strongly_Typed_Language_Configuration_Before_First_Window()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;

            application.UseAtomUI(builder => builder.UseLanguages(
                LanguageTags.ArSA,
                [LanguageTags.ArSA, LanguageTags.EnUS]));
            var window = new Window();
            window.Show();

            application.GetLanguageManager()!.Current.CurrentLanguage.ShouldBe(LanguageTags.ArSA);
            window.FlowDirection.ShouldBe(FlowDirection.RightToLeft);
            window.Close();
        });
    }

    [Fact]
    public void UseAtomUI_Invokes_Generated_Bootstrap_Before_User_Configuration()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = (TestApplication)Application.Current!;
            var order = new List<string>();
            TestApplication.RegisterApplicationLanguagesCallback = builder =>
            {
                order.Add("bootstrap");
                RegisterCatalog(builder);
            };
            try
            {
                application.UseAtomUI(builder =>
                {
                    order.Add("configure");
                    builder.UseLanguages(
                        LanguageTags.EnUS,
                        [LanguageTags.EnUS, LanguageTags.ZhCN]);
                });
            }
            finally
            {
                TestApplication.RegisterApplicationLanguagesCallback = null;
            }

            order.ShouldBe(["bootstrap", "configure"]);
            application.GetLocalizer()!.Get(StartupResourceKind.Value).ShouldBe("English");
        });
    }

    [Fact]
    public void Application_Dynamic_Enum_Resources_Refresh_After_Committed_Change()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = (TestApplication)Application.Current!;
            TestApplication.RegisterApplicationLanguagesCallback = RegisterCatalog;
            try
            {
                application.UseAtomUI(builder => builder.UseLanguages(
                    LanguageTags.EnUS,
                    [LanguageTags.EnUS, LanguageTags.ZhCN]));
            }
            finally
            {
                TestApplication.RegisterApplicationLanguagesCallback = null;
            }

            var text = new TextBlock();
            text.Bind(
                TextBlock.TextProperty,
                new DynamicResourceExtension(StartupResourceKind.Value));
            var window = new Window
            {
                Content = text
            };
            window.Show();

            text.Text.ShouldBe("English");
            application.GetLanguageManager()!.ChangeLanguage(LanguageTags.ZhCN);
            text.Text.ShouldBe("中文");
            window.Close();
        });
    }

    [Fact]
    public void UseAtomUI_Rejects_Duplicate_Initialization()
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            application.UseAtomUI();

            var exception = Should.Throw<InvalidOperationException>(() => application.UseAtomUI());

            exception.Message.ShouldContain("already initialized");
        });
    }

    private static void RegisterCatalog(ILocalizationBuilder builder)
    {
        const string catalogId = "Acme:Acme.StartupResourceKind";
        builder.AddCatalog(new LanguageCatalogDescriptor<StartupResourceKind>(
            catalogId,
            1,
            [new LanguageCatalogUnitDescriptor(1, "Value")],
            static key => key == StartupResourceKind.Value ? 0 : -1));
        builder.AddTranslationBundle(new TranslationBundleDescriptor(
            catalogId,
            1,
            LanguageTags.EnUS,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme",
            ["English"]));
        builder.AddTranslationBundle(new TranslationBundleDescriptor(
            catalogId,
            1,
            LanguageTags.ZhCN,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme",
            ["中文"]));
    }

    private enum StartupResourceKind
    {
        Value = 1
    }
}
