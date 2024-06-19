using Avalonia.Media;

namespace AtomUI.ColorSystem;

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

         var parts = new List<string>(colorExpr.Substring(leftParen + 1, rightParen - leftParen - 1).Split(',', StringSplitOptions.RemoveEmptyEntries));
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
}