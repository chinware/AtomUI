using AtomUI.Controls;

namespace AtomUIGallery.ShowCases.Collapse;

public partial class CollapseShowCase : GalleryReactiveUserControl<CollapseViewModel>
{
    public const string LanguageId = nameof(CollapseShowCase);

    public CollapseShowCase()
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
