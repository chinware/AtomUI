using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Media;

public static class TextUtils
{
   public static Size CalculateTextSize(string text, FontFamily fontFamily, double fontSize)
   {
      var typeface = new Typeface(fontFamily);
      var textRunStyle = new GenericTextRunProperties(typeface, null, fontSize, null, null);
      var textProperties = new GenericTextParagraphProperties(FlowDirection.LeftToRight, TextAlignment.Left, true,
                                                              false,
                                                              textRunStyle, TextWrapping.NoWrap, double.NaN, 0, 0);
      using var textLayout = new TextLayout(text, typeface, fontSize, null);
      return new Size(textLayout.Width, textLayout.Height);
   }
}