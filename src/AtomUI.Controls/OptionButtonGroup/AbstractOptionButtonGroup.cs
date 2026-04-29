using System.Diagnostics;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls.Commons;

public abstract class AbstractOptionButtonGroup : SelectingItemsControl,
                                                 ISizeTypeAware,
                                                 IWaveSpiritAwareControl,
                                                 IFormItemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractOptionButtonGroup>();

    public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
        AvaloniaProperty.Register<AbstractOptionButtonGroup, OptionButtonStyle>(nameof(ButtonStyle));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractOptionButtonGroup>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<AbstractOptionButtonGroup>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public OptionButtonStyle ButtonStyle
    {
        get => GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }
    
    #endregion

    #region 公共事件定义
    public static readonly RoutedEvent<OptionCheckedChangedEventArgs> OptionCheckedChangedEvent =
        RoutedEvent.Register<AbstractOptionButtonGroup, OptionCheckedChangedEventArgs>(
            nameof(OptionCheckedChanged),
            RoutingStrategies.Bubble);
    
    public event EventHandler<OptionCheckedChangedEventArgs>? OptionCheckedChanged
    {
        add => AddHandler(OptionCheckedChangedEvent, value);
        remove => RemoveHandler(OptionCheckedChangedEvent, value);
    }
    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<AbstractOptionButtonGroup, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<AbstractOptionButtonGroup, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);

    internal static readonly StyledProperty<IBrush?> SelectedOptionBorderColorProperty =
        AvaloniaProperty.Register<AbstractOptionButtonGroup, IBrush?>(nameof(SelectedOptionBorderColor));

    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }

    internal IBrush? SelectedOptionBorderColor
    {
        get => GetValue(SelectedOptionBorderColorProperty);
        set => SetValue(SelectedOptionBorderColorProperty, value);
    }

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel
        {
            Orientation = Orientation.Horizontal
        });

    #endregion

    private readonly BorderRenderHelper _borderRenderHelper = new();

    static AbstractOptionButtonGroup()
    {
        SelectionModeProperty.OverrideDefaultValue<AbstractOptionButtonGroup>(SelectionMode.Single | SelectionMode.AlwaysSelected);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<AbstractOptionButtonGroup>(false);
        ItemsPanelProperty.OverrideDefaultValue<AbstractOptionButtonGroup>(DefaultPanel);
        AffectsRender<AbstractOptionButtonGroup>(SelectionModeProperty);

        AffectsMeasure<AbstractOptionButtonGroup>(SizeTypeProperty);
        AffectsRender<AbstractOptionButtonGroup>(SelectedOptionBorderColorProperty,
            ButtonStyleProperty, SelectedItemProperty);
        SelectedItemProperty.Changed.AddClassHandler<AbstractOptionButtonGroup>((group, args) => group.NotifyFormValueChanged(args.NewValue));
    }

    public AbstractOptionButtonGroup()
    {
        if (this is IChildIndexProvider childIndexProvider)
        {
            childIndexProvider.ChildIndexChanged += HandleChildIndexChanged;
        }
    }
    
    private void HandleChildIndexChanged(object? sender, EventArgs args)
    {
        UpdateOptionButtonsPosition();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (this is IChildIndexProvider childIndexProvider)
        {
            childIndexProvider.ChildIndexChanged -= HandleChildIndexChanged;
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);

        if (e.NavigationMethod == NavigationMethod.Directional && e.Source is AbstractOptionButton optionButton)
        {
            e.Handled = UpdateSelectionFromEvent(optionButton, e);
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AbstractOptionButton optionButton)
        {
            if (item != null && item is not Visual)
            {
                {
                    if (!optionButton.IsSet(AbstractOptionButton.ContentProperty))
                    {
                        if (ItemTemplate != null)
                        {
                            optionButton.SetCurrentValue(AbstractOptionButton.ContentProperty, item);
                        }
                        else
                        {
                            if (item is IOptionButtonData optionButtonData)
                            {
                                optionButton.SetCurrentValue(AbstractOptionButton.ContentProperty, optionButtonData.Header);
                            }
                        }
                    }
                }

                {
                    if (item is IOptionButtonData optionButtonData)
                    {
                        if (!optionButton.IsSet(IsEnabledProperty))
                        {
                            optionButton.SetCurrentValue(IsEnabledProperty, optionButtonData.IsEnabled);
                        }
                        
                        if (!optionButton.IsSet(AbstractOptionButton.IconProperty))
                        {
                            optionButton.SetCurrentValue(AbstractOptionButton.IconProperty, optionButtonData.Icon);
                        }
                    }
                }
            }
            
            if (ItemTemplate != null)
            {
                optionButton[!AbstractOptionButton.ContentTemplateProperty] = this[!ItemTemplateProperty];
            }
            
            optionButton[!IsMotionEnabledProperty]                  = this[!IsMotionEnabledProperty];
            optionButton[!SizeTypeProperty]                         = this[!SizeTypeProperty];
            optionButton[!IsWaveSpiritEnabledProperty]              = this[!IsWaveSpiritEnabledProperty];
            optionButton[!AbstractOptionButton.ButtonStyleProperty] = this[!ButtonStyleProperty];
            
            PrepareOptionButton(optionButton, item, index);
            optionButton.IsCheckedChanged += HandleOptionButtonChecked;
        }  
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type AbstractOptionButton.");
        }
    }
    
    protected virtual void PrepareOptionButton(AbstractOptionButton optionButton, object? item, int index)
    {
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is AbstractOptionButton optionButton)
        {
            if (optionButton.IsChecked.HasValue && optionButton.IsChecked.Value)
            {
                var containerIndex = IndexFromContainer(optionButton);
                if (containerIndex != -1)
                {
                    Selection.Select(containerIndex);
                }
                RaiseEvent(new OptionCheckedChangedEventArgs(OptionCheckedChangedEvent, optionButton,
                    index));
            }
        }
    }

    private void HandleOptionButtonChecked(object? sender, RoutedEventArgs args)
    {
        if (sender is AbstractOptionButton optionButton && optionButton.IsChecked.HasValue && optionButton.IsChecked.Value)
        {
            var index = IndexFromContainer(optionButton);
            if (index != -1)
            {
                Selection.Select(index);
            }
            RaiseEvent(new OptionCheckedChangedEventArgs(OptionCheckedChangedEvent, optionButton,
                SelectedIndex));
        }
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        if (container is AbstractOptionButton optionButton)
        {
            optionButton.IsCheckedChanged -= HandleOptionButtonChecked;
        }
        base.ClearContainerForItemOverride(container);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AbstractOptionButton>(item, out recycleKey);
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        if (container is AbstractOptionButton optionButton)
        {
            if (newIndex == 0)
            {
                optionButton.GroupPositionTrait = OptionButtonPositionTrait.First;
            }
            else if (newIndex == ItemCount - 1)
            {
                optionButton.GroupPositionTrait = OptionButtonPositionTrait.Last;
            }
            else
            {
                optionButton.GroupPositionTrait = OptionButtonPositionTrait.Middle;
            }
        }
    }

    private void UpdateOptionButtonsPosition()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var button = Items[i] as AbstractOptionButton;
            Debug.Assert(button != null);
            if (Items.Count > 1)
            {
                if (i == 0)
                {
                    button.GroupPositionTrait = OptionButtonPositionTrait.First;
                }
                else if (i == Items.Count - 1)
                {
                    button.GroupPositionTrait = OptionButtonPositionTrait.Last;
                }
                else
                {
                    button.GroupPositionTrait = OptionButtonPositionTrait.Middle;
                }
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        _borderRenderHelper.Render(context,
            new Size(DesiredSize.Width, DesiredSize.Height),
            new Thickness(1),
            CornerRadius,
            BackgroundSizing.CenterBorder,
            null,
            BorderBrush);
        for (var i = 0; i < ItemCount; ++i)
        {
            var optionButton = ContainerFromIndex(i);
            Debug.Assert(optionButton != null);
            if (ButtonStyle == OptionButtonStyle.Solid)
            {
                if (i <= ItemCount - 2)
                {
                    var nextOption = ContainerFromIndex(i + 1);
                    if (nextOption == SelectedItem || optionButton == SelectedItem)
                    {
                        continue;
                    }
                }
            }

            if (i != ItemCount - 1)
            {
                var offsetX    = optionButton.Bounds.Right - BorderThickness.Left / 2;
                var startPoint = new Point(offsetX, 0);
                var endPoint   = new Point(offsetX, Bounds.Height);
                using var optionState = context.PushRenderOptions(new RenderOptions
                {
                    EdgeMode = EdgeMode.Aliased
                });
                context.DrawLine(new Pen(BorderBrush, BorderThickness.Left), startPoint, endPoint);
            }

            if (ButtonStyle == OptionButtonStyle.Outline)
            {
                if (IsEnabled && optionButton.IsEnabled && optionButton == SelectedItem)
                {
                    // 绘制选中边框
                    var offsetX = optionButton.Bounds.X;
                    var width   = optionButton.DesiredSize.Width;
                    if (i > 0)
                    {
                        offsetX -= BorderThickness.Left;
                        width   += BorderThickness.Left;
                    }

                    var       translationMatrix = Matrix.CreateTranslation(offsetX, 0);
                    using var state             = context.PushTransform(translationMatrix);
                    var       cornerRadius      = new CornerRadius(0);
                    if (i == 0)
                    {
                        cornerRadius = new CornerRadius(CornerRadius.TopLeft, 0, 0, CornerRadius.BottomLeft);
                    }
                    else if (i == ItemCount - 1)
                    {
                        cornerRadius = new CornerRadius(0, CornerRadius.TopRight, CornerRadius.BottomRight, 0);
                    }

                    _borderRenderHelper.Render(context,
                        new Size(width, DesiredSize.Height),
                        BorderThickness,
                        cornerRadius,
                        BackgroundSizing.InnerBorderEdge,
                        null,
                        SelectedOptionBorderColor);
                }
            }
        }
    }
    
    #region 实现 FormItem 接口
    
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    protected virtual void NotifyFormValueChanged(object? value)
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(object? value)
    {
        SetCurrentValue(SelectedItemProperty, value);
    }

    protected virtual object? NotifyGetFormValue()
    {
        return SelectedItem;
    }

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(SelectedItemProperty, false);
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion
}