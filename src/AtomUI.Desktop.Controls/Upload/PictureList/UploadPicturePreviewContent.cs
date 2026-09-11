using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class UploadPicturePreviewContent : AbstractUploadPictureContent
{
    #region 公共属性定义

    public static readonly StyledProperty<IEnumerable<ImagePreviewItem>?> ItemsSourceProperty =
        AvaloniaProperty.Register<UploadPicturePreviewContent, IEnumerable<ImagePreviewItem>?>(nameof(ItemsSource));
    
    public IEnumerable<ImagePreviewItem>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    #endregion

    private UploadImagePreviewer? _uploadImagePreviewer;
    
    static UploadPicturePreviewContent()
    {
        HyperLinkTextBlock.ClickEvent.AddClassHandler<UploadPicturePreviewContent>((o, args) => o.HandleLinkTextClicked());
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FilePathProperty)
        {
            if (FilePath != null)
            {
                SetCurrentValue(ItemsSourceProperty, new[] { new ImagePreviewItem(ImageSource.Parse(FilePath.AbsoluteUri)) });
            }
            else
            {
                SetCurrentValue(ItemsSourceProperty, null);
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _uploadImagePreviewer = e.NameScope.Find<UploadImagePreviewer>("PART_ImagePreviewer");
    }

    private void HandleLinkTextClicked()
    {
        if (_uploadImagePreviewer != null)
        {
            _uploadImagePreviewer.OpenDialog();
        }
    }
}
