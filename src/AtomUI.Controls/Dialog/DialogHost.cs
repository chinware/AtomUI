using System.Collections.Specialized;
using AtomUI.Controls.DialogPositioning;
using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class DialogHost : Window,
                            IDialogHost,
                            IHostedVisualTreeRoot,
                            IStyleHost,
                            IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<Transform?> TransformProperty =
        AvaloniaProperty.Register<DialogHost, Transform?>(nameof(Transform));
    
    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        DialogButtonBox.EscapeStandardButtonProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<bool> IsFooterVisibleProperty =
        Dialog.IsFooterVisibleProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DialogHost>();
        
    public static readonly StyledProperty<bool> IsLoadingProperty = Dialog.IsLoadingProperty.AddOwner<DialogHost>();
    public static readonly StyledProperty<bool> IsConfirmLoadingProperty = Dialog.IsConfirmLoadingProperty.AddOwner<DialogHost>();
    
    public Transform? Transform
    {
        get => GetValue(TransformProperty);
        set => SetValue(TransformProperty, value);
    }
    
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
    
    Visual? IHostedVisualTreeRoot.Host
    {
        get
        {
            // If the parent is attached to a visual tree, then return that. However the parent
            // will possibly be a standalone Popup (i.e. a Popup not attached to a visual tree,
            // created by e.g. a ContextMenu): if this is the case, return the ParentTopLevel
            // if set. This helps to allow the focus manager to restore the focus to the outer
            // scope when the popup is closed.
            var parentVisual = Parent as Visual;
            if (parentVisual?.IsAttachedToVisualTree() == true)
            {
                return parentVisual;
            }
            return ParentTopLevel ?? parentVisual;
        }
    }
    
    IStyleHost? IStyleHost.StylingParent => Parent;
    
    public TopLevel ParentTopLevel { get; }

    Visual IDialogHost.HostedVisualTreeRoot => this;
    
    public AvaloniaList<DialogButton> CustomButtons { get; } = new ();
    
    #endregion
    
    #region 内部属性定义
    
    public static readonly DirectProperty<DialogHost, bool> IsEffectiveFooterVisibleProperty =
        AvaloniaProperty.RegisterDirect<DialogHost, bool>(
            nameof(IsEffectiveFooterVisible),
            o => o.IsEffectiveFooterVisible,
            (o, v) => o.IsEffectiveFooterVisible = v);
    
    private bool _isEffectiveFooterVisible;

    public bool IsEffectiveFooterVisible
    {
        get => _isEffectiveFooterVisible;
        set => SetAndRaise(IsEffectiveFooterVisibleProperty, ref _isEffectiveFooterVisible, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    
    #endregion
    
    protected override Type StyleKeyOverride { get; } = typeof(DialogHost);
    
    private DialogPositionRequest? _dialogPositionRequest;
    private Size _dialogSize;
    private bool _needsUpdate;
    private readonly ManagedDialogPositioner _positioner;
    private Dialog _dialog;
    private ManagedDialogPositionerDialogImplHelper _positionerHelper;
    private PixelPoint _latestDialogPosition;
    private DialogButtonBox? _buttonBox;
    
    public DialogHost(TopLevel parent, Dialog dialog)
    {
        ParentTopLevel    = parent;
        _positionerHelper = new ManagedDialogPositionerDialogImplHelper(PlatformImpl!, MoveResize);
        _positioner       = new ManagedDialogPositioner(_positionerHelper);
        _dialog           = dialog;
#if DEBUG
        this.AttachDevTools();
#endif
        CustomButtons.CollectionChanged += new NotifyCollectionChangedEventHandler(HandleCustomButtonsChanged);
    }
    
    public void SetChild(Control? control) => Content = control;

    void IDialogHost.ConfigurePosition(DialogPositionRequest request)
    {
        _dialogPositionRequest = request;
        _needsUpdate           = true;
        UpdatePosition();
    }

    protected override void ArrangeCore(Rect finalRect)
    {
        if (_dialogSize != finalRect.Size)
        {
            _dialogSize  = finalRect.Size;
            _dialog.NotifyDialogHostMeasured(_dialogSize, _positionerHelper.ClientAreaScreenGeometry);
            _needsUpdate = true;
            UpdatePosition();
        }
        
        base.ArrangeCore(finalRect);
    }
    
    private void UpdatePosition()
    {
        if (_needsUpdate && _dialogPositionRequest is not null)
        {
            _needsUpdate = false;
            _positioner.Update(_dialogPositionRequest, _dialogSize);
        }
    }
    
    private void MoveResize(PixelPoint position, Size size, double scaling)
    {
        if (WindowState == WindowState.Normal)
        {
            if (_latestDialogPosition != position)
            {
                _latestDialogPosition = position;
                Position              = position;
            }
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!e.IsProgrammatic)
        {
            e.Cancel = true;
            Dispatcher.UIThread.Post(() =>
            {
                _dialog.NotifyDialogHostCloseRequest();
            });
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _buttonBox = e.NameScope.Find<DialogButtonBox>(DialogThemeConstants.ButtonBoxPart);
        if (_buttonBox != null)
        {
            _buttonBox.CustomButtons.AddRange(CustomButtons);
            _buttonBox.Clicked             += HandleButtonBoxClicked;
            _buttonBox.ButtonsSynchronized += HandleButtonsSynchronized;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StandardButtonsProperty ||
            change.Property == IsLoadingProperty)
        {
            ConfigureEffectiveFooterVisible();
        }
    }

    private void ConfigureEffectiveFooterVisible()
    {
        if (IsFooterVisible)
        {
            if (!IsLoading)
            {
                SetCurrentValue(IsEffectiveFooterVisibleProperty, StandardButtons.Count > 0 || CustomButtons.Count > 0);
            }
            else
            {
                SetCurrentValue(IsEffectiveFooterVisibleProperty, false);
            }
        }
        else
        {
            SetCurrentValue(IsEffectiveFooterVisibleProperty, false);
        }
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_buttonBox != null)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = e.NewItems!.OfType<DialogButton>();
                    _buttonBox.CustomButtons.AddRange(newItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldItems = e.OldItems!.OfType<DialogButton>();
                    _buttonBox.CustomButtons.RemoveAll(oldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();
            }
        }

        ConfigureEffectiveFooterVisible();
    }

    private void HandleButtonBoxClicked(object? sender, DialogButtonClickedEventArgs args)
    {
        _dialog.NotifyDialogButtonBoxClicked(args.SourceButton);
    }

    private void HandleButtonsSynchronized(object? sender, DialogBoxButtonSyncEventArgs args)
    {
        _dialog.NotifyDialogButtonSynchronized(args.Buttons);
    }
}