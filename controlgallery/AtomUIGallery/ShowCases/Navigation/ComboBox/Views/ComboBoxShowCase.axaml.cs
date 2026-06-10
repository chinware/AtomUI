using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace AtomUIGallery.ShowCases.ComboBox;

public partial class ComboBoxShowCase : GalleryReactiveUserControl<ComboBoxViewModel>
{
    public const string LanguageId = nameof(ComboBoxShowCase);

    public ComboBoxShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is ComboBoxViewModel viewModel)
            {
                InitComboBoxItems(viewModel);
                GalleryBindingUtils.OneWay(viewModel, nameof(ComboBoxViewModel.ComboBoxItems),
                                           vm => vm.ComboBoxItems, TplComboBox,
                                           Avalonia.Controls.ItemsControl.ItemsSourceProperty)
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
