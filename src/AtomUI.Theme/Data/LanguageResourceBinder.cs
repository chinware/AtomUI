using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;

namespace AtomUI.Theme.Data;

public static class LanguageResourceBinder
{
   public static IDisposable CreateBinding(AvaloniaObject target,
                                           AvaloniaProperty targetProperty,
                                           LanguageResourceKey resourceKey,
                                           BindingPriority priority = BindingPriority.Template,
                                           Func<object?, object?>? converter = null)
   {
      var application = Application.Current;
      if (application is null) {
         throw new ApplicationException("The application instance does not exist");
      }

      var themeVariant = (application as IThemeVariantHost).ActualThemeVariant;
      var observable = application.Styles.GetResourceObservable(resourceKey, themeVariant, converter);

      return target.Bind(targetProperty, observable, priority);
   }

   public static string? GetLangResource(LanguageResourceKey resourceKey, ThemeVariant? themeVariant = null)
   {
      var application = Application.Current;
      if (application is null) {
         throw new ApplicationException("The application instance does not exist");
      } 
      themeVariant ??= (application as IThemeVariantHost).ActualThemeVariant;
      if (application.Styles.TryGetResource(resourceKey, themeVariant, out var value)) {
         return value as string;
      }

      return null;
   }
}