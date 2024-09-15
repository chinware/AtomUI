using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public class IconButton : AvaloniaButton, ICustomHitTest
{
    #region 公共属性定义

    public static readonly StyledProperty<PathIcon?> IconProperty
        = AvaloniaProperty.Register<IconButton, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        PathIcon.LoadingAnimationProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        PathIcon.LoadingAnimationDurationProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<double> IconWidthProperty
        = AvaloniaProperty.Register<IconButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty
        = AvaloniaProperty.Register<IconButton, double>(nameof(IconHeight));

    public static readonly StyledProperty<bool> IsEnableHoverEffectProperty
        = AvaloniaProperty.Register<IconButton, bool>(nameof(IsEnableHoverEffect));

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IconAnimation LoadingAnimation
    {
        get => GetValue(LoadingAnimationProperty);
        set => SetValue(LoadingAnimationProperty, value);
    }

    public TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
    }

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    public bool IsEnableHoverEffect
    {
        get => GetValue(IsEnableHoverEffectProperty);
        set => SetValue(IsEnableHoverEffectProperty, value);
    }

    #endregion

    private ControlStyleState _styleState;

    static IconButton()
    {
        AffectsMeasure<IconButton>(IconProperty);
    }

    public IconButton()
    {
        Cursor = new Cursor(StandardCursorType.Hand);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupIcon();
        SetupIconMode();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (VisualRoot is not null)
        {
            if (e.Property == IconProperty)
            {
                var oldIcon = e.GetOldValue<PathIcon?>();
                if (oldIcon is not null)
                {
                    ((ISetLogicalParent)oldIcon).SetParent(null);
                }

                SetupIcon();
            }
            else if (e.Property == IsPressedProperty ||
                     e.Property == IsPointerOverProperty ||
                     e.Property == IsEnabledProperty)
            {
                SetupIconMode();
            }
        }
    }

    private void SetupIcon()
    {
        if (Icon is not null)
        {
            BindUtils.RelayBind(this, LoadingAnimationProperty, Icon, PathIcon.LoadingAnimationProperty);
            BindUtils.RelayBind(this, LoadingAnimationDurationProperty, Icon,
                PathIcon.LoadingAnimationDurationProperty);
            BindUtils.RelayBind(this, IconHeightProperty, Icon, HeightProperty);
            BindUtils.RelayBind(this, IconWidthProperty, Icon, WidthProperty);
        }
    }

    private void SetupIconMode()
    {
        CollectStyleState();
        if (Icon is not null)
        {
            if (_styleState.HasFlag(ControlStyleState.Enabled))
            {
                Icon.IconMode = IconMode.Normal;
                if (_styleState.HasFlag(ControlStyleState.Sunken))
                {
                    Icon.IconMode = IconMode.Selected;
                }
                else if (_styleState.HasFlag(ControlStyleState.MouseOver))
                {
                    Icon.IconMode = IconMode.Active;
                }
            }
            else
            {
                Icon.IconMode = IconMode.Disabled;
            }
        }
    }

    private void CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);
        if (IsPressed)
        {
            _styleState |= ControlStyleState.Sunken;
        }
        else
        {
            _styleState |= ControlStyleState.Raised;
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}