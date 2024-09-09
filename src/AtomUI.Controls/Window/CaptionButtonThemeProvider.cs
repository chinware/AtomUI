using Avalonia.Styling;

namespace AtomUI.Controls;

public class CaptionButtonThemeProvider
{
    public object Key { get; } = typeof(CaptionButton);
    public Type TargetType { get; } = typeof(Button);

    public ControlTheme BuildControlTheme()
    {
        return default!;
    }
}