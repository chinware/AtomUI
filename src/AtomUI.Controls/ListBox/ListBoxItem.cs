using AtomUI.Controls.Utils;
using AtomUI.Media;
using Avalonia;
using Avalonia.Animation;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaListBoxItem = Avalonia.Controls.ListBoxItem;

public class ListBoxItem : AvaloniaListBoxItem
{
    #region 内部属性定义
    internal static readonly DirectProperty<ListBoxItem, SizeType> SizeTypeProperty =
        AvaloniaProperty.RegisterDirect<ListBoxItem, SizeType>(nameof(SizeType),
            o => o.SizeType,
            (o, v) => o.SizeType = v);

    internal static readonly DirectProperty<ListBoxItem, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<ListBoxItem, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    internal static readonly DirectProperty<ListBoxItem, bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.RegisterDirect<ListBoxItem, bool>(nameof(DisabledItemHoverEffect),
            o => o.DisabledItemHoverEffect,
            (o, v) => o.DisabledItemHoverEffect = v);

    private SizeType _sizeType = SizeType.Middle;

    internal SizeType SizeType
    {
        get => _sizeType;
        set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
    }

    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }

    private bool _disabledItemHoverEffect;

    internal bool DisabledItemHoverEffect
    {
        get => _disabledItemHoverEffect;
        set => SetAndRaise(DisabledItemHoverEffectProperty, ref _disabledItemHoverEffect, value);
    }

    #endregion

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SetupTransitions();
    }

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
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                SetupTransitions();
            }
        }
    }
}