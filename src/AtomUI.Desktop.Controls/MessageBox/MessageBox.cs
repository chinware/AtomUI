using System.Collections.Specialized;
using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public enum MessageBoxOkButtonStyle
{
    Default,
    Primary
}

public class MessageBox : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<MessageBox, string?>(nameof (Title));

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<MessageBox, PathIcon?>(nameof (Icon));

    public static readonly StyledProperty<object?> ContentProperty = Dialog.ContentProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty = Dialog.ContentTemplateProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        AvaloniaProperty.Register<MessageBox, MessageBoxStyle>(nameof (Style), MessageBoxStyle.Information);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<DialogHostType> DialogHostTypeProperty =
        Dialog.DialogHostTypeProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<MessageBoxOkButtonStyle> OkButtonStyleProperty =
        AvaloniaProperty.Register<MessageBox, MessageBoxOkButtonStyle>(nameof(OkButtonStyle), MessageBoxOkButtonStyle.Primary);

    public static readonly StyledProperty<string?> OkButtonTextProperty = AvaloniaProperty.Register<MessageBox, string?>(nameof(OkButtonText));

    public static readonly StyledProperty<string?> CancelButtonTextProperty = AvaloniaProperty.Register<MessageBox, string?>(nameof(CancelButtonText));

    public static readonly StyledProperty<bool> IsLoadingProperty = Dialog.IsLoadingProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<bool> IsConfirmLoadingProperty = Dialog.IsConfirmLoadingProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<bool> IsOpenProperty =
        Dialog.IsOpenProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<bool> IsModalProperty =
        Dialog.IsModalProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<bool> IsDragMovableProperty =
        Dialog.IsDragMovableProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<object?> ResultProperty =
        Dialog.ResultProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<bool> IsCenterOnStartupProperty =
        AvaloniaProperty.Register<MessageBox, bool>(nameof(IsCenterOnStartup), true);

    public static readonly StyledProperty<Dimension?> HorizontalOffsetProperty =
        AvaloniaProperty.Register<MessageBox, Dimension?>(nameof(HorizontalOffset));

    public static readonly StyledProperty<Dimension?> VerticalOffsetProperty =
        AvaloniaProperty.Register<MessageBox, Dimension?>(nameof(VerticalOffset));

    public static readonly StyledProperty<Control?> PlacementTargetProperty =
        AvaloniaProperty.Register<MessageBox, Control?>(nameof(PlacementTarget));

    public static readonly StyledProperty<double> HostWidthProperty =
        Dialog.HostWidthProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<double> HostHeightProperty =
        Dialog.HostHeightProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<double> HostMinWidthProperty =
        Dialog.HostMinWidthProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<double> HostMinHeightProperty =
        Dialog.HostMinHeightProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<double> HostMaxWidthProperty =
        Dialog.HostMaxWidthProperty.AddOwner<MessageBox>();

    public static readonly StyledProperty<double> HostMaxHeightProperty =
        Dialog.HostMaxHeightProperty.AddOwner<MessageBox>();

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    [Content]
    [DependsOn(nameof(ContentTemplate))]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public DialogHostType DialogHostType
    {
        get => GetValue(DialogHostTypeProperty);
        set => SetValue(DialogHostTypeProperty, value);
    }

    public MessageBoxOkButtonStyle OkButtonStyle
    {
        get => GetValue(OkButtonStyleProperty);
        set => SetValue(OkButtonStyleProperty, value);
    }

    public string? OkButtonText
    {
        get => GetValue(OkButtonTextProperty);
        set => SetValue(OkButtonTextProperty, value);
    }

    public string? CancelButtonText
    {
        get => GetValue(CancelButtonTextProperty);
        set => SetValue(CancelButtonTextProperty, value);
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

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public bool IsDragMovable
    {
        get => GetValue(IsDragMovableProperty);
        set => SetValue(IsDragMovableProperty, value);
    }

    public object? Result
    {
        get => GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }

    public bool IsCenterOnStartup
    {
        get => GetValue(IsCenterOnStartupProperty);
        set => SetValue(IsCenterOnStartupProperty, value);
    }

    public Dimension? HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    public Dimension? VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }

    [ResolveByName]
    public Control? PlacementTarget
    {
        get => GetValue(PlacementTargetProperty);
        set => SetValue(PlacementTargetProperty, value);
    }

    public double HostWidth
    {
        get => GetValue(HostWidthProperty);
        set => SetValue(HostWidthProperty, value);
    }

    public double HostHeight
    {
        get => GetValue(HostHeightProperty);
        set => SetValue(HostHeightProperty, value);
    }

    public double HostMinWidth
    {
        get => GetValue(HostMinWidthProperty);
        set => SetValue(HostMinWidthProperty, value);
    }

    public double HostMinHeight
    {
        get => GetValue(HostMinHeightProperty);
        set => SetValue(HostMinHeightProperty, value);
    }

    public double HostMaxWidth
    {
        get => GetValue(HostMaxWidthProperty);
        set => SetValue(HostMaxWidthProperty, value);
    }

    public double HostMaxHeight
    {
        get => GetValue(HostMaxHeightProperty);
        set => SetValue(HostMaxHeightProperty, value);
    }

    private Action<IReadOnlyList<DialogButton>>? _buttonsConfigure;

    public Action<IReadOnlyList<DialogButton>>? ButtonsConfigure
    {
        get => _buttonsConfigure;
        set
        {
            _buttonsConfigure = value;
            if (_dialog != null)
            {
                _dialog.ButtonsConfigure = _buttonsConfigure;
            }
        }
    }

    public AvaloniaList<DialogButton> CustomButtons { get; } = new ();

    #endregion

    #region 公共事件定义
    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler? Cancelled;
    public event EventHandler? Confirmed;
    #endregion

    private Dialog? _dialog;
    private MessageBoxContent? _dialogContent;

    public MessageBox()
    {
        this.RegisterTokenResourceScope(MessageBoxToken.ScopeProvider);
        CustomButtons.CollectionChanged += HandleCustomButtonsChanged;
    }

    public object? Open()
    {
        Debug.Assert(_dialog != null);
        return _dialog.Open();
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        Debug.Assert(_dialog != null);
        await _dialog.OpenAsync(cancellationToken);
    }

    public void Cancel()
    {
        Debug.Assert(_dialog != null);
        _dialog.Reject();
    }

    public void Confirm()
    {
        Debug.Assert(_dialog != null);
        _dialog.Accept();
    }

    #region 静态 API

    public static object? ShowMessageBox<TView, TViewModel>(TViewModel? dataContext, MessageBoxOptions? options = null,
                                                            TopLevel? topLevel = null)
        where TView : Control, new()
    {
        return ShowMessageBox(new TView(), dataContext, options, topLevel);
    }

    public static object? ShowMessageBox(Control content, object? dataContext = null, MessageBoxOptions? options = null, TopLevel? topLevel = null)
    {
        var overlayLayer = ResolveOverlayLayer(options, topLevel);
        var messageBox   = CreateMessageBox(content, dataContext, options, overlayLayer);
        overlayLayer.Children.Add(messageBox);
        try
        {
            return messageBox.Open();
        }
        finally
        {
            overlayLayer.Children.Remove(messageBox);
        }
    }

    public static async Task ShowMessageBoxAsync<TView, TViewModel>(TViewModel? dataContext,
                                                                    MessageBoxOptions? options = null,
                                                                    Action<IMessageBoxActionResult>? closed = null,
                                                                    TopLevel? topLevel = null)
        where TView : Control, new()
    {
        await ShowMessageAsync(new TView(), dataContext, options, closed, topLevel);
    }

    public static async Task<object?> ShowMessageBoxModalAsync<TView, TViewModel>(TViewModel? dataContext, MessageBoxOptions? options = null, TopLevel? topLevel = null)
        where TView : Control, new()
    {
        return await ShowMessageModalAsync(new TView(), dataContext, options, topLevel);
    }

    public static async Task ShowMessageAsync(Control content,
                                              object? dataContext = null,
                                              MessageBoxOptions? options = null,
                                              Action<IMessageBoxActionResult>? closed = null,
                                              TopLevel? topLevel = null)
    {
        var overlayLayer = ResolveOverlayLayer(options, topLevel);
        var messageBox   = CreateMessageBox(content, dataContext, options, overlayLayer);

        messageBox.Closed += (_, _) =>
        {
            closed?.Invoke(new MessageBoxActionResult(messageBox.Result));
            overlayLayer.Children.Remove(messageBox);
        };
        overlayLayer.Children.Add(messageBox);
        try
        {
            await messageBox.Dispatcher.InvokeAsync(async () => await messageBox.OpenAsync());
        }
        catch
        {
            overlayLayer.Children.Remove(messageBox);
            throw;
        }
    }

    public static async Task<object?> ShowMessageModalAsync(Control content, object? dataContext = null, MessageBoxOptions? options = null, TopLevel? topLevel = null)
    {
        var overlayLayer = ResolveOverlayLayer(options, topLevel);
        var messageBox   = CreateMessageBox(content, dataContext, options, overlayLayer);
        messageBox.IsModal = true;
        overlayLayer.Children.Add(messageBox);
        try
        {
            await messageBox.Dispatcher.InvokeAsync(async () => await messageBox.OpenAsync());
            return messageBox.Result;
        }
        finally
        {
            overlayLayer.Children.Remove(messageBox);
        }
    }

    private static MessageBox CreateMessageBox(Control content, object? dataContext, MessageBoxOptions? options, Control placementTarget)
    {
        var messageBox = new MessageBox
        {
            Title             = options?.Title,
            IsDragMovable     = options?.IsDragMovable ?? false,
            Style             = options?.Style ?? MessageBoxStyle.Information,
            PlacementTarget   = options?.PlacementTarget ?? placementTarget,
            HorizontalOffset  = options?.HorizontalOffset,
            VerticalOffset    = options?.VerticalOffset,
            DialogHostType    = options?.HostType ?? DialogHostType.Overlay,
            IsCenterOnStartup = options?.IsCenterOnStartup ?? true,
            Content           = content,
            DataContext       = dataContext,
            HostWidth         = options?.Width ?? double.NaN,
            HostHeight        = options?.Height ?? double.NaN,
            HostMinWidth      = options?.MinWidth ?? 0d,
            HostMinHeight     = options?.MinHeight ?? 0d,
            HostMaxWidth      = options?.MaxWidth ?? double.PositiveInfinity,
            HostMaxHeight     = options?.MaxHeight ?? double.PositiveInfinity,
            IsConfirmLoading  = options?.IsConfirmLoading ?? false,
            IsLoading         = options?.IsLoading ?? false,
        };
        if (options?.Icon != null)
        {
            messageBox.Icon = options.Icon;
        }
        return messageBox;
    }

    private static Panel ResolveOverlayLayer(MessageBoxOptions? options, TopLevel? topLevel)
    {
        Visual? anchor = options?.PlacementTarget ?? topLevel ?? Window.GetMainWindow();
        if (anchor is null)
        {
            throw new InvalidOperationException("Unable to resolve TopLevel for MessageBox.");
        }

        if (anchor.GetPopupOverlayLayer() is not Panel overlayLayer)
        {
            throw new InvalidOperationException(
                "Unable to resolve overlay layer for MessageBox; ensure the TopLevel is an atom:Window.");
        }

        return overlayLayer;
    }

    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == StyleProperty)
            {
                ConfigureIcon();
            }
            else if (change.Property == OkButtonStyleProperty)
            {
                ConfigureOkButton();
            }
            else if (change.Property == IsCenterOnStartupProperty)
            {
                ConfigurePositionOnStartup();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_dialog != null)
        {
            _dialog.Opened   -= HandleDialogOpened;
            _dialog.Closed   -= HandleDialogClosed;
            _dialog.Rejected -= HandleDialogCancelled;
            _dialog.Accepted -= HandleDialogConfirmed;
            _dialog.Finished -= HandleDialogFinished;
        }
        _dialog = e.NameScope.Find<Dialog>("PART_Dialog");
        if (_dialog != null)
        {
            _dialogContent = new MessageBoxContent();
            BindUtils.RelayBind(this, IconProperty, _dialogContent, MessageBoxContent.StyleIconProperty);
            BindUtils.RelayBind(this, StyleProperty, _dialogContent, MessageBoxContent.StyleProperty);
            BindUtils.RelayBind(this, ContentProperty, _dialogContent, ContentControl.ContentProperty);
            BindUtils.RelayBind(this, ContentTemplateProperty, _dialogContent, ContentControl.ContentTemplateProperty);
            _dialog.Content          =  _dialogContent;
            _dialog.Opened           += HandleDialogOpened;
            _dialog.Closed           += HandleDialogClosed;
            _dialog.Rejected         += HandleDialogCancelled;
            _dialog.Accepted         += HandleDialogConfirmed;
            _dialog.Finished         += HandleDialogFinished;
            _dialog.ButtonsConfigure =  ButtonsConfigure;
            _dialog.CustomButtons.AddRange(CustomButtons);
        }

        ConfigureOkButton();
        ConfigureIcon();
        ConfigurePositionOnStartup();
    }

    private void HandleDialogOpened(object? sender, EventArgs e)
    {
        Opened?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDialogClosed(object? sender, EventArgs e)
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDialogCancelled(object? sender, EventArgs e)
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDialogConfirmed(object? sender, EventArgs e)
    {
        Confirmed?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDialogFinished(object? sender, DialogFinishedEventArgs e)
    {
        SetCurrentValue(ResultProperty, e.Result);
    }

    private void ConfigureIcon()
    {
        Debug.Assert(_dialog != null);
        if (Style == MessageBoxStyle.Information)
        {
            SetValue(IconProperty, new InfoCircleFilled(), BindingPriority.Template);
            _dialog.StandardButtons = DialogStandardButton.Ok;
        }
        else if (Style == MessageBoxStyle.Success)
        {
            SetValue(IconProperty, new CheckCircleFilled(), BindingPriority.Template);
            _dialog.StandardButtons = DialogStandardButton.Ok;
        }
        else if (Style == MessageBoxStyle.Error)
        {
            SetValue(IconProperty, new CloseCircleFilled(), BindingPriority.Template);
            _dialog.StandardButtons = DialogStandardButton.Ok;
        }
        else if (Style == MessageBoxStyle.Warning)
        {
            SetValue(IconProperty, new ExclamationCircleFilled(), BindingPriority.Template);
            _dialog.StandardButtons = DialogStandardButton.Ok;
        }
        else if (Style == MessageBoxStyle.Normal)
        {
            _dialog.StandardButtons = DialogStandardButton.Ok;
        }
        else if (Style == MessageBoxStyle.Confirm)
        {
            SetValue(IconProperty, new ExclamationCircleFilled(), BindingPriority.Template);
            _dialog.StandardButtons = DialogStandardButton.Ok | DialogStandardButton.Cancel;
        }
    }

    private void ConfigureOkButton()
    {
        Debug.Assert(_dialog != null);
        if (OkButtonStyle == MessageBoxOkButtonStyle.Primary)
        {
            _dialog.DefaultStandardButton = DialogStandardButton.Ok;
        }
        else
        {
            _dialog.DefaultStandardButton = DialogStandardButton.NoButton;
        }
    }

    private void ConfigurePositionOnStartup()
    {
        if (_dialog != null)
        {
            if (IsCenterOnStartup)
            {
                _dialog.HorizontalStartupLocation = DialogHorizontalAnchor.Center;
                _dialog.VerticalStartupLocation   = DialogVerticalAnchor.Center;
            }
            else
            {
                _dialog.HorizontalStartupLocation = DialogHorizontalAnchor.Custom;
                _dialog.VerticalStartupLocation   = DialogVerticalAnchor.Custom;
            }
        }
    }

    protected override Size MeasureCore(Size availableSize)
    {
        if (IsVisible)
        {
            ApplyStyling();
            ApplyTemplate();
        }

        return new Size();
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_dialog != null)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = e.NewItems!.OfType<DialogButton>();
                    _dialog.CustomButtons.AddRange(newItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldItems = e.OldItems!.OfType<DialogButton>();
                    _dialog.CustomButtons.RemoveAll(oldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();
            }
        }
    }
}
