using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public class Window : AvaloniaWindow
{
    protected override Type StyleKeyOverride { get; } = typeof(Window);

    private WindowState? _originState;
    private WindowResizer? _windowResizer;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty)
        {
            if (change.NewValue is WindowState newState)
            {
                if (newState == WindowState.FullScreen)
                {
                    _originState = change.GetOldValue<WindowState>();
                }
                else
                {
                    WindowState = _originState.HasValue ? _originState.Value : newState;;
                }
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _windowResizer = e.NameScope.Find<WindowResizer>(WindowThemeConstants.WindowResizerPart);
        if (_windowResizer != null)
        {
            _windowResizer.TargetWindow = this;
        }
    }
}