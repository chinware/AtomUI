using System.Reactive.Disposables.Fluent;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class QRCodeShowCase : ReactiveUserControl<QRCodeViewModel>
{
    public QRCodeShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is QRCodeViewModel viewModel)
            {
                this.BindCommand(viewModel, vm => vm.SmallerCommand, v => v.SmallerBtn)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.LargerCommand, v => v.LargerBtn)
                    .DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}