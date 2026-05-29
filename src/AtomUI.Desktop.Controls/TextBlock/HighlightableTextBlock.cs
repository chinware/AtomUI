using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[Flags]
public enum TextBlockHighlightStrategy
{
    HighlightedMatch = 0x01,
    HighlightedWhole = 0x02,
    BoldedMatch = 0x04,
    HideUnMatched = 0x8,
    All = HighlightedMatch | BoldedMatch | HideUnMatched
}

public class HighlightableTextBlock : TextBlock
{
    #region 公共属性定义
    public static readonly StyledProperty<TextBlockHighlightStrategy> HighlightStrategyProperty =
        AvaloniaProperty.Register<HighlightableTextBlock, TextBlockHighlightStrategy>(nameof(HighlightStrategy), TextBlockHighlightStrategy.All);
    
    public static readonly StyledProperty<string?> HighlightWordsProperty =
        AvaloniaProperty.Register<HighlightableTextBlock, string?>(nameof(HighlightWords));
    
    public static readonly StyledProperty<IBrush?> HighlightForegroundProperty =
        AvaloniaProperty.Register<HighlightableTextBlock, IBrush?>(nameof(HighlightForeground));
    
    public static readonly StyledProperty<IBrush?> HighlightBackgroundProperty =
        AvaloniaProperty.Register<HighlightableTextBlock, IBrush?>(nameof(HighlightBackground));
    
    public TextBlockHighlightStrategy HighlightStrategy
    {
        get => GetValue(HighlightStrategyProperty);
        set => SetValue(HighlightStrategyProperty, value);
    }
    
    public string? HighlightWords
    {
        get => GetValue(HighlightWordsProperty);
        set => SetValue(HighlightWordsProperty, value);
    }
    
    public IBrush? HighlightForeground
    {
        get => GetValue(HighlightForegroundProperty);
        set => SetValue(HighlightForegroundProperty, value);
    }
    
    public IBrush? HighlightBackground
    {
        get => GetValue(HighlightBackgroundProperty);
        set => SetValue(HighlightBackgroundProperty, value);
    }
    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HighlightStrategyProperty ||
            change.Property == HighlightWordsProperty ||
            change.Property == HighlightBackgroundProperty ||
            change.Property == HighlightForegroundProperty ||
            change.Property == TextProperty)
        {
            NotifyBuildFilterHighlightRuns();
        }
    }

    protected virtual void NotifyBuildFilterHighlightRuns()
    {
        if (string.IsNullOrEmpty(HighlightWords))
        {
            Inlines = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        Debug.Assert(Text != null);

        var strategy        = HighlightStrategy;
        var highlightMatch  = strategy.HasFlag(TextBlockHighlightStrategy.HighlightedMatch);
        var highlightWhole  = !highlightMatch && strategy.HasFlag(TextBlockHighlightStrategy.HighlightedWhole);
        var bold            = strategy.HasFlag(TextBlockHighlightStrategy.BoldedMatch);

        // 注意:HideUnMatched flag 在原实现里就未读取,本轮保留该行为不变(SKILL Tier 1 §1)。
        if (highlightWhole)
        {
            var single = new InlineCollection { CreateRun(Text, isHighlighted: true, bold) };
            Inlines = single;
            return;
        }

        var firstHighlightIndex = Text.IndexOf(HighlightWords, StringComparison.OrdinalIgnoreCase);
        if (firstHighlightIndex == -1)
        {
            var plainRuns = new InlineCollection();
            plainRuns.Add(CreateRun(Text, isHighlighted: false, bold: false));
            Inlines = plainRuns;
            return;
        }

        var highlightRanges = CalculateHighlightRanges(HighlightWords, Text, firstHighlightIndex);
        var runs            = new InlineCollection();

        var cursor = 0;
        foreach (var (start, end) in highlightRanges)
        {
            if (start > cursor)
            {
                runs.Add(CreateRun(Text.Substring(cursor, start - cursor), isHighlighted: false, bold: false));
            }
            runs.Add(CreateRun(Text.Substring(start, end - start), isHighlighted: highlightMatch, bold: highlightMatch && bold));
            cursor = end;
        }
        if (cursor < Text.Length)
        {
            runs.Add(CreateRun(Text.Substring(cursor), isHighlighted: false, bold: false));
        }

        Inlines = runs;
    }

    private Run CreateRun(string segment, bool isHighlighted, bool bold)
    {
        var run = new Run(segment);
        if (isHighlighted)
        {
            run.Foreground = HighlightForeground;
            run.Background = HighlightBackground;
            if (bold)
            {
                run.FontWeight = FontWeight.Bold;
            }
        }
        return run;
    }
    
    protected bool IsNeedHighlight(int pos, in List<(int, int)> ranges)
    {
        foreach (var range in ranges)
        {
            if (pos >= range.Item1 && pos < range.Item2)
            {
                return true;
            }
        }

        return false;
    }
    
    protected List<(int, int)> CalculateHighlightRanges(string highlightWords, string text, int firstIndex = 0)
    {
        var ranges          = new List<(int, int)>();
        var highlightLength = highlightWords.Length;
        var foundIndex      = firstIndex;
        
        while (true)
        {
            if (foundIndex == -1) // 如果没有找到，退出循环
            {
                break;
            }
                
            var currentIndex = foundIndex + highlightLength;
            ranges.Add((foundIndex, currentIndex));
            foundIndex = text.IndexOf(highlightWords, currentIndex, StringComparison.OrdinalIgnoreCase);
        }
        return ranges;
    }
}
