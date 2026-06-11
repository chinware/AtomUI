using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

public class HyperLinkButton : AbstractHyperLinkButton
{
    private IDisposable? _loadingIconMarginBinding;

    public HyperLinkButton()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _loadingIconMarginBinding?.Dispose();
        _loadingIconMarginBinding = null;

        var buttonIcon  = e.NameScope.Find("PART_ButtonIcon") as IconPresenter;
        var loadingIcon = e.NameScope.Find("PART_LoadingIcon") as Icon;
        if (buttonIcon is not null && loadingIcon is not null)
        {
            _loadingIconMarginBinding = loadingIcon.Bind(
                Layoutable.MarginProperty,
                buttonIcon.GetObservable(Layoutable.MarginProperty),
                BindingPriority.LocalValue);
        }
    }
}
