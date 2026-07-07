using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;
using AtomUISteps = AtomUI.Desktop.Controls.Steps;

namespace AtomUIGallery.ShowCases.Steps;

public partial class StepsShowCase : GalleryReactiveUserControl<StepsViewModel>
{
    public const string LanguageId = nameof(StepsShowCase);

    public static readonly StyledProperty<double[]> DashedArrayProperty =
        AvaloniaProperty.Register<StepsShowCase, double[]>(nameof(DashedArray));

    public double[] DashedArray
    {
        get => GetValue(DashedArrayProperty);
        set => SetValue(DashedArrayProperty, value);
    }

    public StepsShowCase()
    {
        InitializeComponent();
        DashedArray = [4d, 3d];

        this.WhenActivated(disposables =>
        {
            ResetInteractiveState();

            var themeManager = Application.Current?.GetThemeManager();
            if (themeManager != null)
            {
                EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshInteractiveButtonText();
                themeManager.LanguageVariantChanged += handler;
                Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                          .DisposeWith(disposables);
            }
        });
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        ResetInteractiveState();
    }

    public void HandleNextButtonClick(object? sender, RoutedEventArgs args)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.MoveToNextInteractiveStep();
        }
    }

    public void HandlePreviousButtonClick(object? sender, RoutedEventArgs args)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.MoveToPreviousInteractiveStep();
        }
    }

    public void HandleInteractiveStepsLoaded(object? sender, RoutedEventArgs args)
    {
        if (sender is not Control root)
        {
            return;
        }

        var steps = FindDescendantByName<AtomUISteps>(root, "CurrentStepContentSteps");
        var presenter = FindDescendantByName<ContentPresenter>(root, "CurrentStepContentPresenter");
        if (steps is null || presenter is null)
        {
            return;
        }

        presenter[!ContentPresenter.ContentProperty] = steps[!AtomUISteps.CurrentContentProperty];
        presenter[!ContentPresenter.ContentTemplateProperty] = steps[!AtomUISteps.CurrentContentTemplateProperty];
    }

    private void ResetInteractiveState()
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.ResetInteractiveStep();
        }
    }

    private void RefreshInteractiveButtonText()
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.RefreshInteractiveButtonText();
        }
    }

    private static T? FindDescendantByName<T>(Control root, string name)
        where T : Control
    {
        if (root is T typedRoot && typedRoot.Name == name)
        {
            return typedRoot;
        }

        return root.GetVisualDescendants().OfType<T>().FirstOrDefault(control => control.Name == name)
               ?? root.GetLogicalDescendants().OfType<T>().FirstOrDefault(control => control.Name == name);
    }
}
