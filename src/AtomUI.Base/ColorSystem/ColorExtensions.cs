using System.Globalization;
using AtomUI.Utils;
using Avalonia.Media;

namespace AtomUI.ColorSystem;


public static class ColorExtensions
{
   public static double GetRedF(this Color color)
   {
      return color.R / 255d;
   }

   public static double GetGreenF(this Color color)
   {
      return color.G / 255d;
   }
   
   public static double GetBlueF(this Color color)
   {
      return color.B / 255d;
   }

   public static double GetAlphaF(this Color color)
   {
      return color.A / 255d;
   }

   public static string HexName(this Color color, ColorNameFormat format = ColorNameFormat.HexRgb)
   {
      uint rgb = color.ToUInt32();
      string formatStr = "x8";
      if (format == ColorNameFormat.HexRgb) {
         formatStr = "x6";
         rgb &= 0xFFFFFF;
      }
      return $"#{rgb.ToString(formatStr, CultureInfo.InvariantCulture)}";
   }
   
   public static Color Desaturate(this Color color, int amount = 10)
   {
      amount = NumberUtils.Clamp(amount, 0, 100);
      HslColor hslColor = color.ToHsl();
      double s = hslColor.S;
      s -= amount / 100d;
      s = NumberUtils.Clamp(s, 0d, 1d);
      return HslColor.FromHsl(hslColor.H, s, hslColor.L).ToRgb();
   }
   
   public static Color Saturate(this Color color, int amount = 10)
   {
      amount = NumberUtils.Clamp(amount, 0, 100);
      HslColor hslColor = color.ToHsl();
      double s = hslColor.S;
      s += amount / 100d;
      s = NumberUtils.Clamp(s, 0d, 1d);
      return HslColor.FromHsl(hslColor.H, s, hslColor.L).ToRgb();
   }
   
   public static Color Greyscale(this Color color)
   {
      return color.Desaturate(100);
   }
   
   public static Color Lighten(this Color color, int amount = 10)
   {
      amount = NumberUtils.Clamp(amount, 0, 100);
      HslColor hslColor = color.ToHsl();
      double l = hslColor.L;
      l += amount / 100d;
      l = NumberUtils.Clamp(l, 0d, 1d);
      return HslColor.FromHsl(hslColor.H, hslColor.S, l).ToRgb();
   }
   
   public static Color Brighten(this Color color, int amount = 10)
   {
      amount = NumberUtils.Clamp(amount, 0, 100);
      int r = color.R;
      int g = color.G;
      int b = color.B;
      int delta = (int)Math.Round(255d * -(amount / 100d));
      r = Math.Max(0, Math.Min(255, r - delta));
      g = Math.Max(0, Math.Min(255, g - delta));
      b = Math.Max(0, Math.Min(255, b - delta));
      return Color.FromRgb((byte)r, (byte)g, (byte)b);
   }
   
   public static Color Darken(this Color color, int amount = 10)
   {
      amount = NumberUtils.Clamp(amount, 0, 100);
      HslColor hslColor = color.ToHsl();
      double l = hslColor.L;
      l -= amount / 100d;
      l = NumberUtils.Clamp(l, 0d, 1d);
      return HslColor.FromHsl(hslColor.H, hslColor.S, l).ToRgb();
   }
   
   public static Color Spin(this Color color, int amount = 10)
   {
      HslColor hslColor = color.ToHsl();
      double h = hslColor.H;
      h = (h + amount) % 360;
      h = h < 0 ? 360 + h : h;
      return HslColor.FromHsl(h, hslColor.S, hslColor.L).ToRgb();
   }
}