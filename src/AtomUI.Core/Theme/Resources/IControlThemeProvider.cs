namespace AtomUI.Theme.Styling;

public interface IControlThemeProvider
{
    object Key { get; }
    Type TargetType { get; }

    BaseControlTheme BuildControlTheme();
}