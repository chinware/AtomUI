using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Desktop.Controls.Utils;

internal static class TextLayoutReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(TextLayout))]
    internal static readonly Lazy<MethodInfo> CreateTextParagraphPropertiesMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(TextLayout).GetMethodInfoOrThrow("CreateTextParagraphProperties",
            BindingFlags.Static | BindingFlags.NonPublic));
    #endregion
    
    public static TextParagraphProperties CreateTextParagraphProperties(Typeface typeface, 
                                                                        double fontSize,
                                                                        IBrush? foreground, 
                                                                        TextAlignment textAlignment,
                                                                        TextWrapping textWrapping,
                                                                        TextDecorationCollection? textDecorations, 
                                                                        FlowDirection flowDirection,
                                                                        double lineHeight,
                                                                        double letterSpacing,
                                                                        FontFeatureCollection? features)
    {
        var result = CreateTextParagraphPropertiesMethodInfo.Value.Invoke(null, [
            typeface,
            fontSize,
            foreground,
            textAlignment,
            textWrapping,
            textDecorations,
            flowDirection,
            lineHeight,
            letterSpacing,
            features
        ]) as TextParagraphProperties;
        Debug.Assert(result != null);
        return result;
    }
}