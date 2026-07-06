using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class UploadPictureShapeList : UploadList
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsShowUploadListProperty =
        Upload.IsShowUploadListProperty.AddOwner<UploadPictureShapeList>();

    public bool IsShowUploadList
    {
        get => GetValue(IsShowUploadListProperty);
        set => SetValue(IsShowUploadListProperty, value);
    }
    
    #endregion

    protected override void NotifyPrepareUploadListItem(AbstractUploadListItem listItem)
    {
        listItem[!IsVisibleProperty] = this[!IsShowUploadListProperty];
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is UploadAppendContentItem)
        {
            recycleKey = null;
            return false;
        }

        return base.NeedsContainerOverride(item, index, out recycleKey);
    }
}
