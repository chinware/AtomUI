using System.Reactive.Disposables;
using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class FloatButtonHost : AbstractFloatButtonHost
{
    public FloatButtonHost()
    {
        this.RegisterTokenResourceScope(FloatButtonToken.ScopeProvider);
    }
    
    protected override AbstractFloatButton CreateFloatButton(CompositeDisposable disposables)
    {
        var floatButton = new FloatButton();
        BindFloatButtonProperties(floatButton, disposables);
        return floatButton;
    }
}
