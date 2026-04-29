using AtomUI.Controls.Commons;
using AtomUI.Theme;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

public class Separator : AbstractSeparator
{
    public Separator()
    {
        this.RegisterTokenResourceScope(SeparatorToken.ScopeProvider);
    }
}

public class VerticalSeparator : Separator
{
    static VerticalSeparator()
    {
        OrientationProperty.OverrideDefaultValue<VerticalSeparator>(Orientation.Vertical);
    }

    protected override Type StyleKeyOverride => typeof(Separator);
}