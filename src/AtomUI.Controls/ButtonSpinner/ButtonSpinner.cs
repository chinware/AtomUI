using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaButtonSpinner = Avalonia.Controls.ButtonSpinner;

public class ButtonSpinner : AvaloniaButtonSpinner,
                             IAnimationAwareControl,
                             IControlSharedTokenResourcesHost,
                             ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(LeftAddOn));

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(RightAddOn));

    public static readonly StyledProperty<object?> InnerLeftContentProperty
        = AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(InnerLeftContent));

    public static readonly StyledProperty<object?> InnerRightContentProperty
        = AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(InnerRightContent));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AddOnDecoratedBox.SizeTypeProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<ButtonSpinner>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<ButtonSpinner>();

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

    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
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

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonSpinnerToken.ID;
    Control IAnimationAwareControl.PropertyBindTarget => this;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private Border? _spinnerHandleDecorator;
    private ButtonSpinnerDecoratedBox? _decoratedBox;
    private CompositeDisposable? _tokenBindingsDisposable;

    public ButtonSpinner()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CornerRadiusProperty)
        {
            SetupSpinnerHandleCornerRadius();
        }
        else if (change.Property == ButtonSpinnerLocationProperty)
        {
            SetupSpinnerHandleCornerRadius();
        }
    }

    private void SetupSpinnerHandleCornerRadius()
    {
        if (_spinnerHandleDecorator is not null)
        {
            if (ButtonSpinnerLocation == Location.Left)
            {
                _spinnerHandleDecorator.CornerRadius = new CornerRadius(CornerRadius.TopLeft,
                    0,
                    0,
                    CornerRadius.BottomLeft);
            }
            else
            {
                _spinnerHandleDecorator.CornerRadius = new CornerRadius(0,
                    CornerRadius.TopRight,
                    CornerRadius.BottomRight,
                    0);
            }
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var borderThickness = _decoratedBox?.BorderThickness ?? default;
        return base.ArrangeOverride(finalSize).Inflate(borderThickness);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _spinnerHandleDecorator = e.NameScope.Find<Border>(ButtonSpinnerTheme.SpinnerHandleDecoratorPart);
        _decoratedBox           = e.NameScope.Find<ButtonSpinnerDecoratedBox>(ButtonSpinnerTheme.DecoratedBoxPart);
        base.OnApplyTemplate(e);
        SetupSpinnerHandleCornerRadius();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}