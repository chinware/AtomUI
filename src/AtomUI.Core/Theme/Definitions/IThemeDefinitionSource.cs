namespace AtomUI.Theme.Definitions;

public interface IThemeDefinitionSource
{
    string SourceIdentity { get; }
    string SourceRevision { get; }

    Stream OpenRead();
}
