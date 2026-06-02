using System.Diagnostics;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Form;

public partial class FormStateShowCase : ReactiveUserControl<FormViewModel>
{
    public FormStateShowCase()
    {
        InitializeComponent();
    }

    private void HandleFormStyleVariantChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is AtomUISegmented segmented &&
            segmented.SelectedItem is SegmentedItem segmentedItem)
        {
            var styleVariant = segmentedItem.Tag as InputControlStyleVariant?;
            Debug.Assert(styleVariant != null);
            if (DataContext is FormViewModel vm)
            {
                vm.FormStyleVariant = styleVariant.Value;
            }
        }
    }

    private void HandleFormRequiredMarkChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is FormViewModel vm &&
            args.CheckedOption.Tag is FormRequiredMark requiredMark)
        {
            vm.FormRequiredMark = requiredMark;
        }
    }

    private void HandleFormSizeTypeChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is FormViewModel vm &&
            args.CheckedOption.Tag is SizeType sizeType)
        {
            vm.FormSizeType = sizeType;
        }
    }
}
