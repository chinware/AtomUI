using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

using AvaloniaNumericUpDown = Avalonia.Controls.NumericUpDown;

public class NumericUpDown : AvaloniaNumericUpDown, IMotionAwareControl, IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<NumericUpDown>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<NumericUpDown>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<NumericUpDown>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<NumericUpDown>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<NumericUpDown>();

    public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
        TextBox.IsEnableClearButtonProperty.AddOwner<NumericUpDown>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<NumericUpDown>();

    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }

    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public AddOnDecoratedVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public AddOnDecoratedStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public bool IsEnableClearButton
    {
        get => GetValue(IsEnableClearButtonProperty);
        set => SetValue(IsEnableClearButtonProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义

    internal static readonly StyledProperty<double> SpinnerHandleWidthProperty =
        ButtonSpinner.SpinnerHandleWidthProperty.AddOwner<NumericUpDown>();
    
    internal static readonly StyledProperty<bool> IsCustomFontSizeProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsCustomFontSize));
    
    internal double SpinnerHandleWidth
    {
        get => GetValue(SpinnerHandleWidthProperty);
        set => SetValue(SpinnerHandleWidthProperty, value);
    }
    
    public bool IsCustomFontSize
    {
        get => GetValue(IsCustomFontSizeProperty);
        set => SetValue(IsCustomFontSizeProperty, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => NumericUpDownToken.ID;
    
    #endregion
    
    public NumericUpDown()
    {
        this.RegisterResources();
    }
}