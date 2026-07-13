using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class CascaderViewFilterListItem : ListBoxItem
{
    public static readonly StyledProperty<bool> IsCandidateSelectedProperty =
        AvaloniaProperty.Register<CascaderViewFilterListItem, bool>(nameof(IsCandidateSelected));

    public bool IsCandidateSelected
    {
        get => GetValue(IsCandidateSelectedProperty);
        set => SetValue(IsCandidateSelectedProperty, value);
    }
}
