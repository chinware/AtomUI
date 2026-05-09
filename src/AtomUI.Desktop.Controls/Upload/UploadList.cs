using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class UploadList : ItemsControl, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<UploadListType> ListTypeProperty =
        Upload.ListTypeProperty.AddOwner<UploadList>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<UploadList>();
    
    public UploadListType ListType
    {
        get => GetValue(ListTypeProperty);
        set => SetValue(ListTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ListTypeProperty)
        {
            RefreshContainers();
        }
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (ListType == UploadListType.Picture)
        {
            return new UploadPictureListItem();
        }
        if (ListType == UploadListType.PictureCard || ListType == UploadListType.PictureCircle)
        {
            return new UploadPictureShapeListItem();
        }
        return new UploadTextListItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AbstractUploadListItem>(item, out recycleKey);
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AbstractUploadListItem listItem)
        {
            if (item != null && item is UploadTaskInfo uploadTaskInfo)
            {
                listItem[!AbstractUploadListItem.TaskIdProperty]   = uploadTaskInfo[!UploadTaskInfo.TaskIdProperty];
                listItem[!AbstractUploadListItem.FileNameProperty] = uploadTaskInfo[!UploadTaskInfo.FileNameProperty];
                listItem[!AbstractUploadListItem.ProgressProperty] = uploadTaskInfo[!UploadTaskInfo.ProgressProperty];
                listItem[!AbstractUploadListItem.IsImageFileProperty] =
                    uploadTaskInfo[!UploadTaskInfo.IsImageFileProperty];
                listItem[!AbstractUploadListItem.StatusProperty]       = uploadTaskInfo[!UploadTaskInfo.StatusProperty];
                listItem[!AbstractUploadListItem.ErrorMessageProperty] = uploadTaskInfo[!UploadTaskInfo.ErrorMessageProperty];
                listItem[!AbstractUploadListItem.FilePathProperty] = uploadTaskInfo[!UploadTaskInfo.FilePathProperty];
            }
            listItem[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            listItem[!ListTypeProperty]        = this[!ListTypeProperty];
            NotifyPrepareUploadListItem(listItem);
        }
    }

    protected virtual void NotifyPrepareUploadListItem(AbstractUploadListItem listItem)
    {
    }
}