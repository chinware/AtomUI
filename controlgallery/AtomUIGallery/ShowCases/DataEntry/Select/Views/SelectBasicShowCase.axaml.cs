using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Select;

public partial class SelectBasicShowCase : ReactiveUserControl<SelectViewModel>
{
    public SelectBasicShowCase()
    {
        InitializeComponent();
        CustomSearchSelect.Filter = new CustomFilter();
    }
}
