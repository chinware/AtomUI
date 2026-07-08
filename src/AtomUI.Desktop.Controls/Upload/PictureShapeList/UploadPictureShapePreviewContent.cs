using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class UploadPictureShapePreviewContent : AbstractUploadPictureContent
{
    #region 公共属性定义

    public static readonly StyledProperty<IList<IImagePreviewSource>?> SourcesProperty =
        AvaloniaProperty.Register<UploadPictureShapePreviewContent, IList<IImagePreviewSource>?>(nameof(Sources));
    
    public IList<IImagePreviewSource>? Sources
    {
        get => GetValue(SourcesProperty);
        set => SetValue(SourcesProperty, value);
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
                SetCurrentValue(SourcesProperty, new[] { new UriImagePreviewSource(FilePath.ToString()) });
            }
            else
            {
                SetCurrentValue(SourcesProperty, null);
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
