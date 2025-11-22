using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class AboutUsPage : ReactiveUserControl<AboutUsViewModel>
{
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
                disposables.Add(BindUtils.RelayBind(themeManager.BindingSource, IThemeManager.IsDarkThemeModeProperty, this, IsDarkThemeModeProperty));
            }
        });
        InitializeComponent();
    }
}