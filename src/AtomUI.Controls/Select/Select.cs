using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class Select : TemplatedControl,
                      IMotionAwareControl,
                      IControlSharedTokenResourcesHost,
                      ISizeTypeAware
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsAllowClear));
    
    public static readonly StyledProperty<bool> IsAutoClearSearchValueProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsAutoClearSearchValue));
    
    public static readonly StyledProperty<bool> IsDefaultActiveFirstOptionProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDefaultActiveFirstOption));
    
    public static readonly StyledProperty<bool> IsDefaultOpenProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDefaultOpen));
    
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDropDownOpen));
    
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<Select, string?>(nameof(PlaceholderText));
    
    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<Select, IBrush?>(nameof(PlaceholderForeground));
    
    public static readonly StyledProperty<bool> IsPopupMatchSelectWidthProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsPopupMatchSelectWidth));
    
    public static readonly StyledProperty<bool> IsFilterOptionProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsFilterOption));
    
    public static readonly StyledProperty<int> DisplayPageSizeProperty = 
        AvaloniaProperty.Register<Select, int>(nameof (DisplayPageSize), 10);
    
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsLoading));
    
    public static readonly StyledProperty<int> MaxCountProperty =
        AvaloniaProperty.Register<Select, int>(nameof(MaxCount));
        
    public static readonly StyledProperty<int> MaxTagCountProperty =
        AvaloniaProperty.Register<Select, int>(nameof(MaxTagCount));
    
    public static readonly StyledProperty<string?> MaxTagPlaceholderProperty =
        AvaloniaProperty.Register<Select, string?>(nameof(MaxTagPlaceholder));
    
    public static readonly StyledProperty<int> MaxTagTextLengthProperty =
        AvaloniaProperty.Register<Select, int>(nameof(MaxTagPlaceholder));
    
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<Select, SelectMode>(nameof(Mode));
    
    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<Select>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<object?> InnerLeftContentProperty = 
        AvaloniaProperty.Register<Select, object?>(nameof(InnerLeftContent));
    
    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        AvaloniaProperty.Register<Select, IDataTemplate?>(nameof(InnerLeftContentTemplate));

    public static readonly StyledProperty<object?> InnerRightContentProperty =
        AvaloniaProperty.Register<Select, object?>(nameof(InnerRightContent));
    
    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        AvaloniaProperty.Register<Select, IDataTemplate?>(nameof(InnerRightContentTemplate));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Select>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<Select>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Select>();
    
    protected static readonly DirectProperty<Select, IList<SelectOption>?> SelectedOptionsProperty =
        AvaloniaProperty.RegisterDirect<Select, IList<SelectOption>?>(
            nameof(SelectedOptions),
            o => o.SelectedOptions);
    
    public static readonly StyledProperty<double> OptionFontSizeProperty =
        AvaloniaProperty.Register<Select, double>(nameof(OptionFontSize));
    
    public static readonly StyledProperty<bool> AutoScrollToSelectedOptionsProperty =
        AvaloniaProperty.Register<Select, bool>(
            nameof(AutoScrollToSelectedOptions),
            defaultValue: false);
    
    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }
    
    public bool IsAutoClearSearchValue
    {
        get => GetValue(IsAutoClearSearchValueProperty);
        set => SetValue(IsAutoClearSearchValueProperty, value);
    }
    
    public bool IsDefaultActiveFirstOption
    {
        get => GetValue(IsDefaultActiveFirstOptionProperty);
        set => SetValue(IsDefaultActiveFirstOptionProperty, value);
    }
    
    public bool IsDefaultOpen
    {
        get => GetValue(IsDefaultOpenProperty);
        set => SetValue(IsDefaultOpenProperty, value);
    }
    
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

    public bool IsPopupMatchSelectWidth
    {
        get => GetValue(IsPopupMatchSelectWidthProperty);
        set => SetValue(IsPopupMatchSelectWidthProperty, value);
    }
    
    public bool IsFilterOption
    {
        get => GetValue(IsFilterOptionProperty);
        set => SetValue(IsFilterOptionProperty, value);
    }
    
    public int DisplayPageSize
    {
        get => GetValue(DisplayPageSizeProperty);
        set => SetValue(DisplayPageSizeProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public int MaxCount
    {
        get => GetValue(MaxCountProperty);
        set => SetValue(MaxCountProperty, value);
    }
    
    public int MaxTagCount
    {
        get => GetValue(MaxTagCountProperty);
        set => SetValue(MaxTagCountProperty, value);
    }
    
    public string? MaxTagPlaceholder
    {
        get => GetValue(MaxTagPlaceholderProperty);
        set => SetValue(MaxTagPlaceholderProperty, value);
    }
    
    public int MaxTagTextLength
    {
        get => GetValue(MaxTagTextLengthProperty);
        set => SetValue(MaxTagTextLengthProperty, value);
    }
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }
    
    public IDataTemplate? LeftAddOnTemplate
    {
        get => GetValue(LeftAddOnTemplateProperty);
        set => SetValue(LeftAddOnTemplateProperty, value);
    }

    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }
    
    public IDataTemplate? RightAddOnTemplate
    {
        get => GetValue(RightAddOnTemplateProperty);
        set => SetValue(RightAddOnTemplateProperty, value);
    }
    
    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }
    
    public IDataTemplate? InnerLeftContentTemplate
    {
        get => GetValue(InnerLeftContentTemplateProperty);
        set => SetValue(InnerLeftContentTemplateProperty, value);
    }

    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }
    
    public IDataTemplate? InnerRightContentTemplate
    {
        get => GetValue(InnerRightContentTemplateProperty);
        set => SetValue(InnerRightContentTemplateProperty, value);
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
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    [Content]
    public AvaloniaList<SelectOption> Options { get; set; } = new();
    
    private IList<SelectOption>? _selectedOptions;

    public IList<SelectOption>? SelectedOptions
    {
        get => _selectedOptions;
        set => SetAndRaise(SelectedOptionsProperty, ref _selectedOptions, value);
    }

    public double OptionFontSize
    {
        get => GetValue(OptionFontSizeProperty);
        set => SetValue(OptionFontSizeProperty, value);
    }
    
    public bool AutoScrollToSelectedOptions
    {
        get => GetValue(AutoScrollToSelectedOptionsProperty);
        set => SetValue(AutoScrollToSelectedOptionsProperty, value);
    }
    #endregion
    
    public IComparer<SelectOption>? FilterSortFn { get; set; }
    public Func<object?, SelectOption, bool>? FilterFn { get; set; }
    public IList<object>? DefaultSelectedOptions { get; set; }
    
    #region 公共事件定义
    
    public event EventHandler? DropDownClosed;
    public event EventHandler? DropDownOpened;

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<Select, double>(nameof(ItemHeight));
    
    internal static readonly StyledProperty<double> MaxPopupHeightProperty =
        AvaloniaProperty.Register<Select, double>(nameof(MaxPopupHeight));
    
    internal static readonly StyledProperty<Thickness> PopupContentPaddingProperty =
        AvaloniaProperty.Register<Select, Thickness>(nameof(PopupContentPadding));
    
    internal static readonly DirectProperty<Select, bool> IsEffectiveShowClearButtonProperty =
        AvaloniaProperty.RegisterDirect<Select, bool>(nameof(IsEffectiveShowClearButton),
            o => o.IsEffectiveShowClearButton,
            (o, v) => o.IsEffectiveShowClearButton = v);
    
    internal double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }
    
    internal double MaxPopupHeight
    {
        get => GetValue(MaxPopupHeightProperty);
        set => SetValue(MaxPopupHeightProperty, value);
    }
    
    internal Thickness PopupContentPadding
    {
        get => GetValue(PopupContentPaddingProperty);
        set => SetValue(PopupContentPaddingProperty, value);
    }
    
    private bool _isEffectiveShowClearButton;

    internal bool IsEffectiveShowClearButton
    {
        get => _isEffectiveShowClearButton;
        set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SelectToken.ID;

    #endregion
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());
    
    private Popup? _popup;
    
    static Select()
    {
        FocusableProperty.OverrideDefaultValue<Select>(true);
    }
    
    public Select()
    {
        this.RegisterResources();
    }
    
    internal void ItemFocused(SelectOptionItem selectOptionItem)
    {
        if (IsDropDownOpen && selectOptionItem.IsFocused && selectOptionItem.IsArrangeValid)
        {
            selectOptionItem.BringIntoView();
        }
    }
    
    private void ConfigureMaxDropdownHeight()
    {
    }
}