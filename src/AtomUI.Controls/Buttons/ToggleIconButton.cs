﻿using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class ToggleIconButton : ToggleButton
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> CheckedIconProperty
        = AvaloniaProperty.Register<ToggleIconButton, Icon?>(nameof(CheckedIcon));

    public static readonly StyledProperty<Icon?> UnCheckedIconProperty
        = AvaloniaProperty.Register<ToggleIconButton, Icon?>(nameof(UnCheckedIcon));

    public static readonly StyledProperty<double> IconWidthProperty
        = AvaloniaProperty.Register<ToggleIconButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty
        = AvaloniaProperty.Register<ToggleIconButton, double>(nameof(IconHeight));

    public Icon? CheckedIcon
    {
        get => GetValue(CheckedIconProperty);
        set => SetValue(CheckedIconProperty, value);
    }

    public Icon? UnCheckedIcon
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

    protected virtual void ConfigureIcon(Icon Icon)
    {
        Icon.SetCurrentValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        Icon.SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        UIStructureUtils.SetTemplateParent(Icon, this);
        BindUtils.RelayBind(this, IconWidthProperty, Icon, WidthProperty);
        BindUtils.RelayBind(this, IconHeightProperty, Icon, HeightProperty);
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
                var icon = IsChecked.HasValue && IsChecked.Value ? CheckedIcon : UnCheckedIcon;
                if (icon is not null)
                {
                    if (!PseudoClasses.Contains(StdPseudoClass.Disabled))
                    {
                        icon.IconMode = IconMode.Normal;
                        if (PseudoClasses.Contains(StdPseudoClass.Pressed))
                        {
                            icon.IconMode = IconMode.Selected;
                        }
                        else if (PseudoClasses.Contains(StdPseudoClass.PointerOver))
                        {
                            icon.IconMode = IconMode.Active;
                        }
                    }
                    else
                    {
                        icon.IconMode = IconMode.Disabled;
                    }
                }
            }
        }

        if (change.Property == CheckedIconProperty ||
            change.Property == UnCheckedIconProperty)
        {
            if (change.NewValue is Icon newIcon)
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
    
    public bool HitTest(Point point)
    {
        return NotifyHistTest(point);
    }

    protected virtual bool NotifyHistTest(Point point)
    {
        return true;
    }
}