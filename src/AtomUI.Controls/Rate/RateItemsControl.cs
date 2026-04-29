using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls.Commons;

internal class RateItemsControl : ItemsControl, ISizeTypeAware
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AbstractRate.IsAllowClearProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<bool> IsAllowHalfProperty =
        AbstractRate.IsAllowHalfProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<IBrush?> StarColorProperty =
        AbstractRate.StarColorProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<IBrush?> StarBgColorProperty =
        AbstractRate.StarBgColorProperty.AddOwner<RateItemsControl>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<RateItemsControl>();
    
    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }
    
    public bool IsAllowHalf
    {
        get => GetValue(IsAllowHalfProperty);
        set => SetValue(IsAllowHalfProperty, value);
    }
    
    public IBrush? StarColor
    {
        get => GetValue(StarColorProperty);
        set => SetValue(StarColorProperty, value);
    }
    
    public IBrush? StarBgColor
    {
        get => GetValue(StarBgColorProperty);
        set => SetValue(StarBgColorProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<RateItemsControl, object?> CharacterProperty =
        AvaloniaProperty.RegisterDirect<RateItemsControl, object?>(
            nameof(Character),
            o => o.Character,
            (o, v) => o.Character = v);
    
    private object? _character;

    internal object? Character
    {
        get => _character;
        set => SetAndRaise(CharacterProperty, ref _character, value);
    }
    
    #endregion
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is RateItem rateItem)
        {
            rateItem[!SizeTypeProperty]              = this[!SizeTypeProperty];
            rateItem[!IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
            rateItem[!RateItem.CharacterProperty]    = this[!CharacterProperty];
            rateItem[!RateItem.StarColorProperty]    = this[!StarColorProperty];
            rateItem[!RateItem.StarBgColorProperty]  = this[!StarBgColorProperty];
            rateItem[!RateItem.IsAllowClearProperty] = this[!IsAllowClearProperty];
            rateItem[!RateItem.IsAllowHalfProperty]  = this[!IsAllowHalfProperty];
            rateItem[!RateItem.FontSizeProperty]     = this[!FontSizeProperty];
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type RateItem.");
        }
    }
}