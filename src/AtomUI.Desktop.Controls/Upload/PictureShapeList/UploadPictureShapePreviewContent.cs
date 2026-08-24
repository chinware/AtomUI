using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class UploadPictureShapePreviewContent : AbstractUploadPictureContent
{
    #region 公共属性定义

    public static readonly StyledProperty<IEnumerable<ImagePreviewItem>?> ItemsSourceProperty =
        AvaloniaProperty.Register<UploadPictureShapePreviewContent, IEnumerable<ImagePreviewItem>?>(nameof(ItemsSource));
    
    public IEnumerable<ImagePreviewItem>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    #endregion

    private UploadImagePreviewer? _uploadImagePreviewer;
    
    static UploadPictureShapePreviewContent()
    {
        IconButton.ClickEvent.AddClassHandler<UploadPictureShapePreviewContent>((o, args) => o.HandleActionButtonClicked((args.Source as IconButton)!));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FilePathProperty)
        {
            if (FilePath != null)
            {
                SetCurrentValue(ItemsSourceProperty, new[] { new ImagePreviewItem(ImageLoadSource.FromUri(FilePath)) });
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

    private void HandleActionButtonClicked(IconButton button)
    {
        if (button.Tag is UploadListActions actionType)
        {
            if (actionType == UploadListActions.Preview)
            {
                if (_uploadImagePreviewer != null)
                {
                    _uploadImagePreviewer.OpenDialog();
                }
            }
        }
    }
}
