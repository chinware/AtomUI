using AtomUI.Demo.Desktop.ViewModels;
using AtomUI.Icon;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using CommunityToolkit.Mvvm.Messaging;

namespace AtomUI.Demo.Desktop.Controls;

public class IconGallery : TemplatedControl
{
    private bool _initialized;
    private readonly IconGalleryModel _galleryModel;

    public static readonly StyledProperty<IconThemeType?> IconThemeTypeProperty =
        AvaloniaProperty.Register<IconInfoItem, IconThemeType?>(
            nameof(IconThemeType));

    public IconThemeType? IconThemeType
    {
        get => GetValue(IconThemeTypeProperty);
        set => SetValue(IconThemeTypeProperty, value);
    }

    public IconGallery()
    {
        _galleryModel = new IconGalleryModel();
        DataContext   = _galleryModel;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (DataContext is ColorItemViewModel v)
        {
            WeakReferenceMessenger.Default.Send(v);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (!_initialized)
        {
            if (IconThemeType.HasValue)
            {
                _galleryModel.LoadThemeIcons(IconThemeType.Value);
            }

            _initialized = true;
        }
    }
}