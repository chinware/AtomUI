using AtomUI.Controls;
using AtomUI.Controls.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using IListItemData = AtomUI.Controls.Data.IListItemData;

namespace AtomUIGallery.ShowCases.List;

public partial class ListBasicShowCase : GalleryReactiveUserControl<ListViewModel>
{
    public ListBasicShowCase()
    {
        InitializeComponent();
    }

    private void HandleSelectionModeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs e)
    {
        if (DataContext is ListViewModel viewModel &&
            e.CheckedOption.IsChecked == true &&
            e.CheckedOption.Tag is SelectionMode selectionMode)
        {
            viewModel.SelectionMode = selectionMode;
        }
    }

    private void HandleAddEmptyItemClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ListViewModel viewModel)
        {
            return;
        }

        var items = viewModel.EmptyDemoItems != null
            ? new List<IListItemData>(viewModel.EmptyDemoItems)
            : new List<IListItemData>();

        items.Add(ListShowCase.CreateDynamicItem());
        viewModel.EmptyDemoItems = items;
    }

    private void HandleRemoveEmptyItemClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ListViewModel viewModel)
        {
            return;
        }

        if (viewModel.EmptyDemoItems is null || viewModel.EmptyDemoItems.Count <= 1)
        {
            viewModel.EmptyDemoItems = [];
            return;
        }

        var items = new List<IListItemData>(viewModel.EmptyDemoItems);
        items.RemoveAt(items.Count - 1);
        viewModel.EmptyDemoItems = items;
    }
}
