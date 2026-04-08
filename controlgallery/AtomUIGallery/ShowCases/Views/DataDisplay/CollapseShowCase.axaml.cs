using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CollapseShowCase : ReactiveUserControl<CollapseViewModel>
{
    public CollapseShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is CollapseViewModel viewModel)
            {
                ExpandButtonPosGroup.OptionCheckedChanged += viewModel.HandleExpandButtonPosOptionCheckedChanged;
                Disposable.Create(() =>
                {
                    ExpandButtonPosGroup.OptionCheckedChanged -= viewModel.HandleExpandButtonPosOptionCheckedChanged;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}