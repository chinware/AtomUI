using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input.Raw;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Internal;

public abstract class RangeInfoPickerInput : InfoPickerInput
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> SecondaryWatermarkProperty =
        AvaloniaProperty.Register<InfoPickerInput, string?>(nameof(SecondaryWatermark));
    
    public string? SecondaryWatermark
    {
        get => GetValue(SecondaryWatermarkProperty);
        set => SetValue(SecondaryWatermarkProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<RangeInfoPickerInput, RangeActivatedPart> RangeActivatedPartProperty =
        AvaloniaProperty.RegisterDirect<RangeInfoPickerInput, RangeActivatedPart>(nameof(RangeActivatedPart),
            o => o.RangeActivatedPart);

    private RangeActivatedPart _rangeActivatedPart;

    internal RangeActivatedPart RangeActivatedPart
    {
        get => _rangeActivatedPart;
        set => SetAndRaise(RangeActivatedPartProperty, ref _rangeActivatedPart, value);
    }

    internal static readonly StyledProperty<double> PickerIndicatorOffsetXProperty =
        AvaloniaProperty.Register<RangeInfoPickerInput, double>(nameof(PickerIndicatorOffsetX), double.NaN);

    internal double PickerIndicatorOffsetX
    {
        get => GetValue(PickerIndicatorOffsetXProperty);
        set => SetValue(PickerIndicatorOffsetXProperty, value);
    }

    internal static readonly StyledProperty<double> PickerIndicatorOffsetYProperty =
        AvaloniaProperty.Register<RangeInfoPickerInput, double>(nameof(PickerIndicatorOffsetY));

    internal double PickerIndicatorOffsetY
    {
        get => GetValue(PickerIndicatorOffsetYProperty);
        set => SetValue(PickerIndicatorOffsetYProperty, value);
    }
    
    internal static readonly StyledProperty<string?> SecondaryTextProperty =
        AvaloniaProperty.Register<TextBlock, string?>(nameof(SecondaryText));
    
    internal string? SecondaryText
    {
        get => GetValue(SecondaryTextProperty);
        set => SetValue(SecondaryTextProperty, value);
    }

    #endregion
        
    static RangeInfoPickerInput()
    {
        AffectsArrange<RangeInfoPickerInput>(PickerIndicatorOffsetXProperty, PickerIndicatorOffsetYProperty);
    }
    
    private protected Rectangle? _rangePickerIndicator;
    private protected Icon? _rangePickerArrow;
    private protected TextBox? _secondaryInfoInputBox;

    public override void Clear()
    {
        _infoInputBox?.Clear();
        _secondaryInfoInputBox?.Clear();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.RunThemeTokenBindingActions();
        _secondaryInfoInputBox = e.NameScope.Get<TextBox>(RangeInfoPickerInputTheme.SecondaryInfoInputBoxPart);
        _rangePickerIndicator  = e.NameScope.Get<Rectangle>(RangeInfoPickerInputTheme.RangePickerIndicatorPart);
        _rangePickerArrow = e.NameScope.Get<Icon>(RangeInfoPickerInputTheme.RangePickerArrowPart);
        
        SetupTransitions();
    }

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            if (_rangePickerIndicator != null)
            {
                _rangePickerIndicator.Transitions ??= new Transitions
                {
                    AnimationUtils.CreateTransition<DoubleTransition>(OpacityProperty),
                    AnimationUtils.CreateTransition<DoubleTransition>(OpacityProperty)
                };
            }
        
            Transitions ??= new Transitions
            {
                AnimationUtils.CreateTransition<DoubleTransition>(PickerIndicatorOffsetXProperty)
            };
        }
        else
        {
            if (_rangePickerIndicator != null)
            {
                _rangePickerIndicator.Transitions = null;
            }

            Transitions = null;
        }
    }
    
    protected override bool FlyoutOpenPredicate(Point position)
    {
        if (!IsEnabled)
        {
            return false;
        }
        if (IsPointerInInfoInputBox(position))
        {
            RangeActivatedPart = RangeActivatedPart.Start;
            return true;
        }
        
        if (IsPointerInSecondaryTextBox(position))
        {
            RangeActivatedPart = RangeActivatedPart.End;
            return true;
        }
        
        return false;
    }
    
    private bool IsPointerInInfoInputBox(Point position)
    {
        if (_infoInputBox is null)
        {
            return false;
        }

        var pos = _infoInputBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
        if (!pos.HasValue)
        {
            return false;
        }

        var targetWidth  = _infoInputBox.Bounds.Width;
        var targetHeight = _infoInputBox.Bounds.Height;
        var startOffsetX = pos.Value.X;
        var endOffsetX   = startOffsetX + targetWidth;
        var offsetY      = pos.Value.Y;
        if (InnerLeftContent is Control leftContent)
        {
            var leftContentPos = leftContent.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
            if (leftContentPos.HasValue)
            {
                startOffsetX = leftContentPos.Value.X + leftContent.Bounds.Width;
            }
        }

        targetWidth = endOffsetX - startOffsetX;
        var bounds = new Rect(new Point(startOffsetX, offsetY), new Size(targetWidth, targetHeight));
        if (bounds.Contains(position))
        {
            return true;
        }

        return false;
    }

    private bool IsPointerInSecondaryTextBox(Point position)
    {
        if (_secondaryInfoInputBox is null)
        {
            return false;
        }

        var pos = _secondaryInfoInputBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
        if (!pos.HasValue)
        {
            return false;
        }

        var targetWidth  = _secondaryInfoInputBox.Bounds.Width;
        var targetHeight = _secondaryInfoInputBox.Bounds.Height;
        var startOffsetX = pos.Value.X;
        var endOffsetX   = startOffsetX + targetWidth;
        var offsetY      = pos.Value.Y;
        if (InnerLeftContent is Control leftContent)
        {
            var leftContentPos = leftContent.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
            if (leftContentPos.HasValue)
            {
                startOffsetX = leftContentPos.Value.X + leftContent.Bounds.Width;
            }
        }

        targetWidth = endOffsetX - startOffsetX;
        var bounds = new Rect(new Point(startOffsetX, offsetY), new Size(targetWidth, targetHeight));
        if (bounds.Contains(position))
        {
            return true;
        }

        return false;
    }
     
    protected override bool ClickHideFlyoutPredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        if (hostProvider.PopupHost != args.Root)
        {
            var inRangeStart = IsPointerInInfoInputBox(args.Position);
            var inRangeEnd   = IsPointerInSecondaryTextBox(args.Position);

            if (inRangeStart)
            {
                RangeActivatedPart = RangeActivatedPart.Start;
            }

            if (inRangeEnd)
            {
                RangeActivatedPart = RangeActivatedPart.End;
            }

            if (!inRangeStart && !inRangeEnd)
            {
                return true;
            }
        }

        return false;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RangeActivatedPartProperty)
        {
            HandleRangeActivatedPartChanged();
        }

        if (this.IsAttachedToVisualTree())
        {
            SetupTransitions();
        }
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var borderThickness = _decoratedBox?.BorderThickness ?? default;
        var size            = base.ArrangeOverride(finalSize).Inflate(borderThickness);
        if (_rangePickerIndicator is not null)
        {
            Canvas.SetLeft(_rangePickerIndicator, PickerIndicatorOffsetX);
            Canvas.SetTop(_rangePickerIndicator, PickerIndicatorOffsetY);
        }

        return size;
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        if (_decoratedBox is not null)
        {
            PickerIndicatorOffsetY = _decoratedBox.DesiredSize.Height - _rangePickerIndicator!.Height;
        }
    
        if (double.IsNaN(PickerIndicatorOffsetX))
        {
            if (_rangeActivatedPart == RangeActivatedPart.None)
            {
                var offset = _infoInputBox!.TranslatePoint(new Point(0, 0), this) ?? default;
                PickerIndicatorOffsetX = offset.X;
            }
        }
    
        return size;
    }
    
    protected override void NotifyFlyoutAboutToClose(bool selectedIsValid)
    {
        RangeActivatedPart = RangeActivatedPart.None;
    }
    
    protected virtual void HandleRangeActivatedPartChanged()
    {
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            PickerPlacement = PlacementMode.BottomEdgeAlignedLeft;
            _infoInputBox!.Focus();
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            PickerPlacement = PlacementMode.BottomEdgeAlignedRight;
            _secondaryInfoInputBox!.Focus();
        }

        SetupPickerIndicatorPosition();
    }
    
    protected void SetupPickerIndicatorPosition()
    {
        if (_rangePickerIndicator is null ||
            _decoratedBox is null ||
            _infoInputBox is null ||
            _secondaryInfoInputBox is null)
        {
            return;
        }

        if (_rangeActivatedPart == RangeActivatedPart.None)
        {
            _rangePickerIndicator.Opacity = 0;
        }
        else if (_rangeActivatedPart == RangeActivatedPart.Start)
        {
            _rangePickerIndicator.Opacity = 1;
            _rangePickerIndicator.Width   = _infoInputBox.Bounds.Width;
            var offset = _infoInputBox.TranslatePoint(new Point(0, 0), this) ?? default;
            PickerIndicatorOffsetX = offset.X;
        }
        else if (_rangeActivatedPart == RangeActivatedPart.End)
        {
            _rangePickerIndicator.Opacity = 1;
            _rangePickerIndicator.Width   = _secondaryInfoInputBox.Bounds.Width;
            var offset = _secondaryInfoInputBox.TranslatePoint(new Point(0, 0), this) ?? default;
            PickerIndicatorOffsetX = offset.X;
        }
    }
}

internal enum RangeActivatedPart
{
    None,
    Start,
    End
}