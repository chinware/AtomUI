using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

public enum CardStyleVariant
{
    Outline,
    Borderless
}

public class Card : HeaderedContentControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<object?> ExtraProperty = 
        AvaloniaProperty.Register<Card, object?>(nameof (Extra));
    
    public static readonly StyledProperty<IDataTemplate?> ExtraTemplateProperty = 
        AvaloniaProperty.Register<Card, IDataTemplate?>(nameof (ExtraTemplate));
    
    public static readonly StyledProperty<CardStyleVariant> StyleVariantProperty = 
        AvaloniaProperty.Register<Card, CardStyleVariant>(nameof (StyleVariant));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty = 
        AvaloniaProperty.Register<Card, SizeType>(nameof (SizeType));
    
    public static readonly StyledProperty<bool> IsLoadingProperty = 
        AvaloniaProperty.Register<Card, bool>(nameof (IsLoading));
    
    public static readonly StyledProperty<bool> IsInnerModeProperty = 
        AvaloniaProperty.Register<Card, bool>(nameof (IsInnerMode));
    
    public static readonly StyledProperty<bool> IsHoverableProperty = 
        AvaloniaProperty.Register<Card, bool>(nameof (IsHoverable));
    
    public static readonly StyledProperty<object?> CoverProperty = 
        AvaloniaProperty.Register<Card, object?>(nameof (Cover));
    
    public static readonly StyledProperty<IDataTemplate?> CoverTemplateProperty = 
        AvaloniaProperty.Register<Card, IDataTemplate?>(nameof (CoverTemplate));
    
    public static readonly StyledProperty<List<Control>?> ActionsProperty = 
        AvaloniaProperty.Register<Card, List<Control>?>(nameof (Actions));
    
    public object? Extra
    {
        get => GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }
    
    public IDataTemplate? ExtraTemplate
    {
        get => GetValue(ExtraTemplateProperty);
        set => SetValue(ExtraTemplateProperty, value);
    }
    
    public CardStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public bool IsInnerMode
    {
        get => GetValue(IsInnerModeProperty);
        set => SetValue(IsInnerModeProperty, value);
    }
    
    public bool IsHoverable
    {
        get => GetValue(IsHoverableProperty);
        set => SetValue(IsHoverableProperty, value);
    }
    
    public object? Cover
    {
        get => GetValue(CoverProperty);
        set => SetValue(CoverProperty, value);
    }
    
    public IDataTemplate? CoverTemplate
    {
        get => GetValue(CoverTemplateProperty);
        set => SetValue(CoverTemplateProperty, value);
    }
    
    public List<Control>? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }
    #endregion
}