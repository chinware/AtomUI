using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

using SvgControl = Avalonia.Svg.Svg;

namespace AtomUI.Desktop.Controls;

internal class ImagePreviewRenderer : Control
{
    #region 公告属性定义

    public static readonly StyledProperty<PreviewImageSource?> SourceProperty =
        AvaloniaProperty.Register<ImagePreviewRenderer, PreviewImageSource?>(nameof(Source));
    
    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<ImagePreviewRenderer, Stretch>(nameof (Stretch), Stretch.Uniform);
    
    public PreviewImageSource? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public Size SourceSize => GetSourceOriginSize();
    
    #endregion
    
    private Control? _sourceControl;

    static ImagePreviewRenderer()
    {
        AffectsMeasure<ImagePreviewRenderer>(SourceProperty);
        SourceProperty.Changed.AddClassHandler<ImagePreviewRenderer>((x, e) => x.HandleSourceChanged(e));
    }
    
    private void HandleSourceChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newSource = (PreviewImageSource?)e.NewValue;

        ClearSourceControl();

        if (newSource != null)
        {
            if (newSource.IsSvg)
            {
                _sourceControl = new SvgControl(new Uri("https://atomui.net"))
                {
                    Source = newSource.SvgContent
                };
                _sourceControl[!SvgControl.StretchProperty] = this[!StretchProperty];
            }
            else
            {
                _sourceControl = new Image
                {
                    Source = newSource.Bitmap
                };
                _sourceControl[!Image.StretchProperty] = this[!StretchProperty];
            }
            VisualChildren.Add(_sourceControl);
            LogicalChildren.Add(_sourceControl);
        }
    }

    private void ClearSourceControl()
    {
        if (_sourceControl is null)
        {
            return;
        }

        VisualChildren.Remove(_sourceControl);
        LogicalChildren.Remove(_sourceControl);
        ((ISetLogicalParent)_sourceControl).SetParent(null);
        _sourceControl = null;
    }

    private Size GetSourceOriginSize()
    {
        Size size = default;
        if (_sourceControl is Image image)
        {
            size = image.Source?.Size ?? default;
        }
        else if (_sourceControl is SvgControl svgControl)
        {
            var picture = svgControl.Model;
            if (picture != null)
            {
                size = new Size(picture.CullRect.Width, picture.CullRect.Height);
            }
        }
        return size;
    }
}
