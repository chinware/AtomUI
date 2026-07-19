using System.Reactive;
using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Language;
using AtomUI.Theme.Styling;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Navigation;
using Avalonia;
using ReactiveUI;

namespace AtomUI.Toolkits.GalleryBase.Shell;

public class GalleryWorkspaceViewModel : ReactiveObject, IScreen, IDisposable
{
    private readonly IThemeManager? _themeManager;
    private readonly ILanguageManager? _languageManager;
    private readonly EventHandler<ThemeChangedEventArgs>? _themeChangedHandler;
    private readonly EventHandler<LanguageVariantChangedEventArgs>? _languageVariantChangedHandler;
    private bool _isDisposed;
    private bool _isDark;
    private bool _isCompact;
    private bool _isMotionEnabled = true;
    private bool _isWaveSpiritEnabled = true;
    private string[] _baseAlgorithms = ["Default"];

    private bool _isZhCN;
    private bool _isZhTW;
    private bool _isEnUS;

    public RoutingState Router { get; } = new();

    public GalleryNavigationViewModel Navigation { get; }

    public ReactiveCommand<bool, Unit> ToggleDarkModeCommand { get; }

    public ReactiveCommand<bool, Unit> ToggleCompactModeCommand { get; }

    public ReactiveCommand<bool, Unit> ToggleMotionCommand { get; }

    public ReactiveCommand<bool, Unit> ToggleWaveSpiritCommand { get; }

    public ReactiveCommand<Unit, Unit> SwitchToZhCNCommand { get; }

    public ReactiveCommand<Unit, Unit> SwitchToZhTWCommand { get; }

    public ReactiveCommand<Unit, Unit> SwitchToEnUSCommand { get; }

    public bool IsZhCN
    {
        get => _isZhCN;
        private set => this.RaiseAndSetIfChanged(ref _isZhCN, value);
    }

    public bool IsZhTW
    {
        get => _isZhTW;
        private set => this.RaiseAndSetIfChanged(ref _isZhTW, value);
    }

    public bool IsEnUS
    {
        get => _isEnUS;
        private set => this.RaiseAndSetIfChanged(ref _isEnUS, value);
    }

    public GalleryWorkspaceViewModel(GalleryBaseConfiguration configuration,
                                     Func<IScreen, GalleryNavigationViewModel>? navigationFactory = null)
    {
        Navigation = navigationFactory?.Invoke(this) ?? new GalleryNavigationViewModel(this, configuration);

        _themeManager = Application.Current?.GetThemeManager();
        _languageManager = Application.Current?.GetLanguageManager();
        SyncThemeState(_themeManager?.CurrentTheme, null);
        SyncLanguageState(_languageManager?.LanguageVariant);

        ToggleDarkModeCommand = ReactiveCommand.CreateFromTask<bool>(SetDarkModeAsync);
        ToggleCompactModeCommand = ReactiveCommand.CreateFromTask<bool>(SetCompactModeAsync);
        ToggleMotionCommand = ReactiveCommand.CreateFromTask<bool>(SetMotionEnabledAsync);
        ToggleWaveSpiritCommand = ReactiveCommand.CreateFromTask<bool>(SetWaveSpiritEnabledAsync);

        SwitchToZhCNCommand = ReactiveCommand.Create(() => SetLanguageVariant(LanguageVariant.zh_CN));
        SwitchToZhTWCommand = ReactiveCommand.Create(() => SetLanguageVariant(LanguageVariant.zh_TW));
        SwitchToEnUSCommand = ReactiveCommand.Create(() => SetLanguageVariant(LanguageVariant.en_US));

        if (_themeManager is not null)
        {
            _themeChangedHandler = HandleThemeChanged;
            _themeManager.ThemeChanged += _themeChangedHandler;
        }
        if (_languageManager is not null)
        {
            _languageVariantChangedHandler = HandleLanguageVariantChanged;
            _languageManager.LanguageVariantChanged += _languageVariantChangedHandler;
        }
    }

    public virtual void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        if (_themeManager is not null && _languageVariantChangedHandler is not null)
        {
            _languageManager!.LanguageVariantChanged -= _languageVariantChangedHandler;
        }
        if (_themeManager is not null && _themeChangedHandler is not null)
        {
            _themeManager.ThemeChanged -= _themeChangedHandler;
        }

        Navigation.Dispose();
    }

    private async Task SetDarkModeAsync(bool isDark)
    {
        _isDark = isDark;
        await ApplyThemeSettingsAsync();
    }

    private async Task SetCompactModeAsync(bool isCompact)
    {
        _isCompact = isCompact;
        await ApplyThemeSettingsAsync();
    }

    private async Task SetMotionEnabledAsync(bool enabled)
    {
        _isMotionEnabled = enabled;
        if (!enabled)
        {
            _isWaveSpiritEnabled = false;
        }
        await ApplyThemeSettingsAsync();
    }

    private async Task SetWaveSpiritEnabledAsync(bool enabled)
    {
        _isWaveSpiritEnabled = enabled && _isMotionEnabled;
        await ApplyThemeSettingsAsync();
    }

    private async Task ApplyThemeSettingsAsync()
    {
        if (_themeManager is null)
        {
            return;
        }

        var algorithms = new List<string>(_baseAlgorithms.Length + 2);
        algorithms.AddRange(_baseAlgorithms);
        if (_isCompact)
        {
            algorithms.Add("Compact");
        }
        if (_isDark)
        {
            algorithms.Add("Dark");
        }

        var config = new ThemeConfigBuilder()
                     .WithAlgorithms(algorithms.ToArray())
                     .WithToken(nameof(SharedTokenKind.EnableMotion), _isMotionEnabled ? "true" : "false")
                     .WithToken(nameof(SharedTokenKind.EnableWaveSpirit), _isWaveSpiritEnabled ? "true" : "false")
                     .Build();
        var result = await _themeManager.ApplyThemeAsync(
            new ThemeRequest(
                _themeManager.CurrentTheme?.ThemeId ?? IThemeManager.DEFAULT_THEME_ID,
                config,
                ThemeTransitionReason.UserRequest));
        if (result.Status == ThemeTransitionStatus.Failed)
        {
            var message = string.Join(" ", result.Diagnostics.Select(static diagnostic => diagnostic.Message));
            throw new ThemeLoadException(message, result.Exception);
        }
    }

    private void HandleThemeChanged(object? sender, ThemeChangedEventArgs args)
    {
        if (!_isDisposed)
        {
            SyncThemeState(args.State, args.Request.Config);
        }
    }

    private void HandleLanguageVariantChanged(object? sender, LanguageVariantChangedEventArgs args)
    {
        if (_isDisposed)
        {
            return;
        }

        SyncLanguageState(args.NewLanguage);
    }

    private void SyncLanguageState(LanguageVariant? variant)
    {
        IsZhCN = variant == LanguageVariant.zh_CN;
        IsZhTW = variant == LanguageVariant.zh_TW;
        IsEnUS = variant == LanguageVariant.en_US;
    }

    private void SyncThemeState(ThemeState? state, ThemeConfig? config)
    {
        if (state is not null)
        {
            _isDark = state.Appearance == ThemeAppearance.Dark;
            _isCompact = state.Algorithms.Contains("Compact", StringComparer.Ordinal);
            _baseAlgorithms = state.Algorithms
                                   .Where(static algorithm =>
                                       !string.Equals(algorithm, "Compact", StringComparison.Ordinal) &&
                                       !string.Equals(algorithm, "Dark", StringComparison.Ordinal))
                                   .ToArray();
            if (_baseAlgorithms.Length == 0)
            {
                _baseAlgorithms = ["Default"];
            }
        }

        _isMotionEnabled = ReadBooleanToken(config, nameof(SharedTokenKind.EnableMotion), true);
        _isWaveSpiritEnabled = _isMotionEnabled &&
                               ReadBooleanToken(config, nameof(SharedTokenKind.EnableWaveSpirit), true);
    }

    private static bool ReadBooleanToken(ThemeConfig? config, string name, bool defaultValue)
    {
        return config is not null &&
               config.Tokens.TryGetValue(name, out var value) &&
               bool.TryParse(value, out var parsed)
            ? parsed
            : defaultValue;
    }

    private void SetLanguageVariant(LanguageVariant variant)
    {
        if (_languageManager is not null)
        {
            _languageManager.LanguageVariant = variant;
        }
    }
}
