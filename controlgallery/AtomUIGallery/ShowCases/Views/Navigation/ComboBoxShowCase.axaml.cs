using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
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
                this.OneWayBind(viewModel, vm => vm.ComboBoxItems, v => v.TplComboBox.ItemsSource)
                    .DisposeWith(disposables);
                Disposable.Create(() =>
                {
                    viewModel.ComboBoxItems = null;
                }).DisposeWith(disposables);
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
