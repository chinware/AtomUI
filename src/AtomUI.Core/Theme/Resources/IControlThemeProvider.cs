namespace AtomUI.Theme.Resources;

public interface IControlThemeProvider
{
    object Key { get; }
    Type TargetType { get; }

    BaseControlTheme BuildControlTheme();
}