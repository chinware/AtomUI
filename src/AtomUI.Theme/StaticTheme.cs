namespace AtomUI.Theme;

public class StaticTheme : Theme
{
    public StaticTheme(string id, string defFilePath)
        : base(id, defFilePath)
    {
    }

    public string DefinitionFilePath => _definitionFilePath;

    public override bool IsDynamic()
    {
        return false;
    }

    internal override void NotifyLoadThemeDef()
    {
        var reader = new ThemeDefinitionReader(this);
        reader.Load(_themeDefinition!);
    }

    internal override void NotifyResetLoadStatus()
    {
    }
}