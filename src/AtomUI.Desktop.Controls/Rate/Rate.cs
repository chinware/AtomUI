using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class Rate : TemplatedControl, 
                    IMotionAwareControl, 
                    ISizeTypeAware,
                    IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AvaloniaProperty.Register<Rate, bool>(nameof(IsAllowClear), true);
    
    public static readonly StyledProperty<bool> IsAllowHalfProperty =
        AvaloniaProperty.Register<Rate, bool>(nameof(IsAllowHalf));
    
    public static readonly StyledProperty<object?> CharacterProperty =
        AvaloniaProperty.Register<Rate, object?>(nameof(Character));
    
    public static readonly StyledProperty<IBrush?> StarColorProperty =
        AvaloniaProperty.Register<Rate, IBrush?>(nameof(StarColor));
    
    public static readonly StyledProperty<IBrush?> StarBgColorProperty =
        AvaloniaProperty.Register<Rate, IBrush?>(nameof(StarBgColor));
    
    public static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<Rate, int>(nameof(Count), 5);
    
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<Rate, double>(nameof(Value));
    
    public static readonly StyledProperty<double> DefaultValueProperty =
        AvaloniaProperty.Register<Rate, double>(nameof(DefaultValue), 0);
    
    public static readonly StyledProperty<bool> IsKeyboardEnabledProperty =
        AvaloniaProperty.Register<Rate, bool>(nameof(IsKeyboardEnabled), true);

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Rate>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Rate>();
    
    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }
    
    public bool IsAllowHalf
    {
        get => GetValue(IsAllowHalfProperty);
        set => SetValue(IsAllowHalfProperty, value);
    }
    
    public object? Character
    {
        get => GetValue(CharacterProperty);
        set => SetValue(CharacterProperty, value);
    }
    
    public IBrush? StarColor
    {
        get => GetValue(StarColorProperty);
        set => SetValue(StarColorProperty, value);
    }
    
    public IBrush? StarBgColor
    {
        get => GetValue(StarBgColorProperty);
        set => SetValue(StarBgColorProperty, value);
    }

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }
    
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public double DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }

    public bool IsKeyboardEnabled
    {
        get => GetValue(IsKeyboardEnabledProperty);
        set => SetValue(IsKeyboardEnabledProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 公共事件定义

    public event EventHandler<RateValueChangedEventArgs>? ValueChanged;
    public event EventHandler<RateValueChangedEventArgs>? HoverValueChanged;

    #endregion
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<Rate, double> EffectiveValueProperty =
        AvaloniaProperty.RegisterDirect<Rate, double>(
            nameof(EffectiveValue),
            o => o.EffectiveValue,
            (o, v) => o.EffectiveValue = v);
    
    private double _effectiveValue;

    internal double EffectiveValue
    {
        get => _effectiveValue;
        set => SetAndRaise(EffectiveValueProperty, ref _effectiveValue, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => RateToken.ID;
    #endregion
    
    private ItemsControl? _itemsControl;
    private IDisposable? _pointerPositionDisposable;
    
    static Rate()
    {
        AffectsMeasure<Rate>(CountProperty, CharacterProperty);
        AffectsRender<Rate>(ValueProperty, StarColorProperty);
    }
    
    public Rate()
    {
        this.RegisterResources();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Character == null)
        {
            SetCurrentValue(CharacterProperty, new StarFilled());
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == CountProperty)
            {
                HandleCountChanged();
            }
        }

        if (change.Property == ValueProperty)
        {
            SetCurrentValue(EffectiveValueProperty, Value);
        }
        else if (change.Property == EffectiveValueProperty)
        {
            HandleValueChanged();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
        _pointerPositionDisposable = inputManager.Process.Subscribe(DetectPointerPosition);
    }
    
    private void DetectPointerPosition(RawInputEventArgs args)
    {
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            var pos = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!) ?? default;
            var bounds =  new Rect(pos, DesiredSize);
            if (!bounds.Contains(pointerEventArgs.Position))
            {
                SetCurrentValue(EffectiveValueProperty, Value);
            }
        }
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsControl = e.NameScope.Find<ItemsControl>(RateThemeConstants.RateItemsPart);
        HandleCountChanged();
        HandleValueChanged();
    }

    private void HandleCountChanged()
    {
        if (_itemsControl != null)
        {
            _itemsControl.Items.Clear();
            for (var i = 0; i < Count; i++)
            {
                var rateItem = new RateItem();
                _itemsControl.Items.Add(rateItem);
            }
        }

        HandleValueChanged();
    }

    private void HandleValueChanged()
    {
        if (_itemsControl != null)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_itemsControl.Items[i] is RateItem rateItem)
                {
                    if (MathUtils.LessThan(i, EffectiveValue))
                    {
                        if (IsAllowHalf)
                        {
                            var delta = EffectiveValue - i;
                            if (MathUtils.LessThanOrClose(delta, 0.5d))
                            {
                                rateItem.SelectedState = RateItemSelectedState.HalfSelected;
                            }
                            else
                            {
                                rateItem.SelectedState = RateItemSelectedState.FullSelected;
                            }
                        }
                        else
                        {
                            rateItem.SelectedState = RateItemSelectedState.FullSelected;
                        }
                    }
                    else
                    {
                        rateItem.SelectedState = RateItemSelectedState.None;
                    }
                }
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_itemsControl != null)
        {
            var point   = e.GetPosition(_itemsControl);
            var offsetX = point.X;
            if (Count > 0)
            {
                var firstItem = _itemsControl.ContainerFromIndex(0) as RateItem;
                var lastItem = _itemsControl.ContainerFromItem(Count - 1) as RateItem;
                if (firstItem != null)
                {
                    if (offsetX < firstItem.Bounds.Left)
                    {
                        SetCurrentValue(EffectiveValueProperty, 0);
                    }
                }

                if (lastItem != null)
                {
                    if (offsetX > lastItem.Bounds.Right)
                    {
                        SetCurrentValue(EffectiveValueProperty, Count);
                    }
                }
            }
            for (var i = 0; i < Count; i++)
            {
                if (_itemsControl.ContainerFromIndex(i) is RateItem rateItem)
                {
                    var bounds = rateItem.Bounds;
                    var left   = bounds.Left;
                    var right  = bounds.Right;
                    var middle = bounds.Center.X;
                    if (MathUtils.GreaterThanOrClose(offsetX, left) && MathUtils.LessThanOrClose(offsetX, middle))
                    {
                        SetCurrentValue(EffectiveValueProperty, i + 0.5);
                    }
                    else if (MathUtils.GreaterThan(offsetX, middle) && MathUtils.LessThan(offsetX, right))
                    {
                        SetCurrentValue(EffectiveValueProperty, i + 1);
                    }
                }
            }
        }
    }
}