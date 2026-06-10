using AtomUI.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Form;

public partial class FormBasicShowCase : GalleryReactiveUserControl<FormViewModel>
{
    public FormBasicShowCase()
    {
        InitializeComponent();
        BasicForm.InitialValues = CreateBasicFormInitialValues();
    }

    private static FormValues CreateBasicFormInitialValues()
    {
        var values = new FormValues();
        values.Add("remember", true);
        return values;
    }
}
