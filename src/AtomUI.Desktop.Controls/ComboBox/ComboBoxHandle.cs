using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class ComboBoxHandle : TemplatedControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ComboBoxHandle>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? HandleClick;
    private IconButton? _iconButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_iconButton != null)
        {
            _iconButton.Click -= HandleIconButtonClick;
        }

        _iconButton = e.NameScope.Find<IconButton>("PART_OpenIndicatorButton");
        if (_iconButton != null)
        {
            _iconButton.Click += HandleIconButtonClick;
        }
    }

    private void HandleIconButtonClick(object? sender, RoutedEventArgs args)
    {
        HandleClick?.Invoke(this, args);
    }
}
