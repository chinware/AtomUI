﻿using AtomUI.Controls.Utils;
using AtomUI.Media;
using Avalonia;
using Avalonia.Animation;

namespace AtomUI.Controls;

using AvaloniaListBoxItem = Avalonia.Controls.ListBoxItem;

public class ListBoxItem : AvaloniaListBoxItem
{
    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ListBoxItem>();

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ListBoxItem>();

    internal static readonly DirectProperty<ListBoxItem, bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.RegisterDirect<ListBoxItem, bool>(nameof(DisabledItemHoverEffect),
            o => o.DisabledItemHoverEffect,
            (o, v) => o.DisabledItemHoverEffect = v);

    internal SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    private bool _disabledItemHoverEffect;

    internal bool DisabledItemHoverEffect
    {
        get => _disabledItemHoverEffect;
        set => SetAndRaise(DisabledItemHoverEffectProperty, ref _disabledItemHoverEffect, value);
    }

    #endregion

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToLogicalTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                SetupTransitions();
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupTransitions();
    }
}