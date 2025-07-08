using System.Globalization;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;

namespace AtomUI.Theme;

/// <summary>
/// 当切换主题时候就是动态的换 ResourceDictionary 里面的东西
/// </summary>
public class ThemeManager : Styles, IThemeManager
{
    public const string THEME_DIR = "Themes";
    public const string DEFAULT_THEME_ID = "DaybreakBlueLight";
    public const string DEFAULT_THEME_RES_PATH = $"avares://AtomUI.Theme/Assets/{THEME_DIR}";
    public static readonly CultureInfo DEFAULT_LANGUAGE = new(LanguageCode.zh_CN);

    private Theme? _activatedTheme;
    private readonly Dictionary<string, Theme> _themePool;
    private readonly List<string> _customThemeDirs;
    private readonly List<string> _builtInThemeDirs;
    private IList<IControlThemesProvider> _controlThemesProviders;
    
    private readonly Dictionary<CultureInfo, ResourceDictionary> _languages;
    private List<ILanguageProvider>? _languageProviders;

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
            var languageResource = TryGetLanguageResource(value ?? DEFAULT_LANGUAGE);
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
    
    internal ThemeManager()
    {
        Initialized += (sender, args) =>
        {

        };
        _themePool       =  new Dictionary<string, Theme>();
        _customThemeDirs =  new List<string>();
        var appName = Application.Current?.Name ?? "AtomUIApplication";
        _builtInThemeDirs = new List<string>
        {
            Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName),
                THEME_DIR)
        };
        DefaultThemeId          = DEFAULT_THEME_ID;
        _controlThemesProviders = new List<IControlThemesProvider>();
        ControlTokenTypes       = new List<Type>();
        _languageProviders      = new List<ILanguageProvider>();
        _languages              = new Dictionary<CultureInfo, ResourceDictionary>();
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

    public ITheme LoadTheme(string id)
    {
        if (_themePool.Count == 0)
        {
            ScanThemes();
        }

        if (!_themePool.ContainsKey(id))
        {
            throw new InvalidOperationException($"Theme: {id} not founded in theme pool.");
        }

        var theme = _themePool[id];
        if (theme.IsLoaded)
        {
            // TODO 这里记录一个日志
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
    /// <param name="id"></param>
    public void UnLoadTheme(string id)
    {
        if (!_themePool.ContainsKey(id))
        {
            // TODO 需要记录一个日志
            return;
        }

        if (_activatedTheme != null && _activatedTheme.Id == id)
        {
            // TODO 需要记录一个日志
            return;
        }

        var theme = _themePool[id];
        theme.NotifyAboutToUnload();
        ThemeAboutToUnload?.Invoke(this, new ThemeOperateEventArgs(theme));
        // TODO 进行卸载操作，暂时没有实现
        theme.NotifyUnloaded();
        ThemeUnloaded?.Invoke(this, new ThemeOperateEventArgs(theme));
    }

    public void SetActiveTheme(string id)
    {
        if (!_themePool.ContainsKey(id))
        {
            // TODO 需要记录一个日志
            return;
        }

        var oldTheme = _activatedTheme;
        if (oldTheme is not null)
        {
            oldTheme.NotifyAboutToDeActive();
        }

        var theme = _themePool[id];
        theme.NotifyAboutToActive();
        ThemeAboutToChange?.Invoke(this, new ThemeOperateEventArgs(oldTheme));
        _activatedTheme = theme;

        var themeVariant  = _activatedTheme.ThemeVariant;
        var themeResource = new ResourceDictionary();
        themeResource.ThemeDictionaries[themeVariant] = theme.ThemeResource;
        Resources.MergedDictionaries.Add(themeResource);

        if (oldTheme is not null)
        {
            oldTheme.NotifyDeActivated();
        }

        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme, oldTheme));
    }
    
    public void RegisterControlThemesProvider(IControlThemesProvider controlThemesProvider)
    {
        _controlThemesProviders.Add(controlThemesProvider);
    }

    public void RegisterLanguageProvider(ILanguageProvider languageProvider)
    {
        _languageProviders?.Add(languageProvider);
    }

    public void RegisterControlTokenType(Type tokenType)
    {
        ControlTokenTypes.Add(tokenType);
    }

    public void ScanThemes()
    {
        // 最开始的是用户指定的目录
        foreach (var path in _customThemeDirs)
        {
            AddThemesFromPath(path, _themePool);
        }

        // 优先级从高到低
        foreach (var path in _builtInThemeDirs)
        {
            AddThemesFromPath(path, _themePool);
        }

        // Assets 中的默认主题
        AddThemesFromAssets(_themePool);

        // TODO 如果这里为空的化需要记录一个日志
    }

    private ResourceDictionary TryGetLanguageResource(CultureInfo locale)
    {
        if (_languages.TryGetValue(locale, out var resource))
        {
            return resource;
        }

        return _languages[DEFAULT_LANGUAGE];
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

    private void AddThemesFromPath(string path, Dictionary<string, Theme> themes)
    {
        var searchPattern = "*.xml";
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, searchPattern);
            if (files.Length > 0)
            {
                AddThemesFromFilePaths(files, themes);
            }
        }
    }

    private void AddThemesFromAssets(Dictionary<string, Theme> themes)
    {
        var filePaths = AssetLoader.GetAssets(new Uri(DEFAULT_THEME_RES_PATH), null);
        AddThemesFromFilePaths(filePaths.Select(path => path.ToString()), themes);
    }

    private void AddThemesFromFilePaths(IEnumerable<string> filePaths, Dictionary<string, Theme> themes)
    {
        foreach (var filePath in filePaths)
        {
            var themeId = Path.GetFileNameWithoutExtension(filePath);
            if (themes.ContainsKey(themeId))
            {
                continue;
            }

            var theme = new Theme(themeId, filePath);
            ThemeCreated?.Invoke(this, new ThemeOperateEventArgs(theme));
            themes.Add(themeId, theme);
            theme.NotifyRegistered();
        }
    }

    internal void Configure()
    {
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

    internal void NotifyInitialized()
    {
        Initialized?.Invoke(this, EventArgs.Empty);
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