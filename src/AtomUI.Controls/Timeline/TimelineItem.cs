using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class TimelineItem : ContentControl
{
    internal const string PendingNode = ":pending";
    internal const string ContentLeft = ":ContentLeft";
    internal const string LabelLeft = ":labelLeft";
    
    #region 公共属性定义
    
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<TimelineItem, string?>(nameof(Label));
    
    public static readonly StyledProperty<PathIcon?> DotIconProperty =
        AvaloniaProperty.Register<Alert, PathIcon?>(nameof(DotIcon));
    
    public static readonly StyledProperty<string> ColorProperty =
        AvaloniaProperty.Register<TimelineItem, string>(nameof(Color), "blue");
    
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    
    public PathIcon? DotIcon
    {
        get => GetValue(DotIconProperty);
        set => SetValue(DotIconProperty, value);
    }
    
    public string Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义
    internal static readonly StyledProperty<int> IndexProperty =
        AvaloniaProperty.Register<TimelineItem, int>(nameof(Index), 0);

    internal static readonly StyledProperty<string> ModeProperty =
        AvaloniaProperty.Register<TimelineItem, string>(nameof(Mode), "left");

    internal static readonly StyledProperty<bool> HasLabelProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(HasLabel), false);

    internal static readonly StyledProperty<bool> IsLastProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(IsLast), false);

    internal static readonly StyledProperty<bool> IsFirstProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(IsFirst), false);

    internal static readonly StyledProperty<bool> ReverseProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(Reverse), false);

    internal static readonly StyledProperty<bool> IsPendingProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(IsPending), false);
    
    internal int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    internal string Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    internal bool HasLabel
    {
        get => GetValue(HasLabelProperty);
        set => SetValue(HasLabelProperty, value);
    }

    internal bool IsFirst
    {
        get => GetValue(IsFirstProperty);
        set => SetValue(IsFirstProperty, value);
    }

    internal bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }

    internal bool Reverse
    {
        get => GetValue(ReverseProperty);
        set => SetValue(ReverseProperty, value);
    }

    internal bool IsPending
    {
        get => GetValue(IsPendingProperty);
        set => SetValue(IsPendingProperty, value);
    }
    #endregion

    protected internal int LabelIndex = 0;
    protected internal int SplitIndex = 1;
    protected internal int ContentIndex = 2;
    protected internal HorizontalAlignment LabelTextAlign = HorizontalAlignment.Right;
    protected internal HorizontalAlignment ContentTextAlign = HorizontalAlignment.Left;
    
    private DockPanel? _splitPanel;
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        CalculateIndex();
        CalculateOther();
        UpdatePseudoClasses();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _splitPanel = scope.Find<DockPanel>(TimelineItemTheme.SplitPanelPart);
        UpdatePseudoClasses();
        SetupShowInfo();
    }

    protected void UpdatePseudoClasses()
    {
        PseudoClasses.Set(PendingNode, IsPending);
        PseudoClasses.Set(ContentLeft, ContentIndex == 0);
        PseudoClasses.Set(LabelLeft, LabelIndex == 0);
    }

    private void CalculateIndex()
    {
        LabelIndex       = 0;
        SplitIndex       = 1;
        ContentIndex     = 2;
        LabelTextAlign   = HorizontalAlignment.Right;
        ContentTextAlign = HorizontalAlignment.Left;
        if (Mode == "right" || (Mode == "alternate" && Index % 2 == 1))
        {
            LabelIndex       = 2;
            ContentIndex     = 0;
            LabelTextAlign   = HorizontalAlignment.Left;
            ContentTextAlign = HorizontalAlignment.Right;
        }

        if (!HasLabel && Mode == "left")
        {
            SplitIndex       = 0;
            ContentIndex     = 1;
            ContentTextAlign = HorizontalAlignment.Left;
        }

        if (!HasLabel && Mode == "right")
        {
            SplitIndex       = 1;
            ContentIndex     = 0;
            ContentTextAlign = HorizontalAlignment.Right;
        }
    }
    
    private void CalculateOther()
    {
        
    }

    private void SetupShowInfo()
    {
        if (_splitPanel is null)
        {
            return;
        }

        var dot           = _splitPanel.Children[0];
        var border        = _splitPanel.Children[1] as Border;
        var rect          = border?.Child as Rectangle;
        var isPendingItem = false;

        if (IsLast && rect is not null)
        {
            rect.IsVisible = false;
        }
        
        if (Parent is Timeline timeline && !String.IsNullOrEmpty(timeline.Pending) && rect is not null && border is not null)
        {
            isPendingItem = Reverse && Index == 0 || !Reverse && timeline.ItemCount - 2 == Index;
            if (isPendingItem)
            {
                rect.StrokeDashArray = new AvaloniaList<double> { 0, 2 };
            }
        }

        if (IsLast || isPendingItem)
        {
            TokenResourceBinder.CreateGlobalTokenBinding(this, Layoutable.MinHeightProperty,
                TimelineTokenResourceKey.LastItemContentMinHeight);
        }
        
    }
}