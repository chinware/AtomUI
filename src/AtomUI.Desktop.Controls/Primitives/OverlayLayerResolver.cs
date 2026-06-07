using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

internal static class OverlayLayerResolver
{
    internal static Panel ResolveOverlayLayer(Visual? placementTarget, TopLevel? topLevel, string ownerName)
    {
        if (TryResolveOverlayLayer(placementTarget) is { } placementOverlayLayer)
        {
            return placementOverlayLayer;
        }

        if (TryResolveOverlayLayer(topLevel) is { } topLevelOverlayLayer)
        {
            return topLevelOverlayLayer;
        }

        if (RuntimePlatform.Features.SupportsNativeWindow &&
            Window.GetMainWindow() is { } mainWindow &&
            TryResolveOverlayLayer(mainWindow) is { } mainWindowOverlayLayer)
        {
            return mainWindowOverlayLayer;
        }

        if (TryResolveOverlayLayer(GetSingleViewMainView()) is { } mainViewOverlayLayer)
        {
            return mainViewOverlayLayer;
        }

        throw new InvalidOperationException(
            $"Unable to resolve overlay layer for {ownerName}; ensure the host visual tree contains a VisualLayerManager.");
    }

    internal static Control ResolvePlacementTarget(Control requester, Control? placementTarget, string ownerName)
    {
        if (placementTarget is not null)
        {
            return placementTarget;
        }

        if (requester.FindLogicalAncestorOfType<Control>() is { } logicalAncestor)
        {
            return logicalAncestor;
        }

        if (RuntimePlatform.Features.SupportsNativeWindow &&
            Window.GetMainWindow() is { } mainWindow)
        {
            return mainWindow;
        }

        if (GetSingleViewMainView() is { } mainView)
        {
            return mainView;
        }

        throw new InvalidOperationException($"Unable to resolve {ownerName} placement target.");
    }

    private static Panel? TryResolveOverlayLayer(Visual? anchor)
    {
        return anchor?.GetPopupOverlayLayer() as Panel;
    }

    private static Control? GetSingleViewMainView()
    {
        return Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime
            ? singleViewLifetime.MainView
            : null;
    }
}
