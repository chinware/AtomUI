namespace AtomUI.Theme;

public class ThemeLoadException : SystemException
{
    public ThemeLoadException(string? message)
        : base(message)
    {
    }

    public ThemeLoadException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

public class LanguageMetaInfoParseException : SystemException
{
    public LanguageMetaInfoParseException(string? message)
        : base(message)
    {
    }

    public LanguageMetaInfoParseException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

public class ThemeResourceRegisterException : SystemException
{
    public ThemeResourceRegisterException(string? message)
        : base(message)
    {
    }

    public ThemeResourceRegisterException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
