using AtomUI.IconPkg;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public class TimelineItem : ContentControl
{
    internal const string PendingNodePC = ":pending";
    internal const string ContentLeftPC = ":ContentLeft";
    internal const string LabelLeftPC = ":labelLeft";

    #region 公共属性定义

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<TimelineItem, string?>(nameof(Label));

    public static readonly StyledProperty<Icon?> DotIconProperty =
        AvaloniaProperty.Register<Alert, Icon?>(nameof(DotIcon));

    public static readonly StyledProperty<string> ColorProperty =
        AvaloniaProperty.Register<TimelineItem, string>(nameof(Color), "blue");

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public Icon? DotIcon
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
    
    internal static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<TimelineItem, int>(nameof(Count), 0);

    internal static readonly StyledProperty<TimeLineMode> ModeProperty =
        AvaloniaProperty.Register<TimelineItem, TimeLineMode>(nameof(Mode), TimeLineMode.Left);

    internal static readonly StyledProperty<bool> HasLabelProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(HasLabel), false);

    internal static readonly StyledProperty<bool> IsLastProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(IsLast), false);

    internal static readonly StyledProperty<bool> ReverseProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(Reverse), false);

    internal static readonly StyledProperty<bool> IsPendingProperty =
        AvaloniaProperty.Register<TimelineItem, bool>(nameof(IsPending), false);

    internal static readonly StyledProperty<HorizontalAlignment> ContentTextAlignProperty =
        AvaloniaProperty.Register<TimelineItem, HorizontalAlignment>(nameof(ContentTextAlign),
            HorizontalAlignment.Left);

    internal static readonly StyledProperty<HorizontalAlignment> LabelTextAlignProperty =
        AvaloniaProperty.Register<TimelineItem, HorizontalAlignment>(nameof(LabelTextAlign), HorizontalAlignment.Left);

    internal int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }
    
    internal int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    internal TimeLineMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    internal bool HasLabel
    {
        get => GetValue(HasLabelProperty);
        set => SetValue(HasLabelProperty, value);
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

    internal HorizontalAlignment ContentTextAlign
    {
        get => GetValue(ContentTextAlignProperty);
        set => SetValue(ContentTextAlignProperty, value);
    }

    internal HorizontalAlignment LabelTextAlign
    {
        get => GetValue(LabelTextAlignProperty);
        set => SetValue(LabelTextAlignProperty, value);
    }

    #endregion
    
    protected internal int LabelIndex = 0;
    protected internal int SplitIndex = 1;
    protected internal int ContentIndex = 2;

    private Grid? _gridContainer;
    private DockPanel? _splitPanel;
    private TextBlock? _labelBlock;
    private ContentPresenter? _itemsContentPresenter;
    private Border? _splitHeadPart;
    private Icon? _dotPart;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ModeProperty || change.Property == CountProperty || change.Property == IndexProperty)
        {
            UpdateAll();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _gridContainer         = scope.Find<Grid>(TimelineItemTheme.GridPart);
        _splitPanel            = scope.Find<DockPanel>(TimelineItemTheme.SplitPanelPart);
        _labelBlock            = scope.Find<TextBlock>(TimelineItemTheme.LabelPart);
        _itemsContentPresenter = scope.Find<ContentPresenter>(TimelineItemTheme.ItemsContentPresenterPart);
        _splitHeadPart         = scope.Find<Border>(TimelineItemTheme.SplitHeadPart);
        _dotPart               = scope.Find<Icon>(TimelineItemTheme.DotPart);
        
        UpdateAll();
    }

    protected void UpdateAll()
    {
        CalculateIndex();
        SetupShowInfo();
        UpdatePseudoClasses();
    }

    protected void UpdatePseudoClasses()
    {
        PseudoClasses.Set(PendingNodePC, IsPending);
        PseudoClasses.Set(ContentLeftPC, ContentIndex == 0);
        PseudoClasses.Set(LabelLeftPC, LabelIndex == 0);
    }

    private void CalculateIndex()
    {
        if (VisualRoot is null || _gridContainer is null)
        {
            return;
        }

        if (Parent is Timeline timeline)
        {
            Index    = timeline.Items.IndexOf(this);
            IsLast   = Index == timeline.Items.Count - 1;
            HasLabel = false;
            
            foreach (var child in timeline.Items)
            {
                if (child is TimelineItem item)
                {
                    if (!string.IsNullOrEmpty(item.Label))
                    {
                        HasLabel = true;
                        break;
                    }
                }
            }
            
        }

        SplitIndex       = 1;
        LabelTextAlign   = HorizontalAlignment.Right;
        ContentTextAlign = HorizontalAlignment.Left;
        if (Mode == TimeLineMode.Right || (Mode == TimeLineMode.Alternate && Index % 2 == 1))
        {
            LabelIndex       = 2;
            ContentIndex     = 0;
            LabelTextAlign   = HorizontalAlignment.Left;
            ContentTextAlign = HorizontalAlignment.Right;
        }
        else
        {
            LabelIndex       = 0;
            ContentIndex     = 2;
            LabelTextAlign   = HorizontalAlignment.Right;
            ContentTextAlign = HorizontalAlignment.Left;
        }

        _gridContainer.ColumnDefinitions[0].Width = GridLength.Star;
        _gridContainer.ColumnDefinitions[2].Width = GridLength.Star;

        if (!HasLabel)
        {
            if (Mode == TimeLineMode.Left)
            {
                LabelIndex       = 0;
                ContentIndex     = 2;
                ContentTextAlign = HorizontalAlignment.Left;
            }

            if (Mode == TimeLineMode.Right)
            {
                LabelIndex       = 2;
                ContentIndex     = 0;
                ContentTextAlign = HorizontalAlignment.Right;
            }

            _gridContainer.ColumnDefinitions[LabelIndex].Width = new GridLength(0);
        }
    }
    
    private void SetupShowInfo()
    {
        if (_splitPanel is null || _itemsContentPresenter is null || _labelBlock is null || _gridContainer is null)
        {
            return;
        }

        Grid.SetColumn(_labelBlock, LabelIndex);
        Grid.SetColumn(_itemsContentPresenter, ContentIndex);
        Grid.SetColumn(_splitPanel, SplitIndex);
        
        var dot           = _splitPanel.Children[0];
        var border        = _splitPanel.Children[1] as Border;
        var rect          = border?.Child as Rectangle;
        var isPendingItem = false;
        MinHeight            = 0;
        
        if (Parent is Timeline timeline && !String.IsNullOrEmpty(timeline.Pending) && rect is not null &&
            border is not null)
        {
            rect.StrokeDashArray = null;
            
            isPendingItem = Reverse && Index == 0 || !Reverse && timeline.ItemCount - 2 == Index;
            if (isPendingItem)
            {
                rect.StrokeDashArray = new AvaloniaList<double> { 0, 2 };
            }
        }
        
        if (rect is not null)
        {
            rect.IsVisible = !IsLast;
        }

        if (IsLast || isPendingItem)
        {
            TokenResourceBinder.CreateGlobalTokenBinding(this, Layoutable.MinHeightProperty,
                TimelineTokenResourceKey.LastItemContentMinHeight);
        }

        if (Color.StartsWith("#"))
        {
            try
            {
                var color = Avalonia.Media.Color.Parse(Color);
                var brush = new SolidColorBrush(color);
                if (_dotPart is not null && _dotPart.NormalFilledBrush is null)
                {
                    _dotPart.NormalFilledBrush = brush;
                }

                if (_splitHeadPart is not null)
                {
                    _splitHeadPart.BorderBrush = brush;
                }
            }
            catch (Exception)
            {
                if (_dotPart is not null && _dotPart.NormalFilledBrush is null)
                {
                    TokenResourceBinder.CreateGlobalTokenBinding(_dotPart, Icon.NormalFilledBrushProperty,
                        DesignTokenKey.ColorPrimary);
                }

                if (_splitHeadPart is not null)
                {
                    TokenResourceBinder.CreateGlobalTokenBinding(_splitHeadPart, Border.BorderBrushProperty,
                        DesignTokenKey.ColorPrimary);
                }
            }
        }
        else
        {
            var tokenText = DesignTokenKey.ColorSuccess;
            switch (Color)
            {
                case "blue":
                    tokenText = DesignTokenKey.ColorPrimary;
                    break;
                case "green":
                    tokenText = DesignTokenKey.ColorSuccess;
                    break;
                case "red":
                    tokenText = DesignTokenKey.ColorError;
                    break;
                case "gray":
                    tokenText = DesignTokenKey.ColorTextDisabled;
                    break;
            }
            if (_dotPart is not null && _dotPart.NormalFilledBrush is null)
            {
                TokenResourceBinder.CreateGlobalTokenBinding(_dotPart, Icon.NormalFilledBrushProperty,
                    tokenText);
            }

            if (_splitHeadPart is not null)
            {
                TokenResourceBinder.CreateGlobalTokenBinding(_splitHeadPart, Border.BorderBrushProperty,
                    tokenText);
            }
        }
    }
}