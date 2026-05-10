using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class RangeDatePickerPresenterTheme : ControlTheme
{
    public RangeDatePickerPresenterTheme() : base(typeof(RangeDatePickerPresenter))
    {
        AvaloniaXamlLoader.Load(this);
    }
}
