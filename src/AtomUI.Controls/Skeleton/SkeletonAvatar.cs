using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

public class SkeletonAvatar : AbstractSkeleton, IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    public static readonly StyledProperty<AvatarShape> ShapeProperty =
        AvaloniaProperty.Register<SkeletonAvatar, AvatarShape>(nameof(AvatarShape), AvatarShape.Circle);
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<SkeletonAvatar>();
    
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<SkeletonAvatar, double>(nameof(Size), Double.NaN);
    
    public AvatarShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<SkeletonAvatar, bool> IsCustomSizeProperty =
        AvaloniaProperty.RegisterDirect<SkeletonAvatar, bool>(
            nameof(IsCustomSize),
            o => o.IsCustomSize,
            (o, v) => o.IsCustomSize = v);
    
    private bool _isCustomSize;

    internal bool IsCustomSize
    {
        get => _isCustomSize;
        set => SetAndRaise(IsCustomSizeProperty, ref _isCustomSize, value);
    }

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SkeletonToken.ID;

    #endregion
    
    public SkeletonAvatar()
    {
        this.RegisterResources();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizeTypeProperty || change.Property == SizeProperty)
        {
            ConfigureSize();
            IsCustomSize = !double.IsNaN(Size);
        }
        else if (change.Property == ShapeProperty)
        {
            ConfigureShape();
        }
    }
    
    private void ConfigureShape()
    {
        if (Shape == AvatarShape.Circle)
        {
            SetValue(CornerRadiusProperty, new CornerRadius(Width / 2), BindingPriority.Template);
        }
    }
    
    private void ConfigureSize()
    {
        if (!double.IsNaN(Size))
        {
            // 不影响模板设置
            SetValue(WidthProperty, Size, BindingPriority.Template);
            SetValue(HeightProperty, Size, BindingPriority.Template);
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureShape();
        ConfigureSize();
        if (!IsFollowMode)
        {
            if (IsActive)
            {
                StartActiveAnimation();
            }
        }
    }
}