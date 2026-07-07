using Avalonia;
using Avalonia.Controls;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

public partial class ButtonSpinnerShowCase : GalleryReactiveUserControl<ButtonSpinnerViewModel>
{
    public const string LanguageId = nameof(ButtonSpinnerShowCase);

    public ButtonSpinnerShowCase()
    {
        InitializeComponent();
        AddHandler(Spinner.SpinEvent, HandleSpin);
    }

    private void HandleSpin(object? sender, SpinEventArgs args)
    {
        if (DataContext is ButtonSpinnerViewModel viewModel)
        {
            viewModel.HandleSpin(args.Source, args);
        }
    }
}
