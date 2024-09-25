using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class MenuShowCase : UserControl
{
    public static readonly StyledProperty<bool> IsDarkProperty =
        AvaloniaProperty.Register<ButtonShowCase, bool>(nameof(IsDark), false);
    
    public static readonly StyledProperty<NavMenuMode> ModeProperty =
        AvaloniaProperty.Register<ButtonShowCase, NavMenuMode>(nameof(Mode), NavMenuMode.Inline);
    
    public bool IsDark
    {
        get => GetValue(IsDarkProperty);
        set => SetValue(IsDarkProperty, value);
    }
    
    public NavMenuMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    public MenuShowCase()
    {
        InitializeComponent();
        DataContext                        =  this;
        ChangeModeSwitch.IsCheckedChanged  += HandleChangeModeCheckChanged;
        ChangeStyleSwitch.IsCheckedChanged += HandleChangeStyleCheckChanged;
    }

    private void HandleChangeModeCheckChanged(object? sender, RoutedEventArgs? args)
    {
        if (ChangeModeSwitch.IsChecked.HasValue)
        {
            if (ChangeModeSwitch.IsChecked.Value)
            {
                Mode = NavMenuMode.Vertical;
            }
            else
            {
                Mode = NavMenuMode.Inline;
            }
        }
        else
        {
            Mode = NavMenuMode.Inline;
        }
    }
    
    private void HandleChangeStyleCheckChanged(object? sender, RoutedEventArgs? args)
    {
        if (ChangeStyleSwitch.IsChecked.HasValue)
        {
            IsDark = ChangeStyleSwitch.IsChecked.Value;
        }
        else
        {
            IsDark = false;
        }
    }
}