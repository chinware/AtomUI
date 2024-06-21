using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Media;

public static class TextUtils
{
   public static Size CalculateTextSize(string text, FontFamily fontFamily, double fontSize)
   {
      var typeface = new Typeface(fontFamily);
      using var textLayout = new TextLayout(text, typeface, fontSize, null);
      return new Size(textLayout.Width, textLayout.Height);
   }
}