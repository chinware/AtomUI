using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class ComboBoxShowCase : ReactiveUserControl<ComboBoxViewModel>
{
    public ComboBoxShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is ComboBoxViewModel viewModel)
            {
                InitComboBoxItems(viewModel);
            }
        });
        InitializeComponent();
    }

    private void InitComboBoxItems(ComboBoxViewModel viewModel)
    {
        var items = new List<ComboBoxItemData>();
        items.Add(new ComboBoxItemData()
        {
            Text = "床前明月光"
        });
        items.Add(new ComboBoxItemData()
        {
            Text = "疑是地上霜"
        });
        items.Add(new ComboBoxItemData()
        {
            Text = "举头望明月"
        });
        items.Add(new ComboBoxItemData()
        {
            Text = "低头思故乡"
        });
        viewModel.ComboBoxItems = items;
    }
}

