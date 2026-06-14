using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUIGallery.ShowCases.Slider;

public partial class SliderShowCase : GalleryReactiveUserControl<SliderViewModel>
{
    public const string LanguageId = nameof(SliderShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public SliderShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
        this.WhenActivated(disposables =>
        {
            if (DataContext is SliderViewModel viewModel)
            {
                var marks = new List<SliderMark>();
                marks.Add(new SliderMark("0°C", 0));
                marks.Add(new SliderMark("26°C", 26));
                marks.Add(new SliderMark("37°C", 37));
                marks.Add(new SliderMark("100°C", 100)
                {
                    LabelFontWeight = FontWeight.Bold,
                    LabelBrush      = new SolidColorBrush(Colors.Red)
                });
                viewModel.SliderMarks = marks;

                Disposable.Create(() =>
                {
                    viewModel.SliderMarks = null;
                }).DisposeWith(disposables);
            }
        });
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
            ApiScenario         => new SliderApiDataGrid(),
            DesignTokenScenario => new SliderDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Slider scenario: {scenario}")
        };
    }
}
