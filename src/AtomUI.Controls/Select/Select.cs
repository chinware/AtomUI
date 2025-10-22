using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AtomUI.Controls;

public class Select : SelectingItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDropDownOpen));
    
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<Select, string?>(nameof(PlaceholderText));
    
    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<Select, IBrush?>(nameof(PlaceholderForeground));
    
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }
    
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }
    
    public IBrush? PlaceholderForeground
    {
        get => GetValue(PlaceholderForegroundProperty);
        set => SetValue(PlaceholderForegroundProperty, value);
    }

    #endregion

    #region 公共事件定义
    
    public event EventHandler? DropDownClosed;
    public event EventHandler? DropDownOpened;

    #endregion
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());
    
    private Popup? _popup;
    
    
    static Select()
    {
        ItemsPanelProperty.OverrideDefaultValue<ComboBox>(DefaultPanel);
        FocusableProperty.OverrideDefaultValue<ComboBox>(true);
        IsTextSearchEnabledProperty.OverrideDefaultValue<ComboBox>(true);
    }
    
    internal void ItemFocused(SelectOptionItem selectOptionItem)
    {
        if (IsDropDownOpen && selectOptionItem.IsFocused && selectOptionItem.IsArrangeValid)
        {
            selectOptionItem.BringIntoView();
        }
    }
}