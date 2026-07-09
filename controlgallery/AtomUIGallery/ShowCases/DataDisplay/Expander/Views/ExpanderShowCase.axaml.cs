using AtomUI.Controls;

namespace AtomUIGallery.ShowCases.Expander;

public partial class ExpanderShowCase : GalleryReactiveUserControl<ExpanderViewModel>
{
    public const string LanguageId = nameof(ExpanderShowCase);

    public ExpanderShowCase()
    {
        InitializeComponent();
    }

    private void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is ExpanderViewModel viewModel)
        {
            viewModel.HandleExpandButtonPosOptionCheckedChanged(sender, args);
        }
    }

    private void HandleExpandDirectionOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is ExpanderViewModel viewModel)
        {
            viewModel.HandleExpandDirectionOptionCheckedChanged(sender, args);
        }
    }
}
