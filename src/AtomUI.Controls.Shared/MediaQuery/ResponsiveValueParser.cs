namespace AtomUI.Controls;

internal static class ResponsiveValueParser
{
    internal delegate T SpanValueParser<T>(ReadOnlySpan<char> input);

    internal static ResponsiveValueMap<T> Parse<T>(
        string input,
        SpanValueParser<T> parseScalar,
        SpanValueParser<T> parseBreakpointValue)
    {
        var trimmed = input.AsSpan().Trim();
        if (trimmed.IsEmpty)
        {
            throw new FormatException("Responsive value cannot be empty.");
        }

        if (trimmed.IndexOf(':') < 0)
        {
            return new ResponsiveValueMap<T>(parseScalar(trimmed));
        }

        return ParseKeyValueFormat(trimmed, parseBreakpointValue);
    }

    private static ResponsiveValueMap<T> ParseKeyValueFormat<T>(
        ReadOnlySpan<char> input,
        SpanValueParser<T> parseBreakpointValue)
    {
        var values = new Dictionary<MediaBreakPoint, T>();
        var span = input;
        var segmentIndex = 0;

        while (!span.IsEmpty)
        {
            segmentIndex++;
            var commaIndex = span.IndexOf(',');
            var segment = commaIndex >= 0 ? span[..commaIndex].Trim() : span.Trim();
            span = commaIndex >= 0 ? span[(commaIndex + 1)..] : ReadOnlySpan<char>.Empty;

            if (segment.IsEmpty)
            {
                throw new FormatException($"Segment {segmentIndex}: Responsive segment is empty.");
            }

            var colonIndex = segment.IndexOf(':');
            if (colonIndex < 0)
            {
                throw new FormatException($"Segment {segmentIndex}: Missing colon separator '{segment.ToString()}'.");
            }

            var key = segment[..colonIndex].Trim();
            var valueSpan = segment[(colonIndex + 1)..].Trim();

            if (key.IsEmpty)
            {
                throw new FormatException($"Segment {segmentIndex}: Breakpoint name is empty.");
            }
            if (valueSpan.IsEmpty)
            {
                throw new FormatException($"The breakpoint '{key.ToString()}' at segment {segmentIndex} is null.");
            }
            if (!ResponsiveBreakpoints.TryParse(key, out var breakPoint))
            {
                throw new FormatException(
                    $"`{segmentIndex}`: Unknown breakpoint '{key.ToString()}', supported: xs, sm, md, lg, xl, xxl, xxxl.");
            }
            if (values.ContainsKey(breakPoint))
            {
                throw new FormatException($"Breakpoint '{key.ToString()}' is duplicated.");
            }

            values.Add(breakPoint, parseBreakpointValue(valueSpan));
        }

        return new ResponsiveValueMap<T>(values);
    }
}
