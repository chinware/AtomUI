using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.QRCode;

public partial class QRCodeShowCase : GalleryReactiveUserControl<QRCodeViewModel>
{
    public const string LanguageId = nameof(QRCodeShowCase);

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
                GalleryBindingUtils.BindCommand(SmallerBtn, viewModel.SmallerCommand).DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(LargerBtn, viewModel.LargerCommand).DisposeWith(disposables);
                
                GalleryBindingUtils.OneWay(viewModel, nameof(QRCodeViewModel.EccLevels), vm => vm.EccLevels,
                                           EccLevelSegmented, Avalonia.Controls.ItemsControl.ItemsSourceProperty)
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
