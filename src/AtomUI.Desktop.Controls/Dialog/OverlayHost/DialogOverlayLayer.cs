using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using AvaloniaVisualLayerManager = Avalonia.Controls.Primitives.VisualLayerManager;

internal sealed class DialogOverlayLayer : Canvas
{
    private readonly Panel _hostLayer;
    private readonly TopLevel? _topLevel;
    private readonly bool _usesArrangedHostBounds;
    // Dialog 弹层作用域:包裹本层的 popup-capable VisualLayerManager,是 Dialog 内容弹层的宿主边界,
    // 保证内容区 popup 渲染在本 scope 全部 presenter 之上(契约见 docs/controls/desktop/feedback/modal/popup-layering-design.md)。
    private readonly AvaloniaVisualLayerManager _popupScope;

    internal Size AvailableSize { get; private set; }

    private DialogOverlayLayer(Panel hostLayer, TopLevel? topLevel)
    {
        _hostLayer = hostLayer;
        _topLevel = topLevel;
        _usesArrangedHostBounds = hostLayer is not Canvas &&
                                  hostLayer.IsAttachedToVisualTree() &&
                                  topLevel is Window window &&
                                  ReferenceEquals(hostLayer, window.GetDrawnDialogOverlayLayer());
        _popupScope = VisualLayerManagerReflectionExtensions.CreatePopupCapableScope();
        _popupScope.Child = this;
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
        var dialogLayer = hostLayer.Children
                                   .OfType<AvaloniaVisualLayerManager>()
                                   .Select(scope => scope.Child)
                                   .OfType<DialogOverlayLayer>()
                                   .FirstOrDefault();
        if (dialogLayer is not null)
        {
            return dialogLayer;
        }

        dialogLayer = new DialogOverlayLayer(hostLayer, topLevel);
        hostLayer.Children.Add(dialogLayer._popupScope);
        return dialogLayer;
    }

    private static (Panel HostLayer, TopLevel? TopLevel) ResolveHostLayer(Visual anchor)
    {
        var topLevel = TopLevel.GetTopLevel(anchor);
        if (topLevel is Window window &&
            window.GetDrawnDialogOverlayLayer() is { } drawnDialogLayer &&
            drawnDialogLayer.IsAttachedToVisualTree())
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
        presenter.Width  = AvailableSize.Width;
        presenter.Height = AvailableSize.Height;
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
        _hostLayer.Children.Remove(_popupScope);
        _popupScope.Child = null;
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
        // 尺寸同步作用于弹层作用域(宿主层的直接子节点);本层在作用域内 Stretch 填满。
        if (_usesArrangedHostBounds)
        {
            _popupScope.ClearValue(WidthProperty);
            _popupScope.ClearValue(HeightProperty);
        }
        else
        {
            _popupScope.Width  = size.Width;
            _popupScope.Height = size.Height;
        }

        Canvas.SetLeft(_popupScope, 0);
        Canvas.SetTop(_popupScope, 0);
        foreach (var presenter in Children.OfType<OverlayDialogPresenter>())
        {
            presenter.Width  = size.Width;
            presenter.Height = size.Height;
            presenter.UpdateLayerBounds(size);
        }
    }
}
