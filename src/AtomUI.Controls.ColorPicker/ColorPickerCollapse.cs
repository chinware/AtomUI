using System.Reactive.Disposables;

namespace AtomUI.Controls;

internal class ColorPickerCollapse : Collapse
{
    protected override void PrepareCollapseItem(CollapseItem collapseItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
        if (item is ColorPickerPalette colorPickerPalette)
        {
            collapseItem.SetCurrentValue(CollapseItem.IsSelectedProperty, colorPickerPalette.IsOpen);
        }
    }
}