namespace AtomUI.Generator.Language;

internal class LanguageInfo
{
    public string Namespace { get; internal set; } = string.Empty;
    public string ResourceCatalog { get; internal set; } = string.Empty;
    public string LanguageId { get; internal set; } = string.Empty;
    public string LanguageCode { get; internal set; } = string.Empty;
    public string ClassName { get; internal set; } = string.Empty;

    public Dictionary<string, string> Items { get; internal set; }

    public LanguageInfo()
    {
        Items = new Dictionary<string, string>();
    }
}