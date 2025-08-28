using AtomUI.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class AbstractColorSliderTheme : ControlTheme
{
    public static HsvColorToBrushConverter HsvColorToBrushConverter = new HsvColorToBrushConverter();
}