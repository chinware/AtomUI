﻿using AtomUI.Controls.Primitives.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input.Raw;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

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
    
    private protected Rectangle? RangePickerIndicator;
    private protected Icon? RangePickerArrow;
    private protected TextBox? SecondaryInfoInputBox;
    private TopLevel? _topLevel;

    public override void Clear()
    {
        InfoInputBox?.Clear();
        SecondaryInfoInputBox?.Clear();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SecondaryInfoInputBox = e.NameScope.Get<TextBox>(RangeInfoPickerInputThemeConstants.SecondaryInfoInputBoxPart);
        RangePickerIndicator  = e.NameScope.Get<Rectangle>(RangeInfoPickerInputThemeConstants.RangePickerIndicatorPart);
        RangePickerArrow = e.NameScope.Get<Icon>(RangeInfoPickerInputThemeConstants.RangePickerArrowPart);
        
        ConfigureTransitions();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _topLevel = TopLevel.GetTopLevel(this);
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (RangePickerIndicator != null)
            {
                RangePickerIndicator.Transitions ??= new Transitions
                {
                    TransitionUtils.CreateTransition<DoubleTransition>(OpacityProperty),
                    TransitionUtils.CreateTransition<DoubleTransition>(OpacityProperty)
                };
            }
        
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<DoubleTransition>(PickerIndicatorOffsetXProperty)
            };
        }
        else
        {
            if (RangePickerIndicator != null)
            {
                RangePickerIndicator.Transitions = null;
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
        if (InfoInputBox is null || _topLevel is null)
        {
            return false;
        }

        var pos = InfoInputBox.TranslatePoint(new Point(0, 0), _topLevel);
        if (!pos.HasValue)
        {
            return false;
        }

        var targetWidth  = InfoInputBox.Bounds.Width;
        var targetHeight = InfoInputBox.Bounds.Height;
        var startOffsetX = pos.Value.X;
        var endOffsetX   = startOffsetX + targetWidth;
        var offsetY      = pos.Value.Y;
        if (InnerLeftContent is Control leftContent)
        {
            var leftContentPos = leftContent.TranslatePoint(new Point(0, 0), _topLevel);
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
        if (SecondaryInfoInputBox is null || _topLevel is null)
        {
            return false;
        }

        var pos = SecondaryInfoInputBox.TranslatePoint(new Point(0, 0), _topLevel);
        if (!pos.HasValue)
        {
            return false;
        }

        var targetWidth  = SecondaryInfoInputBox.Bounds.Width;
        var targetHeight = SecondaryInfoInputBox.Bounds.Height;
        var startOffsetX = pos.Value.X;
        var endOffsetX   = startOffsetX + targetWidth;
        var offsetY      = pos.Value.Y;
        if (InnerLeftContent is Control leftContent)
        {
            var leftContentPos = leftContent.TranslatePoint(new Point(0, 0), _topLevel);
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
        if (hostProvider.PopupHost != null && hostProvider.PopupHost != args.Root)
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
            ConfigureTransitions();
        }
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var borderThickness = DecoratedBox?.BorderThickness ?? default;
        var size            = base.ArrangeOverride(finalSize).Inflate(borderThickness);
        if (RangePickerIndicator is not null)
        {
            Canvas.SetLeft(RangePickerIndicator, PickerIndicatorOffsetX);
            Canvas.SetTop(RangePickerIndicator, PickerIndicatorOffsetY);
        }

        return size;
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        if (DecoratedBox is not null)
        {
            PickerIndicatorOffsetY = DecoratedBox.DesiredSize.Height - RangePickerIndicator!.Height;
        }
    
        if (double.IsNaN(PickerIndicatorOffsetX))
        {
            if (_rangeActivatedPart == RangeActivatedPart.None)
            {
                var offset = InfoInputBox!.TranslatePoint(new Point(0, 0), this) ?? default;
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
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            PickerPlacement = PlacementMode.BottomEdgeAlignedRight;
        }

        SetupPickerIndicatorPosition();
    }
    
    protected void SetupPickerIndicatorPosition()
    {
        if (RangePickerIndicator is null ||
            DecoratedBox is null ||
            InfoInputBox is null ||
            SecondaryInfoInputBox is null)
        {
            return;
        }
        
        if (_rangeActivatedPart == RangeActivatedPart.None)
        {
            RangePickerIndicator.Opacity = 0;
        }
        else if (_rangeActivatedPart == RangeActivatedPart.Start)
        {
            RangePickerIndicator.Opacity = 1;
            RangePickerIndicator.Width   = InfoInputBox.Bounds.Width;
            var offset = InfoInputBox.TranslatePoint(new Point(0, 0), this) ?? default;
            PickerIndicatorOffsetX = offset.X;
        }
        else if (_rangeActivatedPart == RangeActivatedPart.End)
        {
            RangePickerIndicator.Opacity = 1;
            RangePickerIndicator.Width   = SecondaryInfoInputBox.Bounds.Width;
            var offset = SecondaryInfoInputBox.TranslatePoint(new Point(0, 0), this) ?? default;
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