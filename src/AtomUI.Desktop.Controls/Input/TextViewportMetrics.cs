using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

internal static class TextViewportMetrics
{
    internal static readonly AttachedProperty<double?> ViewportWidthProperty =
        AvaloniaProperty.RegisterAttached<Avalonia.Controls.TextBox, double?>(
            "ViewportWidth",
            typeof(TextViewportMetrics));

    internal static double? GetViewportWidth(Avalonia.Controls.TextBox textBox)
    {
        return textBox.GetValue(ViewportWidthProperty);
    }

    internal static void SetViewportWidth(Avalonia.Controls.TextBox textBox, double? value)
    {
        textBox.SetCurrentValue(ViewportWidthProperty, value);
    }

    internal static IDisposable PublishViewportWidth(
        Avalonia.Controls.TextBox textBox,
        ScrollViewer scrollViewer,
        TextPresenter? textPresenter)
    {
        SetViewportWidth(textBox, double.NaN);
        var subscriptions = new CompositeDisposable();
        void UpdateViewportWidth()
        {
            SetViewportWidth(textBox, CalculateViewportWidth(scrollViewer, textPresenter));
        }

        subscriptions.Add(scrollViewer.GetObservable(ScrollViewer.ViewportProperty)
                                      .Subscribe(_ => UpdateViewportWidth()));
        subscriptions.Add(scrollViewer.GetObservable(ScrollViewer.PaddingProperty)
                                      .Subscribe(_ => UpdateViewportWidth()));
        if (textPresenter is not null)
        {
            subscriptions.Add(textPresenter.GetObservable(Layoutable.MarginProperty)
                                           .Subscribe(_ => UpdateViewportWidth()));
        }

        return subscriptions;
    }

    private static double CalculateViewportWidth(ScrollViewer scrollViewer, TextPresenter? textPresenter)
    {
        var padding = scrollViewer.Padding;
        var margin  = textPresenter?.Margin ?? default;
        var width   = scrollViewer.Viewport.Width -
                      padding.Left - padding.Right -
                      margin.Left - margin.Right;

        return double.IsFinite(width) && width > 0 ? width : double.NaN;
    }
}
