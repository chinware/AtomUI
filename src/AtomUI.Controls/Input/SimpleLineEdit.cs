using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

public class SimpleLineEdit : SimpleTextBox
{
    #region 公共属性定义

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        SimpleAddOnDecoratedBox.StyleVariantProperty.AddOwner<SimpleLineEdit>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        SimpleAddOnDecoratedBox.StatusProperty.AddOwner<SimpleLineEdit>();
    
    public static readonly StyledProperty<object?> LeftAddOnProperty =
        SimpleAddOnDecoratedBox.LeftAddOnProperty.AddOwner<SimpleLineEdit>();
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        SimpleAddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<SimpleLineEdit>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        SimpleAddOnDecoratedBox.RightAddOnProperty.AddOwner<SimpleLineEdit>();
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        SimpleAddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<SimpleLineEdit>();
    
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

    #endregion
    
    public SimpleLineEdit()
    {
        this.RegisterResources();
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == AddOnDecoratedStatus.Warning);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == AddOnDecoratedVariant.Outline);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == AddOnDecoratedVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == AddOnDecoratedVariant.Borderless);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StatusProperty ||
            change.Property == LeftAddOnProperty)
        {
            UpdatePseudoClasses();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }
}