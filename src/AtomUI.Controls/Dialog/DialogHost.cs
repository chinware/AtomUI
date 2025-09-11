using AtomUI.Controls.DialogPositioning;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public sealed class DialogHost : Window,
                                 IDialogHost,
                                 IHostedVisualTreeRoot,
                                 IStyleHost,
                                 IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<Transform?> TransformProperty =
        AvaloniaProperty.Register<DialogHost, Transform?>(nameof(Transform));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DialogHost>();
    
    public Transform? Transform
    {
        get => GetValue(TransformProperty);
        set => SetValue(TransformProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
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
    
    #endregion
    
    #region 内部属性定义
    Control IMotionAwareControl.PropertyBindTarget => this;
    #endregion
    
    protected override Type StyleKeyOverride { get; } = typeof(DialogHost);
    
    private DialogPositionRequest? _dialogPositionRequest;
    private Size _dialogSize;
    private bool _needsUpdate;
    private readonly ManagedDialogPositioner _positioner;
    private Dialog _dialog;
    
    public DialogHost(TopLevel parent, Dialog dialog)
    {
        ParentTopLevel = parent;
        _positioner    = new ManagedDialogPositioner(new ManagedDialogPositionerDialogImplHelper(PlatformImpl!,MoveResize));
        _dialog =  dialog;
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
        // Move(position);
        // _scalingOverride = scaling;
        // UpdateScaling(true);
        // Resize(size, true, WindowResizeReason.Layout);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!e.IsProgrammatic)
        {
            e.Cancel = true;
            _dialog.NotifyDialogHostCloseRequest();
        }
    }
}