using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class DashedButtonTheme : ControlTheme
{
    public const string ID = "DashedButton";
    public static IList<double> DashedStyle = [4, 2];
}