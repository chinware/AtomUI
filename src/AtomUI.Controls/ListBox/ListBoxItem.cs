using AtomUI.Media;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaListBoxItem = Avalonia.Controls.ListBoxItem;

public class ListBoxItem : AvaloniaListBoxItem
{
    #region 内部属性定义

    internal static readonly DirectProperty<ListBoxItem, SizeType> SizeTypeProperty =
        AvaloniaProperty.RegisterDirect<ListBoxItem, SizeType>(nameof(SizeType),
            o => o.SizeType,
            (o, v) => o.SizeType = v);

    private SizeType _sizeType = SizeType.Middle;

    internal SizeType SizeType
    {
        get => _sizeType;
        set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
    }

    internal static readonly DirectProperty<ListBoxItem, bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.RegisterDirect<ListBoxItem, bool>(nameof(DisabledItemHoverEffect),
            o => o.DisabledItemHoverEffect,
            (o, v) => o.DisabledItemHoverEffect = v);

    private bool _disabledItemHoverEffect;

    internal bool DisabledItemHoverEffect
    {
        get => _disabledItemHoverEffect;
        set => SetAndRaise(DisabledItemHoverEffectProperty, ref _disabledItemHoverEffect, value);
    }
    
    internal static readonly DirectProperty<ListBoxItem, bool> DisabledItemHoverAnimationProperty =
        AvaloniaProperty.RegisterDirect<ListBoxItem, bool>(nameof(DisabledItemHoverAnimation),
            o => o.DisabledItemHoverAnimation,
            (o, v) => o.DisabledItemHoverAnimation = v);

    private bool _disabledItemHoverAnimation = false;

    internal bool DisabledItemHoverAnimation
    {
        get => _disabledItemHoverAnimation;
        set => SetAndRaise(DisabledItemHoverAnimationProperty, ref _disabledItemHoverAnimation, value);
    }

    #endregion

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (!DisabledItemHoverAnimation)
        {
            Transitions ??= new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
            };
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == DisabledItemHoverAnimationProperty)
            {
                if (DisabledItemHoverAnimation)
                {
                    Transitions?.Clear();
                }
                else
                {
                    Transitions = new Transitions
                    {
                        AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
                    };
                }
            }
        }
    }
}