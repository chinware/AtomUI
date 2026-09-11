namespace AtomUI.Localization;

public interface ILocalizer
{
    string Get<TResourceKind>(TResourceKind key)
        where TResourceKind : struct, Enum;

    string Format<TResourceKind>(TResourceKind key, params object?[] arguments)
        where TResourceKind : struct, Enum;
}
