
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Icon;

public partial class IconShowCase : GalleryReactiveUserControl<IconViewModel>
{
    public const string LanguageId = nameof(IconShowCase);
    private const string OutlinedScenario = "Outlined";
    private const string FilledScenario   = "Filled";
    private const string TwoToneScenario  = "TwoTone";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public IconShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent);
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
            OutlinedScenario => new IconGallery()
            {
                IconThemeType = IconThemeType.Outlined,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            },
            FilledScenario => new IconGallery()
            {
                IconThemeType = IconThemeType.Filled,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            },
            TwoToneScenario => new IconGallery()
            {
                IconThemeType = IconThemeType.TwoTone,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            },
            _ => throw new InvalidOperationException($"Unknown Icon scenario: {scenario}")
        };
    }
}
