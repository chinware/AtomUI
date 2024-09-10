using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Media;

public static class TextUtils
{
    public static Size CalculateTextSize(string text,
                                         double fontSize,
                                         FontFamily fontFamily,
                                         FontStyle fontStyle = FontStyle.Normal,
                                         FontWeight fontWeight = FontWeight.Normal)
    {
        var       typeface   = new Typeface(fontFamily, fontStyle, fontWeight);
        using var textLayout = new TextLayout(text, typeface, fontSize, null);
        return new Size(textLayout.Width, textLayout.Height);
    }
}