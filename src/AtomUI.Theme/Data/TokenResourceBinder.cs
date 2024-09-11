using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Utils;

public static class TokenResourceBinder
{
    public static IDisposable CreateTokenBinding(AvaloniaObject target,
                                                 AvaloniaProperty targetProperty,
                                                 TokenResourceKey resourceKey)
    {
        return target.Bind(targetProperty, new DynamicResourceExtension(resourceKey.Value));
    }

    public static IDisposable CreateTokenBinding(AvaloniaObject target,
                                                 AvaloniaProperty targetProperty,
                                                 string resourceKey)
    {
        return target.Bind(targetProperty, new DynamicResourceExtension(resourceKey));
    }

    public static IDisposable CreateTokenBinding(Control target,
                                                 AvaloniaProperty targetProperty,
                                                 TokenResourceKey resourceKey,
                                                 BindingPriority priority = BindingPriority.Template,
                                                 Func<object?, object?>? converter = null)
    {
        return target.Bind(targetProperty, target.GetResourceObservable(resourceKey, converter), priority);
    }

    public static IDisposable CreateTokenBinding(Control target,
                                                 AvaloniaProperty targetProperty,
                                                 object resourceKey,
                                                 BindingPriority priority = BindingPriority.Template,
                                                 Func<object?, object?>? converter = null)
    {
        return target.Bind(targetProperty, target.GetResourceObservable(resourceKey, converter), priority);
    }

    public static IDisposable CreateGlobalTokenBinding(AvaloniaObject target,
                                                       AvaloniaProperty targetProperty,
                                                       TokenResourceKey resourceKey,
                                                       BindingPriority priority = BindingPriority.Template,
                                                       Func<object?, object?>? converter = null)
    {
        return target.Bind(targetProperty, GetGlobalTokenResourceObservable(resourceKey, null, converter), priority);
    }

    public static IDisposable CreateGlobalResourceBinding(AvaloniaObject target,
                                                          AvaloniaProperty targetProperty,
                                                          object resourceKey,
                                                          BindingPriority priority = BindingPriority.Template,
                                                          Func<object?, object?>? converter = null)
    {
        return target.Bind(targetProperty, GetGlobalResourceObservable(resourceKey, null, converter), priority);
    }

    /// <summary>
    /// 直接在 resource dictionary 中查找，忽略本地覆盖的值
    /// </summary>
    public static IObservable<object?> GetGlobalTokenResourceObservable(TokenResourceKey resourceKey,
                                                                        ThemeVariant? themeVariant = null,
                                                                        Func<object?, object?>? converter = null)
    {
        return GetGlobalResourceObservable(resourceKey, themeVariant, converter);
    }

    public static IObservable<object?> GetGlobalResourceObservable(object resourceKey,
                                                                   ThemeVariant? themeVariant = null,
                                                                   Func<object?, object?>? converter = null)
    {
        var application = Application.Current;
        if (application is null)
        {
            throw new ApplicationException("The application instance does not exist");
        }

        themeVariant ??= (application as IThemeVariantHost).ActualThemeVariant;
        return application.Styles.GetResourceObservable(resourceKey, themeVariant, converter);
    }
}