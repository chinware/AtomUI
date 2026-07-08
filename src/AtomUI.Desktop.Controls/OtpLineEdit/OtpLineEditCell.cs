using AtomUI.Animations;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using AvaloniaTextBox = Avalonia.Controls.TextBox;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Pressed, ":cell-active", ":input-target")]
public class OtpLineEditCell : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> DisplayTextProperty =
        AvaloniaProperty.Register<OtpLineEditCell, string?>(nameof(DisplayText));

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<OtpLineEditCell, string?>(nameof(PlaceholderText));

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<OtpLineEditCell, bool>(nameof(IsActive));

    public static readonly StyledProperty<InputControlStatus> EffectiveStatusProperty =
        AvaloniaProperty.Register<OtpLineEditCell, InputControlStatus>(nameof(EffectiveStatus));

    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<OtpLineEditCell>();

    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<OtpLineEditCell>();

    public string? DisplayText
    {
        get => GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public InputControlStatus EffectiveStatus
    {
        get => GetValue(EffectiveStatusProperty);
        set => SetValue(EffectiveStatusProperty, value);
    }

    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public InputControlStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<OtpLineEditCell>();

    internal static readonly StyledProperty<bool> IsInputTargetProperty =
        AvaloniaProperty.Register<OtpLineEditCell, bool>(nameof(IsInputTarget));

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal bool IsInputTarget
    {
        get => GetValue(IsInputTargetProperty);
        set => SetValue(IsInputTargetProperty, value);
    }

    #endregion

    private OtpTextBox? _textBox;

    private const string CellActivePseudoClass = ":cell-active";
    private const string InputTargetPseudoClass = ":input-target";

    internal AvaloniaTextBox? TextBoxPart => _textBox;

    static OtpLineEditCell()
    {
        PressedMixin.Attach<OtpLineEditCell>();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textBox = e.NameScope.Find<OtpTextBox>("PART_TextBox");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsActiveProperty)
        {
            PseudoClasses.Set(CellActivePseudoClass, change.GetNewValue<bool>());
        }

        if (change.Property == IsInputTargetProperty)
        {
            PseudoClasses.Set(InputTargetPseudoClass, change.GetNewValue<bool>());
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        _textBox = null;
    }
}
