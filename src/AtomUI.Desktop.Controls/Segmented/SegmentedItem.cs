using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected, SegmentedPseudoClass.HasIcon)]
public class SegmentedItem : ContentControl, ISelectable
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<SegmentedItem>();

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<SegmentedItem, Icon?>(nameof(Icon));

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<SegmentedItem>();
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SegmentedItem>();
    
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
    
    #endregion
    
    static SegmentedItem()
    {
        SelectableMixin.Attach<SegmentedItem>(IsSelectedProperty);
        PressedMixin.Attach<SegmentedItem>();
        FocusableProperty.OverrideDefaultValue<SegmentedItem>(true);
        AffectsRender<SegmentedItem>(BackgroundProperty);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!e.Handled && ItemsControl.ItemsControlFromItemContainer(this) is Segmented owner)
        {
            var p = e.GetCurrentPoint(this);

            if (p.Properties.PointerUpdateKind is PointerUpdateKind.LeftButtonReleased)
            {
                if (p.Pointer.Type == PointerType.Mouse)
                {
                    // If the pressed point comes from a mouse, perform the selection immediately.
                    e.Handled = owner.UpdateSelectionFromPointerEvent(this, e);
                }
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconProperty)
        {
            UpdatePseudoClasses();
        }

        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);    
            }
        }
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(SegmentedPseudoClass.HasIcon, Icon is not null);
    }
}