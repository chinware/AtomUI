using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Styling;

namespace AtomUI;

using AvaloniaApplication = Avalonia.Application;

public class Application : AvaloniaApplication, IApplication
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Application>();
    
    public static readonly StyledProperty<bool> IsDarkThemeModeProperty =
        AvaloniaProperty.Register<Application, bool>(nameof(IsDarkThemeMode));
    
    public static readonly StyledProperty<bool> IsCompactThemeModeProperty =
        AvaloniaProperty.Register<Application, bool>(nameof(IsCompactTheme));

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsDarkThemeMode
    {
        get => GetValue(IsDarkThemeModeProperty);
        set => SetValue(IsDarkThemeModeProperty, value);
    }
    
    public bool IsCompactTheme
    {
        get => GetValue(IsCompactThemeModeProperty);
        set => SetValue(IsCompactThemeModeProperty, value);
    }
    
    #endregion
    
    private ThemeManager? _themeManager;
    private bool _themeManagerAttached;

    internal void AttachThemeManager(ThemeManager themeManager)
    {
        _themeManager         = themeManager;
        NotifyThemeManagerAttached(themeManager);
        _themeManagerAttached = true;
        RequestedThemeVariant = new ThemeVariant(_themeManager.DefaultThemeId, null);
        Styles.Add(themeManager);
    }
    
    protected virtual void NotifyThemeManagerAttached(IThemeManager themeManager)
    {}

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ActualThemeVariantProperty && _themeManagerAttached)
        {
            ConfigureThemeVariant();
        }
    }

    private void ConfigureThemeVariant()
    {
        _themeManager?.SetActiveTheme(ActualThemeVariant);
    }
}