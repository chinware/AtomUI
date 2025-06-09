using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupBuddyDecorator : SceneMotionActorControl
{
    #region 公共属性定义

    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Border.BoxShadowProperty.AddOwner<PopupBuddyDecorator>();
    
    public BoxShadows MaskShadows
    {
        get => GetValue(MaskShadowsProperty);
        set => SetValue(MaskShadowsProperty, value);
    }
    
    #endregion
    
    private Popup _popup;
    private IPopupHost? _popupHost;
    private MotionGhostControl _decoratorControl;
    
    public PopupBuddyDecorator(Popup popup)
    {
        _popup            = popup;
        _decoratorControl = new MotionGhostControl();
        BindUtils.RelayBind(this, MaskShadowsProperty, _decoratorControl, MotionGhostControl.MaskShadowsProperty);
        if (_popup is IPopupHostProvider popupHostProvider)
        {
            if (popupHostProvider.PopupHost == null)
            {
                popupHostProvider.PopupHostChanged += host =>
                {
                    _popupHost = host;
                    if (_popupHost != null)
                    {
                        ConfigureDecorator(_popupHost);
                    }
                };
            }
            else
            {
                _popupHost = popupHostProvider.PopupHost;
                if (_popupHost != null)
                {
                    ConfigureDecorator(_popupHost);
                }
            }
        }

        Child = _decoratorControl;
    }

    private void ConfigureDecorator(IPopupHost popupHost)
    {
        var presenter = popupHost.Presenter;
        if (presenter != null)
        {
            var content = presenter.Child;
            if (content is IShadowMaskInfoProvider shadowMaskInfoProvider)
            {
                _decoratorControl.MaskCornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
                var maskBounds   = shadowMaskInfoProvider.GetMaskBounds();
                _decoratorControl.MaskOffset = maskBounds.Position;
                _decoratorControl.MaskSize   = new Size(maskBounds.Width, maskBounds.Height);
            }
            else if (content is Border bordered)
            {
                _decoratorControl.MaskCornerRadius = bordered.CornerRadius;
                _decoratorControl.MaskSize         = bordered.Bounds.Size;
            }
            else if (content is TemplatedControl templatedControl)
            {
                _decoratorControl.MaskCornerRadius = templatedControl.CornerRadius;
                _decoratorControl.MaskSize         = templatedControl.Bounds.Size;
            }
            if (content != null)
            {
                _decoratorControl.Content = BuildDecoratorControlContent(content);
            }
        }
    }
    
    protected override Point CalculateTopLevelGhostPosition()
    {
        var thickness = MaskShadows.Thickness();
        return new Point(thickness.Left, thickness.Top);
    }

    private Control BuildDecoratorControlContent(Control control)
    {
        return new MotionTargetBitmapControl(control.CaptureCurrentBitmap())
        {
            Width  = control.Bounds.Width,
            Height = control.Bounds.Height,
        };
    }

    internal void HideDecoratorContent()
    {
        if (_decoratorControl.Content != null)
        {
            _decoratorControl.Content.Opacity = 0.0;
        }
    }

    internal void ShowDecoratorContent()
    {
        if (_decoratorControl.Content != null)
        {
            _decoratorControl.Content.Opacity = 1.0;
        }
    }
}