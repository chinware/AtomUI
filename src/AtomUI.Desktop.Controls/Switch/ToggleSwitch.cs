
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia.Interactivity;
namespace AtomUI.Desktop.Controls;

public class ToggleSwitch : AbstractToggleSwitch
{
    public ToggleSwitch()
    {
        this.RegisterTokenResourceScope(ToggleSwitchToken.ScopeProvider);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.Dispatcher.Post(this.EnableTransitions);
    }
}