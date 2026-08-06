using AtomUI.Generated.Module;
using AtomUI;

namespace Acme.LocalizationComponent;

public static class ModuleLanguageExtensions
{
    public static IAtomUIBuilder UseLocalizationComponent(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        GeneratedLanguageModuleRegistration.Register(builder.Localization);
        return builder;
    }
}
