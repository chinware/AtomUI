using Avalonia;
using Avalonia.Data;

namespace AtomUI.Controls;

public abstract class SkeletonElement : AbstractSkeleton, ISizeTypeAware
{
    #region 公共属性定义
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<SkeletonButton>();
    
    public static readonly StyledProperty<bool> IsBlockProperty =
        AvaloniaProperty.Register<SkeletonElement, bool>(nameof(IsBlock));
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsBlock
    {
        get => GetValue(IsBlockProperty);
        set => SetValue(IsBlockProperty, value);
    }
    
    #endregion

    static SkeletonElement()
    {
        AffectsMeasure<SkeletonElement>(IsBlockProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsBlockProperty)
        {
            if (IsBlock)
            {
                SetValue(WidthProperty, double.NaN, BindingPriority.Template);
            }
        }
    }

}