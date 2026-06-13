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
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public SliderShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
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

                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider1, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider2, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider3, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider4, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider5, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider6, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider7, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);

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
        EnsureSelectedScenarioContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearLazyScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ExamplesContent.DataContext = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }
        EnsureSelectedScenarioContent();
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not TabStripItem tabStripItem ||
            tabStripItem.Tag is not string scenario)
        {
            return;
        }

        var content = ResolveScenarioContent(scenario);
        if (!ReferenceEquals(ScenarioContentHost.Content, content))
        {
            ScenarioContentHost.Content = content;
        }
    }

    private void ClearLazyScenarioContent()
    {
        if (ScenarioContentHost.Content is not null &&
            !ReferenceEquals(ScenarioContentHost.Content, ExamplesContent))
        {
            ScenarioContentHost.Content = null;
        }
        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (scenario == ExamplesScenario)
        {
            ExamplesContent.DataContext = DataContext;
            return ExamplesContent;
        }

        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _lazyScenarioContentCache.Add(scenario, content);
        }

        return content;
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
