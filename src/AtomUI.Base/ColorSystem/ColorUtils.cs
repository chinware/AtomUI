using AtomUI.Utils;
using Avalonia.Media;

namespace AtomUI.ColorSystem;

public enum WCAG2Level
{
   AA,
   AAA
}

public enum WCAG2Size
{
   Large,
   Small
}

public class WCAG2Parms
{
   public WCAG2Level Level { get; set; } = WCAG2Level.AA;
   public WCAG2Size Size { get; set; } = WCAG2Size.Small;
}

public class WCAG2FallbackParms : WCAG2Parms
{
   public bool IncludeFallbackColors { get; set; }
}

public static class ColorUtils
{
   public static Color FromRgbF(double alpha, double red, double green, double blue)
   {
      return Color.FromArgb((byte)Math.Round(alpha * 255d),
                            (byte)Math.Round(red * 255d),
                            (byte)Math.Round(green * 255d),
                            (byte)Math.Round(blue * 255d));
   }

   public static Color FromRgbF(double red, double green, double blue)
   {
      return FromRgbF(1d, red, green, blue);
   }

   public static Color TransparentColor()
   {
      return Color.FromArgb(0, 255, 255, 255);
   }

   public static Color Desaturate(string color, int amount = 10)
   {
      return Color.Parse(color).Desaturate(amount);
   }

   public static Color Saturate(string color, int amount = 10)
   {
      return Color.Parse(color).Saturate(amount);
   }

   public static Color Lighten(string color, int amount = 10)
   {
      return Color.Parse(color).Lighten(amount);
   }

   public static Color Brighten(string color, int amount = 10)
   {
      return Color.Parse(color).Brighten(amount);
   }

   public static Color Darken(string color, int amount = 10)
   {
      return Color.Parse(color).Darken(amount);
   }

   public static Color Spin(string color, int amount = 10)
   {
      return Color.Parse(color).Spin(amount);
   }

   public static Color OnBackground(in Color frontColor, in Color backgroundColor)
   {
      double fr = frontColor.GetRedF();
      double fg = frontColor.GetGreenF();
      double fb = frontColor.GetBlueF();
      double fa = frontColor.GetAlphaF();

      double br = backgroundColor.GetRedF();
      double bg = backgroundColor.GetGreenF();
      double bb = backgroundColor.GetBlueF();
      double ba = backgroundColor.GetAlphaF();

      double alpha = fa + ba * (1 - fa);

      double nr = (fr * fa + br * ba * (1 - fa)) / alpha;
      double ng = (fg * fa + bg * ba * (1 - fa)) / alpha;
      double nb = (fb * fa + bb * ba * (1 - fa)) / alpha;
      double na = alpha;

      return ColorUtils.FromRgbF(na, nr, ng, nb);
   }

   public static bool IsStableColor(int color)
   {
      return color >= 0 && color <= 255;
   }

   public static bool IsStableColor(float color)
   {
      return color >= 0.0f && color <= 1.0f;
   }

   public static bool IsStableColor(double color)
   {
      return color >= 0.0d && color <= 1.0d;
   }

   public static Color AlphaColor(in Color frontColor, in Color backgroundColor)
   {
      double fR = frontColor.GetRedF();
      double fG = frontColor.GetGreenF();
      double fB = frontColor.GetBlueF();
      double originAlpha = frontColor.GetAlphaF();
      if (originAlpha < 1d) {
         return frontColor;
      }

      double bR = backgroundColor.GetRedF();
      double bG = backgroundColor.GetGreenF();
      double bB = backgroundColor.GetBlueF();

      for (var fA = 0.01d; fA <= 1.0d; fA += 0.01d) {
         double r = Math.Round((fR - bR * (1d - fA)) / fA);
         double g = Math.Round((fG - bG * (1d - fA)) / fA);
         double b = Math.Round((fB - bB * (1d - fA)) / fA);
         if (IsStableColor(r) && IsStableColor(g) && IsStableColor(b)) {
            return ColorUtils.FromRgbF(Math.Round(fA * 100d) / 100d, r, g, b);
         }
      }

      // fallback
      /* istanbul ignore next */
      return ColorUtils.FromRgbF(1.0d, fR, fG, fB);
   }

   public static Color ParseCssRgbColor(string colorExpr)
   {
      if (TryParseCssRgbColor(colorExpr, out Color color)) {
         return color;
      }

      throw new FormatException($"Invalid color string: '{colorExpr.ToString()}'.");
   }

   public static bool TryParseCssRgbColor(string? colorExpr, out Color color)
   {
      color = default;
      if (string.IsNullOrEmpty(colorExpr)) {
         return false;
      }

      if (colorExpr[0] == '#') {
         return Color.TryParse(colorExpr, out color);
      }

      bool isRgba = colorExpr.StartsWith("rgba", StringComparison.InvariantCultureIgnoreCase);
      bool isRgb = false;
      if (!isRgba) {
         isRgb = colorExpr.StartsWith("rgb", StringComparison.InvariantCultureIgnoreCase);
      }

      if (isRgb || isRgba) {
         int leftParen = colorExpr.IndexOf('(');
         int rightParen = colorExpr.IndexOf(')');
         if (leftParen == -1 || rightParen == -1) {
            return false;
         }

         var parts = new List<string>(colorExpr.Substring(leftParen + 1, rightParen - leftParen - 1)
                                               .Split(',', StringSplitOptions.RemoveEmptyEntries));
         if (isRgb) {
            if (parts.Count != 3) {
               return false;
            }

            parts.Add("255");
         } else {
            if (parts.Count != 4) {
               return false;
            }
         }

         List<int> rgbaValues = new List<int>();
         foreach (var part in parts) {
            if (int.TryParse(part, out int partValue)) {
               rgbaValues.Add(partValue);
            } else {
               return false;
            }
         }

         color = Color.FromArgb((byte)rgbaValues[0], (byte)rgbaValues[1], (byte)rgbaValues[2], (byte)rgbaValues[3]);
         return true;
      }

      return false;
   }

   /// Readability Functions
   /// ---------------------
   /// <http://www.w3.org/TR/2008/REC-WCAG20-20081211/#contrast-ratiodef (WCAG Version 2)
   ///
   /// AKA `contrast`
   ///  Analyze the 2 colors and returns the color contrast defined by (WCAG Version 2)
   public static double Readability(Color color1, Color color2)
   {
      return Math.Max(color1.GetLuminance(), color2.GetLuminance() + 0.05) /
             Math.Min(color1.GetLuminance(), color2.GetLuminance() + 0.05);
   }

   ///
   /// Ensure that foreground and background color combinations meet WCAG2 guidelines.
   /// The third argument is an object.
   ///      the 'level' property states 'AA' or 'AAA' - if missing or invalid, it defaults to 'AA';
   ///      the 'size' property states 'large' or 'small' - if missing or invalid, it defaults to 'small'.
   /// If the entire object is absent, isReadable defaults to {level:"AA",size:"small"}.
   ///
   /// Example
   /// new Color().IsReadable('#000', '#111') => false
   /// new Color().IsReadable('#000', '#111', { level: 'AA', size: 'large' }) => false
   public static bool IsReadable(Color color1, Color color2, WCAG2Parms? wcag2 = null)
   {
      wcag2 ??= new WCAG2Parms();
      var readabilityLevel = Readability(color1, color2);
      if (wcag2.Level == WCAG2Level.AA) {
         if (wcag2.Size == WCAG2Size.Large) {
            return NumberUtils.FuzzyGreaterOrEqual(readabilityLevel, 3);
         }
         return NumberUtils.FuzzyGreaterOrEqual(readabilityLevel, 4.5);
      } else if (wcag2.Level == WCAG2Level.AAA) {
         if (wcag2.Size == WCAG2Size.Large) {
            return NumberUtils.FuzzyGreaterOrEqual(readabilityLevel, 4.5);
         }
         return NumberUtils.FuzzyGreaterOrEqual(readabilityLevel, 7);
      }

      return false;
   }

   ///
   /// Given a base color and a list of possible foreground or background
   /// colors for that base, returns the most readable color.
   /// Optionally returns Black or White if the most readable color is unreadable.
   ///
   /// @param baseColor - the base color.
   /// @param colorList - array of colors to pick the most readable one from.
   /// @param args - and object with extra arguments
   ///
   /// Example
   /// new Color().mostReadable('#123', ['#124", "#125'], { includeFallbackColors: false }).toHexString(); // "#112255"
   /// new Color().mostReadable('#123', ['#124", "#125'],{ includeFallbackColors: true }).toHexString();  // "#ffffff"
   /// new Color().mostReadable('#a8015a', ["#faf3f3"], { includeFallbackColors:true, level: 'AAA', size: 'large' }).toHexString(); // "#faf3f3"
   /// new Color().mostReadable('#a8015a', ["#faf3f3"], { includeFallbackColors:true, level: 'AAA', size: 'small' }).toHexString(); // "#ffffff"
   ///
   public static Color? MostReadable(Color baseColor, List<Color> colorList, WCAG2FallbackParms? args)
   {
      args ??= new WCAG2FallbackParms()
      {
         IncludeFallbackColors = false,
         Level = WCAG2Level.AA,
         Size = WCAG2Size.Small
      };
      Color? bestColor = null;
      double bestScore = 0d;
      foreach (var color in colorList) {
         var score = Readability(baseColor, color);
         if (score > bestScore) {
            bestScore = score;
            bestColor = color;
         }
      }
      if (IsReadable(baseColor, bestColor!.Value, new WCAG2Parms() { Level = args.Level, Size = args.Size }) || !args.IncludeFallbackColors) {
         return bestColor;
      }
      args.IncludeFallbackColors = false;
      return MostReadable(baseColor, new List<Color>()
      {
         Color.Parse("#fff"),
         Color.Parse("#000")
      }, args);
   }
}