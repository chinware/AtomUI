using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class SkeletonParagraph : AbstractSkeleton
{
    #region 公共属性定义

    public static readonly StyledProperty<SkeletonWidth> LastLineWidthProperty =
        AvaloniaProperty.Register<SkeletonParagraph, SkeletonWidth>(nameof(LastLineWidth), new SkeletonWidth(1.0, SkeletonUnitType.Percentage));
    
    public static readonly StyledProperty<List<SkeletonWidth>?> LineWidthsProperty =
        AvaloniaProperty.Register<SkeletonParagraph, List<SkeletonWidth>?>(nameof(LineWidths));
    
    public static readonly StyledProperty<int> RowsProperty =
        AvaloniaProperty.Register<SkeletonParagraph, int>(nameof(Rows), 2, validate: i => i >= 1);

    public SkeletonWidth LastLineWidth
    {
        get => GetValue(LastLineWidthProperty);
        set => SetValue(LastLineWidthProperty, value);
    }
    
    public List<SkeletonWidth>? LineWidths
    {
        get => GetValue(LineWidthsProperty);
        set => SetValue(LineWidthsProperty, value);
    }
    
    public int Rows
    {
        get => GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }
    
    #endregion
    
    private StackPanel? _linesLayout;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == RowsProperty)
            {
                BuildLines();
            }
            else if (change.Property == LastLineWidthProperty)
            {
                ConfigureLastLineWidth();
            }
            else if (change.Property == LineWidthsProperty)
            {
                ConfigureLastLineWidths();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _linesLayout = e.NameScope.Find<StackPanel>(SkeletonParagraphThemeConstants.LineLayoutPart);
        BuildLines();
        if (!IsFollowMode)
        {
            if (IsActive)
            {
                StartActiveAnimation();
            }
        }
    }

    private void BuildLines()
    {
        if (_linesLayout != null)
        {
            _linesLayout.Children.Clear();
            for (var i = 0; i < Rows; i++)
            {
                var line = new SkeletonLine();
                if (LineWidths != null && i < LineWidths.Count)
                {
                    line.LineWidth = LineWidths[i];
                }
                else if (i == Rows - 1)
                {
                    line.LineWidth = LastLineWidth;
                }
                line.Follow(this);
                _linesLayout.Children.Add(line);
            }
        }
    }

    private void ConfigureLastLineWidth()
    {
        if (_linesLayout != null && _linesLayout.Children.Count > 0)
        {
            if (_linesLayout.Children.Last() is SkeletonLine lastLine)
            {
                if (LineWidths != null && LineWidths.Count >= _linesLayout.Children.Count)
                {
                    lastLine.LineWidth = LineWidths[_linesLayout.Children.Count - 1];
                }
                else
                {
                    lastLine.LineWidth = LastLineWidth;
                }
            }
        }
    }

    private void ConfigureLastLineWidths()
    {
        if (_linesLayout != null)
        {
            for (var i = 0; i < Rows; i++)
            {
                if (_linesLayout.Children.Last() is SkeletonLine lastLine)
                {
                    if (LineWidths != null && i < LineWidths.Count)
                    {
                        lastLine.LineWidth = LineWidths[i];
                    }
                    else if (i == Rows - 1)
                    {
                        lastLine.LineWidth = LastLineWidth;
                    }
                }
            }
        }
    }
}