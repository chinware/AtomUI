using System.Diagnostics;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Media.TextFormatting;

internal static class TextParagraphPropertiesReflectionExtensions
{
    #region 反射信息定义
    private static readonly Lazy<PropertyInfo> LineSpacingPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(TextParagraphProperties).GetPropertyInfoOrThrow("LineSpacing",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion

    public static double GetLineSpacing(this TextParagraphProperties properties)
    {
        var lineSpacing = LineSpacingPropertyInfo.Value.GetValue(properties) as double?;
        Debug.Assert(lineSpacing != null);
        return lineSpacing.Value;
    }

    public static void SetLineSpacing(this TextParagraphProperties properties, double value)
    {
        LineSpacingPropertyInfo.Value.SetValue(properties, value);
    }
}