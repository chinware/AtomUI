using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class ImagePreviewRenderer : Control
{
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<ImagePreviewRenderer, IImage?>(nameof(Source));

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<ImagePreviewRenderer, Stretch>(nameof(Stretch), Stretch.Uniform);

    private IDisposable? _stretchBindingDisposable;
    private Image? _image;

    static ImagePreviewRenderer()
    {
        AffectsMeasure<ImagePreviewRenderer>(SourceProperty);
        SourceProperty.Changed.AddClassHandler<ImagePreviewRenderer>((control, args) => control.HandleSourceChanged(args));
    }

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public Size SourceSize => Source?.Size ?? default;

    private void HandleSourceChanged(AvaloniaPropertyChangedEventArgs args)
    {
        _stretchBindingDisposable?.Dispose();
        _stretchBindingDisposable = null;
        if (_image is not null)
        {
            LogicalChildren.Remove(_image);
            VisualChildren.Remove(_image);
            _image = null;
        }

        var source = args.GetNewValue<IImage?>();
        if (source is null)
        {
            return;
        }
        _image = new Image { Source = source };
        ((ISetLogicalParent)_image).SetParent(this);
        _stretchBindingDisposable = BindUtils.RelayBind(this, StretchProperty, _image, Image.StretchProperty);
        VisualChildren.Add(_image);
        LogicalChildren.Add(_image);
    }
}
