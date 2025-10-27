using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class ButtonSpinnerDecoratedBox : SimpleAddOnDecoratedBox
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> SpinnerContentProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, object?>(nameof(SpinnerContent));

    public static readonly StyledProperty<bool> IsShowHandleProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, bool>(nameof(IsShowHandle), true);

    public static readonly StyledProperty<bool> IsHandleFloatableProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, bool>(nameof(IsHandleFloatable), true);

    public static readonly StyledProperty<ButtonSpinnerLocation> ButtonSpinnerLocationProperty =
        ButtonSpinner.ButtonSpinnerLocationProperty.AddOwner<ButtonSpinnerDecoratedBox>();

    public object? SpinnerContent
    {
        get => GetValue(SpinnerContentProperty);
        set => SetValue(SpinnerContentProperty, value);
    }

    public bool IsShowHandle
    {
        get => GetValue(IsShowHandleProperty);
        set => SetValue(IsShowHandleProperty, value);
    }

    public bool IsHandleFloatable
    {
        get => GetValue(IsHandleFloatableProperty);
        set => SetValue(IsHandleFloatableProperty, value);
    }

    public ButtonSpinnerLocation ButtonSpinnerLocation
    {
        get => GetValue(ButtonSpinnerLocationProperty);
        set => SetValue(ButtonSpinnerLocationProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, bool> ShowButtonSpinnerProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, bool>(nameof(ShowButtonSpinner),
            o => o.ShowButtonSpinner,
            (o, v) => o.ShowButtonSpinner = v);
    
    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, Thickness> SpinnerBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, Thickness>(nameof(SpinnerBorderThickness),
            o => o.SpinnerBorderThickness,
            (o, v) => o.SpinnerBorderThickness = v);
    
    public static readonly StyledProperty<double> HandleOpacityProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, double>(nameof(HandleOpacity));

    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, double> SpinnerHandleWidthProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, double>(nameof(SpinnerHandleWidth),
            o => o.SpinnerHandleWidth,
            (o, v) => o.SpinnerHandleWidth = v);
    
    public static readonly StyledProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, Thickness>(nameof(ContentPadding));

    private bool _showButtonSpinner;

    internal bool ShowButtonSpinner
    {
        get => _showButtonSpinner;
        set => SetAndRaise(ShowButtonSpinnerProperty, ref _showButtonSpinner, value);
    }
    
    private Thickness _spinnerBorderThickness;

    internal Thickness SpinnerBorderThickness
    {
        get => _spinnerBorderThickness;
        set => SetAndRaise(SpinnerBorderThicknessProperty, ref _spinnerBorderThickness, value);
    }

    internal double HandleOpacity
    {
        get => GetValue(HandleOpacityProperty);
        set => SetValue(HandleOpacityProperty, value);
    }

    private double _spinnerHandleWidth;

    internal double SpinnerHandleWidth
    {
        get => _spinnerHandleWidth;
        set => SetAndRaise(SpinnerHandleWidthProperty, ref _spinnerHandleWidth, value);
    }
    
    internal Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }
    
    #endregion
    
    private IDisposable? _mouseMoveDisposable;
    private IDisposable? _borderThicknessDisposable;
    private Border? _contentFrame;
    
    protected void ConfigureEffectiveContentPadding()
    {
        if (_contentFrame != null)
        {
            if (IsShowHandle && !IsHandleFloatable)
            {
                var padding = SpinnerHandleWidth * 1.1;
                if (ButtonSpinnerLocation == ButtonSpinnerLocation.Right)
                {
                    _contentFrame.SetCurrentValue(PaddingProperty, new Thickness(ContentPadding.Left, ContentPadding.Top, padding,
                        ContentPadding.Bottom));
                }
                else
                {
                    _contentFrame.SetCurrentValue(PaddingProperty, new Thickness(padding, ContentPadding.Top, ContentPadding.Right,
                        ContentPadding.Bottom));
                }
            }
            else
            {
                _contentFrame.SetCurrentValue(PaddingProperty, ContentPadding);
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, SpinnerBorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        ConfigureMoveProcessor();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _mouseMoveDisposable?.Dispose();
        _borderThicknessDisposable?.Dispose();
    }

    private void ConfigureMoveProcessor()
    {
        if (IsShowHandle && IsHandleFloatable)
        {
            _mouseMoveDisposable?.Dispose();
            var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
            _mouseMoveDisposable = inputManager.Process.Subscribe(HandleMouseMove);
        }
    }

    private void HandleMouseMove(RawInputEventArgs args)
    {
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            var pos = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
            if (!pos.HasValue)
            {
                return;
            }

            var bounds = new Rect(pos.Value, Bounds.Size);
            if (bounds.Contains(pointerEventArgs.Position))
            {
                if (IsShowHandle && IsHandleFloatable)
                {
                    HandleOpacity = 1.0;
                }
            }
            else
            {
                if (IsShowHandle && IsHandleFloatable)
                {
                    HandleOpacity = 0.0;
                }
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsShowHandleProperty ||
            change.Property == IsHandleFloatableProperty)
        {
            if (IsShowHandle && !IsHandleFloatable)
            {
                HandleOpacity = 1.0;
            }
            else if (!IsShowHandle)
            {
                HandleOpacity = 0.0;
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsHandleFloatableProperty)
            {
                if (change.OldValue is bool oldValue)
                {
                    if (oldValue)
                    {
                        _mouseMoveDisposable?.Dispose();
                    }
                }

                if (IsHandleFloatable)
                {
                    ConfigureMoveProcessor();
                }
            }
        }

        if (change.Property == IsHandleFloatableProperty ||
            change.Property == IsShowHandleProperty ||
            change.Property == SpinnerHandleWidthProperty ||
            change.Property == ContentPaddingProperty)
        {
            ConfigureEffectiveContentPadding();
        }
    }
    
    protected override void NotifyCreateTransitions(Transitions transitions)
    {
        base.NotifyCreateTransitions(transitions);
        transitions.Add(TransitionUtils.CreateTransition<DoubleTransition>(HandleOpacityProperty));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _contentFrame =  e.NameScope.Find<Border>(AddOnDecoratedBoxThemeConstants.ContentFramePart);
        ConfigureEffectiveContentPadding();
    }
}