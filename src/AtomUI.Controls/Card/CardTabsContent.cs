using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

public class CardTabsContent : TemplatedControl
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> TabBarExtraContentProperty = 
        AvaloniaProperty.Register<Card, object?>(nameof (TabBarExtraContent));
    
    public static readonly StyledProperty<IDataTemplate?> TabBarExtraContentTemplateProperty = 
        AvaloniaProperty.Register<Card, IDataTemplate?>(nameof (TabBarExtraContentTemplate));
    
    public object? TabBarExtraContent
    {
        get => GetValue(TabBarExtraContentProperty);
        set => SetValue(TabBarExtraContentProperty, value);
    }
    
    public IDataTemplate? TabBarExtraContentTemplate
    {
        get => GetValue(TabBarExtraContentTemplateProperty);
        set => SetValue(TabBarExtraContentTemplateProperty,  value);
    }
    #endregion
}