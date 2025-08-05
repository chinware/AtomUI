using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;

namespace AtomUI;

using AvaloniaApplication = Avalonia.Application;

public class Application : AvaloniaApplication, IApplication
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Application>();
    
    public static readonly StyledProperty<string> ActualThemeProperty = 
        AvaloniaProperty.Register<Application, string>(nameof(ActualTheme), inherits: true);
    
    internal static readonly StyledProperty<string?> RequestedThemeProperty = 
        AvaloniaProperty.Register<Application, string?>(nameof(RequestedTheme));

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public string ActualTheme => GetValue(ActualThemeProperty);
    
    public string? RequestedTheme
    {
        get => GetValue(RequestedThemeProperty);
        set => SetValue(RequestedThemeProperty, value);
    }
    
    #endregion
    
    private ThemeManager? _themeManager;

    internal void AttachThemeManager(ThemeManager themeManager)
    {
        _themeManager = themeManager;
        Styles.Add(themeManager);
        NotifyThemeManagerAttached(themeManager);
    }
    
    protected virtual void NotifyThemeManagerAttached(ThemeManager themeManager)
    {}

    public void SetActiveTheme(string themeId)
    {
        
    }

    public void SetActiveLanguage(string languageCode)
    {
        
    }
}