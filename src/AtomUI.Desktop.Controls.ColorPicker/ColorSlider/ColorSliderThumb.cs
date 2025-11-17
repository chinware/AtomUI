using System.Diagnostics;
using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Utilities;
using Thumb = AtomUI.Controls.Primitives.Thumb;

namespace AtomUI.Controls;

internal class ColorSliderThumb : Thumb
{
    protected override Type StyleKeyOverride { get; } = typeof(ColorSliderThumb);
    
    public static readonly StyledProperty<IBrush?> ColorValueBrushProperty = 
        AvaloniaProperty.Register<ColorSliderThumb, IBrush?>(nameof (ColorValueBrush));
    
    public IBrush? ColorValueBrush
    {
        get => GetValue(ColorValueBrushProperty);
        set => SetValue(ColorValueBrushProperty, value);
    }
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<CornerRadius> InnerCornerRadiusProperty = 
        AvaloniaProperty.Register<ColorSliderThumb, CornerRadius>(nameof (InnerCornerRadius));
    
    internal CornerRadius InnerCornerRadius
    {
        get => GetValue(InnerCornerRadiusProperty);
        set => SetValue(InnerCornerRadiusProperty, value);
    }

    #endregion

    private Border? _innerEllipse;
    
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        Debug.Assert(MathUtilities.AreClose(e.NewSize.Width, e.NewSize.Height));
        CornerRadius = new CornerRadius(e.NewSize.Width / 2);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _innerEllipse = e.NameScope.Find<Border>(ColorSliderThumbThemeConstants.InnerEllipsePart);
        if (_innerEllipse != null)
        {
            _innerEllipse.SizeChanged += (sender, args) =>
            {
                InnerCornerRadius = new CornerRadius(args.NewSize.Width / 2);
            };
        }
    }
}