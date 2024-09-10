namespace AtomUI.Generator.Language;

public class LanguageInfo
{
    public LanguageInfo()
    {
        Items = new Dictionary<string, string>();
    }

    public string Namespace { get; internal set; } = string.Empty;
    public string ResourceCatalog { get; internal set; } = string.Empty;
    public string LanguageId { get; internal set; } = string.Empty;
    public string LanguageCode { get; internal set; } = string.Empty;
    public string ClassName { get; internal set; } = string.Empty;

    public Dictionary<string, string> Items { get; internal set; }
}