using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogOverlayLayer : Canvas
{
    private readonly Panel _hostLayer;
    private readonly TopLevel? _topLevel;

    internal Size AvailableSize { get; private set; }

    private DialogOverlayLayer(Panel hostLayer, TopLevel? topLevel)
    {
        _hostLayer = hostLayer;
        _topLevel = topLevel;
        _hostLayer.SizeChanged += HandleHostLayerSizeChanged;
        if (_topLevel is not null)
        {
            _topLevel.PropertyChanged += HandleTopLevelPropertyChanged;
        }
        SynchronizeBounds();
    }

    internal static DialogOverlayLayer GetOrCreate(Visual anchor)
    {
        var (hostLayer, topLevel) = ResolveHostLayer(anchor);
        var dialogLayer = hostLayer.Children.OfType<DialogOverlayLayer>().FirstOrDefault();
        if (dialogLayer is not null)
        {
            return dialogLayer;
        }

        dialogLayer = new DialogOverlayLayer(hostLayer, topLevel);
        hostLayer.Children.Add(dialogLayer);
        return dialogLayer;
    }

    private static (Panel HostLayer, TopLevel? TopLevel) ResolveHostLayer(Visual anchor)
    {
        var topLevel = TopLevel.GetTopLevel(anchor);
        if (topLevel is Window window &&
            window.GetDrawnDialogOverlayLayer() is { } drawnDialogLayer)
        {
            return (drawnDialogLayer, topLevel);
        }

        if (topLevel is not null &&
            topLevel.GetPopupOverlayLayer() is Panel topLevelLayer)
        {
            return (topLevelLayer, topLevel);
        }

        var scopeLayer = ScopeAwareOverlayLayer.GetLayer(anchor) ??
                         throw new InvalidOperationException("Unable to resolve an overlay layer for Dialog.");
        return (scopeLayer, null);
    }

    internal void Add(OverlayDialogPresenter presenter)
    {
        if (presenter.Parent is Panel previousParent)
        {
            previousParent.Children.Remove(presenter);
        }

        SynchronizeBounds();
        presenter.Width  = Width;
        presenter.Height = Height;
        Children.Add(presenter);
    }

    internal void Remove(OverlayDialogPresenter presenter)
    {
        Children.Remove(presenter);
        if (Children.Count != 0)
        {
            return;
        }

        _hostLayer.SizeChanged -= HandleHostLayerSizeChanged;
        if (_topLevel is not null)
        {
            _topLevel.PropertyChanged -= HandleTopLevelPropertyChanged;
        }
        _hostLayer.Children.Remove(this);
    }

    internal void Activate(OverlayDialogPresenter presenter)
    {
        var index = Children.IndexOf(presenter);
        if (index < 0 || index == Children.Count - 1)
        {
            return;
        }

        Children.RemoveAt(index);
        Children.Add(presenter);
    }

    internal bool IsTopmost(OverlayDialogPresenter presenter)
    {
        return Children.Count > 0 && ReferenceEquals(Children[^1], presenter);
    }

    protected override void OnKeyDown(Avalonia.Input.KeyEventArgs e)
    {
        if (!e.Handled &&
            Children.LastOrDefault() is OverlayDialogPresenter presenter &&
            presenter.TryInvokeStandardButton(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private void HandleHostLayerSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        SynchronizeBounds();
    }

    private void HandleTopLevelPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == TopLevel.ClientSizeProperty)
        {
            SynchronizeBounds();
        }
    }

    private void SynchronizeBounds()
    {
        var size = _topLevel?.ClientSize ??
                   (_hostLayer is ScopeAwareOverlayLayer scopeLayer
                       ? scopeLayer.AvailableSize
                       : _hostLayer.Bounds.Size);
        if (size.Width <= 0 || size.Height <= 0)
        {
            size = _hostLayer.Bounds.Size;
        }

        AvailableSize = size;
        Width  = size.Width;
        Height = size.Height;
        Canvas.SetLeft(this, 0);
        Canvas.SetTop(this, 0);
        foreach (var presenter in Children.OfType<OverlayDialogPresenter>())
        {
            presenter.Width  = size.Width;
            presenter.Height = size.Height;
            presenter.UpdateLayerBounds(size);
        }
    }
}
