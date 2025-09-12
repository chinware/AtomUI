using AtomUI.IconPkg;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls.MessageBox;

public class MessageBox : Dialog, IMotionAwareControl, IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        AvaloniaProperty.Register<MessageBox, MessageBoxStyle>(nameof (Style));
    
    public static readonly StyledProperty<DialogStandardButton> StandardButtonsProperty =
        AvaloniaProperty.Register<MessageBox, DialogStandardButton>(nameof (StandardButtons));
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<MessageBox, Icon?>(nameof (Icon));
    
    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }
    
    public DialogStandardButton StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    #endregion
    
    #region 内部属性定义
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => DialogToken.ID;
    
    #endregion
}