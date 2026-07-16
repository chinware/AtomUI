namespace AtomUI.Theme.Schema;

internal static class SchemaIdentifier
{
    internal static void Validate(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        if (value.Length > 128 || !IsStart(value[0]))
        {
            throw new ArgumentException($"'{value}' is not a valid theme schema identifier.", parameterName);
        }

        for (var index = 1; index < value.Length; index++)
        {
            if (!IsPart(value[index]))
            {
                throw new ArgumentException($"'{value}' is not a valid theme schema identifier.", parameterName);
            }
        }
    }

    private static bool IsStart(char value)
    {
        return value is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '_';
    }

    private static bool IsPart(char value)
    {
        return IsStart(value) || value is >= '0' and <= '9' or '.' or '-';
    }
}
