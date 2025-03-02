using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

public class ContextMenu : AvaloniaContextMenu,
                           IAnimationAwareControl,
                           IControlSharedTokenResourcesHost,
                           ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<ContextMenu>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<ContextMenu>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion
    
    private CompositeDisposable? _tokenBindingsDisposable;

    public ContextMenu()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
        // 我们在这里有一次初始化的机会
        var popup = new Popup
        {
            WindowManagerAddShadowHint     = false,
            IsLightDismissEnabled          = true,
            OverlayDismissEventPassThrough = true
        };
        BindUtils.RelayBind(this, IsMotionEnabledProperty, popup, Popup.IsMotionEnabledProperty);
        popup.Opened += this.CreateEventHandler("PopupOpened");
        popup.Closed += this.CreateEventHandler<EventArgs>("PopupClosed");

        popup.AddClosingEventHandler(this.CreateEventHandler<CancelEventArgs>("PopupClosing")!);
        popup.KeyUp += this.CreateEventHandler<KeyEventArgs>("PopupKeyUp");
        Closing += (sender, args) =>
        {
            args.Cancel = true;
        };
        this.SetPopup(popup);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
    
}