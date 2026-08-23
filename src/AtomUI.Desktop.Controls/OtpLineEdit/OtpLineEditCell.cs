using AtomUI.Animations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

using AvaloniaTextBox = Avalonia.Controls.TextBox;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(":cell-active", ":input-target")]
internal class OtpLineEditCell : InputControlFrame
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> DisplayTextProperty =
        AvaloniaProperty.Register<OtpLineEditCell, string?>(nameof(DisplayText));

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<OtpLineEditCell, string?>(nameof(PlaceholderText));

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<OtpLineEditCell, bool>(nameof(IsActive));

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

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsInputTargetProperty =
        AvaloniaProperty.Register<OtpLineEditCell, bool>(nameof(IsInputTarget));

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

        _textBox = Content as OtpTextBox;
        ConfigureTextBoxCursor();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsActiveProperty)
        {
            PseudoClasses.Set(CellActivePseudoClass, change.GetNewValue<bool>());
            IsInputFocusWithin = change.GetNewValue<bool>();
        }

        if (change.Property == IsInputTargetProperty)
        {
            PseudoClasses.Set(InputTargetPseudoClass, change.GetNewValue<bool>());
            ConfigureTextBoxCursor();
        }

        if (change.Property == ContentProperty ||
            change.Property == IsEffectivelyEnabledProperty)
        {
            _textBox = Content as OtpTextBox;
            ConfigureTextBoxCursor();
        }
    }

    private void ConfigureTextBoxCursor()
    {
        if (_textBox is not null)
        {
            _textBox.SetCurrentValue(
                CursorProperty,
                IsInputTarget && IsEffectivelyEnabled
                    ? new Cursor(StandardCursorType.Ibeam)
                    : new Cursor(StandardCursorType.Arrow));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        _textBox = null;
    }
}
