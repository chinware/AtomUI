namespace AtomUI.Theme;

public class StaticTheme : Theme
{
    public StaticTheme(string id, string defFilePath)
        : base(id, defFilePath)
    {
    }

    public override bool IsDynamic()
    {
        return false;
    }

    internal override void NotifyLoadThemeDef()
    {
        var reader = new ThemeDefinitionReader(this);
        reader.Load(ThemeDefinition!);
    }

    internal override void NotifyResetLoadStatus()
    {
    }
}