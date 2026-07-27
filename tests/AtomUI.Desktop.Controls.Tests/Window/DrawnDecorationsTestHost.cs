using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Shouldly;

using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Window;

internal static class DrawnDecorationsTestHost
{
    internal static Action Install(
        AtomUIWindow window,
        Control overlayContent,
        Thickness? frameThickness = null,
        bool wrapOverlayContent = true)
    {
        var topLevelHost = GetTopLevelHost(window);
        var decorationsField = GetDecorationsField(window);
        var originalDecorations = decorationsField.GetValue(topLevelHost);
        var content = new Avalonia.Controls.Chrome.WindowDrawnDecorationsContent
        {
            Overlay = wrapOverlayContent
                ? new Panel { Children = { overlayContent } }
                : overlayContent
        };
        var decorations = new Avalonia.Controls.Chrome.WindowDrawnDecorations();
        typeof(Avalonia.Controls.Chrome.WindowDrawnDecorations)
            .GetProperty(
                nameof(Avalonia.Controls.Chrome.WindowDrawnDecorations.Content),
                BindingFlags.Instance | BindingFlags.Public)
            .ShouldNotBeNull()
            .GetSetMethod(nonPublic: true)
            .ShouldNotBeNull()
            .Invoke(decorations, new object?[] { content });
        if (frameThickness is { } effectiveFrameThickness)
        {
            typeof(Avalonia.Controls.Chrome.WindowDrawnDecorations)
                .GetProperty(
                    nameof(Avalonia.Controls.Chrome.WindowDrawnDecorations.FrameThickness),
                    BindingFlags.Instance | BindingFlags.Public)
                .ShouldNotBeNull()
                .GetSetMethod(nonPublic: true)
                .ShouldNotBeNull()
                .Invoke(decorations, new object?[] { effectiveFrameThickness });
        }

        decorationsField.SetValue(topLevelHost, decorations);

        return () => decorationsField.SetValue(topLevelHost, originalDecorations);
    }

    private static object GetTopLevelHost(AtomUIWindow window)
    {
        return typeof(TopLevel)
               .GetField("_topLevelHost", BindingFlags.Instance | BindingFlags.NonPublic)
               .ShouldNotBeNull()
               .GetValue(window)
               .ShouldNotBeNull();
    }

    private static FieldInfo GetDecorationsField(AtomUIWindow window)
    {
        return GetTopLevelHost(window)
               .GetType()
               .GetField("_decorations", BindingFlags.Instance | BindingFlags.NonPublic)
               .ShouldNotBeNull();
    }
}
