using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogSurfaceCloseRequestedEventArgs : EventArgs
{
    internal object? Result { get; }
    internal DialogCloseReason Reason { get; }
    internal DialogButton SourceButton { get; }

    internal DialogSurfaceCloseRequestedEventArgs(
        object? result,
        DialogCloseReason reason,
        DialogButton sourceButton)
    {
        Result = result;
        Reason = reason;
        SourceButton = sourceButton;
    }
}

internal sealed class DialogSurface : ContentControl, IDisposable
{
    internal static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<DialogSurface, string?>(nameof(Title));

    internal static readonly StyledProperty<PathIcon?> TitleIconProperty =
        AvaloniaProperty.Register<DialogSurface, PathIcon?>(nameof(TitleIcon));

    internal static readonly StyledProperty<bool> IsHeaderVisibleProperty =
        AvaloniaProperty.Register<DialogSurface, bool>(nameof(IsHeaderVisible), true);

    internal static readonly StyledProperty<bool> IsClosableProperty =
        Dialog.IsClosableProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<bool> IsMaximizableProperty =
        Dialog.IsMaximizableProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<bool> IsResizableProperty =
        Dialog.IsResizableProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        Dialog.IsMotionEnabledProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        AvaloniaProperty.Register<DialogSurface, TimeSpan>(nameof(MotionDuration));

    internal static readonly StyledProperty<bool> IsLoadingProperty =
        Dialog.IsLoadingProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<bool> IsConfirmLoadingProperty =
        Dialog.IsConfirmLoadingProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<bool> IsFooterVisibleProperty =
        Dialog.IsFooterVisibleProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        Dialog.StandardButtonsProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        Dialog.DefaultStandardButtonProperty.AddOwner<DialogSurface>();

    internal static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        Dialog.EscapeStandardButtonProperty.AddOwner<DialogSurface>();

    internal static readonly DirectProperty<DialogSurface, bool> IsEffectiveFooterVisibleProperty =
        AvaloniaProperty.RegisterDirect<DialogSurface, bool>(
            nameof(IsEffectiveFooterVisible),
            surface => surface.IsEffectiveFooterVisible);

    internal static readonly DirectProperty<DialogSurface, bool> IsDialogMaximizedProperty =
        AvaloniaProperty.RegisterDirect<DialogSurface, bool>(
            nameof(IsDialogMaximized),
            surface => surface.IsDialogMaximized);

    internal string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    internal PathIcon? TitleIcon
    {
        get => GetValue(TitleIconProperty);
        set => SetValue(TitleIconProperty, value);
    }

    internal bool IsHeaderVisible
    {
        get => GetValue(IsHeaderVisibleProperty);
        set => SetValue(IsHeaderVisibleProperty, value);
    }

    internal bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    internal bool IsMaximizable
    {
        get => GetValue(IsMaximizableProperty);
        set => SetValue(IsMaximizableProperty, value);
    }

    internal bool IsResizable
    {
        get => GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    internal bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    internal bool IsConfirmLoading
    {
        get => GetValue(IsConfirmLoadingProperty);
        set => SetValue(IsConfirmLoadingProperty, value);
    }

    internal bool IsFooterVisible
    {
        get => GetValue(IsFooterVisibleProperty);
        set => SetValue(IsFooterVisibleProperty, value);
    }

    internal DialogStandardButtons StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }

    internal DialogStandardButton DefaultStandardButton
    {
        get => GetValue(DefaultStandardButtonProperty);
        set => SetValue(DefaultStandardButtonProperty, value);
    }

    internal DialogStandardButton EscapeStandardButton
    {
        get => GetValue(EscapeStandardButtonProperty);
        set => SetValue(EscapeStandardButtonProperty, value);
    }

    private bool _isEffectiveFooterVisible;

    internal bool IsEffectiveFooterVisible
    {
        get => _isEffectiveFooterVisible;
        private set => SetAndRaise(IsEffectiveFooterVisibleProperty, ref _isEffectiveFooterVisible, value);
    }

    private bool _isDialogMaximized;

    internal bool IsDialogMaximized
    {
        get => _isDialogMaximized;
        set => SetAndRaise(IsDialogMaximizedProperty, ref _isDialogMaximized, value);
    }

    private readonly Dialog _dialog;
    private readonly CompositeDisposable _bindings = new();
    private CompositeDisposable? _confirmLoadingBindings;
    private IReadOnlyList<DialogButton> _synchronizedButtons = Array.Empty<DialogButton>();

    internal DialogButtonBox? ButtonBox { get; private set; }
    internal OverlayDialogHeader? Header { get; private set; }
    internal OverlayDialogResizer? Resizer { get; private set; }
    internal ShadowsAwareContainer? ShadowHost { get; private set; }

    internal event EventHandler<DialogSurfaceCloseRequestedEventArgs>? CloseRequested;
    internal event EventHandler? HostCloseRequested;
    internal event EventHandler? MaximizeRequested;
    internal event EventHandler? RestoreRequested;
    internal event EventHandler<OverlayDialogResizeEventArgs>? ResizeRequested;
    internal event EventHandler<PointerPressedEventArgs>? HeaderPointerPressed;
    internal event EventHandler<PointerEventArgs>? HeaderPointerMoved;
    internal event EventHandler<PointerReleasedEventArgs>? HeaderPointerReleased;

    internal DialogSurface(Dialog dialog)
    {
        _dialog = dialog;
        Focusable = true;
        _bindings.Add(Bind(
            ContentProperty,
            dialog.GetObservable(Dialog.ContentProperty).Select(_ => dialog.GetSurfaceContent())));
        _bindings.Add(Bind(
            ContentTemplateProperty,
            dialog.GetObservable(Dialog.ContentTemplateProperty).Select(_ => dialog.GetSurfaceContentTemplate())));
        _bindings.Add(Bind(TitleProperty, dialog.GetObservable(Dialog.TitleProperty)));
        _bindings.Add(Bind(TitleIconProperty, dialog.GetObservable(Dialog.TitleIconProperty)));
        _bindings.Add(Bind(IsClosableProperty, dialog.GetObservable(Dialog.IsClosableProperty)));
        _bindings.Add(Bind(IsMaximizableProperty, dialog.GetObservable(Dialog.IsMaximizableProperty)));
        _bindings.Add(Bind(IsResizableProperty, dialog.GetObservable(Dialog.IsResizableProperty)));
        _bindings.Add(Bind(IsMotionEnabledProperty, dialog.GetObservable(Dialog.IsMotionEnabledProperty)));
        _bindings.Add(Bind(IsLoadingProperty, dialog.GetObservable(Dialog.IsLoadingProperty)));
        _bindings.Add(Bind(IsConfirmLoadingProperty, dialog.GetObservable(Dialog.IsConfirmLoadingProperty)));
        _bindings.Add(Bind(IsFooterVisibleProperty, dialog.GetObservable(Dialog.IsFooterVisibleProperty)));
        _bindings.Add(Bind(StandardButtonsProperty, dialog.GetObservable(Dialog.StandardButtonsProperty)));
        _bindings.Add(Bind(DefaultStandardButtonProperty, dialog.GetObservable(Dialog.DefaultStandardButtonProperty)));
        _bindings.Add(Bind(EscapeStandardButtonProperty, dialog.GetObservable(Dialog.EscapeStandardButtonProperty)));
        _bindings.Add(this.GetObservable(IsLoadingProperty).Subscribe(_ => UpdateEffectiveFooterVisibility()));
        _bindings.Add(this.GetObservable(IsFooterVisibleProperty).Subscribe(_ => UpdateEffectiveFooterVisibility()));
        _bindings.Add(this.GetObservable(StandardButtonsProperty).Subscribe(_ => UpdateEffectiveFooterVisibility()));
        dialog.CustomButtons.CollectionChanged += HandleCustomButtonsChanged;
        _bindings.Add(Disposable.Create(() => dialog.CustomButtons.CollectionChanged -= HandleCustomButtonsChanged));
        UpdateEffectiveFooterVisibility();
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SynchronizeCustomButtons();
        UpdateEffectiveFooterVisibility();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ReleaseButtonBox();
        ReleaseHeader();
        ReleaseResizer();
        ShadowHost = null;

        ButtonBox = e.NameScope.Find<DialogButtonBox>("PART_ButtonBox");
        Header = e.NameScope.Find<OverlayDialogHeader>("PART_Header");
        Resizer = e.NameScope.Find<OverlayDialogResizer>("PART_Resizer");
        ShadowHost = e.NameScope.Find<ShadowsAwareContainer>("PART_ShadowHost");
        if (ButtonBox is not null)
        {
            ButtonBox.Clicked += HandleButtonClicked;
            ButtonBox.EffectiveButtonsChanged += HandleEffectiveButtonsChanged;
            SynchronizeCustomButtons();
        }

        if (Header is not null)
        {
            Header.CloseRequest += HandleHeaderCloseRequest;
            Header.MaximizeRequest += HandleHeaderMaximizeRequest;
            Header.NormalizeRequest += HandleHeaderNormalizeRequest;
            Header.PointerPressed += HandleHeaderPointerPressed;
            Header.PointerMoved += HandleHeaderPointerMoved;
            Header.PointerReleased += HandleHeaderPointerReleased;
        }
        if (Resizer is not null)
        {
            Resizer.ResizeRequest += HandleResizeRequest;
        }
    }

    private void SynchronizeCustomButtons()
    {
        if (ButtonBox is null)
        {
            return;
        }

        ButtonBox.CustomButtons.Clear();
        ButtonBox.CustomButtons.AddRange(_dialog.CustomButtons);
    }

    private void HandleButtonClicked(object? sender, DialogButtonClickedEventArgs e)
    {
        if (!_dialog.NotifySurfaceButtonClicked(e.SourceButton))
        {
            return;
        }

        if (e.SourceButton.Role is DialogButtonRole.AcceptRole or
            DialogButtonRole.YesRole or
            DialogButtonRole.ApplyRole or
            DialogButtonRole.ResetRole)
        {
            CloseRequested?.Invoke(this,
                new DialogSurfaceCloseRequestedEventArgs(
                    DialogCode.Accepted,
                    DialogCloseReason.Accepted,
                    e.SourceButton));
        }
        else if (e.SourceButton.Role is DialogButtonRole.RejectRole or DialogButtonRole.NoRole)
        {
            CloseRequested?.Invoke(this,
                new DialogSurfaceCloseRequestedEventArgs(
                    DialogCode.Rejected,
                    DialogCloseReason.Rejected,
                    e.SourceButton));
        }
    }

    private void HandleHeaderCloseRequest(object? sender, EventArgs e)
    {
        HostCloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void HandleHeaderMaximizeRequest(object? sender, EventArgs e)
    {
        MaximizeRequested?.Invoke(this, EventArgs.Empty);
    }

    private void HandleHeaderNormalizeRequest(object? sender, EventArgs e)
    {
        RestoreRequested?.Invoke(this, EventArgs.Empty);
    }

    private void HandleResizeRequest(object? sender, OverlayDialogResizeEventArgs e)
    {
        ResizeRequested?.Invoke(this, e);
    }

    private void HandleHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        HeaderPointerPressed?.Invoke(this, e);
    }

    private void HandleHeaderPointerMoved(object? sender, PointerEventArgs e)
    {
        HeaderPointerMoved?.Invoke(this, e);
    }

    private void HandleHeaderPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        HeaderPointerReleased?.Invoke(this, e);
    }

    private void HandleEffectiveButtonsChanged(object? sender, EventArgs e)
    {
        if (ButtonBox is null)
        {
            return;
        }

        _synchronizedButtons = ButtonBox.EffectiveButtons.ToArray();
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = new CompositeDisposable(_synchronizedButtons.Count);
        foreach (var button in _synchronizedButtons)
        {
            if (button.Role is DialogButtonRole.AcceptRole or DialogButtonRole.YesRole or DialogButtonRole.ApplyRole)
            {
                _confirmLoadingBindings.Add(
                    button.Bind(Button.IsLoadingProperty, this.GetObservable(IsConfirmLoadingProperty)));
            }
        }

        _dialog.ConfigureSurfaceButtons(_synchronizedButtons);
    }

    internal bool TryInvokeStandardButton(Key key)
    {
        var standardButton = key switch
        {
            Key.Enter => DefaultStandardButton,
            Key.Escape => ResolveEscapeStandardButton(),
            _ => DialogStandardButton.NoButton
        };
        if (standardButton == DialogStandardButton.NoButton)
        {
            return false;
        }

        var button = _synchronizedButtons.FirstOrDefault(candidate =>
            candidate.StandardButtonType == standardButton && candidate.IsEffectivelyEnabled);
        if (button is null)
        {
            return false;
        }

        HandleButtonClicked(this, new DialogButtonClickedEventArgs(button));
        return true;
    }

    internal void DisconnectCompositionChildren()
    {
        foreach (var visual in this.GetVisualDescendants().Prepend(this))
        {
            if (ElementComposition.GetElementVisual(visual) is CompositionContainerVisual compositionVisual)
            {
                compositionVisual.Children.RemoveAll();
            }
        }
    }

    private DialogStandardButton ResolveEscapeStandardButton()
    {
        if (_dialog.IsSet(Dialog.EscapeStandardButtonProperty))
        {
            return EscapeStandardButton;
        }

        return StandardButtons.HasFlag(DialogStandardButton.Cancel)
            ? DialogStandardButton.Cancel
            : DialogStandardButton.NoButton;
    }

    private void UpdateEffectiveFooterVisibility()
    {
        IsEffectiveFooterVisible = IsFooterVisible &&
                                   !IsLoading &&
                                   (StandardButtons.Count > 0 || _dialog.CustomButtons.Count > 0);
    }

    public void Dispose()
    {
        ReleaseButtonBox();
        ReleaseHeader();
        ReleaseResizer();
        ShadowHost = null;
        _bindings.Dispose();
    }

    private void ReleaseButtonBox()
    {
        _dialog.ReleaseSurfaceButtons(_synchronizedButtons);
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = null;
        _synchronizedButtons = Array.Empty<DialogButton>();
        if (ButtonBox is null)
        {
            return;
        }

        ButtonBox.Clicked -= HandleButtonClicked;
        ButtonBox.EffectiveButtonsChanged -= HandleEffectiveButtonsChanged;
        ButtonBox.CustomButtons.Clear();
        ButtonBox.ReleaseButtons();
        ButtonBox = null;
    }

    private void ReleaseHeader()
    {
        if (Header is null)
        {
            return;
        }

        Header.CloseRequest -= HandleHeaderCloseRequest;
        Header.MaximizeRequest -= HandleHeaderMaximizeRequest;
        Header.NormalizeRequest -= HandleHeaderNormalizeRequest;
        Header.PointerPressed -= HandleHeaderPointerPressed;
        Header.PointerMoved -= HandleHeaderPointerMoved;
        Header.PointerReleased -= HandleHeaderPointerReleased;
        Header = null;
    }

    private void ReleaseResizer()
    {
        if (Resizer is null)
        {
            return;
        }

        Resizer.ResizeRequest -= HandleResizeRequest;
        Resizer = null;
    }
}
