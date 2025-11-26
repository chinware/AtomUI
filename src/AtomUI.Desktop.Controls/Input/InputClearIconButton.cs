using AtomUI.Icons.AntDesign;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class InputClearIconButton : IconButton
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetCurrentValue(IconProperty, new CloseCircleFilled());
    }
}