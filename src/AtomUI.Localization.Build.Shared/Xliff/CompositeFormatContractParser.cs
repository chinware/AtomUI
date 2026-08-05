namespace AtomUI.Localization.Build;

internal static class CompositeFormatContractParser
{
    internal static bool TryParse(
        string value,
        out int[] placeholderIndexes,
        out string error)
    {
        var indexes = new HashSet<int>();
        for (var position = 0; position < value.Length; position++)
        {
            var character = value[position];
            if (character == '}')
            {
                if (position + 1 < value.Length && value[position + 1] == '}')
                {
                    position++;
                    continue;
                }

                return Invalid("a closing brace is not escaped", out placeholderIndexes, out error);
            }

            if (character != '{')
            {
                continue;
            }

            if (position + 1 < value.Length && value[position + 1] == '{')
            {
                position++;
                continue;
            }

            position++;
            SkipWhitespace(value, ref position);
            var indexStart = position;
            while (position < value.Length && char.IsDigit(value[position]))
            {
                position++;
            }

            if (indexStart == position ||
                !int.TryParse(value.Substring(indexStart, position - indexStart), out var index) ||
                index < 0)
            {
                return Invalid("a format item requires a non-negative numeric index", out placeholderIndexes, out error);
            }
            indexes.Add(index);

            SkipWhitespace(value, ref position);
            if (position < value.Length && value[position] == ',')
            {
                position++;
                SkipWhitespace(value, ref position);
                if (position < value.Length && (value[position] == '-' || value[position] == '+'))
                {
                    position++;
                }
                var alignmentStart = position;
                while (position < value.Length && char.IsDigit(value[position]))
                {
                    position++;
                }
                if (alignmentStart == position)
                {
                    return Invalid("a format item alignment requires an integer", out placeholderIndexes, out error);
                }
                SkipWhitespace(value, ref position);
            }

            if (position < value.Length && value[position] == ':')
            {
                position++;
                while (position < value.Length && value[position] != '}')
                {
                    if (value[position] == '{')
                    {
                        return Invalid("a format component contains an unescaped opening brace", out placeholderIndexes, out error);
                    }
                    position++;
                }
            }

            if (position >= value.Length || value[position] != '}')
            {
                return Invalid("a format item is missing its closing brace", out placeholderIndexes, out error);
            }
        }

        placeholderIndexes = indexes.OrderBy(static index => index).ToArray();
        error = string.Empty;
        return true;
    }

    private static void SkipWhitespace(string value, ref int position)
    {
        while (position < value.Length && char.IsWhiteSpace(value[position]))
        {
            position++;
        }
    }

    private static bool Invalid(
        string message,
        out int[] placeholderIndexes,
        out string error)
    {
        placeholderIndexes = Array.Empty<int>();
        error = message;
        return false;
    }
}
