using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupBuddyDecorator : SceneMotionActorControl
{
    #region 公共属性定义

    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Popup.MaskShadowsProperty.AddOwner<PopupBuddyDecorator>();
    
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
            if (popupHostProvider.PopupHost != null)
            {
                SetupPopupHost(popupHostProvider.PopupHost);
            }
            else
            {
                popupHostProvider.PopupHostChanged += SetupPopupHost; 
            }
        }

        Child = _decoratorControl;
    }
    
    private void SetupPopupHost(IPopupHost? popupHost)
    {
        if (popupHost is PopupRoot popupRoot)
        {
            if (_popupHost is PopupRoot oldPopupRoot)
            {
                var oldPresenter = oldPopupRoot.Presenter;
                if (oldPresenter != null)
                {
                    oldPresenter.SizeChanged -= HandleBuddyPopupRootSizeChanged;
                }
            }
            var presenter = popupRoot.Presenter;
            if (presenter != null)
            {
                presenter.SizeChanged += HandleBuddyPopupRootSizeChanged;
            }
            ConfigureDecorator(presenter);
            CaptureContentControl();
        }
        _popupHost = popupHost;
    }

    private void ConfigureDecorator(ContentPresenter? presenter)
    {
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
        }
    }
    
    private void HandleBuddyPopupRootSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (sender is ContentPresenter presenter)
        {
            ConfigureDecorator(presenter);
            if (_popup.ConfigureBlankMaskWhenMotionAwareOpen)
            {
                ConfigureBlankContentControl();
            }
            else
            {
                CaptureContentControl();
            }
        }
    }
    
    protected override Point CalculateTopLevelGhostPosition()
    {
        var thickness = MaskShadows.Thickness();
        return new Point(thickness.Left, thickness.Top);
    }
    
    internal void CaptureContentControl()
    {
        if (_popupHost != null && _popupHost.Presenter != null)
        {
            var content   = _popupHost.Presenter.Child;
            if (content != null)
            {
                _decoratorControl.Content = new MotionTargetBitmapControl(content.CaptureCurrentBitmap())
                {
                    Width  = content.Bounds.Width,
                    Height = content.Bounds.Height,
                };
            }
        }
    }

    internal void ConfigureBlankContentControl()
    {
        if (_popupHost != null && _popupHost.Presenter != null)
        {
            var content   = _popupHost.Presenter.Child;
            if (content != null)
            {
                if (content is IShadowMaskInfoProvider shadowMaskInfoProvider)
                {
                    var maskBounds   = shadowMaskInfoProvider.GetMaskBounds();
                    var offset = maskBounds.Position;

                    var contentWidth = maskBounds.Width;
                    var contentHeight = maskBounds.Height;
                    var blankControl = new Canvas()
                    {
                        Background = Brushes.Transparent,
                        Width      = contentWidth + offset.X,
                        Height     = contentHeight + offset.Y,
                    };
                    var blankContent = new Border()
                    {
                        Background   = shadowMaskInfoProvider.GetMaskBackground(),
                        CornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius(),
                        Width        = contentWidth,
                        Height       = contentHeight,
                    };
                    blankControl.Children.Add(blankContent);
                    Canvas.SetLeft(blankContent, offset.X);
                    Canvas.SetTop(blankContent, offset.Y);
                    _decoratorControl.Content = blankControl;
                }
                else if (content is Border bordered)
                {
                    _decoratorControl.Content = new Border
                    {
                        Background   = bordered.Background,
                        CornerRadius = bordered.CornerRadius,
                        Width        = bordered.Bounds.Size.Width,
                        Height       = bordered.Bounds.Size.Height
                    };
                }
                else if (content is TemplatedControl templatedControl)
                {
                    _decoratorControl.Content = new Border
                    {
                        Background   = templatedControl.Background,
                        CornerRadius = templatedControl.CornerRadius,
                        Width        = templatedControl.Bounds.Size.Width,
                        Height       = templatedControl.Bounds.Size.Height
                    };
                }
            }
        }
    }
}