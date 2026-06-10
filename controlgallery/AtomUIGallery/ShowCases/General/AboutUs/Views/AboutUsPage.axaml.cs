using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using Avalonia;

namespace AtomUIGallery.ShowCases.AboutUs;

public partial class AboutUsPage : GalleryReactiveUserControl<AboutUsViewModel>
{
    public const string LanguageId = nameof(AboutUsPage);

    public static readonly StyledProperty<bool> IsDarkThemeModeProperty =
        IThemeManager.IsDarkThemeModeProperty.AddOwner<AboutUsPage>();

    public bool IsDarkThemeMode
    {
        get => GetValue(IsDarkThemeModeProperty);
        set => SetValue(IsDarkThemeModeProperty, value);
    }

    public AboutUsPage()
    {
        this.WhenActivated(disposables =>
        {
            var themeManager = Application.Current?.GetThemeManager();
            if (themeManager != null)
            {
                disposables.Add(BindUtils.RelayBind(themeManager.BindingSource, IThemeManager.IsDarkThemeModeProperty,
                    this, IsDarkThemeModeProperty));
            }
        });
        InitializeComponent();
    }
}
