using AtomUI.Controls.Localization;
using AtomUI.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Localization;

public class CommonCatalogTests
{
    static CommonCatalogTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Common_Catalog_Uses_Stable_Ids_For_All_Existing_Keys()
    {
        var expected = new Dictionary<CommonLangResourceKind, int>
        {
            [CommonLangResourceKind.Ok]       = 1,
            [CommonLangResourceKind.Submit]   = 2,
            [CommonLangResourceKind.Cancel]   = 3,
            [CommonLangResourceKind.Reset]    = 4,
            [CommonLangResourceKind.Edit]     = 5,
            [CommonLangResourceKind.Delete]   = 6,
            [CommonLangResourceKind.Save]     = 7,
            [CommonLangResourceKind.NoData]   = 8,
            [CommonLangResourceKind.Loading]  = 9,
            [CommonLangResourceKind.Optional] = 10
        };

        Enum.GetValues<CommonLangResourceKind>().ShouldBe(expected.Keys, ignoreOrder: true);
        foreach (var (kind, id) in expected)
        {
            Convert.ToInt32(kind).ShouldBe(id);
        }
    }

    [Fact]
    public void Common_Markup_Extension_Uses_The_New_Localization_Resource_Base()
    {
        typeof(CommonLangResourceExtension).IsSealed.ShouldBeTrue();
        typeof(CommonLangResourceExtension).BaseType.ShouldBe(
            typeof(LanguageResourceExtension<CommonLangResourceKind>));
        typeof(CommonLangResourceExtension).GetConstructor(Type.EmptyTypes).ShouldNotBeNull();
        typeof(CommonLangResourceExtension).GetConstructor([typeof(CommonLangResourceKind)]).ShouldNotBeNull();
    }

    [Fact]
    public void UseDesktopControls_Registers_All_BuiltIn_Common_Translations()
    {
        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        var localizer = application.GetLocalizer().ShouldNotBeNull();
        var text = new TextBlock();
        var extension = new CommonLangResourceExtension(CommonLangResourceKind.Ok);
        var dynamicResource = extension.ProvideValue(
                new ProvideValueServiceProvider(text, TextBlock.TextProperty))
            .ShouldBeOfType<DynamicResourceExtension>();
        dynamicResource.ResourceKey.ShouldBe(CommonLangResourceKind.Ok);
        text.Bind(TextBlock.TextProperty, dynamicResource);
        var window = new AvaloniaWindow
        {
            Content = text
        };
        window.Show();

        try
        {
            text.Text.ShouldBe("Ok");
            localizer.Get(CommonLangResourceKind.Ok).ShouldBe("Ok");
            languageManager.ChangeLanguage(LanguageTags.ZhCN);
            text.Text.ShouldBe("确定");
            localizer.Get(CommonLangResourceKind.Loading).ShouldBe("正在加载数据");
            languageManager.ChangeLanguage(LanguageTags.ZhTW);
            text.Text.ShouldBe("確定");
            localizer.Get(CommonLangResourceKind.Optional).ShouldBe("(可選)");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Common_Catalog_No_Longer_Contains_Legacy_Language_Providers()
    {
        var controlsAssembly = typeof(CommonLangResourceKind).Assembly;

        controlsAssembly.GetType("AtomUI.Controls.Localization.en_US").ShouldBeNull();
        controlsAssembly.GetType("AtomUI.Controls.Localization.zh_CN").ShouldBeNull();
        controlsAssembly.GetType("AtomUI.Controls.Localization.zh_TW").ShouldBeNull();
    }

    private sealed class ProvideValueServiceProvider(
        object targetObject,
        object targetProperty) : IServiceProvider, IProvideValueTarget
    {
        public object TargetObject { get; } = targetObject;

        public object TargetProperty { get; } = targetProperty;

        public object? GetService(Type serviceType)
        {
            return serviceType == typeof(IProvideValueTarget) ? this : null;
        }
    }
}
