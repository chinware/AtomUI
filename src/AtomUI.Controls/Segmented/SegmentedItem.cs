using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class SegmentedItem : ContentControl,
                             ISelectable,
                             IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<SegmentedItem>();

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<SegmentedItem, Icon?>(nameof(Icon));

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
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<SegmentedItem>();
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SegmentedItem>();
    
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
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    
    #endregion

    private CompositeDisposable? _resourceBindingsDisposable;
    
    static SegmentedItem()
    {
        SelectableMixin.Attach<SegmentedItem>(IsSelectedProperty);
        PressedMixin.Attach<SegmentedItem>();
        FocusableProperty.OverrideDefaultValue<SegmentedItem>(true);
        AffectsRender<SegmentedItem>(BackgroundProperty);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        Debug.Assert(Parent is Segmented, "SegmentedItem's Parent must be Segmented Control.");
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureTransitions();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();    
            }
        }

        if (change.Property == IconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (change.NewValue is Icon newIcon)
            {
                newIcon.SetTemplatedParent(this);
            }
        }
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }
}