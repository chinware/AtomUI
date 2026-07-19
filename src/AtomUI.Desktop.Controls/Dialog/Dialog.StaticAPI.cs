using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

public partial class Dialog
{
    public static async Task<object?> ShowDialogAsync<TView, TViewModel>(
        TViewModel? dataContext,
        DialogOptions? options = null,
        TopLevel? topLevel = null,
        CancellationToken cancellationToken = default)
        where TView : Control, new()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() =>
                ShowDialogAsync<TView, TViewModel>(dataContext, options, topLevel, cancellationToken));
        }

        return await ShowDialogAsync(new TView(), dataContext, options, topLevel, cancellationToken);
    }

    public static async Task<object?> ShowDialogModalAsync<TView, TViewModel>(
        TViewModel? dataContext,
        DialogOptions? options = null,
        TopLevel? topLevel = null,
        CancellationToken cancellationToken = default)
        where TView : Control, new()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() =>
                ShowDialogModalAsync<TView, TViewModel>(dataContext, options, topLevel, cancellationToken));
        }

        return await ShowDialogModalAsync(new TView(), dataContext, options, topLevel, cancellationToken);
    }

    public static Task<object?> ShowDialogAsync(
        Control content,
        object? dataContext = null,
        DialogOptions? options = null,
        TopLevel? topLevel = null,
        CancellationToken cancellationToken = default)
    {
        return ShowDialogCoreAsync(
            content,
            dataContext,
            options,
            topLevel,
            isModal: false,
            cancellationToken);
    }

    public static Task<object?> ShowDialogModalAsync(
        Control content,
        object? dataContext = null,
        DialogOptions? options = null,
        TopLevel? topLevel = null,
        CancellationToken cancellationToken = default)
    {
        return ShowDialogCoreAsync(
            content,
            dataContext,
            options,
            topLevel,
            isModal: true,
            cancellationToken);
    }

    private static async Task<object?> ShowDialogCoreAsync(
        Control content,
        object? dataContext,
        DialogOptions? options,
        TopLevel? topLevel,
        bool isModal,
        CancellationToken cancellationToken)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => ShowDialogCoreAsync(
                content,
                dataContext,
                options,
                topLevel,
                isModal,
                cancellationToken));
        }

        var overlayLayer = ResolveOverlayLayer(options, topLevel);
        var dialog       = CreateDialog(content, dataContext, options, overlayLayer);
        dialog.IsModal = isModal;
        overlayLayer.Children.Add(dialog);
        try
        {
            await dialog.OpenAsync(cancellationToken);
            return dialog.Result;
        }
        finally
        {
            overlayLayer.Children.Remove(dialog);
        }
    }

    private static Dialog CreateDialog(
        Control content,
        object? dataContext,
        DialogOptions? options,
        Control placementTarget)
    {
        return new Dialog
        {
            Title                     = options?.Title,
            TitleIcon                 = options?.TitleIcon,
            IsResizable               = options?.IsResizable ?? false,
            IsClosable                = options?.IsClosable ?? true,
            IsMaximizable             = options?.IsMaximizable ?? false,
            IsMinimizable             = options?.IsMinimizable ?? true,
            IsDragMovable             = options?.IsDragMovable ?? true,
            IsFooterVisible           = options?.IsFooterVisible ?? true,
            IsMotionEnabled           = options?.IsMotionEnabled ?? true,
            PlacementTarget           = options?.PlacementTarget ?? placementTarget,
            MotionAnchorMode          = options?.PlacementTarget is null
                ? DialogMotionAnchorMode.FallbackPlacementTarget
                : DialogMotionAnchorMode.ExplicitPlacementTarget,
            HorizontalOffset          = options?.HorizontalOffset,
            VerticalOffset            = options?.VerticalOffset,
            DialogHostType            = options?.DialogHostType ?? DialogHostType.Overlay,
            StandardButtons           = options?.StandardButtons ?? DialogStandardButton.NoButton,
            DefaultStandardButton     = options?.DefaultStandardButton ?? DialogStandardButton.Ok,
            HorizontalStartupLocation = options?.HorizontalStartupLocation ?? DialogHorizontalAnchor.Center,
            VerticalStartupLocation   = options?.VerticalStartupLocation ?? DialogVerticalAnchor.Center,
            Content                   = content,
            DataContext               = dataContext,
            HostWidth                 = options?.HostWidth ?? double.NaN,
            HostHeight                = options?.HostHeight ?? double.NaN,
            HostMinWidth              = options?.HostMinWidth ?? 0d,
            HostMinHeight             = options?.HostMinHeight ?? 0d,
            HostMaxWidth              = options?.HostMaxWidth ?? double.PositiveInfinity,
            HostMaxHeight             = options?.HostMaxHeight ?? double.PositiveInfinity,
            BeforeCloseAsync          = options?.BeforeCloseAsync
        };
    }

    private static Panel ResolveOverlayLayer(DialogOptions? options, TopLevel? topLevel)
    {
        return OverlayLayerResolver.ResolveOverlayLayer(options?.PlacementTarget, topLevel, nameof(Dialog));
    }
}
