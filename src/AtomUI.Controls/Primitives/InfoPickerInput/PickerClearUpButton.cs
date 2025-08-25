using AtomUI.Controls.Primitives.Themes;
using AtomUI.IconPkg;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class PickerClearUpButton : TemplatedControl
{
    public event EventHandler? ClearRequest;

    public static readonly StyledProperty<bool> IsInClearModeProperty =
        AvaloniaProperty.Register<PickerClearUpButton, bool>(nameof(IsInClearMode));
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<PickerClearUpButton, Icon?>(nameof(Icon));

    public bool IsInClearMode
    {
        get => GetValue(IsInClearModeProperty);
        set => SetValue(IsInClearModeProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private IconButton? _clearButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _clearButton = e.NameScope.Get<IconButton>(PickerClearUpButtonThemeConstants.ClearButtonPart);
        if (_clearButton is not null)
        {
            _clearButton.Click += (sender, args) => { ClearRequest?.Invoke(this, EventArgs.Empty); };
        }
    }
}