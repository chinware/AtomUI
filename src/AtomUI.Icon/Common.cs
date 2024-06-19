using System.Globalization;
using Avalonia.Media;
using Avalonia.Utilities;

namespace AtomUI.Icon;

public enum IconMode
{
   Normal,
   Disabled,
   Active,
   Selected,
}

public enum IconAnimation
{
   None,
   Spin,
   Pulse
}

public enum IconThemeType
{
   Filled,
   Outlined,
   TwoTone
}

public record struct ColorInfo
{
   private Color _normalColor;
   private Color? _activeColor;
   private Color? _selectedColor;
   private Color? _disabledColor;

   public Color NormalColor
   {
      get => _normalColor;
      set => _normalColor = value;
   }

   public Color ActiveColor
   {
      get => _activeColor ?? _normalColor;
      set => _activeColor = value;
   }

   public Color SelectedColor
   {
      get => _selectedColor ?? _normalColor;
      set => _selectedColor = value;
   }
   
   public Color DisabledColor
   {
      get => _disabledColor ?? _normalColor;
      set => _disabledColor = value;
   }

   public ColorInfo(Color color = default)
   {
      _normalColor = color;
   }

   public static ColorInfo Parse(string expr)
   {
      const string exceptionMessage = "Invalid ColorInfo.";
      using (var tokenizer = new StringTokenizer(expr, CultureInfo.InvariantCulture, exceptionMessage)) {
         try {
            var colorInfo = new ColorInfo();
            if (tokenizer.TryReadString(out var color1Str)) {
               colorInfo.NormalColor = Color.Parse(color1Str);
               if (tokenizer.TryReadString(out var color2Str)) {
                  colorInfo.ActiveColor = Color.Parse(color2Str);
                  if (tokenizer.TryReadString(out var color3Str)) {
                     colorInfo.SelectedColor = Color.Parse(color3Str);
                     if (tokenizer.TryReadString(out var color4Str)) {
                        colorInfo.DisabledColor = Color.Parse(color4Str);
                     }
                  }
               }
            } else {
               // 至少要一个
               throw new FormatException(exceptionMessage);
            }
            return colorInfo;
         } catch (Exception e) {
            if (e is not FormatException) {
               throw new FormatException(exceptionMessage, e);
            }
            
            throw;
         }
      }
   }
}

public record struct TwoToneColorInfo
{
   public Color PrimaryColor { get; set; }
   public Color SecondaryColor { get; set; }

   public static TwoToneColorInfo Parse(string expr)
   {
      const string exceptionMessage = "Invalid TwoToneColorInfo.";
      using (var tokenizer = new StringTokenizer(expr, CultureInfo.InvariantCulture, exceptionMessage)) {
         if (tokenizer.TryReadString(out var color1Str)) {
            if (tokenizer.TryReadString(out var color2Str)) {
               return new TwoToneColorInfo
               {
                  PrimaryColor = Color.Parse(color1Str),
                  SecondaryColor = Color.Parse(color2Str)
               };
            }
         }
         // 一定要两个，要不就不要定制
         throw new FormatException(exceptionMessage);
      }
   }
}