using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Interactivity;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

public partial class StepsInteractiveShowCase : GalleryReactiveUserControl<StepsViewModel>
{
    public static readonly StyledProperty<double[]> DashedArrayProperty =
        AvaloniaProperty.Register<StepsInteractiveShowCase, double[]>(nameof(DashedArray));

    public double[] DashedArray
    {
        get => GetValue(DashedArrayProperty);
        set => SetValue(DashedArrayProperty, value);
    }

    public StepsInteractiveShowCase()
    {
        InitializeComponent();
        DashedArray = [4d, 3d];

        this.WhenActivated(disposables =>
        {
            if (DataContext is StepsViewModel viewModel)
            {
                viewModel.CurrentStep            = 0;
                viewModel.PreviousButtonVisible = false;
                SetupNextButtonText();
            }

            NextStepButton.Click += HandleNextButtonClick;
            PreviousButton.Click += HandlePreviousButtonClick;
            var themeManager = Application.Current?.GetThemeManager();
            if (themeManager != null)
            {
                EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => SetupNextButtonText();
                themeManager.LanguageVariantChanged += handler;
                disposables.Add(Disposable.Create(() => themeManager.LanguageVariantChanged -= handler));
            }

            disposables.Add(Disposable.Create(() =>
            {
                NextStepButton.Click -= HandleNextButtonClick;
                PreviousButton.Click -= HandlePreviousButtonClick;
            }));
        });
    }

    private void HandleNextButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            if (viewModel.CurrentStep < CurrentStepContentSteps.ItemCount - 1)
            {
                viewModel.CurrentStep++;
            }

            SetupNextButtonText();
        }
    }

    private void HandlePreviousButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            if (viewModel.CurrentStep > 0)
            {
                viewModel.CurrentStep--;
            }

            SetupNextButtonText();
        }
    }

    private void SetupNextButtonText()
    {
        if (DataContext is StepsViewModel viewModel)
        {
            if (viewModel.CurrentStep == CurrentStepContentSteps.ItemCount - 1)
            {
                NextStepButton.Content = Lang(StepsShowCaseLangResourceKind.P2ContentDone, "Done");
            }
            else
            {
                NextStepButton.Content = Lang(StepsShowCaseLangResourceKind.P2ContentNext, "Next");
            }
        }
    }

    private static string Lang(StepsShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
