using AtomUI.Data;

namespace AtomUI.Toolkits.GalleryBase.Localization;

public interface IGalleryLocalizedText
{
    object Resolve();
}

public sealed class GalleryLocalizedText<TResourceKind> : IGalleryLocalizedText
    where TResourceKind : Enum
{
    public TResourceKind ResourceKind { get; }

    public string Fallback { get; }

    public GalleryLocalizedText(TResourceKind resourceKind, string fallback)
    {
        ResourceKind = resourceKind;
        Fallback     = fallback;
    }

    public object Resolve()
    {
        try
        {
            return LanguageResourceBinder.GetLangResource(ResourceKind) ?? Fallback;
        }
        catch (ApplicationException)
        {
            return Fallback;
        }
    }

    public override string ToString()
    {
        return Resolve().ToString() ?? string.Empty;
    }
}
