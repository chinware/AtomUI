using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;

namespace AtomUIGallery.ShowCases.QRCode;

public partial class QRCodeShowCase : GalleryReactiveUserControl<QRCodeViewModel>
{
    public const string LanguageId = nameof(QRCodeShowCase);

    public QRCodeShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is QRCodeViewModel viewModel)
            {
                viewModel.EccLevels =
                [
                    QRCodeEccLevel.L,
                    QRCodeEccLevel.M,
                    QRCodeEccLevel.Q,
                    QRCodeEccLevel.H
                ];
                Disposable.Create(() =>
                {
                    viewModel.EccLevels = null;
                }).DisposeWith(disposables);
            }
        });
    }

}
