using System.Collections.Specialized;
using AtomUI.Controls.Utils;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using ButtonSizeType = SizeType;
using OptionButtons = AvaloniaList<OptionButton>;

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

public class OptionButtonGroup : TemplatedControl, ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
        AvaloniaProperty.Register<OptionButtonGroup, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

    public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
        AvaloniaProperty.Register<OptionButtonGroup, OptionButtonStyle>(nameof(SizeType));

    public static readonly DirectProperty<OptionButtonGroup, OptionButton?> SelectedOptionProperty =
        AvaloniaProperty.RegisterDirect<OptionButtonGroup, OptionButton?>(nameof(SelectedOption),
            o => o.SelectedOption,
            (o, v) => o.SelectedOption = v);

    internal static readonly StyledProperty<IBrush?> SelectedOptionBorderColorProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(SelectedOptionBorderColor));

    public static readonly RoutedEvent<OptionCheckedChangedEventArgs> OptionCheckedChangedEvent =
        RoutedEvent.Register<SelectingItemsControl, OptionCheckedChangedEventArgs>(
            nameof(OptionCheckedChanged),
            RoutingStrategies.Bubble);

    public event EventHandler<OptionCheckedChangedEventArgs>? OptionCheckedChanged
    {
        add => AddHandler(OptionCheckedChangedEvent, value);
        remove => RemoveHandler(OptionCheckedChangedEvent, value);
    }

    public ButtonSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public OptionButtonStyle ButtonStyle
    {
        get => GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }

    private OptionButton? _optionButton;

    public OptionButton? SelectedOption
    {
        get => _optionButton;
        set => SetAndRaise(SelectedOptionProperty, ref _optionButton, value);
    }

    internal IBrush? SelectedOptionBorderColor
    {
        get => GetValue(SelectedOptionBorderColorProperty);
        set => SetValue(SelectedOptionBorderColorProperty, value);
    }

    #endregion

    [Content] public OptionButtons Options { get; } = new();

    private ControlStyleState _styleState;
    private StackPanel? _layout;
    private readonly BorderRenderHelper _borderRenderHelper = new();

    static OptionButtonGroup()
    {
        AffectsMeasure<OptionButtonGroup>(SizeTypeProperty);
        AffectsRender<OptionButtonGroup>(SelectedOptionProperty, SelectedOptionBorderColorProperty,
            ButtonStyleProperty);
    }

    public OptionButtonGroup()
    {
        Options.CollectionChanged += OptionsChanged;
    }

    protected virtual void OptionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newOptions = e.NewItems!.OfType<OptionButton>().ToList();
                ApplyInButtonGroupFlag(newOptions, true);
                _layout?.Children.AddRange(newOptions);
                break;

            case NotifyCollectionChangedAction.Move:
                _layout?.Children.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                break;

            case NotifyCollectionChangedAction.Remove:
                var removedOptions = e.OldItems!.OfType<OptionButton>().ToList();
                ApplyInButtonGroupFlag(removedOptions, false);
                _layout?.Children.RemoveAll(removedOptions);
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var i = 0; i < e.OldItems!.Count; ++i)
                {
                    var index    = i + e.OldStartingIndex;
                    var oldChild = (OptionButton)e.OldItems![i]!;
                    oldChild.InOptionGroup = false;
                    var child = (OptionButton)e.NewItems![i]!;
                    child.InOptionGroup = true;
                    if (_layout is not null)
                    {
                        _layout.Children[index] = child;
                    }
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }

        UpdateOptionButtonsPosition();
        InvalidateMeasureOnOptionsChanged();
    }

    private void UpdateOptionButtonsPosition()
    {
        for (var i = 0; i < Options.Count; i++)
        {
            var button = Options[i];
            if (Options.Count > 1)
            {
                if (i == 0)
                {
                    button.GroupPositionTrait = OptionButtonPositionTrait.First;
                }
                else if (i == Options.Count - 1)
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

    private void ApplyInButtonGroupFlag(List<OptionButton> buttons, bool inGroup)
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            button.InOptionGroup = inGroup;
            if (inGroup)
            {
                button.IsCheckedChanged += HandleOptionSelected;
            }
            else
            {
                button.IsCheckedChanged   -= HandleOptionSelected;
                button.GroupPositionTrait =  OptionButtonPositionTrait.OnlyOne;
            }
        }
    }

    private void HandleOptionSelected(object? sender, RoutedEventArgs args)
    {
        if (sender is OptionButton optionButton)
        {
            if (optionButton.IsChecked.HasValue && optionButton.IsChecked.Value)
            {
                SelectedOption = optionButton;
                RaiseEvent(new OptionCheckedChangedEventArgs(OptionCheckedChangedEvent, optionButton,
                    Options.IndexOf(optionButton)));
            }
        }
    }

    private protected virtual void InvalidateMeasureOnOptionsChanged()
    {
        InvalidateMeasure();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        HandlePropertyChangedForStyle(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _layout             = scope.Find<StackPanel>(OptionButtonGroupTheme.MainContainerPart);
        HorizontalAlignment = HorizontalAlignment.Left;
        ApplyButtonSizeConfig();
        ApplyButtonStyleConfig();
        _layout?.Children.AddRange(Options);
        CollectStyleState();
    }

    private void CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);
    }

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == SizeTypeProperty)
        {
            ApplyButtonSizeConfig();
        }
        else if (e.Property == ButtonStyleProperty)
        {
            ApplyButtonStyleConfig();
        }
    }

    private void ApplyButtonSizeConfig()
    {
        foreach (var optionButton in Options)
        {
            optionButton.SizeType = SizeType;
        }
    }

    private void ApplyButtonStyleConfig()
    {
        foreach (var optionButton in Options)
        {
            optionButton.ButtonStyle = ButtonStyle;
        }
    }

    public override void Render(DrawingContext context)
    {
        var borderThickness =
            BorderUtils.BuildRenderScaleAwareThickness(BorderThickness, VisualRoot?.RenderScaling ?? 1.0);
        _borderRenderHelper.Render(context,
            new Size(DesiredSize.Width, DesiredSize.Height),
            borderThickness,
            CornerRadius,
            BackgroundSizing.CenterBorder,
            null,
            BorderBrush,
            new BoxShadows());
        for (var i = 0; i < Options.Count; ++i)
        {
            var optionButton = Options[i];
            if (ButtonStyle == OptionButtonStyle.Solid)
            {
                if (i <= Options.Count - 2)
                {
                    var nextOption = Options[i + 1];
                    if (nextOption == SelectedOption || optionButton == SelectedOption)
                    {
                        continue;
                    }
                }
            }

            if (i != Options.Count - 1)
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
                if (optionButton.IsEnabled && optionButton.IsChecked.HasValue && optionButton.IsChecked.Value)
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
                    else if (i == Options.Count - 1)
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