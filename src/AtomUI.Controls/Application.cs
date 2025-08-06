using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
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
        if (_themeManagerAttached)
        {
            if (change.Property == ActualThemeVariantProperty)
            {
                ConfigureThemeVariant();
            }
            else if (change.Property == IsDarkThemeModeProperty ||
                     change.Property == IsCompactThemeModeProperty)
            {
                ConfigureActiveThemeAlgorithms();
            }
            else if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureEnableMotion();
            }
            else if (change.Property == IsWaveSpiritEnabledProperty)
            {
                ConfigureEnableWaveSpirit();
            }
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

    private void ConfigureActiveThemeAlgorithms()
    {
        if (_themeManager == null)
        {
            return;
        }
        var newAlgorithms = new List<ThemeAlgorithm>()
        {
            ThemeAlgorithm.Default
        };
        if (IsDarkThemeMode)
        {
            newAlgorithms.Add(ThemeAlgorithm.Dark);
        }

        if (IsCompactThemeMode)
        {
            newAlgorithms.Add(ThemeAlgorithm.Compact);
        }

        if (_themeManager.ActivatedTheme != null)
        {
            RequestedThemeVariant = Theme.Theme.BuildThemeVariant(_themeManager.ActivatedTheme.Id, newAlgorithms);
        }
    }

    private void ConfigureEnableMotion()
    {
        if (_themeManager == null)
        {
            return;
        }
        var themeResource = _themeManager.Resources[ActualThemeVariant];
        if (themeResource is ResourceDictionary globalResourceDictionary)
        {
            globalResourceDictionary[SharedTokenKey.EnableMotion] = IsMotionEnabled;
        }
    }
    
    private void ConfigureEnableWaveSpirit()
    {
        if (_themeManager == null)
        {
            return;
        }
        if (_themeManager == null)
        {
            return;
        }
        var themeResource = _themeManager.Resources[ActualThemeVariant];
        if (themeResource is ResourceDictionary globalResourceDictionary)
        {
            globalResourceDictionary[SharedTokenKey.EnableWaveSpirit] = IsWaveSpiritEnabled;
        }
    }
}