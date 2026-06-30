using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class UploadPicturePreviewContent : AbstractUploadPictureContent
{
    #region 公共属性定义

    public static readonly StyledProperty<IList<ImageSourceUri>?> SourcesProperty =
        AvaloniaProperty.Register<UploadPicturePreviewContent, IList<ImageSourceUri>?>(nameof(Sources));
    
    public IList<ImageSourceUri>? Sources
    {
        get => GetValue(SourcesProperty);
        set => SetValue(SourcesProperty, value);
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
                SetCurrentValue(SourcesProperty, new[] { ImageSourceUri.Parse(FilePath.ToString()) });
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

    private void HandleLinkTextClicked()
    {
        if (_uploadImagePreviewer != null)
        {
            _uploadImagePreviewer.OpenDialog();
        }
    }
}
