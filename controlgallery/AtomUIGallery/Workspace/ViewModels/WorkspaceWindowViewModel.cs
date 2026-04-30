using System.Reactive;
using AtomUI.Controls;
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
    }
}
