
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

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public PaletteShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
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
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            SynchronizeScenarioContent(content);
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
        ScenarioContentHost.Content = null;
        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content = CreateScenarioContent(scenario);
            _lazyScenarioContentCache.Add(scenario, content);
        }

        SynchronizeScenarioContent(content);
        return content;
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

    private void SynchronizeScenarioContent(Control content)
    {
        content.DataContext = DataContext;
        if (content is ContentControl contentControl)
        {
            contentControl.Content = DataContext;
        }
    }
}
