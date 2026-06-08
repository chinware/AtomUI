using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Primitives;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.Media.Transformation;

namespace AtomUI.Desktop.Controls;

public static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseDesktopControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        themeManagerBuilder.UseCommonControls();
        var controlTokenTypes = RuntimePlatform.Features.SupportsNativeWindow
            ? ControlTokenTypePool.GetTokenTypes()
            : GetBrowserControlTokenTypes();
        foreach (var controlType in controlTokenTypes)
        {
            themeManagerBuilder.AddControlToken(controlType);
        }
        themeManagerBuilder.AddControlThemesProvider(RuntimePlatform.Features.SupportsNativeWindow
            ? new DesktopControlThemesProvider()
            : new BrowserDesktopControlThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }

        themeManagerBuilder.InitializedHandlers.Add(HandleThemeManagerInitialized);

        return themeManagerBuilder;
    }

    private static void HandleThemeManagerInitialized(object? sender, EventArgs e)
    {
        Animation.RegisterCustomAnimator<TransformOperations, MotionTransformOptionsAnimator>();
        if (!RuntimePlatform.Features.SupportsNativeWindow)
        {
            return;
        }

        var inputManager = AvaloniaLocator.CurrentMutable.GetService<IInputManager>();
        Debug.Assert(inputManager != null);
        AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService(inputManager));

        if (sender is ThemeManager themeManager)
        {
            MediaBreakPointThemeBootstrapper.Attach(themeManager);
        }
    }

    private static IList<Type> GetBrowserControlTokenTypes()
    {
        return new List<Type>
        {
            typeof(AddOnDecoratedBoxToken),
            typeof(ArrowDecoratedBoxToken),
            typeof(AutoCompleteToken),
            typeof(BadgeToken),
            typeof(BreadcrumbToken),
            typeof(ButtonSpinnerToken),
            typeof(ButtonToken),
            typeof(CalendarToken),
            typeof(CardToken),
            typeof(CascaderToken),
            typeof(CheckBoxToken),
            typeof(ComboBoxToken),
            typeof(DatePickerToken),
            typeof(EmptyToken),
            typeof(FlyoutHostToken),
            typeof(FloatButtonToken),
            typeof(GroupBoxToken),
            typeof(InfoPickerInputToken),
            typeof(LineEditToken),
            typeof(ListBoxToken),
            typeof(ListViewToken),
            typeof(MenuToken),
            typeof(MessageToken),
            typeof(NavMenuToken),
            typeof(NumericUpDownToken),
            typeof(OptionButtonToken),
            typeof(PaginationToken),
            typeof(PopupConfirmToken),
            typeof(PopupHostToken),
            typeof(RadioButtonToken),
            typeof(ScrollViewerToken),
            typeof(SegmentedToken),
            typeof(SelectToken),
            typeof(SeparatorToken),
            typeof(SkeletonToken),
            typeof(SliderToken),
            typeof(SpaceToken),
            typeof(SplitterToken),
            typeof(SpinToken),
            typeof(StepsToken),
            typeof(TabControlToken),
            typeof(TagToken),
            typeof(TimePickerToken),
            typeof(ToggleSwitchToken),
            typeof(ToolTipToken),
            typeof(TreeSelectToken),
            typeof(TreeViewToken)
        };
    }
}
