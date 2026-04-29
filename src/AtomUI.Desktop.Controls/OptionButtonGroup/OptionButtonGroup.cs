using AtomUI.Controls.Commons;
using AtomUI.Theme;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public class OptionButtonGroup : AbstractOptionButtonGroup
{
    public OptionButtonGroup()
    {
        this.RegisterTokenResourceScope(OptionButtonToken.ScopeProvider);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new OptionButton();
    }
}