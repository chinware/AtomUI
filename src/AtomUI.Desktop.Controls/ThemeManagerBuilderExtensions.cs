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
        foreach (var controlTokenRegistration in controlTokenTypes)
        {
            themeManagerBuilder.AddControlToken(controlTokenRegistration.TokenType);
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
        var inputManager = AvaloniaLocator.CurrentMutable.GetService<IInputManager>();
        if (inputManager is not null)
        {
            AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService(inputManager));
        }

        if (!RuntimePlatform.Features.SupportsNativeWindow)
        {
            return;
        }

        if (sender is ThemeManager themeManager)
        {
            MediaBreakPointThemeBootstrapper.Attach(themeManager);
        }
    }

    private static IList<ControlTokenRegistration> GetBrowserControlTokenTypes()
    {
        return new List<ControlTokenRegistration>
        {
            new ControlTokenRegistration(typeof(AddOnDecoratedBoxToken)),
            new ControlTokenRegistration(typeof(AlertToken)),
            new ControlTokenRegistration(typeof(ArrowDecoratedBoxToken)),
            new ControlTokenRegistration(typeof(AutoCompleteToken)),
            new ControlTokenRegistration(typeof(AvatarToken)),
            new ControlTokenRegistration(typeof(BadgeToken)),
            new ControlTokenRegistration(typeof(BreadcrumbToken)),
            new ControlTokenRegistration(typeof(ButtonSpinnerToken)),
            new ControlTokenRegistration(typeof(ButtonToken)),
            new ControlTokenRegistration(typeof(CalendarToken)),
            new ControlTokenRegistration(typeof(CardToken)),
            new ControlTokenRegistration(typeof(CascaderToken)),
            new ControlTokenRegistration(typeof(CheckBoxToken)),
            new ControlTokenRegistration(typeof(CarouselToken)),
            new ControlTokenRegistration(typeof(CollapseToken)),
            new ControlTokenRegistration(typeof(ComboBoxToken)),
            new ControlTokenRegistration(typeof(DatePickerToken)),
            new ControlTokenRegistration(typeof(DescriptionsToken)),
            new ControlTokenRegistration(typeof(DialogToken)),
            new ControlTokenRegistration(typeof(DrawerToken)),
            new ControlTokenRegistration(typeof(EmptyToken)),
            new ControlTokenRegistration(typeof(ExpanderToken)),
            new ControlTokenRegistration(typeof(FlyoutHostToken)),
            new ControlTokenRegistration(typeof(FloatButtonToken)),
            new ControlTokenRegistration(typeof(FormToken)),
            new ControlTokenRegistration(typeof(GroupBoxToken)),
            new ControlTokenRegistration(typeof(ImagePreviewerToken)),
            new ControlTokenRegistration(typeof(InfoPickerInputToken)),
            new ControlTokenRegistration(typeof(LineEditToken)),
            new ControlTokenRegistration(typeof(ListBoxToken)),
            new ControlTokenRegistration(typeof(ListViewToken)),
            new ControlTokenRegistration(typeof(MarqueeLabelToken)),
            new ControlTokenRegistration(typeof(MentionsToken)),
            new ControlTokenRegistration(typeof(MenuToken)),
            new ControlTokenRegistration(typeof(MessageToken)),
            new ControlTokenRegistration(typeof(MessageBoxToken)),
            new ControlTokenRegistration(typeof(NavMenuToken)),
            new ControlTokenRegistration(typeof(NotificationToken)),
            new ControlTokenRegistration(typeof(NumericUpDownToken)),
            new ControlTokenRegistration(typeof(OptionButtonToken)),
            new ControlTokenRegistration(typeof(PaginationToken)),
            new ControlTokenRegistration(typeof(PopupConfirmToken)),
            new ControlTokenRegistration(typeof(PopupHostToken)),
            new ControlTokenRegistration(typeof(ProgressBarToken)),
            new ControlTokenRegistration(typeof(QRCodeToken)),
            new ControlTokenRegistration(typeof(RadioButtonToken)),
            new ControlTokenRegistration(typeof(RateToken)),
            new ControlTokenRegistration(typeof(ResultToken)),
            new ControlTokenRegistration(typeof(ScrollViewerToken)),
            new ControlTokenRegistration(typeof(SegmentedToken)),
            new ControlTokenRegistration(typeof(SelectToken)),
            new ControlTokenRegistration(typeof(SeparatorToken)),
            new ControlTokenRegistration(typeof(SkeletonToken)),
            new ControlTokenRegistration(typeof(SliderToken)),
            new ControlTokenRegistration(typeof(SpaceToken)),
            new ControlTokenRegistration(typeof(SplitterToken)),
            new ControlTokenRegistration(typeof(SpinToken)),
            new ControlTokenRegistration(typeof(StepsToken)),
            new ControlTokenRegistration(typeof(StatisticToken)),
            new ControlTokenRegistration(typeof(TabControlToken)),
            new ControlTokenRegistration(typeof(TagToken)),
            new ControlTokenRegistration(typeof(TimelineToken)),
            new ControlTokenRegistration(typeof(TextAreaToken)),
            new ControlTokenRegistration(typeof(TimePickerToken)),
            new ControlTokenRegistration(typeof(ToggleSwitchToken)),
            new ControlTokenRegistration(typeof(ToolTipToken)),
            new ControlTokenRegistration(typeof(TourToken)),
            new ControlTokenRegistration(typeof(TransferToken)),
            new ControlTokenRegistration(typeof(TreeSelectToken)),
            new ControlTokenRegistration(typeof(TreeViewToken)),
            new ControlTokenRegistration(typeof(UploadToken))
        };
    }
}
