using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Views;

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
