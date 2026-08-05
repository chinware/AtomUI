using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Localization;

public abstract class LanguageResourceExtension<TResourceKind> : MarkupExtension
    where TResourceKind : struct, Enum
{
    protected LanguageResourceExtension()
    {
    }

    protected LanguageResourceExtension(TResourceKind kind)
    {
        Kind = kind;
    }

    public TResourceKind? Kind { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Kind is not { } kind)
        {
            throw new InvalidOperationException("A language resource key is required.");
        }

        var provideTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (!ShouldUseStaticResourceValue(provideTarget?.TargetObject))
        {
            return new DynamicResourceExtension(kind);
        }

        var application = Application.Current ?? throw new InvalidOperationException(
            "A current Application is required to resolve a static language resource.");
        if (application.TryGetResource(kind, application.ActualThemeVariant, out var value))
        {
            return value ?? string.Empty;
        }

        throw new InvalidOperationException(
            $"Language resource '{typeof(TResourceKind).FullName}.{kind}' is not registered.");
    }

    private static bool ShouldUseStaticResourceValue(object? targetObject)
    {
        return targetObject is not null &&
               targetObject is not AvaloniaObject &&
               targetObject is not SetterBase;
    }
}
