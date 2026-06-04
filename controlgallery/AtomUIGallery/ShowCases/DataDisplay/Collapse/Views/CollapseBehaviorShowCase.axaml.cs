using AtomUI.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Collapse;

public partial class CollapseBehaviorShowCase : ReactiveUserControl<CollapseViewModel>
{
    public CollapseBehaviorShowCase()
    {
        InitializeComponent();
    }

    private void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is CollapseViewModel viewModel)
        {
            viewModel.HandleExpandButtonPosOptionCheckedChanged(sender, args);
        }
    }
}
