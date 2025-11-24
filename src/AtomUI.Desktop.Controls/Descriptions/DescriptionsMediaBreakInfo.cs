namespace AtomUI.Desktop.Controls;

public record DescriptionsMediaBreakInfo
{
    public int ExtraSmall { get; init; }
    public int Small { get; init; }
    public int Medium { get; init; }
    public int Large { get; init; }
    public int ExtraLarge { get; init; }
    public int ExtraExtraLarge { get; init; }

    public DescriptionsMediaBreakInfo()
        : this(1)
    {
    }
    
    public DescriptionsMediaBreakInfo(int column)
    {
        ExtraSmall      = column;
        Small           = column;
        Medium          = column;
        Large           = column;
        ExtraLarge      = column;
        ExtraExtraLarge = column;
    }

    public DescriptionsMediaBreakInfo(int extraSmall, int small, int medium, int large, int extraLarge, int extraExtraLarge)
    {
        ExtraSmall      = extraSmall;
        Small           = small;
        Medium          = medium;
        Large           = large;
        ExtraLarge      = extraLarge;
        ExtraExtraLarge = extraExtraLarge;
    }
    
    public static DescriptionsMediaBreakInfo Parse(string input)
    {
        if (int.TryParse(input.Trim(), out int singleColumn))
        {
            if (singleColumn <= 0)
            {
                throw new ArgumentException("The number of columns must be greater than 0", nameof(input));
            }
                
            return new DescriptionsMediaBreakInfo(singleColumn);
        }
        
        return ParseKeyValueFormat(input);
    }

    private static DescriptionsMediaBreakInfo ParseKeyValueFormat(string input)
    {
        var result       = new DescriptionsMediaBreakInfo();
        var span         = input.AsSpan();
        int segmentIndex = 0;
        
        while (!span.IsEmpty)
        {
            segmentIndex++;
            int                commaIndex = span.IndexOf(',');
            ReadOnlySpan<char> segment    = commaIndex >= 0 ? span[..commaIndex] : span;
            
            ProcessSegmentWithSwitch(segment, segmentIndex, ref result);
            
            span = commaIndex >= 0 ? span[(commaIndex + 1)..] : ReadOnlySpan<char>.Empty;
        }

        return result;
    }

    private static void ProcessSegmentWithSwitch(ReadOnlySpan<char> segment, int segmentIndex, ref DescriptionsMediaBreakInfo result)
    {
        int colonIndex = segment.IndexOf(':');
        if (colonIndex < 0)
        {
            throw new FormatException($"Segment {segmentIndex}: Missing colon separator '{segment.ToString()}'");
        }

        var breakpoint = segment[..colonIndex].Trim();
        var valueSpan  = segment[(colonIndex + 1)..].Trim();
        
        if (breakpoint.IsEmpty)
        {
            throw new FormatException($"Segment {segmentIndex}: Breakpoint name is empty.");
        }
        
        if (valueSpan.IsEmpty)
        {
            throw new FormatException($"The breakpoint '{breakpoint.ToString()}' at segment {segmentIndex} is null.");
        }
        
        if (!int.TryParse(valueSpan, out int value))
        {
            throw new FormatException($"The value of breakpoint '{breakpoint.ToString()}' is not a valid integer.");
        }
        
        if (value <= 0)
        {
            throw new FormatException($"The value of the breakpoint '{breakpoint.ToString()}' must be greater than 0, and its current value is {value}.");
        }

        if (breakpoint.Equals("xs", StringComparison.OrdinalIgnoreCase))
        {
            result = result with { ExtraSmall = value };
        }
        else if (breakpoint.Equals("sm", StringComparison.OrdinalIgnoreCase))
        {
            result = result with { Small = value };
        }
        else if (breakpoint.Equals("md", StringComparison.OrdinalIgnoreCase))
        {
            result = result with { Medium = value };
        }
        else if (breakpoint.Equals("lg", StringComparison.OrdinalIgnoreCase))
        {
            result = result with { Large = value };
        }
        else if (breakpoint.Equals("xl", StringComparison.OrdinalIgnoreCase))
        {
            result = result with { ExtraLarge = value };
        }
        else if (breakpoint.Equals("xxl", StringComparison.OrdinalIgnoreCase))
        {
            result = result with { ExtraExtraLarge = value };
        }
        else
        {
            throw new FormatException($"`{segmentIndex}`: An unknown breakpoint name '{breakpoint.ToString()}', supporting breakpoints are: xs, sm, md, lg, xl, xxl");
        }
    }
}