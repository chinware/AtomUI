using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

internal class UploadPictureShapeList : UploadList
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> TriggerContentProperty =
        AvaloniaProperty.Register<UploadPictureShapeList, object?>(nameof(TriggerContent));
    
    public static readonly StyledProperty<IDataTemplate?> TriggerContentTemplateProperty =
        AvaloniaProperty.Register<UploadPictureShapeList, IDataTemplate?>(nameof(TriggerContentTemplate));
    
    public static readonly StyledProperty<bool> IsShowUploadListProperty =
        Upload.IsShowUploadListProperty.AddOwner<UploadPictureShapeList>();

    public static readonly StyledProperty<bool> IsShowUploadTriggerProperty =
        Upload.IsShowUploadTriggerProperty.AddOwner<UploadPictureShapeList>();
    
    [DependsOn(nameof(TriggerContentTemplate))]
    public object? TriggerContent
    {
        get => GetValue(TriggerContentProperty);
        set => SetValue(TriggerContentProperty, value);
    }

    public IDataTemplate? TriggerContentTemplate
    {
        get => GetValue(TriggerContentTemplateProperty);
        set => SetValue(TriggerContentTemplateProperty, value);
    }
        
    public bool IsShowUploadList
    {
        get => GetValue(IsShowUploadListProperty);
        set => SetValue(IsShowUploadListProperty, value);
    }
    
    public bool IsShowUploadTrigger
    {
        get => GetValue(IsShowUploadTriggerProperty);
        set => SetValue(IsShowUploadTriggerProperty, value);
    }

    #endregion
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (ListType == UploadListType.PictureCard || ListType == UploadListType.PictureCircle)
        {
            if (item is UploadTaskInfo uploadTaskInfo && uploadTaskInfo.IsPictureTriggerTask)
            {
                return new UploadTriggerContent();
            }
        }
        return base.CreateContainerForItemOverride(item, index, recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is UploadTriggerContent listItem)
        {
            listItem[!IsMotionEnabledProperty]                      = this[!IsMotionEnabledProperty];
            listItem[!IsVisibleProperty]                            = this[!IsShowUploadTriggerProperty];
            listItem[!ListTypeProperty]                             = this[!ListTypeProperty];
            listItem[!UploadTriggerContent.ContentProperty]         = this[!TriggerContentProperty];
            listItem[!UploadTriggerContent.ContentTemplateProperty] = this[!TriggerContentTemplateProperty];
        }
    }
    
    protected override void NotifyPrepareUploadListItem(AbstractUploadListItem listItem)
    {
        listItem[!IsVisibleProperty] = this[!IsShowUploadListProperty];
    }
}