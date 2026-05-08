using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

public partial class Dialog
{
    public static object? ShowDialog<TView, TViewModel>(TViewModel? dataContext,
                                                        DialogOptions? options = null,
                                                        TopLevel? topLevel = null)
        where TView : Control, new()
    {
        return ShowDialog(new TView(), dataContext, options, topLevel);
    }

    public static object? ShowDialogModal<TView, TViewModel>(TViewModel? dataContext,
                                                             DialogOptions? options = null,
                                                             TopLevel? topLevel = null)
        where TView : Control, new()
    {
        return ShowDialogModal(new TView(), dataContext, options, topLevel);
    }

    public static object? ShowDialog(Control content,
                                     object? dataContext = null,
                                     DialogOptions? options = null,
                                     TopLevel? topLevel = null)
    {
        var dialog = CreateDialog(content, dataContext, options, ResolvePlacementTarget(options, topLevel));
        return dialog.Open();
    }

    public static object? ShowDialogModal(Control content,
                                          object? dataContext = null,
                                          DialogOptions? options = null,
                                          TopLevel? topLevel = null)
    {
        var dialog = CreateDialog(content, dataContext, options, ResolvePlacementTarget(options, topLevel));
        dialog.IsModal = true;
        return dialog.Open();
    }

    public static async Task ShowDialogAsync<TView, TViewModel>(TViewModel? dataContext,
                                                                DialogOptions? options = null,
                                                                Action<IDialogActionResult>? closed = null,
                                                                TopLevel? topLevel = null,
                                                                CancellationToken cancellationToken = default)
        where TView : Control, new()
    {
        await ShowDialogAsync(new TView(), dataContext, options, closed, topLevel, cancellationToken);
    }

    public static async Task<object?> ShowDialogModalAsync<TView, TViewModel>(TViewModel? dataContext,
                                                                              DialogOptions? options = null,
                                                                              TopLevel? topLevel = null,
                                                                              CancellationToken cancellationToken = default)
        where TView : Control, new()
    {
        return await ShowDialogModalAsync(new TView(), dataContext, options, topLevel, cancellationToken);
    }

    public static async Task ShowDialogAsync(Control content,
                                             object? dataContext = null,
                                             DialogOptions? options = null,
                                             Action<IDialogActionResult>? closed = null,
                                             TopLevel? topLevel = null,
                                             CancellationToken cancellationToken = default)
    {
        var dialog = CreateDialog(content, dataContext, options, ResolvePlacementTarget(options, topLevel));
        dialog.Closed += (_, _) => closed?.Invoke(new DialogActionResult(dialog.Result));
        await dialog.Dispatcher.InvokeAsync(async () => await dialog.OpenAsync(cancellationToken));
    }

    public static async Task<object?> ShowDialogModalAsync(Control content,
                                                           object? dataContext = null,
                                                           DialogOptions? options = null,
                                                           TopLevel? topLevel = null,
                                                           CancellationToken cancellationToken = default)
    {
        var dialog = CreateDialog(content, dataContext, options, ResolvePlacementTarget(options, topLevel));
        dialog.IsModal = true;
        await dialog.Dispatcher.InvokeAsync(async () => await dialog.OpenAsync(cancellationToken));
        return dialog.Result;
    }

    private static Dialog CreateDialog(Control content, object? dataContext, DialogOptions? options, Control placementTarget)
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
            PlacementTarget           = options?.PlacementTarget ?? placementTarget,
            HorizontalOffset          = options?.HorizontalOffset,
            VerticalOffset            = options?.VerticalOffset,
            DialogHostType            = options?.DialogHostType ?? DialogHostType.Overlay,
            StandardButtons           = options?.StandardButtons ?? DialogStandardButton.NoButton,
            DefaultStandardButton     = options?.DefaultStandardButton ?? DialogStandardButton.Ok,
            HorizontalStartupLocation = options?.HorizontalStartupLocation ?? DialogHorizontalAnchor.Center,
            VerticalStartupLocation   = options?.VerticalStartupLocation ?? DialogVerticalAnchor.Center,
            Content                   = content,
            DataContext               = dataContext,
            Width                     = options?.Width ?? double.NaN,
            Height                    = options?.Height ?? double.NaN,
            MinWidth                  = options?.MinWidth ?? 0d,
            MinHeight                 = options?.MinHeight ?? 0d,
            MaxWidth                  = options?.MaxWidth ?? double.PositiveInfinity,
            MaxHeight                 = options?.MaxHeight ?? double.PositiveInfinity
        };
    }

    private static Control ResolvePlacementTarget(DialogOptions? options, TopLevel? topLevel)
    {
        if (options?.PlacementTarget is not null)
        {
            return options.PlacementTarget;
        }

        var resolvedTopLevel = topLevel ?? Window.GetMainWindow();
        if (resolvedTopLevel is null)
        {
            throw new InvalidOperationException("Unable to resolve TopLevel for Dialog.");
        }

        if (resolvedTopLevel.Content is Control contentControl)
        {
            return contentControl;
        }

        return resolvedTopLevel;
    }
}
