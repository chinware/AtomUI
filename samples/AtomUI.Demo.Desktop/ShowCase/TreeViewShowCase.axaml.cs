using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class TreeViewShowCase : UserControl
{
    public static readonly StyledProperty<bool> ShowLineSwitchCheckedProperty =
        AvaloniaProperty.Register<TreeViewShowCase, bool>(nameof(ShowLineSwitchChecked), true);

    public static readonly StyledProperty<bool> ShowIconSwitchCheckedProperty =
        AvaloniaProperty.Register<TreeViewShowCase, bool>(nameof(ShowIconSwitchChecked));

    public static readonly StyledProperty<bool> ShowLeafSwitcherSwitchCheckedProperty =
        AvaloniaProperty.Register<TreeViewShowCase, bool>(nameof(ShowLeafSwitcherSwitchChecked));

    public bool ShowLineSwitchChecked
    {
        get => GetValue(ShowLineSwitchCheckedProperty);
        set => SetValue(ShowLineSwitchCheckedProperty, value);
    }

    public bool ShowIconSwitchChecked
    {
        get => GetValue(ShowIconSwitchCheckedProperty);
        set => SetValue(ShowIconSwitchCheckedProperty, value);
    }

    public bool ShowLeafSwitcherSwitchChecked
    {
        get => GetValue(ShowLeafSwitcherSwitchCheckedProperty);
        set => SetValue(ShowLeafSwitcherSwitchCheckedProperty, value);
    }

    public TreeViewShowCase()
    {
        InitializeComponent();
        DataContext = this;
    }
}