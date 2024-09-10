using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class ToggleIconButton : ToggleButton
{
    #region 公共属性定义

    public static readonly StyledProperty<PathIcon?> CheckedIconProperty
        = AvaloniaProperty.Register<ToggleIconButton, PathIcon?>(nameof(CheckedIcon));

    public static readonly StyledProperty<PathIcon?> UnCheckedIconProperty
        = AvaloniaProperty.Register<ToggleIconButton, PathIcon?>(nameof(UnCheckedIcon));

    public static readonly StyledProperty<double> IconWidthProperty
        = AvaloniaProperty.Register<ToggleIconButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty
        = AvaloniaProperty.Register<ToggleIconButton, double>(nameof(IconHeight));

    public PathIcon? CheckedIcon
    {
        get => GetValue(CheckedIconProperty);
        set => SetValue(CheckedIconProperty, value);
    }

    public PathIcon? UnCheckedIcon
    {
        get => GetValue(UnCheckedIconProperty);
        set => SetValue(UnCheckedIconProperty, value);
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

    #endregion

    private ControlStyleState _styleState;

    static ToggleIconButton()
    {
        AffectsMeasure<ToggleIconButton>(CheckedIconProperty);
        AffectsMeasure<ToggleIconButton>(UnCheckedIconProperty);
        AffectsMeasure<ToggleIconButton>(IsCheckedProperty);
    }

    public ToggleIconButton()
    {
        SetCurrentValue(CursorProperty, new Cursor(StandardCursorType.Hand));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (CheckedIcon is not null)
        {
            ConfigureIcon(CheckedIcon);
        }

        if (UnCheckedIcon is not null)
        {
            ConfigureIcon(UnCheckedIcon);
        }

        ApplyIconToContent();
    }

    protected virtual void ConfigureIcon(PathIcon icon)
    {
        icon.SetCurrentValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        icon.SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        UIStructureUtils.SetTemplateParent(icon, this);
        BindUtils.RelayBind(this, IconWidthProperty, icon, WidthProperty);
        BindUtils.RelayBind(this, IconHeightProperty, icon, HeightProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == IsCheckedProperty)
            {
                ApplyIconToContent();
            }
            else if (change.Property == IsPressedProperty ||
                     change.Property == IsPointerOverProperty)
            {
                CollectStyleState();
                var pathIcon = IsChecked.HasValue && IsChecked.Value ? CheckedIcon : UnCheckedIcon;
                if (pathIcon is not null)
                {
                    if (_styleState.HasFlag(ControlStyleState.Enabled))
                    {
                        pathIcon.IconMode = IconMode.Normal;
                        if (_styleState.HasFlag(ControlStyleState.Sunken))
                        {
                            pathIcon.IconMode = IconMode.Selected;
                        }
                        else if (_styleState.HasFlag(ControlStyleState.MouseOver))
                        {
                            pathIcon.IconMode = IconMode.Active;
                        }
                    }
                    else
                    {
                        pathIcon.IconMode = IconMode.Disabled;
                    }
                }
            }
        }

        if (change.Property == CheckedIconProperty ||
            change.Property == UnCheckedIconProperty)
        {
            if (change.NewValue is PathIcon newIcon)
            {
                ConfigureIcon(newIcon);
                ApplyIconToContent();
            }
        }
    }

    internal virtual void ApplyIconToContent()
    {
        if (IsChecked.HasValue && IsChecked.Value)
        {
            Content = CheckedIcon;
        }
        else
        {
            Content = UnCheckedIcon;
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
        return NotifyHistTest(point);
    }

    protected virtual bool NotifyHistTest(Point point)
    {
        return true;
    }
}