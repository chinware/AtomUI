using Avalonia.Controls;
using AvaloniaContentControl = Avalonia.Controls.ContentControl;
using AtomUITabStrip = AtomUI.Desktop.Controls.TabStrip;
using AtomUITabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUI.Toolkits.GalleryBase.Controls;

public sealed class GalleryShowCaseScenarioController : IDisposable
{
    private const string DefaultExamplesScenario = "Examples";

    private readonly AtomUITabStrip _scenarioTabs;
    private readonly AvaloniaContentControl _contentHost;
    private readonly Control? _examplesContent;
    private readonly string _examplesScenario;
    private readonly Func<string, Control> _createScenarioContent;
    private readonly Action<Control, object?> _synchronizeScenarioContent;
    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);
    private object? _dataContext;
    private bool _disposed;

    public GalleryShowCaseScenarioController(
        AtomUITabStrip scenarioTabs,
        AvaloniaContentControl contentHost,
        Func<string, Control> createScenarioContent,
        Control? examplesContent = null,
        string examplesScenario = DefaultExamplesScenario,
        Action<Control, object?>? synchronizeScenarioContent = null)
    {
        ArgumentNullException.ThrowIfNull(scenarioTabs);
        ArgumentNullException.ThrowIfNull(contentHost);
        ArgumentNullException.ThrowIfNull(createScenarioContent);
        ArgumentException.ThrowIfNullOrWhiteSpace(examplesScenario);

        _scenarioTabs               = scenarioTabs;
        _contentHost                = contentHost;
        _examplesContent            = examplesContent;
        _examplesScenario           = examplesScenario;
        _createScenarioContent      = createScenarioContent;
        _synchronizeScenarioContent = synchronizeScenarioContent ?? SynchronizeDataContext;

        _scenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
    }

    public void Attach(object? dataContext)
    {
        ThrowIfDisposed();
        _dataContext = dataContext;
        EnsureSelectedScenarioContent();
    }

    public void Detach()
    {
        if (_disposed)
        {
            return;
        }

        ClearLazyScenarioContent();
    }

    public void UpdateDataContext(object? dataContext)
    {
        ThrowIfDisposed();

        _dataContext = dataContext;
        if (_examplesContent is not null)
        {
            _synchronizeScenarioContent(_examplesContent, _dataContext);
        }

        foreach (var content in _lazyScenarioContentCache.Values)
        {
            _synchronizeScenarioContent(content, _dataContext);
        }

        EnsureSelectedScenarioContent();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _scenarioTabs.SelectionChanged -= HandleScenarioSelectionChanged;
        ClearLazyScenarioContent();
        _disposed = true;
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (_scenarioTabs.SelectedItem is not AtomUITabStripItem tabStripItem ||
            tabStripItem.Tag is not string scenario)
        {
            return;
        }

        var content = ResolveScenarioContent(scenario);
        if (!ReferenceEquals(_contentHost.Content, content))
        {
            _contentHost.Content = content;
        }
    }

    private void ClearLazyScenarioContent()
    {
        if (_examplesContent is null)
        {
            _contentHost.Content = null;
        }
        else if (_contentHost.Content is not null &&
                 !ReferenceEquals(_contentHost.Content, _examplesContent))
        {
            _contentHost.Content = null;
        }

        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (_examplesContent is not null &&
            scenario == _examplesScenario)
        {
            _synchronizeScenarioContent(_examplesContent, _dataContext);
            return _examplesContent;
        }

        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content = _createScenarioContent(scenario);
            _lazyScenarioContentCache.Add(scenario, content);
        }

        _synchronizeScenarioContent(content, _dataContext);
        return content;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private static void SynchronizeDataContext(Control content, object? dataContext)
    {
        content.DataContext = dataContext;
    }
}
