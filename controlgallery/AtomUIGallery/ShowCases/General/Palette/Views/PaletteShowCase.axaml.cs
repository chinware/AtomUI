
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUIGallery.ShowCases.Palette;

public partial class PaletteShowCase : GalleryReactiveUserControl<PaletteViewModel>
{
    public const string LanguageId = nameof(PaletteShowCase);
    private const string LightScenario = "Light";
    private const string DarkScenario  = "Dark";
    private const string LightPaletteContentTemplateKey = "LightPaletteContentTemplate";
    private const string DarkPaletteContentTemplateKey  = "DarkPaletteContentTemplate";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public PaletteShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, synchronizeScenarioContent: SynchronizeScenarioContent);
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

    private Control CreateScenarioContent(string scenario)
    {
        var template = ResolveScenarioContentTemplate(scenario);
        return new ContentControl()
        {
            ContentTemplate = template,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
        };
    }

    private IDataTemplate ResolveScenarioContentTemplate(string scenario)
    {
        var templateKey = scenario switch
        {
            LightScenario => LightPaletteContentTemplateKey,
            DarkScenario  => DarkPaletteContentTemplateKey,
            _             => throw new InvalidOperationException($"Unknown Palette scenario: {scenario}")
        };

        if (Resources[templateKey] is not IDataTemplate template)
        {
            throw new InvalidOperationException($"Could not find Palette content template: {templateKey}");
        }

        return template;
    }

    private static void SynchronizeScenarioContent(Control content, object? dataContext)
    {
        content.DataContext = dataContext;
        if (content is ContentControl contentControl)
        {
            contentControl.Content = dataContext;
        }
    }
}
