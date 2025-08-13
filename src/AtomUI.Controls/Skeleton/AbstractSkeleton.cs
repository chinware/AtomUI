using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public abstract class AbstractSkeleton : TemplatedControl, 
                                         ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<StyledElement, bool>(nameof(IsActive));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<AbstractSkeleton>();
    
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    #endregion
}