using Avalonia.Platform;
using Avalonia.Styling;

namespace AtomUI;

/// <summary>
/// 当切换主题时候就是动态的换 ResourceDictionary 里面的东西
/// </summary>
public class ThemeManager : Styles, IThemeManager
{
   public const string THEME_DIR = "Themes";
   public const string DEFAULT_THEME_ID = "DaybreakBlueLight";
   public const string DEFAULT_THEME_RES_PATH = $"avares://AtomUI/Assets/{THEME_DIR}";
   
   private Theme? _activatedTheme;
   private Dictionary<string, Theme> _themePool;
   private List<string> _customThemeDirs;
   private List<string> _builtInThemeDirs;
   private string DefaultThemeId { get; set; }
   
   public ITheme? ActivatedTheme => _activatedTheme;
   public IReadOnlyList<string> CustomThemeDirs => _customThemeDirs;
   public static ThemeManager Current { get; }

   public event EventHandler<ThemeOperateEventArgs>? ThemeCreatedEvent;
   public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToLoadEvent;
   public event EventHandler<ThemeOperateEventArgs>? ThemeLoadedEvent;
   public event EventHandler<ThemeOperateEventArgs>? ThemeLoadFailedEvent;
   public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToUnloadEvent;
   public event EventHandler<ThemeOperateEventArgs>? ThemeUnloadedEvent;
   public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToChangeEvent;
   public event EventHandler<ThemeChangedEventArgs>? ThemeChangedEvent;

   static ThemeManager()
   {
      Current = new ThemeManager();
   }

   protected ThemeManager()
   {
      _themePool = new Dictionary<string, Theme>();
      _customThemeDirs = new List<string>();
      _builtInThemeDirs = new List<string>
      {
         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), THEME_DIR)
      };
      DefaultThemeId = DEFAULT_THEME_ID;
   }

   public IReadOnlyCollection<ITheme> AvailableThemes
   {
      get
      {
         if (_themePool.Count == 0) {
            ScanThemes();
         }
         return _themePool.Values;
      }
   }
   
   public ITheme LoadTheme(string id)
   {
      if (_themePool.Count == 0) {
         ScanThemes();
      }

      if (!_themePool.ContainsKey(id)) {
         throw new InvalidOperationException($"Theme: {id} not founded in theme pool.");
      }

      Theme theme = _themePool[id];
      if (theme.IsLoaded) {
         // TODO 这里记录一个日志
         return theme;
      }
      
      theme.NotifyAboutToLoad();
      ThemeAboutToLoadEvent?.Invoke(this, new ThemeOperateEventArgs(theme));
      try {
         theme.Load();
         theme.NotifyLoaded();
         ThemeLoadedEvent?.Invoke(this, new ThemeOperateEventArgs(theme));
         return theme;
      } catch (Exception) {
         ThemeLoadFailedEvent?.Invoke(this, new ThemeOperateEventArgs(theme));
         throw;
      }
   }

   /// <summary>
   /// 取消主题在 avalonia 里面的 resource 资源
   /// </summary>
   /// <param name="id"></param>
   public void UnLoadTheme(string id)
   {
      if (!_themePool.ContainsKey(id)) {
         // TODO 需要记录一个日志
         return;
      }

      if (_activatedTheme != null && _activatedTheme.Id == id) {
         // TODO 需要记录一个日志
         return;
      }

      Theme theme = _themePool[id];
      theme.NotifyAboutToUnload();
      ThemeAboutToUnloadEvent?.Invoke(this, new ThemeOperateEventArgs(theme));
      // TODO 进行卸载操作，暂时没有实现
      theme.NotifyUnloaded();
      ThemeUnloadedEvent?.Invoke(this, new ThemeOperateEventArgs(theme));
   }

   public void SetActiveTheme(string id)
   {
      if (!_themePool.ContainsKey(id)) {
         // TODO 需要记录一个日志
         return;
      }

      Theme? oldTheme = _activatedTheme;
      if (oldTheme is not null) {
         oldTheme.NotifyAboutToDeActive();
      }

      Theme theme = _themePool[id];
      theme.NotifyAboutToActive();
      ThemeAboutToChangeEvent?.Invoke(this, new ThemeOperateEventArgs(oldTheme));
      _activatedTheme = theme;

      ThemeVariant themeVariant = _activatedTheme.ThemeVariant;
      Resources.ThemeDictionaries[themeVariant] = theme.ThemeResource;
      
      if (oldTheme is not null) {
         oldTheme.NotifyDeActivated();
      }
      ThemeChangedEvent?.Invoke(this, new ThemeChangedEventArgs(theme, oldTheme));
   }

   public void RegisterDynamicTheme(DynamicTheme dynamicTheme)
   {
      if (_themePool.ContainsKey(dynamicTheme.Id)) {
         // TODO 需要记录一个日志
         return;
      }
      _themePool.Add(dynamicTheme.Id, dynamicTheme);
      dynamicTheme.NotifyRegistered();
   }

   public void ScanThemes()
   {
      // 最开始的是用户指定的目录
      foreach (var path in _customThemeDirs) {
         AddThemesFromPath(path, _themePool);
      }
      
      // 优先级从高到低
      foreach (var path in _builtInThemeDirs) {
         AddThemesFromPath(path, _themePool);
      }
      
      // Assets 中的默认主题
      AddThemesFromAssets(_themePool);
      
      // TODO 如果这里为空的化需要记录一个日志
   }

   public void AddCustomThemePaths(IList<string> paths)
   {
      foreach (var path in paths) {
         var fullPath = Path.GetFullPath(path);
         if (!_customThemeDirs.Contains(fullPath) && Path.Exists(fullPath)) {
            _customThemeDirs.Add(fullPath);
         }
      }
   }

   private void AddThemesFromPath(string path, Dictionary<string, Theme> themes)
   {
      string searchPattern = "*.xml";
      if (Directory.Exists(path)) {
         string[] files = Directory.GetFiles(path, searchPattern);
         if (files.Length > 0) {
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
      foreach (var filePath in filePaths) {
         var themeId = Path.GetFileNameWithoutExtension(filePath);
         if (themes.ContainsKey(themeId)) {
            continue;
         }
         var theme = new StaticTheme(themeId, filePath);
         ThemeCreatedEvent?.Invoke(this, new ThemeOperateEventArgs(theme));
         themes.Add(themeId, theme);
         theme.NotifyRegistered();
      }
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