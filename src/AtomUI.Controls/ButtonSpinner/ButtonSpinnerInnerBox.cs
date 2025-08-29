using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls; 
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[TemplatePart(ButtonSpinnerThemeConstants.SpinnerHandlePart, typeof(ContentPresenter))]
internal class ButtonSpinnerInnerBox : AddOnDecoratedInnerBox, IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> SpinnerContentProperty =
        AvaloniaProperty.Register<ButtonSpinnerInnerBox, object?>(nameof(SpinnerContent));

    public static readonly StyledProperty<bool> IsShowHandleProperty =
        AvaloniaProperty.Register<ButtonSpinnerInnerBox, bool>(nameof(IsShowHandle), true);
    
    public static readonly StyledProperty<bool> IsHandleFloatableProperty =
        AvaloniaProperty.Register<ButtonSpinnerInnerBox, bool>(nameof(IsHandleFloatable), true);
    
    public static readonly StyledProperty<Location> ButtonSpinnerLocationProperty =
        AvaloniaProperty.Register<ButtonSpinnerInnerBox, Location>(nameof(ButtonSpinnerLocation));

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
    
    public Location ButtonSpinnerLocation
    {
        get => GetValue(ButtonSpinnerLocationProperty);
        set => SetValue(ButtonSpinnerLocationProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<ButtonSpinnerInnerBox, Thickness> SpinnerBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, Thickness>(nameof(SpinnerBorderThickness),
            o => o.SpinnerBorderThickness,
            (o, v) => o.SpinnerBorderThickness = v);
    
    internal static readonly DirectProperty<ButtonSpinnerInnerBox, bool> HandleEffectiveVisibleProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, bool>(nameof(HandleEffectiveVisible),
            o => o.HandleEffectiveVisible,
            (o, v) => o.HandleEffectiveVisible = v);

    internal static readonly DirectProperty<ButtonSpinnerInnerBox, double> SpinnerHandleWidthProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, double>(nameof(SpinnerHandleWidth),
            o => o.SpinnerHandleWidth,
            (o, v) => o.SpinnerHandleWidth = v);
    
    private Thickness _spinnerBorderThickness;

    internal Thickness SpinnerBorderThickness
    {
        get => _spinnerBorderThickness;
        set => SetAndRaise(SpinnerBorderThicknessProperty, ref _spinnerBorderThickness, value);
    }
    
    private bool _handleEffectiveVisible;

    internal bool HandleEffectiveVisible
    {
        get => _handleEffectiveVisible;
        set => SetAndRaise(HandleEffectiveVisibleProperty, ref _handleEffectiveVisible, value);
    }
    
    private double _spinnerHandleWidth;

    internal double SpinnerHandleWidth
    {
        get => _spinnerHandleWidth;
        set => SetAndRaise(SpinnerHandleWidthProperty, ref _spinnerHandleWidth, value);
    }
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }

    #endregion

    private IDisposable? _mouseMoveDisposable;
    
    protected override void BuildEffectiveInnerBoxPadding()
    {
        EffectiveInnerBoxPadding = InnerBoxPadding;
        if (IsShowHandle && !IsHandleFloatable)
        {
            var padding = SpinnerHandleWidth;
            if (ButtonSpinnerLocation == Location.Right)
            {
                EffectiveInnerBoxPadding = new Thickness(InnerBoxPadding.Left, InnerBoxPadding.Top, padding,
                    InnerBoxPadding.Bottom);
            }
            else
            {
                EffectiveInnerBoxPadding = new Thickness(padding, InnerBoxPadding.Top, InnerBoxPadding.Right,
                    InnerBoxPadding.Bottom);
            }
        }
        else
        {
            EffectiveInnerBoxPadding = InnerBoxPadding;
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, SpinnerBorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureMoveProcessor();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _mouseMoveDisposable?.Dispose();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        BuildEffectiveInnerBoxPadding();
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
                    HandleEffectiveVisible = true;
                }
            }
            else
            {
                if (IsShowHandle && IsHandleFloatable)
                {
                    HandleEffectiveVisible = false;
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
                HandleEffectiveVisible = true;
            }
            else if (!IsShowHandle)
            {
                HandleEffectiveVisible = false;
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
            change.Property == SpinnerHandleWidthProperty)
        {
            BuildEffectiveInnerBoxPadding();
        }
    }
}