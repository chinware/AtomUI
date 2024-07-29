using System.Reflection;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme;

/// <summary>
/// 主要是生成主题资源，绘制相关的管理不在这里，因为是公用的所以放在 ThemeManager 里面
/// </summary>
public abstract class Theme : ITheme
{
   protected bool _loaded = false;
   protected bool _loadedStatus = true;
   protected bool _darkMode = false;
   protected bool _activated = false;
   protected string _id;
   protected string? _loadErrorMsg;
   protected IThemeVariantCalculator? _themeVariantCalculator;
   protected ThemeDefinition? _themeDefinition;
   protected string _definitionFilePath;
   protected ResourceDictionary _resourceDictionary;
   protected ThemeVariant _themeVariant;
   protected AliasDesignToken _globalToken;
   protected Dictionary<string, IControlDesignToken> _controlTokens;

   public static readonly IList<string> SUPPORTED_ALGORITHMS;

   public string Id => _id;
   public string DisplayName => string.Empty;
   public bool LoadStatus => _loadedStatus;
   public string? LoadErrorMsg => _loadErrorMsg;
   public bool IsLoaded => _loaded;
   public ThemeVariant ThemeVariant => _themeVariant;
   internal ResourceDictionary ThemeResource => _resourceDictionary;
   public bool IsDarkMode => _darkMode;
   public bool IsActivated => _activated;
   public AliasDesignToken GlobalToken => _globalToken;

   static Theme()
   {
      SUPPORTED_ALGORITHMS = new List<string>
      {
         DefaultThemeVariantCalculator.ID,
         DarkThemeVariantCalculator.ID,
         CompactThemeVariantCalculator.ID,
      };
   }

   public Theme(string id, string defFilePath)
   {
      _id = id;
      _definitionFilePath = defFilePath;
      _themeVariant = ThemeVariant.Default;
      _resourceDictionary = new ResourceDictionary();
      (_resourceDictionary as IThemeVariantProvider).Key = _themeVariant;
      _globalToken = new AliasDesignToken();
      _controlTokens = new Dictionary<string, IControlDesignToken>();
   }

   public List<string> ThemeResourceKeys
   {
      get { return _resourceDictionary.Keys.Select(s => s.ToString()!).ToList(); }
   }

   public abstract bool IsDynamic();

   internal void Load()
   {
      try {
         _themeDefinition = new ThemeDefinition(_id);
         NotifyLoadThemeDef();
         var themeDef = _themeDefinition!;
         var globalTokenConfig = themeDef.GlobalTokens;
         var controlTokenConfig = themeDef.ControlTokens;
         CheckAlgorithmNames(themeDef.Algorithms);

         if (!themeDef.Algorithms.Contains(DefaultThemeVariantCalculator.ID)) {
            themeDef.Algorithms.Insert(0, DefaultThemeVariantCalculator.ID);
         } else if (themeDef.Algorithms.Contains(DefaultThemeVariantCalculator.ID) &&
                    themeDef.Algorithms[0] != DefaultThemeVariantCalculator.ID) {
            themeDef.Algorithms.Remove(DefaultThemeVariantCalculator.ID);
            themeDef.Algorithms.Insert(0, DefaultThemeVariantCalculator.ID);
         }

         if (themeDef.Algorithms.Contains(DarkThemeVariantCalculator.ID)) {
            _darkMode = true;
            _themeVariant = ThemeVariant.Dark;
         } else {
            _darkMode = false;
            _themeVariant = ThemeVariant.Light;
         }

         IThemeVariantCalculator? baseCalculator = null;
         IThemeVariantCalculator calculator = default!;
         foreach (var algorithmId in themeDef.Algorithms) {
            calculator = CreateThemeVariantCalculator(algorithmId, baseCalculator);
            baseCalculator = calculator;
         }

         _themeVariantCalculator = calculator;
         SeedDesignToken seedToken = new SeedDesignToken();
         _globalToken = new AliasDesignToken();
         seedToken.LoadConfig(globalTokenConfig);

         _themeVariantCalculator.Calculate(seedToken, _globalToken);

         // 交付最终的基础色
         seedToken.ColorBgBase = _themeVariantCalculator.ColorBgBase;
         seedToken.ColorTextBase = _themeVariantCalculator.ColorTextBase;

         _globalToken.CalculateTokenValues();

         // TODO 先用算法，然后再设置配置文件中的值，不知道合理不
         _globalToken.LoadConfig(globalTokenConfig);
         _globalToken.BuildResourceDictionary(_resourceDictionary);

         CollectControlTokens();
         foreach (var entry in _controlTokens) {
            // 如果没有修改就使用全局的
            entry.Value.AssignGlobalToken(_globalToken);
         }

         foreach (var entry in controlTokenConfig) {
            var tokenId = entry.Key;
            var controlTokenInfo = entry.Value;
            if (!_controlTokens.ContainsKey(tokenId)) {
               continue;
            }

            var controlAliasToken = (AliasDesignToken)_globalToken.Clone();
            controlAliasToken.SeedToken.LoadConfig(controlTokenInfo.ControlTokens);

            if (controlTokenInfo.UseAlgorithm) {
               _themeVariantCalculator.Calculate(controlAliasToken.SeedToken, controlAliasToken);
               controlAliasToken.CalculateTokenValues();
            }

            var controlToken = _controlTokens[controlTokenInfo.TokenId];
            controlToken.AssignGlobalToken(controlAliasToken);
            (controlToken as AbstractControlDesignToken)!.IsCustomTokenConfig = true;
            (controlToken as AbstractControlDesignToken)!.CustomTokens = controlTokenInfo.ControlTokens.Keys.ToList();
         }

         foreach (var controlToken in _controlTokens.Values) {
            (controlToken as AbstractControlDesignToken)!.CalculateFromAlias();
            if (controlTokenConfig.ContainsKey(controlToken.Id)) {
               (controlToken as AbstractControlDesignToken)!.LoadConfig(controlTokenConfig[controlToken.Id]
                                                                           .ControlTokens);
            }

            controlToken.BuildResourceDictionary(_resourceDictionary);
         }

         _loadedStatus = true;
         _loaded = true;
      } catch (Exception exception) {
         _loadErrorMsg = exception.Message;
         _loadedStatus = false;
         throw;
      }
   }

   protected void CheckAlgorithmNames(IList<string> algorithms)
   {
      foreach (var algorithm in algorithms) {
         if (!SUPPORTED_ALGORITHMS.Contains(algorithm)) {
            throw new ThemeLoadException(
               $"Algorithm: {algorithm} is not supported. Supported algorithms are: {string.Join(',', SUPPORTED_ALGORITHMS)}.");
         }
      }
   }

   protected IThemeVariantCalculator CreateThemeVariantCalculator(string algorithmId,
                                                                  IThemeVariantCalculator? baseAlgorithm)
   {
      IThemeVariantCalculator calculator = default!;
      if (algorithmId == DefaultThemeVariantCalculator.ID) {
         calculator = new DefaultThemeVariantCalculator();
      } else if (algorithmId == DarkThemeVariantCalculator.ID) {
         calculator = new DarkThemeVariantCalculator(baseAlgorithm!);
      } else if (algorithmId == CompactThemeVariantCalculator.ID) {
         calculator = new CompactThemeVariantCalculator(baseAlgorithm!);
      } else {
         throw new ThemeLoadException($"Algorithm: {algorithmId} is not supported.");
      }

      return calculator;
   }

   protected void CollectControlTokens()
   {
      _controlTokens.Clear();
      // TODO 先简单用字符串过滤
      var assemblies = Assembly.GetEntryAssembly()?.GetReferencedAssemblies().Where(assembly =>
      {
         if (assembly.Name is null) {
            return false;
         }
         return assembly.Name.StartsWith("AtomUI");
      }).Select(assemblyName => Assembly.Load(assemblyName));
      var allTypes = assemblies?.SelectMany(assembly => assembly.GetTypes());
      if (allTypes is not null) {
         var controlTokenTypes = allTypes.Where(type =>
                                                   type.IsDefined(typeof(ControlDesignTokenAttribute)) &&
                                                   typeof(AbstractControlDesignToken).IsAssignableFrom(type));
         var controlTokens = controlTokenTypes.Select(type => (AbstractControlDesignToken)Activator.CreateInstance(type)!);
         foreach (var controlToken in controlTokens) {
            _controlTokens.Add(controlToken.Id, controlToken);
         }
      }
   }

   public IControlDesignToken? GetControlToken(string tokenId)
   {
      if (_controlTokens.TryGetValue(tokenId, out var token)) {
         return token;
      }

      return null;
   }

   internal virtual void NotifyAboutToActive() { }

   internal virtual void NotifyActivated()
   {
      _activated = true;
   }

   internal virtual void NotifyAboutToDeActive() { }

   internal virtual void NotifyDeActivated()
   {
      _activated = false;
   }

   internal virtual void NotifyAboutToLoad() { }
   internal virtual void NotifyLoaded() { }
   internal virtual void NotifyAboutToUnload() { }
   internal virtual void NotifyUnloaded() { }
   internal virtual void NotifyResetLoadStatus() { }
   internal abstract void NotifyLoadThemeDef();
   internal virtual void NotifyRegistered() { }
}