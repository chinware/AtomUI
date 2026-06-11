using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia.Controls;
using Avalonia.VisualTree;
using ButtonSpinner = AtomUI.Desktop.Controls.ButtonSpinner;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

public partial class ButtonSpinnerShowCase : GalleryReactiveUserControl<ButtonSpinnerViewModel>
{
    public const string LanguageId = nameof(ButtonSpinnerShowCase);

    public ButtonSpinnerShowCase()
    {
        this.WhenActivated(disposables =>
        {
            BindSpinHandleRecursively(this);
            Disposable.Create(() => UnBindSpinHandleRecursively(this))
                      .DisposeWith(disposables);
        });
        InitializeComponent();
    }

    private void BindSpinHandleRecursively(Control control)
    {
        if (control is AtomUIButtonSpinner spinner)
        {
            if (DataContext is ButtonSpinnerViewModel viewModel)
            {
                spinner.Spin += viewModel.HandleSpin;
            }
        }
        else
        {
            foreach (var item in control.GetVisualChildren())
            {
                if (item is Control childControl)
                {
                    BindSpinHandleRecursively(childControl);
                }
            }
        }
    }

    private void UnBindSpinHandleRecursively(Control control)
    {
        if (control is AtomUIButtonSpinner spinner)
        {
            if (DataContext is ButtonSpinnerViewModel viewModel)
            {
                spinner.Spin -= viewModel.HandleSpin;
            }
        }
        else
        {
            foreach (var item in control.GetVisualChildren())
            {
                if (item is Control childControl)
                {
                    UnBindSpinHandleRecursively(childControl);
                }
            }
        }
    }
}
