using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Badge;

public partial class BadgeShowCase : UserControl, IViewFor<BadgeViewModel>
{
    public BadgeShowCase()
    {
        InitializeComponent();
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as BadgeViewModel;
    }

    public BadgeViewModel? ViewModel { get; set; }
}
