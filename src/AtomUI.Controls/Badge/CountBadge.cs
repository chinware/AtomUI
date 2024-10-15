﻿using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum CountBadgeSize
{
    Default,
    Small
}

public class CountBadge : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> BadgeColorProperty
        = AvaloniaProperty.Register<CountBadge, string?>(
            nameof(BadgeColor));

    public static readonly StyledProperty<int> CountProperty
        = AvaloniaProperty.Register<CountBadge, int>(nameof(Count),
            coerce: (o, v) => Math.Max(0, v));

    public static readonly StyledProperty<Control?> DecoratedTargetProperty =
        AvaloniaProperty.Register<CountBadge, Control?>(nameof(DecoratedTarget));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<CountBadge, Point>(nameof(Offset));

    public static readonly StyledProperty<int> OverflowCountProperty =
        AvaloniaProperty.Register<CountBadge, int>(nameof(OverflowCount), 99,
            coerce: (o, v) => Math.Max(0, v));

    public static readonly StyledProperty<bool> ShowZeroProperty =
        AvaloniaProperty.Register<CountBadge, bool>(nameof(ShowZero));

    public static readonly StyledProperty<CountBadgeSize> SizeProperty =
        AvaloniaProperty.Register<CountBadge, CountBadgeSize>(nameof(Size));

    public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
        AvaloniaProperty.Register<CountBadge, bool>(nameof(BadgeIsVisible), true);
    
    public string? BadgeColor
    {
        get => GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
    }

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    [Content]
    public Control? DecoratedTarget
    {
        get => GetValue(DecoratedTargetProperty);
        set => SetValue(DecoratedTargetProperty, value);
    }

    public Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public int OverflowCount
    {
        get => GetValue(OverflowCountProperty);
        set => SetValue(OverflowCountProperty, value);
    }

    public bool ShowZero
    {
        get => GetValue(ShowZeroProperty);
        set => SetValue(ShowZeroProperty, value);
    }

    public CountBadgeSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool BadgeIsVisible
    {
        get => GetValue(BadgeIsVisibleProperty);
        set => SetValue(BadgeIsVisibleProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        AvaloniaProperty.Register<CountBadge, TimeSpan>(
            nameof(MotionDuration));
    
    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    #endregion
    
    private CountBadgeAdorner? _badgeAdorner;
    private AdornerLayer? _adornerLayer;
    private bool _isInitialized;
    
    static CountBadge()
    {
        AffectsMeasure<CountBadge>(DecoratedTargetProperty,
            CountProperty,
            OverflowCountProperty,
            SizeProperty);
        AffectsRender<CountBadge>(BadgeColorProperty, OffsetProperty);
    }
    
    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (!_isInitialized)
        {
            if (DecoratedTarget is null)
            {
                CreateBadgeAdorner();
            }

            SetupShowZero();
            
            _isInitialized = true;
        }
    }

    private CountBadgeAdorner CreateBadgeAdorner()
    {
        if (_badgeAdorner is null)
        {
            _badgeAdorner = new CountBadgeAdorner();
            SetupTokenBindings();
            HandleDecoratedTargetChanged();
            if (BadgeColor is not null)
            {
                SetupBadgeColor(BadgeColor);
            }
        }

        return _badgeAdorner;
    }

    private void PrepareAdorner()
    {
        var badgeAdorner = CreateBadgeAdorner();
        if (DecoratedTarget is not null)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (_adornerLayer == null)
            {
                return;
            }
            badgeAdorner.ApplyToTarget(_adornerLayer, this);
        }
        else
        {
            badgeAdorner.ApplyToTarget(null, this);
        }
    }

    private void HideAdorner(bool enableMotion)
    {
        // 这里需要抛出异常吗？
        if (_badgeAdorner is null)
        {
            return;
        }
        _badgeAdorner.DetachFromTarget(_adornerLayer, enableMotion);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (BadgeIsVisible)
        {
            PrepareAdorner();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideAdorner(false);
    }

    private void SetupTokenBindings()
    {
        if (_badgeAdorner is not null)
        {
            BindUtils.RelayBind(this, OffsetProperty, _badgeAdorner, CountBadgeAdorner.OffsetProperty);
            BindUtils.RelayBind(this, SizeProperty, _badgeAdorner, CountBadgeAdorner.SizeProperty);
            BindUtils.RelayBind(this, OverflowCountProperty, _badgeAdorner, CountBadgeAdorner.OverflowCountProperty);
            BindUtils.RelayBind(this, CountProperty, _badgeAdorner, CountBadgeAdorner.CountProperty);
        }

        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, GlobalTokenResourceKey.MotionDurationSlow);
    }

    private void HandleDecoratedTargetChanged()
    {
        if (_badgeAdorner is not null)
        {
            if (DecoratedTarget is null)
            {
                ((ISetLogicalParent)_badgeAdorner).SetParent(this);
                VisualChildren.Add(_badgeAdorner);
                _badgeAdorner.IsAdornerMode = false;
            }
            else if (DecoratedTarget is not null)
            {
                _badgeAdorner.IsAdornerMode = true;
                ((ISetLogicalParent)DecoratedTarget).SetParent(this);
                VisualChildren.Add(DecoratedTarget);
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == BadgeIsVisibleProperty)
        {
            if (BadgeIsVisible)
            {
                SetupShowZero();
                PrepareAdorner();
            }
            else
            {
                HideAdorner(true);
            }
        }

        if (VisualRoot is not null)
        {
            if (e.Property == DecoratedTargetProperty)
            {
                HandleDecoratedTargetChanged();
            }

            if (e.Property == BadgeColorProperty)
            {
                SetupBadgeColor(e.GetNewValue<string>());
            }
        }

        if (e.Property == CountProperty ||
            e.Property == ShowZeroProperty)
        {
            SetupShowZero();
        }
    }

    private void SetupShowZero()
    {
        if (Count == 0 && !ShowZero)
        {
            BadgeIsVisible = false;
        }
        else if (Count > 0)
        {
            BadgeIsVisible = true;
        }
    }

    private void SetupBadgeColor(string colorStr)
    {
        colorStr = colorStr.Trim().ToLower();

        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            if (presetColor.Type.ToString().ToLower() == colorStr)
            {
                _badgeAdorner!.BadgeColor = new SolidColorBrush(presetColor.Color());
                return;
            }
        }

        if (Color.TryParse(colorStr, out var color))
        {
            _badgeAdorner!.BadgeColor = new SolidColorBrush(color);
        }
    }
}