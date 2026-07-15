using System.Reactive.Disposables;
using AtomUI.Controls.Commons;

namespace AtomUI.Desktop.Controls;

public class FloatButtonHost : AbstractFloatButtonHost
{
    protected override AbstractFloatButton CreateFloatButton(CompositeDisposable disposables)
    {
        var floatButton = new FloatButton();
        BindFloatButtonProperties(floatButton, disposables);
        return floatButton;
    }
}
