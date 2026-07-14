using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

internal static class WindowDrawnDecorationsReflectionExtensions
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(TopLevel))]
    private static readonly Lazy<FieldInfo?> TopLevelHostFieldInfo = new(() =>
        typeof(TopLevel).GetField("_topLevelHost", BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(
        DynamicallyAccessedMemberTypes.NonPublicFields,
        "Avalonia.Controls.TopLevelHost",
        "Avalonia.Controls")]
    private static readonly Lazy<FieldInfo?> DecorationsFieldInfo = new(() =>
        Type.GetType("Avalonia.Controls.TopLevelHost, Avalonia.Controls")?.GetField(
            "_decorations",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(
        DynamicallyAccessedMemberTypes.NonPublicMethods,
        "Avalonia.Controls.TopLevelHost",
        "Avalonia.Controls")]
    private static readonly Lazy<MethodInfo?> UpdateDrawnDecorationsMethodInfo = new(() =>
        Type.GetType("Avalonia.Controls.TopLevelHost, Avalonia.Controls")?.GetMethod(
            "UpdateDrawnDecorations",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(
        DynamicallyAccessedMemberTypes.NonPublicFields,
        "Avalonia.Controls.TopLevelHost",
        "Avalonia.Controls")]
    private static readonly Lazy<FieldInfo?> ResizeGripsFieldInfo = new(() =>
        Type.GetType("Avalonia.Controls.TopLevelHost, Avalonia.Controls")?.GetField(
            "_resizeGrips",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(
        DynamicallyAccessedMemberTypes.NonPublicProperties,
        "Avalonia.Controls.Chrome.ResizeGripLayer",
        "Avalonia.Controls")]
    private static readonly Lazy<PropertyInfo?> GripThicknessPropertyInfo = new(() =>
        Type.GetType("Avalonia.Controls.Chrome.ResizeGripLayer, Avalonia.Controls")?.GetProperty(
            "GripThickness",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(
        DynamicallyAccessedMemberTypes.PublicFields,
        "Avalonia.Controls.Chrome.DrawnWindowDecorationParts",
        "Avalonia.Controls")]
    private static readonly Lazy<Type?> DrawnWindowDecorationPartsType = new(() =>
        Type.GetType("Avalonia.Controls.Chrome.DrawnWindowDecorationParts, Avalonia.Controls"));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(WindowDrawnDecorations))]
    private static readonly Lazy<PropertyInfo?> RenderScalingPropertyInfo = new(() =>
        typeof(WindowDrawnDecorations).GetProperty(
            "RenderScaling",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(WindowDrawnDecorations))]
    private static readonly Lazy<PropertyInfo?> TitleBarHeightOverridePropertyInfo = new(() =>
        typeof(WindowDrawnDecorations).GetProperty(
            "TitleBarHeightOverride",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaWindow))]
    private static readonly Lazy<MethodInfo?> UpdateDrawnDecorationMarginsMethodInfo = new(() =>
        typeof(AvaloniaWindow).GetMethod(
            "UpdateDrawnDecorationMargins",
            BindingFlags.Instance | BindingFlags.NonPublic));

    public static bool TryUpdateDrawnDecorations(
        this AvaloniaWindow window,
        WindowsDrawnDecorationParts parts)
    {
        var topLevelHost = TopLevelHostFieldInfo.Value?.GetValue(window);
        var avaloniaParts = CreateDrawnDecorationParts(parts);
        if (topLevelHost is null ||
            avaloniaParts is null ||
            UpdateDrawnDecorationsMethodInfo.Value is not { } updateDrawnDecorations)
        {
            return false;
        }

        updateDrawnDecorations.Invoke(
            topLevelHost,
            [avaloniaParts, window.WindowState, window.WindowDecorationsTheme]);

        if (DecorationsFieldInfo.Value?.GetValue(topLevelHost) is WindowDrawnDecorations decorations)
        {
            RenderScalingPropertyInfo.Value?.SetValue(decorations, window.RenderScaling);
            TitleBarHeightOverridePropertyInfo.Value?.SetValue(
                decorations,
                window.ExtendClientAreaTitleBarHeightHint);
        }

        UpdateDrawnDecorationMarginsMethodInfo.Value?.Invoke(window, null);
        return true;
    }

    private static object? CreateDrawnDecorationParts(WindowsDrawnDecorationParts parts)
    {
        var enumType = DrawnWindowDecorationPartsType.Value;
        return enumType is null ? null : Enum.ToObject(enumType, (int)parts);
    }

    public static bool TryTakeOverManagedResizeGrip(
        this TopLevel topLevel,
        double scale,
        out Thickness gripThickness)
    {
        if (!double.IsFinite(scale) || scale < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale));
        }

        gripThickness = default;
        var topLevelHost = TopLevelHostFieldInfo.Value?.GetValue(topLevel);
        if (topLevelHost is null ||
            DecorationsFieldInfo.Value?.GetValue(topLevelHost) is not WindowDrawnDecorations decorations ||
            ResizeGripsFieldInfo.Value?.GetValue(topLevelHost) is not { } resizeGrips ||
            GripThicknessPropertyInfo.Value is not { } gripThicknessProperty)
        {
            return false;
        }

        var frame  = decorations.FrameThickness;
        var shadow = decorations.ShadowThickness;
        gripThickness = new Thickness(
            (frame.Left + shadow.Left) * scale,
            (frame.Top + shadow.Top) * scale,
            (frame.Right + shadow.Right) * scale,
            (frame.Bottom + shadow.Bottom) * scale);

        gripThicknessProperty.SetValue(resizeGrips, default(Thickness));
        return true;
    }
}
