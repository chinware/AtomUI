using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class LoadingIndicatorShowCase : UserControl
{
    public static readonly StyledProperty<bool> IsLoadingSwitchCheckedProperty =
        AvaloniaProperty.Register<ProgressBarShowCase, bool>(nameof(IsLoadingSwitchChecked));

    public bool IsLoadingSwitchChecked
    {
        get => GetValue(IsLoadingSwitchCheckedProperty);
        set => SetValue(IsLoadingSwitchCheckedProperty, value);
    }

    public LoadingIndicatorShowCase()
    {
        DataContext = this;
        InitializeComponent();
    }
}