using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class ComboBoxHandle : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ComboBoxHandle>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler<RoutedEventArgs>? HandleClick;

    #endregion

    private IconButton? _iconButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_iconButton is not null)
        {
            _iconButton.Click -= HandleIconButtonClicked;
        }

        _iconButton = e.NameScope.Find<IconButton>("PART_OpenIndicatorButton");
        if (_iconButton != null)
        {
            _iconButton.Click -= HandleIconButtonClicked;
            _iconButton.Click += HandleIconButtonClicked;
        }
    }

    private void HandleIconButtonClicked(object? sender, RoutedEventArgs args)
    {
        HandleClick?.Invoke(this, args);
    }
}
