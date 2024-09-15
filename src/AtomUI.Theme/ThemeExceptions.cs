namespace AtomUI.Theme;

public class ThemeDefinitionParserException : SystemException
{
    public ThemeDefinitionParserException(string? message)
        : base(message)
    {
    }

    public ThemeDefinitionParserException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

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

public class TokenResourceRegisterException : SystemException
{
    public TokenResourceRegisterException(string? message)
        : base(message)
    {
    }

    public TokenResourceRegisterException(string? message, Exception? innerException)
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