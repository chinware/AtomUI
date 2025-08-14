using System.Diagnostics;
using AtomUI.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class Skeleton : AbstractSkeleton, IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Skeleton, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> IsShowAvatarProperty =
        AvaloniaProperty.Register<Skeleton, bool>(nameof(IsShowAvatar));
    
    public static readonly StyledProperty<bool> IsShowParagraphProperty =
        AvaloniaProperty.Register<Skeleton, bool>(nameof(IsShowParagraph), true);
    
    public static readonly StyledProperty<bool> IsShowTitleProperty =
        AvaloniaProperty.Register<Skeleton, bool>(nameof(IsShowTitle), true);
    
    public static readonly StyledProperty<bool> IsRoundProperty =
        SkeletonLine.IsRoundProperty.AddOwner<Skeleton>();
    
    public static readonly StyledProperty<SkeletonWidth> TitleWidthProperty =
        AvaloniaProperty.Register<Skeleton, SkeletonWidth>(nameof(TitleWidth), new SkeletonWidth(50, SkeletonUnitType.Percentage));
    
    public static readonly StyledProperty<int> ParagraphRowsProperty =
        AvaloniaProperty.Register<Skeleton, int>(nameof(ParagraphRows), 2, validate: i => i >= 1);
    
    public static readonly StyledProperty<SkeletonWidth> ParagraphLastLineWidthProperty =
        AvaloniaProperty.Register<Skeleton, SkeletonWidth>(nameof(ParagraphLastLineWidth), new SkeletonWidth(61.0, SkeletonUnitType.Percentage));
    
    public static readonly StyledProperty<List<SkeletonWidth>?> ParagraphLineWidthsProperty =
        AvaloniaProperty.Register<Skeleton, List<SkeletonWidth>?>(nameof(ParagraphLineWidths));
    
    public static readonly StyledProperty<AvatarShape> AvatarShapeProperty =
        AvaloniaProperty.Register<Skeleton, AvatarShape>(nameof(AvatarShape), AvatarShape.Circle);
        
    public static readonly StyledProperty<AvatarSizeType> AvatarSizeTypeProperty =
        AvaloniaProperty.Register<Skeleton, AvatarSizeType>(nameof(AvatarSizeType), AvatarSizeType.Middle);
    
    public static readonly StyledProperty<double> AvatarSizeProperty =
        AvaloniaProperty.Register<Skeleton, double>(nameof(AvatarSizeType), Double.NaN);
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public bool IsShowAvatar
    {
        get => GetValue(IsShowAvatarProperty);
        set => SetValue(IsShowAvatarProperty, value);
    }

    public bool IsShowParagraph
    {
        get => GetValue(IsShowParagraphProperty);
        set => SetValue(IsShowParagraphProperty, value);
    }
    
    public bool IsShowTitle
    {
        get => GetValue(IsShowTitleProperty);
        set => SetValue(IsShowTitleProperty, value);
    }
    
    public bool IsRound
    {
        get => GetValue(IsRoundProperty);
        set => SetValue(IsRoundProperty, value);
    }
    
    public SkeletonWidth TitleWidth
    {
        get => GetValue(TitleWidthProperty);
        set => SetValue(TitleWidthProperty, value);
    }
    
    public int ParagraphRows
    {
        get => GetValue(ParagraphRowsProperty);
        set => SetValue(ParagraphRowsProperty, value);
    }
    
    public SkeletonWidth ParagraphLastLineWidth
    {
        get => GetValue(ParagraphLastLineWidthProperty);
        set => SetValue(ParagraphLastLineWidthProperty, value);
    }
    
    public List<SkeletonWidth>? ParagraphLineWidths
    {
        get => GetValue(ParagraphLineWidthsProperty);
        set => SetValue(ParagraphLineWidthsProperty, value);
    }
    
    public AvatarSizeType AvatarSizeType
    {
        get => GetValue(AvatarSizeTypeProperty);
        set => SetValue(AvatarSizeTypeProperty, value);
    }
    
    public AvatarShape AvatarShape
    {
        get => GetValue(AvatarShapeProperty);
        set => SetValue(AvatarShapeProperty, value);
    }
    
    public double AvatarSize
    {
        get => GetValue(AvatarSizeProperty);
        set => SetValue(AvatarSizeProperty, value);
    }
    #endregion

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SkeletonToken.ID;

    #endregion

    public Skeleton()
    {
        this.RegisterResources();
    }

    private SkeletonAvatar? _avatar;
    private SkeletonTitle? _title;
    private SkeletonParagraph? _paragraph;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _avatar = e.NameScope.Find<SkeletonAvatar>(SkeletonThemeConstants.AvatarPart);
        _title = e.NameScope.Find<SkeletonTitle>(SkeletonThemeConstants.TitlePart);
        _paragraph = e.NameScope.Find<SkeletonParagraph>(SkeletonThemeConstants.ParagraphPart);
        
        Debug.Assert(_avatar != null);
        Debug.Assert(_title != null);
        Debug.Assert(_paragraph != null);
        
        _avatar.Follow(this);
        _title.Follow(this);
        _paragraph.Follow(this);

        if (IsActive)
        {
            StartActiveAnimation();
        }
    }
}