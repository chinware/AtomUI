namespace AtomUI.Desktop.Controls;

internal class ColorPickerCollapse : Collapse
{
    protected override Type StyleKeyOverride { get; } = typeof(Collapse);

    protected override void PrepareCollapseItem(CollapseItem collapseItem, object? item, int index)
    {
        if (item is ColorPickerPalette colorPickerPalette)
        {
            collapseItem.SetCurrentValue(CollapseItem.IsSelectedProperty, colorPickerPalette.IsOpen);
        }
    }
}