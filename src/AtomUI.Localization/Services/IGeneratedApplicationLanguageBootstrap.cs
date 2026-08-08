using System.ComponentModel;

namespace AtomUI.Localization;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IGeneratedApplicationLanguageBootstrap
{
    void RegisterApplicationLanguages(ILocalizationBuilder builder);
}
