using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class UploadPictureShapeUploadingContent : AbstractUploadPictureContent
{
    #region 公共属性定义

    public static readonly StyledProperty<double> ProgressProperty =
        AbstractUploadListItem.ProgressProperty.AddOwner<UploadPictureShapeUploadingContent>();
    
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    #endregion
}
