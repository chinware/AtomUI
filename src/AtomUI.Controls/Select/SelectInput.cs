using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

using AvaloniaTextBox = Avalonia.Controls.TextBox;

internal class SelectInput : AvaloniaTextBox
{
    #region 公共属性定义
    
    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<SelectInput>();
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<SelectInput>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<SelectInput>();
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<SelectInput>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<SelectInput>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<SelectInput>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<SelectInput>();

    public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
        AvaloniaProperty.Register<SelectInput, bool>(nameof(IsEnableClearButton));
    
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

    #endregion

    #region 内部属性定义
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SelectInput>();

    internal static readonly DirectProperty<SelectInput, bool> IsEffectiveShowClearButtonProperty =
        AvaloniaProperty.RegisterDirect<SelectInput, bool>(nameof(IsEffectiveShowClearButton),
            o => o.IsEffectiveShowClearButton,
            (o, v) => o.IsEffectiveShowClearButton = v);

    private bool _isEffectiveShowClearButton;

    internal bool IsEffectiveShowClearButton
    {
        get => _isEffectiveShowClearButton;
        set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    private SelectInputInnerBox? _inputInnerBox;

    static SelectInput()
    {
        AffectsRender<TextBox>(BorderBrushProperty, BackgroundProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AcceptsReturnProperty ||
            change.Property == IsReadOnlyProperty ||
            change.Property == TextProperty ||
            change.Property == IsEnableClearButtonProperty)
        {
            SetupEffectiveShowClearButton();
        }
        else if (change.Property == StatusProperty || change.Property == StyleVariantProperty)
        {
            UpdatePseudoClasses();
        }
    }

    private void SetupEffectiveShowClearButton()
    {
        if (!IsEnableClearButton)
        {
            IsEffectiveShowClearButton = false;
            return;
        }

        IsEffectiveShowClearButton = !IsReadOnly && !AcceptsReturn && !string.IsNullOrEmpty(Text);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == AddOnDecoratedStatus.Warning);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == AddOnDecoratedVariant.Outline);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == AddOnDecoratedVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == AddOnDecoratedVariant.Borderless);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _inputInnerBox = e.NameScope.Find<SelectInputInnerBox>(SelectInputThemeConstants.InnerBoxPart);
        if (_inputInnerBox != null)
        {
            _inputInnerBox.OwningTextBox = this;
        }

        SetupEffectiveShowClearButton();
    }
}