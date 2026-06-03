using AtomUI;
using AtomUI.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Select;

public partial class SelectAppearanceShowCase : ReactiveUserControl<SelectViewModel>
{
    public SelectAppearanceShowCase()
    {
        InitializeComponent();
    }

    private void HandleSizeTypeChanged(object? sender, OptionCheckedChangedEventArgs e)
    {
        if (DataContext is SelectViewModel viewModel &&
            e.CheckedOption.Tag is SizeType sizeType)
        {
            viewModel.SelectSizeType = sizeType;
        }
    }
}
