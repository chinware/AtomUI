using System.Reactive;
using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ComboBox;

public class ComboBoxViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ComboBox";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private List<ComboBoxItemData>? _comboBoxItems;
    private ComboBoxItemData? _boundSelectedItem;

    public List<ComboBoxItemData>? ComboBoxItems
    {
        get => _comboBoxItems;
        set => this.RaiseAndSetIfChanged(ref _comboBoxItems, value);
    }

    public ComboBoxItemData? BoundSelectedItem
    {
        get => _boundSelectedItem;
        set
        {
            this.RaiseAndSetIfChanged(ref _boundSelectedItem, value);
            this.RaisePropertyChanged(nameof(BoundSelectedItemText));
        }
    }

    public string BoundSelectedItemText => BoundSelectedItem?.Text ?? "-";

    public ComboBoxViewModel(IScreen screen)
    {
        HostScreen                    = screen;
        ComboBoxItems                 = CreateComboBoxItems();
        BoundSelectedItem             = ComboBoxItems[1];
        SetBoundSelectedItemCommand   = ReactiveCommand.Create(SetBoundSelectedItem);
        ClearBoundSelectedItemCommand = ReactiveCommand.Create(ClearBoundSelectedItem);
    }

    public ReactiveCommand<Unit, Unit> SetBoundSelectedItemCommand { get; }

    public ReactiveCommand<Unit, Unit> ClearBoundSelectedItemCommand { get; }

    private void SetBoundSelectedItem()
    {
        if (ComboBoxItems is { Count: > 2 })
        {
            BoundSelectedItem = ComboBoxItems[2];
        }
    }

    private void ClearBoundSelectedItem()
    {
        BoundSelectedItem = null;
    }

    private static List<ComboBoxItemData> CreateComboBoxItems()
    {
        return
        [
            new ComboBoxItemData { Text = "床前明月光" },
            new ComboBoxItemData { Text = "疑是地上霜" },
            new ComboBoxItemData { Text = "举头望明月" },
            new ComboBoxItemData { Text = "低头思故乡" }
        ];
    }

}

public class ComboBoxItemData
{
    public string Text { get; set; } = string.Empty;
}
