using System.Diagnostics;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

public class OptionCheckedChangedEventArgs : RoutedEventArgs
{
    public OptionCheckedChangedEventArgs(RoutedEvent routedEvent, OptionButton option, int index)
        : base(routedEvent)
    {
        CheckedOption = option;
        Index         = index;
    }

    public OptionButton CheckedOption { get; }
    public int Index { get; }
}

public class OptionButtonGroup : SelectingItemsControl,
                                 ISizeTypeAware,
                                 IWaveSpiritAwareControl,
                                 IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<OptionButtonGroup>();

    public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
        AvaloniaProperty.Register<OptionButtonGroup, OptionButtonStyle>(nameof(SizeType));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<OptionButtonGroup>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<OptionButtonGroup>();

    public static readonly RoutedEvent<OptionCheckedChangedEventArgs> OptionCheckedChangedEvent =
        RoutedEvent.Register<OptionButtonGroup, OptionCheckedChangedEventArgs>(
            nameof(OptionCheckedChanged),
            RoutingStrategies.Bubble);

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

    public event EventHandler<OptionCheckedChangedEventArgs>? OptionCheckedChanged
    {
        add => AddHandler(OptionCheckedChangedEvent, value);
        remove => RemoveHandler(OptionCheckedChangedEvent, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<OptionButtonGroup, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<OptionButtonGroup, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);

    internal static readonly StyledProperty<IBrush?> SelectedOptionBorderColorProperty =
        AvaloniaProperty.Register<OptionButtonGroup, IBrush?>(nameof(SelectedOptionBorderColor));

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

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => OptionButtonToken.ID;

    #endregion

    private readonly BorderRenderHelper _borderRenderHelper = new();

    static OptionButtonGroup()
    {
        SelectionModeProperty.OverrideDefaultValue<OptionButtonGroup>(SelectionMode.Single |
                                                                      SelectionMode.AlwaysSelected);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<OptionButtonGroup>(false);
        ItemsPanelProperty.OverrideDefaultValue<OptionButtonGroup>(DefaultPanel);
        AffectsRender<OptionButtonGroup>(SelectionModeProperty);

        AffectsMeasure<OptionButtonGroup>(SizeTypeProperty);
        AffectsRender<OptionButtonGroup>(SelectedOptionBorderColorProperty,
            ButtonStyleProperty, SelectedItemProperty);
    }

    public OptionButtonGroup()
    {
        this.RegisterResources();
        if (this is IChildIndexProvider childIndexProvider)
        {
            childIndexProvider.ChildIndexChanged += (sender, args) => { UpdateOptionButtonsPosition(); };
        }
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new OptionButton();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);

        if (e.NavigationMethod == NavigationMethod.Directional && e.Source is OptionButton)
        {
            e.Handled = UpdateSelectionFromEventSource(e.Source);
        }
    }

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
        if (item is OptionButton optionButton)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, optionButton, OptionButton.SizeTypeProperty);
            BindUtils.RelayBind(this, ButtonStyleProperty, optionButton, OptionButton.ButtonStyleProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, optionButton, OptionButton.IsMotionEnabledProperty);
            BindUtils.RelayBind(this, IsWaveSpiritEnabledProperty, optionButton,
                OptionButton.IsWaveSpiritEnabledProperty);

            optionButton.IsCheckedChanged += HandleOptionButtonChecked;
        }
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is OptionButton optionButton)
        {
            if (optionButton.IsChecked.HasValue && optionButton.IsChecked.Value)
            {
                UpdateSelectionFromEventSource(optionButton);
                RaiseEvent(new OptionCheckedChangedEventArgs(OptionCheckedChangedEvent, optionButton,
                    index));
            }
        }
    }

    private void HandleOptionButtonChecked(object? sender, RoutedEventArgs args)
    {
        if (sender is OptionButton optionButton && optionButton.IsChecked.HasValue && optionButton.IsChecked.Value)
        {
            UpdateSelectionFromEventSource(args.Source);
            RaiseEvent(new OptionCheckedChangedEventArgs(OptionCheckedChangedEvent, optionButton,
                SelectedIndex));
        }
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<OptionButton>(item, out recycleKey);
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        if (container is OptionButton optionButton)
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
            var button = Items[i] as OptionButton;
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
        var borderThickness =
            BorderUtils.BuildRenderScaleAwareThickness(BorderThickness, VisualRoot?.RenderScaling ?? 1.0);
        _borderRenderHelper.Render(context,
            new Size(DesiredSize.Width, DesiredSize.Height),
            new Thickness(1),
            CornerRadius,
            BackgroundSizing.CenterBorder,
            null,
            BorderBrush,
            new BoxShadows());
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
                var offsetX    = optionButton.Bounds.Right - borderThickness.Left / 2;
                var startPoint = new Point(offsetX, 0);
                var endPoint   = new Point(offsetX, Bounds.Height);
                using var optionState = context.PushRenderOptions(new RenderOptions
                {
                    EdgeMode = EdgeMode.Aliased
                });
                context.DrawLine(new Pen(BorderBrush, borderThickness.Left), startPoint, endPoint);
            }

            if (ButtonStyle == OptionButtonStyle.Outline)
            {
                if (optionButton.IsEnabled && optionButton == SelectedItem)
                {
                    // 绘制选中边框
                    var offsetX = optionButton.Bounds.X;
                    var width   = optionButton.DesiredSize.Width;
                    if (i > 0)
                    {
                        offsetX -= borderThickness.Left;
                        width   += borderThickness.Left;
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
                        borderThickness,
                        cornerRadius,
                        BackgroundSizing.InnerBorderEdge,
                        null,
                        SelectedOptionBorderColor,
                        new BoxShadows());
                }
            }
        }
    }
}