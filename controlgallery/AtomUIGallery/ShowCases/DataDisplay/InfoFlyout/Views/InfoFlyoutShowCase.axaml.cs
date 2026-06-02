using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.InfoFlyout;

public partial class InfoFlyoutShowCase : ReactiveUserControl<InfoFlyoutViewModel>
{
    public InfoFlyoutShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is InfoFlyoutViewModel viewModel)
            {
                ArrowSegmented.SelectionChanged += viewModel.HandleSelectionChanged;
                Disposable.Create(() =>
                {
                    ArrowSegmented.SelectionChanged -= viewModel.HandleSelectionChanged;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}
