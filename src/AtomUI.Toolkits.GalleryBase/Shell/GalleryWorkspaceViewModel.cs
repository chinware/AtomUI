using System.Reactive;
using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Navigation;
using Avalonia;
using ReactiveUI;

namespace AtomUI.Toolkits.GalleryBase.Shell;

public class GalleryWorkspaceViewModel : ReactiveObject, IScreen, IDisposable
{
    private readonly IThemeManager? _themeManager;
    private readonly EventHandler<LanguageVariantChangedEventArgs>? _languageVariantChangedHandler;
    private bool _isDisposed;

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

        ToggleDarkModeCommand = ReactiveCommand.Create<bool>(isDark =>
            Application.Current?.SetDarkThemeMode(isDark));

        ToggleCompactModeCommand = ReactiveCommand.Create<bool>(isCompact =>
            Application.Current?.SetCompactThemeMode(isCompact));

        ToggleMotionCommand = ReactiveCommand.Create<bool>(enabled =>
            Application.Current?.SetMotionEnabled(enabled));

        ToggleWaveSpiritCommand = ReactiveCommand.Create<bool>(enabled =>
            Application.Current?.SetWaveSpiritEnabled(enabled));

        SwitchToZhCNCommand = ReactiveCommand.Create(() =>
            Application.Current?.SetLanguageVariant(LanguageVariant.zh_CN));

        SwitchToZhTWCommand = ReactiveCommand.Create(() =>
            Application.Current?.SetLanguageVariant(LanguageVariant.zh_TW));

        SwitchToEnUSCommand = ReactiveCommand.Create(() =>
            Application.Current?.SetLanguageVariant(LanguageVariant.en_US));

        _themeManager = Application.Current?.GetThemeManager();
        SyncLanguageState(_themeManager?.LanguageVariant);
        if (_themeManager is not null)
        {
            _languageVariantChangedHandler = HandleLanguageVariantChanged;
            _themeManager.LanguageVariantChanged += _languageVariantChangedHandler;
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
            _themeManager.LanguageVariantChanged -= _languageVariantChangedHandler;
        }

        Navigation.Dispose();
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
}
