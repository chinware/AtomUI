// Modified based on https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/ListItem.cs

using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class ListItem : ContentControl, ISelectable
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<ListItem>();
    
    public static readonly StyledProperty<Icon?> SelectedIndicatorProperty =
        AvaloniaProperty.Register<List, Icon?>(nameof(SelectedIndicator));
    
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public Icon? SelectedIndicator
    {
        get => GetValue(SelectedIndicatorProperty);
        set => SetValue(SelectedIndicatorProperty, value);
    }
    #endregion
    
    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<ListItem>();

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ListItem>();

    internal static readonly DirectProperty<ListItem, bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.RegisterDirect<ListItem, bool>(nameof(DisabledItemHoverEffect),
            o => o.DisabledItemHoverEffect,
            (o, v) => o.DisabledItemHoverEffect = v);
    
    internal static readonly DirectProperty<ListItem, bool> IsShowSelectedIndicatorProperty =
        AvaloniaProperty.RegisterDirect<ListItem, bool>(nameof(IsShowSelectedIndicator),
            o => o.IsShowSelectedIndicator,
            (o, v) => o.IsShowSelectedIndicator = v);
    
    internal static readonly DirectProperty<ListItem, bool> IsSelectedIndicatorVisibleProperty =
        AvaloniaProperty.RegisterDirect<ListItem, bool>(nameof(IsSelectedIndicatorVisible),
            o => o.IsSelectedIndicatorVisible,
            (o, v) => o.IsSelectedIndicatorVisible = v);

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
    
    private bool _isShowSelectedIndicator;

    internal bool IsShowSelectedIndicator
    {
        get => _isShowSelectedIndicator;
        set => SetAndRaise(IsShowSelectedIndicatorProperty, ref _isShowSelectedIndicator, value);
    }
    
    private bool _isSelectedIndicatorVisible;

    internal bool IsSelectedIndicatorVisible
    {
        get => _isSelectedIndicatorVisible;
        set => SetAndRaise(IsSelectedIndicatorVisibleProperty, ref _isSelectedIndicatorVisible, value);
    }
    #endregion

    private static readonly Point InvalidPoint = new Point(double.NaN, double.NaN);
    private Point _pointerDownPoint = InvalidPoint;
    
    static ListItem()
    {
        SelectableMixin.Attach<ListItem>(IsSelectedProperty);
        PressedMixin.Attach<ListItem>();
        FocusableProperty.OverrideDefaultValue<ListItem>(true);
        AutomationProperties.IsOffscreenBehaviorProperty.OverrideDefaultValue<ListItem>(IsOffscreenBehavior.FromClip);
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }

        if (change.Property == IsSelectedProperty ||
            change.Property == IsShowSelectedIndicatorProperty)
        {
            ConfigureSelectedIndicator();
        }
    }

    private void ConfigureSelectedIndicator()
    {
        SetCurrentValue(IsSelectedIndicatorVisibleProperty, IsShowSelectedIndicator && IsSelected);
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (!IsSet(SelectedIndicatorProperty))
        {
            SetCurrentValue(SelectedIndicatorProperty, AntDesignIconPackage.CheckOutlined());
        }
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _pointerDownPoint = InvalidPoint;
        if (e.Handled)
        {
            return;
        }

        if (!e.Handled && ItemsControl.ItemsControlFromItemContainer(this) is ListDefaultView owner)
        {
            var p = e.GetCurrentPoint(this);

            if (p.Properties.PointerUpdateKind is PointerUpdateKind.LeftButtonPressed or 
                PointerUpdateKind.RightButtonPressed)
            {
                if (p.Pointer.Type == PointerType.Mouse
                    || (p.Pointer.Type == PointerType.Pen && p.Properties.IsRightButtonPressed))
                {
                    // If the pressed point comes from a mouse or right-click pen, perform the selection immediately.
                    // In case of pen, only right-click is accepted, as left click (a tip touch) is used for scrolling. 
                    e.Handled = owner.UpdateSelectionFromPointerEvent(this, e);
                }
                else
                {
                    // Otherwise perform the selection when the pointer is released as to not
                    // interfere with gestures.
                    _pointerDownPoint = p.Position;

                    // Ideally we'd set handled here, but that would prevent the scroll gesture
                    // recognizer from working.
                    ////e.Handled = true;
                }
            }
        }
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!e.Handled && 
            !double.IsNaN(_pointerDownPoint.X) &&
            e.InitialPressMouseButton is MouseButton.Left or MouseButton.Right)
        {
            var point    = e.GetCurrentPoint(this);
            var settings = TopLevel.GetTopLevel(e.Source as Visual)?.PlatformSettings;
            var tapSize  = settings?.GetTapSize(point.Pointer.Type) ?? new Size(4, 4);
            var tapRect = new Rect(_pointerDownPoint, new Size())
                .Inflate(new Thickness(tapSize.Width, tapSize.Height));

            if (new Rect(Bounds.Size).ContainsExclusive(point.Position) &&
                tapRect.ContainsExclusive(point.Position) &&
                ItemsControl.ItemsControlFromItemContainer(this) is ListDefaultView owner)
            {
                if (owner.UpdateSelectionFromPointerEvent(this, e))
                {
                    // As we only update selection from touch/pen on pointer release, we need to raise
                    // the pointer event on the owner to trigger a commit.
                    if (e.Pointer.Type != PointerType.Mouse)
                    {
                        var sourceBackup = e.Source;
                        owner.RaiseEvent(e);
                        e.Source = sourceBackup;
                    }

                    e.Handled = true;
                }
            }
        }

        _pointerDownPoint = InvalidPoint;
    }
}