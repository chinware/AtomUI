using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using Avalonia;

namespace AtomUIGallery.ShowCases.Community;

public partial class CommunityPage : GalleryReactiveUserControl<CommunityViewModel>
{
    public const string LanguageId = nameof(CommunityPage);

    public static readonly StyledProperty<bool> IsDarkThemeModeProperty =
        IThemeManager.IsDarkThemeModeProperty.AddOwner<CommunityPage>();

    public bool IsDarkThemeMode
    {
        get => GetValue(IsDarkThemeModeProperty);
        set => SetValue(IsDarkThemeModeProperty, value);
    }

    public CommunityPage()
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
