namespace AtomUI.Localization;

[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public sealed class LanguageCatalogAttribute : Attribute
{
    public int ContractVersion { get; set; } = 1;
}
