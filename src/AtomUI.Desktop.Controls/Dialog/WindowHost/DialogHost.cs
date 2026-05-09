using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Native;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

internal class DialogHost : Window,
                            IDialogHost,
                            IMotionAwareControl
{
    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<DialogHost>();

    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<DialogHost>();

    public static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        DialogButtonBox.EscapeStandardButtonProperty.AddOwner<DialogHost>();

    public static readonly StyledProperty<bool> IsFooterVisibleProperty =
        Dialog.IsFooterVisibleProperty.AddOwner<DialogHost>();

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(IsModal));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DialogHost>();

    public static readonly StyledProperty<bool> IsLoadingProperty =
        Dialog.IsLoadingProperty.AddOwner<DialogHost>();

    public static readonly StyledProperty<bool> IsConfirmLoadingProperty =
        Dialog.IsConfirmLoadingProperty.AddOwner<DialogHost>();

    public static readonly StyledProperty<IDataTemplate?> DialogContentTemplateProperty =
        AvaloniaProperty.Register<DialogHost, IDataTemplate?>(nameof(DialogContentTemplate));

    public DialogStandardButtons StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }

    public DialogStandardButton DefaultStandardButton
    {
        get => GetValue(DefaultStandardButtonProperty);
        set => SetValue(DefaultStandardButtonProperty, value);
    }

    public DialogStandardButton EscapeStandardButton
    {
        get => GetValue(EscapeStandardButtonProperty);
        set => SetValue(EscapeStandardButtonProperty, value);
    }

    public bool IsFooterVisible
    {
        get => GetValue(IsFooterVisibleProperty);
        set => SetValue(IsFooterVisibleProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsConfirmLoading
    {
        get => GetValue(IsConfirmLoadingProperty);
        set => SetValue(IsConfirmLoadingProperty, value);
    }

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public IDataTemplate? DialogContentTemplate
    {
        get => GetValue(DialogContentTemplateProperty);
        set => SetValue(DialogContentTemplateProperty, value);
    }

    public TopLevel ParentTopLevel { get; }
    public AvaloniaList<DialogButton> CustomButtons { get; } = new();
    object? IDialogHost.Content
    {
        get => _dialogContent.DialogContent;
        set => _dialogContent.DialogContent = value;
    }
    IDataTemplate? IDialogHost.ContentTemplate
    {
        get => DialogContentTemplate;
        set => DialogContentTemplate = value;
    }

    protected override Type StyleKeyOverride { get; } = typeof(DialogHost);

    private readonly Dialog _dialog;
    private readonly DialogWindowContent _dialogContent;
    private CompositeDisposable? _confirmLoadingBindings;
    private Rect _ownerBounds;
    private Size _hostSize;
    private bool _hasAppliedInitialPlacement;

    public DialogHost(TopLevel parent, Dialog dialog)
    {
        ParentTopLevel = parent;
        _dialog        = dialog;
        _dialogContent = new DialogWindowContent();
        Content        = _dialogContent;

        BindContentProperties();
        CustomButtons.CollectionChanged += HandleCustomButtonsChanged;
    }

    public void BindDialog(Dialog dialog, CompositeDisposable disposables)
    {
        ((ISetLogicalParent)this).SetParent(dialog);
        this
            .Bind(ThemeVariantScope.ActualThemeVariantProperty,
                dialog.GetBindingObservable(ThemeVariantScope.ActualThemeVariantProperty))
            .DisposeWith(disposables);
        this[!Window.TitleProperty]                       = dialog[!Dialog.TitleProperty];
        this[!Window.LogoProperty]                        = dialog[!Dialog.TitleIconProperty];
        this[!IsModalProperty]                            = dialog[!Dialog.IsModalProperty];
        this[!IsMotionEnabledProperty]                    = dialog[!Dialog.IsMotionEnabledProperty];
        this[!IsLoadingProperty]                          = dialog[!Dialog.IsLoadingProperty];
        this[!IsConfirmLoadingProperty]                   = dialog[!Dialog.IsConfirmLoadingProperty];
        this[!IsFooterVisibleProperty]                    = dialog[!Dialog.IsFooterVisibleProperty];
        this[!StandardButtonsProperty]                    = dialog[!Dialog.StandardButtonsProperty];
        this[!DefaultStandardButtonProperty]              = dialog[!Dialog.DefaultStandardButtonProperty];
        this[!EscapeStandardButtonProperty]               = dialog[!Dialog.EscapeStandardButtonProperty];
        this[!Window.CanResizeProperty]                   = dialog[!Dialog.IsResizableProperty];
        this[!Window.CanMinimizeProperty]                 = dialog[!Dialog.EffectiveMinimizableProperty];
        this[!Window.CanMaximizeProperty]                 = dialog[!Dialog.IsMaximizableProperty];
        this[!Window.IsCloseCaptionButtonVisibleProperty] = dialog[!Dialog.IsClosableProperty];
        this[!Window.IsMoveEnabledProperty]               = dialog[!Dialog.IsDragMovableProperty];
    }

    public void AttachPlacement(Control placementTarget)
    {
        _ownerBounds = GetOwnerBounds(ParentTopLevel);
        _hasAppliedInitialPlacement = false;
    }

    public void UpdateSizing()
    {
        var measuredSize = MeasureDialogContent();
        SizeToContent = SizeToContent.Manual;
        Width     = double.IsNaN(_dialog.HostWidth) ? measuredSize.Width : _dialog.HostWidth;
        MinWidth  = _dialog.HostMinWidth;
        MaxWidth  = _dialog.HostMaxWidth;
        Height    = double.IsNaN(_dialog.HostHeight) ? measuredSize.Height : _dialog.HostHeight;
        MinHeight = _dialog.HostMinHeight;
        MaxHeight = _dialog.HostMaxHeight;
    }

    private Size MeasureDialogContent()
    {
        _dialogContent.ApplyStyling();
        _dialogContent.ApplyTemplate();

        // 把从 Window 外框到 DialogWindowContent 之间的 chrome 开销加回去：
        //   - Padding：ContentFrame.Padding 对内容的内缩
        //   - TitleBarHeight：CSD 模式下 AtomUI 自绘；非 CSD 下为系统原生标题栏
        //   - FrameShadowThickness：VisualLayerManager 为阴影预留的外边距，
        //     Window 外框尺寸包含这份空间，否则内容会被挤出可视区。
        var padding          = Padding;
        var shadow           = FrameShadowThickness;
        var titleBarOverhead = IsCsdEnabled
            ? IsTitleBarVisible ? TitleBarHeight : 0
            : this.GetSystemTitleBarHeight() ?? 0;
        var horizontalChrome = padding.Left + padding.Right + shadow.Left + shadow.Right;
        var verticalChrome   = padding.Top + padding.Bottom + shadow.Top + shadow.Bottom + titleBarOverhead;

        var ownerSize = GetOwnerBounds(ParentTopLevel).Size;

        // 先在最宽可用空间下量一次，得到 content 的自然宽度和高度上限。
        var loose = new Size(
            ResolveAvailableMeasureSize(ownerSize.Width, _dialog.HostMaxWidth),
            ResolveAvailableMeasureSize(ownerSize.Height, _dialog.HostMaxHeight));
        _dialogContent.Measure(loose);
        var naturalContent = _dialogContent.DesiredSize;

        // Width：HostWidth 硬写优先；否则取自然宽度并被 HostMinWidth/HostMaxWidth 夹住，
        // 保证窗口宽度落在用户指定的区间。
        double windowWidth;
        if (!double.IsNaN(_dialog.HostWidth))
        {
            windowWidth = _dialog.HostWidth;
        }
        else
        {
            var natural = naturalContent.Width + horizontalChrome;
            windowWidth = ResolveMeasuredSize(natural, _dialog.HostMinWidth, _dialog.HostMaxWidth);
        }

        // 反推给 content 的可用宽度，再测一遍让 content（比如可换行文本）在这个宽度下
        // 自己算出真实高度。
        var contentAvailableWidth = Math.Max(0, windowWidth - horizontalChrome);
        _dialogContent.Measure(new Size(contentAvailableWidth, loose.Height));
        var constrainedHeight = _dialogContent.DesiredSize.Height + verticalChrome + 8; // 不知道为啥会少 8 px
        var windowHeight      = ResolveMeasuredSize(constrainedHeight, _dialog.HostMinHeight, _dialog.HostMaxHeight);

        return new Size(windowWidth, windowHeight);
    }

    private static double ResolveAvailableMeasureSize(double ownerSize, double maxSize)
    {
        if (!double.IsNaN(maxSize) && !double.IsInfinity(maxSize))
        {
            return maxSize;
        }

        return ownerSize > 0 ? ownerSize : double.PositiveInfinity;
    }

    private static double ResolveMeasuredSize(double measuredSize, double minSize, double maxSize)
    {
        var size = measuredSize;
        if (double.IsNaN(size) || double.IsInfinity(size))
        {
            size = 0;
        }

        size = Math.Max(size, minSize);
        if (!double.IsNaN(maxSize) && !double.IsInfinity(maxSize))
        {
            size = Math.Min(size, maxSize);
        }

        return size;
    }

    public void UpdatePlacement()
    {
        if (_hostSize == default)
        {
            return;
        }

        var offset = _dialog.CalculatePlacementOffset(_hostSize, _ownerBounds.Size);
        var point  = _ownerBounds.TopLeft + offset;
        Position = new PixelPoint(
            (int)Math.Round(point.X * DesktopScaling),
            (int)Math.Round(point.Y * DesktopScaling));
    }

    protected override void ArrangeCore(Rect finalRect)
    {
        base.ArrangeCore(finalRect);
        _hostSize    = finalRect.Size;
        _ownerBounds = GetOwnerBounds(ParentTopLevel);
        if (!_hasAppliedInitialPlacement)
        {
            UpdatePlacement();
            _hasAppliedInitialPlacement = true;
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!e.IsProgrammatic || CloseByClickCloseCaptionButton)
        {
            e.Cancel = true;
            Dispatcher.Post(() =>
            {
                CloseByClickCloseCaptionButton = false;
                _dialog.NotifyDialogHostCloseRequest();
            });
        }
    }

    public void Close(Action? callback = null)
    {
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = null;
        base.Close();
        // 与 BindDialog 里 SetParent(dialog) 对称：释放 Dialog ↔ DialogHost 的 logical
        // parent/child 引用，避免关闭后双方互锁无法 GC。
        ((ISetLogicalParent)this).SetParent(null);
        callback?.Invoke();
    }

    private void BindContentProperties()
    {
        _dialogContent[!DialogWindowContent.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
        _dialogContent[!DialogWindowContent.IsLoadingProperty]             = this[!IsLoadingProperty];
        _dialogContent[!DialogWindowContent.IsConfirmLoadingProperty]      = this[!IsConfirmLoadingProperty];
        _dialogContent[!DialogWindowContent.IsFooterVisibleProperty]       = this[!IsFooterVisibleProperty];
        _dialogContent[!DialogWindowContent.StandardButtonsProperty]       = this[!StandardButtonsProperty];
        _dialogContent[!DialogWindowContent.DefaultStandardButtonProperty] = this[!DefaultStandardButtonProperty];
        _dialogContent[!DialogWindowContent.EscapeStandardButtonProperty]  = this[!EscapeStandardButtonProperty];
        _dialogContent[!DialogWindowContent.DialogContentTemplateProperty] = this[!DialogContentTemplateProperty];

        _dialogContent.ButtonBoxClicked += HandleButtonBoxClicked;
        _dialogContent.ButtonsSynchronized += HandleButtonsSynchronized;
    }

    private static Rect GetOwnerBounds(TopLevel topLevel)
    {
        if (topLevel is Window window)
        {
            return new Rect(window.Position.X / window.DesktopScaling,
                window.Position.Y / window.DesktopScaling,
                window.ClientSize.Width,
                window.ClientSize.Height);
        }

        return new Rect(default, topLevel.ClientSize);
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                _dialogContent.CustomButtons.AddRange(e.NewItems!.OfType<DialogButton>());
                break;
            case NotifyCollectionChangedAction.Remove:
                _dialogContent.CustomButtons.RemoveAll(e.OldItems!.OfType<DialogButton>());
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }
    }

    private void HandleButtonBoxClicked(object? sender, DialogButtonClickedEventArgs args)
    {
        _dialog.NotifyDialogButtonBoxClicked(args.SourceButton);
    }

    private void HandleButtonsSynchronized(object? sender, DialogBoxButtonSyncEventArgs args)
    {
        _dialog.NotifyDialogButtonSynchronized(args.Buttons);
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = new CompositeDisposable(args.Buttons.Count);
        foreach (var button in args.Buttons)
        {
            if (button.Role == DialogButtonRole.AcceptRole ||
                button.Role == DialogButtonRole.YesRole ||
                button.Role == DialogButtonRole.ApplyRole)
            {
                _confirmLoadingBindings.Add(button.Bind(Button.IsLoadingProperty,
                    this.GetObservable(IsConfirmLoadingProperty)));
            }
        }
    }
}
