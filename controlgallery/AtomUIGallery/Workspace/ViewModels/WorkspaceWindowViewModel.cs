using System.Reactive;
using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;
using ReactiveUI;

namespace AtomUIGallery.Workspace.ViewModels;

public class WorkspaceWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();

    public CaseNavigationViewModel CaseNavigation { get; }

    public ReactiveCommand<bool, Unit> ToggleDarkModeCommand    { get; }
    public ReactiveCommand<bool, Unit> ToggleCompactModeCommand { get; }
    public ReactiveCommand<bool, Unit> ToggleMotionCommand      { get; }
    public ReactiveCommand<bool, Unit> ToggleWaveSpiritCommand  { get; }
    public ReactiveCommand<Unit, Unit> SwitchToZhCNCommand      { get; }
    public ReactiveCommand<Unit, Unit> SwitchToEnUSCommand      { get; }

    private bool _isZhCN;
    public bool IsZhCN
    {
        get => _isZhCN;
        private set => this.RaiseAndSetIfChanged(ref _isZhCN, value);
    }

    private bool _isEnUS;
    public bool IsEnUS
    {
        get => _isEnUS;
        private set => this.RaiseAndSetIfChanged(ref _isEnUS, value);
    }

    public WorkspaceWindowViewModel()
    {
        CaseNavigation = new CaseNavigationViewModel(this);

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

        SwitchToEnUSCommand = ReactiveCommand.Create(() =>
            Application.Current?.SetLanguageVariant(LanguageVariant.en_US));

        var themeManager = Application.Current?.GetThemeManager();
        SyncLanguageState(themeManager?.LanguageVariant);
        if (themeManager != null)
        {
            themeManager.LanguageVariantChanged += (_, args) => SyncLanguageState(args.NewLanguage);
        }
    }

    private void SyncLanguageState(LanguageVariant? variant)
    {
        IsZhCN = variant == LanguageVariant.zh_CN;
        IsEnUS = variant == LanguageVariant.en_US;
    }
}
