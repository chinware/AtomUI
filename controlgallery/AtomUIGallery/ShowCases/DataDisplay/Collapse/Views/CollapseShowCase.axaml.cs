using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Collapse;

public partial class CollapseShowCase : ReactiveUserControl<CollapseViewModel>
{
    public const string LanguageId = nameof(CollapseShowCase);

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
