using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace AtomUIGallery.ShowCases.Tooltip;

public partial class TooltipShowCase : GalleryReactiveUserControl<TooltipViewModel>
{
    public const string LanguageId = nameof(TooltipShowCase);

    public TooltipShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TooltipViewModel viewModel)
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
