using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class PopupBuddyShadowRenderer : Border
{
    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Popup.MaskShadowsProperty.AddOwner<PopupBuddyShadowRenderer>();
    
    public BoxShadows MaskShadows
    {
        get => GetValue(MaskShadowsProperty);
        set => SetValue(MaskShadowsProperty, value);
    }
    
    private Panel? _shadowRendererLayout;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_shadowRendererLayout == null)
        {
            _shadowRendererLayout = new Panel();
            var shadows = BuildShadowRenderers(MaskShadows);
            _shadowRendererLayout.Children.AddRange(shadows);
        }
    }

    /// <summary>
    /// 目前的 Avalonia 版本中，当控件渲染到 RenderTargetBitmap 的时候，如果 BoxShadows 的 Count > 1 的时候，如果不是主阴影，后面的阴影如果
    /// 指定 offset，再 RenderScaling > 1 的情况下是错的。
    /// </summary>
    /// <returns></returns>
    private List<Control> BuildShadowRenderers(in BoxShadows shadows)
    {
        // 不知道这里为啥不行
        var renderers = new List<Control>();
        for (var i = 0; i < shadows.Count; ++i)
        {
            var renderer = new Border
            {
                BorderThickness = new Thickness(0),
                BoxShadow       = new BoxShadows(shadows[i]),
            };
            renderer[!CornerRadiusProperty] = this[!CornerRadiusProperty];
            renderers.Add(renderer);
        }

        return renderers;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree() && _shadowRendererLayout != null)
        {
            if (change.Property == MaskShadowsProperty)
            {
                for (var i = 0; i < MaskShadows.Count; ++i)
                {
                    if (_shadowRendererLayout.Children[i] is Border shadowControl)
                    {
                        shadowControl.BoxShadow = new BoxShadows(MaskShadows[i]);
                    }
                }
            }
        }
    }
}