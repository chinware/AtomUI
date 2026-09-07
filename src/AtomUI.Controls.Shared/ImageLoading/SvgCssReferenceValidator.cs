using System.Text;

namespace AtomUI.Controls;

internal sealed class SvgCssReferenceValidator
{
    internal void Validate(
        string css,
        Action<string> referenceHandler,
        ImageSource source)
    {
        ArgumentNullException.ThrowIfNull(css);
        ArgumentNullException.ThrowIfNull(referenceHandler);
        Scan(css.AsSpan(), referenceHandler, source, rejectFontSource: false);
    }

    private static void Scan(
        ReadOnlySpan<char> css,
        Action<string> referenceHandler,
        ImageSource source,
        bool rejectFontSource)
    {
        var index = 0;
        while (index < css.Length)
        {
            SkipWhitespaceAndComments(css, ref index, source);
            if (index >= css.Length)
            {
                return;
            }
            if (css[index] is '\'' or '"')
            {
                SkipString(css, ref index, source);
                continue;
            }
            if (css[index] == '@')
            {
                index++;
                var atRule = ReadIdentifier(css, ref index, source);
                if (atRule.Equals("import", StringComparison.OrdinalIgnoreCase))
                {
                    throw Unsafe("SVG CSS imports are not allowed.", source);
                }
                if (atRule.Equals("font-face", StringComparison.OrdinalIgnoreCase))
                {
                    SkipWhitespaceAndComments(css, ref index, source);
                    if (index >= css.Length || css[index] != '{')
                    {
                        throw Invalid("SVG CSS font-face rule is invalid.", source);
                    }
                    var block = ReadBlock(css, ref index, source);
                    Scan(block, referenceHandler, source, rejectFontSource: true);
                    continue;
                }
                continue;
            }
            if (!IsIdentifierStart(css[index]))
            {
                index++;
                continue;
            }

            var identifierStart = index;
            var identifier = ReadIdentifier(css, ref index, source);
            var afterIdentifier = index;
            SkipWhitespaceAndComments(css, ref index, source);
            if (rejectFontSource &&
                identifier.Equals("src", StringComparison.OrdinalIgnoreCase) &&
                index < css.Length && css[index] == ':')
            {
                throw Unsafe("SVG CSS external font sources are not allowed.", source);
            }
            if (!identifier.Equals("url", StringComparison.OrdinalIgnoreCase) ||
                index >= css.Length || css[index] != '(')
            {
                index = Math.Max(index, afterIdentifier);
                if (index == identifierStart)
                {
                    index++;
                }
                continue;
            }

            var url = ReadUrl(css, ref index, source);
            referenceHandler(url);
        }
    }

    private static ReadOnlySpan<char> ReadBlock(
        ReadOnlySpan<char> css,
        ref int index,
        ImageSource source)
    {
        var start = ++index;
        var depth = 1;
        while (index < css.Length)
        {
            if (css[index] is '\'' or '"')
            {
                SkipString(css, ref index, source);
                continue;
            }
            if (IsCommentStart(css, index))
            {
                SkipComment(css, ref index, source);
                continue;
            }
            if (css[index] == '{')
            {
                depth++;
            }
            else if (css[index] == '}' && --depth == 0)
            {
                var block = css[start..index];
                index++;
                return block;
            }
            index++;
        }
        throw Invalid("SVG CSS block is not closed.", source);
    }

    private static string ReadUrl(
        ReadOnlySpan<char> css,
        ref int index,
        ImageSource source)
    {
        index++;
        SkipWhitespaceAndComments(css, ref index, source);
        if (index >= css.Length)
        {
            throw Invalid("SVG CSS url() is not closed.", source);
        }

        string value;
        if (css[index] is '\'' or '"')
        {
            value = ReadString(css, ref index, source);
            SkipWhitespaceAndComments(css, ref index, source);
            if (index >= css.Length || css[index] != ')')
            {
                throw Invalid("SVG CSS url() is invalid.", source);
            }
            index++;
        }
        else
        {
            var builder = new StringBuilder();
            while (index < css.Length && css[index] != ')')
            {
                if (char.IsWhiteSpace(css[index]))
                {
                    SkipWhitespaceAndComments(css, ref index, source);
                    if (index >= css.Length || css[index] != ')')
                    {
                        throw Invalid("SVG CSS unquoted url() contains invalid whitespace.", source);
                    }
                    break;
                }
                if (IsCommentStart(css, index) || css[index] is '\'' or '"' or '(')
                {
                    throw Invalid("SVG CSS url() is invalid.", source);
                }
                AppendEscapedCharacter(css, ref index, builder, source);
            }
            if (index >= css.Length || css[index] != ')')
            {
                throw Invalid("SVG CSS url() is not closed.", source);
            }
            index++;
            value = builder.ToString();
        }

        value = value.Trim();
        if (value.Length == 0)
        {
            throw Invalid("SVG CSS url() is empty.", source);
        }
        return value;
    }

    private static string ReadIdentifier(
        ReadOnlySpan<char> css,
        ref int index,
        ImageSource source)
    {
        var builder = new StringBuilder();
        while (index < css.Length && (IsIdentifierCharacter(css[index]) || css[index] == '\\'))
        {
            AppendEscapedCharacter(css, ref index, builder, source);
        }
        return builder.ToString();
    }

    private static string ReadString(
        ReadOnlySpan<char> css,
        ref int index,
        ImageSource source)
    {
        var quote = css[index++];
        var builder = new StringBuilder();
        while (index < css.Length)
        {
            if (css[index] == quote)
            {
                index++;
                return builder.ToString();
            }
            if (css[index] is '\r' or '\n' or '\f')
            {
                throw Invalid("SVG CSS string is invalid.", source);
            }
            AppendEscapedCharacter(css, ref index, builder, source);
        }
        throw Invalid("SVG CSS string is not closed.", source);
    }

    private static void SkipString(
        ReadOnlySpan<char> css,
        ref int index,
        ImageSource source)
    {
        _ = ReadString(css, ref index, source);
    }

    private static void AppendEscapedCharacter(
        ReadOnlySpan<char> css,
        ref int index,
        StringBuilder builder,
        ImageSource source)
    {
        if (css[index] != '\\')
        {
            builder.Append(css[index++]);
            return;
        }

        index++;
        if (index >= css.Length)
        {
            throw Invalid("SVG CSS escape is incomplete.", source);
        }
        if (css[index] is '\r' or '\n' or '\f')
        {
            if (css[index] == '\r' && index + 1 < css.Length && css[index + 1] == '\n')
            {
                index++;
            }
            index++;
            return;
        }

        var hexValue = 0;
        var hexCount = 0;
        while (index < css.Length && hexCount < 6 && TryHex(css[index], out var digit))
        {
            hexValue = hexValue * 16 + digit;
            index++;
            hexCount++;
        }
        if (hexCount > 0)
        {
            if (index < css.Length && char.IsWhiteSpace(css[index]))
            {
                index++;
            }
            if (!Rune.TryCreate(hexValue, out var rune))
            {
                throw Invalid("SVG CSS escape contains an invalid code point.", source);
            }
            builder.Append(rune.ToString());
            return;
        }
        builder.Append(css[index++]);
    }

    private static void SkipWhitespaceAndComments(
        ReadOnlySpan<char> css,
        ref int index,
        ImageSource source)
    {
        while (index < css.Length)
        {
            if (char.IsWhiteSpace(css[index]))
            {
                index++;
                continue;
            }
            if (IsCommentStart(css, index))
            {
                SkipComment(css, ref index, source);
                continue;
            }
            return;
        }
    }

    private static void SkipComment(
        ReadOnlySpan<char> css,
        ref int index,
        ImageSource source)
    {
        index += 2;
        while (index + 1 < css.Length)
        {
            if (css[index] == '*' && css[index + 1] == '/')
            {
                index += 2;
                return;
            }
            index++;
        }
        throw Invalid("SVG CSS comment is not closed.", source);
    }

    private static bool IsCommentStart(ReadOnlySpan<char> css, int index) =>
        index + 1 < css.Length && css[index] == '/' && css[index + 1] == '*';

    private static bool IsIdentifierStart(char value) =>
        value == '-' || value == '_' || value == '\\' || char.IsLetter(value) || value >= 0x80;

    private static bool IsIdentifierCharacter(char value) =>
        IsIdentifierStart(value) || char.IsDigit(value);

    private static bool TryHex(char value, out int result)
    {
        if (value is >= '0' and <= '9')
        {
            result = value - '0';
            return true;
        }
        if (value is >= 'a' and <= 'f')
        {
            result = value - 'a' + 10;
            return true;
        }
        if (value is >= 'A' and <= 'F')
        {
            result = value - 'A' + 10;
            return true;
        }
        result = 0;
        return false;
    }

    private static ImageLoadFailureException Unsafe(string message, ImageSource source) =>
        ImageSourceReadHelpers.Failure(ImageLoadErrorCode.UnsafeVectorContent, message, source.DisplayName);

    private static ImageLoadFailureException Invalid(string message, ImageSource source) =>
        ImageSourceReadHelpers.Failure(ImageLoadErrorCode.InvalidImageData, message, source.DisplayName);
}
