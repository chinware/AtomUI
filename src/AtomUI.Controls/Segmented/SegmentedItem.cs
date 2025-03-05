using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class SegmentedItem : ContentControl,
                             ISelectable,
                             ITokenResourceConsumer
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

    internal static readonly DirectProperty<SegmentedItem, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<SegmentedItem, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);
    
    internal SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }
    
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    
    #endregion

    private CompositeDisposable? _tokenBindingsDisposable;
    
    static SegmentedItem()
    {
        SelectableMixin.Attach<SegmentedItem>(IsSelectedProperty);
        PressedMixin.Attach<SegmentedItem>();
        FocusableProperty.OverrideDefaultValue<SegmentedItem>(true);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
        Debug.Assert(Parent is Segmented, "SegmentedItem's Parent must be Segmented Control.");
        SetupTransitions();
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToLogicalTree())
        {
            if (change.Property == IconProperty)
            {
                SetupItemIcon();
            } 
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                SetupTransitions();    
            }
        }
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupItemIcon();
    }

    private void SetupItemIcon()
    {
        if (Icon is not null)
        {
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                SegmentedTokenKey.ItemColor));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(Icon, Icon.ActiveFilledBrushProperty,
                SegmentedTokenKey.ItemHoverColor));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(Icon, Icon.SelectedFilledBrushProperty,
                SegmentedTokenKey.ItemSelectedColor));
            Icon.SetTemplatedParent(this);
        }
    }

}