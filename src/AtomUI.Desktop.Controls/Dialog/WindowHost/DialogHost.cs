using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal class DialogHost : Window,
                            IDialogHost,
                            IStyleHost,
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

    IStyleHost? IStyleHost.StylingParent => Parent;

    public TopLevel ParentTopLevel { get; }
    public AvaloniaList<DialogButton> CustomButtons { get; } = new();
    object? IDialogHost.Content
    {
        get => _dialogContent.DialogContent;
        set => _dialogContent.DialogContent = value as Control;
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
        this[!Window.CanMinimizeProperty]                 = dialog[!Dialog.EffectiveMinimizableProperty];
        this[!Window.CanMaximizeProperty]                 = dialog[!Dialog.IsMaximizableProperty];
        this[!Window.IsCloseCaptionButtonVisibleProperty] = dialog[!Dialog.IsClosableProperty];
        this[!Window.IsMoveEnabledProperty]               = dialog[!Dialog.IsDragMovableProperty];
    }

    public void AttachPlacement(Control placementTarget)
    {
        _ownerBounds = GetOwnerBounds(ParentTopLevel);
    }

    public void UpdateSizing()
    {
        Width     = _dialog.Width;
        MinWidth  = _dialog.MinWidth;
        MaxWidth  = _dialog.MaxWidth;
        Height    = _dialog.Height;
        MinHeight = _dialog.MinHeight;
        MaxHeight = _dialog.MaxHeight;
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
        UpdatePlacement();
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
        base.Close();
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
                button[!Button.IsLoadingProperty] = this[!IsConfirmLoadingProperty];
            }
        }
    }
}
