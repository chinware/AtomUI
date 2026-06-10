using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Layout;

namespace AtomUIGallery.ShowCases.Form;

public partial class FormLayoutShowCase : GalleryReactiveUserControl<FormViewModel>
{
    public FormLayoutShowCase()
    {
        InitializeComponent();
        LayoutCaseForm.PropertyChanged += HandleLayoutCaseFormPropertyChanged;
        UpdateLayoutCaseFormBounds(LayoutCaseForm.FormLayout);
    }

    private void HandleLayoutCaseFormPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == AtomUIForm.FormLayoutProperty &&
            args.NewValue is FormLayout layout)
        {
            UpdateLayoutCaseFormBounds(layout);
        }
    }

    private void UpdateLayoutCaseFormBounds(FormLayout layout)
    {
        if (layout == FormLayout.Inline)
        {
            LayoutCaseForm.MinWidth            = 0;
            LayoutCaseForm.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        else
        {
            LayoutCaseForm.MinWidth            = 600;
            LayoutCaseForm.HorizontalAlignment = HorizontalAlignment.Left;
        }
    }

    private void HandleFormLayoutOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is FormViewModel vm &&
            args.CheckedOption.Tag is FormLayout formLayout)
        {
            vm.FormLayout = formLayout;
        }
    }
}
