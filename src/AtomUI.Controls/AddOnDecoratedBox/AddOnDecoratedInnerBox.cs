using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

[TemplatePart(AddOnDecoratedInnerBoxTheme.ContentPresenterPart, typeof(ContentPresenter), IsRequired = true)]
public class AddOnDecoratedInnerBox : ContentControl,
                                      IAnimationAwareControl,
                                      ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AddOnDecoratedBox.SizeTypeProperty.AddOwner<AddOnDecoratedInnerBox>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<AddOnDecoratedInnerBox>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<AddOnDecoratedInnerBox>();

    public static readonly StyledProperty<object?> LeftAddOnContentProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, object?>(nameof(LeftAddOnContent));

    public static readonly StyledProperty<object?> RightAddOnContentProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, object?>(nameof(RightAddOnContent));

    public static readonly StyledProperty<bool> IsClearButtonVisibleProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, bool>(nameof(IsClearButtonVisible));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<AddOnDecoratedInnerBox>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<AddOnDecoratedInnerBox>();

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

    public object? RightAddOnContent
    {
        get => GetValue(RightAddOnContentProperty);
        set => SetValue(RightAddOnContentProperty, value);
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

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<Thickness> InnerBoxPaddingProperty =
        AvaloniaProperty.Register<AddOnDecoratedInnerBox, Thickness>(nameof(InnerBoxPadding));

    internal static readonly DirectProperty<AddOnDecoratedInnerBox, Thickness> EffectiveInnerBoxPaddingProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedInnerBox, Thickness>(nameof(EffectiveInnerBoxPadding),
            o => o.EffectiveInnerBoxPadding,
            (o, v) => o.EffectiveInnerBoxPadding = v);

    internal static readonly DirectProperty<AddOnDecoratedInnerBox, Thickness> ContentPresenterMarginProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedInnerBox, Thickness>(nameof(ContentPresenterMargin),
            o => o.ContentPresenterMargin,
            (o, v) => o.ContentPresenterMargin = v);

    private static readonly DirectProperty<AddOnDecoratedInnerBox, double> MarginXSTokenProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedInnerBox, double>(nameof(MarginXSToken),
            o => o.MarginXSToken,
            (o, v) => o.MarginXSToken = v);

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

    private double _marginXSToken;

    private double MarginXSToken
    {
        get => _marginXSToken;
        set => SetAndRaise(MarginXSTokenProperty, ref _marginXSToken, value);
    }

    private Thickness _contentPresenterMargin;

    internal Thickness ContentPresenterMargin
    {
        get => _contentPresenterMargin;
        set => SetAndRaise(ContentPresenterMarginProperty, ref _contentPresenterMargin, value);
    }

    Control IAnimationAwareControl.PropertyBindTarget => this;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private StackPanel? _leftAddOnLayout;
    private StackPanel? _rightAddOnLayout;
    private IconButton? _clearButton;
    private CompositeDisposable? _tokenBindingsDisposable;

    public AddOnDecoratedInnerBox()
    {
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

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

        if (change.Property == LeftAddOnContentProperty || change.Property == RightAddOnContentProperty)
        {
            if (change.OldValue is Control oldControl)
            {
                oldControl.SetTemplatedParent(null);
            }

            if (change.NewValue is Control newControl)
            {
                newControl.SetTemplatedParent(this);
            }
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
        this.AddTokenBindingDisposable(
            TokenResourceBinder.CreateTokenBinding(this, MarginXSTokenProperty, SharedTokenKey.MarginXS));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.RunThemeTokenBindingActions();
        _leftAddOnLayout  = e.NameScope.Find<StackPanel>(AddOnDecoratedInnerBoxTheme.LeftAddOnLayoutPart);
        _rightAddOnLayout = e.NameScope.Find<StackPanel>(AddOnDecoratedInnerBoxTheme.RightAddOnLayoutPart);
        _clearButton      = e.NameScope.Find<IconButton>(AddOnDecoratedInnerBoxTheme.ClearButtonPart);

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

        SetupContentPresenterMargin();
        BuildEffectiveInnerBoxPadding();
    }

    private void HandleLayoutSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        SetupContentPresenterMargin();
    }

    private void SetupContentPresenterMargin()
    {
        var marginLeft  = 0d;
        var marginRight = 0d;
        if (_leftAddOnLayout is not null)
        {
            if (_leftAddOnLayout.DesiredSize.Width > 0 && _leftAddOnLayout.DesiredSize.Height > 0)
            {
                marginLeft = _marginXSToken;
            }
        }

        if (_rightAddOnLayout is not null)
        {
            if (_rightAddOnLayout.DesiredSize.Width > 0 && _rightAddOnLayout.DesiredSize.Height > 0)
            {
                marginRight = _marginXSToken;
            }
        }

        ContentPresenterMargin = new Thickness(marginLeft, 0, marginRight, 0);
    }
}