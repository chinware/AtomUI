namespace AtomUI.Theme.Styling;

public interface IControlThemeProvider
{
    public object Key { get; }
    public Type TargetType { get; }

    public BaseControlTheme BuildControlTheme();
}