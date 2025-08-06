using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;

namespace AtomUI.Theme;

/// <summary>
/// 当切换主题时候就是动态的换 ResourceDictionary 里面的东西
/// </summary>
internal class ThemeManager : Styles, IThemeManager
{
    public const string DEFAULT_THEME_RES_PATH = $"avares://AtomUI.Theme/Assets/{THEME_DIR}";
    public const string DEFAULT_APP_NAME = "AtomUIApplication";
    public const string THEME_DIR = "Themes";

    #region 公共属性定义

    public IList<ThemeAlgorithm>? ActivatedThemeAlgorithms { get; internal set; }
    
    #endregion
    
    public ITheme? ActivatedTheme => _activatedTheme;
    public IReadOnlyList<string> CustomThemeDirs => _customThemeDirs;
    public static ThemeManager Current { get; internal set; } = null!;
    public string DefaultThemeId { get; set; }

    internal List<Type> ControlTokenTypes { get; set; }

    private CultureInfo? _cultureInfo;

    public CultureInfo? CultureInfo
    {
        get => _cultureInfo;

        set
        {
            _cultureInfo = value;
            var languageResource = TryGetLanguageResource(value ?? IThemeManager.DEFAULT_LANGUAGE);
            foreach (var entry in languageResource)
            {
                Resources.Add(entry);
            }
        }
    }

    public event EventHandler<ThemeOperateEventArgs>? ThemeCreated;
    public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToLoad;
    public event EventHandler<ThemeOperateEventArgs>? ThemeLoaded;
    public event EventHandler<ThemeOperateEventArgs>? ThemeLoadFailed;
    public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToUnload;
    public event EventHandler<ThemeOperateEventArgs>? ThemeUnloaded;
    public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToChange;
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public event EventHandler? Initialized;
    
    private Theme? _activatedTheme;
    private readonly Dictionary<ThemeVariant, Theme> _themePool;
    private readonly List<string> _customThemeDirs;
    private readonly List<string> _builtInThemeDirs;
    private IList<IControlThemesProvider> _controlThemesProviders;
    private IList<IThemeAssetPathProvider> _themeAssetPathProviders;
    
    private readonly Dictionary<CultureInfo, ResourceDictionary> _languages;
    private List<ILanguageProvider>? _languageProviders;
    
    internal ThemeManager()
    {
        _themePool       =  new Dictionary<ThemeVariant, Theme>();
        _customThemeDirs =  new List<string>();
        var appName = Application.Current?.Name ?? DEFAULT_APP_NAME;
        _builtInThemeDirs = [Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName),
            THEME_DIR)];
        DefaultThemeId           = IThemeManager.DEFAULT_THEME_ID;
        _controlThemesProviders  = new List<IControlThemesProvider>();
        _themeAssetPathProviders = new List<IThemeAssetPathProvider>();
        ControlTokenTypes        = new List<Type>();
        _languageProviders       = new List<ILanguageProvider>();
        _languages               = new Dictionary<CultureInfo, ResourceDictionary>();
    }

    public IReadOnlyCollection<ITheme> AvailableThemes
    {
        get
        {
            if (_themePool.Count == 0)
            {
                ScanThemes();
            }

            return _themePool.Values;
        }
    }

    internal ITheme LoadTheme(ThemeVariant themeVariant)
    {
        if (!_themePool.TryGetValue(themeVariant, out var theme))
        {
            throw new InvalidOperationException($"Theme: {themeVariant} not founded in theme pool.");
        }
        
        if (theme.IsLoaded)
        {
            return theme;
        }

        theme.NotifyAboutToLoad();
        ThemeAboutToLoad?.Invoke(this, new ThemeOperateEventArgs(theme));
        try
        {
            theme.Load();
            theme.NotifyLoaded();
            ThemeLoaded?.Invoke(this, new ThemeOperateEventArgs(theme));
            return theme;
        }
        catch (Exception)
        {
            ThemeLoadFailed?.Invoke(this, new ThemeOperateEventArgs(theme));
            throw;
        }
    }

    /// <summary>
    /// 取消主题在 avalonia 里面的 resource 资源
    /// </summary>
    /// <param name="themeVariant"></param>
    internal void UnLoadTheme(ThemeVariant themeVariant)
    {
        if (!_themePool.ContainsKey(themeVariant))
        {
            // TODO 需要记录一个日志
            return;
        }

        if (_activatedTheme != null && _activatedTheme.ThemeVariant == themeVariant)
        {
            // TODO 需要记录一个日志
            return;
        }

        var theme = _themePool[themeVariant];
        theme.NotifyAboutToUnload();
        ThemeAboutToUnload?.Invoke(this, new ThemeOperateEventArgs(theme));
        // TODO 进行卸载操作，暂时没有实现
        theme.NotifyUnloaded();
        ThemeUnloaded?.Invoke(this, new ThemeOperateEventArgs(theme));
    }

    public void SetActiveTheme(ThemeVariant themeVariant)
    {
        if (!_themePool.ContainsKey(themeVariant))
        {
            throw new ThemeNotFoundException($"Theme {themeVariant} not found");
        }

        var oldTheme = _activatedTheme;
        if (oldTheme is not null)
        {
            oldTheme.NotifyAboutToDeActive();
        }

        var theme = _themePool[themeVariant];

        if (!theme.IsLoaded)
        {
            LoadTheme(themeVariant);
        }
        
        theme.NotifyAboutToActive();
        ThemeAboutToChange?.Invoke(this, new ThemeOperateEventArgs(oldTheme));
        _activatedTheme = theme;
        
        if (!Resources.ThemeDictionaries.ContainsKey(themeVariant))
        {
            Resources.ThemeDictionaries.Add(themeVariant, theme.ThemeResource);
        }

        if (oldTheme is not null)
        {
            oldTheme.NotifyDeActivated();
        }
        
        theme.NotifyActivated();
        ActivatedThemeAlgorithms = theme.Algorithms;

        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme, oldTheme));
    }
    
    public void RegisterControlThemesProvider(IControlThemesProvider controlThemesProvider)
    {
        _controlThemesProviders.Add(controlThemesProvider);
    }

    public void RegisterControlThemesProvider(IThemeAssetPathProvider themeAssetPathProvider)
    {
        _themeAssetPathProviders.Add(themeAssetPathProvider);
    }

    public void RegisterLanguageProvider(ILanguageProvider languageProvider)
    {
        _languageProviders?.Add(languageProvider);
    }

    public void RegisterControlTokenType(Type tokenType)
    {
        ControlTokenTypes.Add(tokenType);
    }

    internal void ScanThemes()
    {
        // 最开始的是用户指定的目录
        foreach (var path in _customThemeDirs)
        {
            AddThemesFromPath(path, _themePool, false);
        }

        // 优先级从高到低
        foreach (var path in _builtInThemeDirs)
        {
            AddThemesFromPath(path, _themePool, false);
        }

        foreach (var themeAssetPathProvider in _themeAssetPathProviders)
        {
            var filePaths = themeAssetPathProvider.GetThemeFilePaths();
            AddThemesFromFilePaths(filePaths, _themePool, true);
        }

        // Assets 中的默认主题
        AddThemesFromAssets(_themePool);
        Debug.Assert(_themePool.Count > 0);
    }

    private ResourceDictionary TryGetLanguageResource(CultureInfo locale)
    {
        if (_languages.TryGetValue(locale, out var resource))
        {
            return resource;
        }

        return _languages[IThemeManager.DEFAULT_LANGUAGE];
    }

    public void AddCustomThemePaths(IList<string> paths)
    {
        foreach (var path in paths)
        {
            var fullPath = Path.GetFullPath(path);
            if (!_customThemeDirs.Contains(fullPath) && Directory.Exists(fullPath))
            {
                _customThemeDirs.Add(fullPath);
            }
        }
    }

    private void AddThemesFromPath(string path, Dictionary<ThemeVariant, Theme> themes, bool isBuiltIn)
    {
        var searchPattern = "*.xml";
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, searchPattern);
            if (files.Length > 0)
            {
                AddThemesFromFilePaths(files, themes, isBuiltIn);
            }
        }
    }

    private void AddThemesFromAssets(Dictionary<ThemeVariant, Theme> themes)
    {
        var filePaths = AssetLoader.GetAssets(new Uri(DEFAULT_THEME_RES_PATH), null);
        AddThemesFromFilePaths(filePaths.Select(path => path.ToString()), themes, true);
    }

    private void AddThemesFromFilePaths(IEnumerable<string> filePaths, Dictionary<ThemeVariant, Theme> themes, bool isBuiltIn)
    {
        var algorithmsCombination = new List<ISet<ThemeAlgorithm>>();
        algorithmsCombination.Add(new HashSet<ThemeAlgorithm>{ThemeAlgorithm.Default});
        algorithmsCombination.Add(new HashSet<ThemeAlgorithm>{ThemeAlgorithm.Default, ThemeAlgorithm.Dark});
        algorithmsCombination.Add(new HashSet<ThemeAlgorithm>{ThemeAlgorithm.Default, ThemeAlgorithm.Dark, ThemeAlgorithm.Compact});
        algorithmsCombination.Add(new HashSet<ThemeAlgorithm>{ThemeAlgorithm.Default, ThemeAlgorithm.Compact});
        foreach (var filePath in filePaths)
        {
            var themeId      = Path.GetFileNameWithoutExtension(filePath);
            foreach (var algorithms in algorithmsCombination)
            {
                var theme        = new Theme(themeId, filePath, algorithms ,isBuiltIn);
                var themeVariant = theme.ThemeVariant;
                if (themes.ContainsKey(themeVariant))
                {
                    continue;
                }
                ThemeCreated?.Invoke(this, new ThemeOperateEventArgs(theme));
                themes.Add(themeVariant, theme);
                theme.NotifyRegistered();
            }
        }
    }

    internal void Configure()
    {
        ScanThemes();
        foreach (var provider in _controlThemesProviders)
        {
            foreach (var resourceProvider in provider.ControlThemes)
            {
                Resources.MergedDictionaries.Add(resourceProvider);
            }
        }
        _controlThemesProviders.Clear();
        BuildLanguageResources();
    }

    private void BuildLanguageResources()
    {
        if (_languageProviders is not null)
        {
            foreach (var languageProvider in _languageProviders)
            {
                var culture = new CultureInfo(languageProvider.LangCode);
                if (!_languages.ContainsKey(culture))
                {
                    _languages[culture] = new ResourceDictionary();
                }

                var resourceDictionary = _languages[culture];
                languageProvider.BuildResourceDictionary(resourceDictionary);
            }

            _languageProviders = null;
        }
    }

    internal virtual void NotifyInitialized()
    {
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    internal virtual void NotifyAttachedToApplication()
    {
    }
}

public class ThemeOperateEventArgs : EventArgs
{
    public ITheme? Theme { get; }

    public ThemeOperateEventArgs(ITheme? theme)
    {
        Theme = theme;
    }
}

public class ThemeChangedEventArgs : EventArgs
{
    public ITheme? OldTheme { get; }
    public ITheme NewTheme { get; }

    public ThemeChangedEventArgs(ITheme newTheme, ITheme? oldTheme)
    {
        NewTheme = newTheme;
        OldTheme = oldTheme;
    }
}