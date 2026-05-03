namespace AtomUI.Desktop.Controls;

internal class PageSizeComboBoxItem : ComboBoxItem
{
    protected override Type StyleKeyOverride { get; } = typeof(ComboBoxItem);
    public int PageSize { get; set; }
}