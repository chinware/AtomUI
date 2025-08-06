using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Styling;

namespace AtomUI;

using AvaloniaApplication = Avalonia.Application;

public class Application : AvaloniaApplication, IApplication
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Application>();
    
    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Application>();
    
    public static readonly StyledProperty<bool> IsDarkThemeModeProperty =
        AvaloniaProperty.Register<Application, bool>(nameof(IsDarkThemeMode));
    
    public static readonly StyledProperty<bool> IsCompactThemeModeProperty =
        AvaloniaProperty.Register<Application, bool>(nameof(IsCompactThemeMode));

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }
    
    public bool IsDarkThemeMode
    {
        get => GetValue(IsDarkThemeModeProperty);
        set => SetValue(IsDarkThemeModeProperty, value);
    }
    
    public bool IsCompactThemeMode
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
        if (_themeManager != null)
        {
            _themeManager.SetActiveTheme(ActualThemeVariant);
            var algorithms = _themeManager.ActivatedThemeAlgorithms;
            if (algorithms != null)
            {
                IsDarkThemeMode = algorithms.Contains(ThemeAlgorithm.Dark);
                IsCompactThemeMode = algorithms.Contains(ThemeAlgorithm.Compact);
                if (_themeManager.TryGetResource(SharedTokenKey.EnableMotion, ActualThemeVariant, out var enableMotionResource))
                {
                    if (enableMotionResource is bool enableMotion)
                    {
                        IsMotionEnabled = enableMotion;
                    }
                }
                
                if (_themeManager.TryGetResource(SharedTokenKey.EnableWaveSpirit, ActualThemeVariant, out var enableWaveSpiritResource))
                {
                    if (enableWaveSpiritResource is bool enableWaveSpirit)
                    {
                        IsWaveSpiritEnabled = enableWaveSpirit;
                    }
                }
            }
        }
    }
}