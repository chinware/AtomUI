using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[TemplatePart(AddOnDecoratedInnerBoxThemeConstants.ContentPresenterPart, typeof(ContentPresenter), IsRequired = true)]
public class AddOnDecoratedInnerBox : ContentControl, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<AddOnDecoratedInnerBox>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<AddOnDecoratedInnerBox>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<AddOnDecoratedInnerBox>();

    public static readonly StyledProperty<object?> LeftAddOnContentProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, object?>(nameof(LeftAddOnContent));
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnContentTemplateProperty =
        ContentTemplateProperty.AddOwner<ContentPresenter>();

    public static readonly StyledProperty<object?> RightAddOnContentProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, object?>(nameof(RightAddOnContent));
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnContentTemplateProperty =
        ContentTemplateProperty.AddOwner<ContentPresenter>();

    public static readonly StyledProperty<bool> IsClearButtonVisibleProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, bool>(nameof(IsClearButtonVisible));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AddOnDecoratedInnerBox>();

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

    public object? LeftAddOnContent
    {
        get => GetValue(LeftAddOnContentProperty);
        set => SetValue(LeftAddOnContentProperty, value);
    }
    
    public IDataTemplate? LeftAddOnContentTemplate
    {
        get => GetValue(LeftAddOnContentTemplateProperty);
        set => SetValue(LeftAddOnContentTemplateProperty, value);
    }

    public object? RightAddOnContent
    {
        get => GetValue(RightAddOnContentProperty);
        set => SetValue(RightAddOnContentProperty, value);
    }
    
    public IDataTemplate? RightAddOnContentTemplate
    {
        get => GetValue(RightAddOnContentTemplateProperty);
        set => SetValue(RightAddOnContentTemplateProperty, value);
    }

    public bool IsClearButtonVisible
    {
        get => GetValue(IsClearButtonVisibleProperty);
        set => SetValue(IsClearButtonVisibleProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<Thickness> InnerBoxPaddingProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, Thickness>(nameof(InnerBoxPadding));

    internal static readonly DirectProperty<AddOnDecoratedInnerBox, Thickness> EffectiveInnerBoxPaddingProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedInnerBox, Thickness>(nameof(EffectiveInnerBoxPadding),
            o => o.EffectiveInnerBoxPadding,
            (o, v) => o.EffectiveInnerBoxPadding = v);

    internal static readonly DirectProperty<AddOnDecoratedInnerBox, Thickness> EffectiveContentPresenterMarginProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedInnerBox, Thickness>(nameof(EffectiveContentPresenterMargin),
            o => o.EffectiveContentPresenterMargin,
            (o, v) => o.EffectiveContentPresenterMargin = v);

    internal static readonly StyledProperty<double> ContentPresenterMarginProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, double>(nameof(ContentPresenterMargin));

    internal Thickness InnerBoxPadding
    {
        get => GetValue(InnerBoxPaddingProperty);
        set => SetValue(InnerBoxPaddingProperty, value);
    }

    private Thickness _effectiveInnerBoxPadding;

    internal Thickness EffectiveInnerBoxPadding
    {
        get => _effectiveInnerBoxPadding;
        set => SetAndRaise(EffectiveInnerBoxPaddingProperty, ref _effectiveInnerBoxPadding, value);
    }
    
    internal double ContentPresenterMargin
    {
        get => GetValue(ContentPresenterMarginProperty);
        set => SetValue(ContentPresenterMarginProperty, value);
    }

    private Thickness _effectiveContentPresenterMargin;

    internal Thickness EffectiveContentPresenterMargin
    {
        get => _effectiveContentPresenterMargin;
        set => SetAndRaise(EffectiveContentPresenterMarginProperty, ref _effectiveContentPresenterMargin, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;

    #endregion

    private StackPanel? _leftAddOnLayout;
    private StackPanel? _rightAddOnLayout;
    private IconButton? _clearButton;
    private Border? _decorator;
    
    protected virtual void NotifyClearButtonClicked()
    {
    }

    protected virtual void BuildEffectiveInnerBoxPadding()
    {
        BindUtils.RelayBind(this, InnerBoxPaddingProperty, this, EffectiveInnerBoxPaddingProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == StyleVariantProperty)
        {
            UpdatePseudoClasses();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _leftAddOnLayout  = e.NameScope.Find<StackPanel>(AddOnDecoratedInnerBoxThemeConstants.LeftAddOnLayoutPart);
        _rightAddOnLayout = e.NameScope.Find<StackPanel>(AddOnDecoratedInnerBoxThemeConstants.RightAddOnLayoutPart);
        _clearButton      = e.NameScope.Find<IconButton>(AddOnDecoratedInnerBoxThemeConstants.ClearButtonPart);
        _decorator        = e.NameScope.Find<Border>(AddOnDecoratedInnerBoxThemeConstants.InnerBoxDecoratorPart);

        if (_leftAddOnLayout is not null)
        {
            _leftAddOnLayout.SizeChanged += HandleLayoutSizeChanged;
        }

        if (_rightAddOnLayout is not null)
        {
            _rightAddOnLayout.SizeChanged += HandleLayoutSizeChanged;
        }

        if (_clearButton is not null)
        {
            _clearButton.Click += (sender, args) => { NotifyClearButtonClicked(); };
        }

        SetupEffectiveContentPresenterMargin();
        ConfigureTransitions();
        UpdatePseudoClasses();
    }

    private void HandleLayoutSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        SetupEffectiveContentPresenterMargin();
    }

    private void SetupEffectiveContentPresenterMargin()
    {
        var marginLeft  = 0d;
        var marginRight = 0d;
        if (_leftAddOnLayout is not null)
        {
            if (_leftAddOnLayout.DesiredSize.Width > 0 && _leftAddOnLayout.DesiredSize.Height > 0)
            {
                marginLeft = ContentPresenterMargin;
            }
        }

        if (_rightAddOnLayout is not null)
        {
            if (_rightAddOnLayout.DesiredSize.Width > 0 && _rightAddOnLayout.DesiredSize.Height > 0)
            {
                marginRight = ContentPresenterMargin;
            }
        }

        EffectiveContentPresenterMargin = new Thickness(marginLeft, 0, marginRight, 0);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        BuildEffectiveInnerBoxPadding();
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (_decorator != null)
            {
                _decorator.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
                };
            }
        }
        else
        {
            if (_decorator != null)
            {
                _decorator.Transitions?.Clear();
                _decorator.Transitions = null;
            }
        }
    }

    protected virtual void UpdatePseudoClasses()
    {
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == AddOnDecoratedVariant.Outline);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == AddOnDecoratedVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == AddOnDecoratedVariant.Borderless);
    }
}