using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
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
                viewModel.EccLevels = new List<string>
                {
                    nameof(QRCodeEccLevel.L),
                    nameof(QRCodeEccLevel.M),
                    nameof(QRCodeEccLevel.Q),
                    nameof(QRCodeEccLevel.H)
                };
                this.BindCommand(viewModel, vm => vm.SmallerCommand, v => v.SmallerBtn)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.LargerCommand, v => v.LargerBtn)
                    .DisposeWith(disposables);
                
                this.OneWayBind(viewModel, vm => vm.EccLevels, v => v.EccLevelSegmented.ItemsSource)
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    viewModel.EccLevels = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}