using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class UploadPictureUploadingContent : AbstractUploadPictureContent
{
    #region 公共属性定义

    public static readonly StyledProperty<double> ProgressProperty =
        AbstractUploadListItem.ProgressProperty.AddOwner<UploadPictureUploadingContent>();
    
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    #endregion
}
