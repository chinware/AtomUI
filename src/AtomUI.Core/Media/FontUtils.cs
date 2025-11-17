namespace AtomUI.Media;

public static class FontUtils
{
   /// <summary>
   /// 将 value 的值转换为像素
   /// </summary>
   /// <param name="value"></param>
   /// <param name="fontSize"></param>
   /// <param name="renderScaling"></param>
   /// <returns></returns>
   public static double ConvertEmToPixel(double value, double fontSize, double renderScaling = 1.0)
    {
        var fontSizePx = fontSize * value * renderScaling;
        return fontSizePx * value;
    }
}