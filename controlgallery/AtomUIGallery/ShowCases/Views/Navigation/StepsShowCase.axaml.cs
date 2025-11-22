using System.Reactive.Disposables;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class StepsShowCase : ReactiveUserControl<StepsViewModel>
{
    public static readonly StyledProperty<double[]> DashedArrayProperty =
        AvaloniaProperty.Register<StepsShowCase, double[]>(nameof(DashedArray));
    
    public double[] DashedArray
    {
        get => GetValue(DashedArrayProperty);
        set => SetValue(DashedArrayProperty, value);
    }
    
    public StepsShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is StepsViewModel viewModel)
            {
                viewModel.CurrentStep   = 0;
                viewModel.PreviousButtonVisible = false;
            }

            NextStepButton.Click += HandleNextButtonClick;
            PreviousButton.Click += HandlePreviousButtonClick;
            disposables.Add(Disposable.Create(() =>
            {
                NextStepButton.Click -= HandleNextButtonClick;
                PreviousButton.Click -= HandlePreviousButtonClick;
            }));
        });
        InitializeComponent();
        DashedArray = [4d, 3d];
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
                NextStepButton.Content = "Done";
            }
            else
            {
                NextStepButton.Content = "Next";
            }
        }
    }
}