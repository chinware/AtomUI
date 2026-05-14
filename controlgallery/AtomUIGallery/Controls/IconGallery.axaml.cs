using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Models;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;

namespace AtomUIGallery.Controls;

public class IconGallery : TemplatedControl
{
    private const int InitialIconLoadCount = 96;
    private const int IncrementalIconLoadCount = 96;
    private const double LoadMoreScrollThreshold = 240;
    private const string ScrollViewerPart = "PART_ScrollViewer";
    private const string SearchInputPart = "PART_SearchInput";

    public static readonly StyledProperty<IconThemeType?> IconThemeTypeProperty =
        AvaloniaProperty.Register<IconGallery, IconThemeType?>(nameof(IconThemeType));

    public IconThemeType? IconThemeType
    {
        get => GetValue(IconThemeTypeProperty);
        set => SetValue(IconThemeTypeProperty, value);
    }

    #region 内部属性定义

    internal static readonly StyledProperty<AvaloniaList<PackageIconItem>?> IconInfosProperty =
        AvaloniaProperty.Register<IconGallery, AvaloniaList<PackageIconItem>?>(nameof(IconInfos));

    internal AvaloniaList<PackageIconItem>? IconInfos
    {
        get => GetValue(IconInfosProperty);
        set => SetValue(IconInfosProperty, value);
    }

    #endregion

    private readonly List<PackageIconItem> _matchedIconInfos = new();
    private AvaloniaList<PackageIconItem> _activatedIconInfos = new();
    private ScrollViewer? _scrollViewer;
    private SearchEdit? _searchEdit;
    private int _loadedIconCount;

    private bool HasMoreIconInfos => _loadedIconCount < _matchedIconInfos.Count;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged -= HandleScrollChanged;
        }

        if (_searchEdit != null)
        {
            _searchEdit.SearchButtonClick -= HandleSearchButtonClick;
        }

        _scrollViewer = e.NameScope.Find<ScrollViewer>(ScrollViewerPart);
        _searchEdit   = e.NameScope.Find<SearchEdit>(SearchInputPart);
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged += HandleScrollChanged;
        }

        if (_searchEdit != null)
        {
            _searchEdit.SearchButtonClick += HandleSearchButtonClick;
        }

        ReLoadIcons();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged -= HandleScrollChanged;
            _scrollViewer = null;
        }

        if (_searchEdit != null)
        {
            _searchEdit.SearchButtonClick -= HandleSearchButtonClick;
            _searchEdit = null;
        }

        ResetActivatedIconInfos();
        _matchedIconInfos.Clear();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconThemeTypeProperty)
        {
            if (VisualRoot is not null)
            {
                ReLoadIcons();
            }
        }
    }

    private void ReLoadIcons()
    {
        ResetActivatedIconInfos();
        _matchedIconInfos.Clear();

        var allIconClasses = CachedLoadedAssemblyTypeScanner.GetInheritedTypes<Icon>("AtomUI.Icons.AntDesign");
        var iconThemeName = IconThemeType?.ToString();
        var targetClasses = allIconClasses
            .Where(t => string.IsNullOrEmpty(iconThemeName) || t.Name.EndsWith(iconThemeName, StringComparison.Ordinal))
            .OrderBy(t => t.Name)
            .ToArray();

        var filter = _searchEdit?.Text?.Trim();
        foreach (var iconInfoType in targetClasses)
        {
            if (!string.IsNullOrEmpty(filter) &&
                !iconInfoType.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            _matchedIconInfos.Add(new PackageIconItem(
                iconInfoType.Name,
                iconInfoType,
                () => (Icon)Activator.CreateInstance(iconInfoType)!));
        }

        LoadMoreIconInfos(InitialIconLoadCount);
    }

    private void HandleScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (_scrollViewer == null || !HasMoreIconInfos)
        {
            return;
        }

        var remainingHeight = _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height - _scrollViewer.Offset.Y;
        if (remainingHeight <= LoadMoreScrollThreshold)
        {
            LoadMoreIconInfos(IncrementalIconLoadCount);
        }
    }

    private void HandleSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        ReLoadIcons();
    }

    private void LoadMoreIconInfos(int count)
    {
        if (!HasMoreIconInfos)
        {
            return;
        }

        var targetCount = Math.Min(_loadedIconCount + count, _matchedIconInfos.Count);
        for (var i = _loadedIconCount; i < targetCount; i++)
        {
            var iconInfo = _matchedIconInfos[i];
            iconInfo.Icon = iconInfo.Creator?.Invoke();
            _activatedIconInfos.Add(iconInfo);
        }

        _loadedIconCount = targetCount;
    }

    private void ResetActivatedIconInfos()
    {
        foreach (var iconInfo in _activatedIconInfos)
        {
            iconInfo.Icon = null;
        }

        _loadedIconCount    = 0;
        _activatedIconInfos = new AvaloniaList<PackageIconItem>();
        SetCurrentValue(IconInfosProperty, _activatedIconInfos);
        if (_scrollViewer != null)
        {
            _scrollViewer.Offset = new Vector(0, 0);
        }
    }
}

public static class CachedLoadedAssemblyTypeScanner
{
    private static FrozenSet<Assembly>? _loadedAssemblies;
    private static DateTime _lastAssemblyCheck = DateTime.MinValue;
    private static readonly TimeSpan AssemblyCacheTimeout = TimeSpan.FromSeconds(5);

    private static readonly ConcurrentDictionary<string, FrozenSet<Type>> _typeScanCache = new();

    public static Assembly GetLoadedAssembly(string assemblyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(assemblyName);

        var assemblies = GetCachedLoadedAssemblies();

        var assembly = assemblies.FirstOrDefault(asm =>
            asm.GetName().Name?.Equals(assemblyName, StringComparison.OrdinalIgnoreCase) == true);

        return assembly ?? throw new FileNotFoundException($"未找到已加载的程序集: {assemblyName}");
    }

    private static FrozenSet<Assembly> GetCachedLoadedAssemblies()
    {
        if (_loadedAssemblies is null || DateTime.UtcNow - _lastAssemblyCheck > AssemblyCacheTimeout)
        {
            _loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToFrozenSet();
            _lastAssemblyCheck = DateTime.UtcNow;
        }

        return _loadedAssemblies;
    }

    public static IReadOnlySet<Type> GetInheritedTypes(string assemblyName, Type baseType, bool includeAbstract = false)
    {
        ArgumentNullException.ThrowIfNull(baseType);

        var cacheKey = $"{assemblyName}|{baseType.FullName}|{includeAbstract}";

        return _typeScanCache.GetOrAdd(cacheKey, _ =>
        {
            var assembly = GetLoadedAssembly(assemblyName);
            var types = ScanInheritedTypes(assembly, baseType, includeAbstract);
            return types.ToFrozenSet();
        });
    }

    public static IReadOnlySet<Type> GetInheritedTypes<TBase>(string assemblyName, bool includeAbstract = false)
        where TBase : class
    {
        return GetInheritedTypes(assemblyName, typeof(TBase), includeAbstract);
    }

    private static IEnumerable<Type> ScanInheritedTypes(Assembly assembly, Type baseType, bool includeAbstract)
    {
        Type[] types;

        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t is not null).ToArray()!;
        }

        foreach (var type in types)
        {
            if (IsValidInheritedType(type, baseType, includeAbstract))
            {
                yield return type;
            }
        }
    }

    private static bool IsValidInheritedType(Type? type, Type baseType, bool includeAbstract)
    {
        return type is not null &&
               type != baseType &&
               type.IsClass &&
               baseType.IsAssignableFrom(type) &&
               (includeAbstract || !type.IsAbstract);
    }

    public static void ClearCache()
    {
        _typeScanCache.Clear();
        _loadedAssemblies = null;
    }
}
