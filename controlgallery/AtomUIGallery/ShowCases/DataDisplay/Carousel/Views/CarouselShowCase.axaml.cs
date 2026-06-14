using System;
using System.Collections.Generic;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Carousel;

public partial class CarouselShowCase : GalleryReactiveUserControl<CarouselViewModel>
{
    public const string LanguageId = nameof(CarouselShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public CarouselShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new CarouselApiDataGrid(),
            DesignTokenScenario => new CarouselDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Carousel scenario: {scenario}")
        };
    }

    public void HandlePositionOptionChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is CarouselViewModel viewModel)
        {
            if (args.Index == 0)
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Top;
            }
            else if (args.Index == 1)
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Bottom;
            }
            else if (args.Index == 2)
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Left;
            }
            else
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Right;
            }
        }

    }
}
