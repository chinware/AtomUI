using AtomUI.Icons.AntDesign;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls.Themes;

internal class RevealButton : ToggleIconButton
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetCurrentValue(CheckedIconProperty, new EyeOutlined());
        SetCurrentValue(UnCheckedIconProperty, new EyeInvisibleOutlined());
    }
}