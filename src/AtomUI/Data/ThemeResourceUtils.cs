using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Data;

/// <summary>
/// 在不需要绑定的情况下使用
/// </summary>
public static class ThemeResourceUtils
{
   public static object FindTokenResource(Control control, string resourceKey, ThemeVariant? themeVariant = null)
   {
      resourceKey = ProcessResourceKey(control, resourceKey);
      themeVariant ??= (control as IThemeVariantHost).ActualThemeVariant;
      if (control.TryFindResource(resourceKey, themeVariant, out var value)) {
         return value!;
      }
      
      return AvaloniaProperty.UnsetValue;
   }

   public static IObservable<object?> GetTokenResourceObservable(Control control, string resourceKey, Func<object?, object?>? converter = null)
   {
      resourceKey = ProcessResourceKey(control, resourceKey);
      return control.GetResourceObservable(resourceKey, converter);
   }

   private static string ProcessResourceKey(Control control, string resourceKey)
   {
      var tokenIdProvider = control as ITokenIdProvider;
      if (tokenIdProvider is null) {
         throw new ArgumentException($"{nameof(control)} is not ITokenIdProvider");
      }
      var tokenId = tokenIdProvider.TokenId;
      var activeTheme = ThemeManager.Current.ActivatedTheme!;
      var controlToken = activeTheme.GetControlToken(tokenId);
      if (controlToken is null) {
         throw new ArgumentException($"Control token {tokenId} for Control token id provider {nameof(control)} is not exist.");
      }
      
      if (controlToken.IsCustomTokenConfig) {
         // 自定义某些 token 值，有可能全局的 Token 也会被重定义
         if (controlToken.HasToken(resourceKey) || controlToken.CustomTokens.Contains(resourceKey)) {
            resourceKey = $"{tokenIdProvider.TokenId}.{resourceKey}";
         }
      } else {
         if (controlToken.HasToken(resourceKey)) {
            resourceKey = $"{tokenIdProvider.TokenId}.{resourceKey}";
         }
      }

      return resourceKey;
   }

   public static object? FindGlobalTokenResource(string resourceKey, ThemeVariant? themeVariant = null)
   {
      var application = Application.Current;
      if (application is null) {
         return null;
      }
      if (themeVariant is null) {
         themeVariant = (application as IThemeVariantHost).ActualThemeVariant;
      }
      if (Application.Current!.TryFindResource(resourceKey, themeVariant, out var value)) {
         return value;
      }

      return AvaloniaProperty.UnsetValue;
   }
   
   /// <summary>
   /// 直接在 resource dictionary 中查找，忽略本地覆盖的值
   /// </summary>
   /// <param name="resourceKey"></param>
   /// <param name="themeVariant"></param>
   /// <param name="converter"></param>
   /// <returns></returns>
   /// <exception cref="ApplicationException"></exception>
   public static IObservable<object?> GetGlobalTokenResourceObservable(string resourceKey, ThemeVariant? themeVariant = null, 
                                                                       Func<object?, object?>? converter = null)
   {
      var application = Application.Current;
      if (application is null) {
         throw new ApplicationException("The application instance does not exist");
      }
      if (themeVariant is null) {
         themeVariant = (application as IThemeVariantHost).ActualThemeVariant;
      }
      return application.Styles.GetResourceObservable(resourceKey, themeVariant, converter);
   }
}