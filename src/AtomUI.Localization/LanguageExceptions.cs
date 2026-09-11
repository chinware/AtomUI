namespace AtomUI.Localization;

public class LanguageConfigurationException : InvalidOperationException
{
    public LanguageConfigurationException(string message)
        : base(message)
    {
    }

    public LanguageConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class LanguageCatalogException : InvalidOperationException
{
    public LanguageCatalogException(string message)
        : base(message)
    {
    }

    public LanguageCatalogException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class LanguageCoverageException : InvalidOperationException
{
    public LanguageCoverageException(string message)
        : base(message)
    {
    }

    public LanguageCoverageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class LanguageNotSupportedException : ArgumentException
{
    public LanguageNotSupportedException(LanguageTag language)
        : base(CreateMessage(language), nameof(language))
    {
        Language = language;
    }

    public LanguageTag Language { get; }

    private static string CreateMessage(LanguageTag language)
    {
        if (language == default)
        {
            throw new ArgumentException("A valid language tag is required.", nameof(language));
        }

        return $"Language '{language.Value}' is not supported by this application.";
    }
}
