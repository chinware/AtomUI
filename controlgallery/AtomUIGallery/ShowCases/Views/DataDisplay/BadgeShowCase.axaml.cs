using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class BadgeShowCase : ReactiveUserControl<BadgeViewModel>
{
    public BadgeShowCase()
    {
        InitializeComponent();
    }
}