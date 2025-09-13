using AtomUI.IconPkg;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.MessageBox;

public enum MessageBoxOkButtonStyle
{
    Default,
    Primary
}

public class MessageBox : TemplatedControl, 
                          IMotionAwareControl, 
                          IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<MessageBox, string?>(nameof (Title));
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<MessageBox, Icon?>(nameof (Icon));
    
    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        AvaloniaProperty.Register<MessageBox, MessageBoxStyle>(nameof (Style));
    
    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<MessageBox>();
    
    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<MessageBox>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MessageBox>();
    
    public static readonly StyledProperty<DialogHostType> HostTypeProperty =
        AvaloniaProperty.Register<Dialog, DialogHostType>(nameof(HostType), DialogHostType.Overlay);
    
    public static readonly StyledProperty<MessageBoxOkButtonStyle> OkButtonStyleProperty =
        AvaloniaProperty.Register<Dialog, MessageBoxOkButtonStyle>(nameof(OkButtonStyle), MessageBoxOkButtonStyle.Primary);
    
    public static readonly StyledProperty<string?> OkButtonTextProperty = AvaloniaProperty.Register<Dialog, string?>(nameof(OkButtonText));
    
    public static readonly StyledProperty<string?> CancelButtonTextProperty = AvaloniaProperty.Register<Dialog, string?>(nameof(CancelButtonText));
    
    public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<Dialog, bool>(nameof(IsLoading));
    
    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        Dialog.IsLightDismissEnabledProperty.AddOwner<MessageBox>();
    
    public static readonly StyledProperty<bool> IsOpenProperty =
        Dialog.IsOpenProperty.AddOwner<MessageBox>();
    
    public static readonly StyledProperty<bool> IsModalProperty =
        Dialog.IsModalProperty.AddOwner<MessageBox>();
    
    public static readonly StyledProperty<bool> IsCenterOnStartupProperty =
        AvaloniaProperty.Register<MessageBox, bool>(nameof(IsCenterOnStartup));
    
    public static readonly StyledProperty<Dimension?> HorizontalOffsetProperty =
        AvaloniaProperty.Register<MessageBox, Dimension?>(nameof(HorizontalOffset));
    
    public static readonly StyledProperty<Dimension?> VerticalOffsetProperty =
        AvaloniaProperty.Register<MessageBox, Dimension?>(nameof(VerticalOffset));
    
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }
    
    public DialogStandardButtons StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }
    
    public DialogStandardButton DefaultStandardButton
    {
        get => GetValue(DefaultStandardButtonProperty);
        set => SetValue(DefaultStandardButtonProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public DialogHostType HostType
    {
        get => GetValue(HostTypeProperty);
        set => SetValue(HostTypeProperty, value);
    }
    
    public MessageBoxOkButtonStyle OkButtonStyle
    {
        get => GetValue(OkButtonStyleProperty);
        set => SetValue(OkButtonStyleProperty, value);
    }
    
    public string? OkButtonText
    {
        get => GetValue(OkButtonTextProperty);
        set => SetValue(OkButtonTextProperty, value);
    }
    
    public string? CancelButtonText
    {
        get => GetValue(CancelButtonTextProperty);
        set => SetValue(CancelButtonTextProperty, value);
    }
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }
    
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }
    
    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }
    
    public bool IsCenterOnStartup
    {
        get => GetValue(IsCenterOnStartupProperty);
        set => SetValue(IsCenterOnStartupProperty, value);
    }
    
    public Dimension? HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }
    
    public Dimension? VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => DialogToken.ID;
    
    #endregion
}