namespace AtomUI.Media;

public static class FontUtils
{
   /// <summary>
   /// 将 value 的值转换为像素
   /// </summary>
   /// <param name="value">EM 单位值</param>
   /// <param name="fontSize">基础字体大小（像素）</param>
   /// <param name="renderScaling">渲染缩放比例，默认为 1.0</param>
   /// <returns>转换后的像素值</returns>
   public static double ConvertEmToPixel(double value, double fontSize, double renderScaling = 1.0)
    {
        return fontSize * value * renderScaling;
    }
}