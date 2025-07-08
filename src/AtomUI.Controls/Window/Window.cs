namespace AtomUI.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public class Window : AvaloniaWindow
{
    protected override Type StyleKeyOverride { get; } = typeof(Window);
}